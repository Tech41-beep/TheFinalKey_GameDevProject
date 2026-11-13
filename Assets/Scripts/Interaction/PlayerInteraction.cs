using UnityEngine;

/// <summary>
/// Lets the player collect keys: look at a key, see the prompt, press E.
/// Attach to: the Main Camera (child of Player).
/// Inspector: assign "Player Camera" = this camera.
/// Owner: Sarim (UI and Player Interaction)
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;

    private KeyCollectible target;

    private void Update()
    {
        KeyCollectible found = FindKeyInView();

        // Only touch the UI when the target changes.
        if (found != target)
        {
            target = found;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPrompt(target != null);
            }
        }

        if (target != null && GameInput.InteractPressed)
        {
            target.Collect();
            target = null;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPrompt(false);
            }
        }
    }

    private KeyCollectible FindKeyInView()
    {
        if (playerCamera == null) return null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, ~0, QueryTriggerInteraction.Collide))
        {
            KeyCollectible key = hit.collider.GetComponentInParent<KeyCollectible>();
            if (key != null && !key.IsCollected) return key;
        }
        return null;
    }
}
