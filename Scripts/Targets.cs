using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Targets : MonoBehaviour
{
    public PauseMenu pauseMenu;
    public GameObject[] TargetBoxes;
    public GameObject[] TargetBoxesMirror;
    GameObject[] ActiveTargetBoxes;

    public List<float> distances = new();
    public Image[] MapTargets;
    public Image[] MapTargetsMirror;
    Image[] ActiveMapTargets;

    public GameObject TruckTarget;
    public Image TruckMapTarget;
    public Color baseColor;
    public Color highlightColor = Color.white;

    public Image[] MapCells; // NEW: reference to all cells
    public Image[] MapCellsMirror; // NEW: if mirrored
    Image[] ActiveMapCells; // NEW: current array in use

    int targetIndex = 0;
    int targetCount;
    bool firstTarget = true;
    bool anyTargetCompleted = false;

    void Start()
    {
        if (PlayerPrefs.GetInt("Scenario") % 2 == 0)
        {
            ActiveTargetBoxes = TargetBoxes;
            ActiveMapTargets = MapTargets;
            ActiveMapCells = MapCells;
        }
        else
        {
            ActiveTargetBoxes = TargetBoxesMirror;
            ActiveMapTargets = MapTargetsMirror;
            ActiveMapCells = MapCellsMirror;
        }

        foreach (var target in TargetBoxes)
        {
            if (target != null) target.SetActive(false);
        }

        foreach (var target in TargetBoxesMirror)
        {
            if (target != null) target.SetActive(false);
        }

        foreach (var mapTarget in MapTargets)
        {
            if (mapTarget != null) mapTarget.color = baseColor;
        }

        foreach (var mapTarget in MapTargetsMirror)
        {
            if (mapTarget != null) mapTarget.color = baseColor;
        }

        if (ActiveMapCells != null)
        {
            foreach (var cell in ActiveMapCells)
            {
                if (cell != null) cell.color = baseColor;
            }
        }

        if (TruckMapTarget != null)
            TruckMapTarget.color = highlightColor;

        if (TruckTarget != null)
        {
            TruckTarget.GetComponent<MeshRenderer>().enabled = true;
            TruckTarget.SetActive(true);
        }

        targetCount = ActiveTargetBoxes.Length;
    }

    public void TargetReached(GameObject container)
    {
        if (targetIndex >= targetCount) return;

        if (ActiveTargetBoxes == null || ActiveTargetBoxes.Length <= targetIndex || ActiveTargetBoxes[targetIndex] == null)
        {
            Debug.LogWarning("TargetReached: ActiveTargetBoxes[" + targetIndex + "] is null or missing.");
            return;
        }

        if (UserLogs.instance != null)
        {
            UserLogs.instance.AddTargetDistance(Vector3.Distance(container.transform.position, ActiveTargetBoxes[targetIndex].transform.position));
            UserLogs.instance.LapTime();
        }

        if (ActiveMapTargets != null && targetIndex < ActiveMapTargets.Length && ActiveMapTargets[targetIndex] != null)
            ActiveMapTargets[targetIndex].color = baseColor;

        if (ActiveMapCells != null && targetIndex < ActiveMapCells.Length && ActiveMapCells[targetIndex] != null)
            ActiveMapCells[targetIndex].color = baseColor;

        ActiveTargetBoxes[targetIndex].SetActive(false);

        if (TruckMapTarget != null)
            TruckMapTarget.color = highlightColor;

        if (TruckTarget != null)
            TruckTarget.SetActive(true);

        anyTargetCompleted = true;
    }

    public void NextTarget()
    {
        if (firstTarget)
        {
            firstTarget = false;
            targetIndex = 0;

            if (UserLogs.instance != null)
                UserLogs.instance.StartTaskTimer();
        }
        else
        {
            targetIndex++;

            if (UserLogs.instance != null)
                UserLogs.instance.LapTime();
        }

        if (targetIndex >= targetCount)
        {
            Debug.Log("All targets completed.");

            if (UserLogs.instance != null)
            {
                UserLogs.instance.EndTaskTimer();

                if (anyTargetCompleted)
                    UserLogs.instance.OutputLog();
                else
                    Debug.LogWarning("No targets were completed — skipping log output.");
            }

            if (pauseMenu != null)
                pauseMenu.ManualTogglePauseMenu();

            return;
        }

        if (ActiveMapCells != null)
        {
            foreach (var cell in ActiveMapCells)
            {
                if (cell != null) cell.color = baseColor;
            }

            if (targetIndex < ActiveMapCells.Length && ActiveMapCells[targetIndex] != null)
                ActiveMapCells[targetIndex].color = highlightColor;
        }

        if (ActiveMapTargets != null && targetIndex < ActiveMapTargets.Length && ActiveMapTargets[targetIndex] != null)
            ActiveMapTargets[targetIndex].color = highlightColor;

        if (ActiveTargetBoxes != null && targetIndex < ActiveTargetBoxes.Length && ActiveTargetBoxes[targetIndex] != null)
            ActiveTargetBoxes[targetIndex].SetActive(true);
        else
            Debug.LogWarning("Next target is missing or null at index: " + targetIndex);

        if (TruckMapTarget != null)
            TruckMapTarget.color = baseColor;

        if (TruckTarget != null)
            TruckTarget.SetActive(false);
    }
}
