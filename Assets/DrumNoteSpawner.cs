using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumNoteSpawner : MonoBehaviour
{
    public GameObject leftHandNotePrefab;    // Note for left hand (blue maybe)
    public GameObject rightHandNotePrefab;   // Note for right hand (red maybe)
    public Transform spawnPoint;             // Where notes appear
    public Transform targetZone;             // The drum position
    public float travelTime = 2f;
    
    void Start()
    {
        // Spawn alternating left/right notes
        InvokeRepeating("SpawnRandomNote", 0f, 1.5f);  // Every 1.5 seconds
    }
    
    void SpawnRandomNote()
    {
        // Randomly choose left or right hand note
        bool isLeftNote = Random.Range(0, 2) == 0;
        GameObject notePrefab = isLeftNote ? leftHandNotePrefab : rightHandNotePrefab;
        
        SpawnNote(notePrefab, isLeftNote);
    }
    
    // Or spawn in a pattern (left, right, left, right...)
    bool nextIsLeft = true;
    void SpawnPatternNote()
    {
        GameObject notePrefab = nextIsLeft ? leftHandNotePrefab : rightHandNotePrefab;
        SpawnNote(notePrefab, nextIsLeft);
        nextIsLeft = !nextIsLeft;  // Alternate for next time
    }
    
    void SpawnNote(GameObject notePrefab, bool isLeftHand)
    {
        if (notePrefab == null || spawnPoint == null || targetZone == null)
            return;
        
        GameObject newNote = Instantiate(notePrefab, spawnPoint.position, spawnPoint.rotation);
        
        NoteController noteController = newNote.GetComponent<NoteController>();
        if (noteController == null)
            noteController = newNote.AddComponent<NoteController>();
        
        noteController.Setup(targetZone.position, travelTime);
        noteController.isLeftHandNote = isLeftHand;  // Tell the note which hand to use
    }
}