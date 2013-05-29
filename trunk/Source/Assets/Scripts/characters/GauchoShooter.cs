// Enemy ControllerColliderHit
// Description: Control component enemy shooter logic, and properties for gumba


using UnityEngine;
using System.Collections;

public enum ShooterState { Sleeping = 0, Alert, Stunned, Holded, Shooted, Dead }


public class GauchoShooter : MonoBehaviour {


public ShooterState enemyState = ShooterState.Sleeping ;	// set default starting state

public float aimDamping 		= 4.0f;			// speed & time taken for aiming
public float fireRate		 	= 2.0f;			// delay between shots, more lower, more harder to avoid
public float shootPower 		= 10;			// the power or speed of the projectile.
public float deathForce 		= 12.0f;		// when the player jumps on me force him off 'x' amount

public bool  gizmoToggle		= true;			// toggle the debug display radius
public AudioClip bounceHit;					    // hot sound for the enemy splat

private int orientation 		= -1;			// orientation of the enemy
private float nextFire 		    = 3.0f;			// delay between attacks
private float gravity 		    = 9.8f;			// weight of the world pushing enemy down
private float stunTime 		    = 6.0f;			// enemy knocked out time...

public 	float alertRange 		= 2.0f;			// set the range for finding the player
public 	float attackRange 		= 5.0f;			// set range for speed increase
private float distanceToTarget 	= 0.0f;			// the dist to target position stored in square mag.

private float AlertRangeX2			= 0;
private float AttackRangeX2			= 0; 

//private bool  Holded 		= false;
private bool  AnimFlag 		= true;
private bool grounded = false;
//private bool  KnockOut		= false;

public 	Vector3 HoldedPosition = new Vector3( 0,.3f,-.1f);	// change own enemy position when grabed
public Vector3 moveDirection = new Vector3(5, 3, 0);	    // Force * direction of throwing the enemy 
private Vector3 velocity = Vector3.zero;		            // store the enemy movement in velocity (x, y, z)

public Transform target;					                // target to search ( the player )
public Transform aimCompass;					            // it's a simple child transform inside this gameObject
private Transform thisTransform;					        // own enemy's tranform cached

public GameObject projectile;					            // Prefab to be shooted ( set previously it's input )
private AnimSprite animPlay; 					            // : Component
private PlayerControls linkToPlayerControls;

public AudioClip soundCrash;
public AudioClip soundAttack;
public GameObject ParticleStars;



	
void  Start (){
//	AimCompass = this.gameObject.GetComponentInChildren< Transform >(); // not working
//	controller = this.GetComponent ( CharacterController );
	thisTransform = transform;
    AlertRangeX2 = alertRange * alertRange;
    AttackRangeX2 = attackRange * attackRange; 

	if ( rigidbody)								// So, we need a rigidbody somewhere to do sny trigger/collision check, great
	{
    	rigidbody.freezeRotation = true;
    	rigidbody.useGravity = false;
    }

	if (!aimCompass)
		aimCompass = thisTransform.FindChild("AimCompass");
 			
	animPlay = (AnimSprite)GetComponent<AnimSprite>();

    StartCoroutine(CoUpdate());
}

IEnumerator CoUpdate ()	 											// void  Update ()
{
    while (thisTransform)
    {
        if (!target)
        {
            if (Managers.Game.PlayerPrefab)
            {
                target = Managers.Game.PlayerPrefab.transform;			//	We can Use this system to get the player's Id & position
                linkToPlayerControls = (PlayerControls)target.GetComponent<PlayerControls>() ;
            }
            else
                yield return 0;
        }

        if (target)
        if (thisTransform.IsChildOf(target)) 									// check if the player has taken us... 
        {
            thisTransform.position = target.position + HoldedPosition; 			// Update own hold position & player's too
            enemyState = ShooterState.Holded;										// & change enemy state to holded..
            //   		moveDirection =	ThrowDirection;
        }
        else
            distanceToTarget = (target.position - thisTransform.position).sqrMagnitude;	// it's much faster using Square mag. 

        switch (enemyState)														// check states of the character:
        {
            case ShooterState.Sleeping:		// 0# GAUCHO SLEEPING:		
                StartCoroutine( Sleeping());
                break;

            case ShooterState.Alert:		// 1# ALERT STATE:
                velocity = Vector3.zero;

                StartCoroutine( Search());														// Begin Searching of the player & attack him
                break;

            case ShooterState.Stunned: 		// 2# KNOCK OUT:
                velocity = Vector3.zero;
                StartCoroutine( Stunned());
                break;

            case ShooterState.Holded: 		// 3# ENEMY IS TAKEN	
                BeingHolded();
                break;

            case ShooterState.Shooted: 		// 4# ENEMY IS THROWED IN THE AIR
                thisTransform.RotateAroundLocal(Vector3.forward, -orientation * 2 * Time.deltaTime);

                break;

            case ShooterState.Dead:			// 5# ENEMY IS DEAD
                animPlay.PlayFramesFixed(1, 1, 1, orientation);
                thisTransform.RotateAroundLocal(Vector3.forward, -orientation * 45 * Time.deltaTime);
                //				Destroy(gameObject, 5);											// Keep falling and die after a while
                break;
        }

        if (!grounded) velocity.y -= gravity * Time.deltaTime;
        thisTransform.position += velocity * Time.deltaTime;

        if (thisTransform.position.y < 0 && thisTransform != null) Destroy(gameObject, 2);	// If character falls get it up again 

        yield return 0;
    }
	
}

void OnTriggerStay(Collider other)
{										// void  OnCollisionStay (){
//	if ( other.CompareTag( "Untagged" ) )
    	grounded = ((int)enemyState < 3 );    
}


IEnumerator  Sleeping (){
	if( AnimFlag)
 	{
		velocity = Vector3.zero;
		animPlay.PlayFrames( 0, 0, 4, orientation, 2);							// Do zzZZZ Animation
	

 		if (  distanceToTarget <= AlertRangeX2 )
 		{
			AnimFlag = false;
 			float groundPosition= thisTransform.position.y;
 			float timertrigger= Time.time + 0.55f;
 			
			while( timertrigger > Time.time )
			{
//				thisTransform.Translate( 0, 1.5f * Time.deltaTime, 0);
                thisTransform.position = new Vector3(thisTransform.position.x,
                                                     groundPosition + Mathf.Sin((timertrigger - Time.time) * 5) * .5f, 
                                                     thisTransform.position.z);
				animPlay.PlayFramesFixed(1 , 0, 2, orientation); 				// Do Wake up Animation
			
				if ( enemyState != ShooterState.Sleeping)
				 {	
                 thisTransform.position = new Vector3(thisTransform.position.x, groundPosition, thisTransform.position.z);
                    AnimFlag = true;
                    yield break; 
                }
    		
    			yield return 0;
			}
            thisTransform.position = new Vector3(thisTransform.position.x, groundPosition, thisTransform.position.z);
			AnimFlag = true;
 			enemyState = ShooterState.Alert;
			yield break;
		}
	}
}

IEnumerator  Search (){
	orientation = (int)Mathf.Sign( target.position.x - thisTransform.position.x  );

	//  Seek distance and players position/angle
//	if (aimCompass)
//	{
		Quaternion lookPos = Quaternion.LookRotation( thisTransform.position - target.position, Vector3.forward );
	
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
    		Shoot();		
    		
			float timertrigger= Time.time + .5f;
			while( timertrigger > Time.time )									// do some throw animation for a while 
			{
    			animPlay.PlayFramesFixed( 2, 0, 2, orientation); 				// Do Throw Animation
				if ( enemyState != ShooterState.Alert) { AnimFlag = true; yield break;}
    			yield return 0;
			}
    		AnimFlag = true;
    	}
    }
    else enemyState = ShooterState.Sleeping;								// else if player it's out enemy range -> go Sleep
    yield return 0;
}

