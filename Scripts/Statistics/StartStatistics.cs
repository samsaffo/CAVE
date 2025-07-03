using UnityEngine;

public class StartStatistics : MonoBehaviour
{
    public ExportStatistics statistics;
    // Starts the Statistics Script (Since the gameobject the script resides within is inactive, start in that script will not be activated)
    void Start()
    {
        //statistics.StartContainerStartTimeMeasure();
    }

    // Keeps count of the amount of minutes the simulation has been running for.
    void Update()
    {
        //if (statistics.ThroughPutTime.Elapsed.Minutes >= 1)
        //{
        //    statistics.ThroughPutTime.Restart();
        //    statistics.Minutes++;
        //}
    }
}
