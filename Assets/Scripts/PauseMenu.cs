using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    public GameObject GameOverUI;

    public GameObject bottomPlayer;

    private int playerIdleTime;

    // Update is called once per frame
    void Update()
    {
        playerIdleTime = bottomPlayer.GetComponent<KeyboardInputHandler>().playerIdleTime;
        if (!GameOverUI.activeSelf && Input.GetKeyDown(KeyCode.Escape)) {
            if (GameIsPaused) {
                Resume();
            } else {
                Pause();
            }
        }

        if (!GameOverUI.activeSelf && playerIdleTime > 500) {
            Pause();
        } else {
            Resume();
        }
    }

    void Resume() 
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()     
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
