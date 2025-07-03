using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollision : MonoBehaviour
{
    private Targets targetParent;

    void Start()
    {
        targetParent = transform.parent.GetComponent<Targets>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (transform.name == "TruckTarget" && collision.collider.CompareTag("CraneSpreader"))
        {
            targetParent?.NextTarget();
        }
        else if (collision.collider.CompareTag("PickedUpContainer"))
        {
            targetParent?.TargetReached(collision.collider.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (transform.name == "TruckTarget" && other.CompareTag("CraneSpreader"))
        {
            targetParent?.NextTarget();
        }
        else if (other.CompareTag("PickedUpContainer"))
        {
            if (targetParent != null)
                targetParent.TargetReached(other.gameObject);
        }
    }

}
