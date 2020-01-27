using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class ButtonTile : MonoBehaviour, IFocusable
{
   
    public MeshRenderer m_ButtonHighlight;

    public void OnFocusEnter()
    {
        m_ButtonHighlight.enabled = true;
    }

    public void OnFocusExit()
    {
        m_ButtonHighlight.enabled = false;
    }
    

    void Start()
    {
        m_ButtonHighlight.enabled = false;
    }

}
