using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleObject : MonoBehaviour
{
    public void SetVisible(bool active)
    {
        //todo: make this abstract and implement different ways to show/hide
        gameObject.SetActive(active);
    }
}
