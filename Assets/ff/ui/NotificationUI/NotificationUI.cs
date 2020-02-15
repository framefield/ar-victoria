using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;
    [SerializeField] private bool _showDebugNotifications;

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