using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class XRButtonSkyboxController : MonoBehaviour
{
    [Header("References")]
    public XRSimpleInteractable xrButton;   // Your XR button
    public Light pointLight;                // The light to turn on
    public Material skyboxMaterial;         // The skybox material

    [Header("Settings")]
    public float dimDuration = 5f;          // How long to dim the skybox
    public float targetExposure = 0.0f;     // Final dim level

    [Header("Audio (optional)")]
    public AudioSource soundEffect;         // Play when light turns on

    private void Start()
    {
        if (skyboxMaterial != null)
            skyboxMaterial.SetFloat("_Exposure", 2f);
        // Ensure the light starts off
        if (pointLight != null)
            pointLight.intensity = 0f;

        // Hook up the XR button
        if (xrButton != null)
            xrButton.selectEntered.AddListener(OnButtonPressed);
    }

    private void Update()
    {
        #if UNITY_EDITOR
        // Press Space to simulate button press in editor
        if (Input.GetKeyDown(KeyCode.Space))
            OnButtonPressed(null);
        #endif
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
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
