using UnityEngine;

/// <summary>
/// Attach this to your drum top collision zone (make sure it has "Is Trigger" checked).
/// When a collider with a DrumStick enters, this checks `noteCheckZone` for notes
/// and calls their hit method (expects `SimpleNoteMovement` on the note root).
/// Added debug logging and OnTriggerExit handling. Now respects note side (left/right).
/// </summary>
[RequireComponent(typeof(Collider))]
public class DrumTopHitZone : MonoBehaviour
{
    [Tooltip("Collider that defines the area where notes are checked (scoring box).")]
    public Collider noteCheckZone;

    [Tooltip("Tag used on the drum stick colliders.")]
    public string drumStickTag = "DrumStick";

    [Tooltip("Optional layer mask to limit what counts as a note.")]
    public LayerMask noteLayer = ~0;

    [Tooltip("Optional tag for notes (set your note prefabs to this tag).")]
    public string noteTag = "Note";

    [Tooltip("Minimum seconds between sequential hits to avoid double hits.")]
    public float hitCooldown = 0.08f;

    [Header("Debug")]
    public bool debugLogs = true;

    [Header("Audio")]
    public AudioClip drumSound;
    private AudioSource audioSource;

    [Header("Floating Text")] // <-- NEW HEADER
    [Tooltip("The FloatingText prefab to instantiate on hit.")]
    public FloatingText floatingTextPrefab;
    [Tooltip("The position where the floating text should appear.")]
    public Transform textSpawnPoint;

    float lastHitTime = -10f;

    void Start()
    {
        // Ensure there is an AudioSource component on this GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.playOnAwake = false; // Prevents the sound from playing when the scene loads
    }

    void Reset()
    {
        // ensure the collider on this GameObject is a trigger
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Look for DrumStick component on the entering collider or its parent
        var stick = other.GetComponentInParent<DrumStick>() ?? other.GetComponent<DrumStick>();
        if (stick == null)
        {
            if (debugLogs) Debug.Log($"DrumTopHitZone: non-stick object entered: {other.name}");
            return;
        }

        if (Time.time - lastHitTime < hitCooldown)
        {
            if (debugLogs) Debug.Log($"DrumTopHitZone: hit ignored due to cooldown ({Time.time - lastHitTime:F3}s)");
            return;
        }
        if (audioSource != null && drumSound != null)
        {
            audioSource.PlayOneShot(drumSound);
        }

        lastHitTime = Time.time;

        if (debugLogs) Debug.Log($"DrumTopHitZone: {stick.side} stick ENTERED by '{other.name}' at {Time.time:F3}");

        CheckForNoteAndHit(stick.side);
    }

    void OnTriggerExit(Collider other)
    {
        var stick = other.GetComponentInParent<DrumStick>() ?? other.GetComponent<DrumStick>();
        if (stick == null)
        {
            if (debugLogs) Debug.Log($"DrumTopHitZone: non-stick object exited: {other.name}");
            return;
        }

        if (debugLogs) Debug.Log($"DrumTopHitZone: {stick.side} stick EXITED by '{other.name}' at {Time.time:F3}");
        // You can add behavior here if you want to cancel pending hit state or visual feedback.
    }

    void CheckForNoteAndHit(DrumStick.Side stickSide)
    {
        if (noteCheckZone == null)
        {
            Debug.LogWarning("DrumTopHitZone: noteCheckZone not assigned.");
            Debug.Log("Miss");
            return;
        }

        // use bounds of the noteCheckZone to search for colliders (works for BoxCollider or any collider)
        Bounds b = noteCheckZone.bounds;
        Collider[] hits = Physics.OverlapBox(b.center, b.extents, noteCheckZone.transform.rotation, noteLayer);

        if (hits == null || hits.Length == 0)
        {
            Debug.Log("Miss");
            if (debugLogs) Debug.Log("DrumTopHitZone: no colliders found inside noteCheckZone.");
            return;
        }

        if (debugLogs) Debug.Log($"DrumTopHitZone: found {hits.Length} colliders in noteCheckZone. Searching for notes...");

        foreach (var c in hits)
        {
            // optional tag filter
            if (!string.IsNullOrEmpty(noteTag) && !c.CompareTag(noteTag))
                continue;

            // find a SimpleNoteMovement on the collider or its parents
            var note = c.GetComponentInParent<SimpleNoteMovement>();
            if (note == null) note = c.GetComponent<SimpleNoteMovement>();

            if (note != null)
            {
                // Only hit notes that match the stick side.
                // Left stick should hit notes where IsLeftNote == true; right stick should hit IsLeftNote == false.
                bool noteIsLeft = note.IsLeftNote;
                if ((stickSide == DrumStick.Side.Left && !noteIsLeft) ||
                    (stickSide == DrumStick.Side.Right && noteIsLeft))
                {
                    if (debugLogs) Debug.Log($"DrumTopHitZone: skipping note '{note.gameObject.name}' � side mismatch (noteIsLeft={noteIsLeft})");
                    continue;
                }

                // matched a note for this stick side
                note.OnHit();

                // send required console message
                if (stickSide == DrumStick.Side.Left)
                    Debug.Log("Left note hit");
                else
                    Debug.Log("Right note hit");

                if (debugLogs) Debug.Log($"DrumTopHitZone: hitting note '{note.gameObject.name}' for {stickSide} stick.");
                return; // stop after first matched note
            }
        }

        // no matching note found for this stick side
        Debug.Log("Miss");
        if (debugLogs) Debug.Log("DrumTopHitZone: no note matched the search criteria (tag/layer/side).");
    }

    void OnDrawGizmosSelected()
    {
        if (noteCheckZone == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(noteCheckZone.bounds.center, noteCheckZone.bounds.size);
    }
}