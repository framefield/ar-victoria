using System;
using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Unity.InputModule;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using victoria.interaction;

namespace victoria
{
    /// <summary>
    /// Controls a tour that consists of TourStations. 
    /// </summary>
    public class TourController : MonoBehaviour, StatueInteraction.IInteractionListener,
        TourStation.IInteractionListener
    {
        [SerializeField] private Model _model;
        [SerializeField] private InteractionUI _interactionUI = null;
        [SerializeField] private StatueInteraction _interaction = null;
        [SerializeField] private TourStation[] _content = null;

        // called by the AppController
        public void Initialize(ITourEventsListener listener, Camera cam, SoundFX soundFx, NotificationUI notificationUi,
            TourLog tourLog, AnimatedCursor animatedCursor)
        {
            _camera = cam;
            _animatedCursor = animatedCursor;
            _soundFX = soundFx;
            _listener = listener;
            _interaction.Initialize(this, _camera);
            _notificationUI = notificationUi;
            _interactionUI.Initialize(PlayHoveredSegment);
            _tourLog = tourLog;
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
            RenderModel(_interactionUI, _model, _interaction, _camera, _animatedCursor);
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
                    PlayContent(null);
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
            RenderModel(_interactionUI, _model, _interaction, _camera, _animatedCursor);
        }

        private void PlayHoveredSegment()
        {
            _model.CurrentCursorState = Model.CursorState.Playing;
            PlayContent(_model.HoveredSegment.Value);

            _tourLog.LogEvent(TourLog.TourEvent.Play, _model.HoveredSegment.Value);
            _notificationUI.ShowDebugNotification($"Play {_model.HoveredSegment.Value.ToString()}");
        }

        private void PlayContent(InteractiveSegment.SegmentType? type)
        {
            foreach (var c in _content)
            {
                if (c.Type == type)
                    c.Play();
                else
                    c.Stop();
            }

            if (type.HasValue)
                _soundFX.Play(SoundFX.SoundType.ContentStarted);
        }

        private static void RenderModel(InteractionUI interactionUi, Model model, StatueInteraction interaction,
            Camera camera, AnimatedCursor animatedCursor)
        {
            interactionUi.UpdateCursor(model.HitPosition, model.HitNormal, camera);
            var showCustomCursor = model.CurrentCursorState == Model.CursorState.DwellTimer;
            interactionUi.SetCursorVisible(showCustomCursor);
            animatedCursor.gameObject.SetActive(!showCustomCursor);
            RenderHighlightParticles(model, interaction, interactionUi);
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
                {
                    switch (model.CurrentTourState)
                    {
                        case Model.TourState.Prologue:
                            return segment == InteractiveSegment.SegmentType.WholeStatue0;

                        case Model.TourState.Tour:
                            return segment != InteractiveSegment.SegmentType.WholeStatue0 &&
                                   segment != InteractiveSegment.SegmentType.Hall8 &&
                                   segment != model.CompletedContent.Last();

                        case Model.TourState.Epilogue:
                            return false;
                    }

                    return false;
                };
            }

            interaction.SetSegmentsActive(shouldBeActiveEvaluation);
        }

        private static void RenderHighlightParticles(Model model, StatueInteraction interaction,
            InteractionUI interactionUi)
        {
            if (model.IsInGuidedModeOrInMixedModeGuided())
            {
                //particles on next segment
                var nextSegment = model.GetSegmentToGuideTo();
                interactionUi.UpdateHighlightedMeshRenderer(nextSegment != null
                    ? interaction.GetMeshRender(nextSegment.Value)
                    : null);
            }
            else
            {
                //particles on hovered segment
                if (model.CurrentCursorState == Model.CursorState.DwellTimer)
                    interactionUi.UpdateHighlightedMeshRenderer(interaction.GetMeshRender(model.HoveredSegment.Value));
                else
                    interactionUi.UpdateHighlightedMeshRenderer(null);
            }
        }

        #region interaction handler 

