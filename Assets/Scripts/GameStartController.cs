using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using TMPro;
using UnityEditor.TextCore.Text;

public class GameStartController : MonoBehaviour
{
    public static GameStartController Instance { get; private set; }

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
    public TextMeshPro[] uiElementsToShow;
    public TextMeshPro scoreText;

    private bool hasStarted = false;
    public FloatingTextController floatingTextController;

    public int score = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this);
            return;
        }
    }

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
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
            OnButtonPressed();
#endif
    }

    public void OnButtonPressed()
    {
        if (hasStarted) return; // Only trigger once
        hasStarted = true;

        StartCoroutine(StartSequence());
    }
    private IEnumerator EndSequence()
    {
        // Turn off lights gradually
        if (pointLight != null)
        {
            float initialIntensity = pointLight.intensity;
            float duration = 2f; // seconds to fade out
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                pointLight.intensity = Mathf.Lerp(initialIntensity, 0f, t);
                yield return null;
            }

            pointLight.intensity = 0f;
        }
        if (scoreText != null)
        {
            RectTransform rt = scoreText.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Set new position (localPosition) instantly
                rt.localPosition = new Vector3(-11.73f, 6, 2.3f);
            }

            // Set new font size instantly
            scoreText.fontSize = 15f;//xample font size
        }

        // Restore skybox exposure gradually
        if (skyboxMaterial != null)
        {
            float currentExposure = skyboxMaterial.GetFloat("_Exposure");
            float duration = 2f;
            float timer = 0f;
            float targetExposureValue = 2.5f; // default exposure

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(currentExposure, targetExposureValue, t));
                yield return null;
            }

            skyboxMaterial.SetFloat("_Exposure", targetExposureValue);

            hasStarted = false;
        }
    }
    private IEnumerator MonitorSongEnd()
    {
        // Wait until the song finishes
        while (audioSource != null && audioSource.isPlaying)
            yield return null;

        // Song finished, start lights and skybox reset
        StartCoroutine(EndSequence());
    }

    private IEnumerator StartSequence()
    {
        
        SetUIElementsVisibility(false);

        if (scoreText != null)
        {
            RectTransform rt = scoreText.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Set new position (localPosition) instantly
                rt.localPosition = new Vector3(-6.311f, 1.954f, 0);
            }

            // Set new font size instantly
            scoreText.fontSize = 4f;//ample font size
        }

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
            StartCoroutine(MonitorSongEnd());

        }

        if (noteSpawner != null)
            noteSpawner.StartSpawning();
    }

    /// <summary>
    /// Called by other systems (e.g. DrumTopHitZone) to show feedback for a stick-enter event.
    /// noteHit == true => show "Great", false => show "BAD".
    /// worldPosition is optional and used to position the floating text near the hit.
    /// </summary>
    public void ShowHitFeedback(bool noteHit, Vector3? worldPosition = null)
    {
        if (!hasStarted)
        {
            Debug.Log("GameStartController: Ignoring feedback because game has not started.");
            return;
        }
        string msg = noteHit ? "Great" : "BAD";

        if (noteHit)
            score += 100;
        else
            score -= 50;

        if (scoreText != null)
            scoreText.text = $"Score: {score}";

        // Prefer explicit world position, then the spawner's spawnPoint, then this controller position.
        Vector3 pos = worldPosition ??
                      (noteSpawner != null && noteSpawner.spawnPoint != null ? noteSpawner.spawnPoint.position : transform.position);

        // Use existing floating text system in the project
        floatingTextController.SpawnText(msg);

        Debug.Log($"GameStartController: ShowHitFeedback('{msg}') at {pos}");
    }


    private void SetUIElementsVisibility(bool isVisible)
    {
        // --- Hide elements ---
        if (uiElementsToHide != null && uiElementsToHide.Length > 0)
        {
            foreach (TextMeshPro tmp in uiElementsToHide)
            {
                if (tmp == null) continue;

                // Skip XR button if needed
                if (xrButton != null && tmp.gameObject == xrButton.gameObject) continue;

                tmp.enabled = isVisible; // show if isVisible=true, hide if false
            }
        }

        // --- Show elements ---
        if (uiElementsToShow != null && uiElementsToShow.Length > 0)
        {
            foreach (TextMeshPro tmp in uiElementsToShow)
            {
                if (tmp == null) continue;

                tmp.enabled = !isVisible; // opposite of hide array
            }
        }

    }
}