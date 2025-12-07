using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class VRHomeWithControllers : MonoBehaviour
{
    [Header("Controller Setup")]
    public XRController leftController;
    public XRController rightController;
    
    [Header("Start Button")]
    public XRSimpleInteractable startButton; // 3D button with XR Simple Interactable
    
    [Header("Scene Settings")]
    public string gameSceneName = "GameScene";
    public float buttonPressDelay = 0.5f;
    
    [Header("Visual Feedback")]
    public MeshRenderer buttonRenderer;
    public Material normalMaterial;
    public Material hoverMaterial;
    public Material pressedMaterial;
    
    private AudioSource audioSource;
    private bool canPressButton = true;
    
    void Start()
    {
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Setup button events
        if (startButton != null)
        {
            startButton.hoverEntered.AddListener(OnButtonHover);
            startButton.hoverExited.AddListener(OnButtonHoverExit);
            startButton.selectEntered.AddListener(OnButtonPressed);
        }
        else
        {
            Debug.LogError("Assign a 3D button with XR Simple Interactable!");
        }
        
        Debug.Log("VR Home Ready. Point controller at button and press trigger.");
    }
    
    void OnButtonHover(HoverEnterEventArgs args)
    {
        // Visual feedback when pointing at button
        if (buttonRenderer != null && hoverMaterial != null)
            buttonRenderer.material = hoverMaterial;
        
        // Haptic feedback on hover
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            SendHapticImpulse(controllerInteractor.xrController, 0.2f, 0.1f);
        }
    }
    
    void OnButtonHoverExit(HoverExitEventArgs args)
    {
        // Reset button appearance
        if (buttonRenderer != null && normalMaterial != null)
            buttonRenderer.material = normalMaterial;
    }
    
    void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (!canPressButton) return;
        
        // Visual feedback
        if (buttonRenderer != null && pressedMaterial != null)
            buttonRenderer.material = pressedMaterial;
        
        // Haptic feedback
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            SendHapticImpulse(controllerInteractor.xrController, 0.5f, 0.2f);
        }
        
        // Play sound
        if (audioSource != null && audioSource.clip != null)
            audioSource.Play();
        
        // Start game
        canPressButton = false;
        Invoke(nameof(LoadGameScene), buttonPressDelay);
        
        Debug.Log("Button pressed! Starting game...");
    }
    
    void SendHapticImpulse(XRBaseController controller, float amplitude, float duration)
    {
        if (controller != null)
        {
            controller.SendHapticImpulse(amplitude, duration);
        }
    }
    
    void LoadGameScene()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("No game scene name specified!");
        }
    }
    
    void Update()
    {
        // Optional: Keyboard testing in Editor
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadGameScene();
        }
        #endif
    }
    
    void OnDestroy()
    {
        // Clean up event listeners
        if (startButton != null)
        {
            startButton.hoverEntered.RemoveListener(OnButtonHover);
            startButton.hoverExited.RemoveListener(OnButtonHoverExit);
            startButton.selectEntered.RemoveListener(OnButtonPressed);
        }
    }
}