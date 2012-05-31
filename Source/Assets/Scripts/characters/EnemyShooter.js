#pragma strict
// Enemy ControllerColliderHit
// Description: Control component enemy shooter logic, and properties for gumba

enum ShooterState {	Sleeping = 0, Alert, Holded, Stunned, Shooted, Dead }

public var enemyState				: ShooterState = ShooterState.Sleeping ;	// set default starting state

public var aimDamping 				: float 		= 4.0;			// speed & time taken for aiming
public var fireRate					: float		 	= 2.0;			// delay between shots, more lower, more harder to avoid
public var shootPower	 			: float 		= 10;			// the power or speed of the projectile.
public var deathForce				: float 		= 8.0;			// when the player jumps on me force him off 'x' amount

public var gizmoToggle				: boolean		= true;			// toggle the debug display radius
public var bounceHit				: AudioClip;					// hot sound for the enemy splat

private var orientation			 	: int 			= -1;			// orientation of the enemy
private var nextFire				: float 		= 3.0;			// delay between attacks
private var gravity					: float 		= 9.8;			// weight of the world pushing enemy down
private var stunTime				: float 		= 10.0;			// enemy knocked out time...

public 	var alertRange				: float 		= 2.0;			// set the range for finding the player
public 	var attackRange				: float 		= 5.0;			// set range for speed increase
private var distanceToTarget		: float 		= 0.0;			// the dist to target position stored in square mag.

private var AlertRangeX2			: float			= alertRange * alertRange;
private var AttackRangeX2 			: float			= attackRange * attackRange; 

private var Holded					: boolean 		= false;
private var AnimFlag 				: boolean 		= true;
private var grounded 				: boolean		= false;

public 	var HoldedPosition			: Vector3 = Vector3( 0,.3,-.1);	// change own enemy position when grabed
private var moveDirection 			: Vector3 = Vector3( 5, 3,  0);	// Force * direction of throwing the enemy 
private var velocity				: Vector3 = Vector3.zero;		// store the enemy movement in velocity (x, y, z)

public var target					: Transform;					// target to search ( the player )
public var aimCompass				: Transform;					// it's a simple child transform inside this gameObject
//private var thisTransform			: Transform;					// own enemy's tranform cached

public var projectile 				: GameObject;					// Prefab to be shooted ( set previously it's values )
private	var playerLink 				: GameObject;

private var animPlay 				: AnimSprite; 					// : Component
private var linkToPlayerControls 	: PlayerControls;



	
function Start()
{
//	AimCompass = this.gameObject.GetComponentInChildren( Transform ); // not working
//	controller = this.GetComponent ( CharacterController );
	

	if ( rigidbody)								// So, we need a rigidbody somewhere to do sny trigger/collision check, great
	{
    	rigidbody.freezeRotation = true;
    	rigidbody.useGravity = false;
    }
	
	playerLink =  GameObject.Find("Pombero");	//	We can Use this system to get the player's Id & position
	if (playerLink) 
	{
			linkToPlayerControls = playerLink.GetComponent("PlayerControls") as PlayerControls;
			if (! linkToPlayerControls  ) print("big fucking error!");
	
	}else
		 print(" Beware: player link not found!");
			
	animPlay = GetComponent(AnimSprite);
	
	while( true)
		yield CoUpdate();
	
}

function CoUpdate ()	: IEnumerable										// function Update ()

