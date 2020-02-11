using UnityEngine;
using victoria;

public class AppController : MonoBehaviour, TourController.ITourEventsListener
{
    [SerializeField] private TourController _tourController = null;
    [SerializeField] private GameObject _adminTools = null;

    private void Start()
    {
        _tourController.Init(this);
        SetState(State.Admin);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartTour(TourController.TourMode.Guided);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartTour(TourController.TourMode.Unguided);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            StartTour(TourController.TourMode.Mixed);
    }
    
    private void StartTour(TourController.TourMode mode)
    {
        _tourController.StartTour(mode);
        SetState(State.Tour);
    }

    void TourController.ITourEventsListener.OnTourCompleted()
    {
        SetState(State.Admin);
    }

    private void SetState(State state)
    {
        _state = state;
        _adminTools.gameObject.SetActive(_state == State.Admin);
    }
    
    private State _state;
    private enum State
    {
        Admin,
        Tour
    }
}