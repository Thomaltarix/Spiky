using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject titleScreenUI;


    private void Start()
    {
        PauseGame();
    }

    private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        titleScreenUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
