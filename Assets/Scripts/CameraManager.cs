using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject focus;
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = focus.transform.position + focus.transform.TransformDirection(offset);
        transform.rotation = focus.transform.rotation;
        Camera.main.fieldOfView = 90f;
    }
}
