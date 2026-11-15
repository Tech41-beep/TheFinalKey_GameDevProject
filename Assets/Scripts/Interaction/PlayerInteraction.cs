using UnityEngine;

/// <summary>
/// Lets the player collect keys: look at a key, see the prompt, press E.
/// Third-person: the ray follows the crosshair but starts level with the player,
/// so range is measured from the player, not from the camera.
/// Attach to: the Main Camera.
/// Inspector: "Player Camera" = this camera, "Player" = the Player root.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform player;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private float chestHeight = 1.2f;

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
        if (playerCamera == null || player == null) return null;

        Transform cam = playerCamera.transform;

        // Point on the camera's line of sight that is level with the player's chest.
        Vector3 chest = player.position + Vector3.up * chestHeight;
        float along = Mathf.Max(0f, Vector3.Dot(chest - cam.position, cam.forward));
        Vector3 origin = cam.position + cam.forward * along;

        Ray ray = new Ray(origin, cam.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, ~0, QueryTriggerInteraction.Collide))
        {
            KeyCollectible key = hit.collider.GetComponentInParent<KeyCollectible>();
            if (key != null && !key.IsCollected) return key;
        }
        return null;
    }
}