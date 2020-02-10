using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace victoria.tour
{
    /// <summary>
    /// The seven content modules in the scene, that are assigned to a segment of the statue. 
    /// </summary>
    public class TourStation : MonoBehaviour
    {
        [SerializeField] public InteractiveSegment.SegmentType Type;
        private PlayableDirector _playableDirector;

        public void Init(IInteractionListener listener, AudioSource audioSource)
        {
            _interactionListener = listener;
            RenderState(false);
           
            _playableDirector = GetComponent<PlayableDirector>();
//            _playableDirector.stopped += director =>
//           {
////               RenderState(false);
//               listener.ContentCompleted(this);
//           };
            _playableDirector.paused+= director =>
            {
//               RenderState(false);
                listener.ContentCompleted(this);
            };
        }

        private bool isPlaying;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                
                isPlaying = false;
                _interactionListener.ContentCompleted(this);
                _playableDirector.Stop();
            } 
            if (isPlaying && _playableDirector.duration == _playableDirector.time)
            {
                isPlaying = false;
                _interactionListener.ContentCompleted(this);
            }
        }
        
        public void Play()
        {
            gameObject.SetActive(true);

            isPlaying = true;
            _playableDirector.Play();
            RenderState(true);
        }

        private void RenderState(bool visible)
        {
            gameObject.name = Type.ToString();
            if (visible)
                gameObject.name += " >>";
        }

        public interface IInteractionListener
        {
            void ContentCompleted(TourStation completedChapter); //animation completed or "exit" interaction
        }

        private ParticleSystem _highlightParticles;
        private IInteractionListener _interactionListener;

        public void Stop()
        {
            gameObject.SetActive(false);
            _playableDirector.Stop();
            RenderState(false);
        }
    }
}