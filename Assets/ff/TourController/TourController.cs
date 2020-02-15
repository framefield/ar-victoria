﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using victoria.audio;
using victoria.interaction;
using victoria.tour;
using victoria.ui;

namespace victoria.controller
{
    /// <summary>
    /// Controls a tour that consists of TourStations. 
    /// </summary>
    public class TourController : MonoBehaviour, StatueInteraction.IInteractionListener,
        TourStation.IInteractionListener
    {
        [SerializeField] private Model _model;
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

            //extra data necessary in mixed mode
            if (mode == TourMode.Mixed)
                _model.CurrentMixedInitiativeState = Model.MixedInitiativeState.Guided;

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

            RenderHighlightParticles(model, ui.HightlightParticles, interaction);
            ToggleInteractiveSegments(model, interaction);
        }

        private static void ToggleInteractiveSegments(Model model,
            StatueInteraction interaction)
        {
            Func<InteractiveSegment.SegmentType, bool> shouldBeActiveEvaluation = s => false;

            if (model.IsInGuidedModeOrInMixedModeGuided())
            {
                    shouldBeActiveEvaluation = segment => segment == model.GetSegmentToGuideTo();
                
            }
            else
            {
                shouldBeActiveEvaluation = segment =>
                    model.CurrentTourState == Model.TourState.Prologue
                        ? segment == InteractiveSegment.SegmentType.WholeStatue0
                        : segment != InteractiveSegment.SegmentType.WholeStatue0;
            }

            interaction.SetSegmentsActive(shouldBeActiveEvaluation);
        }

        private static void RenderHighlightParticles(Model model, ParticleSystem highlightParticles,
            StatueInteraction interaction)
        {
            if (model.IsInGuidedModeOrInMixedModeGuided())
            {
                //particles on next segment
                var nextSegment = model.GetSegmentToGuideTo();
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
            }
            else
            {
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
            }
        }

        #region interaction handler 

        void StatueInteraction.IInteractionListener.OnBeginHover(StatueInteraction.HoverEventData eventData)
        {
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

            if (_model.TourMode == TourMode.Mixed)
            {
                // at this point the other part (arm or palm) is implicitly the next segment in the queue
                if (_model.PalmXORArmIsCompleted())
                    _model.CurrentMixedInitiativeState = Model.MixedInitiativeState.Guided;
                else
                {
                    //toggle state
                    _model.CurrentMixedInitiativeState =
                        _model.CurrentMixedInitiativeState == Model.MixedInitiativeState.Guided
                            ? Model.MixedInitiativeState.Unguided
                            : Model.MixedInitiativeState.Guided;
                }
            }

            //change states
            switch (_model.CurrentTourState)
            {
                case Model.TourState.Prologue:
                    SetState(Model.TourState.Tour);
                    break;
                case Model.TourState.Tour:
                    if (_model.IsTourCompleted())
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
        private Camera _camera;
        private NotificationUI _notificationUI;

        #region data structure

        [Serializable]
        private struct UI
        {
            public ParticleSystem HightlightParticles;
            public StatueCursor Cursor;
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
        public class Model
        {
            public enum MixedInitiativeState
            {
                Unguided,
                Guided,
            }

            public bool IsInGuidedModeOrInMixedModeGuided()
            {
                return TourMode == TourMode.Guided || TourMode == TourMode.Mixed &&
                       CurrentMixedInitiativeState == MixedInitiativeState.Guided;
            }


            [CanBeNull] public MixedInitiativeState CurrentMixedInitiativeState;

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

            public InteractiveSegment.SegmentType? GetSegmentToGuideTo()
            {
                var allRequiredSegments = TourMode == TourMode.Guided
                    ? InteractiveSegment.AllSegmentTypes()
                    : InteractiveSegment.AllMainSegmentTypesInMixedMode();
                foreach (var segment in allRequiredSegments)
                {
                    if (!CompletedContent.Contains(segment)) return segment;
                }

                return null;
            }

            public bool IsTourCompleted()
            {
                switch (TourMode)
                {
                    case TourMode.Guided:
                    case TourMode.Unguided:
                        return CompletedContent.Count == StatueInteraction.SegmentCount;
                    case TourMode.Mixed:
                        return CompletedContent.Contains(InteractiveSegment.SegmentType.Arm1) &&
                               CompletedContent.Contains(InteractiveSegment.SegmentType.Palm2) &&
                               CompletedContent.Contains(InteractiveSegment.SegmentType.Timeline6) &&
                               CompletedContent.Contains(InteractiveSegment.SegmentType.Head4);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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

                var isGuided = TourMode == TourMode.Guided || TourMode == TourMode.Mixed &&
                               CurrentMixedInitiativeState == MixedInitiativeState.Guided;
                float threshold = isGuided ? SelectionTimeThresholdGuided : SelectionTimeThresholdUnguided;
                return (Time.time - DwellTimerStartTime.Value) / threshold;
            }

            public bool HasCompletedHoverProgress()
            {
                return CalculateNormalizedProgress() >= 1f;
            }

            private const float SelectionTimeThresholdUnguided = 3f;
            private const float SelectionTimeThresholdGuided = 0.1f;

            public bool PalmXORArmIsCompleted()
            {
                if (CompletedContent.Contains(InteractiveSegment.SegmentType.Arm1) &&
                    !CompletedContent.Contains(InteractiveSegment.SegmentType.Palm2))
                    return true;

                if (!CompletedContent.Contains(InteractiveSegment.SegmentType.Arm1) &&
                    CompletedContent.Contains(InteractiveSegment.SegmentType.Palm2))
                    return true;

                return false;
            }
        }

        #endregion
    }
}