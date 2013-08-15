// Enemy ControllerColliderHit
// Description: Control component enemy shooter logic, and properties for gumba

using UnityEngine;
using System.Collections;

public class MonoShooter : MonoBehaviour {
public ShooterState enemyState = ShooterState.Sleeping ;	// set default starting state

public float aimDamping 		= 4.0f;			// speed & time taken for aiming
public float fireRate		 	= 2.0f;			// delay between shots, more lower, more harder to avoid
public float shootPower 		= 9;			// the power or speed of the projectile.
public float deathForce 		= 8.0f;			// when the player jumps on me force him off 'x' amount

public bool  gizmoToggle		= true;			// toggle the debug display radius
public AudioClip bounceHit;					// hot sound for the enemy splat

private int orientation 			= -1;			// orientation of the enemy
private float nextFire 		        = 3.0f;			// delay between attacks
private float gravity 		        = 9.8f;			// weight of the world pushing enemy down
//private float stunTime 		        = 1.0f;			// enemy knocked out time...

public 	float alertRange 		    = 2.0f;			// set the range for finding the player
public 	float attackRange 		    = 5.0f;			// set range for speed increase
private float distanceToTarget 		= 0.0f;			// the dist to target position stored in square mag.

private float AlertRangeX2			= 0;
private float AttackRangeX2			= 0; 

//private bool  Holded 		        = false;
private bool  AnimFlag 		        = true;
private bool  grounded		        = true;

public 	Vector3 HoldedPosition      = new Vector3( 0,.3f,-.1f);	// change own enemy position when grabed
public  Vector3 moveDirection       =  new Vector3( 5, 3,  0);	// Force * direction of throwing the enemy 
private Vector3 velocity            = Vector3.zero;		// store the enemy movement in velocity (x, y, z)

public Transform target;					// target to search ( the player )
public Transform aimCompass;					// it's a simple child transform inside this gameObject
private Transform thisTransform;					// own enemy's tranform cached

public GameObject projectile;					// Prefab to be shooted ( set previously it's input )
private AnimSprite animPlay; 					// : Component
private PlayerControls linkToPlayerControls;

public AudioClip soundCrash;
public AudioClip soundAttack;
public GameObject ParticleStars;


void  Start (){
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
	
		
	animPlay = GetComponent<AnimSprite>();
	
 
    StartCoroutine(CoUpdate());
}


void  OnBecameVisible (){
    enabled = true;
}

void  OnBecameInvisible (){
    enabled = false;
}

IEnumerator CoUpdate()	                                                        // void  Update (){
{
    while (thisTransform )
    {
        if (!target)
        {
            if (Managers.Game.PlayerPrefab)
            {
                target = Managers.Game.PlayerPrefab.transform;
                linkToPlayerControls = (PlayerControls)target.GetComponent<PlayerControls>();
            }
            else
                yield return 0;
        }

        if (target)
        if ( thisTransform.IsChildOf(target)) 									// check if the player has taken us... 
        {
            thisTransform.position = target.position + HoldedPosition; 			// Update own hold position & player's too
            enemyState = ShooterState.Holded;										// & change enemy state to holded..
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

                StartCoroutine( Search() );														// Begin Searching of the player & attack him
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
                thisTransform.RotateAroundLocal(Vector3.forward, -orientation * 2 * Time.deltaTime);

                break;

            case ShooterState.Dead:			// 5# ENEMY IS DEAD
                animPlay.PlayFramesFixed(1, 1, 1, orientation);
                thisTransform.RotateAroundLocal(Vector3.forward, -orientation * 45 * Time.deltaTime);
                grounded = false;

                //				Destroy(gameObject, 15);											// Keep falling and die after a while
                break;
        }

        if (!grounded) velocity.y -= gravity * Time.deltaTime;
        thisTransform.position += velocity * Time.deltaTime;

        if (thisTransform.position.y < 0 && thisTransform != null) Destroy(gameObject, 2);	// If character falls get it up again 

        yield return 0;
    }
	
}

IEnumerator  Sleeping (){
	if( AnimFlag)
 	{
		velocity = Vector3.zero;
		animPlay.PlayFrames( 0, 0, 2, orientation, 2);							// Do zzZZZ Animation
	

 		if (  distanceToTarget <= AlertRangeX2 )
 		{
			AnimFlag = false;
 			float groundPosition= thisTransform.position.y;
 			float timertrigger= Time.time + 0.55f;
 			
			while( timertrigger > Time.time )
			{
//				thisTransform.Translate( 0, 1.5f * TimeLapse.deltaTime, 0);
                thisTransform.position = new Vector3(thisTransform.position.x,
                                                        groundPosition + Mathf.Sin((timertrigger - Time.time) * 5) * .5f,
                                                        thisTransform.position.z);
				animPlay.PlayFramesFixed(1 , 0, 1, orientation); 				// Do Wake up Animation
			
				if ( enemyState != ShooterState.Sleeping)
                {
                    thisTransform.position = new Vector3(thisTransform.position.x, groundPosition, thisTransform.position.z);
                    AnimFlag = true; yield break; 
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
//   
    		Shoot();		
    		float timerHolder= Time.time + .15f;
			while( timerHolder > Time.time )									// do some throw animation for a while 
			{
    			animPlay.PlayFramesFixed( 2, 0, 2, orientation); 				// Do Throw Animation
                if (enemyState != ShooterState.Alert) { AnimFlag = true; yield break; }
    			yield return 0;
			}
    		
			float timertrigger= Time.time + .35f;
			while( timertrigger > Time.time )									// do some throw animation for a while 
			{
    			animPlay.PlayFramesFixed( 2, 2, 2, orientation); 				// Do Throw Animation
                if (enemyState != ShooterState.Alert) { AnimFlag = true; yield break; }
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

		clone.GetComponent<BulletShot>().FireAnimated( aimCompass.up * shootPower, 0, 2, 1); 	// shot with a short animation

}

//function Stunned()																	// "Boleado"
//{
//   			animPlay.PlayFramesFixed( 1, 1, 1, orientation); 
//
//			yield return new WaitForSeconds(0.5f);
//
//			this.tag = "pickup";
//			velocity.x *= -.25;
//			velocity.y = 4.5f;
//			enemyState = ShooterState.Dead;
//
//}

void  BeingHolded ()
{
   grounded = true;		
   velocity = Vector3.zero;
   orientation = linkToPlayerControls.orientation;						// update own orientation according player direction
   animPlay.PlayFramesFixed( 1, 1, 1, orientation); 
   
   if ( collider.enabled &&  !thisTransform.parent) 					// if collider returned & we haven't parent anymore..
   {
    grounded = false;
		
	velocity = moveDirection;
	velocity.y += 3.5f * Input.GetAxis("Vertical");				
	velocity.x *= Mathf.Sign(orientation);
   	enemyState = ShooterState.Shooted;												// Then it means we have been shooted...
    gameObject.tag = "p_shot";	
   }
}



public void Paralize()
{
    velocity = Vector3.zero;

    if ((int)enemyState < 3)
    {
        StopAllCoroutines();
        StartCoroutine(Freeze());
    }
}

IEnumerator Freeze()
{
    float TimeLapse = Time.time + 20;
    float OriginalPos = thisTransform.position.x;

    while (TimeLapse > Time.time)
    {
        thisTransform.position = new Vector3(OriginalPos + (Mathf.Sin(Time.time * 50) * .05f),
                                                                        thisTransform.position.y,
                                                                        thisTransform.position.z);
        if ((int)enemyState > 1)
        {
            if (gameObject.tag == "pickup" && enemyState != ShooterState.Dead)
                enemyState = ShooterState.Dead;

            StartCoroutine(CoUpdate());
            yield break;
        }

        yield return 0;
    }

    thisTransform.position = new Vector3(OriginalPos, thisTransform.position.y, thisTransform.position.z);

    //if (gameObject.tag == "pickup")
    //    gameObject.tag = "Enemy";
    StartCoroutine(CoUpdate());

    //rigidbody.velocity = Vector3.zero;
    velocity = Vector3.zero;
    yield return 0;
}




IEnumerator OnTriggerEnter(  Collider other  )											// other.transform.position == target.position
{																		
	if ( other.CompareTag( "Player") )
	{
		if ( gameObject.CompareTag( "Enemy") && (target.position.y > thisTransform.position.y + .1f) )
		{
			linkToPlayerControls.velocity.y = deathForce; 
			this.tag = "Untagged";
			velocity.x *= -.25f;
			velocity.y = 4.5f;
   			animPlay.PlayFramesFixed( 1, 1, 1, orientation); 
			enemyState = ShooterState.Dead;
			yield return new WaitForSeconds( 0.5f);
			this.tag = "pickup";
			Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);
			Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
			
			
//			enemyState = ShooterState.Stunned;
		}
		else if ( Input.GetAxis("Vertical")!= 0 )
		{
			Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);

            Managers.Audio.Play(soundCrash, thisTransform);
			linkToPlayerControls.velocity.y = deathForce; 
		}
 
	}
	else if ( other.CompareTag("p_shot") )
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

void  BeatDown ()
{
    Managers.Register.Score += 5 * Random.Range(1, 99);
    Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);
	gameObject.tag = "pickup";
	velocity.x *= -.25f;
	velocity.y = 4.5f;
	animPlay.PlayFramesFixed( 1, 1, 1, orientation ); 
	enemyState = ShooterState.Dead;
	Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
	
}

//public void Paralize()
//{
//    Debug.Log("Oh my GOD, el pombero está silvando");
//    velocity = Vector3.zero;
//    enemyState = ShooterState.Dead;
//    //StartCoroutine(Freeze());
//}

}