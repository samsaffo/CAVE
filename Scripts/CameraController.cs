using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*Controls moving camera in game. Should be attached to main camera. Link actions to input manager.*/
public class CameraController : MonoBehaviour
{
    Vector3 Direction;
    public float MoveSpeed = 5f;
    public float SpeedModifier = 2f;

    Vector2 MouseDelta;
    public float Sensitivity = 5f;

    public bool RotationEnabled = false;
    public bool SpeedUp = false;
    public bool TopDownActive = false;

    public GameObject TopDownCamera;

    public void OnMove(InputAction.CallbackContext context)
    {
        Direction = context.ReadValue<Vector3>();
    }

    public void OnSpeedUp(InputAction.CallbackContext context)
    {
        SpeedUp = context.ReadValueAsButton();
    }

    public void UnlockRotation(InputAction.CallbackContext context)
    {
        RotationEnabled = context.ReadValueAsButton();
        Cursor.visible = !RotationEnabled;
        Cursor.lockState = RotationEnabled ? CursorLockMode.Locked : CursorLockMode.None;

    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if(RotationEnabled)
        {
            MouseDelta = context.ReadValue<Vector2>();
        }
        else
        {
            MouseDelta = Vector2.zero;
        }
    }

    public void ToggleTopDown(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            TopDownActive = !TopDownActive;
            TopDownCamera.SetActive(TopDownActive);
        }
    }


    void Update()
    {
        float speed = SpeedUp ? MoveSpeed * SpeedModifier : MoveSpeed;
        transform.position += speed * Time.deltaTime * (transform.forward * Direction.z + transform.right * Direction.x + transform.up * Direction.y);
        transform.eulerAngles += Sensitivity * Time.deltaTime * new Vector3(-MouseDelta.y, MouseDelta.x, 0f);
    }
}
