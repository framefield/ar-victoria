using TMPro;
using UnityEngine;

namespace victoria
{
    /// <summary>
    /// Simple lower third notification ui. Used for debugging as well.
    /// </summary>
    public class NotificationUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label = null;
        [SerializeField] private bool _showDebugNotifications = default;

        public void ShowNotifiation(string text, float durationInSeconds = 2f)
        {
            _timeLeftToShowNotification = durationInSeconds;
            _label.text = text;
        }

        public void ShowDebugNotification(string text, float durationInSeconds = 2f)
        {
            if (_showDebugNotifications)
                ShowNotifiation(text, durationInSeconds);
        }

        private void Update()
        {
            if (_timeLeftToShowNotification < 0)
                return;

            _timeLeftToShowNotification -= Time.deltaTime;
            _label.gameObject.SetActive(_timeLeftToShowNotification >= 0);
        }

        private float _timeLeftToShowNotification;
    }
}