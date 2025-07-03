using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

public class UserTestLog : MonoBehaviour
{
    Stopwatch taskTimer;
    Stopwatch totalTimer;
    List<TimeSpan> times = new List<TimeSpan>();
    List<float> targetDistances = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTaskTimer()
    {
        taskTimer = Stopwatch.StartNew();
        totalTimer = Stopwatch.StartNew();
        UnityEngine.Debug.Log("Timer started");
    }

    public void EndTaskTimer()
    {
        taskTimer.Stop();
        totalTimer.Stop();
    }

    public void LapTime()
    {
        taskTimer.Stop();
        times.Add(taskTimer.Elapsed);
        taskTimer.Restart();
    }

    public void AddTargetDistance(float dist)
    {
        targetDistances.Add(dist);
    }

    public void OutputLog()
    {
        var playerName = PlayerPrefs.GetString("PlayerName");
        var path = Path.Combine(Application.dataPath,"../TestLog_" + playerName + ".txt");
        var file = System.IO.File.AppendText(path);

        string output = "Player: " + playerName + ", Scenario: " + PlayerPrefs.GetInt("Scenario").ToString() + "\n";
        output += "Total Time: " + totalTimer.Elapsed.Minutes + " m " + totalTimer.Elapsed.Seconds + " s " + totalTimer.Elapsed.Milliseconds + " ms\n";

        TimeSpan total_time = new TimeSpan(0);
        float total_target_distances = 0;
        for (int i = 0; i < times.Count; ++i)
        {
            output += "#" + i + " Time: " + times[i].Minutes + " m " + times[i].Seconds + " s " + times[i].Milliseconds + " ms\n";
            output += "#" + i + " Distance: " + targetDistances[i].ToString() + "\n";

            total_time += times[i];
            total_target_distances += targetDistances[i];
        }

        TimeSpan average_time = TimeSpan.FromMilliseconds(total_time.TotalMilliseconds / times.Count);
        output += $"Average Time: {average_time.Minutes} m {average_time.Seconds} s {average_time.Milliseconds} ms\n";
        output += $"Average Target Distances: {total_target_distances / times.Count}\n";

        UnityEngine.Debug.Log(output);
        file.WriteLine(output);
        file.Close();
    }
}
