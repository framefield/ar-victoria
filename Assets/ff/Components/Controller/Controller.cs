using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro.EditorUtilities;
using UnityEngine;
using victoria.interaction;
using victoria.tour;

namespace victoria
{
    public class Controller : MonoBehaviour, StatueInteraction.IInteractionListener,
        PlayableContent.IInteractionListener
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private View _view;
        [SerializeField] private StatueInteraction _interaction;
        [SerializeField] private PlayableContent[] _content;


        private void Start()
        {
            _interaction.Initialize(this, _camera);
            foreach (var c in _content)
            {
                c.Init(this);
            }
        }

        private void StartPlayingIfThresholdReached()
        {
            if (Time.time - _model.HoverStartTime > _selectionTimeThreshold)
            {
                _model.IsPlaying = true;
                var contentToPlay = _content.First(content => content.Type == _model.HoveredSegment);
                contentToPlay.Play();
            }
        }
        

        private static void RenderModel(Model model, View view, Camera camera)
        {
            var isHovering = model.HoveredSegment != null;
            view.HightlightParticles.gameObject.SetActive(isHovering);
            view.Cursor.UpdateCursor(model.HitPosition, model.HitNormal, model.HasHit, camera);

            if (isHovering)
            {
                var hoveredRenderer = model.HoveredRenderer;
                if (view.HightlightParticles.shape.meshRenderer == hoveredRenderer)
                    return;
                var shapeModule = view.HightlightParticles.shape;
                shapeModule.meshRenderer = hoveredRenderer;
                
            }
        }


        [Serializable]
        private struct View
        {
            public ParticleSystem HightlightParticles;
            public Cursor Cursor;
        }

        void StatueInteraction.IInteractionListener.OnBeginHover(StatueInteraction.HoverEventData eventData)
        {
            _model.HasHit = true;
            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            _model.HoveredSegment = eventData.HoveredType;
            _model.HoveredRenderer = eventData.HoveredRenderer;
            _model.HoverStartTime = Time.time;
            RenderModel(_model, _view, _camera);
        }

        void StatueInteraction.IInteractionListener.OnUpdateHover(StatueInteraction.HoverEventData eventData)
        {
            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            RenderModel(_model, _view, _camera);
        }

        void StatueInteraction.IInteractionListener.OnStopHover(InteractiveSegment.SegmentType type)
        {
            _model.HasHit = false;
            _model.HitPosition = null;
            _model.HitNormal = null;
            _model.HoveredSegment = null;
            _model.HoveredRenderer = null;
            RenderModel(_model, _view, _camera);
        }

        private Model _model;
        [SerializeField] private double? _selectionTimeThreshold;

        private struct Model
        {
            public InteractiveSegment.SegmentType? HoveredSegment;
            [CanBeNull] public MeshRenderer HoveredRenderer;
            public bool HasHit;
            public Vector3? HitPosition;
            public Vector3? HitNormal;
            public float? HoverStartTime;
            public bool IsPlaying;
            public List<InteractiveSegment.SegmentType> CompletedContent;
        }

        void PlayableContent.IInteractionListener.ContentCompleted(PlayableContent completedContent)
        {
            _model.CompletedContent.Add(_model.HoveredSegment.Value);
        }

    }
}