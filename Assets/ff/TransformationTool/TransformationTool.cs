using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace victoria
{
    /// <summary>
    /// Small menu to adapt the calibration of the statue.
    /// </summary>
    public class TransformationTool : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private float _translationSensitivity = 0.1f;
        [SerializeField] private float _rotationSensitivity = 0.1f;
        [SerializeField] private float _scaleSensitivity = 0.1f;
        
        [Header("Internal References")]
        [SerializeField] private EventTrigger _translateXPos = null;
        [SerializeField] private EventTrigger _translateXNeg = null;
        [SerializeField] private EventTrigger _translateYPos = null;
        [SerializeField] private EventTrigger _translateYNeg = null;
        [SerializeField] private EventTrigger _translateZPos = null;
        [SerializeField] private EventTrigger _translateZNeg = null;
        [SerializeField] private EventTrigger _rotYPos = null;
        [SerializeField] private EventTrigger _rotYNeg = null;
        [SerializeField] private EventTrigger _scalePos = null;
        [SerializeField] private EventTrigger _scaleNeg = null;
        [SerializeField] private EventTrigger _reset = null;
        [SerializeField] private EventTrigger _showHitMesh = null;
        [SerializeField] private EventTrigger _hideHitMesh = null;
        [SerializeField] private EventTrigger _startAlpha = null;
        [SerializeField] private EventTrigger _startBravo = null;
        [SerializeField] private EventTrigger _startCharlie = null;

        //todo: refactor this using a interaction interface
        public void Initialize(IInteractionListener listener, CalibratedObject calibratedObject, GameObject virtualVictoria)
        {
            _listener = listener;
            _calibratedObject = calibratedObject;
            _virtualVictoria = virtualVictoria;
            AddTrigger(_translateXPos, () => _calibratedObject.Translate(_translationSensitivity*Vector3.right));
            AddTrigger(_translateXNeg, () => _calibratedObject.Translate(_translationSensitivity*Vector3.left));
            AddTrigger(_translateYPos, () => _calibratedObject.Translate(_translationSensitivity*Vector3.up));
            AddTrigger(_translateYNeg, () => _calibratedObject.Translate(_translationSensitivity*Vector3.down));
            AddTrigger(_translateZPos, () => _calibratedObject.Translate(_translationSensitivity*Vector3.forward));
            AddTrigger(_translateZNeg, () => _calibratedObject.Translate(_translationSensitivity*Vector3.back));

            AddTrigger(_rotYPos, () => _calibratedObject.RotateY(_rotationSensitivity*1f));
            AddTrigger(_rotYNeg, () => _calibratedObject.RotateY(_rotationSensitivity*-1f));

            AddTrigger(_scalePos, () => _calibratedObject.ScaleUniform(_scaleSensitivity*1f));
            AddTrigger(_scaleNeg, () => _calibratedObject.ScaleUniform(_scaleSensitivity*-1f));

            AddTrigger(_reset, () => _calibratedObject.ResetCalibration());
            AddTrigger(_showHitMesh, () => _virtualVictoria.gameObject.SetActive(true));
            AddTrigger(_hideHitMesh, () => _virtualVictoria.gameObject.SetActive(false));
            
            AddTrigger(_startAlpha, () => _listener.StartTourCommand(TourController.TourMode.Guided));
            AddTrigger(_startBravo, () => _listener.StartTourCommand(TourController.TourMode.Unguided));
            AddTrigger(_startCharlie, () => _listener.StartTourCommand(TourController.TourMode.Mixed));
        }

        private static void AddTrigger(EventTrigger trigger, Action action)
        {
            var entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerDown};
            entry.callback.AddListener((data) => action.Invoke());
            trigger.triggers.Add(entry);
        }

        private CalibratedObject _calibratedObject;
        private GameObject _virtualVictoria;
        private IInteractionListener _listener;
        
        public interface IInteractionListener
        {
            void StartTourCommand(TourController.TourMode mode);
        }
    }
}