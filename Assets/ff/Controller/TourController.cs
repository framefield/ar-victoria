using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using victoria.interaction;
using victoria.tour;

namespace victoria
{
    public class TourController : MonoBehaviour, StatueInteraction.IInteractionListener,
        TourStation.IInteractionListener
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Camera _camera;
        [SerializeField] private UI _ui;
        [SerializeField] private StatueInteraction _interaction;
        [SerializeField] private TourStation[] _content;

        public void Init(ITourEventsListener listener)
        {
            _listener = listener;
            _interaction.Initialize(this, _camera);

            foreach (var c in _content)
            {
                c.Init(this, _audioSource);
            }

            gameObject.SetActive(false);
        }

        public void StartTour(TourMode mode)
        {
            _model = new Model()
            {
                TourMode = mode,
                CompletedContent = new List<InteractiveSegment.SegmentType>(),
            };
            gameObject.SetActive(true);
        }


        private void Update()
        {
            if (_model.CompletedContent.Count == StatueInteraction.SegmentCount)
            {
                gameObject.SetActive(false);
                _listener.OnTourCompleted();
            }

            RenderModel(_model, _ui, _camera, _interaction.MeshProvider);
            if (_model.CurrentState != Model.State.Hovering)
                return;

            //hovered segment had been played before
            if (_model.CompletedContent.Any(type => type == _model.HoveredSegment))
                return;

            if (_model.HasCompletedHoverProgress())
            {
                _model.CurrentState = Model.State.Playing;
                var contentToPlay = _content.First(content => content.Type == _model.HoveredSegment);
                contentToPlay.Play();
            }
        }

        private static void RenderModel(Model model, UI ui, Camera camera,
            Func<InteractiveSegment.SegmentType, MeshRenderer> rendererProvider)
        {
            ui.Cursor.UpdateCursor(model.HitPosition, model.HitNormal, model.CurrentState, camera,
                model.CalculateNormalizedProgress());
            ui.DebugLabel.text = $"{model.CurrentState}";
            if (model.CurrentState == Model.State.Hovering)
                ui.DebugLabel.text += $"\t: {Time.time - model.HoverStartTime} ";

            if (model.HoveredSegment != null)
                ui.DebugLabel.text += $"\t: {model.HoveredSegment} ";

            if (model.CurrentState == Model.State.Hovering)
            {
                var shapeModule = ui.HightlightParticles.shape;
                shapeModule.meshRenderer = rendererProvider?.Invoke(model.HoveredSegment.Value);
                ui.HightlightParticles.Play();
            }
            else
            {
                ui.HightlightParticles.Stop();
            }

            var allSegments = Enum.GetValues(typeof(InteractiveSegment.SegmentType))
                .Cast<InteractiveSegment.SegmentType>();

            foreach (var segment in allSegments)
            {
                Color c;
                if (segment == model.HoveredSegment)
                {
                    c = new Color(1f, 0f, 0f, 0.1f);
                }
                else if (model.CompletedContent.Contains(segment))
                {
                    c = new Color(0f, 0f, 1f, 0.1f);
                }
                else
                {
                    c = new Color(0f, 1f, 0f, 0.1f);
                }

                rendererProvider(segment).material.color = c;
            }
        }


        [Serializable]
        private struct UI
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
            _model.HoverStartTime = Time.time;
            RenderModel(_model, _ui, _camera, _interaction.MeshProvider);
        }

        void StatueInteraction.IInteractionListener.OnUpdateHover(StatueInteraction.HoverEventData eventData)
        {
            if (_model.CurrentState == Model.State.Playing)
                return;

            if (_model.CompletedContent.Contains(eventData.HoveredType))
                return;

            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            RenderModel(_model, _ui, _camera, _interaction.MeshProvider);
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
            RenderModel(_model, _ui, _camera, _interaction.MeshProvider);
        }

        [SerializeField] private Model _model;
        private ITourEventsListener _listener;

        void TourStation.IInteractionListener.ContentCompleted(TourStation completedChapter)
        {
            _model.CompletedContent.Add(_model.HoveredSegment.Value);
            _model.CurrentState = Model.State.Default;
            _model.HitPosition = null;
            _model.HitNormal = null;
            _model.HoveredSegment = null;
            RenderModel(_model, _ui, _camera, _interaction.MeshProvider);
        }

        public enum TourMode
        {
            Guided,
            Unguided,
            Mixed
        }


        public interface ITourEventsListener
        {
            void OnTourCompleted();
        }


        [Serializable]
        public struct Model
        {
            private const float SelectionTimeThreshold = 1f;

            public enum State
            {
                Default,
                Hovering,
                Playing
            }


            public float CalculateNormalizedProgress()
            {
                if (HoverStartTime == null || CurrentState != State.Hovering)
                    return 0f;
                return (Time.time - HoverStartTime.Value) / SelectionTimeThreshold;
            }

            public bool HasCompletedHoverProgress()
            {
                return CalculateNormalizedProgress() >= 1f;
            }

            public TourMode TourMode;
            public InteractiveSegment.SegmentType? HoveredSegment;
            public State CurrentState;
            public Vector3? HitPosition;
            public Vector3? HitNormal;
            public float? HoverStartTime;
            public List<InteractiveSegment.SegmentType> CompletedContent;
        }
    }
}