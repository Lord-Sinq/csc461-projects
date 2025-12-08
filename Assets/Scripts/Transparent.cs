using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    [Range(0f, 1f)]
    public float targetOpacity = 0.5f; // 1 = opaque, 0 = fully transparent

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