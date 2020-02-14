using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    [Serializable]
    private struct Sound
    {
        public SoundType Type;
        public AudioClip Clip;
    }

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<Sound> _sounds = new List<Sound>();

    public enum SoundType
    {
        CommandRecognized,
        ContentCompleted,
        ContentStarted,
        OnHoverBegin,
        OnHoverEnd,
    }


    public void Play(SoundType type)
    {
        _audioSource.clip = _sounds.First(s => s.Type == type).Clip;
        _audioSource.Play();
    }

}