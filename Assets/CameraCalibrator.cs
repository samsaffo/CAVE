// CameraCalibrator.cs with fixed local position handling
using UnityEngine;
using TMPro;
using System.IO;

[System.Serializable]
public class CameraData
{
    public Vector3 position;
    public Vector3 rotation;
    public float fieldOfView;
}

[System.Serializable]
public class CameraState
{
    public CameraData FrontCamera;
    public CameraData RightCamera;
    public CameraData DownCamera;
    public CameraData LeftCamera;
}

public class CameraCalibrator : MonoBehaviour
{
    public Camera FrontCamera;
    public Camera RightCamera;
    public Camera DownCamera;
    public Camera LeftCamera;
    public TextMeshProUGUI debugText;
    public Camera activeCamera;

    private Vector3 lastMousePosition;
    private bool isRotating = false;
    private float mouseSensitivity = 0.2f;
    private float moveSpeed = 1.0f;
    private string savePath = "Assets/saved-position/camera_state.json";

    void Start()
    {
        string selected = PlayerPrefs.GetString("SelectedCamera");
        FrontCamera.gameObject.SetActive(true);
        RightCamera.gameObject.SetActive(true);
        DownCamera.gameObject.SetActive(true);
        LeftCamera.gameObject.SetActive(true);
        LoadCameraState();

        switch (selected)
        {
            case "Front": activeCamera = FrontCamera; break;
            case "Right": activeCamera = RightCamera; break;
            case "Down": activeCamera = DownCamera; break;
            case "Left": activeCamera = LeftCamera; break;
        }
    }

    void Update()
    {
        if (activeCamera == null) return;

        // rotation adjustment
        if (Input.GetMouseButtonDown(0))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0)) isRotating = false;

        if (isRotating)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            activeCamera.transform.Rotate(Vector3.right, delta.y * mouseSensitivity);
            activeCamera.transform.Rotate(Vector3.up, -delta.x * mouseSensitivity, Space.World);
            lastMousePosition = Input.mousePosition;
        }

        // position adjustment
        if (Input.GetKey(KeyCode.Keypad4))
            activeCamera.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Keypad6))
            activeCamera.transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Keypad5))
            activeCamera.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Keypad2))
            activeCamera.transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Keypad1))
            activeCamera.transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Keypad3))
            activeCamera.transform.position += Vector3.back * moveSpeed * Time.deltaTime;

        // Save/Load with keys
        if (Input.GetKeyDown(KeyCode.S)) SaveCameraState();
        if (Input.GetKeyDown(KeyCode.L)) LoadCameraState();

        //fov
        if (Input.GetKeyDown(KeyCode.F))
        {
            activeCamera.fieldOfView += 20f * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            activeCamera.fieldOfView -= 20f * Time.deltaTime;
        }
        DisplayCameraInfo();
    }

    private void SaveCameraState()
    {
        CameraState state = new CameraState
        {
            FrontCamera = GetCameraData(FrontCamera),
            RightCamera = GetCameraData(RightCamera),
            DownCamera = GetCameraData(DownCamera),
            LeftCamera = GetCameraData(LeftCamera)
        };

        Directory.CreateDirectory(Path.GetDirectoryName(savePath));
        File.WriteAllText(savePath, JsonUtility.ToJson(state, true));
        Debug.Log("Camera state saved to " + savePath);
    }

    private void LoadCameraState()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            CameraState state = JsonUtility.FromJson<CameraState>(json);
            SetCameraData(FrontCamera, state.FrontCamera);
            SetCameraData(RightCamera, state.RightCamera);
            SetCameraData(DownCamera, state.DownCamera);
            SetCameraData(LeftCamera, state.LeftCamera);
            Debug.Log("Camera state loaded from " + savePath);
        }
        else
        {
            Debug.LogWarning("Camera state file not found: " + savePath);
        }
    }

    private CameraData GetCameraData(Camera cam)
    {
        return new CameraData
        {
            position = cam.transform.parent != null ? cam.transform.localPosition : cam.transform.position,
            rotation = cam.transform.eulerAngles,
            fieldOfView = cam.fieldOfView
        };
    }

    private void SetCameraData(Camera cam, CameraData data)
    {
        if (data != null)
        {
            if (cam.transform.parent != null)
            {
                cam.transform.localPosition = data.position;
                Debug.Log($"[Child] Loaded {cam.name} Local Position: {cam.transform.localPosition}");
            }
            else
            {
                cam.transform.position = data.position;
                Debug.Log($"[World] Loaded {cam.name} Position: {cam.transform.position}");
            }

            cam.transform.rotation = Quaternion.Euler(data.rotation);
            cam.fieldOfView = data.fieldOfView;

            Debug.Log($"Loaded {cam.name} - Rotation: {cam.transform.eulerAngles}, FOV: {cam.fieldOfView}");
        }
    }

    void DisplayCameraInfo()
    {
        Vector3 Frot = FrontCamera.transform.eulerAngles;
        Vector3 Drot = DownCamera.transform.eulerAngles;
        Vector3 Rrot = RightCamera.transform.eulerAngles;
        Vector3 Lrot = LeftCamera.transform.eulerAngles;

        debugText.text = $"F: X={Frot.x:F2} Y={Frot.y:F2} Z={Frot.z:F2} | " +
                         $"D: X={Drot.x:F2} Y={Drot.y:F2} Z={Drot.z:F2} | " +
                         $"R: X={Rrot.x:F2} Y={Rrot.y:F2} Z={Rrot.z:F2} | " +
                         $"L: X={Lrot.x:F2} Y={Lrot.y:F2} Z={Lrot.z:F2}";
    }
}
