using System.Collections;
using UnityEngine;



public class InitializeShipStorage : MonoBehaviour
{
    public GameObject[] containers;
    System.Random r = new System.Random();
    ContainerYardScript storageyard;
    GameObject Ship;

    
    /// <summary>
    /// Initializes the ship storage according to simulation mode. Returns IEnumerator and runs as a Coroutine.
    /// Random is used to select a container color.
    /// </summary>
    /// <param name="ship"></param>
    /// <returns>Coroutine WaitForSeconds</returns>
    public IEnumerator InitializeShip(GameObject ship)
    {
        Ship = ship;
        storageyard = ship.transform.GetChild(0).gameObject.GetComponent<ContainerYardScript>();
        Vector3? NotNullPosition;
        ContainerYardScript script = storageyard.GetComponent<ContainerYardScript>();
        Ship.GetComponent<BoatAI>().Parked = true;

        while ((NotNullPosition = script.AskPlace()) != null)
        {
            int containerRandom = r.Next() % containers.Length;
            Instantiate(containers[containerRandom], (Vector3)NotNullPosition, storageyard.transform.rotation);
        }
        yield return new WaitForSeconds(1);
        Ship.GetComponent<BoatAI>().Parked = false;

    }
}
