using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();

    private void OnDrawGizmosSelected()
    {
        if (waypoints.Count > 1)
        {
            Vector3 prev = waypoints[0].position;
            for (int i = 1; i < waypoints.Count; i++)
            {
                Vector3 next = waypoints[i].position;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
            Gizmos.DrawLine(prev, waypoints[0].position);
        }
    }
}
