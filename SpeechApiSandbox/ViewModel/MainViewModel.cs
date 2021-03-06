using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Speech.Synthesis;
using System.Speech.Recognition;

namespace SpeechApiSandbox.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            Log = new ObservableCollection<string>();
            Recognizers = SpeechEngineManager.Recognizers;
            SelectedRecognizer = Recognizers.Any() ? Recognizers.First() : null;

            Start = new RelayCommand(() =>
                {
                    if (listener != null && listener.Recognizer != SelectedRecognizer)
                    {
                        Stop.Execute(null);
                    }

                    listener = new CommandListener(SelectedRecognizer);
                    listener.LogEvent += listener_LogEvent;
                    listener.Start();

                    Log.Add("Started speech command listener " + SelectedRecognizer.Id);
                });

            Stop = new RelayCommand(() =>
                {
                    Log.Add("Stopping speech command listener " + listener.Recognizer.Id);

                    if (listener != null && listener.Recognizer != SelectedRecognizer)
                    {
                        listener.LogEvent -= listener_LogEvent;

                        listener.Dispose();
                        listener = null;
                    }
                    else
                    {
                        listener.Stop();
                    }
                });

            Speak = new RelayCommand(() =>
                {
                    SpeechSynthesizer synth = new SpeechSynthesizer();
                    synth.SetOutputToDefaultAudioDevice();
                    synth.Speak("Lorem ipsum dolor sit am.");
                });
        }

        void listener_LogEvent(object sender, LogEventArgs e)
        {
            Log.Add(e.Message);
        }

        private CommandListener listener;

        public RelayCommand Start { get; private set; }
        public RelayCommand Stop { get; private set; }
        public RelayCommand Speak { get; private set; }

        public ObservableCollection<string> Log { get; set; }
        public ReadOnlyCollection<RecognizerInfo> Recognizers { get; set; }
        public RecognizerInfo SelectedRecognizer { get; set; }
    }
}