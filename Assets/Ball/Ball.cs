using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    // need to:
    // spawn
    // increase speed on hit

    //public PhysicMaterial physicMaterial; // lets us alter properties at run time?
    [Tooltip("The speed at launch")]
    public float launchSpeed = 4.0f;
    [Tooltip("The speed increase on each object interaction")]
    public float speedMultiplier = 0.1f;
    [Tooltip("The maximum velocity (in any direction)")]
    public float maxSpeed = 8.0f;
    
    [Tooltip("The max angle (radians) the ball will launch from vertical axis")]
    public float launchAngleBounds = 30.0f;

    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {

        //physicMaterial = GetComponent<Collider>().material;
        // can use physicMaterial.bounciness = x for various changes in game

        rigidBody = GetComponent<Rigidbody>();

        Launch();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Ball hit - increase speed");
        //Vector3 currentDirection = rigidBody.velocity
        rigidBody.velocity += rigidBody.velocity * speedMultiplier;
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);

        //newVelocity.x = Mathf.Clamp()
        
    }

    public void Launch()
    {

        var launchAngle = Random.Range(-launchAngleBounds, launchAngleBounds);
        Debug.Log(launchAngle);
        var launchVector = Quaternion.AngleAxis(launchAngle, Vector3.up) * Vector3.forward;
        rigidBody.velocity = launchVector * launchSpeed;
    }

    public void ResetPosition(int level)
    {
        Debug.Log("Resetting Position of ball");
        rigidBody.position = new Vector2(0.0f, 0.0f);
        var launchAngle = Random.Range(-launchAngleBounds, launchAngleBounds);
        var launchVector = Quaternion.AngleAxis(launchAngle, Vector3.up) * Vector3.forward;
        rigidBody.velocity = launchVector *  0.0f;

        //Wait for 4 seconds
        StartCoroutine(waiter(level));

          
    

    }

    public IEnumerator waiter(int level)
    {   


    Debug.Log("WAITING");
    //Wait for 4 seconds
    yield return new WaitForSecondsRealtime(1);
    Debug.Log("WAIT DONE");

    Launch();

    // Adjusting speed based on level
    rigidBody.velocity += rigidBody.velocity * (level*speedMultiplier);

  
    }
}
