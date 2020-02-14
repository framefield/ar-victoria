using System;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace victoria
{
    /// <summary>
    /// The gaze cursor with Default, Hover and Playing State
    /// </summary>
    public class Cursor : MonoBehaviour
    {
        [SerializeField] private Style _defaultStyle = Style.Default;
        [SerializeField] private Style _hoverStyle = Style.Default;
        [SerializeField] private Style _playingStyle = Style.Default;
        [SerializeField] private Circle _cursorCircle = null;
        [SerializeField] private Circle _progressCircle = null;
        [SerializeField] private float _lerpFactor = 0.5f;
        [SerializeField] private float _cursorToCamDistance = 2f;

        [Serializable]
        public struct Style
        {
            public static Style Default = new Style()
            {
                Radius = .1f,
                Width = .2f,
            };

            public float Width;
            public float Radius;
            public AnimationCurve Osscilation;
        }

        public void UpdateCursor(Vector3? position, Vector3? normal, TourController.Model.CursorState cursorState,
            Camera cam, float selectionProgress)
        {
            _progressCircle.gameObject.SetActive(selectionProgress > 0f);
            _progressCircle.FillRatio = selectionProgress;
            if (cursorState == TourController.Model.CursorState.Hovering)
            {
                var p = position.Value;
                var n = normal.Value;
                _currentPosition = p;
                _currentRotation = Quaternion.LookRotation(-n);
            }
            else
            {
                var t = cam.transform;
                var forward = t.forward;
                var pos = t.position;
                _currentPosition = pos +_cursorToCamDistance* forward;
                _currentRotation = Quaternion.LookRotation(2 * forward);
            }


            //Render State
            switch (cursorState)
            {
                case TourController.Model.CursorState.Default:
                    _currentStyle = _defaultStyle;
                    break;
                case TourController.Model.CursorState.Hovering:
                    _currentStyle = _hoverStyle;
                    break;
                case TourController.Model.CursorState.Playing:
                    _currentStyle = _playingStyle;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cursorState), cursorState, null);
            }
        }

        private void Update()
        {
            var osscilation = _currentStyle.Osscilation.Evaluate(Time.time % _currentStyle.Osscilation.Duration());
            var r = Mathf.Lerp(_cursorCircle.Radius, _currentStyle.Radius + osscilation, _lerpFactor);
            var w = Mathf.Lerp(_cursorCircle.Width, _currentStyle.Width, _lerpFactor);

            _cursorCircle.Radius = r;
            _progressCircle.Radius = r;

            _cursorCircle.Width = w;
            _progressCircle.Width = w;

            transform.position = Vector3.Lerp(transform.position, _currentPosition, _lerpFactor);
            transform.rotation = Quaternion.Lerp(transform.rotation, _currentRotation, _lerpFactor);
        }

        private Style _currentStyle;
        private Vector3 _currentPosition;
        private Quaternion _currentRotation;
    }
}