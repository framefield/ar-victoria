using System;
using UnityEngine;

namespace ff.common.animation
{
    public class TimedAnimationController
    {
        public float ClipLength => _playedClip ? _playedClip.length : 0.0f;

        public TimedAnimationController(Animator animator, string layerName = "Base Layer")
        {
            _animator = animator;
            _animator.speed = 0;

            if (_animator.layerCount > 1)
            {
                var found = false;
                for (var i = 0; i < _animator.layerCount; i++)
                {
                    var currentLayerName = _animator.GetLayerName(i);
                    if (currentLayerName == layerName)
                    {
                        _layerIndex = i;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Debug.LogWarningFormat("Layer `{0}` was not found, using layer 0.", layerName);
                }
                else if (_layerIndex > 0)
                {
                    var weight = _animator.GetLayerWeight(_layerIndex);
                    if (Math.Abs(weight) < Mathf.Epsilon)
                    {
                        Debug.LogWarningFormat($"Layer `{layerName}`:{_layerIndex} of {_animator.name} has zero weight, setting to 1.0.", _animator);
                        _animator.SetLayerWeight(_layerIndex, 1.0f);
                    }
                }
            }

            var clips = _animator.GetCurrentAnimatorClipInfo(_layerIndex);
            if (clips == null || clips.Length == 0 || clips[0].clip == null)
            {
                Debug.LogAssertion("Invalid ClipDefinition in TimedAnimationController " + animator, animator);
                return;
            }

            _playedClip = clips[0].clip;
            var clipName = _playedClip.name;
            _clipNameHash = Animator.StringToHash(clipName);

            if (!_animator.HasState(_layerIndex, _clipNameHash))
            {
                // To fix this assertion:
                //  1. Double click the animation controller linked in the inspector
                //  2. Select the state
                //  3. Make sure that the name of animator-state matches the name of the motion-clip
                Debug.LogAssertion($"Could not find State {clipName} ClipDefinition in Layer {layerName} TimedAnimationController. " +
                                   $"This usually happens when renaming animation clips, ensure to rename the animator state as well.", animator);
                var currentState = _animator.GetCurrentAnimatorStateInfo(_layerIndex);
                var replacementState = currentState.shortNameHash;
                _clipNameHash = replacementState;
                Debug.LogWarningFormat($"State `{clipName}` was not found, using replacementState {replacementState}.", animator);
            }
        }

        public void AdvanceTimePosition(float deltaTime)
        {
            SetClipPosition(_progress + deltaTime);
        }

        public void SetTimePosition(float time)
        {
            SetClipPosition(time);
        }

        public void SetNormalizedPosition(float time)
        {
            if (_playedClip == null)
            {
                return;     // Return without warning because warning has been printed in constructor
            }

            SetNormalizedClipPosition(time);
            _progress = time * _playedClip.length;
        }

        private void SetClipPosition(float timePosition)
        {
            if (_playedClip == null)
            {
                return;
            }

            var normalizedTime = timePosition / _playedClip.length;
            SetNormalizedClipPosition(normalizedTime);
            _progress = timePosition;
        }

        private void SetNormalizedClipPosition(float normalizedTime)
        {
            /*
                If you came here to find fix AnimatorWarnings in the console, keep on reading.
                To fix "Animate.GoToState: State could not be found"  warnings try to react to the
                warnings "Could not find State..."

                Just for tradition, I leave this comment here

                # If you're motivated, please continue and log your time wasted by
                # trying to fix this problem in the table below:

                # Mmone: 2016-10-12: 1h
                # Tom 2017-12-xx: 2h
                # Tom 2018-02-01: 1h
                # Henrik 2019-02-15: 30min
             */

            _animator.Play(_clipNameHash, _layerIndex, normalizedTime);
            _animator.speed = 0;
        }

        private readonly int _layerIndex = 0;
        private readonly Animator _animator;
        private readonly AnimationClip _playedClip;
        private readonly int _clipNameHash;

        private float _progress;
    }
}