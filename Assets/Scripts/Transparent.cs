using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    [Range(0f, 1f)]
    public float targetOpacity = 0.5f; // 1 = opaque, 0 = fully transparent

<<<<<<< Updated upstream
=======
    [Header("TMP controls")]
    public TMP_Text[] hideTMPs;
    public TMP_Text revealTMP;

    [Header("Startup behaviour")]
    public bool hideTMPsStartHidden = true;
    public bool revealTMPStartsHidden = true;

    [Header("Reveal/fade settings")]
    [Range(0f, 1f)]
    public float revealAlpha = 1f;
    public float fadeDuration = 0.5f;

    // Skip changing any renderers that belong to objects tagged with this tag
    [Header("Safety")]
    [Tooltip("Renderers on GameObjects with this tag will be ignored by SetTransparency.")]
    public string ignoreTag = "UIButton";

    // internal coroutine handle
    private Coroutine fadeCoroutine;

>>>>>>> Stashed changes
    public void SetTransparency(float opacity)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            // SAFETY: skip renderers that are part of UI buttons or have ButtonTriggerForwarder in parents
            if (!string.IsNullOrEmpty(ignoreTag) && rend.gameObject.CompareTag(ignoreTag))
                continue;
            if (rend.GetComponentInParent<ButtonTriggerForwarder>() != null)
                continue;

            foreach (Material mat in rend.materials)
            {
                // Ensure the shader supports transparency
                SetMaterialTransparent(mat);

                Color color = mat.color;
                color.a = Mathf.Clamp01(opacity);
                mat.color = color;
            }
        }
<<<<<<< Updated upstream
=======

        // Handle TextMeshPro components (TextMeshPro + TextMeshProUGUI use TMP_Text)
        TMP_Text[] tmpTexts = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text tmp in tmpTexts)
        {
            // skip if TMP belongs to ignored button
            if (!string.IsNullOrEmpty(ignoreTag) && tmp.gameObject.CompareTag(ignoreTag))
                continue;
            if (tmp.GetComponentInParent<ButtonTriggerForwarder>() != null)
                continue;

            Color c = tmp.color;
            c.a = a;
            tmp.color = c;
        }
>>>>>>> Stashed changes
    }

    private void SetMaterialTransparent(Material mat)
    {
        if (mat.GetTag("RenderType", false) == "Transparent")
            return;

        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}