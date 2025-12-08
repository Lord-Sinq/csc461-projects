using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumNoteSpawner : MonoBehaviour
{
    [Header("Note Prefabs")]
    public GameObject leftNotePrefab;   // For left drum stick
    public GameObject rightNotePrefab;  // For right drum stick
    
    [Header("Spawn Point")]
    public Transform spawnPoint;        // Single spawn location
    
    [Header("Target Positions")]
    public Transform leftNoteTarget;    // Where left notes should go
    public Transform rightNoteTarget;   // Where right notes should go
    
    [Header("Movement Settings")]
    public float travelTime = 2f;
    public float spawnInterval = 1.5f;
    
    [Header("Visual Settings")]
    public Color leftNoteColor = Color.blue;
    public Color rightNoteColor = Color.red;
    public float noteSize = 0.3f;
    
    [Header("Game Control")]
    public bool autoSpawn = true;
    public int maxNotes = 10;
<<<<<<< Updated upstream
    
    private List<GameObject> activeNotes = new List<GameObject>();
    private Coroutine spawnRoutine;
    
=======

    [Header("Auto right-note (spawn without pressing start)")]
    [Tooltip("If true, the spawner will continuously spawn right notes immediately at runtime.")]
    public bool autoSpawnRight = false;
    [Tooltip("Delay before the automatic right-note spawning begins.")]
    public float autoRightStartDelay = 0f;

    [Header("Scheduling")]
    [Tooltip("If true, uses spawnSchedule to spawn notes at specific times (relative to StartSpawning).")]
    public bool useSchedule = false;
    [Tooltip("List of timed spawn events (seconds relative to StartSpawning).")]
    public List<SpawnEvent> spawnSchedule = new List<SpawnEvent>();
    [Tooltip("If true, schedule will loop when finished.")]
    public bool loopSchedule = false;

    [Header("Debug / Editor Controls")]
    public bool enableEditorControls = false;

    private List<GameObject> activeNotes = new List<GameObject>();
    private Coroutine spawnRoutine;
    private Coroutine rightSpawnRoutine;
    private float spawnStartTime; // Time.time when StartSpawning was called

>>>>>>> Stashed changes
    void Start()
    {
        // Set defaults if not assigned
        if (spawnPoint == null)
        {
            spawnPoint = new GameObject("SpawnPoint").transform;
            spawnPoint.SetParent(transform);
            spawnPoint.localPosition = new Vector3(0, 2, 3);
        }
        
        if (leftNoteTarget == null)
        {
            leftNoteTarget = new GameObject("LeftTarget").transform;
            leftNoteTarget.SetParent(transform);
            leftNoteTarget.localPosition = new Vector3(-0.5f, 1, 1);
        }
        
        if (rightNoteTarget == null)
        {
            rightNoteTarget = new GameObject("RightTarget").transform;
            rightNoteTarget.SetParent(transform);
            rightNoteTarget.localPosition = new Vector3(0.5f, 1, 1);
        }
        
        Debug.Log("Two-note spawner ready");
        Debug.Log($"Left notes: {leftNoteColor} → {leftNoteTarget.position}");
        Debug.Log($"Right notes: {rightNoteColor} → {rightNoteTarget.position}");
<<<<<<< Updated upstream
        
        if (autoSpawn)
=======
        Debug.Log($"DrumNoteSpawner: autoSpawn={autoSpawn} autoSpawnRight={autoSpawnRight} GameState.GameStarted={GameState.GameStarted}");

        // Start right-note auto spawner if requested (independent of full game start)
        if (autoSpawnRight)
        {
            rightSpawnRoutine = StartCoroutine(RightSpawnRoutine());
            Debug.Log("DrumNoteSpawner: RightSpawnRoutine started because autoSpawnRight=true");
        }

        // Only auto-start schedule/free-run when autoSpawn true AND GameState.GameStarted
        if (autoSpawn && GameState.GameStarted)
>>>>>>> Stashed changes
        {
            StartSpawning();
        }
    }
    
    void Update()
    {
        // Clean up destroyed notes
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            if (activeNotes[i] == null)
            {
                activeNotes.RemoveAt(i);
            }
        }
        
        // Test keys
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q)) SpawnLeftNote();
        if (Input.GetKeyDown(KeyCode.E)) SpawnRightNote();
        if (Input.GetKeyDown(KeyCode.Space)) ToggleSpawning();
        #endif
    }
    
    public void SpawnLeftNote()
    {
        SpawnNote(true);
    }
    
    public void SpawnRightNote()
    {
        SpawnNote(false);
    }
    
    void SpawnNote(bool isLeftNote)
    {
        // Limit active notes
        if (activeNotes.Count >= maxNotes)
        {
            Destroy(activeNotes[0]);
            activeNotes.RemoveAt(0);
        }
        
        // Choose prefab and target
        GameObject prefab = isLeftNote ? leftNotePrefab : rightNotePrefab;
        Transform target = isLeftNote ? leftNoteTarget : rightNoteTarget;
        
        if (prefab == null || spawnPoint == null || target == null)
        {
            Debug.LogError("Missing reference for note spawning!");
            return;
        }
        
        // Create note
        GameObject note = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        note.name = isLeftNote ? "LeftNote" : "RightNote";
        note.transform.localScale = Vector3.one * noteSize;
        
        // Set color
        MeshRenderer renderer = note.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = isLeftNote ? leftNoteColor : rightNoteColor;
        }
        
        // Add movement script
        SimpleNoteMovement movement = note.GetComponent<SimpleNoteMovement>();
        if (movement == null)
        {
            movement = note.AddComponent<SimpleNoteMovement>();
        }
        
        // Setup movement
        movement.Setup(target.position, travelTime, isLeftNote);
        
        // Track note
        activeNotes.Add(note);
        
        Debug.Log($"Spawned {(isLeftNote ? "LEFT" : "RIGHT")} note to {target.position}");
    }
    
    public void StartSpawning()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnNotesRoutine());
        Debug.Log("Started spawning notes");
    }
    
    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
        Debug.Log("Stopped spawning notes");
    }
    
    void ToggleSpawning()
    {
        if (spawnRoutine == null) StartSpawning();
        else StopSpawning();
    }
    
    IEnumerator SpawnNotesRoutine()
    {
        bool nextIsLeft = true;
        
        while (true)
        {
            SpawnNote(nextIsLeft);
            nextIsLeft = !nextIsLeft; // Alternate
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
<<<<<<< Updated upstream
    
=======

    IEnumerator SpawnScheduledRoutine()
    {
        if (spawnSchedule == null || spawnSchedule.Count == 0)
            yield break;

        int index = 0;

        while (true)
        {
            float now = Time.time - spawnStartTime;

            // if current event time has arrived => spawn
            if (index < spawnSchedule.Count && now >= spawnSchedule[index].time)
            {
                SpawnNote(spawnSchedule[index].isLeft);
                index++;
                continue;
            }

            // finished schedule
            if (index >= spawnSchedule.Count)
            {
                if (loopSchedule)
                {
                    // reset and loop
                    spawnStartTime = Time.time;
                    index = 0;
                    yield return null;
                    continue;
                }
                else
                {
                    spawnRoutine = null;
                    yield break;
                }
            }

            // wait a small amount until the next scheduled time
            float nextTime = spawnSchedule[index].time;
            float wait = Mathf.Max(0.001f, nextTime - now);
            yield return new WaitForSeconds(wait);
        }
    }

    IEnumerator RightSpawnRoutine()
    {
        if (autoRightStartDelay > 0f)
            yield return new WaitForSeconds(autoRightStartDelay);

        while (true)
        {
            SpawnRightNote();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

>>>>>>> Stashed changes
    public void ClearAllNotes()
    {
        foreach (GameObject note in activeNotes)
        {
            if (note != null) Destroy(note);
        }
        activeNotes.Clear();
    }
    
    void OnDrawGizmos()
    {
        // Draw spawn point
        if (spawnPoint != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(spawnPoint.position, 0.2f);
            Gizmos.DrawWireSphere(spawnPoint.position, 0.3f);
        }
        
        // Draw left target and path
        if (leftNoteTarget != null && spawnPoint != null)
        {
            Gizmos.color = leftNoteColor;
            Gizmos.DrawSphere(leftNoteTarget.position, 0.25f);
            Gizmos.DrawWireSphere(leftNoteTarget.position, 0.3f);
            Gizmos.DrawLine(spawnPoint.position, leftNoteTarget.position);
        }
        
        // Draw right target and path
        if (rightNoteTarget != null && spawnPoint != null)
        {
            Gizmos.color = rightNoteColor;
            Gizmos.DrawSphere(rightNoteTarget.position, 0.25f);
            Gizmos.DrawWireSphere(rightNoteTarget.position, 0.3f);
            Gizmos.DrawLine(spawnPoint.position, rightNoteTarget.position);
        }
    }
    
    // Editor buttons
    [ContextMenu("Spawn Left Note")]
    void EditorSpawnLeft() => SpawnLeftNote();
    
    [ContextMenu("Spawn Right Note")]
    void EditorSpawnRight() => SpawnRightNote();
    
    [ContextMenu("Start/Stop Spawning")]
    void EditorToggleSpawning() => ToggleSpawning();
    
    [ContextMenu("Clear All Notes")]
    void EditorClearNotes() => ClearAllNotes();
}