using UnityEngine;
using UnityEngine.AI;

public class BoatAI : MonoBehaviour
{
    private GameObject ShipManager;
    private GameObject DeSpawnPoint;
    NavMeshAgent agent;
    private GameObject[] WayPoints;
    private float WayPointAccuracy = 5f;
    // waypoint index
    int index = 0;
    bool NextWaypointIsDespawn;


    float BaseSpeed;
    float BaseAcceleration;
    public float Deceleration = 12f;
    public bool Parked = false;
    public GameObject CollisionRayOrigin;
    public float CollisionCheckDistance = 5f;
    public float CollisionCheckSpread = 3f;
    ExportStatistics ShipWaitStatistic;
    


    void Start()
    {
        //the agent is the AI boat, used to change speed and direction
        agent = GetComponent<NavMeshAgent>();
        WayPoints = GameObject.FindGameObjectsWithTag("ShipWayPoint");
        DeSpawnPoint = GameObject.FindGameObjectWithTag("ShipDespawnPoint");
        ShipManager = GameObject.FindGameObjectWithTag("ShipManager");
        ShipWaitStatistic = GameObject.FindGameObjectWithTag("PauseCanvas").transform.GetChild(2).GetChild(5).GetComponent<ExportStatistics>();

        BaseSpeed = agent.speed;
        BaseAcceleration = agent.acceleration;

        //check if array is not null
        if (WayPoints.Length > 0)
        {
            NextWaypointIsDespawn = false;
            if (agent.SetDestination(WayPoints[index].transform.position))
            {
                SetSpeed(BaseSpeed);
            }
            else
            {
                throw new System.Exception("Ship needs at least one waypoint");
            }
        }
        else
        {
            throw new System.Exception("Ship needs at least one waypoint");
        }

    }

    public void OperationFinished()
    {
        
        ShipWaitStatistic.StopMeasureTime();
        Parked = false;
        SetSpeed(BaseSpeed);
        gameObject.GetComponent<Rigidbody>().isKinematic = false;

    }


    /// <summary>
    /// Prints out a string for the unity debug console.
    /// </summary>
    /// <param name="args"></param>
    float CalculateDistance (Vector3 Source, Vector3 Destination)
    {
        return Mathf.Abs(Vector3.Distance(Source,Destination));
    }

    void Update()
    {      
        //Continuously avoids collisions with other ships
        CollisionDetection();

        //if ship reaches despawnpoint, then despawn
        if (NextWaypointIsDespawn)
        {
            if (CalculateDistance(gameObject.transform.position, agent.destination) < WayPointAccuracy)
            {
                ShipManager.SendMessage("DeleteShip", gameObject);
            }
        }
        // if inside waypoint-array bounds and the next point isn't a despawn point
        if (!NextWaypointIsDespawn)
        {
            
            //if ship reaches the a waypoint
            if (CalculateDistance(gameObject.transform.position, agent.destination) < WayPointAccuracy)
            {
                //Check what type of waypoint. If the waypoint is of tag "ShipWait", the boat should wait. 
                if (WayPoints[index].transform.GetChild(0).gameObject.CompareTag("ShipWait"))
                {
                    agent.autoBraking = false;
                    Parked = true;
                    SetSpeed(0);
                    GameObject.FindGameObjectWithTag("PanamaxCrane").gameObject.SendMessage("SetStorageYard", gameObject);
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    ShipWaitStatistic.StartTimeMeasure();

                }
                //If next waypoint is inside the bounds of the array
                if (index + 1 < WayPoints.Length)
                {
                    //and that waypoint is of type "ShipWait", autobreak is turned on.
                    if (WayPoints[index + 1].transform.GetChild(0).gameObject.CompareTag("ShipWait"))
                    {
                        agent.autoBraking = true;
                    }
                }
                //Go to next waypoint
                index += 1;
                //if the next waypoint is the last waypoint, despawn is the next waypoint. 
                if (index >= WayPoints.Length)
                {
                    agent.SetDestination(DeSpawnPoint.transform.position);
                    NextWaypointIsDespawn = true;
                    agent.autoBraking = false;
                    return;
                }
                //if ship has been onloaded/offloaded, leave
                agent.SetDestination(WayPoints[index].transform.position);
            }

        }
    }

    /// <summary>
    /// Uses RayCast to detect collisions between boats. Used for boat queueing. 
    /// </summary>
    private void CollisionDetection()
    {
        Vector3 origin = CollisionRayOrigin.transform.position;
        Vector3 forward = transform.forward * CollisionCheckDistance;
        Vector3 offset = transform.right * CollisionCheckSpread;

        RaycastHit hit;
        bool col = false;
        for (int i = -2; i < 3; i++)
        {
            if (Physics.Raycast(origin, forward + offset * i, out hit, CollisionCheckDistance) && hit.transform.tag == "Ship")
            {
                col = true;
                break;
            }
        }

        SetSpeed(col ? 0 : BaseSpeed);
    }

    /// <summary>
    /// Global function to set boat speed.
    /// </summary>
    /// <param name="speed"></param>
    public void SetSpeed(float speed)
    {
        agent.speed = Parked ? 0 : speed;
        agent.acceleration = speed <= 0 || Parked ? Deceleration : BaseAcceleration;
    }

    /// <summary>
    /// When a container hits the boat, stick it to the boat. 
    /// If a container hits another container that is a child of the boat, they should also stick together (destroy the rigidbody)
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Container"))
        {
            other.gameObject.transform.SetParent(gameObject.transform, true);
            var rb = other.gameObject.GetComponent<Rigidbody>();
            Destroy(rb);
        }
    }


}
