using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpreaderCollision : MonoBehaviour
{
    public ManualControl craneControl;

    private void Start()
    {
        // Optionally assign craneControl here if needed
        // craneControl = GetComponent<ManualControl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LimitCrane")
        {
            Debug.Log("Ghost spreader entered limit zone: " + other.gameObject.name);
            // You can add custom logic here if needed
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "LimitCrane")
        {
            Debug.Log("Ghost spreader exited limit zone: " + other.gameObject.name);
            // You can add custom logic here if needed
        }
    }
}
