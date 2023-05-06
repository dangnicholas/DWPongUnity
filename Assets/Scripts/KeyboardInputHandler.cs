using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class KeyboardInputHandler : MonoBehaviour
{
    [Tooltip("Option to use depth camera as player input. Will use keyboard if false.")]
    public bool playerUseDepthCamera = false;

    [Tooltip("Option to use AI's input to move the paddle. False will use keyboard")]
    public bool driveFromMQTT = true;
    [Tooltip("Option to send game state responsively. False will send every n frames. Must check same value in AI Camera's GameStateToMQTT scritp as well")]
    public bool responsiveFrameInference;

    public float speed; // reflect value in original of 6px velocity per tick (0.6f)
    public float maxOffset; // (game width / 2 - paddle width / 2) which is 8.1f

    // abstracting buttons allows for 2 controllers on the same keyboard
    public KeyCode leftButton = KeyCode.LeftArrow;
    public KeyCode rightButton = KeyCode.RightArrow;

    // The game objects transform. Used to change the position of the paddle.
    public Transform myTransform;

    // The AI model's output. 0 is left, 1 is right, 2 is do nothing
    private float aiInference;

    // The Unity game frame that the AI read in and performed the inference on.
    float aiFrame;

    // Will update the paddle action if new AI action
    private bool newMQTTInput;

    // The MQTT receiver to read in the AI's actions
    public MQTTReceiver _eventReceiver;


    // Keeps track of player idle time. Increments per game frame. Used to pause the game if player idles too long
    public int playerIdleTime;

    // The AI camera responsible for taking a snapshot of the current gamestate and sending it to the AI through MQTT
    public GameStateToMQTT gameStateToMQTT;

    public class AIObject {
        public string action { get; set; }
        public string frame { get; set; }
    }

    // Start is called before the first frame update
    void Start()
    {
        newMQTTInput = false;
        Application.targetFrameRate = 60;
        myTransform = gameObject.transform;

        if (_eventReceiver == null)
        {
            _eventReceiver = GetComponent<MQTTReceiver>();

        }

        _eventReceiver.OnConnectionSucceeded += OnConnectionSucceedHandler;
        _eventReceiver.OnMessageArrived += OnMessageArrivedHandler;
    }

    private void OnConnectionSucceedHandler(bool success)
    {
    }

    private void OnMessageArrivedHandler(string newMsg)
    {
        try 
        {
            var newMsgJson = JsonConvert.DeserializeObject<AIObject>(newMsg);
            if (newMsgJson.action != null) {
                aiInference = float.Parse(newMsgJson.action);

                if (responsiveFrameInference) {
                    gameStateToMQTT.publishGameState();
                }
                newMQTTInput = true;
                Debug.Log("Input received " + aiInference + " | frame " + Time.frameCount);

            } else if (newMsgJson.frame != null) {
                aiFrame = float.Parse(newMsgJson.frame);
            }

            //string[] frameAction = newMsg.Replace("\"", "").Split('_');
            //frame = float.Parse(frameAction[0]);
            //action = float.Parse(frameAction[1]);

        } catch (System.FormatException e) 
        {
            Debug.Log("Invalid input received from standalone AI");
        }

        

    }

    // Update is called once per frame
    void Update()
    {
        // looking for specific keycodes is rudimentary and Unity's Input system is a better way to handle this
        // but all control will eventually be networked so this script is only for prototyping.

        if (driveFromMQTT) {
            HandleMQTTInput();
        } else if (playerUseDepthCamera) {
            HandleDepthCamera();
        } else {
            HandleKeyInput();
        }
    }

    private void HandleKeyInput()
    {

        var buttonDown = false;
        float newSpeed = 0.0f;
        if (Input.GetKey(rightButton)) {
            newSpeed += speed;
            buttonDown = true;
            playerIdleTime = 0;
        } else if (Input.GetKey(leftButton)) {
            newSpeed += -speed;
            buttonDown = true;
            playerIdleTime = 0;
        } else {
            playerIdleTime += 1;
        }

        if (buttonDown)
        {
            Vector3 newPosition = myTransform.position;
            newPosition.x += newSpeed;
            newPosition.x = Mathf.Clamp(newPosition.x, -maxOffset, maxOffset);
            myTransform.position = newPosition;

        }
    }

    private void HandleMQTTInput()
    {   
        
        float newSpeed = 0.0f;

        if (newMQTTInput) {
            Dictionary<float, float> action = new Dictionary<float, float>() { { 0f, -1f }, { 1f, 1f }, { 2f, 0f } };
            aiInference = action[aiInference];

            newMQTTInput = false;
        } 

        newSpeed +=  aiInference*speed;

        Vector3 newPosition = myTransform.position;
        newPosition.x += newSpeed;
        newPosition.x = Mathf.Clamp(newPosition.x, -maxOffset, maxOffset);

        myTransform.position = newPosition;

    }

    private void HandleDepthCamera() 
    {
        Vector3 newPosition = myTransform.position;
        if (newPosition.x != aiInference) 
        {
            newPosition.x = aiInference;
            newPosition.x = Mathf.Clamp(newPosition.x, -maxOffset, maxOffset);

            myTransform.position = newPosition;
        }

       
    }
}
