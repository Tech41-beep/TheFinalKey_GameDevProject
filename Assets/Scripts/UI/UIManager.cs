using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the in-game HUD: key progress, objective, interaction prompt and messages.
/// Attach to: the "GameUI" Canvas in the Game scene.
/// Inspector: assign the four Text references.
/// Owner: Sarim (UI and Player Interaction)
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Text objectiveText;
    [SerializeField] private Text keyCountText;
    [SerializeField] private Text promptText;
    [SerializeField] private Text messageText;
    [SerializeField] private float messageDuration = 2.5f;

    private Coroutine messageRoutine;

    private void Awake()
    {
        Instance = this;

        if (objectiveText != null) objectiveText.text = "Objective: Collect 3 Keys and escape";
        if (promptText != null) promptText.gameObject.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Shows progress such as "Keys: 1/3".</summary>
    public void UpdateKeyCount(int collected, int total)
    {
        if (keyCountText != null)
        {
            keyCountText.text = "Keys: " + collected + "/" + total;
        }
    }

    /// <summary>Shows or hides "Press E to Collect".</summary>
    public void ShowPrompt(bool show)
    {
        if (promptText != null)
        {
            promptText.text = "Press E to Collect";
            promptText.gameObject.SetActive(show);
        }
    }

    /// <summary>Shows a message in the middle of the screen for a few seconds.</summary>
    public void ShowMessage(string message)
    {
        if (messageText == null) return;

        if (messageRoutine != null) StopCoroutine(messageRoutine);
        messageRoutine = StartCoroutine(MessageRoutine(message));
    }

    private IEnumerator MessageRoutine(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        messageText.gameObject.SetActive(false);
        messageRoutine = null;
    }
}
