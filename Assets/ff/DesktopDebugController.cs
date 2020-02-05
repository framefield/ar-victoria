using UnityEngine;

namespace victoria
{
    public class DesktopDebugController : MonoBehaviour
    {
        [SerializeField] private float _speed = .05f;
        [SerializeField] private float _rotationSpeed = 1f;

        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
                transform.position += _speed * transform.TransformVector(Vector3.left);
            if (Input.GetKey(KeyCode.S))
                transform.position += _speed * transform.TransformVector(Vector3.back);
            if (Input.GetKey(KeyCode.D))
                transform.position += _speed * transform.TransformVector(Vector3.right);
            if (Input.GetKey(KeyCode.W))
                transform.position += _speed * transform.TransformVector(Vector3.forward);
            if (Input.GetKey(KeyCode.Q))
                AddRotation(Vector3.down * _rotationSpeed);
            if (Input.GetKey(KeyCode.E))
                AddRotation(Vector3.up * _rotationSpeed);
            if (Input.GetKey(KeyCode.Z))
                AddRotation(Vector3.left * _rotationSpeed);
            if (Input.GetKey(KeyCode.X))
                AddRotation(Vector3.right * _rotationSpeed);
        }

        private void AddRotation(Vector3 euler)
        {
            var r = transform.rotation.eulerAngles;
            r += euler;
            transform.rotation = Quaternion.Euler(r);
        }
    }
}