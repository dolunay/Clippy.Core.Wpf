using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Wpf.Clippy;
using Wpf.Clippy.Types;

namespace Clippy.Wpf.Demo.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ClippyCharacter m_character;
        private Point m_characterPosition = new Point(100, 100);
        private SpeechLanguage m_selectedSpeechLanguage;

        public ObservableCollection<SpeechLanguage> SpeechLanguages { get; }
            = new ObservableCollection<SpeechLanguage>();

        public SpeechLanguage SelectedSpeechLanguage
        {
            get => m_selectedSpeechLanguage;
            set
            {
                if (SetField(ref m_selectedSpeechLanguage, value))
                {
                    if (m_character != null)
                    {
                        m_character.SpeechLanguage = value;
                    }

                    OnPropertyChanged(nameof(SpeechVoiceNotice));
                    OnPropertyChanged(nameof(SpeechSampleText));
                    OnPropertyChanged(nameof(CanSpeakSample));
                }
            }
        }

        public string SpeechVoiceNotice => SpeechLanguages.Count == 0
            ? "Desteklenen diller için Windows konuşma sesi bulunamadı. Windows Ayarları > Erişilebilirlik > Anlatıcı > Anlatıcı sesi bölümünden ek sesler yükleyin."
            : $"{SpeechLanguages.Count} desteklenen dil için Windows konuşma sesi bulundu. Listeden bir dil seçebilirsiniz.";

        public bool HasSpeechLanguages => SpeechLanguages.Count > 0;
        public bool CanSpeakSample => SelectedSpeechLanguage != null;

        public string SpeechSampleText => SelectedSpeechLanguage?.CultureName switch
        {
            "tr-TR" => "Merhaba! Ben Clippy. Size nasıl yardımcı olabilirim?",
            "en-US" => "Hello! I'm Clippy. How can I help you?",
            "de-DE" => "Hallo! Ich bin Clippy. Wie kann ich Ihnen helfen?",
            "fr-FR" => "Bonjour ! Je suis Clippy. Comment puis-je vous aider ?",
            "es-ES" => "¡Hola! Soy Clippy. ¿Cómo puedo ayudarte?",
            "it-IT" => "Ciao! Sono Clippy. Come posso aiutarti?",
            "pt-BR" => "Olá! Eu sou o Clippy. Como posso ajudar?",
            "ru-RU" => "Здравствуйте! Я Клиппи. Чем я могу помочь?",
            "ja-JP" => "こんにちは！クリッピーです。どのようにお手伝いできますか？",
            "zh-CN" => "你好！我是 Clippy。有什么可以帮你？",
            _ => "Hello! I'm Clippy. How can I help you?"
        };

        public ObservableCollection<string> Animations { get; }
            = new ObservableCollection<string>();

        public string SelectedAnimation
        {
            get => m_character.GetActiveAnimation(AnimationMode.Loop);
            set
            {
                if (value != m_character.GetActiveAnimation(AnimationMode.Loop))
                {
                    m_character.PlayAnimation(value, AnimationMode.Loop);
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Character> Characters { get; } 
            = new ObservableCollection<Character>();

        public Character SelectedCharacter
        {
            get => m_character.CharacterType;
            set
            {
                if (value != m_character?.CharacterType)
                {
                    RecreateCharacter(value);
                    OnPropertyChanged();
                }
            }
        }

        public ICommand HideCharacter { get; }
        public ICommand ShowCharacter { get; }
        public ICommand SpeakSample { get; }

        public MainWindowViewModel()
        {
            HideCharacter = new DelegateCommand<object>(_ =>
            {
                m_character?.Hide();
            });

            ShowCharacter = new DelegateCommand<object>(_ =>
            {
                m_character?.Show();
            });

            SpeakSample = new DelegateCommand<object>(_ =>
            {
                m_character?.Say(SpeechSampleText, TimeSpan.FromSeconds(5));
            });

            foreach (Character character in Enum.GetValues(typeof(Character)))
            {
                Characters.Add(character);
            }
            SelectedCharacter = Character.Clippy;
        }

        private void OnCharacterDoubleClicked(ClippyCharacter character)
        {
            if (!character.PlayAnimation("Wave", AnimationMode.Once))
            {
                character.PlayAnimation("Pleased", AnimationMode.Once);
            }

            character.Say(SpeechSampleText, TimeSpan.FromSeconds(5));
        }

        private void OnCharacterLocationChanged(ClippyCharacter character, Point location)
        {
            m_characterPosition = location;
        }

        private void OnCharacterAnimationComplete(ClippyCharacter sender, string animationName, AnimationMode mode)
        {
            if (animationName == "Show")
            {
                Application.Current.Dispatcher.InvokeAsync(AskQuestion);
            }
        }

        private void RecreateCharacter(Character character)
        {
            if (m_character != null)
            {
                m_character.OnDoubleClick -= OnCharacterDoubleClicked;
                m_character.OnLocationChanged -= OnCharacterLocationChanged;
                m_character.OnAnimationCompleted -= OnCharacterAnimationComplete;
                m_character.Close();
            }

            m_character = new ClippyCharacter(character);
            m_character.OnDoubleClick += OnCharacterDoubleClicked;
            m_character.OnLocationChanged += OnCharacterLocationChanged;
            m_character.OnAnimationCompleted += OnCharacterAnimationComplete;

            SpeechLanguages.Clear();
            foreach (var language in m_character.InstalledSpeechLanguages)
            {
                SpeechLanguages.Add(language);
            }

            SelectedSpeechLanguage = m_character.SpeechLanguage;
            OnPropertyChanged(nameof(SpeechVoiceNotice));
            OnPropertyChanged(nameof(SpeechSampleText));
            OnPropertyChanged(nameof(HasSpeechLanguages));

            m_character.Show();
            m_character.Location = m_characterPosition;
            
            Animations.Clear();
            foreach (var animationName in m_character.AnimationNames.OrderBy(x => x))
            {
                Animations.Add(animationName);
            }
            SelectedAnimation = Animations.FirstOrDefault(x => x.ToLower().StartsWith("idle"))
                                ?? Animations.FirstOrDefault();
        }

        private void AskQuestion()
        {
            var dismissCommand = new DelegateCommand<string>(message =>
            {
                m_character.Say(message, TimeSpan.FromSeconds(3));
            });

            m_character.Say(new StackPanel
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = $"Hello! My name is {m_character.CharacterType}, how are you today?"
                    },
                    new UniformGrid
                    {
                        Columns = 2,
                        Children =
                        {
                            new Button
                            {
                                Content = "I'm good thanks",
                                Command = dismissCommand,
                                CommandParameter = "That's great to hear!"
                            },
                            new Button
                            {
                                Content = "Not great",
                                Command = dismissCommand,
                                CommandParameter = "I'm sorry to hear that."
                            }
                        }
                    }
                }
            });
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
