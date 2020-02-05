using System.Collections.Generic;

public class AnnotationBoxContainerBehaviour : IWidget {
    public override void initWidget()
    {
        GetComponent<IDataProvider>().project = project;
    }

    public override void updateWidgets()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        List<Annotation> annotations = GetComponent<IDataProvider>().GetData() as List<Annotation>;
        foreach (Annotation a in annotations)
        {
            //Create GUI
            IWidget annotationInfoBox = Instantiate(widgets[0]);
            annotationInfoBox.gameObject.SetActive(true);
            annotationInfoBox.gameObject.name = a._id;

            annotationInfoBox.GetComponent<AnnotationBoxBehaviour>().SetPosition(a.localPosition);
            annotationInfoBox.GetComponent<AnnotationBoxBehaviour>().SetHeader(a._id);
            annotationInfoBox.GetComponent<AnnotationBoxBehaviour>().SetContent(a.description);

            addWidget(annotationInfoBox);
        }
    }

    public void highlightWidget(IWidget widget,bool state)
    {
        for(int i=0;i<transform.childCount;i++)
        {
            if(widget.gameObject.name==transform.GetChild(i).gameObject.name)
            {
                transform.GetChild(i).gameObject.SetActive(state);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(!state);
            }
        }
    }

    public void clearHighlight()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update () {
    }
}
