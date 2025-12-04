using UnityEngine;

public class NoteController : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float journeyTime;
    private float startTime;
    
    public bool isLeftHandNote;  // True = use left hand, False = use right hand
    private Renderer noteRenderer;
    
    void Start()
    {
        noteRenderer = GetComponent<Renderer>();
        
        // Color the note based on which hand to use
        if (noteRenderer != null)
        {
            noteRenderer.material.color = isLeftHandNote ? Color.blue : Color.red;
        }
        
        // Add trigger collider for hit detection
        if (GetComponent<Collider>() == null)
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.25f;
        }
    }
    
    public void Setup(Vector3 targetPos, float travelDuration)
    {
        startPosition = transform.position;
        targetPosition = targetPos;
        journeyTime = travelDuration;
        startTime = Time.time;
    }
    
    void Update()
    {
        float fractionOfJourney = (Time.time - startTime) / journeyTime;
        transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
        
        // Destroy if it goes past the drum
        if (fractionOfJourney >= 1.2f)
        {
            Destroy(gameObject);
            Debug.Log("Note missed!");
        }
    }
    
    public void HitSuccess(Color feedbackColor)
    {
        // Quick visual feedback before destroying
        if (noteRenderer != null)
        {
            noteRenderer.material.color = feedbackColor;
        }
        
        // Wait a frame so player can see the feedback
        Destroy(gameObject, 0.1f);
    }
}