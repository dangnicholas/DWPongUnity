# MQTT Overview

 MQTT and mosquitto will be used for inter-communication within the game. The installation could be found here: [MQTT download](https://mosquitto.org/download/)

## Unity to MQTT
This contains the information sent and received from this Unity game repository: [DWPongUnity](https://github.com/dangnicholas/DWPongUnity)

  - **Publish to MQTT**
     - camera/gamestate: 
        1. Publishes a single string of 0s (paddles and ball) and 1s (empty space) with 92160 digits (e.g. "0000011100...")
     - game/frame:
        1. Publishes a frame count as a string which is grabbed from Unity's Time.frameCount
     - game/level:
        1. Publishes a game level as string
  - **Received from MQTT**
     - paddle1/action
        1. Receives a number from 0-2 (left, right, do nothing) based on inference
     - motion/position
        1. Receives a position ranging from -1.15 to 0.15 representing the player position from the depth camera. Used to control bottom paddle 


## AI to MQTT
This contains the information sent and received from this Unity game repository: [DWPong_AI](https://github.com/dangnicholas/DWPong_AI)

  - **Publish to MQTT**
     - paddle1/action
        1. Publishes a number from 0-2 (left, right, do nothing) based on inference
     - paddle1/frame
        1. Publishes the frame the AI did inference on.
     - motion/position
        1. Publishes a position ranging from -1.15 to 0.15 representing the player position from the depth camera. Used to control bottom paddle 
  - **Received from MQTT**
     - camera/gamestate: 
        1. Receives a single string of 0s (paddles and ball) and 1s (empty space) with 92160 digits (e.g. "0000011100...")
        2. String is converted into a 160x192x3 np array for model input
     - game/frame:
        1. Receives a frame count as a string which is grabbed from Unity's Time.frameCount
     - game/level:
        1. Receives a game level as string