using System;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

namespace victoria.controller
{
    /// <summary>
    /// Main App Controller. Handles the 3 app state - Home, Tour & Admin. Initializes and connects all components in the scene.
    /// </summary>
    public class AppController : MonoBehaviour, TourController.ITourEventsListener, SpeechInput.ICommandListener,
        TransformationTool.IInteractionListener
    {
        [Header("External Reference")] [SerializeField]
        private Camera _camera = null;

        [Header("Internal Reference")] [SerializeField]
        private SpeechInput _speechInput = null;

        [SerializeField] private TourLog _tourLog = null;
        [SerializeField] private TourController _tourController = null;
        [SerializeField] private SoundFX _soundFX = null;
        [SerializeField] private NotificationUI _notificationUI = null;
        [SerializeField] private GameObject _mockEnviroment = null;
        [SerializeField] private AdminComponents _admincomponents = null;

        [Serializable]
        private class AdminComponents
        {
            public TransformationTool TransformationTool = null;
            public TapToPlace CalibratedTransform = null;
            public AnimatedCursor AnimatedCursor = null;
            public Renderer VirtualVictoria = null;
            public Renderer HoldoutVictoria = null;
            public SpatialMappingManager SpatialMapping = null;
        }

        private void Start()
        {
            _tourController.Initialize(this, _camera, _soundFX, _notificationUI, _tourLog,  _admincomponents.AnimatedCursor);
            _speechInput.Initialize(this, _soundFX, _notificationUI);
            _admincomponents.TransformationTool.Initialize(
                this,
                _admincomponents.CalibratedTransform.GetComponent<CalibratedObject>(),
                _admincomponents.VirtualVictoria.gameObject);
            SetState(State.Admin);
            _mockEnviroment.SetActive(Application.isEditor);
        }

        private void StartTour(TourController.TourMode mode)
        {
            _tourController.StartTour(mode);
            _tourLog.StartLog(_camera.transform, mode);
            SetState(State.Tour);
        }

        void TourController.ITourEventsListener.OnTourCompleted()
        {
            SetState(State.Home);
            _tourLog.CompleteLog();
        }

        private void SetState(State state)
        {
            _state = state;
            _admincomponents.CalibratedTransform.enabled = _state == State.Admin;
            _admincomponents.TransformationTool.gameObject.SetActive(_state == State.Admin);
            _admincomponents.VirtualVictoria.gameObject.SetActive(_state == State.Admin);
            _admincomponents.HoldoutVictoria.gameObject.SetActive(_state != State.Admin);
            _admincomponents.SpatialMapping.gameObject.SetActive(_state == State.Admin);
        }

        private State _state;

        private enum State
        {
            Admin,
            Tour,
            Home
        }

        void SpeechInput.ICommandListener.OnCommandDetected(SpeechInput.Command command)
        {
            switch (command)
            {
                case SpeechInput.Command.Alpha:
                    if (_state != State.Tour)
                        StartTour(TourController.TourMode.Guided);
                    break;
                case SpeechInput.Command.Bravo:
                    if (_state != State.Tour)
                        StartTour(TourController.TourMode.Unguided);
                    break;
                case SpeechInput.Command.Charlie:
                    if (_state != State.Tour)
                        StartTour(TourController.TourMode.Mixed);
                    break;
                case SpeechInput.Command.CancelTour:
                    if (_state == State.Tour)
                    {
                        _tourController.AbortTour();
                        _tourLog.CompleteLog();
                    }

                    break;
                case SpeechInput.Command.Admin:
                    if (_state == State.Home)
                    {
                        SetState(State.Admin);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, null);
            }
        }

        void TransformationTool.IInteractionListener.StartTourCommand(TourController.TourMode mode)
        {
            StartTour(mode);
        }
    }
}