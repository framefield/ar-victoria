using System;
using UnityEngine;

namespace victoria
{
    public class Cursor : MonoBehaviour
    {
        [SerializeField] private Style DefaultStyle;
        [SerializeField] private Style HoverStyle;
        [SerializeField] private Style PlayingStyle;
        [SerializeField] private Renderer _cursorRenderer;
        [SerializeField] private Renderer _progressRenderer;

        [Serializable]
        public struct Style
        {
            public float AlphaCutoff;
        }

        public float Tiling = 7f;
        public float RotationSpeed = 0.01f;

        public void UpdateCursor(Vector3? position, Vector3? normal, TourController.Model.State state, Camera camera,
            float selectionProgress)
        {

            _progressRenderer.material.SetTextureOffset("_MainTex", Vector2.right * selectionProgress / 2f);

            if (state != TourController.Model.State.Default)
            {
                var p = position.Value;
                var n = normal.Value;
                _currentPosition = p;
                _currentRotation = Quaternion.LookRotation(-n);
            }
            else
            {
                var t = camera.transform;
                var forward = t.forward;
                var pos = t.position;
                _currentPosition = pos + forward;
                _currentRotation = Quaternion.LookRotation(2 * forward);
            }


            //Render State
            switch (state)
            {
                case TourController.Model.State.Default:
                    _currentStyle = DefaultStyle;
                    break;
                case TourController.Model.State.Hovering:
                    _currentStyle = HoverStyle;
                    break;
                case TourController.Model.State.Playing:
                    _currentStyle = PlayingStyle;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
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