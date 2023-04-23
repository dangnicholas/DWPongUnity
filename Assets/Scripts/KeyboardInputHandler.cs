using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputHandler : MonoBehaviour
{

    public bool driveFromMQTT = true;

    public float speed; // reflect value in original of 6px velocity per tick (0.6f)
    public float maxOffset; // (game width / 2 - paddle width / 2)

    // abstracting buttons allows for 2 controllers on the same keyboard
    public KeyCode leftButton = KeyCode.LeftArrow;
    public KeyCode rightButton = KeyCode.RightArrow;

    private Transform myTransform;
    private float MQTTInput;
    private int frame;
    private bool newFrame;

    public MQTTReceiver _eventSender;

    public int playerIdleTime;

    // Start is called before the first frame update
    void Start()
    {

        newFrame = false;
        // putting this here for now
        Application.targetFrameRate = 60;
        myTransform = gameObject.transform;

        if (_eventSender == null)
        {
            _eventSender = GetComponent<MQTTReceiver>();
        }
        _eventSender.OnConnectionSucceeded += OnConnectionSucceedHandler;
        _eventSender.OnMessageArrived += OnMessageArrivedHandler;
    }

    private void OnConnectionSucceedHandler(bool success)
    {
        //Debug.Log("Event Fired. Connected = " + success);
    }

    private void OnMessageArrivedHandler(string newMsg)
    {
        //Debug.Log("Event Fired. The message is = " + newMsg);
        try 
        {
            MQTTInput = float.Parse(newMsg.Replace("\"", ""));
            // Debug.Log("Frame " + Time.frameCount + " : Event Fired. The MQTTInput message is = " + MQTTInput);
        } catch (System.FormatException e) 
        {
            Debug.Log("Invalid input received from standalone AI");
        }
        newFrame = true;
    }

    // Update is called once per frame
    void Update()
    {
        // looking for specific keycodes is rudimentary and Unity's Input system is a better way to handle this
        // but all control will eventually be networked so this script is only for prototyping.

        if (!driveFromMQTT)
        {
            //drive position from listening to key presses
            HandleKeyInput();
        }
        else
        {
            HandleMQTTInput();
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

            // if (_eventSender.isConnected)
            // {
            //     Debug.Log("Sending paddle position");
            //     _eventSender.Publish("paddle/position", myTransform.position.x.ToString()); // sending current position
            // }
        }
    }

    private void HandleMQTTInput()
    {   
        
        float newSpeed = 0.0f;
        if (newFrame)
        {
            Dictionary<float, float> action = new Dictionary<float, float>(){ {0f,-1f}, {1f,1f}, {2f,0f} };
            MQTTInput = action[MQTTInput];

            newFrame = false;
            
        } 

        newSpeed +=  MQTTInput*speed;

        Vector3 newPosition = myTransform.position;
        newPosition.x += newSpeed;
        newPosition.x = Mathf.Clamp(newPosition.x, -maxOffset, maxOffset);

        myTransform.position = newPosition;

    }
}
