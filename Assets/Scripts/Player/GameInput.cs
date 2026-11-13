using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Tiny helper that reads keyboard/mouse input.
/// Unity 6 projects may use the new Input System or the old Input Manager,
/// so this one file hides the difference. Every other script calls GameInput
/// and never touches UnityEngine.Input directly.
/// Owner: Socheat (Player Controller)
/// </summary>
public static class GameInput
{
    /// <summary>WASD movement. x = left/right, y = forward/back (-1 to 1).</summary>
    public static Vector2 Move
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard kb = Keyboard.current;
            if (kb == null) return Vector2.zero;
            float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
            float y = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
            return new Vector2(x, y);
#else
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif
        }
    }

    /// <summary>Mouse movement this frame (already scaled to feel the same on both input systems).</summary>
    public static Vector2 Look
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            Mouse mouse = Mouse.current;
            if (mouse == null) return Vector2.zero;
            return mouse.delta.ReadValue() * 0.1f;
#else
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
        }
    }

    /// <summary>True only on the frame Space is pressed.</summary>
    public static bool JumpPressed
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.Space);
#endif
        }
    }

    /// <summary>True while Left Shift is held.</summary>
    public static bool RunHeld
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
#else
            return Input.GetKey(KeyCode.LeftShift);
#endif
        }
    }

    /// <summary>True only on the frame E is pressed.</summary>
    public static bool InteractPressed
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.E);
#endif
        }
    }
}
