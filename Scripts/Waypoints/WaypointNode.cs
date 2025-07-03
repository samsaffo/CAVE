using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Keeps track of calculated values for each truck getting a path. Also draws waypoints and connection in the editor view.
 Attach to waypoints.
 */
public class WaypointNode : MonoBehaviour, IHeapItem<WaypointNode>
{
    public enum WaypointType { ExternalSpawn, InternalSpawn, Normal, StorageCrane, ShipCrane, Despawn, Intersection }
    public WaypointType Type = WaypointType.Normal;

    public List<WaypointNode> Neighbours;

    public Dictionary<int, float> GCost;
    public Dictionary<int, float> HCost;
    public float FCost(int id) => GCost.GetValueOrDefault(id, float.MaxValue) + HCost.GetValueOrDefault(id, float.MaxValue); 
    public Dictionary<int, int> HeapIndex;

    public float GetDistance(WaypointNode to) => Vector3.Distance(transform.position, to.transform.position);

    private void OnDrawGizmos()
    {
        switch (Type)
        {
            case WaypointType.Normal:
                Gizmos.color = Color.red;
                break;

            case WaypointType.ExternalSpawn:
            case WaypointType.InternalSpawn:
            case WaypointType.Despawn:
                Gizmos.color = Color.yellow;               
                break;

            case WaypointType.StorageCrane:
            case WaypointType.ShipCrane:
                Gizmos.color = TryGetComponent<TruckDropOff>(out _) ? Color.cyan : Color.magenta;
                break;

            case WaypointType.Intersection:
                Gizmos.color = TryGetComponent<Intersection>(out _) ? Color.green : Color.magenta;
                break;

            default:
                Gizmos.color = Color.white;
                break;
        }

        Gizmos.DrawWireSphere(transform.position, 2f);

        foreach (WaypointNode neightbour in Neighbours)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, neightbour.transform.position);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, Vector3.Normalize(neightbour.transform.position - transform.position) * 10f);
        }
    }

    public int CompareTo(int id, WaypointNode compareNode)
    {
        int compare = FCost(id).CompareTo(compareNode.FCost(id));

        if (compare == 0)
        {
            compare = HCost.GetValueOrDefault(id, float.MaxValue).CompareTo(compareNode.HCost.GetValueOrDefault(id, float.MaxValue));
        }

        return -compare;
    }

    public int GetHeapIndex(int id)
    {
        return HeapIndex.GetValueOrDefault(id, 0);
    }

    public void SetHeapIndex(int id, int index)
    {
        HeapIndex[id] = index;
    }

    public void Awake()
    {
        GCost = new();
        HCost = new();
        HeapIndex = new();
    }
}
