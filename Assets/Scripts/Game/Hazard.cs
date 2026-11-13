using UnityEngine;

/// <summary>
/// PROPOSED lose condition (team must approve): touching a hazard zone ends the game.
/// To disable it, delete the "Hazard" object in the Game scene.
/// Attach to: a Hazard object (Collider with Is Trigger ON).
/// Owner: Socheat / Bonsak (placement)
/// </summary>
public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoseGame();
        }
    }
}
