using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    private AltCarController carController;
    public float steeringSensitivity = 0.01f;
    public float breakingSensitivity = 1.0f;
    public float accelerationSensitivity = 0.3f;    
    public GameObject trackerPrefab;
    NavMeshAgent agent;

    int currentTrackerWP;
    float lookAhead = 100;

    float lastTimeMoving = 0;

    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<AltCarController>();
        GameObject tracker = Instantiate(trackerPrefab, new Vector3(carController.transform.position.x, carController.transform.position.y, carController.transform.position.z + 5), carController.transform.rotation) as GameObject;
        agent = tracker.GetComponent<NavMeshAgent>();
    }

    void ProgressTracker()
    {
        
        if (Vector3.Distance(agent.transform.position, carController.transform.position) > lookAhead)
        {
            agent.isStopped = true;
            return;
        }
        else
        {
            agent.isStopped = false;
        }
        
        agent.SetDestination(circuit.waypoints[currentTrackerWP].position);

        if (Vector3.Distance(agent.transform.position, circuit.waypoints[currentTrackerWP].position) < 4)
        {
            currentTrackerWP++;
            if (currentTrackerWP >= circuit.waypoints.Count)
                currentTrackerWP = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(agent.isStopped);
        ProgressTracker();
        
        Vector3 localTarget;
        float targetAngle;

        localTarget = carController.transform.InverseTransformPoint(agent.transform.position);
        targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        


        //carController.Throttle = throttle;
        //carController.Steering = steer;
        //carController.Handbrake = brake;
        //carController.SpotLight = spotLight;
        
    }
}
