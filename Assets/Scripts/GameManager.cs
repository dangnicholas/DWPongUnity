using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

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
        playerOneText.GetComponent<TextMeshProUGUI>().text = _playerOneScore.ToString();
        
        this.ball.ResetPosition(gameLevel-1);

    }

    // This is the player's (bottom paddle) score keeping method
    public void PlayerTwoScore()
    {
        _playerTwoScore++;
        if (_playerTwoScore % 3 == 0) {
            _playerTwoScore = 0;
            gameLevel++;
            gameLevelText.GetComponent<TextMeshProUGUI>().text = "Level: " + gameLevel.ToString();

            if (_eventSender.isConnected)
            {
                _eventSender.Publish("game/level", ""+gameLevel);
                Debug.Log("PUBLISHED LEVEL");
            }
        }
        playerTwoText.GetComponent<TextMeshProUGUI>().text = _playerTwoScore.ToString();

        this.ball.ResetPosition(gameLevel-1);

    }
    
}
