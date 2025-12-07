using UnityEngine;

public class SimpleNoteMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private float travelTime;
    private float startTime;
    private Vector3 startPosition;
    private bool isLeftNote;
    private bool hasArrived = false;
    
    [Header("Visual Effects")]
    public float rotationSpeed = 180f;
    public float bobSpeed = 3f;
    public float bobHeight = 0.1f;
    
    public void Setup(Vector3 target, float timeToTarget, bool leftNote)
    {
        targetPosition = target;
        travelTime = timeToTarget;
        isLeftNote = leftNote;
        startTime = Time.time;
        startPosition = transform.position;
        
        // Auto-destroy after travel time + buffer
        Destroy(gameObject, travelTime + 1f);
    }
    
    void Update()
    {
        if (hasArrived) return;
        
        float t = (Time.time - startTime) / travelTime;
        
        if (t >= 1f)
        {
            ArriveAtTarget();
            return;
        }
        
        // Move toward target
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);
        
        // Add slight arc (optional)
        float arcHeight = 0.5f;
        float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
        transform.position += Vector3.up * arc;
        
        // Visual effects
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Bobbing
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position += Vector3.up * bob;
        
        // Scale down as approaching
        float scale = Mathf.Lerp(1f, 0.8f, t);
        transform.localScale = Vector3.one * scale;
    }
    
    void ArriveAtTarget()
    {
        hasArrived = true;
        
        // Flash color
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }
        
        // Start fading out
        StartCoroutine(FadeOutAndDestroy(0.5f));
    }
    
    System.Collections.IEnumerator FadeOutAndDestroy(float fadeTime)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Color startColor = renderer.material.color;
        float elapsed = 0f;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            
            // Fade out
            Color color = startColor;
            color.a = 1f - t;
            renderer.material.color = color;
            
            // Shrink
            transform.localScale = Vector3.one * (1f - t * 0.5f);
            
            yield return null;
        }
        
        Destroy(gameObject);
    }
    
    // Called when hit by drum stick
    public void OnHit()
    {
        if (hasArrived) return;
        
        Debug.Log($"Hit {(isLeftNote ? "LEFT" : "RIGHT")} note!");
        
        // Change to hit color
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.green;
        }
        
        // Quick destroy
        Destroy(gameObject, 0.1f);
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