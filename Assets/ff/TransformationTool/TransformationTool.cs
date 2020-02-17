using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;
using victoria.controller;

namespace victoria.admintools
{
    /// <summary>
    /// Small menu to adapt the calibration of the statue.
    /// </summary>
    public class TransformationTool : MonoBehaviour
    {
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
        public void Initialize(CalibratedObject calibratedObject, GameObject virtualVictoria, Action<TourController.TourMode> tourStartCallback)
        {
            _calibratedObject = calibratedObject;
            _virtualVictoria = virtualVictoria;
            AddTrigger(_translateXPos, () => _calibratedObject.Translate(Vector3.right));
            AddTrigger(_translateXNeg, () => _calibratedObject.Translate(Vector3.left));
            AddTrigger(_translateYPos, () => _calibratedObject.Translate(Vector3.up));
            AddTrigger(_translateYNeg, () => _calibratedObject.Translate(Vector3.down));
            AddTrigger(_translateZPos, () => _calibratedObject.Translate(Vector3.forward));
            AddTrigger(_translateZNeg, () => _calibratedObject.Translate(Vector3.back));

            AddTrigger(_rotYPos, () => _calibratedObject.RotateY(1f));
            AddTrigger(_rotYNeg, () => _calibratedObject.RotateY(-1f));

            AddTrigger(_scalePos, () => _calibratedObject.ScaleUniform(1f));
            AddTrigger(_scaleNeg, () => _calibratedObject.ScaleUniform(-1f));

            AddTrigger(_reset, () => _calibratedObject.ResetCalibration());
            AddTrigger(_showHitMesh, () => _virtualVictoria.gameObject.SetActive(true));
            AddTrigger(_hideHitMesh, () => _virtualVictoria.gameObject.SetActive(false));
            
            AddTrigger(_startAlpha, () => tourStartCallback.Invoke(TourController.TourMode.Unguided));
            AddTrigger(_startBravo, () => tourStartCallback.Invoke(TourController.TourMode.Guided));
            AddTrigger(_startCharlie, () => tourStartCallback.Invoke(TourController.TourMode.Mixed));
        }

        private static void AddTrigger(EventTrigger trigger, Action action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => action.Invoke());
            trigger.triggers.Add(entry);
        }

        private CalibratedObject _calibratedObject;
        private GameObject _virtualVictoria;
    }
}