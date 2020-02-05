using System;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public abstract class IWidget : MonoBehaviour {
    public Project project;
    public GestureSettings[] acceptedGestures;
    public String[] keywords;
    public IWidget[] widgets;
    // Use this for initialization
    void Start () {
        IWidget parent = transform.parent.gameObject.GetComponent<IWidget>();
        if(parent!=null)
        {
            project = parent.project;
        }
        initWidget();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public abstract void initWidget();
    public abstract void updateWidgets();

    public void addWidget(IWidget widget)
    {
        widget.transform.SetParent(transform);
        widget.transform.localPosition = Vector3.zero;
        widget.project = project;
        widget.initWidget();
    }

    public void removeWidget(IWidget widget)
    {
    }

    public virtual void OnHoldCompleted(HoldCompletedEventArgs args)
    {
    }

    public virtual void OnHoldStarted(HoldStartedEventArgs args)
    {
    }

    public virtual void OnVisionEntered()
    {
    }

    public virtual void OnVisionLeave()
    {
    }

    public virtual void OnTapped(TappedEventArgs args)
    {
    }

    public virtual void OnNavigationCanceled(NavigationCanceledEventArgs args)
    {
    }

    public virtual void OnNavigationCompleted(NavigationCompletedEventArgs args)
    {
    }

    public virtual void OnNavigationUpdated(NavigationUpdatedEventArgs args)
    {
    }

    public virtual void OnNavigationStarted(NavigationStartedEventArgs args)
    {
    }

    public virtual void OnManipulationStarted(ManipulationStartedEventArgs args)
    {
    }

    public virtual void OnHoldCanceled(HoldCanceledEventArgs args)
    {
    }

    public virtual void OnManipulationUpdated(ManipulationUpdatedEventArgs args)
    {
    }

    public virtual void OnManipulationCompleted(ManipulationCompletedEventArgs args)
    {
    }

    public virtual void OnControllerButtonDown(String buttonId)
    {
    }

    public virtual void ManipulationCanceled(ManipulationCanceledEventArgs args)
    {
    }

    public virtual void OnControllerButtonUp(String buttonId)
    {
    }

    public virtual void OnAxisChanged(string v, Vector3 orientVec)
    {
    }

    public virtual void OnKeywordRecognized(String keyword)
    {
    }
}
