import time

import paho.mqtt.client as mqtt
import numpy as np
import json

from exhibit.shared import utils
from exhibit.shared.config import Config
import cv2
import math

class AISubscriber:
    """
    MQTT compliant game state subscriber.
    Always stores the latest up-to-date combination of game state factors.
    """

    def on_connect(self, client, userdata, flags, rc):
        print("Connected with result code " + str(rc))
        client.subscribe("puck/position")
        client.subscribe("player1/score")
        client.subscribe("player2/score")
        client.subscribe("paddle1/position")
        client.subscribe("paddle2/position")
        client.subscribe("game/level")
        client.subscribe("game/frame")

        # NICK
        client.subscribe("camera/gamestate")

    def convert_rgb(self, x):
        """
        This method was written to convert the game state passed over MQTT from Unity from a single integer
        to a rgb pixel value which is either white (255, 255, 255) representing a game object
        or a blue color (140, 60, 0) representing the empty space
        """
        if int(x) == 0:
            return [140, 60, 0]
        else:
            return [255, 255, 255]

    def on_message(self, client, userdata, msg):
        """
        This method will listen over MQTT for messages passed on the topics below which is the game state published
        from the Unity game engine
        """


        topic = msg.topic
        payload = json.loads(msg.payload)
        # Position Implementation below
        if self.config.UNITY_RECV_POSITIONS:
            if topic == "puck/position":
                self.puck_x = payload["x"]
                self.puck_y = payload["y"]
            if topic == "paddle1/position":
                self.bottom_paddle_x = payload["position"]
            if topic == "paddle2/position":
                self.top_paddle_x = payload["position"]
            if topic == "game/level":
                print(payload)
                self.game_level = payload["level"]
            if topic == "game/frame":
                self.frame = payload["frame"]
                if Config.instance().NETWORK_TIMESTAMPS:
                    print(f'{time.time_ns() // 1_000_000} F{self.frame} RECV GM->AI')
                self.trailing_frame = self.latest_frame
                self.latest_frame = self.render_latest_preprocessed()
        else:
            # Unity sending 0s and 1s array representing gamestate
            if topic == "camera/gamestate":
                self.trailing_frame = self.latest_frame
                gamestate = np.array([self.convert_rgb(int(x)) for x in payload["gamestate"]], dtype=np.float32).reshape(160, 192, 3)
                self.latest_frame = utils.preprocess(np.flipud(gamestate))

            if topic == "game/frame":
                self.frame = payload["frame"]
                if Config.instance().NETWORK_TIMESTAMPS:
                    print(f'{time.time_ns() // 1_000_000} F{self.frame} RECV GM->AI')

            # ai_driver will handle different level than it currently has in publish_inference
            if topic == "game/level":
                self.game_level = payload["level"]



    def draw_rect(self, screen, x, y, w, h, color):
        """
        Utility to draw a rectangle on the screen state ndarray
        :param screen: ndarray representing the screen
        :param x: leftmost x coordinate
        :param y: Topmost y coordinate
        :param w: width (px)
        :param h: height (px)
        :param color: RGB int tuple
        :return:
        """
        # IMPORTANT: See notes in the corresponding method in Pong.py
        # This needs to be set up such that everything is drawn symmetrically
        y = math.ceil(y)
        x = math.ceil(x)
        screen[max(y, 0):y+h, max(x, 0):x+w] = color

    def publish(self, topic, message, qos=0):
        """
        Use the state subscriber to send a message since we have the connection open anyway
        :param topic: MQTT topic
        :param message: payload object, will be JSON stringified
        :return:
        """
        if topic == 'paddle1/frame' and Config.instance().NETWORK_TIMESTAMPS:
            print(f'{time.time_ns() // 1_000_000} F{message["frame"]} SEND AI->GM')

        try:
            p = json.dumps(message)
        except:
            p = message

        self.client.publish(topic, payload=p, qos=qos)

    def render_latest(self, bottom=False):
        """
        Render the current game pixel state by hand in an ndarray
        :return: ndarray of RGB screen pixels
        """
        screen = np.zeros((self.config.HEIGHT, self.config.WIDTH, 3), dtype=np.float32)
        screen[:, :] = (140, 60, 0)  # BGR for a deep blue
        if bottom:
            self.draw_rect(screen, self.bottom_paddle_x - self.config.PADDLE_WIDTH / 2, self.config.BOTTOM_PADDLE_Y - (self.config.PADDLE_HEIGHT / 2),
                  self.config.PADDLE_WIDTH, self.config.PADDLE_HEIGHT, 255)
        else:
            self.draw_rect(screen, self.top_paddle_x - self.config.PADDLE_WIDTH / 2, self.config.TOP_PADDLE_Y - (self.config.PADDLE_HEIGHT / 2),
                     self.config.PADDLE_WIDTH, self.config.PADDLE_HEIGHT, 255)
        self.draw_rect(screen, self.puck_x - self.config.BALL_DIAMETER / 2, self.puck_y - (self.config.BALL_DIAMETER / 2),
                  self.config.BALL_DIAMETER, self.config.BALL_DIAMETER, 255)

        if bottom:  # Flip screen vertically because the model is trained as the top paddle
            screen = np.flip(screen, axis=0)
        #appendix = "_flip" if bottom else ""
        #cv2.imwrite(f"frame{self.frame}{appendix}.png", screen)
        return screen

    def render_latest_preprocessed(self):
        """
        Render the current game pixel state by hand in an ndarray
        Scaled down for AI consumption
        :return: ndarray of RGB screen pixels
        """
        latest = self.render_latest()
        return utils.preprocess(latest)

    def render_latest_diff(self):
        """
        Render the current game pixel state, subtracted from the previous
        Guarantees that adjacent frames are used for the diff
        :return: ndarray of RGB screen pixels
        """
        if self.trailing_frame is None:
            return self.latest_frame
        return self.latest_frame - self.trailing_frame

    def render_latest_add(self):
        """
        Render the current game pixel state, subtracted from the previous
        Guarantees that adjacent frames are used for the add.
        :return: ndarray of RGB screen pixels
        """
        if self.trailing_frame is None:
            return self.latest_frame
        return (self.latest_frame + self.trailing_frame)*-1

    def ready(self):
        """
        Determine if all state attributes have been received since initialization
        :return: Boolean indicating that all state values are populated.
        """
        return self.puck_x is not None \
               and self.puck_y is not None \
               and self.bottom_paddle_x is not None \
               and self.top_paddle_x is not None \
               and self.game_level is not None

    def __init__(self, config, trigger_event=None):
        """
        :param trigger_event: Function to call each time a new state is received
        """
        self.config = config
        self.trigger_event = trigger_event
        self.client = mqtt.Client(client_id="ai_module")
        self.client.on_connect = lambda client, userdata, flags, rc : self.on_connect(client, userdata, flags, rc)
        self.client.on_message = lambda client, userdata, msg : self.on_message(client, userdata, msg)
        self.client.connect_async("localhost", port=1883, keepalive=60)
        self.puck_x = None
        self.puck_y = None
        self.bottom_paddle_x = None
        self.top_paddle_x = None
        self.game_level = None
        self.frame = 0
        self.latest_frame = None
        self.trailing_frame = None

    def start(self):
        self.client.loop_forever()
