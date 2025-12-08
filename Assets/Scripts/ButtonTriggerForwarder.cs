using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ButtonTriggerForwarder : MonoBehaviour
{
    [Tooltip("XRButtonSkyboxController instance to call when trigger happens.")]
    public XRButtonSkyboxController xrController;

    [Tooltip("GameStartController instance to call when trigger happens.")]
    public GameStartController gameStartController;

    [Tooltip("Tag of the controller hand/rig colliders that should activate the button.")]
    public string handTag = "PlayerHand";

    [Tooltip("Optional SpriteRenderer for the button (will be forced visible at runtime).")]
    public SpriteRenderer spriteRenderer;

    [Header("Physics safety")]
    [Tooltip("If true the script will ensure a kinematic Rigidbody exists and freeze transforms so the button doesn't move.")]
    public bool enforceStaticAtRuntime = true;

    [Header("Debug")]
    public bool debugLogs = true;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void Start()
    {
        // Ensure collider is a trigger so OnTriggerEnter runs reliably
        var col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            if (debugLogs) Debug.Log($"ButtonTriggerForwarder: set collider.isTrigger on '{gameObject.name}'.");
        }

        // Ensure SpriteRenderer visible (some transparency scripts may hide it)
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && !spriteRenderer.enabled)
        {
            spriteRenderer.enabled = true;
            if (debugLogs) Debug.Log($"ButtonTriggerForwarder: enabled SpriteRenderer on '{gameObject.name}'.");
        }

        // Prevent physics / other scripts from moving the button at runtime
        if (enforceStaticAtRuntime)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            if (debugLogs) Debug.Log($"ButtonTriggerForwarder: ensured Rigidbody kinematic + FreezeAll on '{gameObject.name}'.");
        }

        // Try to auto-find controllers if not assigned (helps inspector mistakes)
        if (gameStartController == null)
        {
            gameStartController = FindObjectOfType<GameStartController>();
            if (gameStartController != null && debugLogs)
                Debug.Log($"ButtonTriggerForwarder: auto-assigned GameStartController '{gameStartController.gameObject.name}'.");
        }

        if (xrController == null)
        {
            xrController = FindObjectOfType<XRButtonSkyboxController>();
            if (xrController != null && debugLogs)
                Debug.Log($"ButtonTriggerForwarder: auto-assigned XRButtonSkyboxController '{xrController.gameObject.name}'.");
        }

        if (gameStartController == null && xrController == null && debugLogs)
            Debug.LogWarning("ButtonTriggerForwarder: no controller assigned or found in scene.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (debugLogs) Debug.Log($"ButtonTriggerForwarder: OnTriggerEnter by '{other.name}' tag='{other.tag}'");

        if (!string.IsNullOrEmpty(handTag) && !other.CompareTag(handTag))
        {
            if (debugLogs) Debug.Log($"ButtonTriggerForwarder: ignoring '{other.name}' (tag != {handTag}).");
            return;
        }

        // Prefer GameStartController if assigned (so sprite can trigger the game start)
        if (gameStartController != null)
        {
            if (debugLogs) Debug.Log($"ButtonTriggerForwarder: calling PressButton() on GameStartController '{gameStartController.gameObject.name}'.");
            gameStartController.PressButton();
            return;
        }

        if (xrController != null)
        {
            if (debugLogs) Debug.Log($"ButtonTriggerForwarder: calling PressButton() on XRButtonSkyboxController '{xrController.gameObject.name}'.");
            xrController.PressButton();
            return;
        }

        Debug.LogWarning("ButtonTriggerForwarder: no controller available to call PressButton().");
    }
}