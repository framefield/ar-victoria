using System;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using victoria.admintools;
using victoria.audio;
using victoria.input;
using victoria.ui;

namespace victoria.controller
{
    /// <summary>
    /// Main App Controller. Handles the 3 app state - Home, Tour & Admin. Initializes and connects all components in the scene.
    /// </summary>
    public class AppController : MonoBehaviour, TourController.ITourEventsListener, SpeechInput.ICommandListener
    {
        [SerializeField] private SpeechInput _speechInput = null;
        [SerializeField] private Camera _camera = null;
        [SerializeField] private TourController _tourController = null;
        [SerializeField] private SoundFX _soundFX = null;
        [SerializeField] private AdminComponents _admincomponents = null;
        [SerializeField] private NotificationUI _notificationUI;

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
            _tourController.Initialize(this, _camera, _soundFX, _notificationUI);
            _speechInput.Initialize(this, _soundFX, _notificationUI);
            _admincomponents.TransformationTool.Initialize(
                _admincomponents.CalibratedTransform.GetComponent<CalibratedObject>(),
                _admincomponents.VirtualVictoria.gameObject,
                StartTour);
            SetState(State.Admin);
        }

        private void StartTour(TourController.TourMode mode)
        {
            _tourController.StartTour(mode);
            SetState(State.Tour);
        }

        void TourController.ITourEventsListener.OnTourCompleted()
        {
            SetState(State.Home);
        }

        private void SetState(State state)
        {
            _state = state;
            _admincomponents.CalibratedTransform.enabled = _state == State.Admin;
            _admincomponents.TransformationTool.gameObject.SetActive(_state == State.Admin);
            _admincomponents.AnimatedCursor.gameObject.SetActive(_state == State.Admin);
            _admincomponents.VirtualVictoria.gameObject.SetActive(_state == State.Admin);
            _admincomponents.HoldoutVictoria.gameObject.SetActive(_state != State.Admin);
            _admincomponents.SpatialMapping.gameObject.SetActive(_state == State.Admin);
//        _currentStateLabel.text = $"State: {state.ToString()}";
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
                        StartTour(TourController.TourMode.Unguided);
                    break;
                case SpeechInput.Command.Bravo:
                    if (_state != State.Tour)
                        StartTour(TourController.TourMode.Guided);
                    break;
                case SpeechInput.Command.Charlie:
                    if (_state != State.Tour)
                        StartTour(TourController.TourMode.Mixed);
                    break;
                case SpeechInput.Command.CancelTour:
                    if (_state == State.Tour)
                        _tourController.AbortTour();
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
    }
}