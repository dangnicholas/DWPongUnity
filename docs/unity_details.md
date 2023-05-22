# Unity Details

## Game Objects

### Main Camera
- Transform
    - The position of the camera which is on origin on the x and z axis and 12.49 above the game on the y axis
- Camera
    - Render Type is Base type to render the game to the screen
- Audio Listener
    - Automatically created when camera game object created.
- Universal Additional Camera Data (Script)
    - Automatically created when camera game object created.

### Global Volume
- Transform
    - Positioned at origin, unused

### AI Camera
- Transform
   - positioned on origin x and z axis (-0.02) with the y axis being 11.32 just closer and below the Main Camera.
- Camera
    - Render Type is Base type to render the game to the screen
- Universal Additional Camera Data (Script)
    - Automatically created when camera game object created.
- Camera
    - Render Type is Base type to render the game to the screen
- Audio Listener
    - Automatically created when camera game object created.
- Game State To MQTT (Script)
    - Found in Assets > Scripts
    - Inference Frame Interval: Frame interval to send gamestate to MQTT
    - Responsive Frame Inference: Option to send gamestate immediately after receiving AI action. Currently False. (check is True)
    - Publish Positions: Currently not used
    - Use Camera Gamestate: Option to send the gamestate by snapshotting game and sending as 2d array of 0s and 1s representing the game state. Currently False. (check is True)
    - Event Sender: Uses an MQTT Receiver object in the bullet point below.
- MQTT Receiver: Found in Assets > MQTT > MQTTReceiver.cs. Used to subscribe and publish to MQTT
    - All configurations are intially set upon adding component to game object except the following below.
    - Auto Connect: To automatically connect. Currently True.
    - Topic Subscribe: game/level. Where the AI will listen for the game level
    - Topic Publish: camera/gamestate. Where the MQTT Receiver will publish the gamestate if "Use Camera Gamestate" is checked in Game State To MQTT (Script) above component.
    
    
### GameManager
- Transform
    - Not necessary since not a visible game object player interacts with
- Game Manager (Script)
    - Found in Assets > Scripts
    - Event Sender AI Camera (MQTT Receiver): MQTT Receiver object already created and used to publish game state if "Use Camera Gamestate" is unchecked. Will publish current game level. 3 points required to advance to next game level. Levels are from 1 to 3 and stays at 3 as the max game level.
    - Player Prefab: Not currently used.
    - Opponent Prefab: Not currently used.
    - Ball: The Ball script found on Ball game object. Will call to reset position on ball if a player scores. Passes the current game level to adjust the speech that the ball gets launched.
    - Player One Text: Found in Heirarchy Canvas > GamePlayUI > PlayerOneScoreUI. Used to change the Text to display the player one score.
    - Player Two Text: Found in Heirarchy Canvas > GamePlayUI > PlayerTwoScoreUI. Used to change the Text to display the player two score.
    - Game Level Text: Found in Heirarchy Canvas > GamePlayUI > LevelUI. Used to change the text to the game level.
    - Player Score Sound Effect: The Audio Source component found in GameManager game object.
    - Audio Source: Found in Assets > Sounds > score. Used to play sound when a player scores. Can change the volume settings of the sound.
    
    
### BottomPlayer
- Transform
    - Positioned at (0,0,-7). X is scaled 1.5 for a wider paddle.
- Keyboard Input Handler (Script)
    - Found in Assets > Scripts > KeyboardInputHandler.cs
    - Player Use Depth Camera. To allow the bottom paddle to be controlled with Depth camera. Can always use Keyboard "a" and "d" keys. Currently checked (True).
    - Drive From MQTT: Option to use AI from MQTT to control paddle. Currently false (unchecked).
    - Responsive Frame Inference: Option to publish gamestate responsively immediately after it receives AI input. Check adjustable settings documentation in Unity Overview.
    - Speed: The speed of the paddle. Currently 0.3.
    - Max Offset: The max position +-8.1 left and right from the center of the game the paddle can move.
    - Depth Min Position: The minimum value the depth camera outputs when standing as close to edge (left) of depth camera to get reading. Used for conversion formula of depth camera value ranges to Unity position value ranges.
    - Depth Max Position: The maximum value the depth camera outputs when standing as close to edge (right) of depth camera to get reading. Used for conversion formula of depth camera value ranges to Unity position value ranges.
    - Left Button: The keyboard button to move paddle left. (A)
    - Right Button: The keyboard button to move paddle right. (D)
    - My Transform: Currently unused.
    - Event Receiver: Uses an MQTT Receiver object in the bullet point below.
- MQTT Receiver (Script)
    - Found in Assets > MQTT > MQTTReceiver.cs. Used to subscribe and publish to MQTT
    - All configurations are intially set upon adding component to game object except the following below.
    - Auto Connect: To automatically connect. Currently True.
    - Topic Subscribe: motion/position. Will listen for motion_driver.py depth camera position values published to this topic to control paddle.
    - Topic Publish: motion/position. Not currently used.

### Bounds
- Bounds were created by Riley and we added Goal script to top and bottom barrier and renamed them to be easier to follow.
- RightBarrier: A visible wall that the Ball will bounce off.
- LefttBarrier: A visible wall that the Ball will bounce off.
- BottomBarrier: A visible wall that the Ball will bounce off.
    - Goal (Script): Found in Assets > Scripts > Goal.cs. If Ball game object collides with it, will call GameManager (Game Object) > GameManager.cs (Script) > PlayerOneScore (or PlayerTwoScore) depending on Player Goal
    - Player Goal: Set to 1 so player one score method called.
