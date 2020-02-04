using UnityEngine;

namespace victoria
{
    public class Cursor : MonoBehaviour
    {
        public void UpdateCursor(Vector3? position, Vector3? normal, bool hasHit, Camera camera)
        {
            if (hasHit)
            {
                var p = position.Value;
                var n = normal.Value;
                transform.position = p;
                transform.LookAt(p-n);
            }
            else
            {
                var t = camera.transform;
                var forward = t.forward;
                var pos = t.position;
                transform.position = pos + forward;
                transform.LookAt( pos + 2* forward);
            }
        }
    }
}