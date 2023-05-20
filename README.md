# DWPongUnity
The goal of this project is to reimplementate the game engine component of DWPongDiscoveryWorld inside the Unity game engine instead. This will allow for better maintainability as well as more scalability to add additional features to the game as Unity is a much more popular engine for game development.

## Exhibit Setup

- Unity 2021.3.10f1 was used to develop the unity game.
- Python 3.8.10 was used to run the AI and depth camera modules
- mosquitto-2.0.15-install-windows-x64.exe was the installation for MQTT
- Intel Realsense v2.53.1 was the driver installed for the project.

### Install python libraries
Note: pip install pyrealsense2 individually may work. Currently used Windows 10, python 3.8.10, pip version 21.1.1 and was able to install the requirements.
```bash
$ cd DWPongUnity\StandaloneAI
$ pip install -r requirements.txt
```
### Install Depth Camera
- [Intel Realsense D435 depth camera Getting Started](https://www.intelrealsense.com/get-started-depth-camera/)
- Exact Version used: [Intel Realsense D435 depth camera](https://github.com/IntelRealSense/librealsense/releases/tag/v2.53.1)

### Install Mosquitto
- Used mosquitto-2.0.15-install-windows-x64.exe for project: [Mosquitto](https://mosquitto.org/download/)

### Start AI
```bash
$ cd DWPongUnity\StandaloneAI\exhibit
$ python ai\ai_driver.py
```

### Start Depth Camera
```bash
$ cd DWPongUnity\StandaloneAI\exhibit
$ python motion\motion_driver.py
```

### Start Unity Game
```bash
$ cd DWPongUnity\Builds
$ DWPong.exe
```

![exhibit demo](docs/exhibit_demo.gif)

## Documentation
* [MQTT Overview](docs/mqtt_overview.md)
* [Unity Overview](docs/unity_overview.md)
  * [Unity Details](docs/unity_details.md) 
* [AI & Depth Camera Overview](docs/ai_overview.md)
