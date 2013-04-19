
// This script moves the character controller forward
// Make sure to attach a character controller to the same game object.

using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

private Transform thisTransform;				// own player tranform cached
private CharacterController controller;
[HideInInspector] public PlayerProperties properties;
[HideInInspector] public AnimSprite animPlay; 				// : Component

[HideInInspector] public int orientation = 1;					// Move Direction: -1 == left, +1 == right
[HideInInspector] public Vector3 velocity = Vector3.zero;	// Start quiet 



public float gravity = 20.0f;
public float fallSpeed = 0.5f;				// speed of falling down ( division factor )

public float walkSpeed = 1.0f;				// standard walk speed
public float runSpeed = 2.0f; 				// running speed 

public float walkJump = 8.0f;				// jump height from walk
public float runJump = 9.0f;				// jump height from run	
public float Depth = .25f;				    // Depth in position.z for the player's character

public AudioClip JumpSound ;

private bool  jumpEnable = false;			// toggle for default jump
private bool  runJumpEnable = false;		// toggle for run jump

private float afterHitForceDown = 1.0f;		// toggle for crouch jump
private int isHoldingObj 	= 0;			// anim row index change when player holds something

private int layerMask = 1 << 8;


void Start()						//	BallScript ballScript = target.GetComponent<BallScript>() as BallScript;
{
	thisTransform = transform;
    controller	= 	GetComponent< CharacterController >();
    properties = GetComponent<PlayerProperties>();
    animPlay = GetComponent<AnimSprite>();
	animPlay.PlayFrames(2 , 0, 1, orientation);
	
	Physics.IgnoreCollision(controller.collider,  transform.GetComponentInChildren<BoxCollider>());
//	if(controller.collider != gameObject.collider) print("CHan!");
	
//	transform.GetComponentInChildren<BoxCollider>().transform.position = controller.transform.position - Vector3(0, -.22,0);
	
}

void Update(){
//void  FixedUpdate (){
   isHoldingObj = System.Convert.ToByte(properties._pickedObject != null);
   bool  Stand = Physics.Linecast (thisTransform.position, thisTransform.TransformPoint ( -Vector3.up), layerMask) ;
   
   if ( controller.isGrounded )
	{
		jumpEnable 		= false;
		runJumpEnable 	= false;

		velocity = new Vector3 ( Input.GetAxis( "Horizontal"), 0, 0 );
	
		if (Input.GetAxis( "Horizontal") == 0 )									// IDLE -> keep stand quiet
			 animPlay.PlayFramesFixed (2 + isHoldingObj, 0, 1, orientation );	
	
		if ( Input.GetAxis(	"Horizontal") != 0)									// WALK
		{
            orientation = (int)(Mathf.Sign(velocity.x)); 		// (int)Math.Ceiling()	// If move direction changes -> flip sprite
			
			velocity.x *= walkSpeed;										// If player is moving ->  Animate Walking..
            animPlay.PlayFramesFixed((0 + isHoldingObj), 0, 8, orientation);
		}	
		
		if ( (velocity.x != 0) &&  Input.GetButton( "Fire1") )						// RUN 
		{
			velocity *= runSpeed;
			animPlay.PlayFramesFixed( 2, 1, 2, orientation , 1.005f) ;
		}

//   Debug.DrawLine ( thisTransform.position  , thisTransform.TransformPoint ( -Vector3.up   ), Color.blue);
//   if(Stand)
//   {																		// JUMP
		if ( Input.GetButtonDown ( "Jump" ) && ( !Input.GetButton( "Fire1")	||  velocity.x == 0 ) )	// Quiet jump
		{											// check player dont make a Running Jump being quiet in the same spot
			velocity.y = walkJump;
			//Instantiate ( particleJump, particlePlacement, transform.rotation );
            Managers.Audio.Play(JumpSound, thisTransform, 1.0f, 1.0f);
			jumpEnable = true;
		}
		
		if ( Input.GetButtonDown( "Jump" ) && ((Input.GetButton( "Fire1") && velocity.x != 0) || properties.BurnOut))// running jump
		{
			velocity.y = runJump;
			//Instantiate ( particleJump, particlePlacement, transform.rotation );
            Managers.Audio.Play(JumpSound, thisTransform, 1.0f, 0.75f);
			runJumpEnable = true;
		}
	}
	
	if ( Input.GetButtonDown ( "Jump" ) && Stand )	// slope jump
		{	
			velocity.y = walkJump;
			Managers.Audio.Play(JumpSound, thisTransform, 1.0f, 1.25f);
			jumpEnable = true;
		}
  	
	if ( !controller.isGrounded && !Stand)	// && !Stand	// Do Slide
	{
		velocity.x = Input.GetAxis( "Horizontal");
		//animPlay.PlayFrames ( 2, 5, 1, orientation );
		
		if ( Input.GetButtonUp ( "Jump" ) )						// check if the player keep pressing jump button..
			velocity.y *= fallSpeed ;							// if not then brake the jump
				
		if ( velocity.x != 0 )
			orientation = (int)Mathf.Sign(velocity.x); 				// If move direction changes -> update & flip sprite
		
		
		if ( jumpEnable )
		{
			velocity.x *= walkSpeed;	 						// If player is jumping -> Update & Animate jumping type.
			animPlay.PlayFrames ( 2 + isHoldingObj, 4, 1, orientation );
		}
		
		if ( runJumpEnable )									
		{
			velocity.x *= runSpeed;
			animPlay.PlayFrames ( 2 + isHoldingObj, 4, 1, orientation );
		}
		
		if ( velocity.y < 0  && !Stand ) 						// check when player stops elevation & becomes down..
		{
			animPlay.PlayFrames ( 2 + isHoldingObj, 5, 1, orientation );
		
			if ( Input.GetButton ( "Jump" )  && isHoldingObj == 0 ) // check if the player keep pressing jump button..
			{
			  velocity.y += 18 * Time.deltaTime;
			  animPlay.PlayFrames ( 2, 6, 2, orientation );
			}
		}
	}

	if ( controller.collisionFlags == CollisionFlags.Above )
	{
		velocity.y = 0;											// set velocity on Y to 0, stop upward motion
		velocity.y -= afterHitForceDown;						// apply force downward so player doesn't have in the air
	}


    if (properties.BurnOut)
        BurnOut();

    if (Managers.Dialog.IsInConversation())
        velocity.x = 0;

	velocity.y -= gravity * Time.deltaTime ;
	controller.Move( velocity * Time.deltaTime );


    var Pos = thisTransform.position;
    Pos.z = Depth;
    thisTransform.position = Pos;
    thisTransform.rotation = Quaternion.Euler(0, 0, 0);
    //thisTransform.rotation.y = 0;
    //thisTransform.rotation.x = 0;
    // take Character Controller Skin Width = 0.05f else 0.001f
}

