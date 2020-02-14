using UnityEngine;

namespace victoria.tour
{
    /// <summary>
    /// Component that can be hit by the gaze cursor raycasts.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(Collider))]
    public class InteractiveSegment : MonoBehaviour
    {
        public enum SegmentType {
            WholeStatue0,
            Arm1,
            Palm2,
            WingsFront3,
            Head4,
            WingsBack5,
            Timeline6,
            Garment7,
            Hall8,
        }
        
        public SegmentType Type;

    }
}