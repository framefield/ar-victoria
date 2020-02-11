using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace victoria
{
    public class Cursor : MonoBehaviour
    {
        [SerializeField] private Style _defaultStyle = Style.Default;
        [SerializeField] private Style _hoverStyle = Style.Default;
        [SerializeField] private Style _playingStyle = Style.Default;
        [SerializeField] private Renderer _cursorRenderer = null;
        [SerializeField] private Renderer _progressRenderer = null;

        [Serializable]
        public struct Style
        {
            public float AlphaCutoff;

            public static Style Default = new Style()
            {
                AlphaCutoff = 0f
            };
        }

        public void UpdateCursor(Vector3? position, Vector3? normal, TourController.Model.CursorState cursorState,
            Camera cam,
            float selectionProgress)
        {
            _progressRenderer.material.SetTextureOffset("_MainTex", Vector2.right * selectionProgress / 2f);

            if (cursorState != TourController.Model.CursorState.Default)
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
                _currentPosition = pos + forward;
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

        private Style _currentStyle;
        private Vector3 _currentPosition;
        private Quaternion _currentRotation;
        [SerializeField] private float _lerpFactor = 0.5f;

        private void Update()
        {
            var currentCutoff = _cursorRenderer.material.GetFloat("_Cutoff");
            var newCutoff = Mathf.Lerp(currentCutoff, _currentStyle.AlphaCutoff, _lerpFactor);
            _cursorRenderer.material.SetFloat("_Cutoff", newCutoff);


            transform.position = Vector3.Lerp(transform.position, _currentPosition, _lerpFactor);
            transform.rotation = Quaternion.Lerp(transform.rotation, _currentRotation, _lerpFactor);
        }
    }
}