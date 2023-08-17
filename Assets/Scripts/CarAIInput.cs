using UnityEngine;
using Random = UnityEngine.Random;

public class CarAIInput : MonoBehaviour
{
    private AltCarController carAIReference;

    #region UTILITY

    private float randomValue;
    private float avoidOtherCarTime;
    private float avoidOtherCarSlowdown;
    private float avoidPathOffset;
    
    public bool reverseGearOn = false;
    
    private bool avoidingObstacle = false;
    private bool isBraking = false;

    private float avoidObstacleMultiplier;

    private float targetAngle;

    #endregion

    private void Awake()
    {
        #region REFERENCES

        carAIReference = GetComponent<AltCarController>();

        #endregion

        randomValue = Random.value * 100;
    }


    private void FixedUpdate()
    {
        //Debug.Log(carAIReference.isDriving);
        if (carAIReference.carAItarget == null)// || !carAIReference.isDriving)
        {
            Debug.Log("?");
            carAIReference.Move(0, 0, -1f, 1f);
        }
        else
        {
            
            #region MAX SPEED

            Vector3 fwd = transform.forward;
            if (carAIReference.rb.velocity.magnitude > carAIReference.maxSpeed * 0.1f)
            {
                fwd = carAIReference.rb.velocity;
            }

            float desiredSpeed = carAIReference.maxSpeed;

            #endregion
            /*
           #region Brake
           Vector3 delta = carAIReference.carAItarget.position - transform.position;
           float distanceCautiousFactor = Mathf.InverseLerp(carAIReference.cautiousDistance, 0, delta.magnitude);
           float spinningAngle = carAIReference.rb.angularVelocity.magnitude * carAIReference.cautiousAngularVelocityFactor;
           float cautiousnessRequired = Mathf.Max(Mathf.InverseLerp(0, carAIReference.cautiousAngle, spinningAngle), distanceCautiousFactor);
           desiredSpeed = Mathf.Lerp(carAIReference.maxSpeed, carAIReference.maxSpeed * carAIReference.cautiousSpeedFactor, cautiousnessRequired);
           #endregion Brake
            */

            #region EVASIVE ACTION

            Vector3 offsetTargetPos = carAIReference.carAItarget.position;

            if (Time.time < avoidOtherCarTime)
            {
                desiredSpeed *= avoidOtherCarSlowdown;
                offsetTargetPos += carAIReference.carAItarget.right * avoidPathOffset;
            }
            else
            {
                offsetTargetPos += carAIReference.carAItarget.right * (Mathf.PerlinNoise(Time.time * carAIReference.lateralWanderSpeed, randomValue) * 2 - 1) * carAIReference.lateralWander;
            }

            #endregion
            

            #region SENSITIVITY
            
            float accelBrakeSensitivity = (desiredSpeed < carAIReference.currentSpeed)
                                              ? carAIReference.brakeSensitivity
                                              : carAIReference.accelSensitivity;            

            float accel = Mathf.Clamp((desiredSpeed - carAIReference.currentSpeed) * accelBrakeSensitivity, -1, 1);           
            
            //Debug.Log(accel);
            #endregion

            #region STEER

            accel *= carAIReference.wanderAmount + (Mathf.PerlinNoise(Time.time * carAIReference.accelWanderSpeed, randomValue) * carAIReference.wanderAmount);
            //Debug.Log(accel);
            Vector3 localTarget;

            localTarget = transform.InverseTransformPoint(offsetTargetPos);

            if (avoidingObstacle)
            {
                targetAngle = carAIReference.maximumSteerAngle * avoidObstacleMultiplier;
            }
            else
            {
                targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            }

            float steer = Mathf.Clamp(targetAngle * carAIReference.steerSensitivity, -1, 1) * Mathf.Sign(carAIReference.currentSpeed);

            #endregion

            #region MOVE CAR

            if (isBraking)
            {
                Debug.Log("B");
                if (Vector3.Dot(carAIReference.rb.velocity, transform.forward) > 0)
                {
                    carAIReference.Move(steer, 0f, 0f, 1f);
                }
            }
            else if (reverseGearOn)
            {
                Debug.Log("R");
                carAIReference.Move(-steer, -1f, -1f, 0f);
            }
            else
            {
                //Debug.Log(accel);
                carAIReference.Move(steer, accel, accel, 0);
            }

            #endregion
        }
    }


    #region COLLISION WITH OTHER CARS

    private void OnCollisionStay(Collision col)
    {
        if (col.rigidbody != null)
        {
            var otherAI = col.rigidbody.GetComponent<CarAIInput>();
            if (otherAI != null)
            {
                WaitforCar(otherAI.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "WaitZone")
        {
            isBraking = true;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "WaitZone")
        {
            isBraking = false;
        }
    }


    private void WaitforCar(GameObject gameobject)
    {
        avoidOtherCarTime = Time.time + 1;

        if (Vector3.Angle(transform.forward, gameobject.transform.position - transform.position) < 90)
        {
            avoidOtherCarSlowdown = 0.5f;
        }
        else
        {
            avoidOtherCarSlowdown = 1;
        }

        var otherCarLocalDelta = transform.InverseTransformPoint(gameobject.transform.position);
        float otherCarAngle = Mathf.Atan2(otherCarLocalDelta.x, otherCarLocalDelta.z);
        avoidPathOffset = carAIReference.lateralWander * -Mathf.Sign(otherCarAngle);
    }

    #endregion

    public void SetTarget(Transform target)
    {
        carAIReference.carAItarget = target;
        carAIReference.isDriving = true;
    }
}
