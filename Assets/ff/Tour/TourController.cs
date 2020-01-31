using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace victoria.tour
{
    /// <summary>
    /// Holds state of the interaction with the <see cref="PlayableContent"/>. Triggers play on them.
    /// </summary>
    public class TourController : MonoBehaviour, PlayableContent.IInteractionListener
    {
        [Header("Internal References")]
        [SerializeField] private PlayableContent[] _content;
        [Header("External Prefabs")]
        [SerializeField] private ParticleSystem _hightlightParticles;
        public struct State
        {
            private PlayableContent playableContentInFocus;
            private PlayableContent.State _state;
        }

        private void Start()
        {
            foreach (var c in _content)
            {
                c.Init(_hightlightParticles, this);
            }
        }

        void PlayableContent.IInteractionListener.Hover()
        {
        }

        void PlayableContent.IInteractionListener.Unhover()
        {
        }

        void PlayableContent.IInteractionListener.ContentCompleted()
        {
        }
    }
}