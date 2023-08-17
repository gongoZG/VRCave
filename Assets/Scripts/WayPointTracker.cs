using UnityEngine;

public class WayPointTracker : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    private WayPointPath circuit;
    private AltCarController ACAI;

    private float lookAheadForTargetOffset = 5;
    private float lookAheadForTargetFactor = .1f;
    private float lookAheadForSpeedOffset = 10;
    private float lookAheadForSpeedFactor = .2f;
    private float pointToPointThreshold = 4;

    private float progressDistance;
    private int progressNum;
    private Vector3 lastPosition;
    private float speed;


    #region KEY POINTS

    [HideInInspector]
    public Transform[] pathTransform;

    [HideInInspector]
    public WayPointPath.RoutePoint targetPoint { get; private set; }
    [HideInInspector]
    public WayPointPath.RoutePoint speedPoint { get; private set; }
    [HideInInspector]
    public WayPointPath.RoutePoint progressPoint { get; private set; }

    #endregion

    private void Start()
    {
        #region GET ACAI VALUES

        ACAI = this.GetComponent<AltCarController>();
        circuit = ACAI.AICircuit;
        lookAheadForTargetOffset = ACAI.lookAheadForTarget;
        lookAheadForTargetFactor = ACAI.lookAheadForTargetFactor;
        lookAheadForSpeedOffset = ACAI.lookAheadForSpeedOffset;
        lookAheadForSpeedFactor = ACAI.lookAheadForSpeedFactor;
        pointToPointThreshold = ACAI.pointThreshold;

        #endregion

        if (target == null)
        {
            target = ACAI.carAItarget;
        }

        Reset();
    }

    public void Reset()
    {
        progressDistance = 0;
        progressNum = 0;
    }


    private void Update()
    {
        FollowPath();
    }  

    #region FOLLOW PATH

    public void FollowPath()    {

        if (Time.deltaTime > 0)
        {
            speed = Mathf.Lerp(speed, (lastPosition - transform.position).magnitude / Time.deltaTime, Time.deltaTime);
        }
        target.position = circuit.GetRoutePoint(progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor * speed).position;
        target.rotation = Quaternion.LookRotation(circuit.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor * speed).direction);

        progressPoint = circuit.GetRoutePoint(progressDistance);
        Vector3 progressDelta = progressPoint.position - transform.position;
        if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
        {
            progressDistance += progressDelta.magnitude * 0.5f;
        }

        lastPosition = transform.position;
    }

    #endregion

    #region DRAW DIRECTION

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        { 
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawWireSphere(circuit.GetRoutePosition(progressDistance), 1);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(target.position, target.position + target.forward);
        }
    }

    #endregion
}
