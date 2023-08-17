using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SteeringWheel : MonoBehaviour
{
    public InputActionReference gripAction = null;

    public GameObject rightHand;
    private GameObject rightHandModel;
    private Transform rightHandOriginalParent;
    private bool rightHandOnWheel = false;

    public GameObject leftHand;
    private GameObject leftHandModel;
    private Transform leftHandOriginalParent;
    private bool leftHandOnWheel = false;
    public Transform[] snapPositions;



    float previousRotationZ;
    bool wheelLocked=false;
    public float currentSteeringWheelRotation = 0;

    private float turnDampening = 250;

    public Transform directionalObject;

    private bool grip;
    private void Start()
    {
  
    }

    private void Update()
    {        
        gripAction.action.canceled += ReleaseHandsFromWheel;
        ConvertHandRotationToSteeringWheelRotation();

        currentSteeringWheelRotation = -transform.rotation.eulerAngles.z;
    }

    private void OnTriggerStay(Collider other)
    {        
        if (other.CompareTag("RightPlayerHand") && rightHandOnWheel == false)
        {            
            gripAction.action.started += PlaceRightHand;
        }

        if (other.CompareTag("LeftPlayerHand") && rightHandOnWheel == false)
        {
            gripAction.action.started += PlaceLeftHand;
        }
    }

    private void PlaceRightHand(InputAction.CallbackContext context)
    {
        Debug.Log("Right");
        PlaceHandOnWheel(ref rightHand, ref rightHandOriginalParent, ref rightHandOnWheel);
    }

    private void PlaceLeftHand(InputAction.CallbackContext context)
    {        
        PlaceHandOnWheel(ref leftHand, ref leftHandOriginalParent, ref leftHandOnWheel);
    }

    private void PlaceHandOnWheel(ref GameObject hand, ref Transform originalParent, ref bool handOnWheel)
    {        
        var shortestDistance = Vector3.Distance(snapPositions[0].position, hand.transform.position);
        var bestSnap = snapPositions[0];

        foreach(var snapPosition in snapPositions)
        {
            if (snapPosition.childCount == 0)
            {
                var distance = Vector3.Distance(snapPosition.position, hand.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestSnap = snapPosition;
                }
            }
        }
        originalParent = hand.transform;
        leftHandModel = hand.transform.GetChild(0).gameObject;
        hand.transform.GetChild(0).parent = bestSnap.transform;
        hand.transform.position = bestSnap.transform.position;

        handOnWheel = true;
    }
    private void ReleaseHandsFromWheel(InputAction.CallbackContext context)
    {
        
        if (rightHandOnWheel == true)
        {
            leftHandModel.transform.parent = rightHandOriginalParent;
            rightHand.transform.position = rightHandOriginalParent.position;
            rightHand.transform.rotation = rightHandOriginalParent.rotation;
            rightHandOnWheel = false;
        }

        if (leftHandOnWheel == true)
        {
            leftHandModel.transform.parent = leftHandOriginalParent;
            leftHandOriginalParent.transform.GetChild(1).transform.SetAsFirstSibling();
            leftHandModel.transform.position = leftHandOriginalParent.position;
            leftHandModel.transform.rotation = leftHandOriginalParent.rotation;
            leftHandOnWheel = false;
        }

        if(leftHandOnWheel == false && rightHandOnWheel == false)
        {
            //transform.parent = null;
        }
    }

    private void ConvertHandRotationToSteeringWheelRotation()
    {
        if (rightHandOnWheel == true && leftHandOnWheel == false)
        {
                Quaternion newRot = Quaternion.Euler(0, 0, rightHandOriginalParent.transform.rotation.eulerAngles.z);
                directionalObject.localRotation = newRot;
                transform.parent = directionalObject;                        
        }
        else if (rightHandOnWheel == false && leftHandOnWheel == true)
        {
                Quaternion newRot = Quaternion.Euler(0, 0, leftHandOriginalParent.transform.rotation.eulerAngles.z);
                directionalObject.localRotation = newRot;
                transform.parent = directionalObject;     
        }
        //two hand are never tested
        else if (rightHandOnWheel == true && leftHandOnWheel == true)
        {
            Quaternion newRotLeft = Quaternion.Euler(0, 0, leftHandOriginalParent.transform.rotation.eulerAngles.z);
            Quaternion newRotRight = Quaternion.Euler(0, 0, rightHandOriginalParent.transform.rotation.eulerAngles.z);
            Quaternion finalRot = Quaternion.Slerp(newRotLeft, newRotRight, 1.0f / 2.0f);
            directionalObject.localRotation = finalRot;
            transform.parent = directionalObject;

        }
    }
}
