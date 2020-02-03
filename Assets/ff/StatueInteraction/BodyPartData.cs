using System;
using UnityEngine;
#pragma warning disable CS0649

namespace victoria.bodyParts
{
    [Serializable]
    public class BodyPartData<T>
    {
        [SerializeField] private T _wholeStatue0 ;
        [SerializeField] private T _arm1 ;
        [SerializeField] private T _palm2;
        [SerializeField] private T _wingsFront3;
        [SerializeField] private T _head4;
        [SerializeField] private T _wingsBack5;
        [SerializeField] private T _timeline6;
        [SerializeField] private T _garment7;
        [SerializeField] private T _hall8;

        public T this[BodyPart bodyPart]
        {
            get
            {
                switch (bodyPart)
                {
                    case BodyPart.WholeStatue0: return _wholeStatue0;
                    case BodyPart.Arm1: return _arm1;
                    case BodyPart.Palm2: return _palm2;
                    case BodyPart.WingsFront3: return _wingsFront3;
                    case BodyPart.Head4: return _head4;
                    case BodyPart.WingsBack5: return _wingsBack5;
                    case BodyPart.Timeline6: return _timeline6;
                    case BodyPart.Garment7: return _garment7;
                    default:
                    case BodyPart.Hall8: return _hall8;
                }
            }
        }
    }

    [Serializable]
    public class BodyPartRenderers : BodyPartData<MeshRenderer>
        {
        }

    public enum BodyPart
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
}