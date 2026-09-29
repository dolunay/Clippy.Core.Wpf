using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Clippy.Types;

namespace Wpf.Clippy.ViewModels
{
    public class ClippyViewModel : INotifyPropertyChanged
    {
        private enum State
        {
            Hidden,
            Showing,
            Active,
            Hiding
        }

        private static readonly (string CultureName, string DisplayName)[] SupportedSpeechLanguages =
        {
            ("tr-TR", "Türkçe (Türkiye)"),
            ("en-US", "English (United States)"),
            ("de-DE", "Deutsch (Deutschland)"),
            ("fr-FR", "Français (France)"),
            ("es-ES", "Español (España)"),
            ("it-IT", "Italiano (Italia)"),
            ("pt-BR", "Português (Brasil)"),
            ("ru-RU", "Русский (Россия)"),
            ("ja-JP", "日本語 (日本)"),
            ("zh-CN", "中文 (中国)"),
        };

        [SupportedOSPlatform("windows")]
        private static IReadOnlyList<SpeechLanguage> GetInstalledSpeechLanguagesCore()
        {
            using var synthesizer = new SpeechSynthesizer();
            var installedVoices = synthesizer.GetInstalledVoices()
                .Where(voice => voice.Enabled)
                .Select(voice => voice.VoiceInfo)
                .ToLookup(voice => voice.Culture.Name, StringComparer.OrdinalIgnoreCase);

            return SupportedSpeechLanguages
                .Select(language =>
                {
                    var voice = installedVoices[language.CultureName].FirstOrDefault();
                    return voice == null
                        ? null
                        : new SpeechLanguage(language.CultureName, language.DisplayName, voice.Name);
                })
                .Where(language => language != null)
                .ToArray();
        }

        internal static IReadOnlyList<SpeechLanguage> GetInstalledSpeechLanguages()
        {
            return OperatingSystem.IsWindows()
                ? GetInstalledSpeechLanguagesCore()
                : Array.Empty<SpeechLanguage>();
        }

        private readonly CharacterData m_data;
        private readonly CancellationTokenSource m_cancellationTokenSource;

        private CharacterData.CharacterAnimation m_activeAnimation;
        private string m_loopingAnimation;
        private string m_playOnceAnimation;
        private int m_frameIndex;
        private Point m_frameCoords;
        private Rect m_frameRect;
        private readonly object m_animationLock = new object();

        private State m_state = State.Hidden;
        private Visibility m_canvasVisibility = Visibility.Hidden;
        private Action m_onHideComplete;

        private Popup m_speechPopup;
        private ClippyMessage m_activeMessage;
        private SpeechSynthesizer m_speechSynthesizer;
        private bool m_isSpeechEnabled = true;
        private string m_speechCultureName;
        private string m_speechVoiceName;

        public IReadOnlyList<SpeechLanguage> InstalledSpeechLanguages { get; }

        public bool IsSpeechEnabled
        {
            get => m_isSpeechEnabled;
            set
            {
                m_isSpeechEnabled = value;
                if (!value && OperatingSystem.IsWindows())
                {
                    StopSpeech();
                }
            }
        }

        public delegate void ClippyViewModelAnimationCompletedEventHandler(ClippyViewModel sender, string animationName, AnimationMode mode);
        public event ClippyViewModelAnimationCompletedEventHandler OnAnimationCompleted;

        public IReadOnlyCollection<string> AnimationNames => m_data.Animations.Keys;

        public string GetActiveAnimation(AnimationMode mode)
        {
            if (mode == AnimationMode.Once)
            {
                return m_playOnceAnimation;
            }
            return m_loopingAnimation;
        }

