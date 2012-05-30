#pragma strict

// Player properties Component
// Description: set and stores pickups and state of player

enum PlayerState
{
	Asleep 			= 0,	// Dead stand By 
	Flickering 		= 1,		
	Normal			= 2,		
	Invisible		= 4,
	Small			= 8,
	WildFire		= 16
}

enum Items
{
	Empty 			= 0,	// default  
	Hat 			= 1,		
	Whistler		= 2,		
	Invisibility	= 4,
	Smallibility	= 8,
	Fire			= 16
}

public var playerState 	 : PlayerState 	= PlayerState.Normal;	// set display state of mario in Inspector 
public var Inventory	 : Items		= Items.Empty;			// Inventory system activation
//private var previousState : PlayerState = playerState;	// set display state of mario in Inspector 

var lives					: int	= 3;
//var health				: int	= 3;
var key						: int   = 0;
var fireGauge				: int   = 0;
var hatPower				: int   = 0;
var coins					: int  	= 0;
private var coinLife		: int 	= 20;

var projectileFire			: GameObject;
//var projectileSocketRight	: Transform;
//var projectileSocketLeft	: Transform;

//var materialPlayerStandard	: Material;
//var materialPlayerFire		: Material;

var changeState				: boolean = true;
var hasFire					: boolean = false;

@HideInInspector var dead 	: boolean = false;
private var canShoot		: boolean = false;

public  var soundDie				: AudioClip;
private var soundRate		: float = 0.0;
private var AuxGrav 		: float = 0.0;
private var	soundDelay		: float = 0.0;

private var playerControls  : PlayerControls;
private var charController 	: CharacterController;

function Start()
{
	playerControls = GetComponent ( PlayerControls );
	charController = GetComponent  ( CharacterController );
	AuxGrav = playerControls.gravity;	
	SetPlayerState( PlayerState.Asleep);
	
	while (true)
		yield CoUpdate();
}


function CoUpdate() : IEnumerator
{
	PlayerLives();

	UpdatePlayerState();
	
   	if ( Input.GetButtonUp("Fire1")) HoldingKey = false;
	
}

function UpdatePlayerState()
{
	dead = ((playerState == 0) || ( playerState == PlayerState.WildFire ));

	
	switch (playerState)
	{
		case PlayerState.Asleep:
			if ( changeState )
			{
				renderer.enabled = true;
				renderer.material.color = Color.white;
				playerControls.enabled = false;
				changeState = false;
				yield Sleeping();
			}
			playerControls.animPlay.PlayFrames ( 4, 3, 1, playerControls.orientation);
			
			if ( Input.GetButtonUp("Fire1") || Input.GetButtonUp("Jump") )
			{
				playerControls.enabled = true;
				SetPlayerState( PlayerState.Flickering );	
			}
			break;
			
		case PlayerState.WildFire:
			playerState |= PlayerState.Normal;
			yield Burning();
			break;
			
		default:
			playerState |= PlayerState.Normal; // This its needed in case of direct switch from Unity GUI drag & drop
			
			if( (playerState  & PlayerState.Flickering) == PlayerState.Flickering )
				if ( changeState )	{ changeState = false; 	yield Flickering();		}
			
		
			if( (playerState  & PlayerState.Invisible) == PlayerState.Invisible )
				if ( changeState )	{ changeState = false; yield Invisible(); }
						
//			if( (playerState  & PlayerState.Small) == PlayerState.Small )
//			{
//				if ( changeState )	{ changeState = false; yield Small(); }
//			}


			
			if ( Input.GetButton("Fire1") && !HoldingKey )		PlayerThrows();
			else 
				if ( !_pickedObject && Input.GetButtonDown("Fire2")	)	UseInventory();

		if ( Input.GetButtonDown("Fire2")) ThrowHat();
	}

}

function UseInventory()
{
	// Use Inventory
	
	switch ( Inventory )
	{
		case Items.Empty: 
			break;
			
		case Items.Hat: 			// Throw Hat..
		if( (Inventory  & Items.Hat) == Items.Hat )
			Inventory &= (~Items.Hat);
			break;
						
		case Items.Whistler:		// Wisp Whistler
			Inventory &= (~Items.Whistler);
			break;
			
		case Items.Invisibility:	// Turn Invisible 
			AddPlayerState( PlayerState.Invisible );
			Inventory &= (~Items.Invisibility);
			break;
			
		case Items.Smallibility: 	// Get Smaller
			Inventory &= (~Items.Smallibility);
			break;			
			
		case Items.Fire: 			// Do Fire things..
				if ( fireGauge == 1 ) ; 	// instantiate flame
			else 
				if ( fireGauge  < 3 ) ;	// instantiate fireball
			else
				SetPlayerState( PlayerState.WildFire );
							  
			fireGauge = 0;
			Inventory &= (~Items.Fire);
			break;
	}
}

