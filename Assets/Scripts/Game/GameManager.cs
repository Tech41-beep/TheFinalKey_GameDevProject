using UnityEngine;

/// <summary>
/// Central game state for the Game scene: counts keys and decides win / lose.
/// There must be exactly ONE GameManager, only in the Game scene.
/// Attach to: an empty "GameManager" object in the Game scene.
/// Owner: Kimtech (Scene Management and Game Flow)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public const int TotalKeys = 3;

    public int CollectedKeys { get; private set; }

    private bool gameEnded;

    private void Awake()
    {
        // Avoid duplicate GameManagers.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        // Show 0/3 when the game begins.
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateKeyCount(CollectedKeys, TotalKeys);
        }
    }

    public bool HasAllKeys()
    {
        return CollectedKeys >= TotalKeys;
    }

    public int KeysRemaining()
    {
        return TotalKeys - CollectedKeys;
    }

    /// <summary>Called by KeyCollectible when the player picks up a key.</summary>
    public void CollectKey()
    {
        if (gameEnded || CollectedKeys >= TotalKeys) return;

        CollectedKeys++;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateKeyCount(CollectedKeys, TotalKeys);
            UIManager.Instance.ShowMessage("Key collected! (" + CollectedKeys + "/" + TotalKeys + ")");
        }
    }

    /// <summary>Called by ExitDoor when the player leaves with all keys.</summary>
    public void WinGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        SceneController.Load(SceneController.WinScene);
    }

    /// <summary>Called by Hazard (or any future failure condition).</summary>
    public void LoseGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        SceneController.Load(SceneController.LoseScene);
    }

    /// <summary>Reloads the Game scene from the start.</summary>
    public void RestartGame()
    {
        SceneController.Load(SceneController.GameScene);
    }
}
