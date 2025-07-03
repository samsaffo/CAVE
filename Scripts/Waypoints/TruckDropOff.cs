using System;
using UnityEngine;

/*Forwards message from truck to crane. Should be attached to each loading spot waypoint.*/

public class TruckDropOff : MonoBehaviour
{
    public GameObject Crane;
    private Action<bool, Vector3> function;
    public TruckNavigation CurrentTruck;

    public void Start()
    {
        GantryCraneMovement gcm = null;
        Crane.transform.TryGetComponent<GantryCraneMovement>(out gcm);
        function = gcm != null ? gcm.OnTruckArrival : Crane.transform.GetComponent<PanamaxCraneMovement>().OnTruckArrival;
    }

    public void EnqueuePosition(bool pickup, Vector3 pos)
    {
        function.Invoke(pickup, pos);
    }

    public void RegisterTruck(TruckNavigation truck)
    {
        CurrentTruck = truck;
    }

    public void LetTruckLeave()
    {
        if (CurrentTruck == null) return;

        CurrentTruck.Stop(false);
        CurrentTruck = null;
    } 
}
