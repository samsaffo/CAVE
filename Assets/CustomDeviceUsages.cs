using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public static class InitCustomDeviceUsages
{
    static InitCustomDeviceUsages()
    {
        Initialize();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        InputSystem.RegisterLayoutOverride(@"
            {
                ""name"" : ""JoystickConfigurationUsageTags"",
                ""extend"" : ""Joystick"",
                ""commonUsages"" : [
                    ""LeftHand"", ""RightHand""
                ]
            }
        ");
    }
}

public class CustomDeviceUsages : MonoBehaviour
{
    // private Joystick m_LeftJoystick, m_RightJoystick;

    //protected void OnEnable()
    //{
      //  m_LeftJoystick = Joystick.all[0];
        //m_RightJoystick = Joystick.all[1];
        //InputSystem.AddDeviceUsage(m_LeftJoystick, CommonUsages.LeftHand);
        //InputSystem.AddDeviceUsage(m_RightJoystick, CommonUsages.RightHand);
    //}

//    protected void OnDisable()
 //   {
   //     if (m_LeftJoystick != null && m_LeftJoystick.added)
     //       InputSystem.RemoveDeviceUsage(m_LeftJoystick, CommonUsages.LeftHand);
       // m_LeftJoystick = null;
        //if (m_RightJoystick != null && m_RightJoystick.added)
          //  InputSystem.RemoveDeviceUsage(m_RightJoystick, CommonUsages.RightHand);
        //m_RightJoystick = null;
    //}
}
