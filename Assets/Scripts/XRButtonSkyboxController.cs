using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class XRButtonSkyboxController : MonoBehaviour
{
    [Header("References")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable xrButton;   // Your XR button
    public Light pointLight;                // The light to turn on
    public Material skyboxMaterial;         // The skybox material

    [Header("Settings")]
    public float dimDuration = 1f;          // How long to dim the skybox
    public float targetExposure = 0.0f;     // Final dim level

    [Header("Instructions Transparency")]
    [Tooltip("Assign the Instructions GameObject's MakeTransparent component")]
    public MakeTransparent instructionsTarget;
    [Range(0f, 1f)]
    public float instructionsOpacity = 0f; // 0 = invisible, 1 = opaque

    [Header("Spawner")]
    [Tooltip("Assign your DrumNoteSpawner here.")]
    public DrumNoteSpawner drumNoteSpawner;
    public bool startSpawningOnPress = true;

    [Header("Debug / Editor Controls")]
    [Tooltip("Allow Space to simulate the big red button while testing. Disable for release so only the physical button triggers the game.")]
    public bool enableEditorControls = false;

    [Header("Audio (optional)")]
    public AudioSource soundEffect;         // Play when light turns on

    private bool spawnStarted = false;

    private void Start()
    {
        if (skyboxMaterial != null)
            skyboxMaterial.SetFloat("_Exposure", 2f);
        // Ensure the light starts off
        if (pointLight != null)
            pointLight.intensity = 0f;

        // Make sure any assigned spawner will not auto-start (safety)
        if (drumNoteSpawner != null)
        {
            drumNoteSpawner.autoSpawn = false;
        }

        // Additionally turn off autoSpawn on any other spawners in the scene so none starts before button
        var allSpawners = FindObjectsOfType<DrumNoteSpawner>();
        foreach (var s in allSpawners)
            s.autoSpawn = false;

        // Hook up the XR button
        if (xrButton != null)
        {
            xrButton.selectEntered.AddListener(OnButtonPressed);
            Debug.Log("XRButtonSkyboxController: listener added to xrButton.selectEntered.");
        }
        else
        {
            Debug.LogWarning("XRButtonSkyboxController: xrButton not assigned in inspector. Assign the XRSimpleInteractable (big red button).");
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        // Press Space to simulate button press in editor (optional)
        if (enableEditorControls && Input.GetKeyDown(KeyCode.Space))
            PressButton(); // use the same entry point
#endif
    }

    // Public parameterless method so you can also hook the button via the inspector UnityEvent (OnSelectEnter) if needed.
    public void PressButton()
    {
        Debug.Log("XRButtonSkyboxController.PressButton() called.");
        HandleButtonActivated();
    }

    // This overload is used when selectEntered fires.
    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        Debug.Log("XRButtonSkyboxController.OnButtonPressed(SelectEnterEventArgs) invoked.");
        HandleButtonActivated();
    }

    // Centralized handler called by both entry points.
    private void HandleButtonActivated()
    {
        // Prevent double-activation
        if (spawnStarted && startSpawningOnPress)
        {
            Debug.Log("XRButtonSkyboxController: already started.");
        }

        // Turn Instructions opaque/transparent via MakeTransparent component
        if (instructionsTarget != null)
        {
            instructionsTarget.SetTransparency(Mathf.Clamp01(instructionsOpacity));
            Debug.Log("XRButtonSkyboxController: instructions transparency set.");
        }
        else
        {
            Debug.LogWarning("XRButtonSkyboxController: instructionsTarget not assigned.");
        }

        // Start spawning notes (once) if assigned
        if (startSpawningOnPress && !spawnStarted)
        {
            spawnStarted = true;
            // Set global flag so other systems know game is started
            GameState.GameStarted = true;

            if (drumNoteSpawner != null)
            {
                drumNoteSpawner.StartSpawning();
                Debug.Log("XRButtonSkyboxController: started assigned DrumNoteSpawner.");
            }
            else
            {
                // if no specific spawner assigned, start all spawners found in the scene
                var allSpawners = FindObjectsOfType<DrumNoteSpawner>();
                foreach (var s in allSpawners)
                {
                    s.StartSpawning();
                }
                Debug.Log($"XRButtonSkyboxController: started {allSpawners.Length} spawners found in scene.");
            }
        }

        // Continue existing behavior: dim skybox, enable light, play SFX
        StartCoroutine(DimSkyboxThenLight());
    }

    private IEnumerator DimSkyboxThenLight()
    {
        if (skyboxMaterial == null)
        {
            Debug.LogWarning("Skybox material not assigned!");
            yield break;
        }

        float initialExposure = skyboxMaterial.GetFloat("_Exposure");
        float timer = 0f;

        // Gradually dim the skybox
        while (timer < dimDuration)
        {
            timer += Time.deltaTime;
            float t = timer / dimDuration;
            float newExposure = Mathf.Lerp(initialExposure, targetExposure, t);
            skyboxMaterial.SetFloat("_Exposure", newExposure);
            yield return null;
        }

        // Ensure final exposure is exact
        skyboxMaterial.SetFloat("_Exposure", targetExposure);

        // Turn on the light immediately
        if (pointLight != null)
            pointLight.intensity = 100f; // Adjust intensity as desired

        // Play sound effect (optional)
        if (soundEffect != null)
            soundEffect.Play();
    }

    private void OnDestroy()
    {
        if (xrButton != null)
            xrButton.selectEntered.RemoveListener(OnButtonPressed);
    }
}
