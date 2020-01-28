using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentStep : MonoBehaviour
{
    public VisibleObject[] _visibleObjects;
    public AudioClip _audioClip;

    public void SetActive(bool active)
    {
        foreach (var visibleObject in _visibleObjects)
        {
            visibleObject.SetVisible(active);
        }
    }
}
