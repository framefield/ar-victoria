using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

public class AnnotationBoxBehaviour : IWidget
{
    private Vector3 annPos;

    // Use this for initialization
    public override void initWidget()
    {
        //transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        transform.localPosition = transform.parent.localPosition;
        transform.Find("InfoBox").gameObject.SetActive(true);
        transform.Find("InfoBox").transform.SetParent(transform);
        transform.Find("InfoBox").GetComponent<RectTransform>().SetParent(transform);
        transform.Find("InfoBox").GetComponent<RectTransform>().localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.Find("InfoBox").GetComponent<RectTransform>().localPosition = 1.5f * annPos;

        transform.Find("InfoBox/Content").gameObject.SetActive(true);
        transform.Find("InfoBox/Content/ContentText").gameObject.SetActive(true);
        transform.Find("InfoBox/Header").gameObject.SetActive(true);
        transform.Find("InfoBox/Header/HeaderText").gameObject.SetActive(true);

    }

    public override void updateWidgets()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 annPosWorld = transform.position;
        RaycastHit hitInfo;

        if (Physics.Raycast(cameraPos, annPosWorld - cameraPos, out hitInfo))
        {
            Vector3 forwardCamera = Camera.main.transform.forward;
            Vector3 forwardGui = -1.0f * transform.forward;
            Vector3 axis = -Camera.main.transform.position;
            //GetComponent<RectTransform>().rotation = Camera.main.transform.rotation;
            transform.Find("InfoBox").GetComponent<RectTransform>().localRotation = Quaternion.LookRotation(axis);
            //transform.Find("InfoBox/Header").gameObject.transform.localRotation = Quaternion.LookRotation(axis);
            //GetComponent<RectTransform>().localRotation = Quaternion.LookRotation(axis);

            //Fade Objects
            GameObject line = transform.Find("Line").gameObject;
            GameObject header = transform.Find("InfoBox/Header").gameObject;
            GameObject headerText = transform.Find("InfoBox/Header/HeaderText").gameObject;
            GameObject body = transform.Find("InfoBox/Content").gameObject;
            GameObject bodyText = transform.Find("InfoBox/Content/ContentText").gameObject;

            Color c = header.GetComponent<Image>().color;
            c.a = 0.3f / Vector3.Distance(cameraPos, annPosWorld);
            header.GetComponent<Image>().color = c;
            c = body.GetComponent<Image>().color;
            c.a = 0.3f / Vector3.Distance(cameraPos, annPosWorld);
            body.GetComponent<Image>().color = c;
            c = headerText.GetComponent<Text>().color;
            c.a = 0.3f / Vector3.Distance(cameraPos, annPosWorld);
            headerText.GetComponent<Text>().color = c;
            c = bodyText.GetComponent<Text>().color;
            c.a = 0.3f / Vector3.Distance(cameraPos, annPosWorld);
            bodyText.GetComponent<Text>().color = c;
            c = line.GetComponent<LineRenderer>().startColor;
            c.a = 0.3f / Vector3.Distance(cameraPos, annPosWorld);
            c = line.GetComponent<LineRenderer>().startColor = c;
            c = line.GetComponent<LineRenderer>().endColor;
            c.a = 0.3f / Vector3.Distance(cameraPos, annPosWorld);
            c = line.GetComponent<LineRenderer>().endColor = c;

            if (Math.Abs(Vector3.Distance(cameraPos, hitInfo.point) - Vector3.Distance(cameraPos, annPosWorld)) < 0.0001f)
            {
                //GetComponent<RectTransform>().Rotate(axis, angle);
                GetComponent<Canvas>().enabled = true;
                transform.Find("Line").GetComponent<LineRenderer>().enabled = true;
            }
            else
            {
                //GetComponent<Canvas>().enabled = false;
                //transform.Find("Line").GetComponent<LineRenderer>().enabled = false;
            }
        }
    }

    public override void OnVisionEntered()
    {
        gameObject.transform.parent.GetComponent<AnnotationBoxContainerBehaviour>().highlightWidget(this, true);
    }

    public override void OnVisionLeave()
    {
        gameObject.transform.parent.GetComponent<AnnotationBoxContainerBehaviour>().clearHighlight();
    }

    public override void OnTapped(TappedEventArgs args)
    {
        transform.Find("InfoBox/Content").gameObject.SetActive(false);
    }

    public void SetPosition(Vector3 annPos)
    {
        this.annPos = annPos;
        transform.Find("Line").GetComponent<LineRenderer>().widthMultiplier = 0.01f;
        transform.Find("Line").GetComponent<LineRenderer>().SetPosition(0, annPos);
        transform.Find("Line").GetComponent<LineRenderer>().SetPosition(1, 1.5f * annPos);
        transform.Find("Line").GetComponent<LineRenderer>().enabled = true;

        transform.Find("InfoBox").GetComponent<RectTransform>().localPosition = 1.5f * annPos;

    }

    public void SetHeader(string text)
    {
        GameObject header = transform.Find("InfoBox/Header/HeaderText").gameObject;
        Text t = header.GetComponent<Text>();
        t.text = text;
    }

    public void SetContent(string text)
    {
        GameObject header = transform.Find("InfoBox/Content/ContentText").gameObject;
        Text t = header.GetComponent<Text>();
        t.text = text;
    }
}
