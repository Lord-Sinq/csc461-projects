using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 30f;     // Upward movement speed
    public float lifeTime = 1f;       // How long before disappearing
    public TextMeshProUGUI textUI;    // Reference to TMP text

    private float timer = 0f;
    private Color startColor;

    void Start()
    {
        startColor = textUI.color;
    }

    void Update()
    {
        // Move upward
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // Fade out
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / lifeTime);
        textUI.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        // Destroy after lifetime
        if (timer >= lifeTime)
            Destroy(gameObject);
    }

    public void SetMessage(string message)
    {
        textUI.text = message;
    }
}
