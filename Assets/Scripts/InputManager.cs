using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public InputActionReference accelReference = null;
    public Transform steeringWheel;
    public float accel;
    public float steer;
    public bool spotLight;
    public float handbrake;
    public float footbrake;
    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        setSteer();
        accelReference.action.started += accelerate;
        accelReference.action.canceled += resetAccelerate;
        spotLight = true;// Input.GetKeyDown(KeyCode.L);
        footbrake = 0;
        handbrake = 0;
        //if (Input.GetKey(KeyCode.R)) { footbrake = -1; } else { footbrake = 0; }
        //if (Input.GetKey(KeyCode.Space)) { handbrake = 1; } else { handbrake = 0; }        
    }

    public void accelerate(InputAction.CallbackContext context)
    {
        accel = 1;
    }
    private void resetAccelerate(InputAction.CallbackContext context)
    {
        accel = 0;
    }

    private void setSteer()
    {        
        var angle = 0f;
        if(steeringWheel.transform.localEulerAngles.z > 180)
        {
            angle = 360f - steeringWheel.transform.localEulerAngles.z;
        }
        else
        {
            angle = - steeringWheel.transform.localEulerAngles.z;

        }
        //Debug.Log(angle);
        steer = angle / 180;
        //Debug.Log(steer);
    }
}
