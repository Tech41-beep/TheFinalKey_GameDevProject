using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The ONLY place in the project that loads scenes.
/// - Buttons call the public instance methods (LoadGame, LoadMainMenu, QuitGame).
/// - Other scripts (GameManager) call the static Load(...) method.
/// Attach to: an empty "SceneController" object in MainMenu, Win and Lose scenes.
/// Owner: Kimtech (Scene Management and Game Flow)
/// </summary>
public class SceneController : MonoBehaviour
{
    // Scene names must match the files in Assets/Scenes and Build Settings.
    public const string MainMenuScene = "MainMenu";
    public const string GameScene = "Game";
    public const string WinScene = "Win";
    public const string LoseScene = "Lose";

    private void Start()
    {
        // Menus need a visible mouse. (CameraController locks it again in the Game scene.)
        bool isGame = SceneManager.GetActiveScene().name == GameScene;
        Cursor.lockState = isGame ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isGame;
        Time.timeScale = 1f;
    }

    /// <summary>Loads a scene by name and resets the time scale.</summary>
    public static void Load(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    // ---- Methods for UI buttons (OnClick) ----
    public void LoadMainMenu() { Load(MainMenuScene); }
    public void LoadGame() { Load(GameScene); }   // also used for Restart / Retry
    public void LoadWin() { Load(WinScene); }
    public void LoadLose() { Load(LoseScene); }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
