using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class IDataService : MonoBehaviour{
    public String serviceName;
    public GameObject annotationInfoBox;
    public Dictionary<IDataProvider,Action<List<Annotation>>> annotationCallback = new Dictionary<IDataProvider, Action<List<Annotation>>>();

    void Start()
    {
        InitService();
    }

    void Update()
    {
        UpdateService();
    }

    public abstract void InitService();
    public abstract void UpdateService();
    public abstract void LoadObject(Project project, float scale, Vector3 offset);
    public abstract void LoadAnnotations(IDataProvider provider, String query);
}
