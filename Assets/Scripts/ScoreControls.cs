using UnityEngine;
using TMPro;

/// <summary>
/// Central scoring controller. Attach once in the scene and assign the transparent scoring box (a GameObject with a Collider)
/// and optional floating text prefab (TextMeshPro) for feedback.
/// Call ScoreControls.Instance.RegisterHit(note) when the player hits a note.
/// Note: RegisterHit no longer destroys the note; the note's own hit routine should handle visuals and destruction.
/// </summary>
public class ScoreControls : MonoBehaviour
{
    public static ScoreControls Instance { get; private set; }

    [Header("Scoring Area")]
    [Tooltip("Collider (usually the transparent box) that defines the scoring region.")]
    public Collider scoringBoxCollider;

    public enum Axis { X, Y, Z }
    [Tooltip("Local axis of the scoring box that corresponds to the note travel direction.")]
    public Axis scoringAxis = Axis.Z;

    [Header("Thresholds (normalized 0..1 from center)")]
    [Range(0f, 1f)] public float perfectThreshold = 0.15f;
    [Range(0f, 1f)] public float goodThreshold = 0.35f;

    [Header("Feedback")]
    [Tooltip("Optional floating text prefab (TextMeshPro) spawned at note position. Prefab should contain a TMP_Text component.")]
    public GameObject floatingTextPrefab;
    public float floatingTextLifetime = 1.0f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Call this when a note is hit. The method will evaluate note position inside the scoringBoxCollider and assign a HitType,
    /// register it with ScoreManager and spawn optional floating text feedback.
    /// The note's own script should handle the disappearance (so we avoid double-destroy).
    /// </summary>
    public void RegisterHit(GameObject note)
    {
        if (note == null) return;

        ScoreManager.HitType hitType = ComputeHitType(note.transform.position);

        // Register score
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.RegisterHit(hitType);

        // Spawn floating text feedback
        if (floatingTextPrefab != null)
        {
            GameObject f = Instantiate(floatingTextPrefab, note.transform.position, Quaternion.identity);
            TMP_Text txt = f.GetComponentInChildren<TMP_Text>();
            if (txt != null)
            {
                txt.text = hitType == ScoreManager.HitType.Perfect ? "Perfect" :
                           hitType == ScoreManager.HitType.Good ? "Good" : "Miss";
            }
            Destroy(f, floatingTextLifetime);
        }

        // NOTE: Do NOT destroy the note here. Let the note play its hit animation and destroy itself.
    }

    /// <summary>
    /// Compute the HitType by evaluating the note position relative to the scoringBoxCollider center along the chosen local axis.
    /// If collider is missing or outside, returns Miss.
    /// </summary>
    public ScoreManager.HitType ComputeHitType(Vector3 worldPosition)
    {
        if (scoringBoxCollider == null) return ScoreManager.HitType.Miss;

        // Use bounds of the collider to compute normalized distance along the chosen axis (0 = center, 1 = edge)
        Bounds b = scoringBoxCollider.bounds;
        Vector3 localPos = worldPosition - b.center;

        float halfSize = scoringAxis switch
        {
            Axis.X => b.extents.x,
            Axis.Y => b.extents.y,
            _ => b.extents.z,
        };

        if (halfSize <= 0f) return ScoreManager.HitType.Miss;

        float axisValue = scoringAxis switch
        {
            Axis.X => localPos.x,
            Axis.Y => localPos.y,
            _ => localPos.z,
        };

        float normalized = Mathf.Clamp01(Mathf.Abs(axisValue) / halfSize);

        if (normalized <= perfectThreshold) return ScoreManager.HitType.Perfect;
        if (normalized <= goodThreshold) return ScoreManager.HitType.Good;
        return ScoreManager.HitType.Miss;
    }
}
