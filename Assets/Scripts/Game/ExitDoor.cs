using UnityEngine;

/// <summary>
/// The exit. Wins the game only if all 3 keys are collected,
/// otherwise tells the player how many keys are still missing.
/// Attach to: the Exit object (BoxCollider with Is Trigger ON).
/// Owner: Sarim (UI and Player Interaction)
/// </summary>
public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.HasAllKeys())
        {
            GameManager.Instance.WinGame();
        }
        else if (UIManager.Instance != null)
        {
            int missing = GameManager.Instance.KeysRemaining();
            string word = missing == 1 ? "key" : "keys";
            UIManager.Instance.ShowMessage("The door is locked! You need " + missing + " more " + word + ".");
        }
    }
}
