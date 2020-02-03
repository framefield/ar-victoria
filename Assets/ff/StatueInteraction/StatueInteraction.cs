using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using victoria.tour;

namespace victoria.interaction
{
    public class StatueInteraction : MonoBehaviour
    {
        [SerializeField] private List<InteractiveSegment> _segments;
        [SerializeField] private Camera _camera;

        public interface IInteractionListener
        {
            void OnBeginHover(InteractiveSegment.SegmentType type);
            void OnStopHover(InteractiveSegment.SegmentType type);
        }

        void Update()
        {
            RaycastHit hit;
            var origin = _camera.transform.position;
            var direction = _camera.transform.TransformDirection(Vector3.forward);
            if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
                Func<InteractiveSegment, bool> equalsComparision = s => s.transform == hit.transform;
                if (_segments.Any(equalsComparision))
                    HandleHit(_segments.First(equalsComparision));
                else
                    HandleHit(null);
            }
            else
            {
                Debug.DrawRay(origin, direction * 1000, Color.white);
                HandleHit(null);
            }
        }

        private void HandleHit([CanBeNull] InteractiveSegment hitSegment)
        {
            if (hitSegment == _hoveredSegment)
                return;

            if (_hoveredSegment != null)
                _interactionListener?.OnStopHover(_hoveredSegment.Type);

            if (hitSegment != null)
                _interactionListener?.OnBeginHover(_hoveredSegment.Type);

            _hoveredSegment = hitSegment;
        }
        
        private IInteractionListener _interactionListener;
        private InteractiveSegment _hoveredSegment;
    }
}