using UnityEngine;

/// <summary>
/// First-person movement: WASD walk, Left Shift run, Space jump.
/// Uses a CharacterController ONLY (no Rigidbody on the player).
/// Attach to: the Player GameObject (tag = "Player").
/// Owner: Socheat (Player Controller)
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 6.5f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        bool grounded = controller.isGrounded;

        // Keep the player pressed onto the floor so isGrounded stays reliable.
        if (grounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        // Horizontal movement relative to where the player is facing.
        Vector2 input = GameInput.Move;
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        move = Vector3.ClampMagnitude(move, 1f);
        float speed = GameInput.RunHeld ? runSpeed : walkSpeed;

        // Jump.
        if (grounded && GameInput.JumpPressed)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}
