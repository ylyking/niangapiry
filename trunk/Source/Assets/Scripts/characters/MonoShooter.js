#pragma strict
// Enemy ControllerColliderHit
// Description: Control component enemy shooter logic, and properties for gumba

public var enemyState				: ShooterState = ShooterState.Sleeping ;	// set default starting state

public var aimDamping 				: float 		= 4.0;			// speed & time taken for aiming
public var fireRate					: float		 	= 2.0;			// delay between shots, more lower, more harder to avoid
public var shootPower	 			: float 		= 9;			// the power or speed of the projectile.
public var deathForce				: float 		= 8.0;			// when the player jumps on me force him off 'x' amount

public var gizmoToggle				: boolean		= true;			// toggle the debug display radius
public var bounceHit				: AudioClip;					// hot sound for the enemy splat

private var orientation			 	: int 			= -1;			// orientation of the enemy
private var nextFire				: float 		= 3.0;			// delay between attacks
private var gravity					: float 		= 9.8;			// weight of the world pushing enemy down
private var stunTime				: float 		= 1.0;			// enemy knocked out time...

public 	var alertRange				: float 		= 2.0;			// set the range for finding the player
public 	var attackRange				: float 		= 5.0;			// set range for speed increase
private var distanceToTarget		: float 		= 0.0;			// the dist to target position stored in square mag.

private var AlertRangeX2			: float			= alertRange * alertRange;
private var AttackRangeX2 			: float			= attackRange * attackRange; 

private var Holded					: boolean 		= false;
private var AnimFlag 				: boolean 		= true;
private var grounded 				: boolean		= true;

public 	var HoldedPosition			: Vector3 = Vector3( 0,.3,-.1);	// change own enemy position when grabed
public  var moveDirection 			: Vector3 = Vector3( 5, 3,  0);	// Force * direction of throwing the enemy 
private var velocity				: Vector3 = Vector3.zero;		// store the enemy movement in velocity (x, y, z)

public var target					: Transform;					// target to search ( the player )
public var aimCompass				: Transform;					// it's a simple child transform inside this gameObject
private var thisTransform			: Transform;					// own enemy's tranform cached

public var projectile 				: GameObject;					// Prefab to be shooted ( set previously it's values )
private var animPlay 				: AnimSprite; 					// : Component
private var linkToPlayerControls 	: PlayerControls;

var soundCrash 						: AudioClip;
var soundAttack						: AudioClip;
var ParticleStars					: GameObject;


