using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.Linq;

using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


public class GameStateToMQTT : MonoBehaviour  // : M2MqttUnityClient //
{
    Camera snapCam;
    int resWidth = 256;
    int resHeight = 256;

    public MQTTReceiver _eventSender;

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

        // This sends the gamestate every 15 frames since our laptops can't inference fast
        if (Time.frameCount % 15 == 0)
        {
            snapCam.Render();
            RenderTexture.active = snapCam.targetTexture;
            Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            publishGameState(snapshot);
        }

    }

    void publishGameState(Texture2D snapshot) 
    {
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
            Debug.Log("Published shape: " + values.Length);
            _eventSender.Publish("camera/gamestate", string.Join("", values));

            Debug.Log("Published frame: " + Time.frameCount);
            _eventSender.Publish("game/frame", string.Join("", Time.frameCount));
        }
    }

}