void  Shoot (){
//	if (aimCompass) 
//	{
		Managers.Audio.Play(soundAttack, thisTransform);
    	// Add fireRate and current time to nextFire
    	nextFire = Time.time + fireRate;

		// Instantiate the projectile
    	GameObject clone = (GameObject)Instantiate ( projectile, aimCompass.position, aimCompass.rotation);
    
    	Physics.IgnoreCollision(clone.collider, this.gameObject.collider); 				// it works but the distance it s lame
 	// clone.transform.Translate( Vector3( 0, 1, 0) ); 						// avoid hits between shot & shooter own colliders  

    	clone.name = "Shot";

		// Add speed to the target
	//   if (clone.rigidbody) clone.rigidbody.velocity = aimCompass.TransformDirection (  Vector3.up * shootPower); else
	//	clone.GetComponent<BulletShot>().Fire( aimCompass.up * shootPower, 5); 	// shot with a speed of 5 & a rotation of 10
		clone.GetComponent<BulletShot>().FireAnimated( aimCompass.up * shootPower, 2, 2, 2); 	// shot with a short animation
//	}	
//	else 
//    {
// 		nextFire = Time.time + fireRate;
//		GameObject simpleClone = Instantiate ( projectile, thisTransform.position, thisTransform.rotation);
// 		Physics.IgnoreCollision(simpleClone.collider, this.gameObject.collider); 				 
//	   	simpleClone.name = "Shot";
//		simpleClone.GetComponent<BulletShot>().FireAnimated( Vector3( orientation * shootPower,0,0), 2, 2, 2); 	 
// 	}
}


