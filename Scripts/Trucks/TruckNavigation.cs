using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class TruckNavigation : MonoBehaviour
{
    public bool IsInternal = false;
    public float WayPointAccuracy = 5f;
    public float Deceleration = 12f;

    NodeGraph wpManager;
    List<WaypointNode> Path;
    int i = 0;

    [SerializeField]
    int CollisionCount = 0;

    NavMeshAgent navMeshAgent;
    float BaseSpeed, BaseAcceleration;

    public AttachDetachCont Trailer;

    private void Start()
    {
        Trailer = transform.parent != null ? transform.parent.GetComponentInChildren<AttachDetachCont>() : null;

        if (Trailer == null)
        {
            Debug.LogWarning("TruckNavigation: Trailer (AttachDetachCont) not found!");
        }
        else
        {
            foreach (Collider col in Trailer.GetComponents<Collider>())
            {
                var meshCol = transform.GetChild(0).GetComponent<MeshCollider>();
                if (meshCol != null)
                    Physics.IgnoreCollision(meshCol, col);
            }
        }

        wpManager = FindObjectOfType<NodeGraph>();
        if (wpManager == null)
        {
            Debug.LogError("TruckNavigation: NodeGraph not found!");
            return;
        }

        Path = wpManager.GetRandomPathFromSpawn(GetInstanceID(), IsInternal);
        if (Path == null || Path.Count == 0)
        {
            Debug.LogError("TruckNavigation: No path found for truck!");
            return;
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.destination = Path[i].gameObject.transform.position;
        BaseSpeed = navMeshAgent.speed;
        BaseAcceleration = navMeshAgent.acceleration;
    }

    public void Stop(bool stopped)
    {
        navMeshAgent.isStopped = stopped;
        navMeshAgent.acceleration = stopped ? Deceleration : BaseAcceleration;
    }

    void FixedUpdate()
    {
        if (Path == null || Path.Count == 0) return;

        if (CalculateDistance(transform.position, navMeshAgent.destination) < WayPointAccuracy)
        {
            GoToNextPoint();
        }
    }

    float CalculateDistance(Vector3 Source, Vector3 Destination)
    {
        return Vector3.Distance(Source, Destination);
    }

    public void SetSpeed(float speed)
    {
        navMeshAgent.speed = speed;
        navMeshAgent.acceleration = speed <= 0 ? Deceleration : BaseAcceleration;
    }

    void GoToNextPoint()
    {
        if (Path == null || i >= Path.Count) return;

        switch (Path[i].Type)
        {
            case WaypointNode.WaypointType.StorageCrane:
            case WaypointNode.WaypointType.ShipCrane:
                Stop(true);
                if (Trailer != null)
                {
                    var dropoff = Path[i].gameObject.GetComponent<TruckDropOff>();
                    if (dropoff != null)
                    {
                        dropoff.EnqueuePosition(Trailer.HasContainer, Trailer.GetPosition());
                        Trailer.ReleaseContainer(100);
                        dropoff.RegisterTruck(this);
                    }
                }
                break;

            case WaypointNode.WaypointType.Despawn:
                int id = GetInstanceID();
                Path[i].GCost.Remove(id);
                Path[i].HCost.Remove(id);
                Path[i].HeapIndex.Remove(id);
                Destroy(transform.parent.gameObject);
                return;
        }

        i++;
        if (i >= Path.Count)
        {
            Path = wpManager.GetRandomPath(GetInstanceID(), Path[^1], IsInternal);
            i = 0;
        }

        if (Path != null && Path.Count > 0)
        {
            navMeshAgent.destination = Path[i].gameObject.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ExternalTruck") || other.CompareTag("CraneSpreader") || other.CompareTag("PickedUpContainer"))
        {
            CollisionCount++;
            SetSpeed(0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ExternalTruck") || other.CompareTag("CraneSpreader") || other.CompareTag("PickedUpContainer"))
        {
            CollisionCount = Mathf.Max(0, CollisionCount - 1);
            if (CollisionCount <= 0)
                SetSpeed(BaseSpeed);
        }
    }
}