{
	velocity = Vector3.zero; 
		
//	distanceToTarget = Vector3.Distance( target.position, transform.position);	// Check enemy & players distance
	distanceToTarget = (target.position - transform.position).sqrMagnitude;		// it's much faster using Square mag. 

    switch ( enemyState )										// check states of the character:
		{
			case ShooterState.Sleeping:								// GAUCHO SLEEPING:		
				if (!grounded)
					velocity.y -= gravity * Time.deltaTime; 			// just update gravity and keep standby 
				
				Sleeping();							
				break;
						
			case ShooterState.Alert:								// ALERT STATE:
				if (!grounded) 
					velocity.y -= gravity * Time.deltaTime;			
						
				Search();												// Begin Searching of the player & attack him
				break;
				
			case ShooterState.Stunned: 								// KNOCK OUT:
				if (!grounded)
					velocity.y -= gravity * Time.deltaTime ;				
				Stunned();								
				break;			
			
			case ShooterState.Holded: 								// ENEMY IS TAKEN
				BeingHolded();
				break;
				
			case ShooterState.Shooted: 								// ENEMY IS THROWED IN THE AIR
					velocity = moveDirection;
					moveDirection.y -= gravity * Time.deltaTime;
					transform.RotateAroundLocal( Vector3.forward, -orientation * 2 * Time.deltaTime ); 
									
				break;				
				
			case ShooterState.Dead:									// ENEMY IS DEAD
					velocity = moveDirection;
					moveDirection.y -= gravity * Time.deltaTime;
					transform.RotateAroundLocal( Vector3.forward, -orientation * 45 * Time.deltaTime ); 
					Destroy(gameObject, 5);							// Keep falling and die after a while
//				Die();
				break;
		
		}

	transform.position += velocity * Time.deltaTime;
}

function Sleeping()
{
	if( AnimFlag) animPlay.PlayFrames(0 , 0, 2, 4, orientation);

 	if (  distanceToTarget <= AlertRangeX2 )
 	{
 		AnimFlag = false;
 		var timertrigger = Time.time + 0.75f;
		while( timertrigger > Time.time )
		{
			animPlay.PlayFrames(1 , 0, 2, orientation); 
			if ( enemyState != ShooterState.Sleeping) return;
    		
    		yield;
		}
		AnimFlag = true;
 		enemyState = ShooterState.Alert;
		return;
	}
}

function Search()
{	
	// Seek distance and players position/angle
	var lookPos : Quaternion = Quaternion.LookRotation( transform.position - target.position, Vector3.forward );
	orientation = Mathf.Sign( target.position.x - transform.position.x  );
//	print( target.position + " - gaucho: " + transform.position );
	
	lookPos.y = 0;
	lookPos.x = 0;

	aimCompass.rotation = Quaternion.Slerp( aimCompass.rotation, lookPos, Time.deltaTime * aimDamping);
				
    if ( distanceToTarget <= AttackRangeX2 )						// if player's near enemy -> Aim & Attack him
    {	
    	if( AnimFlag) animPlay.PlayFrames(1 , 2, 2, orientation);	
    			
    	if ( Time.time > nextFire )									// wait some time between shots 
    	{
    		AnimFlag = false;
    		Shoot();				
    		
			var timertrigger = Time.time + .5f;
			while( timertrigger > Time.time )						// do some throw animation for a while 
			{
    			animPlay.PlayFrames(2 , 0, 2, orientation); 
				if ( enemyState != ShooterState.Alert){ AnimFlag = true; return; }
    			yield;
			}
    		AnimFlag = true;
    	}
    }
    else
     enemyState = ShooterState.Sleeping;						// else if player it's out enemy range -> go Sleep
}

function Shoot() 
{
    // Add fireRate and current time to nextFire
    nextFire = Time.time + fireRate;

	// Instantiate the projectile
    var clone : GameObject = Instantiate (projectile, aimCompass.position , aimCompass.rotation);
    
    Physics.IgnoreCollision(clone.collider, this.gameObject.collider); 	// it works but the distance it s lame
 // clone.transform.Translate( Vector3( 0, 1, 0) ); 					// avoid hits between shot & shooter own colliders  

    clone.name = "Shot";

	// Add speed to the target
//   if (clone.rigidbody) clone.rigidbody.velocity = aimCompass.TransformDirection (  Vector3.up * shootPower); else
//	clone.GetComponent(BulletShot).Fire( aimCompass.up * shootPower, 5); 	// shot with a speed of 5 & a rotation of 10
	clone.GetComponent(BulletShot).FireAnimated( aimCompass.up * shootPower, 1, 1, 2); 	// shot with a short animation
}

