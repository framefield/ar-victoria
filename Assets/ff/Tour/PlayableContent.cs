using System;
using UnityEngine;

public class PlayableContent : MonoBehaviour, InteractiveComponent.IInteractionListener
{
    public event Action ActivatedEvent;
    public event Action DeactivatedEvent;

    [SerializeField] private VisibleObject[] _visibleObjects;
    [SerializeField] private AudioClip _audioClip;
    
    public interface IInteractionListener
    {
        void Hover();
        void Unhover();
        void ContentCompleted(); //animation completed or "exit" interaction
    }
        
    public enum State
    {
        Inactive, Hovered, Active
    }
    
    public void Init(ParticleSystem highlightParticles, IInteractionListener listener)
    {
        _interactionListener = listener;
        _highlightParticles = highlightParticles;
        GetComponent<InteractiveComponent>().Init(this);
    }
    
    public void SetState(State state)
    {
        foreach (var visibleObject in _visibleObjects)
        {
            visibleObject.SetVisible(state == State.Active);
        }
    }
    
    void InteractiveComponent.IInteractionListener.Hover()
    {
        _interactionListener.Hover();
    }

    void InteractiveComponent.IInteractionListener.Unhover()
    {
        _interactionListener.Unhover();
    }

    private ParticleSystem _highlightParticles;
    private IInteractionListener _interactionListener;
}
