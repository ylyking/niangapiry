// Gumba ControllerColliderHit
// Description: Control component enemy gumba logic, and properties for gumba

enum GumbaState { moveLeft = 0 , moveRight, moveStop, jumpAir, enemyDie, goHome }

var gumbaState = GumbaState.moveLeft ;		// set default starting state

var moveSpeed		: float		= 30.0;		// set the speed of the gumba
var attackMoveSpeed	: float 		= 75.0;		// set the speed for attack
var jumpSpeed		: float 		= 3.0;		// set enemy jump height

var changeDirection	: float 		= 1.0;		// set facing direction delay 
var attackRange		: float 		= 1.0;		// set range for speed increase
var searchRange		: float 		= 2.0;		// set the range for finding the player
var returnHomeRange	: float 		= 2.0;		// set range for enemy to return to patrol
var deathForce		: float 		= 7.0;		// when the player jumps on me force him off 'x' amount
var homePosition	: Transform;				// load the home position
var chaseTarget		: Transform;							// load the player target
var gizmoToggle		: boolean	= true;						//toggle the debug display radius
var bounceHit		: AudioClip;							// hot sound for the enemy splat

private var velocity			: Vector3	= Vector3.zero;	// stroe the enemy movement in velocity (x, y, z)
private var gravity				: float 	= 20.0;			// weight of the world pushing enemy down
private var currentState;									// hold current state for setting later
private var aniPlay;										// Animation component
private var isRight				: boolean 	= false; 		// setting move orientation
private var myTransform 		: Vector3;					// store initial position
private var resetMoveSpeed  	: float 	= 0.0;			// store initial move speed
private var distanceToHome		: float		= 0.0;			// get dist to home position
private var distanceToTarget	: float 	= 0.0;			// get dist to target position
private var controller : CharacterController;				// get controller

function Start()
{
	myTransform = transform.position;
	resetMoveSpeed = moveSpeed;
	linkToPlayerPropeties = GetComponent(PlayerProperties);
	controller = GetComponent ( CharacterController );
	aniPlay = GetComponent (AnimSprite );
	
	// if (homePos == Vector3.zero)
		// homePos = myTransform;
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
	if ( other.tag == "collisionBoxFeet" )
	{
		var playerLink : GameObject;
		playerLink =  GameObject.Find("player");
		playerLink.GetComponent(PlayerControls).velocity.y  = deathForce; // make the player bounce

		if (bounceHit)
		{
		audio.clip = bounceHit;
		audio.Play();
		}
		
		var boxCollider = GetComponent( BoxCollider) as BoxCollider;
		if (boxCollider)
		{
			boxCollider.size = Vector3(0,0,0);
			Destroy ( boxCollider);
			gumbaState = GumbaState.enemyDie;
		}
		else
		{
			Debug.Log("Could not load box Collider");
		}		
	}
	if ( other.tag == "enemy" )
	{
		if ( other.collider != this.collider )
		{
		Physics.IgnoreCollision(other.collider, this.collider);
		}
	}
}

function Update () 
{
	// actualizar distancia entre enemy y player
	distanceToTarget = Vector3.Distance(chaseTarget.position, transform.position );	
	
	if ( distanceToTarget <= searchRange )		// SI distancia al objetivo es menor que el rango de busqueda..
	{
		ChasePlayer ();						// perseguír al player y...
		if ( distanceToTarget <= attackRange )	// si está dentro de rango de ataque..
		{
		// ChasePlayer ();
		moveSpeed = attackMoveSpeed;			// acelerar la velocidad de ataque
		}
		else
		{
		// ChasePlayer ();
		moveSpeed = resetMoveSpeed;			// sino continuar rastreandolo a velocidad normal
		}
	}
	else									// SINO chequear rutina habitual de comportamiento cuando está solo...
	{
		// revisar (actualizar) distancia entre enemigo y su rincón habitual
		distanceToHome = Vector3.Distance( homePosition.position, transform.position);
		
		if( distanceToHome > returnHomeRange)	// si está demasiado lejor de casa...
		{
			GoHome();						// Volver a casa! ( cambia dirección de lado y avanzar indefinidamente.. 
		}									// ... hasta que vuelva a alejarse demasiado y se llama a esta función de nuevo)
	}
	
	if (controller.isGrounded )				// si el enemigo está sobre el suelo...
	{
		switch ( gumbaState )					// checar estados
		{
			case GumbaState.moveLeft:
				PatrolLeft();
				break;
			case GumbaState.moveRight:
				PatrolRight();
				break;
			case GumbaState.moveStop:
				if  ( isRight ) 
					IdleRight();
				else
					IdleLeft();
				break;		
			case GumbaState.jumpAir:
				if  ( isRight ) 
					JumpRight();
				else  
					JumpLeft();
				break;
			case GumbaState.enemyDie:
				if  ( isRight ) 
					DieRight();
				else
					DieLeft();
				break;
			case GumbaState.goHome:
				GoHome();
				break;		
		}
	}
	// Apply gravity
	velocity.y -= gravity * Time.deltaTime;	// apply the gravity
	// Move the controller
	controller.Move(velocity * Time.deltaTime);// move the controller
//	controller.e
}
					
