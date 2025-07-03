using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Diagnostics;
using System;

public class ExportStatistics : MonoBehaviour
{
    public GameObject[] TextBoxes;
    Stopwatch stopwatch = new Stopwatch();

    public Stopwatch ThroughPutTime = new Stopwatch();
    int ContainerThroughPutAmount = 0;
    float ContainersPerMinute = -1;
    public int Minutes = 0;

    int count = 0;
    long AverageSecs = 0;

    //Dictionary that keeps track of individual containers and their respective waiting times.
    private Dictionary<int, float> ContainerTimeTable;

    //Keeps track of Total amount of containers that has passed through the simulation and the time they have waited in the storage yard.
    private struct TimeWaited
    {
        public int Containers;
        public float Time;
    }
    private TimeWaited TimeAverageContainerWait = new TimeWaited();
    public Dictionary<int, float> GetTimeTable() => ContainerTimeTable;

    public float GetThroughPutTime() => ContainersPerMinute;

    public void IncrementContainerThroughPutAmount() => ContainerThroughPutAmount++;

    public void AddToStruct(int id) 
    {
        TimeAverageContainerWait.Time += (Time.time - ContainerTimeTable[id]);
        TimeAverageContainerWait.Containers++;
    }
    public void StartContainerStartTimeMeasure()
    {
        TimeAverageContainerWait.Containers = 0;
        TimeAverageContainerWait.Time = 0f;
        ContainerTimeTable = new Dictionary<int, float>();
        ThroughPutTime.Start();
    }

    /// <summary>
    /// Saves the statistics to a TXT file. 
    /// </summary>
    public void ExportStatistic()
    {
        using (StreamWriter writer = new StreamWriter(@".\Statistics\Statistics.txt"))
        {
            foreach (var item in TextBoxes)
            {
                TMP_Text textelement = item.GetComponent<TMP_Text>();
                string text = textelement.text;
                writer.WriteLine(text);
                UnityEngine.Debug.Log(text);
            }
        }

    }

    /// <summary>
    /// Starts time measurement that is used in statistics for average ship waiting time. Called by the Ship.
    /// </summary>
    public void StartTimeMeasure()
    {
        stopwatch.Start();
    }
    
    /// <summary>
    /// Stops time measurement that is used in statistics for average ship waiting time. Called by the Ship.
    /// </summary>
    public void StopMeasureTime()
    {
        count++;
        stopwatch.Stop();
        AverageSecs += (stopwatch.ElapsedMilliseconds / 1000);
        stopwatch.Reset();
    }

    /// <summary>
    /// Getter function used in PauseMenu.cs. Prints the average ship waiting time to be used in the textbox.
    /// </summary>
    /// <returns>String</returns>
    public string GetShipTimeString()
    {
        if (count == 0)
        {
            return "No Data";
        }
        var Average = AverageSecs / count;
        return "Average ship wait time: " + (int)(Average / 60) + " Minutes, " + (int)((Average % 60)) + " Seconds";
    }

    /// <summary>
    /// Getter function used in PauseMenu.cs. Prints the average Container Wait Time to be used in the textbox.
    /// </summary>
    /// <returns>String</returns>

    public string GetContainerWaitTimeString()
    {
        if (TimeAverageContainerWait.Containers == 0)
        {
            return "No Data";
        }
        var Average = TimeAverageContainerWait.Time / TimeAverageContainerWait.Containers;
        return "Average container wait time: " + (int)(Average / 60) + " Minutes, " + (int)((Average % 60)) + " Seconds";
    }

    /// <summary>
    /// Getter function used in PauseMenu.cs. Prints the average Container Throughput to be used in the textbox.
    /// </summary>
    /// <returns>String</returns>

    public string GetContainerThroughPutString()
    {
        if (Minutes == 0)
        {
            return "No Data";
        }

        return "Average container throughput: " + String.Format("{0:0.00}", (float)ContainerThroughPutAmount / Minutes)  + " per minute";
    }
}



