using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public FloatingText floatingTextPrefab; // Drag prefab here
    public Canvas canvas;                   // Drag your UI canvas

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

    public void SpawnText(string message)
    {
        FloatingText ft = Instantiate(floatingTextPrefab, canvas.transform);
        ft.transform.localPosition = Vector3.zero; // Or any position you want
        ft.SetMessage(message);
    }
}