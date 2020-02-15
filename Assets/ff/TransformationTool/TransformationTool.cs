using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;

public class TransformationTool : MonoBehaviour
{

    [SerializeField] private EventTrigger _translateXPos;
    [SerializeField] private EventTrigger _translateXNeg;
    [SerializeField] private EventTrigger _translateYPos;
    [SerializeField] private EventTrigger _translateYNeg;
    [SerializeField] private EventTrigger _translateZPos;
    [SerializeField] private EventTrigger _translateZNeg;
    [SerializeField] private EventTrigger _rotYPos;
    [SerializeField] private EventTrigger _rotYNeg;
    [SerializeField] private EventTrigger _scalePos;
    [SerializeField] private EventTrigger _scaleNeg;
    [SerializeField] private EventTrigger _reset;
    [SerializeField] private EventTrigger _showHitMesh;
    [SerializeField] private EventTrigger _hideHitMesh;
    
    public void Initialize(CalibratedObject calibratedObject, GameObject virtualVictoria)
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