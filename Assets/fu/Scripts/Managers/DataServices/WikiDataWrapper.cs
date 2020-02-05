using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;

public class WikiDataWrapper : IDataService {

    private UnityWebRequest CreateGetRequest(string query, bool data = false)
    {
        UnityWebRequest request = UnityWebRequest.Get("https://query.wikidata.org/sparql?query=" + query);
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
        StartCoroutine(DownloadFile(project, scale, offset));
    }

    public override void LoadAnnotations(IDataProvider provider, String query)
    {
        StartCoroutine(DownloadAnnotations(provider, query));
    }

    private IEnumerator DownloadFile(Project project, float scale, Vector3 offset)
    {
        string id;
        if (project.ids.TryGetValue(serviceName, out id))
        {

            string query = @"SELECT ?model ?modelLabel
                            WHERE
                            {
                                wd:" + id + @" wdt:P4896 ?model.
                                SERVICE wikibase:label { bd:serviceParam wikibase:language 'en'.}
                            }";

            UnityWebRequest webop = CreateGetRequest(query, false);

            yield return webop.SendWebRequest();
            if (webop.isNetworkError && webop.responseCode != 200L)
            {
                Debug.Log(webop.error);
            }
            else
            {
                string text = webop.downloadHandler.text;
                Debug.Log(text.Length);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(text);
                XmlNodeList results = doc.GetElementsByTagName("result");
                string url = "";
                foreach (XmlNode node in results)
                {
                    XmlNodeList bindings = node.ChildNodes;
                    foreach (XmlNode binding in bindings)
                    {
                        XmlAttributeCollection attrs = binding.Attributes;
                        foreach (XmlAttribute attr in attrs)
                        {
                            if (attr.Name == "name" && attr.Value == "model")
                            {
                                url = binding.FirstChild.FirstChild.Value;
                            }
                        }
                    }
                }

                StartCoroutine(DownloadObject(url, project.name, scale, offset));
            }
        }
    }

    private IEnumerator DownloadObject(string url, string name, float scale, Vector3 offset)
    {
        UnityWebRequest webop = UnityWebRequest.Get(url);
        webop.chunkedTransfer = false;
        webop.SetRequestHeader("Accept", "multipart/related");
        webop.downloadHandler = new DownloadHandlerBuffer();
        yield return webop.SendWebRequest();
        if (webop.isNetworkError && webop.responseCode != 200L)
        {
            Debug.Log(webop.error);
        }
        else
        {
            byte[] text = webop.downloadHandler.data;
            StlImporter importer = new StlImporter();
            GameObject obj = GameObject.Find("Holograms/" + name).gameObject;
            obj.transform.localScale = new Vector3(scale, scale, scale);
            obj.transform.localPosition = offset;
            obj.name = name;
            obj.layer = 0;
            obj.AddComponent<MeshRenderer>();
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshCollider>();
            obj.GetComponent<MeshFilter>().mesh = importer.ImportFile(text);
            obj.GetComponent<MeshFilter>().sharedMesh = obj.GetComponent<MeshFilter>().mesh;
            obj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            obj.GetComponent<MeshFilter>().mesh.RecalculateTangents();
            obj.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            obj.GetComponent<MeshCollider>().sharedMesh = null;
            obj.GetComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().mesh;
            obj.GetComponent<MeshCollider>().convex = false;
            obj.GetComponent<MeshCollider>().enabled = true;
            obj.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/defaultMat");
            obj.GetComponent<Renderer>().enabled = true;
            obj.SetActive(true);
        }
    }

    private IEnumerator DownloadAnnotations(IDataProvider provider, String query)
    {
        UnityWebRequest webop = CreateGetRequest(query.Replace("\n"," "));

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
                annotation.localPosition = UnityEngine.Random.onUnitSphere * 20f;
                annotation.localPosition.y = Math.Abs(annotation.localPosition.y);
                XmlNodeList bindings = node.ChildNodes;
                foreach (XmlNode binding in bindings)
                {
                    XmlAttributeCollection attrs = binding.Attributes;
                    foreach (XmlAttribute attr in attrs)
                    {
                        if (attr.Name == "name" && attr.Value == "wdLabel")
                        {
                            annotation._id = binding.FirstChild.FirstChild.Value;
                        }
                        else if (attr.Name == "name" && attr.Value == "ps_Label")
                        {
                            annotation.description = binding.FirstChild.FirstChild.Value;
                        }
                    }
                }
                annotations.Add(annotation);
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
        serviceName = "WikiData";
	}
	
	// Update is called once per frame
	public override void UpdateService () {
		
	}
}
