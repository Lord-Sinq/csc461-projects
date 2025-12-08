using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to the ScoreBox GameObject (the collider that defines scoring area).
/// Requires the Collider to be set as "Is Trigger".
/// Logs when notes enter/exit and reports their side and time inside the box.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ScoreBoxMonitor : MonoBehaviour
{
    [Tooltip("Optional tag to filter note colliders (leave blank to accept any).")]
    public string noteTag = "Note";

    [Tooltip("Optional layer mask to filter note colliders.")]
    public LayerMask noteLayer = ~0;

    [Header("Debug")]
    public bool debugLogs = true;


    // Track entry times so we can report duration on exit
    private readonly Dictionary<GameObject, float> _entryTimes = new Dictionary<GameObject, float>();

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Layer filter
        if (((1 << other.gameObject.layer) & noteLayer) == 0)
            return;

        // Tag filter
        if (!string.IsNullOrEmpty(noteTag) && !other.CompareTag(noteTag))
            return;

        // Find note component
        var note = other.GetComponentInParent<SimpleNoteMovement>() ?? other.GetComponent<SimpleNoteMovement>();
        if (note == null) return;

        GameObject noteRoot = note.gameObject;
        float entryTime = Time.time;
        _entryTimes[noteRoot] = entryTime;

        string side = note.IsLeftNote ? "Left" : "Right";
        Debug.Log($"ScoreBoxMonitor: NOTE ENTER - '{noteRoot.name}' side={side} time={entryTime:F3}s");

        if (debugLogs)
        {
            // Additional debug info: note world pos and bounding box center
            Debug.Log($"ScoreBoxMonitor(Debug): note pos={noteRoot.transform.position} boxCenter={GetComponent<Collider>().bounds.center}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Layer filter
        if (((1 << other.gameObject.layer) & noteLayer) == 0)
            return;

        if (!string.IsNullOrEmpty(noteTag) && !other.CompareTag(noteTag))
            return;

        var note = other.GetComponentInParent<SimpleNoteMovement>() ?? other.GetComponent<SimpleNoteMovement>();
        if (note == null) return;

        GameObject noteRoot = note.gameObject;
        float exitTime = Time.time;
        if (_entryTimes.TryGetValue(noteRoot, out float enterTime))
        {
            float duration = exitTime - enterTime;
            string side = note.IsLeftNote ? "Left" : "Right";
            Debug.Log($"ScoreBoxMonitor: NOTE EXIT  - '{noteRoot.name}' side={side} exitTime={exitTime:F3}s duration={duration:F3}s");
            _entryTimes.Remove(noteRoot);
        }
        else
        {
            // If we didn't see an enter (possible if entered before monitor active), still log exit with time.
            string side = note.IsLeftNote ? "Left" : "Right";
            Debug.Log($"ScoreBoxMonitor: NOTE EXIT (no recorded entry) - '{noteRoot.name}' side={side} time={exitTime:F3}s");
        }
    }

    void OnDisable()
    {
        _entryTimes.Clear();
    }
}