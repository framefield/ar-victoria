using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace victoria
{
    /// <summary>
    /// Hold the available speech input commands and fires events when they are recognized.
    /// </summary>
    public class SpeechInput : MonoBehaviour
    {
        public enum Command
        {
            Alpha,
            Bravo,
            Charlie,
            CancelTour,
            Admin
        }

        private Dictionary<string, Command> _textsForCommands = new Dictionary<string, Command>()
        {
            {"start alpha", Command.Alpha}, //start unguided tour
            {"start bravo", Command.Bravo}, //start guided tour
            {"start charlie", Command.Charlie}, //start mixed initiative tour
            {"admin mode", Command.Admin},
            {"cancel tour", Command.CancelTour}
        };

        public void Initialize(ICommandListener listener, SoundFX soundFx, NotificationUI notificationUi)
        {
            _notificationUI = notificationUi;
            _listener = listener;
            _soundFX = soundFx;
        }

        private void Start()
        {
            _keywordRecognizer = new KeywordRecognizer(_textsForCommands.Keys.ToArray());
            _keywordRecognizer.OnPhraseRecognized += OnKeywordRecognized;
            _keywordRecognizer.Start();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                FireCommand(Command.Alpha);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                FireCommand(Command.Bravo);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                FireCommand(Command.Charlie);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                FireCommand(Command.CancelTour);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                FireCommand(Command.Admin);
        }

        private void OnKeywordRecognized(PhraseRecognizedEventArgs args)
        {
            FireCommand(_textsForCommands[args.text]);
            _notificationUI.ShowNotifiation($"Command Recognized: {args.text}", 2f);
            ;
            Debug.Log(args.text);
        }

        private void FireCommand(Command c)
        {
            _soundFX.Play(SoundFX.SoundType.CommandRecognized);
            _listener.OnCommandDetected(c);
        }

        public interface ICommandListener
        {
            void OnCommandDetected(Command command);
        }

        private KeywordRecognizer _keywordRecognizer = null;
        private ICommandListener _listener;
        private SoundFX _soundFX;
        private NotificationUI _notificationUI;
    }
}