- TopBarrier: A visible wall that the Ball will bounce off.
    - Goal (Script): Found in Assets > Scripts > Goal.cs. If Ball game object collides with it, will call GameManager (Game Object) > GameManager.cs (Script) > PlayerOneScore (or PlayerTwoScore) depending on Player Goal
    - Player Goal: Set to 2 so player two score method called.

### Ball
- Transform: Set to be (0,0,-2.42) Set by Riley. Ball is center of game but closer to bottom player.
- Sphere Collider: Disabled when adding the player 1 and 2 scoring functionality to have it work. Created by Riley.
- Ball (Script):
    - Found in Assets > Scripts. Initially created by Riley. Added the pause when the ball resets position and uses level to set the initial launch speed.
    - Launch Speed: 7. The speed to launch the ball initially.
    - Speed Multiplier: The multiplier to increase ball speed each time it bounces. Currently 0.1.
    - Max Speed: The max speed the ball can go. Currenly 10.
    - Launch Angle Bounds: The max angle to launch the ball. Angle is randomly calculated within this bounds. Currently 30.
    - RigidBody: Set to none but is grabbed and used in Ball (Script) for the physics of ball. Created by Riley.
    - Ball Bounce Sound Effect: Ball (Audio Source). The sound the ball makes when it bounces. Component added found below bullet point.
    - Audio Source: Found in Assets > Sounds > bounce. Used to play sound when a player scores. Can change the volume settings of the sound.
    
### Lighting
- Unused. Created by Riley.

### Top Player
- Transform
    - Positioned at (0,0,7). X is scaled 1.5 for a wider paddle.
- Keyboard Input Handler (Script)
    - Found in Assets > Scripts > KeyboardInputHandler.cs
    - Player Use Depth Camera. To allow the bottom paddle to be controlled with Depth camera. Can always use Keyboard "a" and "d" keys. Currently unchecked (False).
    - Drive From MQTT: Option to use AI from MQTT to control paddle. Currently true (checked).
    - Responsive Frame Inference: Option to publish gamestate responsively immediately after it receives AI input. Check adjustable settings documentation in Unity Overview.
    - Speed: The speed of the paddle. Currently 0.3.
    - Max Offset: The max position +-8.1 left and right from the center of the game the paddle can move.
    - Depth Min Position: The minimum value the depth camera outputs when standing as close to edge (left) of depth camera to get reading. Used for conversion formula of depth camera value ranges to Unity position value ranges. 1.15
    - Depth Max Position: The maximum value the depth camera outputs when standing as close to edge (right) of depth camera to get reading. Used for conversion formula of depth camera value ranges to Unity position value ranges. -0.15
    - Left Button: The keyboard button to move paddle left. None. AI controls this.
    - Right Button: The keyboard button to move paddle right. None. AI controls this.
    - My Transform: Currently unused.
    - Event Receiver: Uses an MQTT Receiver object in the bullet point below.
- MQTT Receiver (Script)
    - Found in Assets > MQTT > MQTTReceiver.cs. Used to subscribe and publish to MQTT
    - All configurations are intially set upon adding component to game object except the following below.
    - Auto Connect: To automatically connect. Currently True.
    - Topic Subscribe: paddle1/#. Will listen for AI action and frame it inferenced on. MQTT Overview documentation has details on how the AI publishes.
    - Topic Publish: motion/fram_action. Not currently used.

### Quad
- The bottom checkered board of the game. Created by Riley.

### EventSystem
- Currently Unused.

### Canvas
- These UI components can be viewed in Unity Overview documentation.
- GamePlayUI
    - PlayerOneScoreUI: Text of the player score
    - PlayerTwoScoreUI: Text of the player score
    - LevelUI: Text of the Game level
- PauseMenu
    - PausedText: Text on a panel labeled Paused
- GameOver
    - GameOverText: Text on a panel labeled Game Over
- MainMenu
    - MenuTitle
        - Text of the menu title
    - PlayButton
        - Will hide the main menu, unpause game, and game will play.
    - SettingsButton
        - Initial page to adjust settings but maybe remove in exhibit so visitors don't have chance of accessing this.
    - Quit
        - Exits the Unity application. Can remove so vistors can't access and quit the game.
- SettingsMenu
    - BallSpeedSlider: A slider with text that display the ball speed multiplier that can change when this slider is used.
    - BackButton: Goes back to Main menu screen
    - Shows a slider and text to allow user to change the ball speed multiplier as the ball bounces each time. Can remove this settings menu so visitors can't change things easily.


## Scripts

All C# scripts used in Unity are found in Assets > Scripts Folder.

### Ball.cs
- Usage: Ball (Game Object)

### GameManager.cs
- Usage: GameManager (Game Object)

### GameStateToMQTT.cs
- Usage: AICamera (Game Object)

### Goal.cs
- Usage: Bounds > TopBarrier, BottomBarrier

### KeyboardInputHandler.cs
- Usage: BottomPlayer, TopPlayer

### MainMenu.cs
- Usage: Canvas (Game Object)

### PauseMenu.cs
- Usage: Canvas (Game Object)

### MQTTReceiver.cs
- Assets > MQTT folder added by Riley.
- Found in Assets > MQTT > MQTTReceiver.cs
- Usage: BottomPlayer, TopPlayer, AICamera

## Materials

Materials found in Assets > Materials

### Paddle
- Usage: Ball > Cube, BottomPlayer > Cube , TopPlayer > Cube

### GameBoard
- Usage: Quad (Game Object)

## Models

- Models found in Assets > Models
- Folder added by Riley.

## Ball

- Models found in Assets > Ball
- Folder added by Riley.

## Player

- Contains prefab for a paddle

## Sounds

Contains sounds used in game.

### bounce.ogg
- Usage: Ball (Game Object)

### return.ogg
- Usage: Not currently used

### score.ogg
- Usage: GameManager (Game Object)

### credits.txt
- To credit the sounds used.

