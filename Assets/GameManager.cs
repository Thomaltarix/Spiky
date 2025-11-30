using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameObject titleScreenUI;
    public GameObject endScreenUI;

    private static GameManager instance;

    private bool _restarting = false;


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
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void EndGame()
    {
        PauseGame();
        endScreenUI.SetActive(true);
    }

    public void RestartGame()
    {
        _restarting = true;
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        titleScreenUI.SetActive(false);
        endScreenUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
