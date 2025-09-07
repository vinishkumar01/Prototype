using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class PlayerScript : MonoBehaviour
{
    Vector2 velocity;
    
    Controller2D controller;
    [SerializeField]float MoveSpeed = 5f;
    [SerializeField] float jumpVelocity;
    [SerializeField] float gravity;
    [SerializeField] float JumpHeight = 4f;
    [SerializeField] float timetoJumpApex = .4f;
    float VelocityXSmoothing; 
    [SerializeField]float accelerationtimeAirborne = .2f;
    [SerializeField]float accelerationtimeGrounded = .1f;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * JumpHeight) / Mathf.Pow(timetoJumpApex,2);
        jumpVelocity = Mathf.Abs(gravity) * timetoJumpApex;
        Debug.Log("Gravity: " + gravity + " jumpVelocity: " + jumpVelocity);
    }

    void Update()
    {
        //Gravity increases even when the object is collided and rested on any platform we are going to make the objects gravity be zero or still by checking a condition wether its not colliding 
       if(controller.collisionsInfo.above || controller.collisionsInfo.below)
       {
            velocity.y = 0;
       }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(Input.GetKeyDown(KeyCode.Space) && controller.collisionsInfo.below)
        {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * MoveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX,ref VelocityXSmoothing,(controller.collisionsInfo.below)? accelerationtimeGrounded : accelerationtimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
