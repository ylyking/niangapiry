#pragma strict
// Enemy ControllerColliderHit
// Description: Control component enemy shooter logic, and properties for gumba

//enum EnemyState { moveLeft = 0 , moveRight, moveStop, jumpAir, enemyDie, goHome }
enum ShooterState {	Sleeping = 0, Alert, Holded, Stunned, Dead }

var enemyState		: ShooterState = ShooterState.Sleeping ;	// set default starting state

var aimDamping 		: float 		= 4.0;		// speed & time taken for aiming
var fireRate		: float		 	= 2.0;		// time delay between shots, if too lower it's more harder to avoid
var shootPower	 	: float 		= 10;		// the power or speed of the projectile.

var deathForce		: float 		= 8.0;		// when the player jumps on me force him off 'x' amount
var alertRange		: float 		= 2.0;		// set the range for finding the player
var attackRange		: float 		= 5.0;		// set range for speed increase
var gizmoToggle		: boolean		= true;		//toggle the debug display radius
var bounceHit		: AudioClip;				// hot sound for the enemy splat

private var animPlay 				: AnimSprite; 					// : Component

private var orientation			 	: int 			= -1;			// orientation of the enemy
private var nextFire				: float 		= 3.0;
private var gravity					: float 		= 9.8;			// weight of the world pushing enemy down
private var stunTime				: float 		= 10.0;
private var distanceToTarget		: float 		= 0.0;			// get dist to target position

private var Holded					: boolean 		= false;
private var AnimFlag 				: boolean 		= true;
private var grounded 				: boolean		= false;
private var velocity				: Vector3 		= Vector3.zero;	// stroe the enemy movement in velocity (x, y, z)

public var target					: Transform;				// target to search ( the player)
public var aimCompass				: Transform;				// it's a simple child transform inside this gameObject

public var projectile 				: GameObject;				// Prefab to be shooted ( set previously it's values )
private	var playerLink 				: GameObject;

private var linkToPlayerPropeties 	: PlayerProperties;
private var linkToPlayerControls 	: PlayerControls;

private var AlertRangeX2			: float			= alertRange * alertRange;
private var AttackRangeX2 			: float			= attackRange * attackRange;

	
function Start()
{
//	AimCompass = this.gameObject.GetComponentInChildren( Transform ); // not working

	if (this.rigidbody)	
	{
    	this.rigidbody.freezeRotation = true;
    	rigidbody.useGravity = false;
    }

//	EnemyPosition = transform.position;
//	resetMoveSpeed = moveSpeed;

//	controller = this.GetComponent ( CharacterController );
	
	playerLink =  GameObject.Find("Pombero");
	if (playerLink) 
	{
			linkToPlayerControls = playerLink.GetComponent("PlayerControls") as PlayerControls;
			
			if (! linkToPlayerControls  ) print("big fucking error!");
	}
	else print(" Beware: player link not found!");
			
	animPlay = GetComponent(AnimSprite);
	
	while( true)
	{
		CoUpdate();
		yield;
	}
}


//function Update ()
function CoUpdate ()
{

// actualizar distancia entre enemy y player
//	distanceToTarget = Vector3.Distance( target.position, transform.position );	
	distanceToTarget = (target.position - transform.position).sqrMagnitude;	

 
    if (grounded)
    {
    	switch ( enemyState )					// check states
		{
			case ShooterState.Sleeping:
				Sleeping();
				break;
						
			case ShooterState.Alert:
				Search();
				break;
				
			case ShooterState.Stunned: 
				Stunned();
				break;			
			
			case ShooterState.Holded: 
				BeingHolded();
				break;
				
			case ShooterState.Dead:
				break;
		
		}
//    	print(" Gaucho is grounded");
    }
    

    if (this.rigidbody)	    // We apply gravity manually for more tuning control		
    {
    	rigidbody.AddForce(Vector3 (0, -gravity * rigidbody.mass, 0));
    	grounded = false;
    }
    else grounded = true;
        
}

