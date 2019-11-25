using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/**
 * Class representing an Annotated Object. Holds the virtual representation of a physical object in a Gameobject.
 * A Dictonary of pairs of annotation ids and Annotations hold the actual Annotations
 */
public class AnnotatedObject : IWidget {
    public override void initWidget()
    {
        foreach(IWidget w in widgets)
        {
            IWidget instance = GameObject.Instantiate(w);
            instance.gameObject.SetActive(true);
            addWidget(instance);
        }
    }

    public override void updateWidgets()
    {
    }
}
