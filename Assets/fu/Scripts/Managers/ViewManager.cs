using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ViewManager : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        EventSystem guiInfo = EventSystem.current;
        if (guiInfo.IsPointerOverGameObject())
        {
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            PointerEventData pointerEventData = new PointerEventData(guiInfo);
            pointerEventData.position = new Vector2(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
            guiInfo.RaycastAll(pointerEventData, raycastResults);
            GameObject focus = null;
            foreach (RaycastResult r in raycastResults)
            {
                focus = r.gameObject;
                transform.position = r.worldPosition;
                transform.rotation = Quaternion.FromToRotation(Vector3.up, r.worldNormal);
                break;
            }

            GameObject.Find("Managers").GetComponent<GameObjectManager>().setFocus(focus);
        }
        else if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            GameObject focused = hitInfo.collider.gameObject;
            if (focused != null)
            {
                //GameObject.Find("Managers").GetComponent<GameObjectManager>().setFocus(focused);
            }
            else
            {
                //GameObject.Find("Managers").GetComponent<GameObjectManager>().setFocus(null);
            }
            transform.position = hitInfo.point;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
        else
        {
            GameObject.Find("Managers").GetComponent<GameObjectManager>().setFocus(null);
        }
    }
}
