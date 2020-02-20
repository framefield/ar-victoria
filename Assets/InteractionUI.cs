using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class InteractionUI : MonoBehaviour
{
    private PlayableDirector _playableDirector;
    [SerializeField] private TimelineAsset _guidedTimeline;
    [SerializeField] private TimelineAsset _unguidedTimeline;
    [SerializeField] private Transform _cursorTransform;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private ParticleSystem _cursorParticles;
    [SerializeField] private float _lerpFactor = 0.5f;

    public enum Mode
    {
        Guided,
        Unguided
    }

    public void Initialize(Action selectionHandler)
    {
        _playableDirector = GetComponent<PlayableDirector>();
        _playableDirector.stopped += director =>
        {
            if (!_hasStopBeenTriggeredManually)
            {
                selectionHandler.Invoke();
                gameObject.SetActive(false);
            }
        };
        gameObject.SetActive(false);
    }

    private void Update()
    {
       _cursorTransform.position = Vector3.Lerp(_cursorTransform.position, _cursorPosition, _lerpFactor);
       _cursorTransform.rotation = Quaternion.Lerp(_cursorTransform.rotation, _cursorRotation, _lerpFactor);
    }

    public void UpdateCursor(Vector3? position, Vector3? normal)
    {
        if (!position.HasValue)
            return;
        var p = position.Value;
        var n = normal.Value;
        _cursorPosition = p;
        _cursorRotation = Quaternion.LookRotation(-n);
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
    }
  
    private Vector3 _cursorPosition;
    private Quaternion _cursorRotation;

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

        gameObject.SetActive(true);
        _playableDirector.Play();
    }

    public void CancelSelectionTimer()
    {
        _hasStopBeenTriggeredManually = true;
        _playableDirector.Stop();
        gameObject.SetActive(false);
    }

    private bool _hasStopBeenTriggeredManually;
}