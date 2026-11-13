using UnityEngine;

/// <summary>
/// A collectible key. Collected once, then it disappears.
/// PlayerInteraction calls Collect() when the player presses E while looking at it.
/// Attach to: the Key prefab root (needs a Collider with Is Trigger ON).
/// Owner: Sarim (UI and Player Interaction)
/// </summary>
public class KeyCollectible : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 90f;
    [SerializeField] private float bobHeight = 0.1f;
    [SerializeField] private float bobSpeed = 2f;

    private bool collected;
    private Vector3 startPosition;

    public bool IsCollected { get { return collected; } }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Gentle spin and float so keys are easy to notice.
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        transform.position = startPosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
    }

    public void Collect()
    {
        if (collected) return; // a key can only count once
        collected = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CollectKey();
        }

        gameObject.SetActive(false);
    }
}
