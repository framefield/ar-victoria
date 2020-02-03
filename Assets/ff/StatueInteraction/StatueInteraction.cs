using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using victoria;
using victoria.bodyParts;

public class StatueInteraction : MonoBehaviour
{
    [SerializeField] private BodyPartData<MeshRenderer> _meshes;
    [SerializeField] private BodyPartRenderers _renderers;
    
    public interface IInteractionListener
    {
        void OnHover();
        void OnUnhover();
    }
}
