using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class GameStartController : MonoBehaviour
{
<<<<<<< Updated upstream
    [Header("XR Button")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable xrButton;

    [Header("Skybox & Lighting")]
    public Material skyboxMaterial;
    public Light pointLight;
    public float dimDuration = 3f;      // Seconds to dim skybox
    public float targetExposure = 0.2f; // Final exposure
    public float lightIntensity = 100f; // Light turns on to this intensity

=======
    [Header("Start Button")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable startButton;  // 3D button in VR space
    public GameObject startButtonUI;           // Optional UI button
    
    [Header("Game Systems")]
    public DrumNoteSpawner noteSpawner;       // Your existing spawner script
    public GameObject drumSticks;             // Drum sticks to enable
    public GameObject countdownDisplay;       // 3, 2, 1, GO! display
    
    [Header("Settings")]
    public float countdownTime = 3f;          // 3 second countdown
    public bool autoStartOnPickup = false;    // Start when sticks are picked up
    
>>>>>>> Stashed changes
    [Header("Audio")]
    public AudioClip musicClip;
    public AudioClip soundEffectClip; // assign your MP3 here
    private AudioSource audioSource;   // internal AudioSource to play the clip

    [Header("Game Systems")]
    public DrumNoteSpawner noteSpawner; // Your existing spawner

    private bool hasStarted = false;

    private void Start()
    {   
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        // Initial skybox exposure
        if (skyboxMaterial != null)
            skyboxMaterial.SetFloat("_Exposure", 2.5f);

        // Initial light off
        if (pointLight != null)
            pointLight.intensity = 0f;

        // Hook up XR button
        if (xrButton != null)
            xrButton.selectEntered.AddListener(OnButtonPressed);
    }

    private void Update()
    {
<<<<<<< Updated upstream
=======
        if (!gameStarted)
        {
            StartGame();
        }
    }
    
    public void StartGame()
    {
        if (gameStarted) return;
        
        gameStarted = true;
        
        // Disable start button
        if (startButton != null)
            startButton.enabled = false;
        
        // Start countdown
        StartCoroutine(GameStartSequence());
    }
    
    System.Collections.IEnumerator GameStartSequence()
    {
        Debug.Log("Game starting in 3...");
        
        // Show countdown
        if (countdownDisplay != null)
        {
            countdownDisplay.SetActive(true);
            Text countdownText = countdownDisplay.GetComponent<Text>();
            
            // Countdown from 3
            for (int i = 3; i > 0; i--)
            {
                if (countdownText != null)
                    countdownText.text = i.ToString();
                
                // Play beep
                if (countdownBeep != null)
                    audioSource.PlayOneShot(countdownBeep);
                
                yield return new WaitForSeconds(1f);
            }
            
            // GO!
            if (countdownText != null)
                countdownText.text = "GO!";
            
            if (gameStartSound != null)
                audioSource.PlayOneShot(gameStartSound);
            
            yield return new WaitForSeconds(0.5f);
            
            // Hide countdown
            countdownDisplay.SetActive(false);
        }
        else
        {
            // No visual countdown, just wait
            yield return new WaitForSeconds(countdownTime);
        }
        
        // START THE GAME!
        StartGameplay();
    }
    
    void StartGameplay()
    {
        Debug.Log("GAME STARTED! Notes spawning now!");
        
        // Enable note spawning
        if (noteSpawner != null)
        {
            noteSpawner.enabled = true;
            
            // If your spawner has a StartSpawning method, call it
            var method = noteSpawner.GetType().GetMethod("StartSpawning");
            if (method != null)
            {
                method.Invoke(noteSpawner, null);
            }
            else
            {
                // Try to start InvokeRepeating
                noteSpawner.InvokeRepeating("SpawnRandomNote", 0f, 1.5f);
            }
        }
        
        // Enable drum sticks if they were disabled
        if (drumSticks != null)
        {
            drumSticks.SetActive(true);
            
            // Make sticks grabbable
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable[] grabComponents = drumSticks.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            foreach (var grab in grabComponents)
            {
                grab.enabled = true;
            }
        }
        
        // You could also start background music here
    }
    
    void Update()
    {
        // Optional: Start game when sticks are picked up
        if (autoStartOnPickup && !gameStarted)
        {
            // Check if sticks are being held
            if (AreSticksPickedUp())
            {
                StartGame();
            }
        }
        
        // Editor testing
>>>>>>> Stashed changes
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
            OnButtonPressed(null);
        #endif
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
<<<<<<< Updated upstream
        if (hasStarted) return; // Only trigger once
        hasStarted = true;

        StartCoroutine(StartSequence());
=======
        // Check if drum sticks are being held
        // This depends on your XR setup
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable[] grabs = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        foreach (var grab in grabs)
        {
            if (grab.isSelected) // Stick is being held
                return true;
        }
        return false;
>>>>>>> Stashed changes
    }

    private IEnumerator StartSequence()
    {
        // --- 1. Dim Skybox ---
        if (skyboxMaterial != null)
        {
            float initialExposure = skyboxMaterial.GetFloat("_Exposure");
            float timer = 0f;

            while (timer < dimDuration)
            {
                timer += Time.deltaTime;
                float t = timer / dimDuration;
                float newExposure = Mathf.Lerp(initialExposure, targetExposure, t);
                skyboxMaterial.SetFloat("_Exposure", newExposure);
                yield return null;
            }

            skyboxMaterial.SetFloat("_Exposure", targetExposure);
        }

        // --- 2. Turn on Light ---
        if (pointLight != null)
            pointLight.intensity = lightIntensity;

        // --- 3. Play Sound ---
        if (soundEffectClip != null)
            audioSource.PlayOneShot(soundEffectClip);


        // --- 4. Start Note Spawner ---
        if (musicClip != null)
        {
            audioSource.clip = musicClip;
            audioSource.loop = false;
            audioSource.Play();
        }

        if (noteSpawner != null)
            noteSpawner.StartSpawning();
    }

    private void OnDestroy()
    {
        if (xrButton != null)
            xrButton.selectEntered.RemoveListener(OnButtonPressed);
    }
}