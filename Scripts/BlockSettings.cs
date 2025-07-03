using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Attach to Storage yard cranes and their respective loading waypoint to disable them depending on main menu settings.*/
public class BlockSettings : MonoBehaviour
{
    [Range (1, 8)]
    public int BlockId; //assign in editor
    public GameObject emptyStorage;
    
    int BlockLimit;
    NodeGraph NodeGraph;

    void Start()
    {
        BlockLimit = PlayerPrefs.GetInt("StorageYardAmount");
        NodeGraph = FindObjectOfType<NodeGraph>();

        if (BlockId > BlockLimit)
        {
            WaypointNode node;
            if(gameObject.TryGetComponent<WaypointNode>(out node))
            {
                NodeGraph.RemoveTargets(node);
                return;
            }
            //if it's not a node, it should be crane and is disabled
            GameObject gm = Instantiate(emptyStorage, gameObject.transform.parent);

            gm.transform.position = gameObject.transform.GetChild(1).position;

            gameObject.SetActive(false);
        }
    }
}
