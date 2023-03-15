using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public MQTTReceiver _eventSender;

    // We will handle the general loop and game logic here

    [Header("Player PreFabs")]
    public GameObject playerPrefab;
    public GameObject opponentPrefab;

    // The ball which we reset position once game loop ends
    public Ball ball;

    // These are the UI elements that display the score and game level
    public GameObject playerOneText;
    public GameObject playerTwoText;
    public GameObject gameLevelText;

    // These will keep track of the player scores and the game level
    private int _playerOneScore;
    private int _playerTwoScore;
    private int gameLevel;

    public GameObject gameOverUI;


    // These are game variables that can be changed
    private static int nextLevelPointRequirement = 3;
    private static int maxGameLevel = 3;
    private static int gameOverPointRequirement = 3;



    // Start is called before the first frame update
    void Start()
    {
        gameLevel = 1;
        if (_eventSender == null)
        {
            _eventSender = GetComponent<MQTTReceiver>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // This is the AI's (top paddle) score keeping method
    public void PlayerOneScore()
    {
        _playerOneScore++;
        if (_playerOneScore == gameOverPointRequirement) 
        {
            Debug.Log("Game Over. Resetting game!");
            StartCoroutine(GameOverResetGame());
        } else 
        {
            playerOneText.GetComponent<TextMeshProUGUI>().text = _playerOneScore.ToString();

            this.ball.ResetPosition(gameLevel - 1);
        }

        

    }

    // This is the player's (bottom paddle) score keeping method
    public void PlayerTwoScore()
    {
        _playerTwoScore++;

        if (gameLevel != maxGameLevel && _playerTwoScore % nextLevelPointRequirement == 0)
            {
            _playerTwoScore = 0;
            gameLevel++;
            playerTwoText.GetComponent<TextMeshProUGUI>().text = _playerTwoScore.ToString();
            this.ball.ResetPosition(gameLevel - 1);
            gameLevelText.GetComponent<TextMeshProUGUI>().text = "Level: " + gameLevel.ToString();
            if (_eventSender.isConnected) 
                {
                _eventSender.Publish("game/level", "" + gameLevel);
                // Debug.Log("PUBLISHED LEVEL");
            }
        } else 
        {
            playerTwoText.GetComponent<TextMeshProUGUI>().text = _playerTwoScore.ToString();
            this.ball.ResetPosition(gameLevel - 1);
        }

        
        

    }

    // This will reset the game if the player lost 3 points to the AI
    public IEnumerator GameOverResetGame() 
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Game Manager WAITING");
        //string currentSceneName = SceneManager.GetActiveScene().name;
        //SceneManager.LoadScene(currentSceneName);
        _playerOneScore = 0;
        _playerTwoScore = 0;
        gameLevel = 1;

        gameLevelText.GetComponent<TextMeshProUGUI>().text = "Level: " + gameLevel.ToString();
        playerOneText.GetComponent<TextMeshProUGUI>().text = _playerOneScore.ToString();
        playerTwoText.GetComponent<TextMeshProUGUI>().text = _playerTwoScore.ToString();

        if (_eventSender.isConnected) {
            _eventSender.Publish("game/level", "" + gameLevel);
        }

        this.ball.ResetPosition(gameLevel - 1);
        //Wait for 2 seconds
        yield return new WaitForSecondsRealtime(2);
        Debug.Log("Game Manager WAIT DONE");
        gameOverUI.SetActive(false);
        Time.timeScale = 1f;
    }

}
