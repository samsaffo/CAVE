using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpawnPointBehaviour : MonoBehaviour
{
    public GameObject Ship;
    public float RealSpawnOffSet = 50.0f;
    public int ShipLimit = 5;
    
    private List<GameObject> ShipsMultiple;
    private float ShipLength;
    private bool CanSpawn;
    private Vector3 SpawnPoint;
    public float SpawnInterval;
    private Queue<byte> Queue;

    ExportStatistics ExportStatistics;


    /// <summary>
    /// Initialize the spawnpoint behaviour. Gets boat model, spawn freq, points, and invokes a repeating spawnship function.
    /// </summary>
    void Start()
    {
        SpawnInterval = PlayerPrefs.GetInt("ShipFrequency");
        SpawnPoint = GameObject.FindGameObjectWithTag("ShipStartingPoint").transform.position;
        ShipsMultiple = new List<GameObject>();
        var MeshRender = Ship.GetComponent<MeshRenderer>();
        ShipLength = MeshRender.bounds.size.z;
        Ship.transform.position = SpawnPoint;
        InvokeRepeating("SpawnShip", 0.0f, SpawnInterval);
        Queue = new Queue<byte>();
        ExportStatistics = GameObject.FindGameObjectWithTag("PauseCanvas").transform.GetChild(2).gameObject.transform.GetChild(5).gameObject.GetComponent<ExportStatistics>();

    }
    /// <summary>
    /// If the ship spawnpoint is not obstructed, a new ship is spawned.
    /// </summary>
    private void Update()
    {
        if (CheckForSpawnOK())
        {
            if (Queue.Count > 0)
            {
                Queue.Dequeue();
                var ship = Instantiate(Ship, SpawnPoint, Quaternion.identity);
                ship.SetActive(true);
                ShipsMultiple.Add(ship);
            }
            
        }
    }

    /// <summary>
    /// If the amount of ships waiting to spawn does exceed 6, we put the waiting ship in the spawn-queue. The added entry to the queue is a byte that represents a ship.
    /// </summary>
    void SpawnShip()
    {
        if (Queue.Count < 6)
        {
            //Queue.Enqueue(0);
        }   
    }

    /// <summary>
    /// Helper function for Update(). We check if spawning is viable with the amount of ships that are in the world already (and the spawnpoint isn't obstructed by another ship). 
    /// </summary>
    /// <returns>True if boat can spawn, False otherwise</returns>
    bool CheckForSpawnOK()
    {
        if (ShipsMultiple.Count <= ShipLimit)
        {
            CanSpawn = true;
            foreach (var item in ShipsMultiple)
            {
                if (Vector3.Distance(item.transform.position, SpawnPoint) < ShipLength * 1.5f)
                {
                    CanSpawn = false;
                    break;
                }
            }
        }
        return CanSpawn && ShipsMultiple.Count <= ShipLimit;
    }

    /// <summary>
    /// Destroy the instance of the boat. Add containers to statistics.
    /// </summary>
    /// <param name="shipToDelete"></param>
    public void DeleteShip(GameObject shipToDelete)
    {
        ShipsMultiple.Remove(shipToDelete);
        var containeryard = shipToDelete.transform.GetChild(0).GetComponent<ContainerYardScript>();
        var size = containeryard.Width * containeryard.Height * containeryard.Length;
        for (int i = 0; i < size; i++)
        {
            ExportStatistics.IncrementContainerThroughPutAmount();
        }
        Destroy(shipToDelete);
    }

    
}