function Sleeping()
{
	if( AnimFlag) animPlay.PlayFrames(0 , 0, 1, orientation);

 	if (  distanceToTarget <= AlertRangeX2 )
 	{
 		AnimFlag = false;
		animPlay.PlayFrames(0 , 1, 1, orientation);
 		yield WaitForSeconds(0.25); // 	yield WaitForSeconds(0.167);
 		
		enemyState = ShooterState.Alert;
		AnimFlag = true;
	}
//	else animPlay.PlayFrames(0 , 0, 1, 1);
}

function Search()
{	
	// Seek distance and players position/angle
	var lookPos : Quaternion = Quaternion.LookRotation( transform.position - target.position, Vector3.forward );
	orientation = 	Mathf.Sign( target.position.x - transform.position.x  );
	
//	print( target.position + " - gaucho: " + transform.position );
	
	lookPos.y = 0;
	lookPos.x = 0;

	aimCompass.rotation = Quaternion.Slerp( aimCompass.rotation, lookPos, Time.deltaTime * aimDamping);
				
    // Attack
    if ( distanceToTarget <= AttackRangeX2 )
    {	
    	if( AnimFlag) animPlay.PlayFrames(0 , 2, 2, orientation);
    			
//    	if ( Input.GetKey ("x") && Time.time > nextFire)
    	if ( Time.time > nextFire )
    	{
    		AnimFlag = false;
    		animPlay.PlayFrames(1 , 0, 1, orientation); 
    		Shoot();
    		yield WaitForSeconds(.5f);
    		AnimFlag = true;
    	}
    }
    else enemyState = ShooterState.Sleeping;
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
//	clone.GetComponent(BulletShot).Fire( aimCompass.up * shootPower, 5); 	// shot with a speed of 5 and a rotation of 10
	clone.GetComponent(BulletShot).FireAnimated( aimCompass.up * shootPower, 1, 1, 2); 	// shot with a short animation
}

function Stunned()								// Hay que revisar esto
{
	this.tag = "pickup";  						//  cambiar tag a pickable
	animPlay.PlayFrames(0 , 1, 1, orientation);	// animar voleado
												
	if ( transform.parent && transform.rigidbody.isKinematic ) // Player Grabs him change EnemyState > Handled	
		enemyState = ShooterState.Holded ;		
																									
	yield WaitForSeconds(stunTime);			// if not after a while change EnemyState > Default
		
	if ( !transform.parent  )				// if player !isHolding( this)
	{
		this.tag = "Untagged";  
		enemyState = ShooterState.Sleeping;
	}
}

function BeingHolded()
{
	
}

function OnCollisionStay ()
{
    grounded = true;    
}

function OnTriggerEnter( other : Collider)
{
	// if (other.tag == "pathNode" )
	// {
		// var linkToPathNode = other.GetComponent(pathNode);
		// gumbaState =   parseInt(linkToPathNode.pathInstruction); // Here we need to do a typeCast with parseInt();
		
		// if (linkToPathNode.overrideJump )
		// {
			// jumpSpeed = linkToPathNode.jumpOverride;
		// }
	// }

		
	if ( other.transform.tag == "Player" && other.transform.position.y > transform.position.y  )
	{
//			Debug.Log(" Enemy: " + transform.position.y );
//			Debug.Log(" Player: " + other.transform.position.y);
			linkToPlayerControls.velocity.y = deathForce; 
	
		if ( this.tag != "pickup"  )
		{
//			Debug.Log(" Player rejump: " + linkToPlayerControls.velocity.y);
//			linkToPlayerControls.velocity.y *= -1;
			// make the player bounce
//			linkToPlayerControls.velocity.y += deathForce; // make the player bounce
//			Debug.Log("Triggering something: " + other.gameObject.name + " with Gausho!" );
		
//			this.tag = "pickup";
//			rigidbody.isKinematic = false;

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
	if ( other.tag == "enemy" )
	{
		if ( other.collider != this.collider )
		{
			Physics.IgnoreCollision(other.collider, this.collider);
		}
	}
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

