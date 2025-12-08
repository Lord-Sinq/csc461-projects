using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float lifetime = 1f;

    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string message)
    {
        if (text != null)
        {
            text.text = message;  // THIS MUST SET THE TEXT
        }

        Destroy(gameObject, lifetime);  // auto-remove
    }

    void Update()
    {
        // move up
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // fade out
        if (text != null)
        {
            Color c = text.color;
            c.a -= Time.deltaTime / lifetime;
            text.color = c;
        }
    }
}