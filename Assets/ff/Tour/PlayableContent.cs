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

        private AudioSource _audioSource;

        public void Init(IInteractionListener listener, AudioSource audioSource)
        {
            _audioSource = audioSource;
            _interactionListener = listener;
            RenderState(false);
            GetComponent<Animator>().StopPlayback();
        }

        public void Play()
        {
            _completedAnimation = false;
            _completedAudio = false;

            RenderState(true);

            var animator = GetComponent<Animator>();
            var controller = new TimedAnimationController(animator, "visible");
            StartCoroutine(PlayClipRoutine(controller, () =>
            {
                _completedAnimation = true;
                TryToCompleteContent();
            }));

            _audioSource.clip = _audioClip;
            StartCoroutine(PlayAudioRoutine(_audioSource, () =>
            {
                _completedAudio = true;
                TryToCompleteContent();
            }));
        }

        private bool _completedAudio;
        private bool _completedAnimation;

        private void TryToCompleteContent()
        {
            if (!_completedAnimation) return;
            if (!_completedAudio) return;

            RenderState(false);
            _interactionListener.ContentCompleted(this);
        }


        private void RenderState(bool visible)
        {
            gameObject.name = Type.ToString();
            if (visible)
                gameObject.name += " >>";
        }


        IEnumerator PlayAudioRoutine(AudioSource source, Action onCompleteCallback)
        {
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
            onCompleteCallback.Invoke();
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