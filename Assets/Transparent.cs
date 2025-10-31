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
}