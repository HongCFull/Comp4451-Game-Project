using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //for player wasd movement
    [SerializeField] float speed = 20f;   
    private CharacterController controller;

    //for camera perspective control
    Camera mainCam;
    private Vector3 moveDirection;
    private float turnspeed = 15;

    //for jump handling
    [SerializeField] float gravity =-9.81f;
    [SerializeField] float jumpHeight = 10;
    [SerializeField] private LayerMask groundMask;
    private Vector3 jumpVelocity ;
    private bool isGrounded = true;
    private float distanceToGround;
    private bool headHitSomething=false;

    void Start(){
        controller = GetComponent<CharacterController>();
       // SetCamera();
    }

    void Update(){
       // JumpHandle();
        Move();
    }

    void SetCamera(){
        mainCam=Camera.main;
        Cursor.visible=false;
        Cursor.lockState=CursorLockMode.Locked;
    }

    void OnControllerColliderHit(ControllerColliderHit other) { //called back when the character controller's "collider" hit something 
        if(Physics.Raycast(transform.position,Vector3.up,GetComponent<CharacterController>().height/2+0.1f))    //note that we only checked the center of the head(instead of the whole head)
            headHitSomething=true;
    }

    RaycastHit temp;       //an object being returned after ray cast
    void JumpHandle(){
        distanceToGround= controller.height/2 +0.1f;
       // isGrounded=Physics.CheckSphere(transform.position,distanceToGround,groundMask);       //version1
       // isGrounded=Physics.Raycast(transform.position,Vector3.down,GetComponent<CharacterController>().height/2+0.1f);    //version2
        Vector3 origin = new Vector3(
            transform.position.x,
            transform.position.y-GetComponent<CharacterController>().height/2+GetComponent<CharacterController>().radius,   // the y value of the center of bottom sphere
            transform.position.z);

        isGrounded=Physics.SphereCast(origin,GetComponent<CharacterController>().radius,Vector3.down,out temp, +0.03f );    //check if going downward by d=0.01 will hit something or not
       
        if(isGrounded && jumpVelocity.y <0){  
            jumpVelocity.y = -0.1f;
            headHitSomething=false; //reset 
        }

        if(isGrounded && Input.GetKeyDown(KeyCode.Space)){
            jumpVelocity.y = Mathf.Sqrt(jumpHeight*-2*gravity);
        }

        //check if head is hitted by something
        if(headHitSomething&& jumpVelocity.y>0){
            Debug.Log("Hitted something");
            jumpVelocity.y=0;   
        }

        // apply gravity
        jumpVelocity.y+=gravity*Time.deltaTime;
        controller.Move(jumpVelocity*Time.deltaTime);
    }


    void Move(){
    //Player wasd movement 
    
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        controller.Move(transform.forward*speed*vertical*Time.deltaTime);
        controller.Move(transform.right*speed*horizontal*Time.deltaTime);

        JumpHandle();
    }
}
