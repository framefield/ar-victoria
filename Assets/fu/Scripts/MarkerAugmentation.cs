using UnityEngine;

public class MarkerAugmentation : MonoBehaviour {
    public Project project;
    public Vector3[,] trackerPos = new Vector3[2, 2];
    public bool[,] trackerState = new bool[2,2] {{false,false},{false,false}};
    public string trackedObject = "";

	// Update is called once per frame
	void Update () {
        if ((trackerState[0, 0] && trackerState[1, 1]))
        {
            if (!GameObject.Find("Managers").GetComponent<GameObjectManager>().ObjectExists(trackedObject))
            {
                GameObject.Find("Managers").GetComponent<GameObjectManager>().SpawnObject(project, trackerPos[0, 0] + 0.5f * (trackerPos[1, 1] - trackerPos[0, 0]), 0.01f);
//                VuforiaBehaviour.Instance.enabled = false;
            }
        }
        else if(trackerState[0, 1] && trackerState[1, 0])
        {
            if (!GameObject.Find("Managers").GetComponent<GameObjectManager>().ObjectExists(trackedObject))
            {
                GameObject.Find("Managers").GetComponent<GameObjectManager>().SpawnObject(project, trackerPos[0, 1] + 0.5f * (trackerPos[1, 0] - trackerPos[0, 1]), 0.01f);
//                VuforiaBehaviour.Instance.enabled = false;
            }
        }
        else
        {
            //GameObject.Find("Holograms").GetComponent<GameObjectManager>().DeleteObject(trackedObject);
            //target.SetActiveRecursively(false);
        }
    }
}
