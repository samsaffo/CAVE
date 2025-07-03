using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class InitializeGroundStorageYard : MonoBehaviour
{
    public GameObject[] containers;
    System.Random r = new System.Random();
    public ExportStatistics ExportStatistics;


    public void InitializeGroundStorage(Tuple<ContainerYardScript, float> Info)
    {
        Vector3? NotNullPosition;
        for (int i = 0; i < Info.Item2; i++)
        {
            if((NotNullPosition = Info.Item1.AskPlace()) != null)
            {
                var obj = Instantiate(containers[r.Next() % containers.Length], (Vector3)NotNullPosition, Info.Item1.transform.rotation);
                obj.transform.SetParent(Info.Item1.gameObject.transform);
                ExportStatistics.GetTimeTable().Add(obj.GetInstanceID(),Time.time + 0.01f);
                
            }
        }
    }
}
