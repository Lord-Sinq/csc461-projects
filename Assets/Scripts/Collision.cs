using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public GameObject textObject;
    // Start is called before the first frame update
    void Start()
    {
        if (textObject != null)
            textObject.SetActive(false);
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera")) // or use "Player" if that's your tag
        {
            if (textObject != null)
                textObject.SetActive(true);
        }
    }

    // Trigger exit event
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (textObject != null)
                textObject.SetActive(false);
        }
    }
}
