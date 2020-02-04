using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using victoria.interaction;

namespace victoria.tour
{
    /// <summary>
    /// Holds state of the interaction with the <see cref="PlayableContent"/>. Triggers play on them.
    /// </summary>
    public class TourController : MonoBehaviour, PlayableContent.IInteractionListener, StatueInteraction.IInteractionListener
    {
        [Header("Internal References")]
        [SerializeField] private PlayableContent[] _content;

        [SerializeField] private StatueInteraction _interaction;
       
        private void Start()
        {
            _interaction.Initialize(this);
            foreach (var c in _content)
            {
                c.Init( this);
            }
        }
        
        void PlayableContent.IInteractionListener.ContentCompleted(PlayableContent completedContent)
        {
            
        }

        void StatueInteraction.IInteractionListener.OnBeginHover(InteractiveSegment.SegmentType type)
        {
//            _interaction.
        }

        void StatueInteraction.IInteractionListener.OnStopHover(InteractiveSegment.SegmentType type)
        {
            
        }
    }
}