void  BurnOut (){
	velocity.x =  orientation * runSpeed * 2;
	animPlay.PlayFramesFixed ( 5, 0, 4, orientation , 1.005f) ;
}



//function OnDrawGizmos()	
//{
//	Gizmos.DrawLine(  transform.position , transform.TransformPoint ( -Vector3.up ));	 
//
//}


// This Version of the Script allows to run without any buttons
//  just do a classic double tap & start running

//private float lastTime = -1.0f;
//private bool  running = false;

//function UpdateTap()
//{
//   
//   //HoldingObj = System.Convert.ToByte(flipped < 0)	
//   gravity = 20.0f;
//   
//   if ( controller.isGrounded )
//	{
//		jumpEnable 		= false;
//		runJumpEnable 	= false;
//		
//		velocity = Vector3 ( Input.GetAxis( "Horizontal"), 0, 0 );
//		
//		if ( !Input.GetAxis( "Horizontal") )								// IDLE -> keep stand quiet
//		{
//			 running = false;
//			 animPlay.PlayFrames (2 + isHoldingObj, 0, 1, orientation );	
//		}									
// 		
//		if ( Input.GetAxis( "Horizontal") ) 								// MOVE
//		{
//			orientation = Mathf.Sign(velocity.x); 				// If move direction changes -> update & flip sprite
//			
//			velocity.x *= walkSpeed;
//			animPlay.PlayFrames ( 0 + isHoldingObj, 0, 8, orientation );	
//
//			
//			if (running)
// 			{					
// 				velocity *= runSpeed;
//				animPlay.PlayFrames ( 2, 1, 2, orientation );
// 			}		
//			
//			if( Input.GetKeyDown( "left") || Input.GetKeyDown( "right") )		
//			{
//				running = (Time.time - lastTime < 0.2f);						// RUN
//				lastTime = Time.time;
// 			}
//		}				
//
//																			// JUMP
//		if ( Input.GetButtonDown( "Jump" ) && (!running ) )				// check player dont make a Running Jump,
// 				//  being quiet in the same spot
//		{
//			velocity.y = walkJump;
////			Instantiate ( particleJump, particlePlacement, transform.rotation );
////			PlaySound ( soundJump, 0);
//			jumpEnable = true;
//		}
//		
//		if ( Input.GetButtonDown( "Jump" ) && running && velocity.x ) // running jump
//		{
//			velocity.y = runJump;
////			Instantiate ( particleJump, particlePlacement, transform.rotation );
////			PlaySound ( soundJump, 0);
//			runJumpEnable = true;
//		}
//	}
//	
//	if ( !controller.isGrounded )
//	{
//		velocity.x = Input.GetAxis( "Horizontal");
////		animPlay.PlayFrames ( 2, 3, 1, orientation );
//		
//		if ( Input.GetButtonUp ( "Jump" ) )						// check if the player keep pressing jump button..
//		{
//			velocity.y *= fallSpeed ;							// if not then brake the jump
//		}
//		
//		if ( velocity.x )
//		{
//			orientation = Mathf.Sign(velocity.x); 				// If move direction changes -> update & flip sprite
//		}
//		
//		if ( jumpEnable )
//		{
//			velocity.x *= walkSpeed;	 						// If player is moving -> Update & Animate Walking.
//			animPlay.PlayFrames ( 2 + isHoldingObj, 3, 2, 2, orientation );
//
//		}
//		
//		if ( runJumpEnable )
//		{
//			velocity.x *= runSpeed;
//			animPlay.PlayFrames ( 2 + isHoldingObj, 4, 1, orientation );
//
//		}
//		
//		if ( velocity.y < 0)
//		{
//			animPlay.PlayFrames ( 2 + isHoldingObj, 5, 1, orientation );
//		
//			if ( Input.GetButton ( "Jump" ) )						// check if the player keep pressing jump button..
//			{
//		  	  gravity = 1.0f ;							// if not then brake the gravity
//			  animPlay.PlayFrames ( 2 + isHoldingObj, 6, 2, orientation );
//			}
//		}
//	}
//	
////	if ( controller.collisionFlags == CollisionFlags.Above )
////	{
////		velocity.y = 0;									// set velocity on Y to 0, stop upward motion
////		velocity.y -= afterHitForceDown;				// apply force downward so player doesnŽt have in the air
////	}
//
//	velocity.y -= gravity * Time.deltaTime;
//	controller.Move( velocity * Time.deltaTime );
//	
//	
//	if (thisTransform.position.y < -5 )	thisTransform.position = Vector3( 0.5f, 10, 0.25f );	// If character falls get it up again 
//}




}