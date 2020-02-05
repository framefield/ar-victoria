using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour {
    private KeywordRecognizer keywordRecognizer = null;

	// Use this for initialization
	void Start () {
	}

    public void ChangeKeyWords(String[] keywords)
    {
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
        if (keywords.Length != 0)
        {
            keywordRecognizer = new KeywordRecognizer(keywords.ToArray());
            keywordRecognizer.OnPhraseRecognized += onKeywordRecognized;
            keywordRecognizer.Start();
        }
    }

    public void ClearKeywords()
    {
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
        keywordRecognizer = null;
    }

    private void onKeywordRecognized(PhraseRecognizedEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            focused.OnKeywordRecognized(args.text);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
