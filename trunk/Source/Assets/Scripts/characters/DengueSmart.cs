using UnityEngine;
using System.Collections;

public class DengueSmart : MonoBehaviour 
{
    public enum EnemyState { StandBy = 0, Roaming, Attacking, GoHome, Holded, Shooted, Dead }


public float moveSpeed = 2;		                        // set the speed of the BatiVamp
bool FallingDown = false;
float FallingHeight = 0;
float FallingSpeed = -4;
float timeLapse = 0;
int ReleaseKeys = 0;

public int orientation = 1;		                        // set facing direction delay 
public float attackRange = 3;		                    // set range for speed increase
public float searchRange = 15;		                    // set the range for finding the player
public float deathForce = 7;		                    // when the player jumps on me force him off 'x' amount

public AudioClip bounceHit;							// hot sound for the enemy splat
public AudioClip soundCrash;
public GameObject ParticleStars;

Vector3 homePosition		= Vector3.zero ;			// load the home position
Vector3 moveDirection = new Vector3(5, 3, 0);	// Force * direction of throwing the enemy 
Transform thisTransform		;				            // load the player target
Transform playerTransform	;				            // load the player target
PlayerControls playerControl;

public bool gizmoToggle		= false;		                //toggle the debug display radius

public Vector3 HoldedPosition = new Vector3(0, .3f, -.1f);	// change own enemy position when grabed
private Vector3  velocity   = Vector3.zero;	            // stroe the enemy movement in velocity (x, y, z)
private float gravity = 9.8f;	                    // weight of the world pushing enemy down

private EnemyState currentState = EnemyState.StandBy; // hold current state for setting later
private AnimSprite aniPlay;							    // Animation component
private float distanceToTarget = 0;	                    // get dist to target position


