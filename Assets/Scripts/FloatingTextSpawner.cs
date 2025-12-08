using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance { get; private set; }

    public FloatingText floatingTextPrefab; // Drag prefab here (must contain a RectTransform and FloatingText)
    public Canvas canvas;                   // Drag your UI canvas (Screen Space - Overlay / Camera / World)

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

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

    // Simple spawn at canvas center
    public void SpawnText(string message)
    {
        SpawnText(message, (Vector3?)null);
    }

    // Spawn with a world position so the text appears near the hit note on the canvas
    public void SpawnText(string message, Vector3? worldPosition)
    {
        if (floatingTextPrefab == null || canvas == null) return;

        FloatingText ft = Instantiate(floatingTextPrefab, canvas.transform);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransform ftRect = ft.GetComponent<RectTransform>();

        // Default position: center
        Vector2 anchoredPos = Vector2.zero;

        if (worldPosition.HasValue)
        {
            Camera cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPosition.Value);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out anchoredPos);
        }

        ftRect.anchoredPosition = anchoredPos;
        ft.SetText(message);
    }

    // Convenience method to show which drumstick hit
    public void SpawnHitText(bool isLeft, Vector3? worldPosition = null)
    {
        string msg = isLeft ? "Left Hit" : "Right Hit";
        SpawnText(msg, worldPosition);
    }
}