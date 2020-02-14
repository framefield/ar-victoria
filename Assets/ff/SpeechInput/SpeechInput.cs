using System;
using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Unity.InputModule;
using TMPro;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechInput : MonoBehaviour
{
    [SerializeField] private TMP_Text _debug;

    public enum Command
    {
        Alpha,
        Bravo,
        Charlie,
        AdminAbort,
        Calibrate,
    }

    private Dictionary<string, Command> _textsForCommands = new Dictionary<string, Command>()
    {
        {"alpha", Command.Alpha}, //start unguided tour
        {"bravo", Command.Bravo}, //start guided tour
        {"charlie", Command.Charlie}, //start mixed initiative tour
        {"admin abort", Command.AdminAbort},
        {"calibrate", Command.Calibrate},
    };

    public void Init(ICommandListener listener, SoundFX soundFx)
    {
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
            FireCommand(Command.AdminAbort);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            FireCommand(Command.Calibrate);
    }

    private void OnKeywordRecognized(PhraseRecognizedEventArgs args)
    {
        FireCommand(_textsForCommands[args.text]);
        _debug.text = args.text;
        Debug.Log(args.text);
    }

    private void FireCommand(Command c)
    {
        _soundFX.Play(SoundFX.SoundType.CommandRecognized);
        _listener.OnCommandDetected(c);
    }
    
    private KeywordRecognizer _keywordRecognizer = null;

    public interface ICommandListener
    {
        void OnCommandDetected(Command command);
    }

    private ICommandListener _listener;
    private SoundFX _soundFX;
}