        void StatueInteraction.IInteractionListener.OnBeginHover(StatueInteraction.HoverEventData eventData)
        {
            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            _model.HoveredSegment = eventData.HoveredType;

            if( _model.CurrentCursorState == Model.CursorState.Playing)
                return;
            
            if (_model.CurrentTourState == Model.TourState.Prologue) //quick fix, to quick select first the tour station - whole statue 
                PlayHoveredSegment();
            else 
                BeginDwellTimerForHoveredSegment();

            RenderModel(_interactionUI, _model, _interaction, _camera, _animatedCursor);
        }

        private void BeginDwellTimerForHoveredSegment()
        {
            var mode = _model.IsInGuidedModeOrInMixedModeGuided()
                ? InteractionUI.Mode.Guided
                : InteractionUI.Mode.Unguided;
            _interactionUI.StartSelectionTimer(mode);

            _model.CurrentCursorState = Model.CursorState.DwellTimer;
            _soundFX.Play(SoundFX.SoundType.OnDwellTimerBegin);
            _notificationUI.ShowDebugNotification($"Start Dwell Timer {_model.HoveredSegment}");
        }

        private void CancelDwellTimerForHoveredSegment()
        {
            _interactionUI.CancelSelectionTimer();
            _model.CurrentCursorState = Model.CursorState.Default;
            _soundFX.Play(SoundFX.SoundType.OnDwellTimerCanceled);
            _notificationUI.ShowDebugNotification($"Cancel Dwell Timer {_model.HoveredSegment}");
        }

        void StatueInteraction.IInteractionListener.OnUpdateHover(StatueInteraction.HoverEventData eventData)
        {
            _model.HitPosition = eventData.HitPosition;
            _model.HitNormal = eventData.HitNormal;
            RenderModel(_interactionUI, _model, _interaction, _camera, _animatedCursor);
        }

        void StatueInteraction.IInteractionListener.OnStopHover(InteractiveSegment.SegmentType type)
        {
            _model.HitPosition = null;
            _model.HitNormal = null;
            _model.HoveredSegment = null;
            if (_model.CurrentCursorState == Model.CursorState.DwellTimer)
                CancelDwellTimerForHoveredSegment();
            RenderModel(_interactionUI, _model, _interaction, _camera, _animatedCursor);
        }

        void TourStation.IInteractionListener.ContentCompleted(TourStation completedChapter)
        {
            _model.CompletedContent.Add(completedChapter.Type);
            
            var mode = _model.IsInGuidedModeOrInMixedModeGuided()
                ? InteractionUI.Mode.Guided
                : InteractionUI.Mode.Unguided;
            _interactionUI.Reset(mode); // set the ui timeline to t=0.0 
            
            _tourLog.LogEvent(TourLog.TourEvent.Complete, completedChapter.Type);
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

            RenderModel(_interactionUI, _model, _interaction, _camera, _animatedCursor);
        }

        #endregion

        private ITourEventsListener _listener;
        private SoundFX _soundFX;
        private Camera _camera;
        private NotificationUI _notificationUI;
        private TourLog _tourLog;
        private AnimatedCursor _animatedCursor;

        #region data structure

        [Serializable]
        private struct UI
        {
            public ParticleSystem HightlightParticles;
            public TMP_Text DebugLabel;

            public static UI Empty = new UI()
            {
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
            public InteractiveSegment.SegmentType? HoveredSegment;
            public CursorState CurrentCursorState;
            public Vector3? HitPosition;
            public Vector3? HitNormal;
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
                        return CompletedContent.Distinct().Count() == StatueInteraction.SegmentCount;
                    case TourMode.Mixed:
                        return CompletedContent.Contains(InteractiveSegment.SegmentType.Arm1) &&
                               CompletedContent.Contains(InteractiveSegment.SegmentType.Palm2) &&
                               CompletedContent.Contains(InteractiveSegment.SegmentType.Timeline6) &&
                               CompletedContent.Contains(InteractiveSegment.SegmentType.Head4);
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