using UnityEngine;
using System.Collections;

public class Enable_multiple_display : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Displays found: " + Display.displays.Length);

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
            Debug.Log("Activated display: " + i);
        }
    }
}