	void Start () 
    {
        thisTransform = transform ;
        thisTransform.position += Vector3.back * 2;
        homePosition = thisTransform.position;

        aniPlay = GetComponent<AnimSprite>();

        searchRange = Random.Range(10, 100);

        StartCoroutine(CoUpdate());
	}


	
    IEnumerator CoUpdate() 
    {
        while (thisTransform)
        {
            switch (currentState)
            {
                case EnemyState.StandBy:
                    if (!playerTransform)
                    {
                        if (Managers.Game.PlayerPrefab)
                        {
                            playerTransform = Managers.Game.PlayerPrefab.transform;			        //	We can Use this system to get the player's Id & position
                            playerControl = playerTransform.GetComponent<PlayerControls>();
                            currentState = EnemyState.Roaming;
                        }
                        else yield return 0;
                    }
                    break;

                case EnemyState.Roaming:
                    if (!playerTransform)
                        break;

                    distanceToTarget = Vector3.Distance(playerTransform.position, thisTransform.position);

                    if (distanceToTarget <= searchRange)		            // SI distancia al objetivo es menor que el rango de busqueda..
                        if (distanceToTarget <= attackRange  && thisTransform.position.y >= playerTransform.position.y + 1)	                    // si está dentro de rango de ataque..
                            currentState = EnemyState.Attacking;			                                // acelerar la velocidad de ataque
                        else
                            ChasePlayer();			                                // sino continuar rastreandolo a velocidad normal
                    else									                        // SINO chequear rutina habitual de comportamiento cuando está solo...
                        GoHome();						                            // Volver a casa! ( cambia dirección de lado y avanzar indefinidamente.. 
                    break;

                case EnemyState.Attacking:
                        AttackPlayer();
                    break;


                case EnemyState.Holded:
                        thisTransform.position = playerTransform.position + HoldedPosition; 	// Update own hold position & player's too
                        BeingHolded();
                    break;

                case EnemyState.Shooted:
                    if (playerTransform)
                        if (thisTransform.IsChildOf(playerTransform)) 								// check if the player has taken us... 
                            currentState = EnemyState.Holded;										// & change enemy state to holded..

                    thisTransform.RotateAroundLocal(Vector3.forward, -orientation * 45 * Time.deltaTime);
                    velocity.y -= gravity * Time.deltaTime;
                    thisTransform.position += velocity * Time.deltaTime;

                    break;

                case EnemyState.Dead:
                    if (playerTransform)
                        if (thisTransform.IsChildOf(playerTransform)) 								// check if the player has taken us... 
                            currentState = EnemyState.Holded;										// & change enemy state to holded..

                    thisTransform.RotateAroundLocal(Vector3.forward, -orientation * 45 * Time.deltaTime);
                    velocity.y -= gravity * Time.deltaTime;
                    thisTransform.position += velocity * Time.deltaTime;

                    if (thisTransform.position.y < 0 && thisTransform != null)                      // If player was killeds
                    {
                        timeSpawn -= Time.deltaTime;

                        if (timeSpawn <= 0)
                        {
                            timeSpawn = Random.Range(1, 3);

                            velocity = Vector3.zero;
                            thisTransform.rotation = Quaternion.Euler(Vector3.zero);
                            gameObject.tag = "Enemy";
                            thisTransform.parent = Managers.Tiled.MapTransform;
                            currentState = EnemyState.Roaming;
                            thisTransform.position = homePosition;
                            Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);
                            searchRange = Random.Range(10, 100);

                            Debug.Log("Dengue Respawned");
                        }
                    }
                        //Destroy(gameObject, 2);	// If character falls get it up again 

                    break;
            }

            yield return 0;
        }
	}

    float timeSpawn = 2;

    void ChasePlayer()
    {
        orientation = (int)Mathf.Sign(playerTransform.position.x - thisTransform.position.x);
        aniPlay.PlayFramesFixed(3, 0, 4, orientation);
        
       
        Vector3 Distance = playerTransform.position + (Vector3.up) - thisTransform.position;
        float Displacement = moveSpeed * Time.deltaTime;


        thisTransform.position += (Distance.normalized * Displacement);
        thisTransform.position += (Vector3.up * Mathf.Sin(Time.time * 5) * Time.deltaTime);

    }

    void GoHome()
    {
        orientation = (int)Mathf.Sign(homePosition.x - thisTransform.position.x);
        aniPlay.PlayFramesFixed(3, 0, 4, orientation);

        Vector3 Distance = homePosition - thisTransform.position;
        float Displacement = 1 * Time.deltaTime;

        if (Displacement >= Distance.magnitude)
        {
            thisTransform.position = homePosition;
        }
        else
        {
            thisTransform.position += (Distance.normalized * Displacement);
            thisTransform.position += (Vector3.up * Mathf.Sin(Time.time * 5) * Time.deltaTime);
        }
    }

    void AttackPlayer()
    {
        if (!FallingDown)
        {
            FallingDown = true;
            FallingHeight = thisTransform.position.y + .25f;
            FallingSpeed = -4;
            orientation = (int)Mathf.Sign(playerTransform.position.x - thisTransform.position.x);
        }

        thisTransform.position += new Vector3( Time.deltaTime * 3 * orientation, FallingSpeed * Time.deltaTime, 0);
        FallingSpeed += 5 * Time.deltaTime;
        aniPlay.PlayFramesFixed(3, 0, 4, orientation);
        
        

        if (FallingDown && thisTransform.position.y > FallingHeight)
        {
            FallingDown = false;
            FallingSpeed = -7;
            FallingHeight = 0;
            currentState = EnemyState.Roaming;
        }

    }

    void BeingHolded()
    {
        velocity = Vector3.zero;
        orientation = playerControl.orientation;						// update own orientation according player direction
        aniPlay.PlayFramesFixed(0, 3, 1, orientation);
        

        if (collider.enabled && !thisTransform.parent) 					// if collider returned & we haven't parent anymore..
        {
            velocity = moveDirection;
            velocity.y += 3.5f * Input.GetAxis("Vertical");
            velocity.x *= Mathf.Sign(orientation);
            currentState = EnemyState.Shooted;												// Then it means we have been shooted...
            this.tag = "p_shot";
        }
    }

    void BeatDown()
    {
        Managers.Register.Score += 50;
        Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);
        Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);
        gameObject.tag = "pickup";
        velocity.x *= -.25f;
        velocity.y = 4.5f;
        aniPlay.PlayFramesFixed(0, 3, 1, orientation);
        currentState = EnemyState.Dead;
    }

    public void Paralize()
    {
        velocity = Vector3.zero;

        if ((int)currentState < 3)
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
            if ((int)currentState > 4)
            {
                if (gameObject.tag == "pickup" && currentState != EnemyState.Dead)
                    currentState = EnemyState.Dead;

                StartCoroutine(CoUpdate());
                yield break;
            }

            yield return 0;
        }

        thisTransform.position = new Vector3(OriginalPos, thisTransform.position.y, thisTransform.position.z);

        StartCoroutine(CoUpdate());

        velocity = Vector3.zero;
        yield return 0;
    }




    IEnumerator OnTriggerEnter(Collider other)											// other.transform.position == target.position
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("Enemy") && (playerTransform.position.y > thisTransform.position.y + .1))
            {
                playerTransform.GetComponent<PlayerControls>().velocity.y = deathForce;
                this.tag = "Untagged";
                velocity.x *= -.25f;
                velocity.y = 4.5f;
                aniPlay.PlayFramesFixed(0, 3, 1, orientation);
                currentState = EnemyState.Dead;
                yield return new WaitForSeconds(0.5f);
                this.tag = "pickup";
                Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);

                Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);
            }
            else if (Input.GetAxis("Vertical") != 0)
            {
                Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);

                Managers.Audio.Play(soundCrash, thisTransform);
                playerTransform.GetComponent<PlayerControls>().velocity.y = deathForce;
            }

        }
        else if (other.CompareTag("p_shot"))
        {
            Managers.Register.Score += Random.Range(1, 99);
            BeatDown();
        }
        else if (gameObject.CompareTag("p_shot") && !other.CompareTag("Item"))
        {
            yield return new WaitForSeconds(0.01f);
            BeatDown();
        }
        yield return 0;

    }





    void OnDrawGizmos()		// toggle the gizmos for designer to see 6 Debug reaching ranges
    {
        if (gizmoToggle)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, searchRange);

        }
    }


}
