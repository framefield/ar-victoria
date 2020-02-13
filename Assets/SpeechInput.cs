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
        {"alpha", Command.Alpha},
        {"bravo", Command.Bravo},
        {"charlie", Command.Charlie},
        {"admin abort", Command.AdminAbort},
        {"calibrate", Command.Calibrate},
    };

    public void Init(ICommandListener listener)
    {
        _listener = listener;
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
            _listener.OnCommandDetected(Command.Alpha);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            _listener.OnCommandDetected(Command.Bravo);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            _listener.OnCommandDetected(Command.Charlie);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            _listener.OnCommandDetected(Command.AdminAbort);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            _listener.OnCommandDetected(Command.Calibrate);
    }

    private void OnKeywordRecognized(PhraseRecognizedEventArgs args)
    {
        _listener.OnCommandDetected(_textsForCommands[args.text]);
        _debug.text = args.text;
        Debug.Log(args.text);
    }
    private KeywordRecognizer _keywordRecognizer = null;

    public interface ICommandListener
    {
        void OnCommandDetected(Command command);
    }

    private ICommandListener _listener;
}