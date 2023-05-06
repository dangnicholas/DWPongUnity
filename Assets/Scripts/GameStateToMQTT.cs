using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.Linq;

using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

using Newtonsoft.Json;

public class GameStateToMQTT : MonoBehaviour  // : M2MqttUnityClient //
{
    Camera snapCam;
    int resWidth = 256;
    int resHeight = 256;

    [Tooltip("Option to set the n amount of frames to pass before sending game state")]
    public int inferenceFrameInterval;

    [Tooltip("Option to send game state responsively. False will send every n frames. Must check same value in TopPlayer KeyboardInputHandler script as well")]
    public bool responsiveFrameInference;

    [Tooltip("Option to publish paddle and ball positions instead of sending over snapshot of game")]
    public bool publishPositions;

    [Tooltip("Option to grab gamestate from camera, otherwise publish paddle and pong positions. Using to debug AI currently")]
    public bool useCameraGamestate;

    // The MQTT sender to send the game state to the AI
    public MQTTReceiver _eventSender;

    // Used when responsiveFrameInference is set to true
    bool firstGameState = true;


    // Start is called before the first frame update
    void Start()
    {
        if (_eventSender == null)
        {
            _eventSender = GetComponent<MQTTReceiver>();
        }

    }

    private void Awake() 
    {
        // Creates a snapCam if needed which will screenshot the game to send over MQTT
        snapCam = GetComponent<Camera>();
        if (snapCam.targetTexture == null) 
        {
            snapCam.targetTexture = new RenderTexture(resWidth, resHeight, 16);
        } else
        {
            resWidth = snapCam.targetTexture.width;
            resHeight = snapCam.targetTexture.height;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (responsiveFrameInference) {
            if ((Time.frameCount > 30) && (firstGameState)) {
                Debug.Log("Publish game start | frame " + Time.frameCount);
                publishGameState();
                firstGameState = false;
            }
        } else {
            if (Time.frameCount % inferenceFrameInterval == 0) {
                if (useCameraGamestate) {
                    snapCam.Render();
                    RenderTexture.active = snapCam.targetTexture;
                    Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
                    publishGameState();
                } else {
                    // Testing this out
                    old_publishGameState();
                }
            }
        }
        
    }

    public void publishGameState() 
    {
        snapCam.Render();
        RenderTexture.active = snapCam.targetTexture;
        Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        // Gets a snapshot of the game to send over MQTT
        snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        Color[] colors = snapshot.GetPixels();

        // turns the image into a list of 0s and 1s based on if there is color or not to pass to MQTT
        int[] values = colors.Select(color => 
            {
                if (color.r > 0 || color.b > 0 || color.g > 0) { return 1; } 
                else { return 0; }
            }
            ).ToArray();


        // Sends the gamestate over MQTT for inference from an AI
        if (_eventSender.isConnected)
        {
            
            _eventSender.Publish("camera/gamestate", JsonConvert.SerializeObject(new { gamestate = values })); //string.Join("", values)

            _eventSender.Publish("game/frame", JsonConvert.SerializeObject(new { frame = Time.frameCount })); //string.Join("", Time.frameCount));

            Debug.Log("Published gamestate | frame " + Time.frameCount + " | gamestate size " + values.Length);
        }
    }

    // Will publish paddles and ball positions which was the same as the last senior design project
    // Publishing a snapshot from the AI camera was our initial plan to have the model be flexible for more game objects
    // but it is currently underperforming.
    public void old_publishGameState() {
        ;
        Transform topPlayerTransform = GameObject.Find("TopPlayer").GetComponent<Transform>();
        Transform bottomPlayerTransform = GameObject.Find("BottomPlayer").GetComponent<Transform>();
        Transform ballTransform = GameObject.Find("Ball").GetComponent<Transform>();

        string topPlayerPayload = JsonConvert.SerializeObject(new { position = topPlayerTransform.position.x* 10 + 81 });
        string bottomPlayerPayload = JsonConvert.SerializeObject(new { position = bottomPlayerTransform.position.x* 10 + 81 });
        string ballPayload = JsonConvert.SerializeObject(new { x = ballTransform.position.x * 10 + 96, y = 160 - (ballTransform.position.z * 10 + 80) });

        // Sends the gamestate over MQTT for inference from an AI
        if (_eventSender.isConnected) {
            _eventSender.Publish("puck/position", ballPayload);
            _eventSender.Publish("paddle2/position", topPlayerPayload);
            _eventSender.Publish("game/frame", JsonConvert.SerializeObject(new { frame = Time.frameCount })); //string.Join("", Time.frameCount)
            _eventSender.Publish("paddle1/position", bottomPlayerPayload);
        }
    }

}

