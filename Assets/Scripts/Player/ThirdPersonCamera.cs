using UnityEngine;

/// <summary>
/// Third-person orbit camera.
/// Attach to: the Player root.
/// Inspector: assign "Camera Transform" = Main Camera (the child of Player).
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Camera Settings")]
    [SerializeField] private float distance = 4f;
    [SerializeField] private float height = 1.6f;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float minPitch = 5f;
    [SerializeField] private float maxPitch = 45f;

    [Header("Collision")]
    [SerializeField] private float collisionRadius = 0.2f;
    [SerializeField] private LayerMask collisionMask = ~0;

    private float yaw;
    private float pitch = 15f;

    private void Start()
    {
        yaw = transform.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Vector2 look = GameInput.Look * sensitivity;
        yaw += look.x;
        pitch = Mathf.Clamp(pitch - look.y, minPitch, maxPitch);

        // Player turns with the camera so W moves away from it.
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focus = transform.position + Vector3.up * height;
        Vector3 direction = rotation * Vector3.back;

        // Pull the camera in if a wall is between it and the player.
        float actualDistance = distance;
        if (Physics.SphereCast(focus, collisionRadius, direction, out RaycastHit hit,
                distance, collisionMask, QueryTriggerInteraction.Ignore))
        {
            if (!hit.transform.IsChildOf(transform))
            {
                actualDistance = Mathf.Max(hit.distance, 0.3f);
            }
        }

        // World position and rotation override the camera's child offset.
        cameraTransform.position = focus + direction * actualDistance;
        cameraTransform.rotation = rotation;
    }
}