function Stunned()														// "Boleado"
{
	this.tag = "pickup";  												//  change tag to a pickable thing for being holded

	var timertrigger = Time.time + stunTime;
	while( timertrigger > Time.time )									// start knock out time counter
	{
		animPlay.PlayFrames(3 , 0, 2, orientation);						// animate Stunning..
		
//		if ( transform.parent && linkToPlayerControls.isHoldingObj && !collider.enabled ) 
		if ( transform.parent && !collider.enabled ) 
		{																// if Player Grabs him change EnemyState > Handled	
		
			transform.position = transform.parent.position +  HoldedPosition;	// Update own hold position & player's too
   			linkToPlayerControls.properties.GrabPosition  =   HoldedPosition; 	
			enemyState = ShooterState.Holded ;									// & change enemy state to holded..
		}
		
		if ( enemyState != ShooterState.Stunned) return;				// so he is not stunned anymore & must quit here
    	yield;
	}
		
	if ( !transform.parent  )											// but if stun time is over & player !isHolding( this)..
	{
		this.tag = "Untagged";  
    	if ( distanceToTarget > AlertRangeX2 )
			enemyState = ShooterState.Sleeping;							// after a while change EnemyState > Default
		else 
			enemyState = ShooterState.Alert;							// or alert if the player is near
	}
}



function BeingHolded()
{
   orientation = linkToPlayerControls.orientation;						// update own orientation according player direction
   animPlay.PlayFrames(3 , 2, 1, orientation); 
   
   if ( collider.enabled &&  !transform.parent) 						// if collider returned & we haven't parent anymore..
   {
  	grounded = false;
		
//	moveDirection = Vector3( Mathf.Sign(orientation) * 5, 3, 0);
	moveDirection.x  *= Mathf.Sign(orientation);
   	enemyState = ShooterState.Shooted;									// Then it means we have been shooted...
   }
}



function OnTriggerStay () 		//function OnCollisionStay ()
{
    grounded = true;    
}

function OnTriggerEnter( other : Collider)
{																		// other.transform.position == target.position
	if ( other.tag == "Player" && other.transform.position.y > transform.position.y + .1 )
	{
//			Debug.Log(" Enemy: " + transform.position.y );
//			Debug.Log(" Player: " + other.transform.position.y);
			linkToPlayerControls.velocity.y = deathForce; 
	
		if ( this.tag != "pickup"  )
		{
			enemyState = ShooterState.Stunned;
		}
 
//		if (bounceHit)
//		{
//		audio.clip = bounceHit;
//		audio.Play();
//		}
//		
//		var boxCollider = GetComponent( BoxCollider) as BoxCollider;
//		if (boxCollider)
//		{
//			boxCollider.size = Vector3(0,0,0);
//			Destroy ( boxCollider);
//			enemyState = EnemyState.EnemyDie;
//		}
//		else
//		{
//			Debug.Log("Could not load box Collider");
//		}		
	}
	if ( enemyState == ShooterState.Shooted &&  other.tag != "Player"  )
	{
//			moveDirection = Vector3( Mathf.Sign(orientation) * 5, 3, 0)
		moveDirection *= -.5;
		enemyState = ShooterState.Dead;
	}
//	if ( other.tag == "enemy" )
//	{
//		if ( other.collider != this.collider )
//		{
//			Physics.IgnoreCollision(other.collider, this.collider);
//		}
//	}
}


function OnDrawGizmos()		// toggle the gizmos for designer to see 6 Debug reaching ranges
{
	if (gizmoToggle)
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere( transform.position, alertRange);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere( transform.position, attackRange);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(  aimCompass.position , aimCompass.TransformPoint ( Vector3.up * 1.5) );
//		Gizmos.DrawWireSphere( homePosition.position, returnHomeRange);
	}
}

