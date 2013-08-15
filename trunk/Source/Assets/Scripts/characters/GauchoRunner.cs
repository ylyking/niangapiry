// Enemy ControllerColliderHit
// Description: Control component enemy shooter logic, and properties for gumba


using UnityEngine;
using System.Collections;



public class GauchoRunner : MonoBehaviour {


public ShooterState enemyState = ShooterState.Sleeping ;	// set default starting state

public float aimDamping 		= 4.0f;			// speed & time taken for aiming
public float fireRate		 	= 2.0f;			// delay between shots, more lower, more harder to avoid
public float shootPower 		= 10;			// the power or speed of the projectile.
public float deathForce 		= 12.0f;		// when the player jumps on me force him off 'x' amount

public bool  gizmoToggle		= true;			// toggle the debug display radius
public AudioClip bounceHit;					    // hot sound for the enemy splat

private int orientation 		= -1;			// orientation of the enemy
private float nextFire          = 3;			// delay between attacks
private float nextJump          = 3;			// delay between attacks
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
//private int layerMask = 1 << 8;



	
void  Start (){
//	AimCompass = this.gameObject.GetComponentInChildren< Transform >(); // not working
//	controller = this.GetComponent ( CharacterController );
	thisTransform = transform;

    alertRange = Random.Range(3, 10);

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
                yield break;
                
        }

        if (target)
            if (thisTransform.IsChildOf(target)) 									// check if the player has taken us... 
            {
                thisTransform.position = target.position + HoldedPosition; 			// Update own hold position & player's too
                enemyState = ShooterState.Holded;										// & change enemy state to holded..
            }
            else
                distanceToTarget = (target.position - thisTransform.position).sqrMagnitude;	// it's much faster using Square mag. 

        
        if ( grounded)
            velocity.y = 0;
        //velocity = Vector3.zero;

        //Debug.DrawLine(thisTransform.TransformPoint(Vector3.down * .4f), thisTransform.TransformPoint(Vector3.down * .5f), Color.blue);
        if ((int)enemyState < 3)
            //grounded = Physics.Linecast(thisTransform.position, thisTransform.TransformPoint(Vector3.down * .5f));
            grounded = Physics.Linecast(thisTransform.TransformPoint(Vector3.down * .4f), thisTransform.TransformPoint(Vector3.down * .5f));

