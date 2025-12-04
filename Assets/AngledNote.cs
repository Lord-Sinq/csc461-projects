using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngledNote : MonoBehaviour
{
    [Header("Movement Angle")]
    [SerializeField] private float angle = 0f; // 0 = right, 90 = up, -90 = down
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool useWorldSpace = true;
    
    [Header("Control")]
    [SerializeField] private KeyCode startKey = KeyCode.Space;
    [SerializeField] private bool destroyOnClick = false;
    
    private bool isMoving = false;
    private Vector2 direction;
    
    void Start()
    {
        CalculateDirection();
        
        // Optional: Start moving when this note is clicked
        if (destroyOnClick)
        {
            // Add collider if not present
            if (GetComponent<Collider2D>() == null)
            {
                gameObject.AddComponent<BoxCollider2D>();
            }
        }
    }
    
    void Update()
    {
        // Start movement with key
        if (Input.GetKeyDown(startKey) && !isMoving)
        {
            isMoving = true;
        }
        
        // Move if active
        if (isMoving)
        {
            MoveNote();
        }
        
        // Optional: Click to destroy
        if (destroyOnClick && Input.GetMouseButtonDown(0))
        {
            CheckClick();
        }
    }
    
    void CalculateDirection()
    {
        // Convert angle to direction vector
        float angleRad = angle * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
    }
    
    void MoveNote()
    {
        Vector3 movement;
        
        if (useWorldSpace)
        {
            // Move in world space
            movement = new Vector3(direction.x, direction.y, 0) * speed * Time.deltaTime;
            transform.position += movement;
        }
        else
        {
            // Move relative to object's rotation
            movement = transform.right * speed * Time.deltaTime;
            transform.position += movement;
        }
    }
    
    void CheckClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D collider = GetComponent<Collider2D>();
        
        if (collider != null && collider.OverlapPoint(mousePos))
        {
            Destroy(gameObject);
            Debug.Log("Note clicked and destroyed!");
        }
    }
    
    public void SetAngle(float newAngle)
    {
        angle = newAngle;
        CalculateDirection();
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    
    void OnDrawGizmos()
    {
        CalculateDirection();
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, direction * 1.5f);
        
        // Draw angle arc
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        DrawAngleArc(transform.position, 1f, angle - 45, angle + 45);
    }
    
    void DrawAngleArc(Vector3 center, float radius, float fromAngle, float toAngle)
    {
        Vector3 fromDir = Quaternion.Euler(0, 0, fromAngle) * Vector3.right;
        Vector3 toDir = Quaternion.Euler(0, 0, toAngle) * Vector3.right;
        
        UnityEditor.Handles.color = new Color(0, 1, 0, 0.3f);
        UnityEditor.Handles.DrawSolidArc(center, Vector3.forward, fromDir, toAngle - fromAngle, radius);
    }
}