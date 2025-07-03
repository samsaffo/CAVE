using System.Collections;
using UnityEngine;

/*
 Handles spawning of internal and external trucks according to main menu settings.
 There should only be one copy in the scene. 
 */

public class TruckSpawning : MonoBehaviour
{
    //Link in editor to prefabs for trucks
    public GameObject ExternalTruckSpawn;
    public GameObject InternalTruckSpawn;

    //Link in editor to spawn waypoints
    public Transform ExternalSpawnPoint;
    public Transform InternalSpawnPoint;

    //Link in editor to container prefabs
    public GameObject[] Containers;

    GameObject ExternalTruck;
    GameObject InternalTruck;

    private IEnumerator InternalSpawn;

    //These values are fetched from the main menu settings
    int SpawnFrequency = 5;
    bool loadMode;
    int InternalSpawnCount = 1;

    int contIndex = 0;
    private bool SpawnFirst = true;

    //Controls how far away previous truck must be before next one can spawn
    public float SpawnRadius = 30f;

    void Start()
    {
        loadMode = true; //PlayerPrefs.GetInt("isLoadMode") == 1 ? true : false; //only load mode for test
        SpawnFrequency = 1; // PlayerPrefs.GetInt("ExternalTruckFrequency"); //make trucks appear immediately for test
        InternalSpawnCount = PlayerPrefs.GetInt("InternalTruckCount");

        InternalSpawn = SpawnInternalTruck();
        //StartCoroutine(InternalSpawn); //disabled for user test setup so only external trucks spawn

        InvokeRepeating("SpawnExternalTruck", 0.0f, SpawnFrequency);
    }

    void SpawnExternalTruck()
    {
        if (SpawnFirst || Vector3.Distance(ExternalSpawnPoint.position, ExternalTruck.transform.GetChild(0).position) > SpawnRadius)
        {
            SpawnFirst = false;
            ExternalTruck = Instantiate(ExternalTruckSpawn, ExternalSpawnPoint.position, ExternalSpawnPoint.rotation);
            GameObject trailer = ExternalTruck.transform.Find("Trailer").gameObject;

            //add container if simulation is set to load the ship
            if (loadMode)
            {
                Instantiate(Containers[contIndex++], trailer.transform.position + new Vector3(0, 2f, 0), trailer.transform.rotation * Quaternion.Euler(0, 90, 0));
                contIndex = contIndex >= Containers.Length ? 0 : contIndex;
            }
        }
    }

    IEnumerator SpawnInternalTruck()
    {
        Debug.Log("Internal truck spawn: " + InternalSpawnCount);
        bool spawnFirst = true;
        while(InternalSpawnCount > 0)
        {
            if(spawnFirst || Vector3.Distance(InternalSpawnPoint.position, InternalTruck.transform.GetChild(0).position) > SpawnRadius)
            {
                InternalTruck = Instantiate(InternalTruckSpawn, InternalSpawnPoint.position, InternalSpawnPoint.rotation);
                InternalSpawnCount--;
                spawnFirst = false;
            }

            yield return new WaitForSeconds(3);
        }
    }
}
