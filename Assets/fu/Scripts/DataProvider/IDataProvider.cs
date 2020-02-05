using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public abstract class IDataProvider : MonoBehaviour {
    public TextAsset query;
    public Project project;
    protected List<Annotation> annotations;

    // Use this for initialization
    void Start () {
        registerQueries();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void registerQueries()
    {
        if (query != null)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(query.text);
            XmlNodeList queries = document.GetElementsByTagName("query");
            foreach (XmlNode q in queries)
            {
                string id = "";
                string service = "";
                string query = "";
                XmlAttributeCollection attrs = q.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    if (attr.Name == "service")
                    {
                        service = attr.Value;
                    }
                }
                query = q.FirstChild.Value;
                if (project.ids.TryGetValue(service, out id))
                {
                    query = query.Replace("%id", id);
                }
                IWidget widget = GetComponent<IWidget>();
                GameObject.Find("Managers").GetComponent<DataServiceManager>().registerWidget(this, service, query);
                GameObject.Find("Managers").GetComponent<DataServiceManager>().LoadAnnotations(this);
            }
        }
    }

    public void setAnnotations(List<Annotation> annotations)
    {
        this.annotations = annotations;
        GetComponent<IWidget>().updateWidgets();
    }

    public void addAnnotations(List<Annotation> annotations)
    {
        this.annotations = this.annotations.Concat(annotations).ToList();
        GetComponent<IWidget>().updateWidgets();
    }

    public void addAnnotation(Annotation annotation)
    {
        annotations.Add(annotation);
        GetComponent<IWidget>().updateWidgets();
    }

    public void updateAnnotation(string id, string text)
    {
        GetComponent<IWidget>().updateWidgets();
    }

    public void deleteAnnotation(string id)
    {
        //annotations.Remove(id);
        annotations = annotations.Where(x => { return x._id != id; }).ToList();
        GetComponent<IWidget>().updateWidgets();
    }

    public abstract object GetData();
}
