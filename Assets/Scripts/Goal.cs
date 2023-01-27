using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int playerGoal;
    private void OnCollisionEnter(Collision collision)
    {
        // If the ball touches the opponents barrier, then one of the players have scored
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (playerGoal == 1)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().PlayerOneScore();
            }

            if (playerGoal == 2)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().PlayerTwoScore();
            }
        }
    }
}
