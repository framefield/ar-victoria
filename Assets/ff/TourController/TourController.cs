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
        private Camera _camera = null;
        [SerializeField] private UI _ui = UI.Empty;
        [SerializeField] private StatueInteraction _interaction = null;
        [SerializeField] private TourStation[] _content = null;

        // called by the AppController
        public void Initialize(ITourEventsListener listener, Camera cam, SoundFX soundFx, NotificationUI notificationUi)
        {
            _camera = cam;
            _soundFX = soundFx;
            _listener = listener;
            _interaction.Initialize(this, _camera);
            _notificationUI = notificationUi;
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
            _notificationUI.ShowDebugNotification($"Start tour {mode.ToString()}");
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
            if (_model.CurrentCursorState != Model.CursorState.DwellTimer)
                return;

            if (_model.HasCompletedHoverProgress())
            {
                _model.CurrentCursorState = Model.CursorState.Playing;
                PlayContent(_model.HoveredSegment.Value);
                _notificationUI.ShowDebugNotification($"Play {_model.HoveredSegment.Value.ToString()}");
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
            Func<InteractiveSegment.SegmentType, bool> shouldBeActiveEvaluation = s => false;
            switch (model.TourMode)
            {
                case TourMode.Guided:
                    shouldBeActiveEvaluation = segment => segment == model.GetNextUnvisitedSegment();
                    break;
                case TourMode.Unguided:
                    //disable visited segments
                    shouldBeActiveEvaluation = segment => model.CurrentTourState == Model.TourState.Prologue
                        ? segment == InteractiveSegment.SegmentType.WholeStatue0
                        : segment != InteractiveSegment.SegmentType.WholeStatue0;
                    break;
                case TourMode.Mixed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            interaction.SetSegmentsActive(shouldBeActiveEvaluation);
        }

        private static void RenderHighlightParticles(Model model, ParticleSystem highlightParticles,
            StatueInteraction interaction)
        {
            switch (model.TourMode)
            {
                case TourMode.Guided:
                    //particles on next segment
                    var nextSegment = model.GetNextUnvisitedSegment();
                    if (nextSegment != null)
                    {
                        var shapeModule = highlightParticles.shape;
                        var renderer = interaction.GetMeshRender(nextSegment.Value);

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

                    break;
                case TourMode.Unguided:
                    //particles on hovered segment
                    if (model.CurrentCursorState == Model.CursorState.DwellTimer)
                    {
                        var shapeModule = highlightParticles.shape;
                        var renderer = interaction.GetMeshRender(model.HoveredSegment.Value);

                        // hovered renderer has changed
                        if (renderer != shapeModule.meshRenderer || !highlightParticles.isPlaying)
                        {
                            shapeModule.meshRenderer = renderer;
                            highlightParticles.Play();
                        }

                        var rate = model.CompletedContent.Contains(model.HoveredSegment.Value) ? 500 : 2000;
                        var emissionModule = highlightParticles.emission;
                        if (emissionModule.rateOverTimeMultiplier != rate)
                        {
                            emissionModule.rateOverTimeMultiplier = rate;
                            highlightParticles.Play();
                        }
                    }
                    else
                    {
                        highlightParticles.Stop();
                    }

                    break;
                case TourMode.Mixed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void RenderDebugLabel(Model model, TMP_Text debugLabel)
        {
            switch (model.TourMode)
            {
                case TourMode.Guided:
                    debugLabel.text = $"{model.GetNextUnvisitedSegment()}";
                    break;
                case TourMode.Unguided:
                    debugLabel.text = $"{model.CurrentCursorState}";
                    if (model.CurrentCursorState == Model.CursorState.DwellTimer)
                        debugLabel.text += $"\t: {Time.time - model.DwellTimerStartTime} ";
                    if (model.HoveredSegment != null)
                        debugLabel.text += $"\t: {model.HoveredSegment} ";
                    break;
                case TourMode.Mixed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region interaction handler 

        void StatueInteraction.IInteractionListener.OnBeginHover(StatueInteraction.HoverEventData eventData)
        {
//            if (_model.CurrentCursorState == Model.CursorState.Playing)
//                return;

            switch (_model.CurrentTourState)
            {
                case Model.TourState.Prologue:
                    if (eventData.HoveredType != InteractiveSegment.SegmentType.WholeStatue0)
                        return;
                    break;
            }

            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            _model.HoveredSegment = eventData.HoveredType;

            if (_model.CurrentCursorState != Model.CursorState.Playing)
            {
                BeginDwellTimerForHoveredSegment();
            }
            RenderModel(_model, _ui, _camera, _interaction);
        }

        private void BeginDwellTimerForHoveredSegment()
        {
            _model.CurrentCursorState = Model.CursorState.DwellTimer;
            _model.DwellTimerStartTime = Time.time;
            _soundFX.Play(SoundFX.SoundType.OnDwellTimerBegin);
            _notificationUI.ShowDebugNotification($"Start Dwell Timer {_model.HoveredSegment}");
        }


        private void CancelDwellTimerForHoveredSegment()
        {
            _model.CurrentCursorState = Model.CursorState.Default;
            _model.DwellTimerStartTime = float.PositiveInfinity;
            _soundFX.Play(SoundFX.SoundType.OnDwellTimerCanceled);
            _notificationUI.ShowDebugNotification($"Cancel Dwell Timer {_model.HoveredSegment}");
        }


        void StatueInteraction.IInteractionListener.OnUpdateHover(StatueInteraction.HoverEventData eventData)
        {
            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            RenderModel(_model, _ui, _camera, _interaction);
        }

        void StatueInteraction.IInteractionListener.OnStopHover(InteractiveSegment.SegmentType type)
        {
            _model.HitPosition = null;
            _model.HitNormal = null;
            _model.HoveredSegment = null;
            if (_model.CurrentCursorState == Model.CursorState.DwellTimer)
                CancelDwellTimerForHoveredSegment();
            RenderModel(_model, _ui, _camera, _interaction);
        }

        void TourStation.IInteractionListener.ContentCompleted(TourStation completedChapter)
        {
            _model.CompletedContent.Add(completedChapter.Type);
            _soundFX.Play(SoundFX.SoundType.ContentCompleted);
            _notificationUI.ShowDebugNotification(
                $"Completed {completedChapter.Type}, {_model.CompletedContent.Count}/{StatueInteraction.SegmentCount}"
            );
            
            //change states
            switch (_model.CurrentTourState)
            {
                case Model.TourState.Prologue:
                    SetState(Model.TourState.Tour);
                    break;
                case Model.TourState.Tour:
                    if (_model.CompletedContent.Count == StatueInteraction.SegmentCount)
                        SetState(Model.TourState.Epilogue);
                    break;
                case Model.TourState.Epilogue:
                    SetState(Model.TourState.Inactive);
                    break;
            }

            // check if cursor is on a segment, if so, start dwell timer for it
            if (_model.HoveredSegment != null)
            {
                _model.CurrentCursorState = Model.CursorState.DwellTimer;
                BeginDwellTimerForHoveredSegment();
            }
            else
            {
                _model.CurrentCursorState = Model.CursorState.Default;
            }
            RenderModel(_model, _ui, _camera, _interaction);
        }

        #endregion

        private ITourEventsListener _listener;
        private SoundFX _soundFX;
        private NotificationUI _notificationUI;

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

            [FormerlySerializedAs("SegmentUnderCursor")]
            public InteractiveSegment.SegmentType? HoveredSegment;

            [FormerlySerializedAs("_currentCursorState")] [FormerlySerializedAs("CurrentState")]
            public CursorState CurrentCursorState;

            public Vector3? HitPosition;
            public Vector3? HitNormal;

            [FormerlySerializedAs("HoverStartTime")]
            public float? DwellTimerStartTime;

            public List<InteractiveSegment.SegmentType> CompletedContent;

            public InteractiveSegment.SegmentType? GetNextUnvisitedSegment()
            {
                foreach (var segment in InteractiveSegment.AllSegmentTypes())
                {
                    if (!CompletedContent.Contains(segment)) return segment;
                }

                return null;
            }

            public enum CursorState
            {
                Default,
                DwellTimer,
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
                if (DwellTimerStartTime == null || CurrentCursorState != CursorState.DwellTimer)
                    return 0f;

                float threshold = 0f;
                switch (TourMode)
                {
                    case TourMode.Guided:
                        threshold = SelectionTimeThresholdGuided;
                        break;
                    case TourMode.Unguided:
                        threshold = SelectionTimeThresholdUnguided;
                        if (CompletedContent.Contains(HoveredSegment.Value))
                            threshold *= 2f;
                        break;
                    case TourMode.Mixed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                return (Time.time - DwellTimerStartTime.Value) / threshold;
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