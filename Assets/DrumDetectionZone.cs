using UnityEngine;

public class DrumDetectionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StickR"))
        {
            Debug.Log("Right stick hit the drum! " + other.name);
        }
        if (other.CompareTag("StickL"))
        {
            Debug.Log("Left stick hit the drum! " + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("StickR"))
        {
            Debug.Log("Right stick left the drum! " + other.name);
        }
        if (other.CompareTag("StickL"))
        {
            Debug.Log("Left stick left the drum! " + other.name);
        }
    }
}