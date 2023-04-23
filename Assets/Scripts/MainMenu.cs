using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MainMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject mainMenuUI;
    public GameObject settingMenuUI;

    public GameObject GameOverUI;
    public GameObject pauseMenuUI;

    public GameObject sliderValueText;

    private void Start() {
        Pause();
    }

    // Update is called once per frame
    void Update() {
        if (!GameOverUI.activeSelf && !pauseMenuUI.activeSelf && Input.GetKeyDown(KeyCode.M)) {
            if (GameIsPaused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    void Resume() {
        mainMenuUI.SetActive(false);
        settingMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause() {
        mainMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void PlayGame() {
        mainMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void SettingsMenu() {
        mainMenuUI.SetActive(false);
        settingMenuUI.SetActive(true);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void BackButton() {
        mainMenuUI.SetActive(true);
        settingMenuUI.SetActive(false);
    }

    public void SetBallSpeedMultiplier(float ballSpeedMultiplier) {
        GameObject.Find("Ball").GetComponent<Ball>().SetBallSpeedMultiplier(ballSpeedMultiplier);
        sliderValueText.GetComponent<TextMeshProUGUI>().SetText(Math.Round(ballSpeedMultiplier, 2).ToString());
    }

}
