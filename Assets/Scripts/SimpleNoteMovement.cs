using UnityEngine;

public class SimpleNoteMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private float travelTime;
    private float startTime;
    private Vector3 startPosition;
    private bool isLeftNote;
    private bool hasArrived = false;
    private bool isHit = false;

    [Header("Visual Effects")]
    public float rotationSpeed = 180f;
    public float bobSpeed = 3f;
    public float bobHeight = 0.1f;

    void Awake()
    {
    }

    public void Setup(Vector3 target, float timeToTarget, bool leftNote)
    {
        targetPosition = target;
        travelTime = Mathf.Max(0.0001f, timeToTarget);
        isLeftNote = leftNote;
        startTime = Time.time;
        startPosition = transform.position;

        // Auto-destroy after travel time + buffer (failsafe)
        Destroy(gameObject, travelTime + 1f);
    }

    // Public accessor so other systems can check note side (left/right)
    public bool IsLeftNote => isLeftNote;

    void Update()
    {
        // If note was hit, let the hit coroutine handle disappearance / animation
        if (isHit) return;

        if (hasArrived) return;

        float t = (Time.time - startTime) / travelTime;

        if (t >= 1f)
        {
            ArriveAtTarget();
            return;
        }

        // Move toward target
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);

        // Bobbing
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position += Vector3.up * bob;

        // Scale down as approaching
        float scale = Mathf.Lerp(1f, 0.8f, t);
        transform.localScale = Vector3.one * scale;
    }

    void ArriveAtTarget()
    {
        if (hasArrived) return;

        hasArrived = true;

        // Flash color
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }

        // Start fading out and destroy at the target end
        StartCoroutine(FadeOutAndDestroy(0.5f));
    }

    System.Collections.IEnumerator FadeOutAndDestroy(float fadeTime)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Color startColor = renderer != null ? renderer.material.color : Color.white;
        float elapsed = 0f;

        Vector3 startScale = transform.localScale;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeTime);

            // Fade out
            if (renderer != null)
            {
                Color color = startColor;
                color.a = 1f - t;
                renderer.material.color = color;
            }

            // Shrink
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

        Destroy(gameObject);
    }

    // Called when hit by drum stick
    public void OnHit()
    {
        // Ignore hits after arriving (already at target and fading) or if already hit
        if (hasArrived || isHit) return;

        isHit = true;

        Debug.Log($"Hit {(isLeftNote ? "LEFT" : "RIGHT")} note!");

        // Stop any running coroutines so hit behavior takes over
        StopAllCoroutines();

        // Quick hit color change and disappear animation
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.green;
        }

        // Spawn floating UI text showing which stick hit (optional)
        FloatingTextSpawner.Instance?.SpawnHitText(isLeftNote, transform.position);

        // Register the hit with the ScoreControls (position-based scoring)
        if (ScoreControls.Instance != null)
            ScoreControls.Instance.RegisterHit(gameObject);
        else if (ScoreManager.Instance != null)
            ScoreManager.Instance.RegisterHit(ScoreManager.HitType.Miss); // fallback: register miss or choose default

        StartCoroutine(HitAndDestroy(0.12f));
    }

    System.Collections.IEnumerator HitAndDestroy(float duration)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Color startColor = renderer != null ? renderer.material.color : Color.white;
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        // Optional: jump a tiny bit toward the target end for visual feedback
        Vector3 hitTarget = targetPosition;
        Vector3 hitStart = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Move quickly a short distance toward the target (gives sense of hitting toward end)
            transform.position = Vector3.Lerp(hitStart, hitTarget, t * 0.5f);

            // Fade out and shrink
            if (renderer != null)
            {
                Color c = startColor;
                c.a = 1f - t;
                renderer.material.color = c;
            }

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && !hasArrived)
        {
            // Draw line to target
            Gizmos.color = isLeftNote ? Color.blue : Color.red;
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}