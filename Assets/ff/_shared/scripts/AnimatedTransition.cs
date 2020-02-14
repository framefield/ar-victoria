using System;
using System.Collections.Generic;
//using ff.utils;
using UnityEngine;

namespace ff.common.animation
{
    [RequireComponent(typeof(Animator))]
    public class AnimatedTransition : MonoBehaviour
    {
        public bool Active = false; // Set by BaseAnimatedTransition
        public float FadeDuration = 1;
        public string LayerName = "visible";


        [Tooltip("playback transition on start")]
        [SerializeField]
        private bool _activateOnStart = false;

        [SerializeField]
        private List<GameObject> _childrenDeactivatedWhenHidden = new List<GameObject>();

        [Header("--- For debug only (Set by BaseAnimatedTransition) -----------")]
        [Range(0, 2)]
        protected float _timePosition = 0; // between 0 and 2,  1 being is visible

        public event Action OnTransitionInCompleted;
        public event Action OnTransitionOutCompleted;
        public event Action OnTransitionInStarted;
        public float NormalizedTime => _timePosition;

        public bool HasStarted => Active || _timePosition > 0;

        public void CompleteTransition()
        {
            if (Active != _isActive)
            {
                _isActive = Active;
                SetState(Active ? State.TransitionIn : State.TransitionOut);
            }

            switch (_state)
            {
                case State.TransitionIn:
                    FadeIn(FadeDuration + 1.0f);
                    break;
                case State.TransitionOut:
                    FadeOut(FadeDuration + 1.0f);
                    break;
            }

        }

        protected virtual void Awake()
        {
            var animator = GetComponent<Animator>();

            SetState(Active ? State.Active : State.Inactive);
            _isActive = Active;
            _timePosition = Active ? 1.0f : 0.0f;

            _controller = new TimedAnimationController(animator, LayerName);

            UpdateVisibilityOfChildren();
        }

        protected virtual void Start()
        {
            if (_activateOnStart)
                Active = true;
        }

        protected void Update()
        {
            if (Active != _isActive)
            {
                _isActive = Active;
                SetState(Active ? State.TransitionIn : State.TransitionOut);
            }

            switch (_state)
            {
                case State.TransitionIn:
                    FadeIn(Time.deltaTime);
                    break;
                case State.TransitionOut:
                    FadeOut(Time.deltaTime);
                    break;
                default:
                    UpdateAnimatorTime();
                    break;
            }
        }


        private void SetState(State newState)
        {
            if (newState == _state)
                return;

            var visibilityNeedsUpdate = false;
            switch (newState)
            {
                case State.Inactive:
                    OnTransitionOutCompleted?.Invoke();
                    visibilityNeedsUpdate = true;
                    break;

                case State.TransitionIn:
                    OnTransitionInStarted?.Invoke();
                    visibilityNeedsUpdate = true;
                    break;

                case State.TransitionOut:
                    break;

                case State.Active:
                    OnTransitionInCompleted?.Invoke();
                    break;
            }
            _state = newState;
            if (visibilityNeedsUpdate)
                UpdateVisibilityOfChildren();
        }


        private void FadeIn(float deltaTime)
        {
            if (_timePosition < 1)
            {
                _timePosition = Mathf.Min(1, _timePosition + deltaTime / FadeDuration);
            }
            // animate backwards...
            else if (_timePosition > 1)
            {
                _timePosition = Mathf.Max(1, _timePosition - deltaTime / FadeDuration);
            }

            if (Math.Abs(_timePosition - 1) < Mathf.Epsilon)
            {
                SetState(State.Active);
            }

            UpdateAnimatorTime();
            AnimatorTimeUpdated(_timePosition, _state);
        }


        private void FadeOut(float deltaTime)
        {
            // animate backwards...
            if (_timePosition < 1)
            {
                _timePosition = Mathf.Max(0, _timePosition - deltaTime / FadeDuration);
            }
            else if (_timePosition >= 1)
            {
                _timePosition = Mathf.Min(2, _timePosition + deltaTime / FadeDuration);
            }

            if (_timePosition <= 0 || _timePosition >= 2)
            {
                SetState(State.Inactive);
                _timePosition = 0; // Fading in should always be forward...
            }

            UpdateAnimatorTime();
            AnimatorTimeUpdated(_timePosition, _state);
        }

        private void UpdateAnimatorTime()
        {
            var clipDuration = _controller.ClipLength;

            if (clipDuration < 0.9f || clipDuration > 2.1f)
            {
                if (!_alreadyPrintedWarning)
                {
                    Debug.Log(
                        $"AnimatedTransition requires an Animator with exactly 1 clip that should have a length of either 1 or 2 sec. {name} has duration {clipDuration}", this);
                    _alreadyPrintedWarning = true;
                }
            }

            var clipIncludesFadeOut = clipDuration > 1.2;
            var normalizedTime = clipIncludesFadeOut
                ? _timePosition / clipDuration
                : (1 - Mathf.Abs(1 - _timePosition)) / clipDuration;

            _controller.SetNormalizedPosition(normalizedTime);
        }

        private void UpdateVisibilityOfChildren()
        {
            var shouldBeVisible = _state != State.Inactive;
            foreach (var o in _childrenDeactivatedWhenHidden)
            {
                o.SetActive(shouldBeVisible);
            }
        }

        protected virtual void AnimatorTimeUpdated(float time, State state)
        {
        }

        private bool _isActive;
        private State _state = State.Inactive;
        private TimedAnimationController _controller;
        private bool _alreadyPrintedWarning = false;

        protected enum State
        {
            Inactive,
            TransitionIn,
            Active,
            TransitionOut,
        }
    }
}