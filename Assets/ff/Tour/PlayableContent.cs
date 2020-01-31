using System;
using UnityEngine;

namespace victoria.tour
{
    /// <summary>
    /// The seven content modules in the scene, that are assigned to a segment of the statue. 
    /// </summary>
    public class PlayableContent : MonoBehaviour, InteractiveComponent.IInteractionListener
    {
        public enum State
        {
            Inactive,
            Hovered,
            Active
        }

        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private VisibleObject[] _visibleObjects;


        void InteractiveComponent.IInteractionListener.Hover()
        {
            _interactionListener.Hover();
        }

        void InteractiveComponent.IInteractionListener.Unhover()
        {
            _interactionListener.Unhover();
        }
        
        public void Init(ParticleSystem highlightParticles, IInteractionListener listener)
        {
            _interactionListener = listener;
            _highlightParticles = highlightParticles;
            GetComponent<InteractiveComponent>().Init(this);
        }

        public void SetState(State state)
        {
            foreach (var visibleObject in _visibleObjects)
            {
                visibleObject.SetVisible(state == State.Active);
            }
        }

        public interface IInteractionListener
        {
            void Hover();
            void Unhover();
            void ContentCompleted(); //animation completed or "exit" interaction
        }

        private ParticleSystem _highlightParticles;
        private IInteractionListener _interactionListener;
    }
}
