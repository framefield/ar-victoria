using System;
using UnityEngine;
using UnityEngine.Playables;

namespace victoria
{
    /// <summary>
    /// The seven content modules in the scene, that are assigned to a segment of the statue. 
    /// </summary>
    public class TourStation : MonoBehaviour
    {
        [SerializeField] public InteractiveSegment.SegmentType Type;

        public void Init(IInteractionListener listener)
        {
            _interactionListener = listener;
            _playableDirector = GetComponent<PlayableDirector>();
            SetState(State.Stopped);
        }

        private void SetState(State state)
        {
            gameObject.SetActive(state!=State.Stopped);
            gameObject.name = Type.ToString();
            gameObject.name +=$" : {state}" ;
            switch (state)
            {
                case State.Stopped:
                    _playableDirector.time = 0f;
                    _playableDirector.Evaluate();
                    _playableDirector.Stop();
                    break;
                case State.Playing:
                    _playableDirector.time = 0f;
                    _playableDirector.Evaluate();
                    _playableDirector.Play();
                    
                    break;
                case State.Idle:
                    _interactionListener.ContentCompleted(this);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            _state = state;
        }
        
        
        private void Update()
        {
            if(_state!=State.Playing)
                return;
            
            if ( _playableDirector.duration == _playableDirector.time ||Input.GetKeyDown(KeyCode.Q))
                SetState(State.Idle);
        }
        
        public void Play()
        {
          SetState(State.Playing);
        }
        
        public void Stop()
        {
          SetState(State.Stopped);
        }

        public interface IInteractionListener
        {
            void ContentCompleted(TourStation completedChapter); //animation completed or "exit" interaction
        }

        private ParticleSystem _highlightParticles;
        private IInteractionListener _interactionListener;
        private PlayableDirector _playableDirector;
        private bool _isPlaying;
        private State _state;

        private enum State
        {
            Stopped, Playing, Idle
        }
    }
}
