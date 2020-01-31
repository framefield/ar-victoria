using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TourController : MonoBehaviour
{
    public struct State
    {
        private PlayableContent playableContentInFocus;
        private PlayableContent.State _state;
    }
    
    [SerializeField] private PlayableContent[] _steps;
}
