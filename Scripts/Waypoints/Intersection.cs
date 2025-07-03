using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

/*Controls behavior of intersections, stopping all incoming trucks and choosing who can go next.
 Current setup: first come, first serve. Each truck will stop until intersection is cleared, and then allowed to go in order of arrival.*/

[RequireComponent(typeof(SphereCollider))]
public class Intersection : MonoBehaviour
{
    [SerializeField]
    public ConcurrentQueue<TruckNavigation> TruckQueue;
    TruckNavigation Current;

    void Start()
    {
        TruckQueue = new();
        var sc = GetComponent<SphereCollider>();
        sc.radius = 15f;
        sc.isTrigger = true;
    }

    private void FixedUpdate()
    {
        //a truck is currently moving through
        if (Current != null)
            return;

        //intersection is cleared, let next truck go
        if (TruckQueue.TryDequeue(out TruckNavigation temp))
        {
            Current = temp;
            Current.Stop(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ExternalTruck"))
        {
            var current = other.gameObject.transform.parent.GetComponentInChildren<TruckNavigation>();
            //check that new truck isn't the one currently moving through intersection so that trailer isn't added as new truck
            if (current != null && Current != current)
            {
                TruckQueue.Enqueue(current);
                current.Stop(true);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("ExternalTruck"))
        {
            var current = other.gameObject.transform.parent.GetComponentInChildren<TruckNavigation>();
            if (Current != null && Current.GetInstanceID() == current.GetInstanceID())
            {
                Current = null;
            }
        }
    }
}
