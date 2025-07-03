using UnityEngine.UI;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class ToggleAction : MonoBehaviour
{
    Toggle m_Toggle;
    public AudioSource[] audio_sources;
    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Toggle GameObject
        m_Toggle = GetComponent<Toggle>();
        //Add listener for when the state of the Toggle changes, to take action
        m_Toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(m_Toggle);
        });
    }

    // Update is called once per frame
    void ToggleValueChanged(Toggle change)
    {
        foreach(var source in audio_sources)
        {
            source.mute = !change.isOn;
        }
    }
}
