using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameObject titleScreenUI;
    public GameObject endScreenUI;
    public GameObject playerStatsUI;

    private static GameManager instance;

    private bool _restarting = false;

    private bool _upgrading = false;

    public bool IsPaused { get; private set; } = false;

    private void Start()
    {
        PauseGame();

        if (_restarting)
        {
            _restarting = false;
            StartGame();
        }
        else
        {
            titleScreenUI.SetActive(true);
            endScreenUI.SetActive(false);
            playerStatsUI.SetActive(false);
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            _restarting = instance._restarting;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void PauseGame()
    {
        IsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void EndGame()
    {
        PauseGame();
        endScreenUI.SetActive(true);
        playerStatsUI.SetActive(false);
    }

    public void RestartGame()
    {
        instance._restarting = true;
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void StartGame()
    {
        IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        titleScreenUI.SetActive(false);
        endScreenUI.SetActive(false);
        playerStatsUI.SetActive(true);
        Time.timeScale = 1f;
    }

    public void UpgradeScreen()
    {
        if (!_upgrading)
        {
            PauseGame();
            _upgrading = true;
        }
        else 
        {
            StartGame();
            _upgrading = false;
        }
    }
}
