
using UnityEngine;
using System.Collections;

public class DengueFly : MonoBehaviour
{

    public ShooterState enemyState = ShooterState.Alert;	// set default starting state

    public float aimDamping = 4.0f;					// speed & time taken for aiming

    public AudioClip bounceHit;							// hot sound for the enemy splat
    public float deathForce = 8.0f;			// when the player jumps on me force him off 'x' amount

    private int orientation = -1;					// orientation of the enemy
    private float gravity = 9.8f;					// weight of the world pushing enemy down

    //private bool Holded = false;
    //private bool AnimFlag = true;

    public Vector3 HoldedPosition = new Vector3(0, .3f, -.1f);	// change own enemy position when grabed
    public Vector3 moveDirection = new Vector3(5, 3, 0);	// Force * direction of throwing the enemy 
    private Vector3 velocity = Vector3.zero;			// store the enemy movement in velocity (x, y, z)

    public Transform target;							// target to search ( the player )
    private Transform thisTransform;							// own enemy's tranform cached

    private AnimSprite animPlay; 							// : Component
    private PlayerControls linkToPlayerControls;
    private bool grounded = true;

    public AudioClip soundCrash;
    //public AudioClip soundAttack;
    public GameObject ParticleStars;


    void Start(){
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
			linkToPlayerControls = target.GetComponent<PlayerControls>() as PlayerControls;
	}else 	print(" Beware Dengue target empty: player link not found!");
			
	animPlay = GetComponent<AnimSprite>();

    //while( true)
    //    yield return new CoUpdate();
    StartCoroutine(CoUpdate());
}

    void OnBecameVisible()
    {
        enabled = true;
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }

    IEnumerator CoUpdate()
    {
        while (true)
        {
            if (thisTransform.IsChildOf(target)) 									// check if the player has taken us... 
            {
                thisTransform.position = target.position + HoldedPosition; 			// Update own hold position & player's too
                enemyState = ShooterState.Holded;										// & change enemy state to holded..
            }

            switch (enemyState)														// check states of the character:
            {
                case ShooterState.Alert:		// 1# ALERT STATE:
                    animPlay.PlayFramesFixed(3, 0, 4, orientation);
                    velocity = Vector3.zero;
                    velocity.x = Mathf.Sin(Time.time);
                    orientation = (int)Mathf.Sign(Mathf.Sin(Time.time));
                    break;

                case ShooterState.Holded: 		// 3# ENEMY IS TAKEN	
                    BeingHolded();

                    break;

                case ShooterState.Shooted: 		// 4# ENEMY IS THROWED IN THE AIR
                    thisTransform.RotateAroundLocal(Vector3.forward, -orientation * 2 * Time.deltaTime);

                    break;

                case ShooterState.Dead:			// 5# ENEMY IS DEAD
                    animPlay.PlayFramesFixed(0, 3, 1, orientation);
                    thisTransform.RotateAroundLocal(Vector3.forward, -orientation * 45 * Time.deltaTime);
                    grounded = false;
                    break;
            }

            if (!grounded) velocity.y -= gravity * Time.deltaTime;
            thisTransform.position += velocity * Time.deltaTime;

            if (thisTransform.position.y < 0) Destroy(gameObject, 2);	// If character falls get it up again 

            yield return 0;
        }

    }


    void BeingHolded()
    {
        grounded = true;
        velocity = Vector3.zero;
        orientation = linkToPlayerControls.orientation;						// update own orientation according player direction
        animPlay.PlayFramesFixed(0, 3, 1, orientation);

        //Debug.Log("BeingHolded called!");

        if (collider.enabled && !thisTransform.parent) 					// if collider returned & we haven't parent anymore..
        {
            grounded = false;

            velocity = moveDirection;
            velocity.y += 3.5f * Input.GetAxis("Vertical");
            velocity.x *= Mathf.Sign(orientation);
            enemyState = ShooterState.Shooted;												// Then it means we have been shooted...
            this.tag = "p_shot";
        }
    }





    IEnumerator OnTriggerEnter(Collider other)											// other.transform.position == target.position
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("Enemy") && (target.position.y > thisTransform.position.y + .1))
            {
                linkToPlayerControls.velocity.y = deathForce;
                this.tag = "Untagged";
                velocity.x *= -.25f;
                velocity.y = 4.5f;
                animPlay.PlayFramesFixed(0, 3, 1, orientation);
                enemyState = ShooterState.Dead;
                yield return new WaitForSeconds(0.5f);
                this.tag = "pickup";
                Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);

                Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);
                //			enemyState = ShooterState.Stunned;
            }
            else if (Input.GetAxis("Vertical")!=0)
            {
                Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);

                Managers.Audio.Play(soundCrash, thisTransform);
                linkToPlayerControls.velocity.y = deathForce;
            }

        }
        else if (other.CompareTag("p_shot"))
        {
            Managers.Game.Score += 75;
            BeatDown();
        }
        else if (gameObject.CompareTag("p_shot") && !other.CompareTag("Item"))
        {
            yield return new WaitForSeconds(0.01f);
            BeatDown();
        }
        yield return 0;

    }

    void BeatDown()
    {
        Managers.Game.Score += 50;
        Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);
        Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);
        gameObject.tag = "pickup";
        velocity.x *= -.25f;
        velocity.y = 4.5f;
        animPlay.PlayFramesFixed(0, 3, 1, orientation);
        enemyState = ShooterState.Dead;
    }

}