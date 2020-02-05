using System;
using System.Collections.Generic;
using UnityEngine;

public class DataServiceManager : MonoBehaviour {
    public Dictionary<String,IDataService> services = new Dictionary<string,IDataService>();
    private Dictionary<IDataProvider, List<KeyValuePair<String, String>>> registrations = new Dictionary<IDataProvider, List<KeyValuePair<string, string>>>();

    // Use this for initialization
    void Start () {
        WikiDataWrapper wd = GameObject.Find("Services").GetComponent<WikiDataWrapper>();
        DBPediaWrapper dbpedia = GameObject.Find("Services").GetComponent<DBPediaWrapper>();
        services.Add(wd.serviceName, wd);
        services.Add(dbpedia.serviceName, dbpedia);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnObject(Project project, Vector3 offset, float scale = 1.0f)
    {
        GameObject.Find("Services").GetComponent<WikiDataWrapper>().LoadObject(project, scale, offset);
    }

    public void registerWidget(IDataProvider provider, String service, String query)
    {
        IDataService s;
        List<KeyValuePair<String, String>> r;
        if (!registrations.TryGetValue(provider, out r))
        {
            r = new List<KeyValuePair<string, string>>();
            r.Add(new KeyValuePair<string, string>( service, query));
            registrations.Add(provider, r);
        }
        else
        {
            r.Add(new KeyValuePair<string, string>(service, query));
        }
        if(services.TryGetValue(service,out s))
        {
            Action<List<Annotation>> callbacks;
            if (!s.annotationCallback.TryGetValue(provider, out callbacks))
            {
                s.annotationCallback.Add(provider, (x =>
                {
                    provider.setAnnotations(x);
//                    provider.updateWidgets();
                }));
            }
        }
    }

    public void LoadAnnotations(IDataProvider provider)
    {
        List<KeyValuePair<String, String>> r;
        if (registrations.TryGetValue(provider,out r))
        {
            foreach(KeyValuePair<String, String> q in r)
            {
                IDataService s;
                if(services.TryGetValue(q.Key,out s))
                {
                    s.LoadAnnotations(provider, q.Value);
                }
            }
        }
    }
}