function PatrolRight()						// move enemy right.. or left
{
	velocity.x = moveSpeed * Time.deltaTime;	// move the controller right
	aniPlay.PlayFrames(6, 0, 16, 1);
	isRight = true;
	currentState = gumbaState;				// a jump state  check up
}

function PatrolLeft()						
{
	velocity.x = -moveSpeed * Time.deltaTime;	// move the controller left
	aniPlay.PlayFrames(7, 0, 16, 1);
	isRight = false;
	currentState = gumbaState;				// a jump state  check up
	}

function IdleRight()							// set movement to 0 and face right or left
{
	velocity.x = 0;	// move the controller left
	aniPlay.PlayFrames(0, 0, 29, 1);
	isRight = true;
	currentState = gumbaState;				// a jump state  check up
}

function IdleLeft()						
{
	velocity.x = 0;	// move the controller left
	aniPlay.PlayFrames(2, 0, 31, 1);
	isRight = false;
	currentState = gumbaState;				// a jump state  check up
}

function JumpRight()							// get jump and face left/right
{
	velocity.y = jumpSpeed;
	aniPlay.PlayFrames(8, 7, 1, 1 ); 
	isRight = true;
	currentState = gumbaState;				// a jump state  check up
}

function JumpLeft()
{
	velocity.y = jumpSpeed;
	aniPlay.PlayFrames(9, 7, 1, 1 ); 
	isRight = false;
	currentState = gumbaState;				// a jump state  check up
}

function DieRight()							// kill enemy by right or left
{
	velocity.x = 0;
	yield WaitForSeconds(0.1);
	aniPlay.PlayFrames( 10, 0, 16, 1);
	yield WaitForSeconds(0.4);
	Destroy(gameObject);
}

function DieLeft()
{
	velocity.x = 0;
	yield WaitForSeconds(0.1);
	aniPlay.PlayFrames( 11, 0, 16, 1);
	yield WaitForSeconds(0.4);
	Destroy(gameObject);
}

function ChasePlayer ()													// Check where the player is in relation to our position
{
	if ( transform.position.x  <=  chaseTarget.position.x - changeDirection) // chequear orientación de busqueda del player..
	{
		gumbaState = GumbaState.moveRight;								// moverse hacia derecha o izq. según lado
	}
	if ( transform.position.x  >=  chaseTarget.position.x + changeDirection)	// (changeDirection es un delay de distancia para girar)
	{
		gumbaState = GumbaState.moveLeft;
	}
}

function GoHome()														// Find the home origin and return back to it
{
	if ( transform.position.x  <= homePosition.position.x )					// si distancia izq. o derecha menor/mayor que origen...			
	{
		gumbaState = GumbaState.moveRight;								// avanzar en su dirección hasta proxima llamada a GoHome ( cuando vuelva a salírse del home range )
	}
	if ( transform.position.x >= homePosition.position.x )
	{
		gumbaState = GumbaState.moveLeft;
	}
}

function OnDrawGizmos()		// toggle the gizmos for designer to see 6 Debug reaching ranges
{
	if (gizmoToggle)
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere( transform.position, attackRange);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere( transform.position, searchRange);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere( homePosition.position, returnHomeRange);
	}
}