using UnityEngine;

public class InteractiveComponent : MonoBehaviour
{
    private IInteractionListener _interactionListener;

    public interface IInteractionListener
    {
        void Hover();
        void Unhover();
    }

    public void Init(IInteractionListener listener)
    {
        _interactionListener = listener;
    }
    
    public void Hover()
    {
        _interactionListener.Hover();
    }

    public void Unhover()
    {
        _interactionListener.Unhover();
    }
}

