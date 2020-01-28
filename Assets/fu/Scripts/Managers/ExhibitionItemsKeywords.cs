
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests {

    [RequireComponent(typeof(Renderer))]
    public class ExhibitionItemsKeywords : MonoBehaviour, ISpeechHandler {

        private bool _isHidden = false;

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            ChangeMode(eventData.RecognizedText);
            throw new System.NotImplementedException();
        }

        public void ChangeMode(string mode)
        {
            switch (mode.ToLower())
            {
                case "start exhibition":
                    IsHidden = true;
                    break;
                case "start config":
                    IsHidden = false;
                    break;
            }
        }


        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                if (value == true)
                {
                    gameObject.layer = 15;
                }
                else
                {
                    gameObject.layer = 0;
                }
                _isHidden = value;
            }
        }

    }
}

