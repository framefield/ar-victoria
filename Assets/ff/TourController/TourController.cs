using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using victoria.interaction;
using victoria.tour;

namespace victoria
{
    /// <summary>
    /// Controls a tour that consists of TourStations.
    /// </summary>
    public class TourController : MonoBehaviour, StatueInteraction.IInteractionListener,
        TourStation.IInteractionListener
    {
        [SerializeField] private Model _model;
        [SerializeField] private Camera _camera = null;
        [SerializeField] private UI _ui = UI.Empty;
        [SerializeField] private StatueInteraction _interaction = null;
        [SerializeField] private TourStation[] _content = null;

        // called by the AppController
        public void Init(ITourEventsListener listener, SoundFX soundFx)
        {
            _soundFX = soundFx;
            _listener = listener;
            _interaction.Initialize(this, _camera);

            foreach (var c in _content)
            {
                c.Init(this);
            }

            SetState(Model.TourState.Inactive);
        }

        // called by the AppController
        public void StartTour(TourMode mode)
        {
            _model = new Model()
            {
                TourMode = mode,
                CompletedContent = new List<InteractiveSegment.SegmentType>(),
            };
            SetState(Model.TourState.Prologue);
        }

        // called by the AppController
        public void AbortTour()
        {
            SetState(Model.TourState.Inactive);
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
            RenderModel(_model, _ui, _camera, _interaction);
            if (_model.CurrentCursorState != Model.CursorState.Hovering)
                return;

            //hovered segment had been played before
            if (_model.CompletedContent.Any(type => type == _model.HoveredSegment))
                return;

            if (_model.HasCompletedHoverProgress())
            {
                _model.CurrentCursorState = Model.CursorState.Playing;
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

            _soundFX.Play(SoundFX.SoundType.ContentStarted);
            var contentToPlay = _content.First(content => content.Type == type);
            contentToPlay.Play();
        }

        private static void RenderModel(Model model, UI ui, Camera camera,
            StatueInteraction interaction)
        {
            ui.Cursor.UpdateCursor(model.HitPosition, model.HitNormal, model.CurrentCursorState, camera,
                model.CalculateNormalizedProgress());

            RenderDebugLabel(model, ui.DebugLabel);
            RenderHighlightParticles(model, ui.HightlightParticles, interaction);
            ToggleInteractiveSegments(model, interaction);
        }

        private static void ToggleInteractiveSegments(Model model,
            StatueInteraction interaction)
        {
            var allSegments = Enum.GetValues(typeof(InteractiveSegment.SegmentType))
                .Cast<InteractiveSegment.SegmentType>();

            foreach (var segment in allSegments)
            {
                    var shouldBeActive =model.CurrentTourState == Model.TourState.Prologue
                        ? segment == InteractiveSegment.SegmentType.WholeStatue0
                        : segment != InteractiveSegment.SegmentType.WholeStatue0;
                    interaction.SetSegmentActive(segment, shouldBeActive);
            }
        }

        private static void RenderHighlightParticles(Model model, ParticleSystem highlightParticles,
            StatueInteraction interaction)
        {
            if (model.CurrentCursorState == Model.CursorState.Hovering)
            {
                var shapeModule = highlightParticles.shape;
                var renderer = interaction.GetMeshRender(model.HoveredSegment.Value);

                // hovered renderer has changed
                if (renderer != shapeModule.meshRenderer || !highlightParticles.isPlaying)
                {
                    shapeModule.meshRenderer = renderer;
                    highlightParticles.Play();
                }
            }
            else
            {
                highlightParticles.Stop();
            }
        }

        private static void RenderDebugLabel(Model model, TMP_Text debugLabel)
        {
            debugLabel.text = $"{model.CurrentCursorState}";
            if (model.CurrentCursorState == Model.CursorState.Hovering)
                debugLabel.text += $"\t: {Time.time - model.HoverStartTime} ";
            if (model.HoveredSegment != null)
                debugLabel.text += $"\t: {model.HoveredSegment} ";
        }

        void StatueInteraction.IInteractionListener.OnBeginHover(StatueInteraction.HoverEventData eventData)
        {
            if (_model.CurrentCursorState == Model.CursorState.Playing)
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

            _soundFX.Play(SoundFX.SoundType.OnHoverBegin);

            _model.CurrentCursorState = Model.CursorState.Hovering;
            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            _model.HoveredSegment = eventData.HoveredType;
            _model.HoverStartTime = Time.time;
            RenderModel(_model, _ui, _camera, _interaction);
        }

        void StatueInteraction.IInteractionListener.OnUpdateHover(StatueInteraction.HoverEventData eventData)
        {
            if (_model.CurrentCursorState == Model.CursorState.Playing)
                return;

            if (_model.CompletedContent.Contains(eventData.HoveredType))
                return;

            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            RenderModel(_model, _ui, _camera, _interaction);
        }

        void StatueInteraction.IInteractionListener.OnStopHover(InteractiveSegment.SegmentType type)
        {
            if (_model.CompletedContent.Contains(type))
                return;

            if (_model.CurrentCursorState == Model.CursorState.Playing)
                return;

            _soundFX.Play(SoundFX.SoundType.OnHoverEnd);

            _model.CurrentCursorState = Model.CursorState.Default;
            _model.HitPosition = null;
            _model.HitNormal = null;
            _model.HoveredSegment = null;
            RenderModel(_model, _ui, _camera, _interaction);
        }


        void TourStation.IInteractionListener.ContentCompleted(TourStation completedChapter)
        {
            _soundFX.Play(SoundFX.SoundType.ContentCompleted);
            _model.CompletedContent.Add(_model.HoveredSegment.Value);
            _model.CurrentCursorState = Model.CursorState.Default;
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
            RenderModel(_model, _ui, _camera, _interaction);
        }

        private ITourEventsListener _listener;
        private SoundFX _soundFX;

        #region data structure

        [Serializable]
        private struct UI
        {
            public ParticleSystem HightlightParticles;
            public Cursor Cursor;
            public TMP_Text DebugLabel;

            public static UI Empty = new UI()
            {
                Cursor = null,
                DebugLabel = null,
                HightlightParticles = null
            };
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
            public TourState CurrentTourState;
            public TourMode TourMode;
            public InteractiveSegment.SegmentType? HoveredSegment;

            [FormerlySerializedAs("_currentCursorState")] [FormerlySerializedAs("CurrentState")]
            public CursorState CurrentCursorState;

            public Vector3? HitPosition;
            public Vector3? HitNormal;
            public float? HoverStartTime;
            public List<InteractiveSegment.SegmentType> CompletedContent;

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

            public float CalculateNormalizedProgress()
            {
                if (HoverStartTime == null || CurrentCursorState != CursorState.Hovering)
                    return 0f;
                var threshold = TourMode == TourMode.Guided
                    ? SelectionTimeThresholdGuided
                    : SelectionTimeThresholdUnguided;
                return (Time.time - HoverStartTime.Value) / threshold;
            }

            public bool HasCompletedHoverProgress()
            {
                return CalculateNormalizedProgress() >= 1f;
            }

            private const float SelectionTimeThresholdUnguided = 3f;
            private const float SelectionTimeThresholdGuided = 0.1f;
        }

        #endregion
    }
}