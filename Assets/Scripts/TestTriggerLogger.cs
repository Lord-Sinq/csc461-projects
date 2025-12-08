using UnityEngine;

// Attach this to your button GameObject to diagnose why triggers aren't firing.
[RequireComponent(typeof(Collider))]
public class TestTriggerLogger : MonoBehaviour
{
    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void Start()
    {
        Debug.Log($"TestTriggerLogger started on '{gameObject.name}' collider.isTrigger={GetComponent<Collider>().isTrigger}");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"TestTriggerLogger: ENTER by '{other.name}' tag='{other.tag}' layer={LayerMask.LayerToName(other.gameObject.layer)} hasRigidbody={other.attachedRigidbody != null} kinematic={(other.attachedRigidbody!=null?other.attachedRigidbody.isKinematic:false)}");
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"TestTriggerLogger: EXIT by '{other.name}'");
    }
}