using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class test : MonoBehaviour
{
    public Circuit circuit;
    NavMeshAgent agent;
    int currentTrackerWP;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(agent.transform.position, circuit.waypoints[currentTrackerWP].position) < 4)
        {
            currentTrackerWP++;
            if (currentTrackerWP >= circuit.waypoints.Count)
                currentTrackerWP = 0;
        }
        agent.SetDestination(circuit.waypoints[currentTrackerWP].position);
    }
}
