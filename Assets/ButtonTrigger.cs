using UnityEngine;

public class MBButtonTrigger : MonoBehaviour
{
    public GameStartController gameStartController;

    private void OnTriggerEnter(Collider other)
    {
        // Detect if the collider is the Meta controller
        if (other.CompareTag("MetaController")) // make sure your controller has this tag
        {
            if (gameStartController != null)
                gameStartController.OnButtonPressed();
        }
    }
}