public void Paralize()
{
    Debug.Log("Oh my GOD, el pombero está silvando");
    velocity = Vector3.zero;
    enemyState = ShooterState.Stunned;
    //StartCoroutine(Freeze());
}

//IEnumerator Freeze()
//{
//    float TimeLapse = Time.time + 10;
//    enabled = false;
//    float OriginalPos = thisTransform.position.x;

//    while (TimeLapse > Time.time)
//    {
//        thisTransform.position = new Vector3(OriginalPos + Mathf.Sin(TimeLapse * 5) * .15f,
//                                     thisTransform.position.y,
//                                     thisTransform.position.z);
//        yield return 0;
//    }

//    thisTransform.position = new Vector3(OriginalPos, thisTransform.position.y, thisTransform.position.z);
//    enabled = true;
//    yield return 0;
//}

IEnumerator Stunned()																	// "Boleado"
{
	if ( this.CompareTag("pickup") ) 
        yield break;
// 		AnimFlag = false;
	this.tag = "pickup";  													// change tag to a pickable thing for being holded

	float timertrigger= Time.time + stunTime;
	while( timertrigger > Time.time )												// start knock out time counter
	{
		animPlay.PlayFramesFixed( 3, 0, 2, orientation);							// animate Stunning..
		
//	//	if ( transform.parent && linkToPlayerControls.isHoldingObj && !collider.enabled ) 
//	//	if ( thisTransform.IsChildOf( target) && !collider.enabled ) 		// if Player Grabs him change EnemyState > Handled
//		if ( thisTransform.IsChildOf( target) ) 	 
//		{																	
//			thisTransform.position = target.position +  HoldedPosition; 			// Update own hold position & player's too
//	//		linkToPlayerControls.properties.GrabPosition = HoldedPosition; 	
//   			enemyState = ShooterState.Holded ;									// & change enemy state to holded..
//   			moveDirection =	ThrowDirection;
//		}	
		if ( enemyState != ShooterState.Stunned ) yield break;	// so he is not stunned anymore & must quit here
    	yield return 0;
	}
		
	if ( !thisTransform.parent.CompareTag("Player") )								// but if stun time is over & player !isHolding( this)..
	{
		this.tag = "Enemy";  
    	if ( distanceToTarget > AlertRangeX2 )
			enemyState = ShooterState.Sleeping;										// after a while change EnemyState > Default
		else 
			enemyState = ShooterState.Alert;										// or alert if the player is near
	}
    yield return 0;
//	}
}

