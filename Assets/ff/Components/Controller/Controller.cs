using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Unity;
using JetBrains.Annotations;
using TMPro;
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

        private void Update()
        {
            RenderModel(_model, _view, _camera);
            if (_model.CurrentState != Model.State.Hovering)
                return;

            //hovered segment had been played before
            if (_model.CompletedContent.Any(type => type == _model.HoveredSegment))
                return;

            if (Time.time - _model.HoverStartTime > _selectionTimeThreshold)
            {
                _model.CurrentState = Model.State.Playing;
                var contentToPlay = _content.First(content => content.Type == _model.HoveredSegment);
                contentToPlay.Play();
            }
        }


        private static void RenderModel(Model model, View view, Camera camera)
        {
            view.Cursor.UpdateCursor(model.HitPosition, model.HitNormal, model.CurrentState, camera);
            view.DebugLabel.text = $"{model.CurrentState}";
            if (model.CurrentState == Model.State.Hovering)
                view.DebugLabel.text += $"\t: {Time.time - model.HoverStartTime} ";


            if (model.CurrentState == Model.State.Hovering)
            {
//                if (view.HightlightParticles.shape.meshRenderer ==  model.HoveredRenderer)
//                    return;
                var shapeModule = view.HightlightParticles.shape;
                shapeModule.meshRenderer =  model.HoveredRenderer;
                view.HightlightParticles.Play();
            }
            else
            {
                view.HightlightParticles.Stop();
            }
        }


        [Serializable]
        private struct View
        {
            public ParticleSystem HightlightParticles;
            public Cursor Cursor;
            public TMP_Text DebugLabel;
        }

        void StatueInteraction.IInteractionListener.OnBeginHover(StatueInteraction.HoverEventData eventData)
        {
            if (_model.CurrentState == Model.State.Playing)
                return;

            if (_model.CompletedContent.Contains(eventData.HoveredType))
                return;
            _model.CurrentState = Model.State.Hovering;
            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            _model.HoveredSegment = eventData.HoveredType;
            _model.HoveredRenderer = eventData.HoveredRenderer;
            _model.HoverStartTime = Time.time;
            RenderModel(_model, _view, _camera);
        }

        void StatueInteraction.IInteractionListener.OnUpdateHover(StatueInteraction.HoverEventData eventData)
        {
            if (_model.CurrentState == Model.State.Playing)
                return;

            if (_model.CompletedContent.Contains(eventData.HoveredType))
                return;

            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            RenderModel(_model, _view, _camera);
        }

        void StatueInteraction.IInteractionListener.OnStopHover(InteractiveSegment.SegmentType type)
        {
            if (_model.CompletedContent.Contains(type))
                return;

            if (_model.CurrentState == Model.State.Playing)
                return;

            _model.CurrentState = Model.State.Default;
            _model.HitPosition = null;
            _model.HitNormal = null;
            _model.HoveredSegment = null;
            _model.HoveredRenderer = null;
            RenderModel(_model, _view, _camera);
        }

        [SerializeField] private Model _model;
        [SerializeField] private float _selectionTimeThreshold;

        [Serializable]
        public struct Model
        {
            public enum State
            {
                Default,
                Hovering,
                Playing
            }

            public InteractiveSegment.SegmentType? HoveredSegment;
            [CanBeNull] public MeshRenderer HoveredRenderer;
            public State CurrentState;
            public Vector3? HitPosition;
            public Vector3? HitNormal;
            public float? HoverStartTime;
            public List<InteractiveSegment.SegmentType> CompletedContent;
        }

        void PlayableContent.IInteractionListener.ContentCompleted(PlayableContent completedContent)
        {
            _model.CompletedContent.Add(_model.HoveredSegment.Value);
            _model.CurrentState = Model.State.Default;
            _model.HitPosition = null;
            _model.HitNormal = null;
            _model.HoveredSegment = null;
            _model.HoveredRenderer = null;
            RenderModel(_model, _view, _camera);
        }
    }
}