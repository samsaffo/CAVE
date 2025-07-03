using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class UserLogs : MonoBehaviour
{
    public static UserLogs instance;

    private void Awake()
    {
        instance = this;
    }

    Stopwatch taskTimer;
    Stopwatch totalTimer;
    List<TimeSpan> times = new List<TimeSpan>();
    List<float> targetDistances = new List<float>();

    int spreader_collisions;
    int container_container_collisions;
    int container_platform_collisions;

    public void IncreaseSpreaderCollision() => spreader_collisions++;
    public void IncreaseContainerContainerCollision() => container_container_collisions++;
    public void IncreaseContainerPlatformCollision() => container_platform_collisions++;

    public void StartTaskTimer()
    {
        taskTimer = Stopwatch.StartNew();
        totalTimer = Stopwatch.StartNew();
        UnityEngine.Debug.Log("Timer started");
    }

    public void EndTaskTimer()
    {
        if (taskTimer != null) taskTimer.Stop();
        if (totalTimer != null) totalTimer.Stop();
    }

    public void LapTime()
    {
        if (taskTimer == null) return;
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
        DateTime now = DateTime.Now;
        string formattedDate = now.ToString("yy-MM-dd HH:mm:ss");

        string playerName = PlayerPrefs.GetString("PlayerName", "Unknown");
        string path = Path.Combine(Application.dataPath, "../TestLog_" + playerName + ".txt");
        StreamWriter file = File.AppendText(path);

        string output = "Player: " + playerName + ", Scenario: " + PlayerPrefs.GetInt("Scenario").ToString() + "\n";
        output += "Current time: " + formattedDate + "\n";
        output += "Total Time: " + totalTimer.Elapsed.Minutes + " m " + totalTimer.Elapsed.Seconds + " s " + totalTimer.Elapsed.Milliseconds + " ms\n";

        TimeSpan total_time = new TimeSpan(0);
        float total_target_distances = 0;

        int count = Mathf.Min(times.Count, targetDistances.Count);

        for (int i = 0; i < count; i++)
        {
            output += "#" + i + " Time: " + times[i].Minutes + " m " + times[i].Seconds + " s " + times[i].Milliseconds + " ms\n";
            output += "#" + i + " Distance: " + targetDistances[i] + "\n";

            total_time += times[i];
            total_target_distances += targetDistances[i];
        }

        TimeSpan average_time = count > 0
            ? TimeSpan.FromMilliseconds(total_time.TotalMilliseconds / count)
            : TimeSpan.Zero;

        output += "Container-Container Collisions: " + container_container_collisions + "\n";
        output += "Container-Platform Collisions: " + container_platform_collisions + "\n";
        output += "Spreader Collisions: " + spreader_collisions + "\n";
        output += "Average Time: " + average_time.Minutes + "m " + average_time.Seconds + "s " + average_time.Milliseconds + "ms\n";
        output += "Average Target Distances: " + (count > 0 ? (total_target_distances / count).ToString() : "N/A") + "\n";

        UnityEngine.Debug.Log(output);
        file.WriteLine(output);
        file.Close();
    }
}
