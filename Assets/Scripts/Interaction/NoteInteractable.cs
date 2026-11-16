using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteInteractable : MonoBehaviour
{
    [TextArea(3, 8)] public string noteText =
        "The exit is just ahead.\nKeep to the right.\n\nDON'T TRUST THIS PATH.";
    public GameObject promptUI;
    public string promptMessage = "Press E to read note";
    public NotePanel notePanel;

    bool playerInRange;
    bool promptShown;

    void Update()
    {
        bool shouldShow = playerInRange && !notePanel.IsOpen;

        if (shouldShow != promptShown) SetPrompt(shouldShow);

        if (shouldShow && Input.GetKeyDown(KeyCode.E))
            notePanel.Open(noteText);
    }

    void SetPrompt(bool show)
    {
        promptShown = show;
        if (!promptUI) return;

        if (show)
        {
            var tmp = promptUI.GetComponentInChildren<TMP_Text>(true);
            if (tmp) tmp.text = promptMessage;
            else
            {
                var legacy = promptUI.GetComponentInChildren<Text>(true);
                if (legacy) legacy.text = promptMessage;
            }
        }
        promptUI.SetActive(show);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}