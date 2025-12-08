using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class GameStartController : MonoBehaviour
{
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
    
    [Header("Audio")]
    public AudioClip countdownBeep;
    public AudioClip gameStartSound;
    
    private bool gameStarted = false;
    private AudioSource audioSource;
    
    void Start()
    {
        // Get audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Setup start button
        if (startButton != null)
        {
            startButton.selectEntered.AddListener(OnStartButtonPressed);
        }
        
        // Setup UI button
        if (startButtonUI != null)
        {
            Button uiButton = startButtonUI.GetComponent<Button>();
            if (uiButton != null)
            {
                uiButton.onClick.AddListener(StartGame);
            }
        }
        
        // Disable note spawning initially
        if (noteSpawner != null)
        {
            // Make sure your DrumNoteSpawner has a way to stop/start
            // If not, add: public void StopSpawning() and public void StartSpawning()
            noteSpawner.enabled = false;
        }
        
        // Hide countdown
        if (countdownDisplay != null)
            countdownDisplay.SetActive(false);
        
        Debug.Log("Game ready. Press START button to begin!");
    }
    
    void OnStartButtonPressed(SelectEnterEventArgs args)
    {
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
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space) && !gameStarted)
        {
            StartGame();
        }
        #endif
    }
    
    bool AreSticksPickedUp()
    {
        // Check if drum sticks are being held
        // This depends on your XR setup
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable[] grabs = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        foreach (var grab in grabs)
        {
            if (grab.isSelected) // Stick is being held
                return true;
        }
        return false;
    }
    
    void OnDestroy()
    {
        // Clean up
        if (startButton != null)
        {
            startButton.selectEntered.RemoveListener(OnStartButtonPressed);
        }
    }
}