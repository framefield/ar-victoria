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

        [SerializeField] private View _view; 

        public interface IInteractionListener
        {
            void OnBeginHover(InteractiveSegment.SegmentType type);
            void OnStopHover(InteractiveSegment.SegmentType type);
        }

        public void Initialize(IInteractionListener listener)
        {
            _interactionListener = listener;
        }
        
        void Update()
        {
            RaycastHit hit;
            var origin = _camera.transform.position;
            var direction = _camera.transform.TransformDirection(Vector3.forward);
            _model.HasHit = Physics.Raycast(origin, direction, out hit, Mathf.Infinity);
            if (_model.HasHit)
            {
                _model.HitPosition = hit.point;                
                _model.HitNormal= hit.normal;                
                
                Func<InteractiveSegment, bool> equalsComparision = s => s.transform == hit.transform;
                if (_segments.Any(equalsComparision))
                    UpdateHoverStates(_segments.First(equalsComparision));
                else
                    UpdateHoverStates(null);
            }
            else
            {
                UpdateHoverStates(null);
            }

            RenderModel(_model ,_view, _camera);
        }

        private static void RenderModel( Model model, View view, Camera camera)
        {
            var isHovering =model.HoveredSegment != null;
            view.HightlightParticles.gameObject.SetActive(isHovering);
            view.Cursor.UpdateCursor(model.HitPosition, model.HitNormal, model.HasHit,camera );
            
            if (isHovering)
            {
                var hoveredRenderer = model.HoveredSegment.GetMeshRenderer();
                if (view.HightlightParticles.shape.meshRenderer == hoveredRenderer)
                    return;
                var shapeModule = view.HightlightParticles.shape;
                shapeModule.meshRenderer = hoveredRenderer;
            }
        }

        private void UpdateHoverStates([CanBeNull] InteractiveSegment hitSegment)
        {
            if (hitSegment == _model.HoveredSegment)
                return;

            if ( _model.HoveredSegment != null)
                _interactionListener?.OnStopHover( _model.HoveredSegment.Type);

            if (hitSegment != null)
                _interactionListener?.OnBeginHover(hitSegment.Type);

            _model.HoveredSegment = hitSegment;
        }
        
        private IInteractionListener _interactionListener;
        private Model _model;
        private struct Model
        {
            [CanBeNull] public InteractiveSegment HoveredSegment;

            public bool HasHit;
            public Vector3 HitPosition;
            public Vector3 HitNormal;
        }

        [Serializable]
        private struct View
        {
            public ParticleSystem HightlightParticles;
            public Cursor Cursor;
        }
    }
}