function ThrowHat()
{
	// Instantiate the projectile
    var clone : GameObject = Instantiate (projectileFire, transform.position , transform.rotation);
    
    Physics.IgnoreCollision(clone.collider, this.gameObject.collider); 	// it works but the distance it s lame
 // clone.transform.Translate( Vector3( 0, 1, 0) ); 					// avoid hits between shot & shooter own colliders  

    clone.name = "Hat";

	// Add speed to the target
	clone.GetComponent(BulletShot).FireAnimated( Vector3 ( playerControls.orientation * 1, 0, 0) , 5, 0, 4); 	// shot with a short animation
}


function Sleeping()		: IEnumerator
{
	while ( !charController.isGrounded )
	{
		charController.Move( Vector3( 0, -1.0, 0 ) * Time.deltaTime );
		
		if ( playerState ) return; 
		
		yield;	
	}
}

function Flickering()	: IEnumerator
{
	var timertrigger = Time.time + 5;
	
	while( timertrigger > Time.time )
	{
		if(Time.frameCount % 4 == 0)
			renderer.enabled = !renderer.enabled;

//		if ((Mathf.FloorToInt(Time.time) % 2) == 0)
//			renderer.material.color = Color.red;
//		else 
//			renderer.material.color = Color.white;

		if ( dead ) return;
		
		yield ;
	}
	
		renderer.enabled = true;
//		playerState &= ( PlayerState.Normal | PlayerState.Invisible);
		playerState &= (~PlayerState.Flickering);
}

function Small()		: IEnumerator
{
	SetPlayerState( PlayerState.Normal);

}

function Invisible()	: IEnumerator
{
	var timertrigger = Time.time + 20;
			 
	while( timertrigger > Time.time )
	{
//		if ( playerState == PlayerState.Invisible )
//		{
 			var lerp : float = Mathf.PingPong (Time.time * .45f, 1.0) ;
// 			renderer.material.color.a = Mathf.Lerp ( .0f, .5f,  lerp);
 			renderer.material.SetFloat( "_Cutoff", Mathf.Lerp ( -0.15f, .7f,  lerp));
 			
// 		}
// 		else 
// 		{
// 			renderer.material.color = Color.white;
//		}

		if ( dead ) return;
			
		yield ;			 
	}

 		renderer.material.SetFloat( "_Cutoff", 0.9);

// 		renderer.material.color = Color.white;
//		playerState &= ( PlayerState.Normal | PlayerState.Flickering);
		playerState &= (~PlayerState.Invisible);

}

function Burning()		: IEnumerator
{
//	renderer.material = materialFire;
	renderer.material.color = Color.white;
	renderer.enabled = true;
	var timertrigger = Time.time + 20;
	
	while( timertrigger > Time.time )
	{

// 		var lerp : float = Mathf.PingPong (Time.time, 1.0) / 1.0;
//    	renderer.material.color = Color.Lerp (Color.red, Color.yellow, lerp);
		if(Time.frameCount % 2 == 0)
			renderer.material.color = Color.red;
		else 
			renderer.material.color = Color.yellow;
						
//		if ( playerState != PlayerState.WildFire ) return; 
		if ( !playerState  ) return; 

		
		yield;
	}
	
	renderer.material.color = Color.white;
//	playerState &= PlayerState.Normal ;
	playerState &= (~PlayerState.WildFire);
}










function SetPlayerState( newState : PlayerState )
{
	playerState = newState;
	changeState = true;
}

function AddPlayerState( newState : PlayerState )
{
	playerState |= newState;
	changeState = true;
}
	
function PlaySound ( soundName : AudioClip, soundDelay : float)
{
	if ( !audio.isPlaying && Time.time > soundRate )
	{
		soundRate = Time.time + soundDelay;
		audio.clip = soundName;
		audio.Play();
		yield WaitForSeconds ( audio.clip.length );
	}
}
function PlayerLives()
{
	if ( lives == 0 )
	{
//		PlaySound ( soundDie, 0 );
		yield WaitForSeconds ( 3 );
		print("Player is Dead");
//		Application.LoadLevel ( "2D Mario Screen Lose" );
	}
}

