using System.Collections;
using UnityEngine;

public class ContainerCollisionScript : MonoBehaviour
{
    public float cooldownTime = 1f;
    private bool inCooldown;

    private IEnumerator Cooldown()
    {
        inCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        inCooldown = false;
    }

    private void Start()
    {
        StartCoroutine(Cooldown());
    }

    private void OnCollisionEnter(Collision collision)
    {
        float audioLevel = collision.relativeVelocity.magnitude / 5.0f; // Can rename if not used for audio

        if (collision.gameObject.tag == "CraneSpreader")
        {
            Debug.Log("Crane spreader collision detected");
            if (UserLogs.instance != null)
                UserLogs.instance.IncreaseSpreaderCollision();
        }

        if (inCooldown)
            return;

        if (gameObject.tag == "PickedUpContainer" && collision.gameObject.tag == "Container")
        {
            Debug.Log("Container collision detected, level: " + audioLevel);
            if (UserLogs.instance != null)
                UserLogs.instance.IncreaseContainerContainerCollision();
        }
        else if (collision.gameObject.name == "Platform")
        {
            Debug.Log("Platform collision detected, level: " + audioLevel);
            if (UserLogs.instance != null)
                UserLogs.instance.IncreaseContainerPlatformCollision();
        }

        StartCoroutine(Cooldown());
    }
}
