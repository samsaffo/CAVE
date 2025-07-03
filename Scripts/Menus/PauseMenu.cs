using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

/*Link actions to buttons in pause menu and input manager*/
public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuPanel;
    //public GameObject StatisticsPanel;
    //public GameObject[] TextBoxes;

    public bool Paused = false;

    public void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //if (StatisticsPanel != null && StatisticsPanel.activeSelf)
            //{
            //    OnClickStatistics();
            //}
            //else
            //{
                Paused = !Paused;
                PauseMenuPanel.SetActive(Paused);
                Time.timeScale = Paused ? 0f : 1f;
            //}
        }
    }

    public void ManualTogglePauseMenu()
    {
        Paused = !Paused;
        PauseMenuPanel.SetActive(Paused);
        Time.timeScale = Paused ? 0f : 1f;
    }

    // Pause menu panel events
    public void OnClickStatistics()
    {
        PauseMenuPanel.SetActive(!PauseMenuPanel.activeSelf);
        //StatisticsPanel.SetActive(!StatisticsPanel.activeSelf);
        //TextBoxes[0].GetComponent<TMP_Text>().text = StatisticsPanel.transform.GetChild(5).gameObject.GetComponent<ExportStatistics>().GetShipTimeString();
        //TextBoxes[1].GetComponent<TMP_Text>().text = StatisticsPanel.transform.GetChild(5).gameObject.GetComponent<ExportStatistics>().GetContainerWaitTimeString();
        //TextBoxes[2].GetComponent<TMP_Text>().text = StatisticsPanel.transform.GetChild(5).gameObject.GetComponent<ExportStatistics>().GetContainerThroughPutString();

    }

    public void OnClickReturn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void OnClickX()
        {
            Paused = !Paused;
        PauseMenuPanel.SetActive(Paused);
            Time.timeScale = Paused ? 0f : 1f;
        }

    //Statistics panel events
    public void OnClickExport() 
    { 

    }
}
