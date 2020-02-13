using System;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using victoria;

public class AppController : MonoBehaviour, TourController.ITourEventsListener, SpeechInput.ICommandListener
{
    [SerializeField] private SpeechInput _speechInput = null;
    [SerializeField] private TourController _tourController = null;
    [SerializeField] private SoundFX _soundFX;
     [SerializeField] private AdminComponents _admincomponents = null;

    [Serializable]
    private class AdminComponents
    {
        public GameObject TransformationTool = null;
        public TapToPlace CalibratedTransform = null;
        public AnimatedCursor AnimatedCursor = null;
        public GameObject VirtualVictoria = null;
    }

    private void Start()
    {
        _tourController.Init(this, _soundFX);
        _speechInput.Init(this, _soundFX);
        SetState(State.Admin);
    }

    private void StartTour(TourController.TourMode mode)
    {
        _tourController.StartTour(mode);
        SetState(State.Tour);
    }

    void TourController.ITourEventsListener.OnTourCompleted()
    {
        SetState(State.Admin);
    }

    private void SetState(State state)
    {
        _state = state;
        _admincomponents.CalibratedTransform.enabled = _state == State.Admin;
        _admincomponents.TransformationTool.gameObject.SetActive(_state == State.Admin);
        _admincomponents.AnimatedCursor.gameObject.SetActive(_state == State.Admin);
        _admincomponents.VirtualVictoria.gameObject.SetActive(_state == State.Admin);
    }

    private State _state;

    private enum State
    {
        Admin,
        Tour
    }

    void SpeechInput.ICommandListener.OnCommandDetected(SpeechInput.Command command)
    {
        switch (command)
        {
            case SpeechInput.Command.Alpha:
                if (_state == State.Admin)
                    StartTour(TourController.TourMode.Guided);
                break;
            case SpeechInput.Command.Bravo:
                if (_state == State.Admin)
                    StartTour(TourController.TourMode.Unguided);
                break;
            case SpeechInput.Command.Charlie:
                if (_state == State.Admin)
                    StartTour(TourController.TourMode.Mixed);
                break;
            case SpeechInput.Command.AdminAbort:
                if (_state == State.Tour)
                    _tourController.AbortTour();
                break;
            case SpeechInput.Command.Calibrate:
                if (_state == State.Admin)
                {
                    var camV = Camera.main.transform.TransformVector(Vector3.forward);
                    _admincomponents.CalibratedTransform.GetComponent<CalibratedObject>()
                        .SetPosition(Camera.main.transform.position + 3 * camV);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command), command, null);
        }
    }
}