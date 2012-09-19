#pragma strict

public var enemyState				: ShooterState = ShooterState.Alert;	// set default starting state

public var aimDamping 				: float 		= 4.0;					// speed & time taken for aiming

public var bounceHit				: AudioClip;							// hot sound for the enemy splat
public var deathForce				: float 		= 8.0;			// when the player jumps on me force him off 'x' amount

private var orientation			 	: int 			= -1;					// orientation of the enemy
private var gravity					: float 		= 9.8;					// weight of the world pushing enemy down

private var Holded					: boolean 		= false;
private var AnimFlag 				: boolean 		= true;

public 	var HoldedPosition			: Vector3 		= Vector3( 0,.3,-.1);	// change own enemy position when grabed
public  var moveDirection 			: Vector3 		= Vector3( 5, 3,  0);	// Force * direction of throwing the enemy 
private var velocity				: Vector3 		= Vector3.zero;			// store the enemy movement in velocity (x, y, z)

public var target					: Transform;							// target to search ( the player )
private var thisTransform			: Transform;							// own enemy's tranform cached

private var animPlay 				: AnimSprite; 							// : Component
private var linkToPlayerControls 	: PlayerControls;
private var grounded 				: boolean		= true;

var soundCrash 						: AudioClip;
var soundAttack						: AudioClip;
var ParticleStars					: GameObject;


function Start ()
{
	thisTransform = transform;
    grounded = true;	
    
	if ( rigidbody)												// we need a rigidbody somewhere to do sny trigger/collision
	{
    	rigidbody.freezeRotation = true;
    	rigidbody.useGravity = false;
    }
    	
	if (!target) 
		target =  GameObject.Find("Pombero").transform;			//	We can Use this system to get the player's Id & position
	
	if ( target) 
	{
			linkToPlayerControls = target.GetComponent("PlayerControls") as PlayerControls;
	}else 	print(" Beware Dengue target empty: player link not found!");
			
	animPlay = GetComponent(AnimSprite);
	
	while( true)
		yield CoUpdate();
}

function OnBecameVisible () {
    enabled = true;
}

function OnBecameInvisible () {
    enabled = false;
}

function CoUpdate () 	: IEnumerable	
{
	if ( thisTransform.IsChildOf( target) ) 									// check if the player has taken us... 
	{																	
		thisTransform.position = target.position +  HoldedPosition; 			// Update own hold position & player's too
   		enemyState = ShooterState.Holded ;										// & change enemy state to holded..
	}
	
    switch ( enemyState )														// check states of the character:
	{
		case ShooterState.Alert:		// 1# ALERT STATE:
				animPlay.PlayFramesFixed(3 , 0, 4, orientation);
				velocity = Vector3.zero;
				velocity.x = Mathf.Sin(Time.time);		
				orientation = Mathf.Sign( Mathf.Sin(Time.time) );	
			break;
		
		case ShooterState.Holded: 		// 3# ENEMY IS TAKEN	
				BeingHolded();
			break;
				
		case ShooterState.Shooted: 		// 4# ENEMY IS THROWED IN THE AIR
				thisTransform.RotateAroundLocal( Vector3.forward, -orientation * 2 * Time.deltaTime ); 
									
			break;				
				
		case ShooterState.Dead:			// 5# ENEMY IS DEAD
				animPlay.PlayFramesFixed(0 , 3, 1, orientation);
				thisTransform.RotateAroundLocal( Vector3.forward, -orientation * 45 * Time.deltaTime ); 
				grounded = false;
			break;		
	}
	
	if (!grounded)	velocity.y -= gravity * Time.deltaTime ;	
	thisTransform.position += velocity * Time.deltaTime;
	
	if (thisTransform.position.y < 0 )	Destroy(gameObject, 2);	// If character falls get it up again 
	
}


function BeingHolded()
{
   grounded = true;		
   velocity = Vector3.zero;
   orientation = linkToPlayerControls.orientation;						// update own orientation according player direction
   animPlay.PlayFramesFixed( 0, 3, 1, orientation); 
   
   if ( collider.enabled &&  !thisTransform.parent ) 					// if collider returned & we haven't parent anymore..
   {
    grounded = false;
	
	velocity = moveDirection;
	velocity.y += 3.5 * Input.GetAxis("Vertical");				
	velocity.x *= Mathf.Sign(orientation);
   	enemyState = ShooterState.Shooted;												// Then it means we have been shooted...
   	this.tag = "p_shot";
   }
}





function OnTriggerEnter( other : Collider)											// other.transform.position == target.position
{																		
	if ( other.CompareTag( "Player") )
	{
		if ( gameObject.CompareTag( "Enemy") && (target.position.y > thisTransform.position.y + .1) )
		{
			linkToPlayerControls.velocity.y = deathForce; 
			this.tag = "Untagged";
			velocity.x *= -.25;
			velocity.y = 4.5;
   			animPlay.PlayFramesFixed( 0, 3, 1, orientation); 
			enemyState = ShooterState.Dead;
			yield WaitForSeconds( 0.5);
			this.tag = "pickup";
			Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
			
			AudioManager.Get().Play(soundCrash, thisTransform, 6.0, 1.0);
//			enemyState = ShooterState.Stunned;
		}
		else if ( Input.GetAxis("Vertical") )
		{
			Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
		
			AudioManager.Get().Play(soundCrash, thisTransform);
			linkToPlayerControls.velocity.y = deathForce; 
		}
 
	}
	else if ( other.CompareTag("p_shot") )
	{
		GameManager.Get().Score += 75;
		BeatDown();
	}
	else if ( gameObject.CompareTag("p_shot") && !other.CompareTag("Item") )
	{
		yield WaitForSeconds(0.01);
		BeatDown();
	}
}

function BeatDown()
{
	GameManager.Get().Score += 50;
	Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
	AudioManager.Get().Play(soundCrash, thisTransform, 6.0, 1.0);
	gameObject.tag = "pickup";
	velocity.x *= -.25;
	velocity.y = 4.5;
	animPlay.PlayFramesFixed( 0, 3, 1, orientation); 
	enemyState = ShooterState.Dead;
}
