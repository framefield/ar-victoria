using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace victoria.tour
{
    /// <summary>
    /// The seven content modules in the scene, that are assigned to a segment of the statue. 
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayableContent : MonoBehaviour
    {
        [SerializeField] public InteractiveSegment.SegmentType Type;
        [SerializeField] private AudioClip _audioClip = null;

        public void Init(IInteractionListener listener)
        {
            _interactionListener = listener;
            SetVisible(false);
        }

        public void Play()
        {
            SetVisible(true);
            var animator = GetComponent<Animator>();
            var controller = new TimedAnimationController(animator, "visible");

            StartCoroutine(PlayClipRoutine(controller, () =>
            {
                SetVisible(false);
                _interactionListener.ContentCompleted(this);
            }));
        }

        private void SetVisible(bool visible)
        {
            gameObject.name = Type.ToString();
            if (visible)
                gameObject.name += " >>";
        }


        IEnumerator PlayClipRoutine(TimedAnimationController animator, Action onCompleteCallback)
        {
            var timePosition = 0f;
            while (timePosition <= animator.ClipLength)
            {
                timePosition += Time.deltaTime;
                animator.SetTimePosition(timePosition);
                yield return new WaitForFixedUpdate();
            }

            onCompleteCallback.Invoke();
        }

        public interface IInteractionListener
        {
            void ContentCompleted(PlayableContent completedContent); //animation completed or "exit" interaction
        }

        private ParticleSystem _highlightParticles;
        private IInteractionListener _interactionListener;
    }
}