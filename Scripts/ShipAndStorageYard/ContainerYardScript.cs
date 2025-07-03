using System.Collections.Generic;
using UnityEngine;
using System;

public class ContainerYardScript : MonoBehaviour
{
    public GameObject Prefab;
    private Vector3 CellSize;
    public int Height, Length, Width;

    private List<Vector3Int> pointersTakeable;
    private List<Vector3Int> pointersPlaceable;
    bool RunOnce = false;
    public bool PeekPlace => pointersPlaceable.Count > 0;

    public bool PeekTake => pointersTakeable.Count > 0;

    void Start()
    {
        CellSize = Prefab.GetComponent<BoxCollider>().size + new Vector3(0.5f, 0.5f, 0.5f);
        pointersTakeable = new List<Vector3Int>();
        pointersPlaceable = new List<Vector3Int>();

        InitializeGridPointers();
    }

    private void Update()
    {
        if (!RunOnce)
        {
            if (transform.parent.CompareTag("Ship"))
            {
                int LoadorUnload = PlayerPrefs.GetInt("isLoadMode");

                if (LoadorUnload == 0)
                {
                    var ShipStorageScript = GameObject.FindGameObjectWithTag("InitializeShip").GetComponent<InitializeShipStorage>();
                    StartCoroutine(ShipStorageScript.InitializeShip(transform.parent.gameObject));
                }
            }
            else
            {
                //float StartAmount = (Height * Width * Length) * PlayerPrefs.GetInt("ContainerSpawnAmount") / 100;
                //GameObject.FindGameObjectWithTag("InitializeGroundStorageYard").GetComponent<InitializeGroundStorageYard>().SendMessage("InitializeGroundStorage", new Tuple<ContainerYardScript, float>(this, StartAmount));
            }
            RunOnce = true;
        }
    }

    /// <summary>
    /// Calculates positions for the containers
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>Vector3?</returns>
    Vector3 NewPosition(int x, int y, int z)
    {
        Vector3 planePosition = gameObject.transform.position;
        Vector3 position = gameObject.transform.right * (y * CellSize.y) + (gameObject.transform.up) * (x * CellSize.x) + gameObject.transform.forward * (z * CellSize.z) + planePosition;
        return position;
    }

    /// Initializes the grid pointers used by AskPlace & AskTake
    void InitializeGridPointers()
    {
        for (int y = 0; y < Width; y++)
        {
            for (int z = 0; z < Length; z++)
            {
                pointersPlaceable.Add(new Vector3Int(0, y, z)); // x is 0 because the pointers always start at the bottom
            }
        }
    }
    /// <summary>
    /// Gives the crane a position of an available container slot. 
    /// Returns null if no slots are available, otherwise returns a Vector3? containing the available slots' position.
    /// </summary>
    /// <returns>Vector3?</returns>
    public Vector3? AskPlace()
    {
        if (pointersPlaceable.Count == 0)
        {
            return null;
        }
        System.Random random = new System.Random();
        var index = random.Next() % pointersPlaceable.Count;
        // calculate position to place new container
        var posInfo = NewPosition(pointersPlaceable[index].x, pointersPlaceable[index].y, pointersPlaceable[index].z);
        // after this point we assume the container will be placed by the crane.
        var temp = pointersPlaceable[index];
        // increment the pointer
        var updated = new Vector3Int(pointersPlaceable[index].x + 1, pointersPlaceable[index].y, pointersPlaceable[index].z);
        
        if (temp.x + 1 == Height) // the pointer is now at the top, remove from Placeable and update the pointer in Takeable.
        {
            // remove old pointer
            pointersPlaceable.Remove(temp);
            pointersTakeable.Remove(temp);
            pointersTakeable.Add(updated);
        }
        else if (temp.x == 0) // check if we just placed a container on the ground level, i.e. this pointer is now also Takeable.
        {
            // remove old pointer
            pointersPlaceable.Remove(temp);
            pointersTakeable.Add(updated);
            pointersPlaceable.Add(updated);
        }
        else if (temp.x < Height) //just replace old pointer with new one when the pointer is not at the top. Otherwise its just removed from placeable. If the pointer is not on the ground it will be in Takeable.
        {
            // remove old pointer
            pointersPlaceable.Remove(temp);
            pointersPlaceable.Add(updated);
            // sync with Takeable list
            pointersTakeable.Remove(temp);
            pointersTakeable.Add(updated);
        }
        else
            throw new Exception("temp.x+1 was oob");
        return posInfo;
    }

    

    /// <summary>
    /// Gives the crane a position of an available container.
    /// Returns null if no containers are available, otherwise returns a Vector3? containing the available containers' position.
    /// </summary>
    /// <returns>Vector3?></returns>
    public Vector3? AskTake()
    {
        if (pointersTakeable.Count == 0)
        {
            return null;
        }
        System.Random random = new System.Random();
        var index = random.Next() % pointersTakeable.Count;

        // calculate position to take new container from
        var posInfo = NewPosition(pointersTakeable[index].x, pointersTakeable[index].y, pointersTakeable[index].z);
        
        // after this point we assume the container will be taken by the crane.
        var temp = pointersTakeable[index];
        // decrement the pointer
        var updated = new Vector3Int(pointersTakeable[index].x - 1, pointersTakeable[index].y, pointersTakeable[index].z);
        

        if (temp.x - 1 == 0) // the pointer is now on the ground, remove from Takeable and update the pointer in Placeable.
        {
            // remove old pointer
            pointersTakeable.Remove(temp);
            pointersPlaceable.Remove(temp);
            pointersPlaceable.Add(updated);
        }
        else if (temp.x == Height) // check if we just took a container from the top, i.e. this pointer is now also Placeable.
        {
            // remove old pointer
            pointersTakeable.Remove(temp);
            pointersTakeable.Add(updated);
            pointersPlaceable.Add(updated);
        }
        else if (temp.x > 0 ) //just replace old pointer with new one when the pointer is not at the ground. Otherwise its just removed from Takeable. If the pointer is not at the top it will be in Placeable.
        {
            // remove old pointer
            pointersTakeable.Remove(temp);
            pointersTakeable.Add(updated);
            // sync with Placeable list
            pointersPlaceable.Remove(temp);
            pointersPlaceable.Add(updated);
        }
        else
            throw new Exception("temp.x-1 was negative");
        return posInfo;
    }
}
