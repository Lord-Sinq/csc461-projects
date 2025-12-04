using UnityEngine;
using System.Collections.Generic;

public class DrumDetectionZone : MonoBehaviour
{
    public float perfectHitRadius = 0.3f;
    public float goodHitRadius = 0.6f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StickR"))
        {
            Debug.Log("Right stick hit drum!");
            CheckForNotes(false);  // false = right hand hit
        }
        if (other.CompareTag("StickL"))
        {
            Debug.Log("Left stick hit drum!");
            CheckForNotes(true);   // true = left hand hit
        }
    }
    
    void CheckForNotes(bool wasLeftStick)
    {
        NoteController[] allNotes = FindObjectsOfType<NoteController>();
        
        if (allNotes.Length == 0)
        {
            Debug.Log("Miss! No notes to hit");
            return;
        }
        
        // Find the closest note to the drum
        NoteController closestNote = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (NoteController note in allNotes)
        {
            float distance = Vector3.Distance(note.transform.position, transform.position);
            if (distance < closestDistance && distance <= goodHitRadius)
            {
                closestDistance = distance;
                closestNote = note;
            }
        }
        
        if (closestNote != null)
        {
            // Check if correct hand was used
            bool correctHand = (closestNote.isLeftHandNote == wasLeftStick);
            
            // Score based on accuracy and correct hand
            string accuracy = "Miss";
            int points = 0;
            
            if (closestDistance <= perfectHitRadius)
            {
                accuracy = correctHand ? "PERFECT!" : "Wrong hand!";
                points = correctHand ? 200 : 50;
            }
            else if (closestDistance <= goodHitRadius)
            {
                accuracy = correctHand ? "Good!" : "Wrong hand!";
                points = correctHand ? 100 : 25;
            }
            
            Debug.Log($"{accuracy} +{points} points");
            
            // Visual feedback based on accuracy
            if (correctHand)
            {
                closestNote.HitSuccess(Color.green);  // Good hit
            }
            else
            {
                closestNote.HitSuccess(Color.yellow); // Wrong hand hit
            }
            
            // TODO: Add to score system
        }
        else
        {
            Debug.Log("Miss! Note not in range");
        }
    }
}