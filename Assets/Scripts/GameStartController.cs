using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using TMPro;

public class GameStartController : MonoBehaviour
{
    [Header("XR Button")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable xrButton;

    [Header("Skybox & Lighting")]
    public Material skyboxMaterial;
    public Light pointLight;
    public float dimDuration = 3f;      // Seconds to dim skybox
    public float targetExposure = 0.2f; // Final exposure
    public float lightIntensity = 100f; // Light turns on to this intensity

    [Header("Audio")]
    public AudioClip musicClip;
    public AudioClip soundEffectClip; // assign your MP3 here
    private AudioSource audioSource;   // internal AudioSource to play the clip

    [Header("Game Systems")]
    public DrumNoteSpawner noteSpawner; // Your existing spawner

    [Header("UI")]
    public TextMeshPro[] uiElementsToHide;

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
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
            OnButtonPressed(null);
        #endif
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (hasStarted) return; // Only trigger once
        hasStarted = true;

        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        SetUIElementsVisibility(false);

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
    private void SetUIElementsVisibility(bool isVisible)
    {
        if (uiElementsToHide == null || uiElementsToHide.Length == 0)
        {
            Debug.LogWarning("UI Elements array is empty. Nothing to hide.");
            return;
        }

        foreach (TextMeshPro tmp in uiElementsToHide)
        {
            if (tmp != null)
            {
                // The most reliable way to hide UI is to disable the GameObject.
                tmp.gameObject.SetActive(isVisible);
                
                // OR you can disable the renderer component:
                // tmp.enabled = isVisible;
                
                // OR you can change the alpha (transparency):
                // tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, isVisible ? 1f : 0f);
            }
        }
    }
}