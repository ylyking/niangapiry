#pragma strict
// Enemy ControllerColliderHit
// Description: Control component enemy shooter logic, and properties for gumba

//enum EnemyState { moveLeft = 0 , moveRight, moveStop, jumpAir, enemyDie, goHome }
enum ShooterState {	Sleeping = 0, Alert, Holded, Stunned, Shooted, Dead }

public var enemyState				: ShooterState = ShooterState.Sleeping ;	// set default starting state

public var aimDamping 				: float 		= 4.0;			// speed & time taken for aiming
public var fireRate					: float		 	= 2.0;	// time delay between shots, more lower it's more harder to avoid
public var shootPower	 			: float 		= 10;			// the power or speed of the projectile.

public var deathForce				: float 		= 8.0;			// when the player jumps on me force him off 'x' amount
public var alertRange				: float 		= 2.0;			// set the range for finding the player
public var attackRange				: float 		= 5.0;			// set range for speed increase
public var gizmoToggle				: boolean		= true;			//toggle the debug display radius
public var bounceHit				: AudioClip;					// hot sound for the enemy splat

private var animPlay 				: AnimSprite; 					// : Component

private var orientation			 	: int 			= -1;			// orientation of the enemy
private var nextFire				: float 		= 3.0;
private var gravity					: float 		= 9.8;			// weight of the world pushing enemy down
private var stunTime				: float 		= 10.0;
private var distanceToTarget		: float 		= 0.0;			// get dist to target position

private var Holded					: boolean 		= false;
private var AnimFlag 				: boolean 		= true;
private var grounded 				: boolean		= false;
private var velocity				: Vector3 		= Vector3.zero;	// store the enemy movement in velocity (x, y, z)

public var target					: Transform;					// target to search ( the player)
public var aimCompass				: Transform;					// it's a simple child transform inside this gameObject

public var projectile 				: GameObject;					// Prefab to be shooted ( set previously it's values )
private	var playerLink 				: GameObject;

private var linkToPlayerPropeties 	: PlayerProperties;
private var linkToPlayerControls 	: PlayerControls;

