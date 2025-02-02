/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

// examine: https://workshops.cetools.org/codelabs/CASA0019-unity-mqtt/index.html?index=..%2F..index#3

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class MQTTReceiver : M2MqttUnityClient
{

    #region Variables
    [Header("MQTT topics")]
    [Tooltip("Set the topic to subscribe. !!!ATTENTION!!! multi-level wildcard # subscribes to all topics")]
    public string topicSubscribe = "#"; // topic to subscribe. !!! The multi-level wildcard # is used to subscribe to all the topics. Attention i if #, subscribe to all topics. Attention if MQTT is on data plan
    [Tooltip("Set the topic to publish (optional)")]
    public string topicPublish = ""; // topic to publish

    [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
    public bool autoTest = false;

    //using C# Property GET/SET and event listener to reduce Update overhead in the controlled objects
    private string m_msg;

    public string msg
    {
        get
        {
            return m_msg;
        }
        set
        {
            if (m_msg == value) return;
            m_msg = value;
            if (OnMessageArrived != null)
            {
                OnMessageArrived(m_msg);
            }
        }
    }


    public event OnMessageArrivedDelegate OnMessageArrived;
    public delegate void OnMessageArrivedDelegate(string newMsg);

    //using C# Property GET/SET and event listener to expose the connection status
    private bool m_isConnected;

    public bool isConnected
    {
        get
        {
            return m_isConnected;
        }
        set
        {
            if (m_isConnected == value) return;
            m_isConnected = value;
            if (OnConnectionSucceeded != null)
            {
                OnConnectionSucceeded(isConnected);
            }
        }
    }
    public event OnConnectionSucceededDelegate OnConnectionSucceeded;
    public delegate void OnConnectionSucceededDelegate(bool isConnected);

    // a list to store the messages
    private List<string> eventMessages = new List<string>();

    #endregion

    #region Functions

    public void Publish(string topic, string msgToPublish)
    {
        client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(msgToPublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
    }

    protected override void OnConnecting()
        {
            base.OnConnecting();
        }

    protected override void OnConnected()
        {
            base.OnConnected();
            isConnected=true;

            if (autoTest)
            {
                Publish("TEST","TEST");
            }
        }

    protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
        }

    protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
            isConnected=false;
        }

    protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
        }

    protected override void SubscribeTopics()
        {
            client.Subscribe(new string[] { topicSubscribe }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

    protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { topicSubscribe });
        }

    protected override void Start()
        {
            base.Start();
        }

    protected override void DecodeMessage(string topic, byte[] message)
        {
            //The message is decoded
            msg = System.Text.Encoding.UTF8.GetString(message);

            //Debug.Log("Received: " + msg);
            //Debug.Log("from topic: " + m_msg);

            StoreMessage(msg);
            if (topic == topicSubscribe)
            {
                if (autoTest)
                {
                    autoTest = false;
                    Disconnect();
                }
            }
        }

    private void StoreMessage(string eventMsg)
        {
            if (eventMessages.Count > 50)
            {
                eventMessages.Clear();
            }
            eventMessages.Add(eventMsg);
        }

    protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

        }

    private void OnDestroy()
        {
            Disconnect();
        }

    private void OnValidate()
        {
            if (autoTest)
            {
                autoConnect = true;
            }
        }

    #endregion

}
