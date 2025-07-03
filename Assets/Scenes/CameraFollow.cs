using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Drag the object you want the camera to follow
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Adjust as needed

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
