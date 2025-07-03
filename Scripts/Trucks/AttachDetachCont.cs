using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/*Handles attaching a container to the trailer, and calls truck to start moving when container is added or removed (ie done loading).
 Should be attached to "Trailer" in the truck prefab.*/

public class AttachDetachCont : MonoBehaviour
{
    TruckNavigation truckNav;
    public bool HasContainer => gameObject.transform.GetChild(0).CompareTag("Container");

    private void Start()
    {
        truckNav = gameObject.transform.parent.Find("Terminal Truck").GetComponent<TruckNavigation>();
    }

    public Vector3 GetPosition()
    {
        return transform.position + transform.up * 2.1f + transform.right * 2.3f;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Container"))
        {
            var container = collision.gameObject;
            
            //do not pick up containers if colliding with another truck
            if (container.transform.parent != null && container.transform.parent.CompareTag("ExternalTruck")) 
                return; 
            
            container.transform.SetParent(gameObject.transform, true);
            container.transform.SetAsFirstSibling(); //for HasContainer
            container.transform.position = new Vector3(transform.position.x, container.transform.position.y + 0.15f, transform.position.z) ;
            container.transform.rotation = transform.rotation * Quaternion.Euler(0, 90, 0);

            var rb = container.GetComponent<Rigidbody>();
            rb.isKinematic = true;

            truckNav.Stop(false);
        }
    }

    public async void ReleaseContainer(int delayInMilliSeconds)
    {
        if (HasContainer)
        {
            await Task.Delay(delayInMilliSeconds);
            var container = gameObject.transform.GetChild(0);
            container.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void AbandonLoad()
    {
        var container = gameObject.transform.GetChild(0);
        container.parent = null;
    }

    private void OnTransformChildrenChanged()
    {
        truckNav.Stop(false);
    }

}
