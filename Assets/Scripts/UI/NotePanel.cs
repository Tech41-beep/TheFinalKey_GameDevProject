using UnityEngine;
using TMPro;

public class NotePanel : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text bodyText;
    public bool IsOpen => panel.activeSelf;

    void Start() => panel.SetActive(false);

    void Update()
    {
        if (IsOpen && Input.GetKeyDown(KeyCode.Escape)) Close();
    }

    public void Open(string text)
    {
        bodyText.text = text;
        panel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }
}