            switch (enemyState)														// check states of the character:
            {
                case ShooterState.Sleeping:		// 0# GAUCHO SLEEPING:		
                    velocity.x = 0;
                    StartCoroutine(Sleeping());
                    break;

                case ShooterState.Alert:		// 1# ALERT STATE:
                    //velocity = Vector3.zero;
                    StartCoroutine(Search());														// Begin Searching of the player & attack him
                    break;

                case ShooterState.Stunned: 		// 2# KNOCK OUT:
                    velocity.x = 0;
                    StartCoroutine(Stunned());
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


            if (!grounded)  //&& ((int)enemyState < 3))
                velocity.y -= gravity * Time.deltaTime;
            
        thisTransform.position += velocity * Time.deltaTime;

        if (thisTransform.position.y < 0 && thisTransform != null) 
            Destroy(gameObject, 2);	                                // If character falls get it up again 

        yield return 0;
    }
	
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

IEnumerator  Search ()
{
    //orientation = (int)Mathf.Sign( target.position.x - thisTransform.position.x  );
	
    if ( distanceToTarget <= AttackRangeX2 )									// if player's near enemy -> Aim & Attack him
    {	
        animPlay.PlayFrames( 3, 2, 2, orientation, 16);			// Do Running Animation


//#if UNITY_EDITOR
        //velocity.x = 450 * orientation * Time.deltaTime;
        velocity.x = 2.5f * orientation;
//#else
//        //velocity.x = 900 * orientation * Time.deltaTime;
//        velocity.x = 4 * orientation;
//#endif


        if (Time.time > nextFire)												// wait some time between shots 
            ReOrientate();


        if ( Time.time > nextJump)
            Jump();

        //Debug.DrawLine(thisTransform.position, thisTransform.TransformPoint(-Vector3.right), Color.blue);
        //Debug.DrawLine(thisTransform.position, thisTransform.TransformPoint(Vector3.right), Color.blue);

        RaycastHit hit;
        if (Physics.Linecast(thisTransform.position, thisTransform.TransformPoint(-Vector3.right), out hit) &&
             hit.collider.gameObject.tag == "Untagged" && hit.collider.gameObject.layer != 11)
                    orientation = 1;

        if (Physics.Linecast(thisTransform.position, thisTransform.TransformPoint(Vector3.right), out hit) &&
            hit.collider.gameObject.tag == "Untagged" && hit.collider.gameObject.layer != 11)
                    orientation = -1;

    }
    else enemyState = ShooterState.Sleeping;								// else if player it's out enemy range -> go Sleep
    yield return 0;
}

void  ReOrientate ()
{
    nextFire = Time.time + 2;
    orientation = (int)Mathf.Sign(target.position.x - thisTransform.position.x);
}

void Jump()
{
    nextJump = Time.time + Random.Range(1, 4);

    if (thisTransform.position.y + .5f  < target.position.y )
    {   velocity.y = 5;
        grounded = false;
    }
}



IEnumerator Stunned()																	// "Boleado"
{
	if ( this.CompareTag("pickup") ) 
        yield break;
	this.tag = "pickup";  													// change tag to a pickable thing for being holded

	float timertrigger= Time.time + stunTime;
	while( timertrigger > Time.time )												// start knock out time counter
	{
		animPlay.PlayFramesFixed( 3, 0, 2, orientation);							// animate Stunning..
		
		if ( enemyState != ShooterState.Stunned ) 
            yield break;	// so he is not stunned anymore & must quit here
    	    
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
	velocity = moveDirection;
	velocity.y += 3.5f * Input.GetAxis("Vertical");				
	velocity.x *= Mathf.Sign(orientation);
   	enemyState = ShooterState.Shooted;												// Then it means we have been shooted...
   }
}





void  BeatDown ()
{
    grounded = false;
    Managers.Register.Score += 10 * Random.Range(1, 15);
    Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);		
	gameObject.tag = "pickup";
	velocity.x *= -.25f;
	velocity.y = 4.5f;
	animPlay.PlayFramesFixed( 1, 1, 1, orientation ); 
	enemyState = ShooterState.Dead;
	Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
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
    rigidbody.isKinematic = true;
    float OriginalPos = thisTransform.position.x;

    while (TimeLapse > Time.time)
    {
        thisTransform.position = new Vector3(OriginalPos + (Mathf.Sin(Time.time * 50) * .05f),
                                                                        thisTransform.position.y,
                                                                        thisTransform.position.z);

        if ((int)enemyState > 1  )
        {
            if (gameObject.tag == "pickup" && enemyState != ShooterState.Dead)
                gameObject.tag = "Enemy";
            StartCoroutine(CoUpdate());
            rigidbody.isKinematic = true;

            yield break;
        }

        yield return 0;
    }

    thisTransform.position = new Vector3(OriginalPos, thisTransform.position.y, thisTransform.position.z);
    rigidbody.isKinematic = false;

    if (gameObject.tag == "pickup" && enemyState != ShooterState.Dead)
        gameObject.tag = "Enemy";
    StartCoroutine(CoUpdate());

    rigidbody.velocity = Vector3.zero;
    velocity = Vector3.zero;
    yield return 0;
}


IEnumerator OnTriggerEnter(Collider other)											// other.transform.position == target.position
{
    if (other.CompareTag("Player"))
    {
        if (gameObject.CompareTag("Enemy") && (target.position.y > collider.transform.position.y + .1f)) //  thisTransform.position.y + .1f))
        {
            linkToPlayerControls.velocity.y = deathForce;
            Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);
            enemyState = ShooterState.Stunned;
            animPlay.PlayFramesFixed(3, 0, 2, orientation);							// animate Stunning..
            Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);

        }
        else if (Input.GetAxis("Vertical") != 0)
        {
            Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);

            Managers.Audio.Play(soundCrash, thisTransform);
            linkToPlayerControls.velocity.y = deathForce;
        }
    }
    else if (other.CompareTag("p_shot"))
    {
        BeatDown();
    }
    else if (gameObject.CompareTag("p_shot") && !other.CompareTag("Item"))
    {
        yield return new WaitForSeconds(0.01f);
        BeatDown();
    }
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