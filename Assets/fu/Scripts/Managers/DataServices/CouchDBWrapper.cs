using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class ProjectInfoJson
{
    public String _id;
    public String _rev;
    public ProjectJson[] projects;
}

[Serializable]
public class ProjectJson
{
    public String _id;
    public String name;
    public String activeTopic;
}

[Serializable]
public class AnnotationReponseValueJson
{
    public string id;
    public string key;
    public Annotation doc;
}

[Serializable]
public class AnnotationResponseJson
{
    public int total_rows;
    public int offset;
    public AnnotationReponseValueJson[] rows;
}

[Serializable]
public class TopicResponseJson
{
    public string _id;
    public string _rev;
    public string fileName;
    public string fileEnding;
    public AttachmentResponseJson[] _attachments;
}

[Serializable]
public class FileResponseJson
{
    public string content_type;
    public int revpos;
    public string digest;
    public int length;
    public bool stub;
}

[Serializable]
public class AttachmentResponseJson
{
    public FileResponseJson file;
}

/**
 *Class that Wraps the communication of the ProcessAnnotator's CouchDB  
 */
public class CouchDBWrapper : IDataService {
    public string username;
    public string password;
    public string url;

    public ProjectInfoJson projects;

    //Create a Get Request for a specified uri
    private UnityWebRequest CreateGetRequest(string uri,bool data=false)
    {
        string authorization = username + ":" + password;
        byte[] binaryAuthorization = System.Text.Encoding.UTF8.GetBytes(authorization);
        authorization = Convert.ToBase64String(binaryAuthorization);
        UnityWebRequest request = UnityWebRequest.Get(url+uri);
        request.chunkedTransfer = false;
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("AUTHORIZATION", "Basic " + authorization);
        if(data)
        {
            request.SetRequestHeader("Accept", "multipart/related");
        }
        return request;
        /*while(!webop.isDone)
        {
        }
        // Show results as text
        String text = System.Text.Encoding.ASCII.GetString(((DownloadHandlerBuffer)webop.webRequest.downloadHandler).data);
        Debug.Log(text);
        return text;*/
    }

    public override void LoadObject(Project project,float scale,Vector3 offset)
    {
        //string text = GetRequest("/" + id + "/topic_/file");
        StartCoroutine(DownloadFile(project, scale, offset));
    }

    public override void LoadAnnotations(IDataProvider provider, String query)
    {
        StartCoroutine(DownloadAnnotations(provider, query));
    }

    //Download a ModelFile from the CouchDB specified by id and name
    private IEnumerator DownloadFile(Project project, float scale,Vector3 offset)
    {
        //string text = GetRequest("/" + id + "/topic_/file");
        string id;
        if (project.ids.TryGetValue(serviceName, out id))
        {
            Debug.Log("/" + id + "/topic_/file/");
            UnityWebRequest webop = CreateGetRequest("/" + id + "/topic_/file", true);

            yield return webop.SendWebRequest();
            if (webop.isNetworkError && webop.responseCode != 200L)
            {
                Debug.Log(webop.error);
            }
            else
            {
                string text = webop.downloadHandler.text;
                Debug.Log(text.Length);
                ObjImporter importer = new ObjImporter();
                GameObject obj = GameObject.Find("Holograms/" + name).gameObject;
                obj.transform.localScale = new Vector3(scale, scale, scale);
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
                obj.transform.localPosition = offset + new Vector3(0.0f, scale * obj.GetComponent<MeshFilter>().mesh.bounds.extents.y, 0.0f);

                obj.GetComponent<MeshCollider>().sharedMesh = null;
                obj.GetComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().mesh;
                obj.GetComponent<MeshCollider>().convex = false;
                obj.GetComponent<MeshCollider>().enabled = true;
                obj.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/defaultMat");
                obj.GetComponent<Renderer>().enabled = true;
                obj.SetActive(true);
            }
        }
    }

    //Download Annotations for an object in the CouchDB specified by ID and name
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
            AnnotationResponseJson resp = JsonUtility.FromJson<AnnotationResponseJson>(text);
            List<Annotation> annotations = new List<Annotation>();
            foreach (AnnotationReponseValueJson r in resp.rows)
            {
                if (r.doc._id != "info" && r.doc._id != "topic_")
                {
                    r.doc.service = this;
                    annotations.Add(r.doc);
                }
            }
            Action<List<Annotation>> callback;
            if (annotationCallback.TryGetValue(provider, out callback))
            {
                callback.Invoke(annotations);
            }
        }
    }

    public override void InitService()
    {
        serviceName = "CouchDB";
    }

    // Update is called once per frame
    public override void UpdateService () {
		
	}
}
