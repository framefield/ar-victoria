using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace victoria.interaction
{
    /// <summary>
    /// Component that can be hit by the gaze cursor raycasts.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(Collider))]
    public class InteractiveSegment : MonoBehaviour
    {
        public static IEnumerable<SegmentType> AllSegmentTypes()
        {
            return Enum.GetValues(typeof(SegmentType)).Cast<SegmentType>();
        }

        public enum SegmentType
        {
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