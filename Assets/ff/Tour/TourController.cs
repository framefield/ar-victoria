using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TourController : MonoBehaviour
{
    
    
    
    [SerializeField] private ContentStep[] _steps;

    private void  Start()
    {
        UpdateActiveStep();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _activeStepIndex= (_activeStepIndex+1)%_steps.Length;
            UpdateActiveStep();
        }
    }
    
    private void UpdateActiveStep()
    {
        foreach (var step in _steps)
        {
            step.SetActive(step == _steps[_activeStepIndex]);
        }
    }
    
    private int _activeStepIndex;

}
