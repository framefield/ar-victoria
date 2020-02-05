using UnityEngine;

public class PivotGizmo : MonoBehaviour {

    public float gizmoSize = 0.25f;
    public Color gizmoColor = Color.yellow;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
