using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace victoria
{
    /// <summary>
    /// Visualize hover and selection for the two <see cref="TourMode"/>s. The selection is triggered when the timeline has
    /// been played completely. To configure duration and appearance of the animations, simply change the according timeline
    /// asset.
    /// </summary>
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] private float _cursorToCamDistance = 3.5f;
        [SerializeField] private TimelineAsset _guidedTimeline = null;
        [SerializeField] private TimelineAsset _unguidedTimeline = null;
        [SerializeField] private Transform _cursorTransform = null;
        [SerializeField] private ParticleSystem _particles = null;
        [SerializeField] private float _lerpFactor = 0.5f;

        public enum Mode
        {
            Guided,
            Unguided
        }

        public void Initialize(Action selectionHandler)
        {
            _playableDirector = GetComponent<PlayableDirector>();
            _playableDirector.Stop();
            _playableDirector.time = 0f;
            _playableDirector.Evaluate();
            _playableDirector.stopped += director =>
            {
                if (_hasStopBeenTriggeredManually)
                    return;
                selectionHandler.Invoke();
            };
        }

        public void Reset(Mode mode)
        {
            switch (mode)
            {
                case Mode.Guided:
                    _playableDirector.playableAsset = _guidedTimeline;
                    break;
                case Mode.Unguided:
                    _playableDirector.playableAsset = _unguidedTimeline;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            _playableDirector.Stop();
            _playableDirector.time = 0f;
            _playableDirector.Evaluate();
        }


        private void Update()
        {
            _cursorTransform.position = Vector3.Lerp(_cursorTransform.position, _cursorPosition, _lerpFactor);
            _cursorTransform.rotation = Quaternion.Lerp(_cursorTransform.rotation, _cursorRotation, _lerpFactor);
        }

        public void UpdateCursor(Vector3? position, Vector3? normal, Camera cam)
        {
            if (!position.HasValue)
            {
                var t = cam.transform;
                var forward = t.forward;
                var pos = t.position;
                _cursorPosition = pos + _cursorToCamDistance * forward;
                _cursorRotation = Quaternion.LookRotation(2 * forward);
            }

            else
            {
                var p = position.Value;
                var n = normal.Value;
                _cursorPosition = p;
                _cursorRotation = Quaternion.LookRotation(-n);
            }
        }

        public void SetCursorVisible(bool visible)
        {
            //skip first frame when player is enabled
            if (!gameObject.activeSelf && visible)
            {
                _playableDirector.time = 1f;
                _playableDirector.Evaluate();
                _playableDirector.Play();
            }

            _cursorTransform.gameObject.SetActive(visible);
        }

        public void UpdateHighlightedMeshRenderer(MeshRenderer rendererToHighlight)
        {
            var shapeModule = _particles.shape;

            // hovered renderer has changed
            if (rendererToHighlight != shapeModule.meshRenderer || !_particles.isPlaying)
            {
                shapeModule.meshRenderer = rendererToHighlight;
                _particles.Play();
            }

            if (rendererToHighlight == null)
            {
                _particles.Stop();
            }
        }

        public void StartSelectionTimer(Mode mode)
        {
            _hasStopBeenTriggeredManually = false;
            switch (mode)
            {
                case Mode.Guided:
                    _playableDirector.playableAsset = _guidedTimeline;
                    break;
                case Mode.Unguided:
                    _playableDirector.playableAsset = _unguidedTimeline;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            _playableDirector.Play();
        }

        public void CancelSelectionTimer()
        {
            _hasStopBeenTriggeredManually = true;
            _playableDirector.Stop();
            _playableDirector.time = 0f;
            _playableDirector.Evaluate();
        }

        private Vector3 _cursorPosition;
        private Quaternion _cursorRotation;
        private bool _hasStopBeenTriggeredManually;
        private PlayableDirector _playableDirector;
    }
}