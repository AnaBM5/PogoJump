using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float baseJumpForceX = 5f;         //base values to apply the jump multipliers to
    [SerializeField] private float baseJumpForceY = 5f;
    
    private GameObject topSection;                  //reference to the top section of the player object
    private Gyroscope gyroInput;                    //reference to the gyroscope

    private float startingOrientationZ;             //reference to the starting z orientation of the screen
    private float jumpMultiplier = 0f;              //multiplier value determined by how long the user presses the screen
    
    public float gyroOffset;                        //value that gets added/substracted from the rotation obtained
                                                    //from the gyroscope
                                                    
    private Transform _transform;                   //components of the player references here to avoid making a 
    private Rigidbody2D _rigidbody;                 //call for them on each update
    private Collider2D _playerCollider;

    private bool gameStarted;                       //bool that determines if the input is being registered or not


    [SerializeField] private DebugInfo debugScript; //reference used for script only active in debug mode
    
    // Start is called before the first frame update
    void Start()
    {
        //Gets references to the necessary components
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _playerCollider = gameObject.GetComponent<Collider2D>();
        topSection = transform.GetChild(0).gameObject;

        //activates gyro input
        gyroInput = Input.gyro;
        gyroInput.enabled = true;
        
    }
    void FixedUpdate()
    {
        if (!gameStarted && IsGrounded())   //starts gyro input after the player has landed
            gameStarted = true;             //on the first platform
        
        else if (gameStarted)               //after the game starts checks for gyro and
        {                                   //jump input
            GyroscopeInput();
            JumpInput();
        }

        debugScript.bodyVelocity.text ="Body Velocity: " + _rigidbody.velocity;
        debugScript.angularVelocity.text = "Angular Velocity: " +_rigidbody.angularVelocity;
    }


    void GyroscopeInput()
    {
        //gives the player object anew rotation based on the gyro input in z
        Quaternion targetRotation = Quaternion.Euler(0, 0, gyroOffset);
        targetRotation.z += gyroInput.attitude.z - startingOrientationZ ;
        transform.rotation = targetRotation;
        
    }
    void JumpInput()
    {
        
        if(Input.GetMouseButton(0) && IsGrounded()) //if the user is pressing down the screen
        {                                           //and the player is grounded
           
            if (jumpMultiplier <= 1.5f)             //as long as the jumpMultiplier hasn't exceeded the limit
            {
                jumpMultiplier+=0.04f;              //increases the jumpMultiplier value
                float topSectionHeight = topSection.transform.localPosition.y-0.02f;    //and moves down the top part of the player object
                topSection.transform.localPosition = new Vector3(0, topSectionHeight, 0);
            }
            
            
        }
        else if (Input.GetMouseButtonUp(0) && IsGrounded()) //when the user stops touching the screen
        {
            float jumpForce = baseJumpForceY * jumpMultiplier;      //determines the y force that will be applied to the jump
            float jumpAngle = GetJumpAngle();                       //Calls method to determine x force of the jump        
            
            _rigidbody.velocity = Vector2.zero;                     //resets velocities so it doesn't affect the physic of the jump
            _rigidbody.angularVelocity = 0f;

            //_rigidbody.velocity = new Vector2(baseJumpForceX * jumpAngle, jumpForce);
            _rigidbody.AddForce(new Vector2(baseJumpForceX * jumpAngle, jumpForce)); //applies force to player object
            debugScript.forceApplied.text = "Force Appied: " + baseJumpForceX * jumpAngle + ", " + jumpForce;
            topSection.transform.localPosition = new Vector3(0, 3.34f, 0);          //moves top part to its original position
            jumpMultiplier = 0f;                                                         //resets jumpMultiplier
        }
        
    }
    public float GetJumpAngle()     //Method to determine how much x force is going to be applied on the jump
    {                               //based on its z rotation
        
        float jumpAngle = gameObject.transform.localEulerAngles.z;      //gets the current rotation of the gameObject
        int jumpDirection = -1;
        
        if (jumpAngle >= 180)                           //Unity measures angles counterclockwise from 0 to 360,
        {                                               //this section converts any angle over 180 as if it
            jumpAngle -= 180;                           // was measured clockwise from 0 to 180
            jumpAngle = 180 - jumpAngle;
            jumpDirection = 1;
        }
        
        debugScript.calculatedRotation.text = "Calculated Rotation: "+ jumpAngle* jumpDirection;
        
        jumpAngle = Mathf.Sin(jumpAngle * Mathf.Deg2Rad);           //Gets the sin of the angle
        
        if (jumpAngle < 0.1f && jumpAngle> 0f) jumpAngle = 0.1f;    //avoids values that are too small
        
        debugScript.bodyRotation.text = "xForce: "+ (jumpAngle* jumpDirection);
        return jumpAngle * jumpDirection;                           //returns the value times the jump direction
    }

    private void KillPlayer()
    {
        Destroy(this.gameObject);
    }

   

    public bool IsGrounded()            //Determines if the player object is grounded
    {
        return _playerCollider.IsTouchingLayers(LayerMask.GetMask("Ground")); //based on whether it is colliding
                                                                                            //with a "ground" object
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("DeathZone"))      
        {                                   //kills the player when it enters the death zone
            KillPlayer();
        }
    }
    

    public void StartPlayer()
    {
        _rigidbody.simulated = true;                    //starts physics simulation
        startingOrientationZ = gyroInput.attitude.z;    //sets the current gyro orientation as the starting one
                                                        //for reference when the player tilts the phone
    }
}
