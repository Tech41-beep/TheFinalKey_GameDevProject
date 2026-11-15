using UnityEngine;
[RequireComponent(typeof(Collider))]
public class Hazard : MonoBehaviour {
    private bool hasTriggered = false;

    // Makes sure the collider is a trigger, even if you forget to tick it.
    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hazard touched by: " + other.name);

        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Hazard: no GameManager found in the scene.");
            return;
        }

        hasTriggered = true;
        Debug.Log("Hazard hit: loading Lose scene.");
        GameManager.Instance.LoseGame();
    }
}