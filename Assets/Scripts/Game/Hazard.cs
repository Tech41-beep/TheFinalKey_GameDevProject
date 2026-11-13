using UnityEngine;

public class Hazard : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
        {
            hasTriggered = true;
            GameManager.Instance.LoseGame();
        }
    }
}