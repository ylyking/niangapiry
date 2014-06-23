using UnityEngine;
using System.Collections;

public class Piranha : MonoBehaviour {

    public enum EnemyState { StandBy = 0, Roaming, Searching, Hooked, GoHome, Holded, Shooted, Dead }

    public float moveSpeed = 1.5f;		                        // set the speed of the BatiVamp
    public float AttackSpeed = 2.5f;		                        // set the speed of the BatiVamp
    float timeLapse = 0;
    int ReleaseKeys = 0;

    public int orientation          = 1;		                        // set facing direction delay 
    public float attackRange        = 1;		                    // set range for speed increase
    public float searchRange        = 3;		                    // set the range for finding the player
    public float homeRange          = .5f;		                    // set the range for finding the player
    public float deathForce         = 7;		                    // when the player jumps on me force him off 'x' amount
    private float gravity           = 9.8f;	                        // weight of the world pushing enemy down

    Vector3 homePosition            = Vector3.zero;			        // load the home position
    Vector3 moveDirection           = new Vector3(5, 3, 0);	        // Force * direction of throwing the enemy 
    Transform thisTransform;				                // load the player target
    Transform playerTransform;				                // load the player target
    PlayerControls playerControl;
    //PlayerProperties playerProps;
    //CharacterController playerController;

    public bool gizmoToggle         = true;		                //toggle the debug display radius
    AudioClip bounceHit;				            // hot sound for the enemy splat

    public Vector3 HoldedPosition   = new Vector3(0, .3f, -.1f);	// change own enemy position when grabed
    private Vector3 velocity        = Vector3.zero;	            // stroe the enemy movement in velocity (x, y, z)

    public EnemyState currentState = EnemyState.StandBy; // hold current state for setting later
    private AnimSprite aniPlay;							    // Animation component
    private float distanceToTarget  = 0;	                    // get dist to target position

    public bool OnWater = false;
    public bool OnGround = false;

