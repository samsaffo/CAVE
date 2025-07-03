using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Handles the paths for both external and internal trucks. Should be attached to a game object that is the parent of all waypoints.
 There should only be one copy in the scene.
 */
public class NodeGraph : MonoBehaviour
{
    public bool AutoRenameWaypoints = false;

    [SerializeField]
    private List<WaypointNode> GantryCraneTargets;
    [SerializeField]
    private List<WaypointNode> PanamaxCraneTargets;
    [SerializeField]
    private List<WaypointNode> DespawnTargets;
    private WaypointNode ExternalSpawnNode;
    private WaypointNode InternalSpawnNode;

    private bool IsLoadMode;

    public List<WaypointNode> GetRandomPathFromSpawn(int id, bool isInternal)
    {
        if(isInternal)
        {
            return GetRandomPath(id, InternalSpawnNode, isInternal);
        }

        return GetRandomPath(id, ExternalSpawnNode, isInternal);
    }

    /*Calculates a path from start to ship crane/despawn through a storage yard, depending on load mode and if truck is internal*/
    public List<WaypointNode> GetRandomPath(int id, WaypointNode start, bool isInternal)
    {
        List<WaypointNode> path;
        if (isInternal)
        {
            if (IsLoadMode)
            {
                var GCIndex = UnityEngine.Random.Range(0, GantryCraneTargets.Count);
                var PCIndex = UnityEngine.Random.Range(0, PanamaxCraneTargets.Count);
                path = GetPath(id, start, GantryCraneTargets[GCIndex]);
                path.AddRange(GetPath(id, GantryCraneTargets[GCIndex], PanamaxCraneTargets[PCIndex]));
            }
            else
            {
                var PCIndex = UnityEngine.Random.Range(0, PanamaxCraneTargets.Count);
                var GCIndex = UnityEngine.Random.Range(0, GantryCraneTargets.Count);
                path = GetPath(id, start, PanamaxCraneTargets[PCIndex]);
                path.AddRange(GetPath(id, PanamaxCraneTargets[PCIndex], GantryCraneTargets[GCIndex]));
            }
        }
        else
        {
            var GCIndex = UnityEngine.Random.Range(0, GantryCraneTargets.Count);
            var DIndex = UnityEngine.Random.Range(0, DespawnTargets.Count);
            path = GetPath(id, start, GantryCraneTargets[GCIndex]);
            path.AddRange(GetPath(id, GantryCraneTargets[GCIndex], DespawnTargets[DIndex]));
        }

        return path;
    }

    // A* over all nodes and get the shortest path from start to target.
    public List<WaypointNode> GetPath(int id, WaypointNode start, WaypointNode target)
    {
        // Path map.
        Dictionary<WaypointNode, WaypointNode> parentList = new Dictionary<WaypointNode, WaypointNode>();
        // List of available nodes (not visited).   
        Heap<WaypointNode> openSet = new Heap<WaypointNode>(transform.childCount);
        // List of visited nodes.
        HashSet<WaypointNode> closedSet = new HashSet<WaypointNode>();

        start.GCost[id] = 0;
        start.HCost[id] = start.GetDistance(target);

        openSet.Add(id, start);

        while (openSet.Count > 0)
        {
            WaypointNode current = openSet.RemoveFirst(id);

            // Add to the closed set
            closedSet.Add(current);

            // If target is reached, the path is complete. (Retrace to derive the final path).
            if (current == target)
            {
                return RetracePath(parentList, start, target);
            }

            // Loop through all neighbours of the current node
            foreach (WaypointNode neighbour in current.Neighbours)
            {
                // If the neighbour is in the closed set, it has already been visited.
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                // Calculate the current path distance with the neighbour added. 
                float costToNeighbour = current.GCost.GetValueOrDefault(id, float.MaxValue) + current.GetDistance(neighbour);

                // Check if the neigbour is already in the set. If not, set distances and add it.
                // If the current cost to the neigbour is lower (i.e., shorter path to neighbour found), update the values.
                if (!openSet.Contains(id, neighbour) || costToNeighbour < neighbour.GCost.GetValueOrDefault(id, float.MaxValue))
                {
                    parentList[neighbour] = current;
                    // Calculate costs (G = distance to start, H = distance to target).
                    neighbour.GCost[id] = costToNeighbour;
                    neighbour.HCost[id] = neighbour.GetDistance(target);
                    // Set current node as parent to the neighbour.
                    if (!openSet.Contains(id, neighbour)) {
                        openSet.Add(id, neighbour);
                    }
                }
            }
        }

        return null;
    }

    private List<WaypointNode> RetracePath(Dictionary<WaypointNode, WaypointNode> parentList, WaypointNode start, WaypointNode target)
    {
        List<WaypointNode> path = new List<WaypointNode>();
        WaypointNode current = target;
        while(current != start)
        {
            path.Add(current);
            current = parentList[current];
        }
        path.Reverse();
        return path;
    }

    public void Awake()
    {
        GantryCraneTargets = new List<WaypointNode>();
        PanamaxCraneTargets = new List<WaypointNode>();
        DespawnTargets = new List<WaypointNode>();

        IsLoadMode = PlayerPrefs.GetInt("isLoadMode") == 1;

        //sort all waypoints into proper list
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var node = child.GetComponent<WaypointNode>();

            switch(node.Type)
            {
                case WaypointNode.WaypointType.Despawn:
                    DespawnTargets.Add(node);
                    break;
                case WaypointNode.WaypointType.ShipCrane:
                    PanamaxCraneTargets.Add(node);
                    break;
                case WaypointNode.WaypointType.StorageCrane:
                    GantryCraneTargets.Add(node);
                    break;
                case WaypointNode.WaypointType.ExternalSpawn:
                    ExternalSpawnNode = node;
                    break;
                case WaypointNode.WaypointType.InternalSpawn:
                    InternalSpawnNode = node;
                    break;
                default:
                    break;
            }
        }
    }

    //used when disabling a storage yard, so that node can no longer be targeted by GetRandomPath
    public void RemoveTargets(WaypointNode node)
    {
        GantryCraneTargets.Remove(node);
        PanamaxCraneTargets.Remove(node);
        DespawnTargets.Remove(node);
    }

    //Update names of waypoint children according to type, and remove/add type-specific components
    //Fired by ticking "Auto Rename Waypoints" in editor
    private void OnDrawGizmos()
    {
        if (AutoRenameWaypoints)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).GetComponent<WaypointNode>();

                if (child.Type != WaypointNode.WaypointType.Intersection && child.TryGetComponent<Intersection>(out Intersection intersection))
                {
                    DestroyImmediate(intersection);
                    DestroyImmediate(child.GetComponent<SphereCollider>());
                }
                else if (child.Type == WaypointNode.WaypointType.Intersection && !child.TryGetComponent<Intersection>(out _))
                    child.gameObject.AddComponent<Intersection>();

                child.name = $"WP ({i + 1}) - {child.Type}";
            }
            AutoRenameWaypoints = false;
        }
    }
}
