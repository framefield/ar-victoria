using UnityEngine;

namespace ff.ui
{
    public class CameraFacingBillboard : MonoBehaviour
    {
        [SerializeField]
        [Range(0, 1.0f)]
        public float SizeFactor = 0.0224f;

        [SerializeField]
        [Range(0, 0.1f)]
        public float MinSize = 0.0088f;

        [SerializeField]
        private bool _adaptSize = true;

        [SerializeField]
        private bool _onlyApplyYAxis = default;

        [SerializeField]
        private BillboardRotationMode _rotationMode = BillboardRotationMode.ParallelToCamRotation;

        public enum BillboardRotationMode
        {
            ParallelToCamRotation,
            LookAtCam,
        }

        private void Awake()
        {
            _initialScale = transform.localScale;
        }

        private void LateUpdate()
        {
            if (!_camera)
            {
                _camera = Camera.main;
            }

            if (_camera == null)
                return;

            Vector3 lookAtVector = Vector3.zero;
            switch (_rotationMode)
            {
                case BillboardRotationMode.LookAtCam:
                    lookAtVector = transform.position - _camera.transform.position;
                    break;
                case BillboardRotationMode.ParallelToCamRotation:
                    lookAtVector = _camera.transform.rotation * Vector3.forward;
                    break;
            }
            transform.LookAt(transform.position + lookAtVector,
                _camera.transform.rotation * Vector3.up);

            if (_onlyApplyYAxis)
            {
                var angles = transform.eulerAngles;
                transform.rotation = Quaternion.Euler(0, angles.y, 0);
            }

            if (_adaptSize)
            {
                var distanceToCamera = (Camera.main.transform.position - transform.position).magnitude;
                transform.localScale = _initialScale * (MinSize + distanceToCamera * SizeFactor);
            }
        }

        public Vector2 GetScaleFactor()
        {
            return new Vector2(SizeFactor * _initialScale.x, SizeFactor * _initialScale.y);
        }

        private Vector3 _initialScale = Vector3.one;

        private Camera _camera;
    }
}