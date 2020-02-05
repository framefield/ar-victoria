using UnityEngine;
using UnityEngine.XR.WSA.Input;


public class GestureManager : MonoBehaviour {
    private GestureRecognizer gestureRecognizer = null;
	// Use this for initialization
	void Start () {
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.ManipulationStarted += ManipulationStarted;
        gestureRecognizer.ManipulationUpdated += ManipulationUpdated;
        gestureRecognizer.ManipulationCompleted += ManipulationCompleted;
        gestureRecognizer.ManipulationCanceled += ManipulationCanceled;
        gestureRecognizer.HoldStarted += HoldStarted;
        gestureRecognizer.HoldCompleted += HoldCompleted;
        gestureRecognizer.HoldCanceled += HoldCanceled;
        gestureRecognizer.NavigationStarted += NavigationStarted;
        gestureRecognizer.NavigationUpdated += NavigationUpdated;
        gestureRecognizer.NavigationCompleted += NavigationCompleted;
        gestureRecognizer.NavigationCanceled += NavigationCanceled;
        gestureRecognizer.Tapped += Tapped;
	}

    public void ChangeAcceptedGestures(GestureSettings[] gestures)
    {
        GestureSettings mask = new GestureSettings();
        gestureRecognizer.StopCapturingGestures();
        if (gestures != null)
        {
            foreach (GestureSettings g in gestures)
            {
                mask |= g;
            }
            gestureRecognizer.SetRecognizableGestures(mask);
        }
        else
        {
            gestureRecognizer.SetRecognizableGestures(GestureSettings.None);
        }
        gestureRecognizer.StartCapturingGestures();
    }

    public void ClearGestures()
    {
        gestureRecognizer.StopCapturingGestures();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.None);
        gestureRecognizer.StartCapturingGestures();
    }

    private void Tapped(TappedEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().OnTapped(args);
        }
    }

    private void NavigationCanceled(NavigationCanceledEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().OnNavigationCanceled(args);
        }
    }

    private void NavigationCompleted(NavigationCompletedEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().OnNavigationCompleted(args);
        }
    }

    private void NavigationUpdated(NavigationUpdatedEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().OnNavigationUpdated(args);
        }
    }

    private void NavigationStarted(NavigationStartedEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().OnNavigationStarted(args);
        }
    }

    private void HoldCanceled(HoldCanceledEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().OnHoldCanceled(args);
        }
    }

    private void HoldCompleted(HoldCompletedEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().OnHoldCompleted(args);
        }
    }

    private void HoldStarted(HoldStartedEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().OnHoldStarted(args);
        }
    }

    private void ManipulationCompleted(ManipulationCompletedEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().OnManipulationCompleted(args);
        }
    }

    private void ManipulationStarted(ManipulationStartedEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().OnManipulationStarted(args);
        }
    }

    private void ManipulationUpdated(ManipulationUpdatedEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().OnManipulationUpdated(args);
        }
    }

    private void ManipulationCanceled(ManipulationCanceledEventArgs args)
    {
        IWidget focused = GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus();
        if (focused != null)
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().getFocus().ManipulationCanceled(args);
        }
    }

    // Update is called once per frame
    void Update () {
    }
}
