using System;
using System.Linq;
using HoloToolkit.Unity.InputModule;
using TMPro;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechInput : MonoBehaviour {
    private KeywordRecognizer keywordRecognizer = null;

    [SerializeField] private TMP_Text _debug;
    [SerializeField] private string[] _keywords;
    private void Start()
    {
        keywordRecognizer = new KeywordRecognizer(_keywords.ToArray());
        keywordRecognizer.OnPhraseRecognized += onKeywordRecognized;
        keywordRecognizer.Start();
        
    }

    private void onKeywordRecognized(PhraseRecognizedEventArgs args)
    {
        _debug.text = args.text;
        Debug.Log(args.text);
    }
}