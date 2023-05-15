# Unity Overview

This page contains an overview of the Unity pong game implementation found here: [DWPongUnity](https://github.com/dangnicholas/DWPongUnity)

## Game Objects Overview
- **Main Camera**: The camera that the player sees
- **AI Camera**: The camera that is filtered to only see the top paddle and ball
- **Game Manager**: The game object that handles the basic game loop
- **Top Player**: The top paddle that the AI controls
- **Bottom Player**: The bottom paddle that the player controls
- **Ball**: A ball game object that can bounce on the bounds and paddles
- **Bounds**: Contains 4 game objects for each wall of the game
- **Quad**: The floor of the game
- **Canvas**: The canvas used for all the UI implementations

## UI Overview

- Pressing 'M' will open the main menu of the game.
![img](main_menu.png)

- An idle player will result in the pause menu popping up and pausing the game until the player moves
![img](pause_menu.png)

- A Game Over menu will pop up when the player loses 3 points to the AI
![img](game_over_menu.png)

## Adjustable Settings
Some settings can be adjusted in the Unity editor

## UML Diagram

Below is an UML Diagram of the Scripts created. There are a total of 7 scritps found in the Assets/Scripts folder
![img](DWPongUnity_UML.png)