private var AlertRangeX2			: float			= alertRange * alertRange;
private var AttackRangeX2 			: float			= attackRange * attackRange;

	
function Start()
{
//	AimCompass = this.gameObject.GetComponentInChildren( Transform ); // not working

	if ( rigidbody)				// So, we need a rigidbody somewhere to do a simple trigger/collision check, great
	{
    	rigidbody.freezeRotation = true;
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
		velocity = Vector3.zero; 
		
// actualizar distancia entre enemy y player
//	distanceToTarget = Vector3.Distance( target.position, transform.position );	
	distanceToTarget = (target.position - transform.position).sqrMagnitude;	

     	     	switch ( enemyState )					// check states
		{
			case ShooterState.Sleeping:
				if (!grounded) velocity.y -= gravity * Time.deltaTime; // (air drag)
				
				Sleeping();
				break;
						
			case ShooterState.Alert:
				if (!grounded) velocity.y -= gravity * Time.deltaTime;				
				Search();
				break;
				
			case ShooterState.Stunned: 
//				if (!grounded)velocity.y -= gravity * Time.deltaTime ;				
				Stunned();
				break;			
			
			case ShooterState.Holded: 
				BeingHolded();
				break;
			case ShooterState.Shooted: 
				velocity.y -= gravity * Time.deltaTime;
				Fired();
				break;				
			case ShooterState.Dead:
//				if (!grounded)velocity.y -= gravity * Time.deltaTime ;
				Die();
				break;
		
		}
//  }
//   	else if (!grounded) transform.position.y -= gravity * Time.deltaTime;

//    if (this.rigidbody)	    // We apply gravity manually for more tuning control		
//    {
//    	rigidbody.AddForce(Vector3 (0, -gravity * rigidbody.mass, 0));
//    	grounded = false;
//    }
//    else grounded = true;

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
 		
		enemyState = ShooterState.Alert;
		AnimFlag = true;
		return;
	}
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
    	if( AnimFlag) animPlay.PlayFrames(1 , 2, 2, orientation);
    			
    	if ( Time.time > nextFire )
    	{
    		AnimFlag = false;
//    		animPlay.PlayFrames(2 , 0, 2, orientation); 
    		Shoot();
//    		yield WaitForSeconds(.5f);
			var timertrigger = Time.time + .5f;
			while( timertrigger > Time.time )
			{
    			animPlay.PlayFrames(2 , 0, 2, orientation); 
				if ( enemyState != ShooterState.Alert) return;
    			yield;
			}
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

function Stunned()									// Boleado
{
	this.tag = "pickup";  							//  change tag a pickable

	var timertrigger = Time.time + stunTime;
	while( timertrigger > Time.time )				// start knock out time counter
	{
		animPlay.PlayFrames(3 , 0, 2, orientation);	// animate Stunning..
		
		if ( transform.parent && !collider.enabled ) // if Player Grabs him change EnemyState > Handled	
			enemyState = ShooterState.Holded ;

		if ( enemyState != ShooterState.Stunned) return;		  // so he is not stunned anymore
    	yield;
	}
		
	if ( !transform.parent  )						// but if knock out time is over & player !isHolding( this)..
	{
		this.tag = "Untagged";  
    	if ( distanceToTarget > AlertRangeX2 )
			enemyState = ShooterState.Sleeping;		// after a while change EnemyState > Default
		else 
			enemyState = ShooterState.Alert;		// or alert if the player is near
			
	}
}



function BeingHolded()
{
//   this.collider.isTrigger = false;
   orientation = linkToPlayerControls.orientation;
   animPlay.PlayFrames(3 , 2, 1, orientation); 
   
   if ( collider.enabled &&  !transform.parent) 
   {
//   	moveDirection = Vector3( Mathf.Sign(orientation), Mathf.Sign(Input.GetAxis( "Vertical")),0) * 1.5; 	// == transform.up * speed ( orientation of move + force of impulse )
//   	grounded = false;
//   	moveDirection = Vector3( orientation * 0.7, 0.7, 0) * 2;
	
	
	moveDirection = Vector3( Mathf.Sign(orientation) * 3, 4, 0);
	NextPosition = StartPosition = transform.position;
   	enemyState = ShooterState.Shooted;
   }
}
//var Gravitation : Vector3 = Vector3.zero;
var moveDirection : Vector3 = Vector3.zero;
var StartPosition : Vector3 = Vector3.zero;
var NextPosition  : Vector3 = Vector3.zero;
var TimeLapse     : float	= 0;

function Fired()
{

//		NextPosition.x += moveDirection.x * Time.deltaTime; // multiply with speed to go faster
//		NextPosition.y += moveDirection.y * Time.deltaTime;

////////////////////////////////////////////////////////////////////////////////////////////////////	

	TimeLapse  += Time.deltaTime;							// the difference it's time increment
	transform.position.x = StartPosition.x + (moveDirection.x * TimeLapse) ;
	transform.position.y = StartPosition.y + (moveDirection.y * TimeLapse) - (.5 * gravity * (TimeLapse * TimeLapse));
		

		
//		Gravitation.y += (.5 * gravity  )*(Time.deltaTime * Time.deltaTime) ; 
//		// You probably either want to use x * x or Time.time * Time.time.
//		moveDirection.y -= Gravitation.y ;  
//		
//	
//
//
//		NextPosition = Vector3( moveDirection.x, StartSpeed, 0);
//		if ( Time.time > ) StartSpeed = 0;
//		NextPosition.y -= gravity * Time.deltaTime;
//		transform.position += NextPosition * Time.deltaTime ;
		
		
}

function Die()
{
}

function OnTriggerStay () 		//function OnCollisionStay ()
{
    grounded = true;    
}

function OnTriggerEnter( other : Collider)
{
		
	if ( other.tag == "Player" && other.transform.position.y > transform.position.y  )
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

