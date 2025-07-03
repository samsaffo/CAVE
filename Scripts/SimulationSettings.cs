using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSettings : MonoBehaviour
{
    public enum FrameLimits
    {
        NoLimit = 0,
        FPS10 = 10,
        FPS30 = 30,
        FPS60 = 60,
        FPS120 = 120
    };
    public FrameLimits limit;

    private void Awake()
    {
        Application.targetFrameRate = (int)limit;
        Screen.SetResolution(1920, 1080, true);
    }
}
