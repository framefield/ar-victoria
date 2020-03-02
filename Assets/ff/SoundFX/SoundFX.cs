using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace victoria
{
    /// <summary>
    /// Holds the data on sound fx and plays them.
    /// </summary>
    public class SoundFX : MonoBehaviour
    {
        [Serializable]
        private class Sound
        {
            public SoundType Type = default;
            public AudioClip Clip = null;
        }

        [SerializeField] private AudioSource _audioSource=null;
        [SerializeField] private List<Sound> _sounds = new List<Sound>();

        public enum SoundType
        {
            CommandRecognized,
            ContentCompleted,
            ContentStarted,
            OnDwellTimerBegin,
            OnDwellTimerCanceled,
        }
        
        public void Play(SoundType type)
        {
            _audioSource.clip = _sounds.First(s => s.Type == type).Clip;
            _audioSource.Play();
        }
    }
}