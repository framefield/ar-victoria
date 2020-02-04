using System;
using UnityEngine;

namespace victoria
{
    public class DesktopDebugController : MonoBehaviour
    {
        [SerializeField] private float _speed =.05f;
        [SerializeField] private float _rotationSpeed=1f;

        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
                transform.position +=_speed* transform.TransformVector(Vector3.left);
            if (Input.GetKey(KeyCode.S))
                transform.position +=_speed* transform.TransformVector(Vector3.back);
            if (Input.GetKey(KeyCode.D))
                transform.position +=_speed* transform.TransformVector(Vector3.right);
            if (Input.GetKey(KeyCode.W))
                transform.position +=_speed* transform.TransformVector(Vector3.forward);
            if (Input.GetKey(KeyCode.Q))
                transform.localRotation *=Quaternion.Euler(0f,-_rotationSpeed ,0f);
            if (Input.GetKey(KeyCode.E))
                transform.localRotation *=Quaternion.Euler(0f,_rotationSpeed ,0f);   
            if (Input.GetKey(KeyCode.Z))
                transform.localRotation *=Quaternion.Euler(-_rotationSpeed ,0f,0f);
            if (Input.GetKey(KeyCode.X))
                transform.localRotation *=Quaternion.Euler(_rotationSpeed ,0f,0f);

            
        }
    }
}