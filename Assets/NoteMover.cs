using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Vector2 moveDirection = Vector2.left; // Moves left by default
    [SerializeField] private bool moveOnStart = false; // Should it move immediately?
    
    [Header("Input Control")]
    [SerializeField] private KeyCode startKey = KeyCode.Space;
    
    private bool isMoving = false;
    private Rigidbody2D rb;
    
    void Start()
    {
        // Normalize direction to ensure consistent speed
        moveDirection = moveDirection.normalized;
        
        // Try to get Rigidbody2D (optional, for physics)
        rb = GetComponent<Rigidbody2D>();
        
        if (moveOnStart)
        {
            StartMoving();
        }
    }
    
    void Update()
    {
        // Start moving when key is pressed
        if (Input.GetKeyDown(startKey) && !isMoving)
        {
            StartMoving();
        }
        
        // Simple movement without physics
        if (isMoving && rb == null)
        {
            MoveWithoutPhysics();
        }
    }
    
    void MoveWithoutPhysics()
    {
        // Calculate movement
        Vector3 movement = new Vector3(moveDirection.x, moveDirection.y, 0) 
                          * moveSpeed * Time.deltaTime;
        
        // Apply movement
        transform.position += movement;
    }
    
    void FixedUpdate()
    {
        // Physics-based movement (if using Rigidbody2D)
        if (isMoving && rb != null)
        {
            MoveWithPhysics();
        }
    }
    
    void MoveWithPhysics()
    {
        rb.velocity = moveDirection * moveSpeed;
    }
    
    public void StartMoving()
    {
        isMoving = true;
        Debug.Log("Note started moving!");
    }
    
    public void StopMoving()
    {
        isMoving = false;
        if (rb != null) rb.velocity = Vector2.zero;
    }
    
    // Draw movement direction in Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 direction = new Vector3(moveDirection.x, moveDirection.y, 0);
        Gizmos.DrawRay(transform.position, direction * 2f);
        Gizmos.DrawSphere(transform.position + direction * 2f, 0.2f);
    }
}