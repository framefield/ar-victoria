using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;

public class DBPediaWrapper : IDataService {

    private UnityWebRequest CreateGetRequest(string query, bool data = false)
    {
        UnityWebRequest request = UnityWebRequest.Get("https://dbpedia.org/sparql?default-graph-uri=http://dbpedia.org&query=" + query);
        request.chunkedTransfer = false;
        request.downloadHandler = new DownloadHandlerBuffer();
        if (data)
        {
            request.SetRequestHeader("Accept", "multipart/related");
        }
        return request;
    }


    public override void LoadObject(Project project, float scale, Vector3 offset)
    {
    }

    public override void LoadAnnotations(IDataProvider provider, String query)
    {
        StartCoroutine(DownloadAnnotations(provider, query));
    }


    private IEnumerator DownloadAnnotations(IDataProvider provider, String query)
    {
        UnityWebRequest webop = CreateGetRequest(query);

        yield return webop.SendWebRequest();
        if (webop.isNetworkError && webop.responseCode != 200L)
        {
            Debug.Log(webop.error);
        }
        else
        {
            string text = webop.downloadHandler.text;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);
            List<Annotation> annotations = new List<Annotation>();
            XmlNodeList results = doc.GetElementsByTagName("result");
            foreach (XmlNode node in results)
            {
                Annotation annotation = new Annotation();
                annotation.service = this;
                annotation.creationDate = DateTime.Today.ToString();
                annotation.localPosition = UnityEngine.Random.onUnitSphere * 200;
                annotation.localPosition.y = Math.Abs(annotation.localPosition.y);
                XmlNodeList bindings = node.ChildNodes;
                foreach (XmlNode binding in bindings)
                {
                    XmlAttributeCollection attrs = binding.Attributes;
                    foreach (XmlAttribute attr in attrs)
                    {
                        if (attr.Name == "name" && attr.Value == "label")
                        {
                            annotation._id = binding.FirstChild.FirstChild.Value;
                        }
                        else if (attr.Name == "name" && attr.Value == "value")
                        {
                            annotation.description = binding.FirstChild.FirstChild.Value;
                        }
                    }
                }
            }
            Action<List<Annotation>> callback;
            if (annotationCallback.TryGetValue(provider, out callback))
            {
                callback.Invoke(annotations);
            }
        }
    }

    // Use this for initialization
    public override void InitService () {
        serviceName = "DBPedia";
	}
	
	// Update is called once per frame
	public override void UpdateService () {
		
	}
}
