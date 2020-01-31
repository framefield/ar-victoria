using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentStep : MonoBehaviour
{
    public event Action ActivatedEvent;
    public event Action DeactivatedEvent;
    
    [SerializeField] private VisibleObject[] _visibleObjects;
    [SerializeField] private AudioClip _audioClip;
    public void Init(ParticleSystem highlightParticles)
    {
        _highlightParticles = highlightParticles;
    }

    public void SetActive(bool active)
    {
    }

    private enum State
    {
        Inactive, Hovered, Active
    }


    private void RenderState(State state)
    {
        
        foreach (var visibleObject in _visibleObjects)
        {
            visibleObject.SetVisible(state == State.Active);
        }
    }
    
    
    private State _state;
    private ParticleSystem _highlightParticles;
}
