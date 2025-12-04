using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private int maxNotes = 10;
    
    [Header("Note Properties")]
    [SerializeField] private float noteSpeed = 5f;
    [SerializeField] private float[] availableAngles = { -45f, 0f, 45f }; // Multiple angles
    [SerializeField] private Color[] noteColors = { Color.red, Color.blue, Color.green };
    
    [Header("Control")]
    [SerializeField] private KeyCode spawnKey = KeyCode.N;
    [SerializeField] private bool autoSpawn = true;
    
    private List<GameObject> activeNotes = new List<GameObject>();
    private float spawnTimer = 0f;
    
    void Start()
    {
        if (notePrefab == null)
        {
            Debug.LogError("Note prefab not assigned!");
            enabled = false;
        }
    }
    
    void Update()
    {
        // Manual spawn with key
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnNote();
        }
        
        // Auto-spawn
        if (autoSpawn)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnNote();
                spawnTimer = 0f;
            }
        }
        
        // Clean up destroyed notes
        activeNotes.RemoveAll(note => note == null);
    }
    
    void SpawnNote()
    {
        if (activeNotes.Count >= maxNotes)
        {
            // Remove oldest note
            if (activeNotes.Count > 0)
            {
                Destroy(activeNotes[0]);
                activeNotes.RemoveAt(0);
            }
        }
        
        // Create new note
        GameObject note = Instantiate(notePrefab, transform.position, Quaternion.identity);
        activeNotes.Add(note);
        
        // Set random properties
        SetupNote(note);
        
        Debug.Log($"Spawned note {activeNotes.Count}/{maxNotes}");
    }
    
    void SetupNote(GameObject note)
    {
        // Set random angle
        AngledNote angledNote = note.GetComponent<AngledNote>();
        if (angledNote != null && availableAngles.Length > 0)
        {
            float randomAngle = availableAngles[Random.Range(0, availableAngles.Length)];
            angledNote.SetAngle(randomAngle);
            angledNote.SetSpeed(noteSpeed);
        }
        
        // Set random color
        SpriteRenderer sr = note.GetComponent<SpriteRenderer>();
        if (sr != null && noteColors.Length > 0)
        {
            Color randomColor = noteColors[Random.Range(0, noteColors.Length)];
            sr.color = randomColor;
        }
        
        // Scale variation
        float randomScale = Random.Range(0.8f, 1.2f);
        note.transform.localScale = Vector3.one * randomScale;
    }
    
    public void ClearAllNotes()
    {
        foreach (GameObject note in activeNotes)
        {
            if (note != null) Destroy(note);
        }
        activeNotes.Clear();
        Debug.Log("Cleared all notes");
    }
    
    void OnDrawGizmos()
    {
        // Draw spawn point
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawIcon(transform.position, "SpawnIcon.png", true);
        
        // Draw spawn directions
        Gizmos.color = new Color(1, 1, 0, 0.5f);
        foreach (float angle in availableAngles)
        {
            float angleRad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            Gizmos.DrawRay(transform.position, dir * 2f);
        }
    }
}