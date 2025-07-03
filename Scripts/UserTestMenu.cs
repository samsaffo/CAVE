using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;



public class UserTestMenu : MonoBehaviour
{
    //public GameObject MainMenuPanel;

    public TMP_InputField PlayerNameInput;
    public Button[] Scenario_btns;
    public ColorBlock BtnActiveColor;
    public ColorBlock BtnInactiveColor;
    private int scenarioSelected;



    public void Start()
    {
        foreach (var btn in Scenario_btns)
        {
            btn.colors = BtnInactiveColor;
        }

        var namePreset = PlayerPrefs.GetString("PlayerName");
        if (namePreset != string.Empty)
        {
            PlayerNameInput.text = namePreset;
        }

        OnClickScenario("1");
    }


    public void StartSimulation()
    {
        PlayerPrefs.SetInt("isLoadMode", 1);
        PlayerPrefs.SetInt("InternalTruckCount", 0);
        PlayerPrefs.SetInt("ExternalTruckFrequency",  1);
        PlayerPrefs.SetInt("ShipFrequency", 1);
        PlayerPrefs.SetInt("ContainerSpawnAmount", 0);
        PlayerPrefs.SetInt("StorageYardAmount", 1);


        PlayerPrefs.SetInt("Scenario", scenarioSelected);
        PlayerPrefs.SetString("PlayerName", PlayerNameInput.text);

        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }


    // Settings menu panel events
    public void OnClickScenario(string num)
    {
        scenarioSelected = Int32.Parse(num);
        Debug.Log(scenarioSelected);

        foreach (var btn in Scenario_btns)
        {
            btn.colors = BtnInactiveColor;
        }
        Scenario_btns[scenarioSelected-1].colors = BtnActiveColor;
    }




    public void Quit()
    {
        Application.Quit();
    }
}