        public ClippyMessage ActiveMessage
        {
            get => m_activeMessage;
            private set
            {
                if (SetField(ref m_activeMessage, value))
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        m_speechPopup.IsOpen = true;
                    });
                }
            }
        }

        public ImageSource ImageMap { get; }

        public Rect FrameRect
        {
            get => m_frameRect;
            set => SetField(ref m_frameRect, value);
        }

        public Point FrameCoords
        {
            get => m_frameCoords;
            set
            {
                if (SetField(ref m_frameCoords, value))
                {
                    FrameRect = new Rect(
                        -m_frameCoords.X, -m_frameCoords.Y,
                        FrameRect.Width, FrameRect.Height
                        );
                }
            }
        }

        public Visibility CanvasVisibility
        {
            get => m_canvasVisibility;
            private set => SetField(ref m_canvasVisibility, value);
        }

        public ClippyViewModel(Character character)
        {
            InstalledSpeechLanguages = GetInstalledSpeechLanguages();
            var defaultLanguage = InstalledSpeechLanguages.FirstOrDefault(language =>
                string.Equals(language.CultureName, CultureInfo.CurrentUICulture.Name, StringComparison.OrdinalIgnoreCase))
                ?? InstalledSpeechLanguages.FirstOrDefault(language =>
                    string.Equals(language.CultureName, "tr-TR", StringComparison.OrdinalIgnoreCase))
                ?? InstalledSpeechLanguages.FirstOrDefault(language =>
                    string.Equals(language.CultureName, "en-US", StringComparison.OrdinalIgnoreCase))
                ?? InstalledSpeechLanguages.FirstOrDefault();

            SpeechLanguage = defaultLanguage;

            m_data = LoadCharacterData(character);
            ImageMap = LoadCharacterImageMap(character);

            FrameRect = new Rect(
                0, 0,
                m_data.FrameSize[0], m_data.FrameSize[1]
            );

            m_cancellationTokenSource = new CancellationTokenSource();
            Task.Run(UpdateAsync);
        }

        public SpeechLanguage SpeechLanguage
        {
            get => InstalledSpeechLanguages.FirstOrDefault(language =>
                string.Equals(language.CultureName, m_speechCultureName, StringComparison.OrdinalIgnoreCase));
            set
            {
                m_speechCultureName = value?.CultureName;
                m_speechVoiceName = value?.VoiceName;
                if (m_speechSynthesizer != null)
                {
                    StopSpeech();
                    ConfigureSpeechVoice(m_speechSynthesizer);
                }
            }
        }

        internal void Close()
        {
            m_cancellationTokenSource.Cancel();
            if (OperatingSystem.IsWindows())
            {
                StopSpeech();
                m_speechSynthesizer?.Dispose();
                m_speechSynthesizer = null;
            }
        }

        internal void Show()
        {
            m_state = State.Showing;
            CanvasVisibility = Visibility.Visible;

            if (AnimationNames.Contains("Show"))
            {
                PlayAnimation("Show", AnimationMode.Once);
                return;
            }

            m_state = State.Active;
            var idle = AnimationNames.FirstOrDefault(x => x.ToLower().Contains("idle")) ??
                       AnimationNames.FirstOrDefault();
            PlayAnimation(idle, AnimationMode.Loop);
        }

        internal void Hide(Action onHidden)
        {
            StopSpeech();
            m_onHideComplete = onHidden;
            m_speechPopup.IsOpen = false;
            m_state = State.Hiding;

            if (AnimationNames.Contains("Hide"))
            {
                PlayAnimation("Hide", AnimationMode.Once);
                return;
            }

            CanvasVisibility = Visibility.Hidden;
            m_onHideComplete?.Invoke();
        }

        internal void SetSpeechPopup(Popup speechPopup)
        {
            m_speechPopup = speechPopup;
        }

        internal bool PlayAnimation(string animationName, AnimationMode mode)
        {
            if (animationName == null)
            {
                return false;
            }

            if (!m_data.Animations.ContainsKey(animationName))
            {
                return false;
            }

            void SetFrameZero()
            {
                if (m_activeAnimation == null)
                {
                    return;
                }

                var frame = m_activeAnimation.Frames[0];
                if (frame.Images == null)
                {
                    return;
                }

                var images = frame.Images[0];
                FrameCoords = new Point(-images[0], -images[1]);
            }

            lock (m_animationLock)
            {
                if (mode == AnimationMode.Once)
                {
                    m_playOnceAnimation = animationName;
                    m_frameIndex = 0;
                    m_data.Animations.TryGetValue(animationName, out m_activeAnimation);
                    SetFrameZero();
                    return true;
                }

                m_loopingAnimation = animationName;
                if (m_playOnceAnimation == null)
                {
                    m_frameIndex = 0;
                    m_data.Animations.TryGetValue(animationName, out m_activeAnimation);
                    SetFrameZero();
                }
            }

            return true;
        }

        internal void Say(string message, TimeSpan dismissAfter)
        {
            ActiveMessage = new ClippySpeechMessage(message, dismissAfter);

            if (IsSpeechEnabled && !string.IsNullOrWhiteSpace(message) && OperatingSystem.IsWindows())
            {
                if (m_speechSynthesizer == null)
                {
                    m_speechSynthesizer = new SpeechSynthesizer();
                    ConfigureSpeechVoice(m_speechSynthesizer);
                }

                StopSpeech();
                m_speechSynthesizer.SpeakAsync(message);
            }
        }

        internal void Say(FrameworkElement content, TimeSpan? dismissAfter)
        {
            StopSpeech();
            ActiveMessage = new ClippyCustomMessage(content, dismissAfter);
        }

        private void StopSpeech()
        {
            if (OperatingSystem.IsWindows())
            {
                m_speechSynthesizer?.SpeakAsyncCancelAll();
            }
        }

        private void ConfigureSpeechVoice(SpeechSynthesizer synthesizer)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            ConfigureSpeechVoiceOnWindows(synthesizer);
        }

        [SupportedOSPlatform("windows")]
        private void ConfigureSpeechVoiceOnWindows(SpeechSynthesizer synthesizer)
        {
            if (!string.IsNullOrEmpty(m_speechVoiceName))
            {
                synthesizer.SelectVoice(m_speechVoiceName);
            }
        }

        private async Task UpdateAsync()
        {
            var token = m_cancellationTokenSource.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (m_activeMessage != null)
                    {
                        if (m_activeMessage.ShouldDismiss)
                        {
                            m_activeMessage = null;
                            StopSpeech();

                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                m_speechPopup.IsOpen = false;
                            });
                        }
                    }

                    CharacterData.AnimationFrame frame = null;
                    var frameCount = 0;
                    lock (m_animationLock)
                    {
                        if (m_activeAnimation != null 
                            && m_state != State.Hidden)
                        {
                            var frames = m_activeAnimation.Frames;
                            frameCount = frames.Length;
                            frame = frames[m_frameIndex++];
                        }
                    }

                    if (frame == null)
                    {
                        CanvasVisibility = Visibility.Hidden;
                        await Task.Delay(TimeSpan.FromMilliseconds(20), token)
                            .ConfigureAwait(false);
                        continue;
                    }

                    if (frame.Images != null)
                    {
                        CanvasVisibility = Visibility.Visible;

                        var images = frame.Images[0];
                        FrameCoords = new Point(-images[0], -images[1]);
                    }
                    else
                    {
                        CanvasVisibility = Visibility.Hidden;
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(frame.Duration), token)
                        .ConfigureAwait(false);

                    if (m_frameIndex >= frameCount)
                    {
                        OnAnimationCompleted?.Invoke(this,
                            m_playOnceAnimation ?? m_loopingAnimation,
                            m_playOnceAnimation != null ? AnimationMode.Once : AnimationMode.Loop);

                        lock (m_animationLock)
                        {
                            m_frameIndex = 0;
                        }

                        if (m_state == State.Showing)
                        {
                            m_state = State.Active;
                            m_playOnceAnimation = null;
                            PlayAnimation(m_loopingAnimation, AnimationMode.Loop);
                        }
                        else if (m_state == State.Hiding)
                        {
                            m_state = State.Hidden;
                            CanvasVisibility = Visibility.Hidden;
                            m_playOnceAnimation = null;
                            m_onHideComplete?.Invoke();
                        }
                        else if (m_playOnceAnimation != null)
                        {
                            m_playOnceAnimation = null;
                            PlayAnimation(m_loopingAnimation, AnimationMode.Loop);
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Ignored
            }
        }

        private ImageSource LoadCharacterImageMap(Character character)
        {
            var uri = new Uri($"pack://application:,,,/Wpf.Clippy;Component/Resources/{character}/map.png", UriKind.Absolute);
            return BitmapFrame.Create(uri,
                BitmapCreateOptions.None,
                BitmapCacheOption.OnLoad);
        }

        private CharacterData LoadCharacterData(Character character)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"Wpf.Clippy.Resources.{character}.data.json"))
            {
                if (stream == null)
                {
                    throw new Exception($"Failed to find resource named 'Wpf.Clippy.Resources.{character}.data.json'");
                }

                var data = JsonSerializer.Deserialize<CharacterData>(stream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return data;
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
