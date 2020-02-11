using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        [SerializeField] private Camera _camera = null;
        [SerializeField] private UI _ui = UI.Empty;
        [SerializeField] private StatueInteraction _interaction = null;
        [SerializeField] private TourStation[] _content = null;

        public void Init(ITourEventsListener listener)
        {
            _listener = listener;
            _interaction.Initialize(this, _camera);
            foreach (var c in _content)
            {
                c.Init(this);
            }

            SetState(Model.TourState.Inactive);
        }

        public void StartTour(TourMode mode)
        {
            _model = new Model()
            {
                TourMode = mode,
                CompletedContent = new List<InteractiveSegment.SegmentType>(),
            };
            SetState(Model.TourState.Prologue);
        }

        private void SetState(Model.TourState tourState)
        {
            gameObject.SetActive(tourState != Model.TourState.Inactive);

            switch (tourState)
            {
                case Model.TourState.Inactive:
                    _listener.OnTourCompleted();
                    break;
                case Model.TourState.Prologue:
                    break;
                case Model.TourState.Tour:
                    break;
                case Model.TourState.Epilogue:
                    PlayContent(InteractiveSegment.SegmentType.Hall8);
                    break;
            }

            _model.CurrentTourState = tourState;
        }

        private void Update()
        {
            RenderModel(_model, _ui, _camera, _interaction.MeshProvider);
            if (_model._currentCursorState != Model.CursorState.Hovering)
                return;

            //hovered segment had been played before
            if (_model.CompletedContent.Any(type => type == _model.HoveredSegment))
                return;

            if (_model.HasCompletedHoverProgress())
            {
                _model._currentCursorState = Model.CursorState.Playing;
                PlayContent(_model.HoveredSegment.Value);
            }
        }

        private void PlayContent(InteractiveSegment.SegmentType type)
        {
            foreach (var c in _content)
            {
                if (c.Type == type)
                    c.Play();
                else
                    c.Stop();
            }

            var contentToPlay = _content.First(content => content.Type == type);


            contentToPlay.Play();
        }

        private static void RenderModel(Model model, UI ui, Camera camera,
            Func<InteractiveSegment.SegmentType, MeshRenderer> rendererProvider)
        {
            ui.Cursor.UpdateCursor(model.HitPosition, model.HitNormal, model._currentCursorState, camera,
                model.CalculateNormalizedProgress());
            ui.DebugLabel.text = $"{model._currentCursorState}";
            if (model._currentCursorState == Model.CursorState.Hovering)
                ui.DebugLabel.text += $"\t: {Time.time - model.HoverStartTime} ";

            if (model.HoveredSegment != null)
                ui.DebugLabel.text += $"\t: {model.HoveredSegment} ";

            if (model._currentCursorState == Model.CursorState.Hovering)
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
                if (model.CurrentTourState == Model.TourState.Prologue)
                {
                    rendererProvider(segment).gameObject
                        .SetActive(segment == InteractiveSegment.SegmentType.WholeStatue0);
                }
                else
                {
                    rendererProvider(segment).gameObject
                        .SetActive(segment != InteractiveSegment.SegmentType.WholeStatue0);
                }


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
            public static UI Empty = new UI()
            {
                Cursor =  null,
                DebugLabel = null,
                HightlightParticles = null
            };
        }

        void StatueInteraction.IInteractionListener.OnBeginHover(StatueInteraction.HoverEventData eventData)
        {
            if (_model._currentCursorState == Model.CursorState.Playing)
                return;

            switch (_model.CurrentTourState)
            {
                case Model.TourState.Prologue:
                    if (eventData.HoveredType != InteractiveSegment.SegmentType.WholeStatue0)
                        return;
                    break;

                case Model.TourState.Tour:
                    if (_model.CompletedContent.Contains(eventData.HoveredType))
                        return;
                    if (eventData.HoveredType == InteractiveSegment.SegmentType.Hall8)
                        return;
                    break;
            }


            _model._currentCursorState = Model.CursorState.Hovering;
            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            _model.HoveredSegment = eventData.HoveredType;
            _model.HoverStartTime = Time.time;
            RenderModel(_model, _ui, _camera, _interaction.MeshProvider);
        }

        void StatueInteraction.IInteractionListener.OnUpdateHover(StatueInteraction.HoverEventData eventData)
        {
            if (_model._currentCursorState == Model.CursorState.Playing)
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

            if (_model._currentCursorState == Model.CursorState.Playing)
                return;

            _model._currentCursorState = Model.CursorState.Default;
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
            _model._currentCursorState = Model.CursorState.Default;
            _model.HitPosition = null;
            _model.HitNormal = null;
            _model.HoveredSegment = null;

            //change states
            switch (_model.CurrentTourState)
            {
                case Model.TourState.Prologue:
                    SetState(Model.TourState.Tour);
                    break;
                case Model.TourState.Tour:
                    if (_model.CompletedContent.Count == StatueInteraction.SegmentCount - 1)
                        SetState(Model.TourState.Epilogue);
                    break;
                case Model.TourState.Epilogue:
                    SetState(Model.TourState.Inactive);
                    break;
            }

            Debug.Log("complete");

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

            public enum CursorState
            {
                Default,
                Hovering,
                Playing
            }

            public enum TourState
            {
                Inactive,
                Prologue,
                Tour,
                Epilogue
            }

            public TourState CurrentTourState;


            public float CalculateNormalizedProgress()
            {
                if (HoverStartTime == null || _currentCursorState != CursorState.Hovering)
                    return 0f;
                return (Time.time - HoverStartTime.Value) / SelectionTimeThreshold;
            }

            public bool HasCompletedHoverProgress()
            {
                return CalculateNormalizedProgress() >= 1f;
            }

            public TourMode TourMode;
            public InteractiveSegment.SegmentType? HoveredSegment;
            [FormerlySerializedAs("CurrentState")] public CursorState _currentCursorState;
            public Vector3? HitPosition;
            public Vector3? HitNormal;
            public float? HoverStartTime;
            public List<InteractiveSegment.SegmentType> CompletedContent;
        }
    }
}