function Start()
{
//	controller = this.GetComponent ( CharacterController );
	thisTransform = transform;

	if ( rigidbody)								// So, we need a rigidbody somewhere to do sny trigger/collision check, great
	{
    	rigidbody.freezeRotation = true;
    	rigidbody.useGravity = false;
    }
    	
	if (!target) 
		target =  GameObject.Find("Pombero").transform;			//	We can Use this system to get the player's Id & position
	
	if (!aimCompass)
		aimCompass = thisTransform.FindChild("AimCompass");
	
	if ( target) 
	{
			linkToPlayerControls = target.GetComponent("PlayerControls") as PlayerControls;
//			if (! linkToPlayerControls  ) print("big fucking error!");
	}else print(" Beware target empty: player link not found!");
			
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

function CoUpdate ()	: IEnumerable											// function Update ()
{

	if ( thisTransform.IsChildOf( target) ) 									// check if the player has taken us... 
	{																	
		thisTransform.position = target.position +  HoldedPosition; 			// Update own hold position & player's too
   		enemyState = ShooterState.Holded ;										// & change enemy state to holded..
	}
	else 
	distanceToTarget = (target.position - thisTransform.position).sqrMagnitude;	// it's much faster using Square mag. 

    switch ( enemyState )														// check states of the character:
	{
		case ShooterState.Sleeping:		// 0# GAUCHO SLEEPING:		
				Sleeping();	
			break;
						
		case ShooterState.Alert:		// 1# ALERT STATE:
				velocity = Vector3.zero;
						
				Search();														// Begin Searching of the player & attack him
			break;
				
//		case ShooterState.Stunned: 		// 2# KNOCK OUT:
//				velocity = Vector3.zero;
//				
//				Stunned();								
//			break;			
			
		case ShooterState.Holded: 		// 3# ENEMY IS TAKEN	
				BeingHolded();
			break;
				
		case ShooterState.Shooted: 		// 4# ENEMY IS THROWED IN THE AIR
				thisTransform.RotateAroundLocal( Vector3.forward, -orientation * 2 * Time.deltaTime ); 
									
			break;				
				
		case ShooterState.Dead:			// 5# ENEMY IS DEAD
				animPlay.PlayFramesFixed(1 , 1, 1, orientation);
				thisTransform.RotateAroundLocal( Vector3.forward, -orientation * 45 * Time.deltaTime ); 
    			grounded = false;
				
//				Destroy(gameObject, 15);											// Keep falling and die after a while
			break;		
	}
	
	if (!grounded)	velocity.y -= gravity * Time.deltaTime ;	
	thisTransform.position += velocity * Time.deltaTime;
	
	if (thisTransform.position.y < 0 )	Destroy(gameObject, 2);	// If character falls get it up again 
	
}

function Sleeping()
{
	if( AnimFlag)
 	{
		velocity = Vector3.zero;
		animPlay.PlayFrames( 0, 0, 2, orientation, 2);							// Do zzZZZ Animation
	

 		if (  distanceToTarget <= AlertRangeX2 )
 		{
			AnimFlag = false;
 			var groundPosition = thisTransform.position.y;
 			var timertrigger = Time.time + 0.55f;
 			
			while( timertrigger > Time.time )
			{
//				thisTransform.Translate( 0, 1.5 * Time.deltaTime, 0);
				thisTransform.position.y = groundPosition + Mathf.Sin((timertrigger - Time.time) * 5) * .5;
				animPlay.PlayFramesFixed(1 , 0, 1, orientation); 				// Do Wake up Animation
			
				if ( enemyState != ShooterState.Sleeping)
				 {	thisTransform.position.y = groundPosition; AnimFlag = true;  return; }
    		
    			yield;
			}
			thisTransform.position.y = groundPosition;
			AnimFlag = true;
 			enemyState = ShooterState.Alert;
			return;
		}
	}
}

function Search()
{
	orientation = Mathf.Sign( target.position.x - thisTransform.position.x  );

	//  Seek distance and players position/angle
//	if (aimCompass)
//	{
		var lookPos : Quaternion = Quaternion.LookRotation( thisTransform.position - target.position, Vector3.forward );
	
		lookPos.y = 0;
		lookPos.x = 0;
		aimCompass.rotation = Quaternion.Slerp( aimCompass.rotation, lookPos, Time.deltaTime * aimDamping);
//	}
	
    if ( distanceToTarget <= AttackRangeX2 )									// if player's near enemy -> Aim & Attack him
    {	
    	if( AnimFlag) animPlay.PlayFramesFixed( 1, 2, 2, orientation);			// Do Seeking Animation
    			
    	if ( Time.time > nextFire )												// wait some time between shots 
    	{
    		AnimFlag = false;
//   
    		Shoot();		
    		var timerHolder = Time.time + .15f;
			while( timerHolder > Time.time )									// do some throw animation for a while 
			{
    			animPlay.PlayFramesFixed( 2, 0, 2, orientation); 				// Do Throw Animation
				if ( enemyState != ShooterState.Alert) { AnimFlag = true; return;}
    			yield;
			}
    		
			var timertrigger = Time.time + .35f;
			while( timertrigger > Time.time )									// do some throw animation for a while 
			{
    			animPlay.PlayFramesFixed( 2, 2, 2, orientation); 				// Do Throw Animation
				if ( enemyState != ShooterState.Alert) { AnimFlag = true; return;}
    			yield;
			}
    		AnimFlag = true;
    	}
    }
    else enemyState = ShooterState.Sleeping;								// else if player it's out enemy range -> go Sleep
}

function Shoot() 
{
//	if (aimCompass) 
//	{
		AudioManager.Get().Play(soundAttack, thisTransform);
		
    	// Add fireRate and current time to nextFire
    	nextFire = Time.time + fireRate;

		// Instantiate the projectile
    	var clone : GameObject = Instantiate ( projectile, aimCompass.position, aimCompass.rotation);
    
    	Physics.IgnoreCollision(clone.collider, this.gameObject.collider); 				// it works but the distance it s lame
 	// clone.transform.Translate( Vector3( 0, 1, 0) ); 						// avoid hits between shot & shooter own colliders  

    	clone.name = "Shot";

		clone.GetComponent(BulletShot).FireAnimated( aimCompass.up * shootPower, 0, 2, 1); 	// shot with a short animation

}

//function Stunned()																	// "Boleado"
//{
//   			animPlay.PlayFramesFixed( 1, 1, 1, orientation); 
//
//			yield WaitForSeconds(0.5);
//
//			this.tag = "pickup";
//			velocity.x *= -.25;
//			velocity.y = 4.5;
//			enemyState = ShooterState.Dead;
//
//}

function BeingHolded()
{
   grounded = true;		
   velocity = Vector3.zero;
   orientation = linkToPlayerControls.orientation;						// update own orientation according player direction
   animPlay.PlayFramesFixed( 1, 1, 1, orientation); 
   
   if ( collider.enabled &&  !thisTransform.parent) 					// if collider returned & we haven't parent anymore..
   {
    grounded = false;
		
	velocity = moveDirection;
	velocity.y += 3.5 * Input.GetAxis("Vertical");				
	velocity.x *= Mathf.Sign(orientation);
   	enemyState = ShooterState.Shooted;												// Then it means we have been shooted...
    gameObject.tag = "p_shot";	
    
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
   			animPlay.PlayFramesFixed( 1, 1, 1, orientation); 
			enemyState = ShooterState.Dead;
			yield WaitForSeconds( 0.5);
			this.tag = "pickup";
			AudioManager.Get().Play(soundCrash, thisTransform, 6.0, 1.0);
			Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
			
			
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
		GameManager.Get().Score += 100;
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
	GameManager.Get().Score += 100;
	AudioManager.Get().Play(soundCrash, thisTransform, 6.0, 1.0);
	gameObject.tag = "pickup";
	velocity.x *= -.25;
	velocity.y = 4.5;
	animPlay.PlayFramesFixed( 1, 1, 1, orientation ); 
	enemyState = ShooterState.Dead;
	Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
	
}

