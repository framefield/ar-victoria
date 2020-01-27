using HoloToolkit.Unity;
using UnityEngine;

public class TTS : MonoBehaviour {
    private TextToSpeech textToSpeech;
    // Use this for initialization
    // void Start () {
    //    var soundManager = GameObject.Find("AudioManager");
    //  TextToSpeech textToSpeech = soundManager.GetComponent<TextToSpeech>();
    //textToSpeech.Voice = TextToSpeechVoice.Zira;
    //textToSpeech.StartSpeaking("Welcome to the Holographic App ! You can use Gaze, Gesture and Voice Command to interact with it!");
    //Debug.Log("Start Audio");
    //}

    private void Awake()

    {

        textToSpeech = GetComponent<TextToSpeech>();
        if (textToSpeech != null) { 
        var msg = string.Format("Willkommen bei Museum vier punkt null. Nehmen Sie sich Zeit anzukommen und den Ort zu explorieren. Unsere Ausstellung ist ein Experiment mit neuen Technologien, wie diese Hololens. Welcome to the Holographic App ! You can use Gaze, Gesture and Voice Command to interact with it!", textToSpeech.Voice.ToString());
        textToSpeech.StartSpeaking(msg); }
      
        Debug.Log("Start Audio");
    }

    



}
	
	