void  BeingHolded (){
   grounded = true;		
   velocity = Vector3.zero;
   orientation = linkToPlayerControls.orientation;						// update own orientation according player direction
   animPlay.PlayFramesFixed( 1, 1, 1, orientation); 
   
   if ( collider.enabled &&  !thisTransform.parent) 					// if collider returned & we haven't parent anymore..
   {
    grounded = false;
	
	gameObject.tag = "p_shot";		
//	moveDirection = Vector3( Mathf.Sign(orientation) * 5, 3, 0);
	velocity = moveDirection;
	velocity.y += 3.5f * Input.GetAxis("Vertical");				
	velocity.x *= Mathf.Sign(orientation);
   	enemyState = ShooterState.Shooted;												// Then it means we have been shooted...
   }
}





IEnumerator OnTriggerEnter(  Collider other  )											// other.transform.position == target.position
{																		
	if ( other.CompareTag( "Player") )
	{
		if ( gameObject.CompareTag( "Enemy") && (target.position.y > thisTransform.position.y + .1f) )
		{
			linkToPlayerControls.velocity.y = deathForce; 
			Managers.Audio.Play( soundCrash, thisTransform, 6.0f, 1.0f);		
			enemyState = ShooterState.Stunned;
			Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
			
		}
		else if ( Input.GetAxis("Vertical")!=0 )
		{
			Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
		
			Managers.Audio.Play(soundCrash, thisTransform);
			linkToPlayerControls.velocity.y = deathForce; 
		}
 
 
//		if (bounceHit)
//		{
//		audio.clip = bounceHit;
//		audio.Play();
//		}
//		
//		float boxCollider= GetComponent< BoxCollider>() as BoxCollider;
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
	else if ( other.CompareTag("p_shot")  )
	{
        Managers.Register.Score += 100;
		BeatDown();
	}
	else if ( gameObject.CompareTag("p_shot") && !other.CompareTag("Item") )
	{
		yield return new WaitForSeconds(0.01f);
		BeatDown();
	}
}

void  BeatDown (){
    Managers.Register.Score += 100;
    Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);		
	gameObject.tag = "pickup";
	velocity.x *= -.25f;
	velocity.y = 4.5f;
	animPlay.PlayFramesFixed( 1, 1, 1, orientation ); 
	enemyState = ShooterState.Dead;
	Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
	
}
//	else if (  !other.CompareTag("Untagged") && !other.CompareTag("Item")  )
//	{
//		if ( other.CompareTag("p_shot")  ||  gameObject.CompareTag("p_shot")  )
//		{
//			
//			gameObject.tag = "pickup";
//			velocity.x *= -.25;
//			velocity.y = 4.5f;
//   			animPlay.PlayFramesFixed( 1, 1, 1, orientation); 
//			enemyState = ShooterState.Dead;
//		}
//	}
//}

//void OnDrawGizmos()										// toggle the gizmos for designer to see 6 Debug reaching ranges
//{
//	if (gizmoToggle)
//	{
//		Gizmos.color = Color.red;
//		Gizmos.DrawWireSphere( transform.position, alertRange);
//		Gizmos.color = Color.blue;
//		Gizmos.DrawWireSphere( transform.position, attackRange);
//		Gizmos.color = Color.green;
//		Gizmos.DrawLine(  aimCompass.position , aimCompass.TransformPoint ( Vector3.up * 1.5f) );
////		Gizmos.DrawWireSphere( homePosition.position, returnHomeRange);
//	}
//}

}