function AddKeys (  numKey : int)
{
	key += numKey;

}

function AddCoin(numCoin : int)
{
	coins = coins + numCoin;
}

public var GrabPosition   		: Vector3 	= Vector3( 0f, 0.5f, 0f);				
public var ThrowForce   		: float		= 400f;	//How strong the throw is. This assumes the picked object has a rigidbody component attached
public var AlphaAmount  		: float 	= 0.5f; //this will be the alpha amount. It's public so you can change it in the editor

@HideInInspector
		var _pickedObject 		: Transform;
private var _originalColor 		: Color;
//private var HoldingObj			: boolean 	= false;
private var HoldingKey			: boolean 	= false;

//function OnControllerColliderHit (hit : ControllerColliderHit)
function OnTriggerStay( hit : Collider)
{
 	if ( playerState < 8 )

       if ( !_pickedObject && hit.transform.tag == "pickup" && Input.GetButtonDown( "Fire1") )
       {
         HoldingKey = true;
             	
         //caches the picked object
         _pickedObject = hit.transform;
         
         ThrowForce = _pickedObject.rigidbody.mass * 5;
         
         _pickedObject.rigidbody.isKinematic = true;
         
         _pickedObject.collider.enabled = false;
                  
         //caches the picked object's color for resetting later
//         _originalColor = hit.transform.renderer.material.color;
         _originalColor = _pickedObject.renderer.material.color;

         //this will snap the picked object to the "hands" of the player
         _pickedObject.position = transform.position + GrabPosition; 
//		_pickedObject.position = HoldPosition.position ;

         //this will set the HoldPosition as the parent of the pickup so it will stay there
         _pickedObject.parent = this.transform; 
//         _pickedObject.parent = HoldPosition; // = this.transform;

         //this will change the alpha amount on the object's color to make it half transparent
         _pickedObject.renderer.material.color = new Color( _pickedObject.renderer.material.color.r,
                             								_pickedObject.renderer.material.color.g,
                            								_pickedObject.renderer.material.color.b,
                                        					 AlphaAmount);
       }

}

function PlayerThrows()												// Object Throwing without physics engine
{
    	if ( _pickedObject )
         {
	      var orientation = playerControls.orientation;
		
          _pickedObject.parent = null;        		//resets the pickup's parent to null so it won't keep following the player

          _pickedObject.renderer.material.color = _originalColor; 	//resets pickup's color so it won't stay half transparent 
          
          _pickedObject.collider.enabled = true;
                    
          _pickedObject.collider.isTrigger = true;
                    
          //applies force to the rigidbody to create a throw
//          _pickedObject.rigidbody.AddForce( Vector3( orientation, 1,0) * ThrowForce, ForceMode.Impulse);
//          _pickedObject.rigidbody.AddForce( Vector3( orientation, Input.GetAxis( "Vertical"),0) * ThrowForce, ForceMode.Impulse);
//          _pickedObject.position += Vector3( orientation, Input.GetAxis( "Vertical"),0) * 1.5;
  
          //resets the _pickedObject 
          _pickedObject = null;
         }
 }

//function PlayerThrows()
//{
//    	if ( _pickedObject )
//         {
//	      var orientation = playerControls.orientation;
//		
//          _pickedObject.parent = null;        		//resets the pickup's parent to null so it won't keep following the player
//
//          _pickedObject.renderer.material.color = _originalColor; 	//resets pickup's color so it won't stay half transparent 
//          
//          _pickedObject.rigidbody.isKinematic = false;				// check this if you re using physics Unity system
//          
//          _pickedObject.collider.enabled = true;
//                    
//          _pickedObject.collider.isTrigger = true;
//                    
//          //applies force to the rigidbody to create a throw
////          _pickedObject.rigidbody.AddForce( Vector3( orientation, 1,0) * ThrowForce, ForceMode.Impulse);
//          _pickedObject.rigidbody.AddForce( Vector3( orientation, Input.GetAxis( "Vertical"),0) * ThrowForce, ForceMode.Impulse);
//          
//          _pickedObject.rigidbody.useGravity = true;				// check this if you re using physics Unity system
//  
//          //resets the _pickedObject 
//          _pickedObject = null;
//         }
// }

@script AddComponentMenu( "Utility/Player Propierties Script" )

