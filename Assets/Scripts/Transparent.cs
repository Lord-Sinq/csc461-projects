using UnityEngine;
using TMPro;

public class MakeTransparent : MonoBehaviour
{
    [Range(0f, 1f)]
    public float targetOpacity = 0.5f; // 1 = opaque, 0 = fully transparent

    [Header("TMP controls")]
    [Tooltip("TMPs to hide when the game starts (can be multiple).")]
    public TMP_Text[] hideTMPs;
    [Tooltip("Single TMP to reveal when the game starts.")]
    public TMP_Text revealTMP;

    [Header("Startup behaviour")]
    public bool hideTMPsStartHidden = true;
    public bool revealTMPStartsHidden = true;

    [Header("Reveal/fade settings")]
    [Range(0f, 1f)]
    public float revealAlpha = 1f;
    public float fadeDuration = 0.5f;

    // internal coroutine handle
    private Coroutine fadeCoroutine;

    public void SetTransparency(float opacity)
    {
        float a = Mathf.Clamp01(opacity);

        // Handle renderers / materials (3D TextMeshPro uses MeshRenderer)
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                // Ensure the shader supports transparency
                SetMaterialTransparent(mat);

                Color color = mat.color;
                color.a = a;
                mat.color = color;
            }
        }

        // Handle TextMeshPro components (TextMeshPro + TextMeshProUGUI use TMP_Text)
        TMP_Text[] tmpTexts = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text tmp in tmpTexts)
        {
            Color c = tmp.color;
            c.a = a;
            tmp.color = c;
        }
    }

    private void SetMaterialTransparent(Material mat)
    {
        // If already transparent, skip
        if (mat.GetTag("RenderType", false) == "Transparent")
            return;

        // Switch to transparent rendering mode for Standard Shader
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    void Start()
    {
        // Initialize hide TMPs
        if (hideTMPs != null && hideTMPs.Length > 0)
        {
            foreach (var t in hideTMPs)
            {
                if (t == null) continue;
                SetTMPAlphaImmediate(t, hideTMPsStartHidden ? 0f : t.color.a);
            }
        }

        // Initialize reveal TMP
        if (revealTMP != null && revealTMPStartsHidden)
        {
            SetTMPAlphaImmediate(revealTMP, 0f);
        }
    }

    // Convenience: call this when the big red button is pressed.
    // Hides all hideTMPs and reveals the single revealTMP (with optional smooth fade).
    public void ApplyGameStartState(bool smooth = true)
    {
        // hide the list
        if (hideTMPs != null)
        {
            foreach (var t in hideTMPs)
            {
                if (t == null) continue;
                if (smooth && fadeDuration > 0f)
                {
                    if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                    fadeCoroutine = StartCoroutine(FadeTMPAlpha(t, t.color.a, 0f, fadeDuration));
                }
                else
                {
                    SetTMPAlphaImmediate(t, 0f);
                }
            }
        }

        // reveal the chosen TMP
        if (revealTMP != null)
        {
            if (smooth && fadeDuration > 0f)
            {
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeTMPAlpha(revealTMP, revealTMP.color.a, revealAlpha, fadeDuration));
            }
            else
            {
                SetTMPAlphaImmediate(revealTMP, revealAlpha);
            }
        }
    }

    // Immediate set TMP alpha
    private void SetTMPAlphaImmediate(TMP_Text tmp, float a)
    {
        Color c = tmp.color;
        c.a = Mathf.Clamp01(a);
        tmp.color = c;
    }

    // Fade coroutine for single TMP
    private System.Collections.IEnumerator FadeTMPAlpha(TMP_Text tmp, float from, float to, float duration)
    {
        float elapsed = 0f;
        from = Mathf.Clamp01(from);
        to = Mathf.Clamp01(to);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float cur = Mathf.Lerp(from, to, t);
            SetTMPAlphaImmediate(tmp, cur);
            yield return null;
        }

        SetTMPAlphaImmediate(tmp, to);
        fadeCoroutine = null;
    }
}