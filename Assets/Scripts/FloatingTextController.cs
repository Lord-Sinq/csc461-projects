using UnityEngine;
using TMPro;

public class FloatingTextController : MonoBehaviour
{
    public FloatingText floatingTextPrefab; // Drag prefab here (must have RectTransform + FloatingText)
    public Canvas canvas;                   // Drag your UI canvas (Screen Space - Overlay / Camera / World)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            SpawnText("Up!");

        if (Input.GetKeyDown(KeyCode.DownArrow))
            SpawnText("Down!");

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SpawnText("Left!");

        if (Input.GetKeyDown(KeyCode.RightArrow))
            SpawnText("Right!");
    }

    // Old API: center of canvas
    public void SpawnText(string message)
    {
        SpawnText(message, (Vector3?)null);
    }

    // New: optionally spawn near a world position (e.g. where the note was hit)
    public void SpawnText(string message, Vector3? worldPosition)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogWarning("FloatingTextController: floatingTextPrefab not assigned.");
            return;
        }

        if (canvas == null)
        {
            Debug.LogWarning("FloatingTextController: canvas not assigned.");
            return;
        }

        FloatingText ft = Instantiate(floatingTextPrefab, canvas.transform);
        RectTransform ftRect = ft.GetComponent<RectTransform>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        if (ftRect == null || canvasRect == null)
        {
            Debug.LogWarning("FloatingTextController: Missing RectTransform on prefab or canvas.");
            return;
        }

        Vector2 anchoredPos = Vector2.zero;

        if (worldPosition.HasValue)
        {
            Camera cam = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                cam = null; // WorldToScreenPoint ignores camera when passed null for Overlay
            }
            else
            {
                cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
                if (cam == null)
                    Debug.LogWarning("FloatingTextController: No camera assigned for ScreenSpace - Camera or World canvas.");
            }

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPosition.Value);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out anchoredPos);
        }

        ftRect.anchoredPosition = anchoredPos;
        ft.SetText(message);

        Debug.Log($"FloatingTextController: Spawned '{message}' at anchoredPosition {anchoredPos} on canvas '{canvas.name}'.");
    }

    // Convenience wrapper for drum hits
    public void SpawnHitText(bool isLeft, Vector3? worldPosition = null)
    {
        SpawnText(isLeft ? "Left Hit" : "Right Hit", worldPosition);
    }
}