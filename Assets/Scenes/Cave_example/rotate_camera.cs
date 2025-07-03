using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_camera : MonoBehaviour
{
    public float rotationSpeed = 50f;

    void Update()
    {
        // Horizontal rotation
        if (Input.GetKey(KeyCode.A))
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.D))
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Vertical rotation
        if (Input.GetKey(KeyCode.U))
            transform.Rotate(Vector3.right, -rotationSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.J))
            transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}

