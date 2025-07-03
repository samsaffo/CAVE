using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

enum Mode
{
    Slider,
    Text
}
/*Link actions to buttons and sliders in main menu*/

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject SettingsPanel;
    public GameObject CallibrationPanel;

    public Button LoadBtn;
    public Button UnloadBtn;

    //CAVE Settings Button
    public Button FrontCalb;
    public Button DownCalb;
    public Button RightCalb;
    public Button LeftCalb;
    public Button ResetCalb;
    //CAVE Cameras up right left down
    public Camera FrontCamera;
    public Camera RightCamera;
    public Camera DownCamera;
    public Camera LeftCamera;
    private Camera activeCalibCamera;
    private bool isCalibrating = false;



    public Slider ITruckSlider;
    public TMP_InputField ITruckInput;

    public Slider ETruckSlider;
    public TMP_InputField ETruckInput;

    public Slider ShipSlider;
    public TMP_InputField ShipInput;

    public Slider ContainerSlider;
    public TMP_InputField ContainerInput;

    public Slider StorageYardSlider;
    public TMP_InputField StorageYardInput;

    public bool isLoadMode = true;

    private ColorBlock ActiveColors;
    private ColorBlock InactiveColors;

    [SerializeField] private Limits myLimits;

    private bool IsStarting = true;

    private void OpenLimitWindow(string message)
    {
        myLimits.gameObject.SetActive(true);
        myLimits.okButton.onClick.AddListener(OKClicked);
        myLimits.messageText.text = message;
    }

    private void OKClicked()
    {
        myLimits.gameObject.SetActive(false);
    }

    public void Start()
    {
        ActiveColors = LoadBtn.colors;
        InactiveColors = UnloadBtn.colors;

        if (PlayerPrefs.GetInt("isLoadMode") == 1)
        {
            OnClickLoad();
        }
        else
        {
            OnClickUnload();

        }

        ITruckSlider.value = PlayerPrefs.GetInt("InternalTruckCount");
        ETruckSlider.value = PlayerPrefs.GetInt("ExternalTruckFrequency");
        ShipSlider.value = PlayerPrefs.GetInt("ShipFrequency");
        ContainerSlider.value = PlayerPrefs.GetInt("ContainerSpawnAmount");
        StorageYardSlider.value = PlayerPrefs.GetInt("StorageYardAmount");

        IsStarting = false;
    }

    // Main menu panel events
    public void OnClickSettings()
    {
        SettingsPanel.SetActive(!SettingsPanel.activeSelf);
        MainMenuPanel.SetActive(!MainMenuPanel.activeSelf);
        CallibrationPanel.SetActive(!CallibrationPanel.activeSelf);
    }

    public void StartSimulation()
    {
        PlayerPrefs.SetInt("isLoadMode", isLoadMode ? 1 : 0);
        PlayerPrefs.SetInt("InternalTruckCount", value: (int)ITruckSlider.value);
        PlayerPrefs.SetInt("ExternalTruckFrequency", value: (int)ETruckSlider.value);
        PlayerPrefs.SetInt("ShipFrequency", value: (int)ShipSlider.value);
        PlayerPrefs.SetInt("ContainerSpawnAmount", value: (int)ContainerSlider.value);
        PlayerPrefs.SetInt("StorageYardAmount", value: (int)StorageYardSlider.value);

        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }


    // Settings menu panel events
    public void OnClickLoad()
    {
        if (!isLoadMode)
        {
            LoadBtn.colors = ActiveColors;
            UnloadBtn.colors = InactiveColors;
            isLoadMode = true;
        }
    }

    public void OnClickUnload()
    {
        if (isLoadMode)
        {
            UnloadBtn.colors = ActiveColors;
            LoadBtn.colors = InactiveColors;
            isLoadMode = false;
        }
    }
    //CAVE Calibration
    public void OnClickFront()
    {
        PlayerPrefs.SetString("SelectedCamera", "Front");
        PlayerPrefs.Save();
        SceneManager.LoadScene("CaveTestScene");
    }

    public void OnClickRight()
    {
        PlayerPrefs.SetString("SelectedCamera", "Right");
        PlayerPrefs.Save();
        SceneManager.LoadScene("CaveTestScene");
    }

    public void OnClickDown()
    {
        PlayerPrefs.SetString("SelectedCamera", "Down");
        PlayerPrefs.Save();
        SceneManager.LoadScene("CaveTestScene");
    }

    public void OnClickLeft()
    {
        PlayerPrefs.SetString("SelectedCamera", "Left");
        PlayerPrefs.Save();
        SceneManager.LoadScene("CaveTestScene");
    }
    public void onClickReset()
    {
        // Clear all camera settings before saving new defaults
        ClearCameraSettings("Front");
        ClearCameraSettings("Right");
        ClearCameraSettings("Left");
        ClearCameraSettings("Down");
        // Save default camera settings to PlayerPrefs
        SaveCameraSettings("Front", new Vector3(0, -1.81f, -2.81f), new Vector3(0, 0, 0), 110);
        SaveCameraSettings("Right", new Vector3(0, -1.81f, -2.81f), new Vector3(0, 90, 0), 111);
        SaveCameraSettings("Left", new Vector3(0, -1.81f, -2.81f), new Vector3(0, -90, 0), 111);
        SaveCameraSettings("Down", new Vector3(0, -1.81f, -2.81f), new Vector3(90, 0, 0), 78);

        PlayerPrefs.Save();
        Debug.Log("Camera settings reset to default.");

        SceneManager.LoadScene("CaveTestScene");
    }

    private void SaveCameraSettings(string label, Vector3 position, Vector3 rotation, float fov)
    {
        // Clear old settings before saving new ones
        ClearCameraSettings(label);
        // Save position
        PlayerPrefs.SetFloat($"CamPosX_{label}", position.x);
        PlayerPrefs.SetFloat($"CamPosY_{label}", position.y);
        PlayerPrefs.SetFloat($"CamPosZ_{label}", position.z);

        // Save rotation
        PlayerPrefs.SetFloat($"CamRotX_{label}", rotation.x);
        PlayerPrefs.SetFloat($"CamRotY_{label}", rotation.y);
        PlayerPrefs.SetFloat($"CamRotZ_{label}", rotation.z);

        // Save FOV
        PlayerPrefs.SetFloat($"CamFOV_{label}", fov);

        // Debug log to confirm the saved values
        Debug.Log($"Saved {label} Camera Settings: Position (X={position.x}, Y={position.y}, Z={position.z}), " +
                  $"Rotation (X={rotation.x}, Y={rotation.y}, Z={rotation.z}), FOV={fov}");
    }

    private void ActivateCamera(Camera cam)
    {
        // Disable all calibration cameras first
        FrontCamera.gameObject.SetActive(false);
        RightCamera.gameObject.SetActive(false);
        DownCamera.gameObject.SetActive(false);
        LeftCamera.gameObject.SetActive(false);

        // Activate the selected one
        cam.gameObject.SetActive(true);
        activeCalibCamera = cam;
        isCalibrating = true;
    }
    void Update()
    {
        if (isCalibrating && activeCalibCamera != null)
        {
            float rotationSpeed = 50f;

            if (Input.GetKey(KeyCode.U))
                activeCalibCamera.transform.Rotate(Vector3.right, -rotationSpeed * Time.deltaTime); // Up
            if (Input.GetKey(KeyCode.J))
                activeCalibCamera.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime); // Down
            if (Input.GetKey(KeyCode.H))
                activeCalibCamera.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime); // Left
            if (Input.GetKey(KeyCode.K))
                activeCalibCamera.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // Right
        }
    }

    //-------------------------------------------------------------------------------------

    Double ConvertTextToDouble(string input)
    {
        double num;

        if (!Double.TryParse(input, out double number))
        {
            OpenLimitWindow("Invalid input");
            return 1.0f;
        }
        else
        {
            num = Double.Parse(input);
            if (num < 0 || num > 99)
            {
                OpenLimitWindow("Number out of range. Please choose a number between 0 and 99.");
                return 1;
            }
            return num;
        }
    }

    private void ITruckChange(Mode m)
    {
        if (IsStarting)
        {
            return;
        }

        double val;
        if (m.Equals(Mode.Text))
        {
            if (!Double.TryParse(ITruckInput.text, out double number))
            {
                ITruckSlider.value = 1;
                ITruckInput.text = ITruckSlider.value.ToString();
                OpenLimitWindow("Invalid input");
            }
            val = Double.Parse(ITruckInput.text);
        }
        else
            val = (int)ITruckSlider.value;


        if (val > ContainerSlider.value)
        {
            ITruckSlider.value = ContainerSlider.value;
            ITruckInput.text = ITruckSlider.value.ToString();
            OpenLimitWindow("Number of internal trucks must be lower or equal to the number of containers.");
        }
        else
        {
            ITruckSlider.value = (float)val; ;
            ITruckInput.text = val.ToString();
        }
    }


    public void OnITruckSliderMove()
    {
        ITruckChange(Mode.Slider);
    }

    public void OnITruckInputChange()
    {
        ITruckChange(Mode.Text);
    }

    public void OnETruckSliderMove()
    {
        ETruckInput.text = ETruckSlider.value.ToString();
    }

    public void OnETruckInputChange()
    {
        double number = ConvertTextToDouble(ETruckInput.text);

        ETruckSlider.value = (float)number;
    }

    public void OnShipSliderMove()
    {
        ShipInput.text = ShipSlider.value.ToString();
    }

    public void OnShipInputChange()
    {
        double number;
        if (!Double.TryParse(ShipInput.text, out number))
        {
            ShipSlider.value = 1;
            ShipInput.text = ShipSlider.value.ToString();
            OpenLimitWindow("Invalid input");
        }


        if (int.Parse(ShipInput.text) > 0)
        {
            ShipSlider.value = int.Parse(ShipInput.text);
        }
        else
        {
            ShipSlider.value = 1;
            ShipInput.text = ShipSlider.value.ToString();
            OpenLimitWindow("Invalid input");
        }

    }

    public void OnContainerInputChange()
    {
        double number = ConvertTextToDouble(ContainerInput.text);
        if (number < ITruckSlider.value)
        {
            ContainerSlider.value = ITruckSlider.value;
            ContainerInput.text = ContainerSlider.value.ToString();
            OpenLimitWindow("Number of containers must be higher or equal to the number of internal trucks.");
        }
        else
            ContainerSlider.value = (float)number;
    }
    public void OnContainerSliderMove()
    {
        if (ContainerSlider.value < ITruckSlider.value)
        {
            ContainerSlider.value = ITruckSlider.value;
            ContainerInput.text = ContainerSlider.value.ToString();
            OpenLimitWindow("Number of containers must be higher or equal to the number of internal trucks.");
        }
        else
        {
            ContainerInput.text = ContainerSlider.value.ToString();
        }

    }

    public void OnStorageYardInputChange()
    {

        double number;
        if (!Double.TryParse(StorageYardInput.text, out number))
        {
            StorageYardSlider.value = 1;
            StorageYardInput.text = StorageYardSlider.value.ToString();
            OpenLimitWindow("Invalid input");
        }

        if (int.Parse(StorageYardInput.text) > 0 && int.Parse(StorageYardInput.text) <= 8)
        {
            StorageYardSlider.value = int.Parse(StorageYardInput.text);
        }
        else if (int.Parse(StorageYardInput.text) > 8)
        {
            StorageYardSlider.value = 8;
            StorageYardInput.text = StorageYardSlider.value.ToString();
            OpenLimitWindow("Invalid input");
        }
        else
        {
            StorageYardSlider.value = 1;
            StorageYardInput.text = StorageYardSlider.value.ToString();
            OpenLimitWindow("Invalid input");
        }

    }
    public void OnStorageYardSliderMove()
    {
        StorageYardInput.text = StorageYardSlider.value.ToString();

    }

    public void Quit()
    {
        Application.Quit();
    }
    // Method to clear existing camera settings from PlayerPrefs
    private void ClearCameraSettings(string label)
    {
        PlayerPrefs.DeleteKey($"CamPosX_{label}");
        PlayerPrefs.DeleteKey($"CamPosY_{label}");
        PlayerPrefs.DeleteKey($"CamPosZ_{label}");

        PlayerPrefs.DeleteKey($"CamRotX_{label}");
        PlayerPrefs.DeleteKey($"CamRotY_{label}");
        PlayerPrefs.DeleteKey($"CamRotZ_{label}");

        PlayerPrefs.DeleteKey($"CamFOV_{label}");

        Debug.Log($"Cleared {label} Camera Settings from PlayerPrefs.");
    }
}
