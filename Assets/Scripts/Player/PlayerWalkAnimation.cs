using UnityEngine;

/// <summary>
/// Swings the arms and legs while the player moves.
/// Attach to: the Player root.
/// Inspector: assign the four limb Transforms.
/// </summary>
public class PlayerWalkAnimation : MonoBehaviour
{
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;
    [SerializeField] private Transform leftLeg;
    [SerializeField] private Transform rightLeg;

    [SerializeField] private float swingAngle = 35f;
    [SerializeField] private float swingSpeed = 8f;

    private CharacterController controller;
    private Vector3[] basePos = new Vector3[4];
    private Vector3[] pivot = new Vector3[4];
    private Transform[] limbs;
    private float phase;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        limbs = new[] { leftArm, rightArm, leftLeg, rightLeg };

        for (int i = 0; i < limbs.Length; i++)
        {
            basePos[i] = limbs[i].localPosition;
            // Capsule is 2 units tall, so the top of the limb is scale.y above its center.
            pivot[i] = basePos[i] + Vector3.up * limbs[i].localScale.y;
        }
    }

    private void Update()
    {
        Vector3 flat = controller.velocity;
        flat.y = 0f;
        float speed = flat.magnitude;
        bool moving = speed > 0.1f && controller.isGrounded;

        float amount = moving ? swingAngle * Mathf.Clamp01(speed / 4f) : 0f;
        if (moving) phase += Time.deltaTime * swingSpeed * Mathf.Max(1f, speed / 4f);

        float swing = Mathf.Sin(phase) * amount;

        // Opposite arm and leg swing together.
        Apply(0, swing);    // left arm
        Apply(1, -swing);   // right arm
        Apply(2, -swing);   // left leg
        Apply(3, swing);    // right leg
    }

    private void Apply(int i, float angle)
    {
        Quaternion rot = Quaternion.Euler(angle, 0f, 0f);
        limbs[i].localRotation = rot;
        limbs[i].localPosition = pivot[i] + rot * (basePos[i] - pivot[i]);
    }
}