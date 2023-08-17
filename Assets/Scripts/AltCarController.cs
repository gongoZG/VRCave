using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AltCarController : MonoBehaviour
{
    [SerializeField] bool isPlayer = false;

    
    public WayPointPath AICircuit;
    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public Transform carAItarget;
    [HideInInspector] private GameObject carAItargetObj;
    [HideInInspector] public WayPointTracker wayPointTracker; 
    [HideInInspector] public CarAIInput carAIInput;

    public LightManager lightManager;
    public List<WheelCollider> throttleWheels;
    public List<GameObject> steeringWheels;
    public List<GameObject> leftWheelMeshes;
    public List<GameObject> rightWheelMeshes;
    public Transform CM;
    public Rigidbody rb;
    public List<GameObject> brakeLights;

    
    #region CUSTOM CONTROLS

    public AnimationCurve enginePower;

    public float maximumSteerAngle;
    [Range(0, 0.5f)] public float tractionControl;
    [Range(0, 0.5f)] public float slipLimit = 0.3f;

    public Vector3 centerOfMass;
    public float vehicleMass = 1000f;
    public float motorTorque = 6000f;
    public float brakeTorque = 20000f;
    public float reverseTorque = 500f;
    public float handbrakeTorque = 10000000f;
    public float maxSpeed = 200f;

    public int numberOfGears = 5;
    public float downForce = 300f;

    public bool ABS = true;
    #endregion

    #region INPUTS
    [SerializeField] [Range(0, 0.1f)] public float accelSensitivity = 0.05f;
    [SerializeField] [Range(0, 1)] public float wanderAmount = 0.3f;

    [SerializeField] [Range(0, 0.1f)] public float steerSensitivity = 0.04f;
    [SerializeField] [Range(0, 10)] public float lateralWander = 2.56f;

    [SerializeField] [Range(0, 180)] public float cautiousAngle = 30f;
    [SerializeField] [Range(0, 200)] public float cautiousDistance = 100f;

    [SerializeField] [Range(0, 1)] public float cautiousSpeedFactor = 0.5f;
    [SerializeField] [Range(0, 1)] public float brakeSensitivity = 1f;  
    
    
    [SerializeField] public float cautiousAngularVelocityFactor = 30f;
    [SerializeField] public float lateralWanderSpeed = 0.5f;
    
    [SerializeField] public float accelWanderSpeed = 0.1f;
    [SerializeField] public bool isDriving= true;

    public float AccelInput { get; private set; }
    public float BrakeInput { get; private set; }

    #endregion

    #region UTILITY

    public float currentSpeed;
    private float currentTorque;
    private float rpmRange = 1f;
    public int currentGear;
    private float gearFactor;
    public bool reverseGearOn;
    public float RPM { get; private set; }

    #endregion

    #region AI
    public bool IsPlayer
    {
        get => isPlayer;
        set => isPlayer = value;
    }

    float steering;
    public float Steering
    {
        get => steering;
        set => steering = Mathf.Clamp(value, -1f, 1f);

    }

    float accel;
    public float Acceleration
    {
        get => accel;
        set => accel = value;
    }

    [SerializeField] float handbrake;
    public float Handbrake
    {
        get => handbrake;
        set => handbrake = value;
    }

    [SerializeField] float footbrake;
    public float Footbrake
    {
        get => footbrake;
        set => footbrake = value;
    }

    [SerializeField] bool spotLight;
    public bool SpotLight
    {
        get => spotLight;
        set => spotLight = value;
    }
    #endregion AI

    #region PROGRESS TRACKER        
    [SerializeField] [Range(5, 50)] public float lookAheadForTarget = 5;
    [SerializeField] public float lookAheadForTargetFactor = .1f;
    [SerializeField] public float lookAheadForSpeedOffset = 10;
    [SerializeField] public float lookAheadForSpeedFactor = .2f;
    [SerializeField] [Range(1, 10)] public float pointThreshold = 4;
    public Transform AItarget;

    #endregion

    void Start()
    { 
        carAIInput = gameObject.AddComponent<CarAIInput>();
        wayPointTracker = gameObject.AddComponent<WayPointTracker>();
        inputManager = GetComponent<InputManager>();
        lightManager = GetComponent<LightManager>();
        currentTorque = motorTorque - (tractionControl * motorTorque);
        rb.mass = vehicleMass;
        if (CM)
        {
            rb.centerOfMass = CM.position;
        }

        #region AI REFERENCES

        carAItargetObj = new GameObject("WaypointsTarget");
        carAItargetObj.transform.parent = this.transform.GetChild(1);
        carAItarget = carAItargetObj.transform;

        #endregion
    }
    void Update()
    {
        if (isPlayer)
        {
            carAIInput.enabled = false;
            wayPointTracker.enabled = false;
            inputManager.enabled = true;
            PlayerInput();
        }
        else
        {
            carAIInput.enabled = true;
            wayPointTracker.enabled = true;
        }
        currentSpeed = rb.velocity.magnitude * 2.23693629f;
        isDrivingDebug();
        HeadLight();
        BrakeLight();
        //Debug.Log(transform.InverseTransformVector(rb.velocity).z * 2.23694f);
    }

    void FixedUpdate()
    {
        //Debug.Log(handbrake);
        if (reverseGearOn)
        {
            Move(steering, -accel, footbrake, handbrake);
        }
        else
        {
            Move(steering, accel, footbrake, handbrake);
        }        
    }

    #region Player
    void PlayerInput()
    {
        accel = inputManager.accel;
        steering = inputManager.steer;
        footbrake = inputManager.footbrake;
        handbrake = inputManager.handbrake;
        spotLight = inputManager.spotLight;
    }
    #endregion Player

    #region Light
    void HeadLight()
    {
        if (spotLight)
        {
            lightManager.ToggleHeadlights();
        }
    }

    void BrakeLight()
    {
        if (isPlayer == true)
        {
            foreach (GameObject brakeLight in brakeLights)
            {
                brakeLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", inputManager.handbrake == 1 || inputManager.footbrake == -1 ? new Color(0.5f, 0.111f, 0.111f) : Color.black);
            }
        }
    }
    #endregion Light

    #region Driving   
    public void Move(float steering, float accel, float footbrake, float handbrake)
    {
        steering = Mathf.Clamp(steering, -1, 1);        
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);
        
        CalculateRPM();

        AutoGearSystem();

        SteeringWheels(steering);

        ApplyDrive(accel, footbrake);

        MaxSpeedReached();

        HandBraking(handbrake);

        AddDownForce();

    }

    private void ApplyDrive(float Accel, float footbrake)
    {
        float thrustTorque;

        thrustTorque = Accel * (currentTorque / 4f) * enginePower.Evaluate(1);
        foreach (WheelCollider wheel in throttleWheels)
        {
            wheel.GetComponent<WheelCollider>().motorTorque = thrustTorque;
        }

        #region FOOTBRAKE

        if (footbrake > 0)
        {
            if (currentSpeed > 5 && Vector3.Angle(transform.forward, rb.velocity) < 50f)
            {
                reverseGearOn = false;
            }
            else
            {
                reverseGearOn = true;
            }
        }
        else
        {
            reverseGearOn = false;
        }

        if (!ABS)
        {
            if (currentSpeed > 5 && Vector3.Angle(transform.forward, rb.velocity) < 50f)
            {
                foreach (WheelCollider wheel in throttleWheels)
                {
                    wheel.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;
                }
            }
            else if (footbrake > 0)
            {
                foreach (WheelCollider wheel in throttleWheels)
                {
                    wheel.GetComponent<WheelCollider>().brakeTorque = 0f;
                    wheel.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;
                }
            }
        }
        else
        {
            if (currentSpeed > 5 && Vector3.Angle(transform.forward, rb.velocity) < 50f)
            {
                StartCoroutine(ABSCoroutine(footbrake));
            }
            else if (footbrake > 0)
            {
                foreach (WheelCollider wheel in throttleWheels)
                {
                    wheel.GetComponent<WheelCollider>().brakeTorque = 0f;
                    wheel.GetComponent<WheelCollider>().motorTorque = -reverseTorque * footbrake;
                }
            }
        }

        #endregion

    }


    private void SteeringWheels(float steering)
    {
        foreach (GameObject wheel in steeringWheels)
        {
            var steerAngle = steering * maximumSteerAngle;
            wheel.GetComponent<WheelCollider>().steerAngle = Mathf.Lerp(wheel.GetComponent<WheelCollider>().steerAngle, steerAngle, 0.5f);
            wheel.transform.localEulerAngles = new Vector3(0f, steerAngle, 0f);
        }

        foreach (GameObject wheelMesh in leftWheelMeshes)
        {
            wheelMesh.transform.Rotate(rb.velocity.magnitude * (transform.InverseTransformDirection(rb.velocity).z >= 0 ? 1 : -1) / (2 * Mathf.PI * 0.33f), 0f, 0f);
        }
        foreach (GameObject wheelMesh in rightWheelMeshes)
        {
            wheelMesh.transform.Rotate(rb.velocity.magnitude * (transform.TransformDirection(rb.velocity).z >= 0 ? 1 : -1) / (2 * Mathf.PI * 0.33f), 0f, 0f);
        }

    }
    private void MaxSpeedReached()
    {        
        if (currentSpeed > maxSpeed)
        {
            rb.velocity = (maxSpeed / 2.23693629f) * rb.velocity.normalized;
        }
    }

    private void HandBraking(float handbrake)
    {
        if (handbrake > 0f)
        {
            var hbTorque = handbrake * handbrakeTorque;
            foreach (WheelCollider wheel in throttleWheels)
            {
                wheel.GetComponent<WheelCollider>().brakeTorque = hbTorque;                
            }
        }
    }

    IEnumerator ABSCoroutine(float footbrake)
    {
        foreach (WheelCollider wheel in throttleWheels)
        {
            wheel.GetComponent<WheelCollider>().brakeTorque = brakeTorque * footbrake;
        }
        yield return new WaitForSeconds(0.1f);

        foreach (WheelCollider wheel in throttleWheels)
        {
            wheel.GetComponent<WheelCollider>().brakeTorque = 0;
        }
        yield return new WaitForSeconds(0.1f);
    }

    private void isDrivingDebug()
    {
        if (rb.velocity.magnitude < 1)
        {
            if (isDriving)
            {
                if (carAIInput.reverseGearOn)
                {
                    rb.AddRelativeForce(0, 0, -vehicleMass * 1000 * Time.deltaTime);
                }
                else
                {
                    rb.AddRelativeForce(0, 0, vehicleMass * 1000 * Time.deltaTime);
                }
            }
        }
    }

    #endregion Driving   

    #region GEAR SYSTEM
    private void AutoGearSystem()
    {
        float gearRatio = Mathf.Abs(currentSpeed / maxSpeed);

        float gearUp = (1 / (float)numberOfGears) * (currentGear + 1);
        float gearDown = (1 / (float)numberOfGears) * currentGear;

        if (currentGear > 0 && gearRatio < gearDown)
        {
            currentGear--;
        }

        if (gearRatio > gearUp && (currentGear < (numberOfGears - 1)))
        {
            if (!reverseGearOn)
            {
                currentGear++;
            }
        }
    }

    // Curved Bias towards 1 for a value between 0-1
    private static float BiasCurve(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }


    // Smooth Lerp with no fixed Boundaries
    private static float SmoothLerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }


    private void CalculateGearFactor()
    {
        // Smooth Gear Changing
        float f = (1 / (float)numberOfGears);

        var targetGearFactor = Mathf.InverseLerp(f * currentGear, f * (currentGear + 1), Mathf.Abs(currentSpeed / maxSpeed));
        gearFactor = Mathf.Lerp(gearFactor, targetGearFactor, Time.deltaTime * 5f);
    }


    private void CalculateRPM()
    {
        // Calculate engine RPM
        CalculateGearFactor();
        float gearNumFactor;

        gearNumFactor = (currentGear / (float)numberOfGears);

        var minRPM = SmoothLerp(0f, rpmRange, BiasCurve(gearNumFactor));
        var maxRPM = SmoothLerp(rpmRange, 1f, gearNumFactor);

        RPM = SmoothLerp(minRPM, maxRPM, gearFactor);
    }

    #endregion

    #region AERODYNAMICS
    private void AddDownForce()
    {
        rb.AddForce(-transform.up * downForce * rb.velocity.magnitude);
    }

    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= slipLimit && currentTorque >= 0)
        {
            currentTorque -= 10 * tractionControl;
        }
        else
        {
            currentTorque += 10 * tractionControl;
            if (currentTorque > motorTorque)
            {
                currentTorque = motorTorque;
            }
        }
    }

    #endregion
}  

