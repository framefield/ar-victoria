using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace victoria.interaction
{
    /// <summary>
    /// Does raycast interaction with the segments of the statue.
    /// </summary>
    public class StatueInteraction : MonoBehaviour
    {
        public const int SegmentCount = 8;
        [SerializeField] private List<InteractiveSegment> _segments = null;

        public struct HoverEventData
        {
            public InteractiveSegment.SegmentType HoveredType;
            public Vector3 HitPosition;
            public Vector3 HitNormal;
        }

        public void SetSegmentsActive(Func<InteractiveSegment.SegmentType, bool> evaluator)
        {
            foreach (var segment in _segments)
            {
                segment.gameObject.SetActive(evaluator(segment.Type));
            }
        }
        
        public MeshRenderer GetMeshRender(InteractiveSegment.SegmentType type)
        {
            return _segments.First(segment => segment.Type == type).GetComponent<MeshRenderer>();
        }

        public interface IInteractionListener
        {
            void OnBeginHover(HoverEventData eventData);
            void OnUpdateHover(HoverEventData eventData);
            void OnStopHover(InteractiveSegment.SegmentType type);
        }

        public void Initialize(IInteractionListener listener, Camera camera)
        {
            _camera = camera;
            _interactionListener = listener;
        }

        void Update()
        {
            var layerMask = LayerMask.GetMask("VictoriaCursor");
            var origin = _camera.transform.position;
            var direction = _camera.transform.TransformDirection(Vector3.forward);
            var hasHit = Physics.Raycast(origin, direction, out var hit, Mathf.Infinity,layerMask);

            var hitSegment = hit.transform?.GetComponent<InteractiveSegment>();
            var hitNormalIsPointingTowardsCam = _camera.transform.InverseTransformVector(hit.normal).z < 0;
            if (hasHit && hitSegment != null&&hitNormalIsPointingTowardsCam)
            {
                var eventData = new HoverEventData
                {
                    HitPosition = hit.point,
                    HitNormal = hit.normal,
                    HoveredType = hitSegment.Type,
                };

                if (hitSegment == _lastHitSegment)
                {
                    _interactionListener.OnUpdateHover(eventData);
                }
                else
                {
                    if (_lastHitSegment != null)
                        _interactionListener.OnStopHover(_lastHitSegment.Type);
                    _lastHitSegment = hitSegment;
                    _interactionListener.OnBeginHover(eventData);
                }
            }
            else
            {
                if (_lastHitSegment != null)
                {
                    var type = _lastHitSegment.Type;
                    _lastHitSegment = null;
                    _interactionListener.OnStopHover(type);
                }
            }
        }

        private InteractiveSegment _lastHitSegment;
        private IInteractionListener _interactionListener;
        private Camera _camera;
    }
}