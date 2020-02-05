using System.Collections.Generic;
using System;
using UnityEngine;

public class GameObjectManager : MonoBehaviour {
    public GameObject annotatedObjectPrefab;
    public List<IDataService> dataServices;

    private String focusedWidget = "";

    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    public void SpawnObject(Project project, Vector3 offset, float scale = 1.0f)
    {
        
        GameObject obj = Instantiate(annotatedObjectPrefab);
        GameObject parent = GameObject.Find("Holograms");
        obj.GetComponent<AnnotatedObject>().project = project;
        obj.name = project.name;
        obj.transform.SetParent(parent.transform);
        
        DataServiceManager dataServices = GameObject.Find("Managers").GetComponent<DataServiceManager>();
        dataServices.SpawnObject(project,offset, scale);
    }

    internal void setFocus(GameObject widget)
    {
        if (widget == null)
        {
            if (focusedWidget != "")
            {
                getFocus().OnVisionLeave();
            }
            focusedWidget = "";
            GetComponent<GestureManager>().ClearGestures();
            GetComponent<SpeechManager>().ClearKeywords();

        }
        else
        {
            while (widget != null)
            {
                IWidget focus = widget.GetComponent<IWidget>();
                if (focus != null && focusedWidget != GetGameObjectPath(widget))
                {
                    if (focusedWidget != "")
                    {
                        getFocus().OnVisionLeave();
                    }
                    focusedWidget = GetGameObjectPath(widget);
                    if (focusedWidget != "")
                    {
                        getFocus().OnVisionEntered();
                        GetComponent<GestureManager>().ChangeAcceptedGestures(getFocus().acceptedGestures);
                        GetComponent<SpeechManager>().ChangeKeyWords(getFocus().keywords);
                    }

                    break;
                }
                else if(focusedWidget!="" && focusedWidget == GetGameObjectPath(widget))
                {
                    break;
                }
                if (widget.transform.parent != null)
                {
                    widget = widget.transform.parent.gameObject;
                }
                else
                {
                    if (focusedWidget != "")
                    {
                        getFocus().OnVisionLeave();
                    }
                    focusedWidget = "";
                    GetComponent<GestureManager>().ClearGestures();
                    GetComponent<SpeechManager>().ClearKeywords();
                    return;
                }
            }
        }
    }

    internal IWidget getFocus()
    {
        GameObject obj = GameObject.Find(focusedWidget);
        if(obj == null)
        {
            return null;
        }
        IWidget ret = GameObject.Find(focusedWidget).GetComponent<IWidget>();
        return ret;
    }

    public void DeleteObject(String name)
    {
        Debug.Log("Delete " + name);
        if (GameObject.Find("Holograms/" + name) != null)
        {
            Destroy(GameObject.Find("Holograms/" + name));
        }
    }

    //Reset all loaded Objects
    public void Clear()
    {
        GameObject holograms = GameObject.Find("Holograms");
        for(int i=0;i<holograms.transform.childCount;i++)
        {
            Destroy(holograms.transform.GetChild(i));
        }
    }

    //Check whether an object with name already is loaded
    public bool ObjectExists(String name)
    {
        return GameObject.Find("Holograms/" + name) != null;
    }

    //Get an Object from the Manager by its name
    public GameObject GetObject(String name)
    {
        return GameObject.Find("Holograms/" + name);
    }

    public void UpdateAnnotedObject(AnnotatedObject obj)
    {
        if (GameObject.Find("Holograms/" + name) != null)
        {
            Debug.Log("GameObjectManager");
        }
    }

    // Use this for initialization
    void Start () {
    }


    // Update is called once per frame
    void Update () {
		
	}
}
