using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace victoria.tour
{
    /// <summary>
    /// The seven content modules in the scene, that are assigned to a segment of the statue. 
    /// </summary>
    public class PlayableContent : MonoBehaviour
    {
        [FormerlySerializedAs("_interactiveComponent")] [SerializeField]
        public InteractiveSegment.SegmentType Type;

        [SerializeField] private AudioClip _audioClip = null;
        [SerializeField] private VisibleObject[] _visibleObjects = null;

        public void Init(IInteractionListener listener)
        {
            _interactionListener = listener;
        }

        public void Play()
        {
            SetVisible(true);
            StartCoroutine(ExampleCoroutine());
        }

        private void SetVisible(bool visible)
        {
            foreach (var visibleObject in _visibleObjects)
            {
                visibleObject.SetVisible(visible);
            }

            gameObject.name = Type.ToString();
            if (visible)
                gameObject.name += " >>";
        }


        IEnumerator ExampleCoroutine()
        {
            Debug.Log("Started Coroutine at timestamp : " + Time.time);

            yield return new WaitForSeconds(5);

            Debug.Log("Finished Coroutine at timestamp : " + Time.time);
            SetVisible(false);
            _interactionListener.ContentCompleted(this);
        }

        public interface IInteractionListener
        {
            void ContentCompleted(PlayableContent completedContent); //animation completed or "exit" interaction
        }

        private ParticleSystem _highlightParticles;
        private IInteractionListener _interactionListener;
    }
}