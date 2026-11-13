using UnityEngine;

/// <summary>
/// First-person mouse look.
/// Left/right turns the whole player body; up/down tilts only the camera.
/// Attach to: the Main Camera (a child of the Player).
/// Inspector: assign "Player Body" = the Player root object.
/// Owner: Socheat (Player Controller)
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float maxLookAngle = 85f;

    private float pitch;

    private void Start()
    {
        // Hide and lock the mouse while playing.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 look = GameInput.Look * sensitivity;

        // Turn the body left/right.
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up, look.x);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Tilt the camera up/down (clamped so you cannot flip over).
        pitch = Mathf.Clamp(pitch - look.y, -maxLookAngle, maxLookAngle);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