	void Start()
    {
        thisTransform = transform;
        thisTransform.localScale = Vector3.one * .5f;
        homePosition = thisTransform.position + (Vector3.up * .25f);

        aniPlay = GetComponent<AnimSprite>();

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
                            //playerProps = playerTransform.GetComponent<PlayerProperties>();
                            //playerController = playerTransform.GetComponent<CharacterController>();
                            currentState = EnemyState.Roaming;
                        }
                        else yield return 0;
                    }
                    break;

                case EnemyState.Roaming:
                    distanceToTarget = Vector3.Distance(playerTransform.position, thisTransform.position);

                    if (distanceToTarget <= searchRange)
                        currentState = EnemyState.Searching;
                    else
                        Roam();

                    break;

                case EnemyState.Searching:
                    distanceToTarget = Vector3.Distance(playerTransform.position, thisTransform.position);

                    if (distanceToTarget <= searchRange)		            // SI distancia al objetivo es menor que el rango de busqueda..
                        if (distanceToTarget <= attackRange)
                            ChasePlayer(AttackSpeed);                    // si está dentro de rango de ataque..
                        else
                            ChasePlayer(moveSpeed);			                                // sino continuar rastreandolo a velocidad normal
                    else
                            GoHome();						// Volver a casa! ( cambia dirección de lado y avanzar indefinidamente.. 
                    break;

                case EnemyState.Hooked:
                    if (Input.anyKeyDown || Managers.Game.InputLeft || Managers.Game.InputRight
                        || Managers.Game.InputDown || Managers.Game.InputUp || Input.GetButtonDown("Fire1")
                        || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Jump"))
                        ReleaseKeys++;

                    thisTransform.position += (Vector3.up * Mathf.Sin(Time.time * 15) * Time.deltaTime);

                    orientation = playerControl.orientation;
                    aniPlay.PlayFramesFixed(5, 2, 2, orientation, 1);

                    timeLapse -= Time.deltaTime;

                    if (ReleaseKeys > 30 || timeLapse <= 0)
                        BeatDown();

                    break;

                case EnemyState.Holded:
                    thisTransform.position = playerTransform.position + HoldedPosition; 	// Update own hold position & player's too
                    BeingHolded();
                    break;

                case EnemyState.Shooted:
                    if (playerTransform)
                        if (thisTransform.IsChildOf(playerTransform)) 								// check if the player has taken us... 
                            currentState = EnemyState.Holded;										// & change enemy state to holded..

				thisTransform.RotateAround(Vector3.forward, -orientation * 45 * Time.deltaTime);
                    break;

                case EnemyState.Dead:
                    if (playerTransform)
                        if (thisTransform.IsChildOf(playerTransform)) 								// check if the player has taken us... 
                            currentState = EnemyState.Holded;										// & change enemy state to holded..

				thisTransform.RotateAround(Vector3.forward, -orientation * 45 * Time.deltaTime);
                    break;
            }


            if (  (!OnWater &&   (int)currentState < 3) ||  (int)currentState > 5) //&& ((int)enemyState < 3))
                velocity.y -= gravity * Time.deltaTime;

            thisTransform.position += velocity * Time.deltaTime;

            if (thisTransform.position.y < 0 && thisTransform != null)
                Destroy(gameObject, 2);

            yield return 0;
        }
    }


    void Roam()
    {
        aniPlay.PlayFramesFixedFPS(5, 2, 2, orientation, 1, 5);
        
        RaycastHit hit;

        //Debug.DrawLine(thisTransform.position, thisTransform.TransformPoint(Vector3.right), Color.red);
        if (Physics.Linecast(thisTransform.position, thisTransform.TransformPoint(-Vector3.right), out hit) &&
            hit.collider.gameObject.tag == "Untagged" && hit.collider.gameObject.layer != 11)
            orientation = 1;

        //Debug.DrawLine(thisTransform.position, thisTransform.TransformPoint(Vector3.left), Color.red);
        if (Physics.Linecast(thisTransform.position, thisTransform.TransformPoint(Vector3.right), out hit) &&
            hit.collider.gameObject.tag == "Untagged" && hit.collider.gameObject.layer != 11)
            orientation = -1;

        if (OnWater)
        {
            velocity.x = 2 * orientation ;
            velocity.y = Mathf.Sin(Time.time * 3) * .35f;
        }
        else
        {
            //Debug.DrawLine(thisTransform.TransformPoint(Vector3.down * .4f), thisTransform.TransformPoint(Vector3.down * .5f), Color.blue);
            if ((int)currentState < 3 
                && Physics.Linecast(thisTransform.position, thisTransform.TransformPoint(Vector3.down * .5f), out hit)
                && hit.collider.gameObject.tag == "Untagged" && hit.collider.gameObject.layer != 11)
                    velocity.y = 2;

                velocity.x = 1 * orientation;
        }
    }

    void ChasePlayer(float speed)
    {
        if (timeLapse < Time.time)
        {
            timeLapse = Time.time + 1.5f;
            orientation = (int)Mathf.Sign(playerTransform.position.x - thisTransform.position.x);
        }

        aniPlay.PlayFramesFixedFPS(5, 2, 2, orientation, 1, 18);


        if (OnWater)
        {
           Vector3 Distance = playerTransform.position - thisTransform.position;

            float Displacement = speed * Time.deltaTime;

            //if (Displacement >= Distance.magnitude)
            //{
            //    thisTransform.position = playerTransform.position;
            //}
            //else
            //{
                thisTransform.position += (Distance.normalized * Displacement);
                thisTransform.position += (Vector3.up * Mathf.Sin(Time.time * 5) * Time.deltaTime); // else Swim to there..
            //}

            if (playerTransform.position.y > thisTransform.position.y + .5f)      // Gran Salto Bajo Agua!
                velocity.y = 4;

        }
        else
        {
            RaycastHit hit;

            if ((int)currentState < 3
                && Physics.Linecast(thisTransform.position, thisTransform.TransformPoint(Vector3.down * .5f), out hit)
                && hit.collider.gameObject.tag == "Untagged" && hit.collider.gameObject.layer != 11)
            {
                velocity.y = 2;
            }

            velocity.x = 2 * orientation ;
        }
    }

    void GoHome() 
    {
        orientation = (int)Mathf.Sign(homePosition.x - thisTransform.position.x);
        aniPlay.PlayFramesFixedFPS(5, 2, 2, orientation, 1, 14);
        

        if (OnWater)
        {
            velocity = Vector3.zero;

            Vector3 Distance = homePosition - thisTransform.position;
            float Displacement = 2 * Time.deltaTime;

            if (Displacement >= Distance.magnitude)
            {
                thisTransform.position = homePosition;
                currentState = EnemyState.Roaming;
            }
            else
                thisTransform.position += (Distance.normalized * Displacement);

        }
        else
        {
            RaycastHit hit;

            if ((int)currentState < 3
                && Physics.Linecast(thisTransform.position, thisTransform.TransformPoint(Vector3.down * .5f), out hit)
                && hit.collider.gameObject.tag == "Untagged" && hit.collider.gameObject.layer != 11)
            {
                velocity.y = 2;
            }

            velocity.x = 2 * orientation ;


            if ( Mathf.Abs( homePosition.x - thisTransform.position.x) < homeRange)
            {
                thisTransform.position = homePosition;
                currentState = EnemyState.Roaming;
            }
        }
    }

    public void Paralize()
    {
        velocity = Vector3.zero;

        if ((int)currentState < 4)
        {
            StopAllCoroutines();
            StartCoroutine(Freeze());
        }
    }

    IEnumerator Freeze()
    {
        float TimeLapse = Time.time + 20;
        float OriginalPos = thisTransform.position.x;


        if (currentState == EnemyState.Hooked)
        {
            //gameObject.tag = "pickup";
            thisTransform.parent = null;
            currentState = EnemyState.Dead;
            BeatDown();
        }

        while (TimeLapse > Time.time)
        {
            thisTransform.position = new Vector3(OriginalPos + (Mathf.Sin(Time.time * 50) * .05f),
                                                                            thisTransform.position.y,
                                                                            thisTransform.position.z);
            if ((int)currentState > 4)
            {
                if (gameObject.tag == "pickup" && currentState != EnemyState.Dead)
                    currentState = EnemyState.Dead;
                //Debug.Log("Palomet DesParalized!");

                StartCoroutine(CoUpdate());
                yield break;
            }

            yield return 0;
        }

        thisTransform.position = new Vector3(OriginalPos, thisTransform.position.y, thisTransform.position.z);

        gameObject.tag = "Enemy";
        StartCoroutine(CoUpdate());

        velocity = Vector3.zero;
        yield return 0;
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.layer == 4)
        {
            OnWater = true;

            if ((int)currentState > 5)
            {
                currentState = EnemyState.Searching;
                thisTransform.tag = "Enemy";
                thisTransform.parent = Managers.Tiled.MapTransform;
                thisTransform.rotation = Quaternion.Euler(Vector3.zero);
            }
        }

        if (hit.CompareTag("Player"))
        {
            if (gameObject.CompareTag("Enemy"))
                if (currentState != EnemyState.Hooked && playerTransform.position.y > thisTransform.position.y + .25f)
                {
                    playerControl.velocity.y = deathForce;
                    this.tag = "Untagged";
                    velocity.x *= -.25f;
                    velocity.y = 4.5f;
                    aniPlay.PlayFramesFixed(5, 4, 1, orientation, 1);

                    currentState = EnemyState.Dead;
                    //yield return new WaitForSeconds(0.5f);
                    this.tag = "pickup";
                    //Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);
                    //Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);
                }
                else if (Input.GetAxis("Vertical") != 0)
                {
                    //Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);

                    //Managers.Audio.Play(soundCrash, thisTransform);
                    playerControl.velocity.y = deathForce;
                }
                else if (currentState == EnemyState.Searching) // && playerController.isGrounded)
                {
                    ReleaseKeys = 0;
                    velocity = Vector3.zero;
                    timeLapse = Random.Range(5, 15);
                    thisTransform.parent = playerTransform;
                    thisTransform.localPosition = Vector3.down * .2f + Vector3.forward * .05f;
                    this.tag = "Enemy";
                    currentState = EnemyState.Hooked;
                }

        }
        else if (hit.CompareTag("p_shot"))
        {
            Managers.Register.Score += Random.Range(1, 99);
            BeatDown();
        }
        else if (gameObject.CompareTag("p_shot") && !hit.CompareTag("Item"))
        {
            //yield return new WaitForSeconds(0.01f);
            Managers.Register.Score += 50;
            BeatDown();
        }
        //yield return 0;

    }


    void OnTriggerStay(Collider hit)
    {
        if (hit.gameObject.layer == 4)
            OnWater =  true;
    }

    void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.layer == 4)
            OnWater = false;
    }


    void BeingHolded()
    {
        velocity = Vector3.zero;
        orientation = playerControl.orientation;						// update own orientation according player direction
        aniPlay.PlayFramesFixed(5, 4, 1, orientation, 1);

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
        //Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);
        //Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);
        thisTransform.parent = null;
        gameObject.tag = "pickup";
        velocity.x *= -.25f;
        velocity.y = 4.5f;
        aniPlay.PlayFramesFixed(5, 4, 1, orientation, 1);
        currentState = EnemyState.Dead;
    }




    void OnDrawGizmos()		// toggle the gizmos for designer to see 6 Debug reaching ranges
    {
        if (gizmoToggle)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, searchRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(homePosition, homeRange);

        }
    }

}
