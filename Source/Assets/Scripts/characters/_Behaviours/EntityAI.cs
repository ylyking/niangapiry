using UnityEngine;
using System.Collections;


    public class EntityAI : MonoBehaviour          
    {
        public float deathForce = 12;		                    // when the player jumps on me force him off 'x' amount

        [HideInInspector] public float gravity = 9.8f;			    // weight of the world pushing enemy down
        [HideInInspector] public int orientation = -1;			    // orientation of the enemy
        [HideInInspector] public bool grounded = false;

        public Vector3 HoldedPosition = new Vector3(0, .3f, -.1f);	// change own enemy position when grabed
        public Vector3 moveDirection = new Vector3(5, 3, 0);	    // Force * direction of throwing the enemy 
        [HideInInspector] public Vector3 velocity = Vector3.zero;		            // store the enemy movement in velocity (x, y, z)

        public Transform target;					                // target to search ( the player )
        public Transform aimCompass;					            // it's a simple child transform inside this gameObject

        public GameObject projectile;					            // Prefab to be shooted ( set previously it's input )
        [HideInInspector] public AnimSprite animPlay; 					            // : Component
        [HideInInspector] public PlayerControls linkToPlayerControls;

        public AudioClip soundCrash;
        public AudioClip soundAttack;
        public AudioClip bounceHit;				                    // hot sound for the enemy splat
        public GameObject ParticleStars;

        // Strategy Pattern without Components, in Ur Face Unity! suck it down
        protected Behaviours.BehaviourBase ActiveBehaviour = null;  // Behaviour Composition over Enemy

        public enum EnemyBehaviour { Gaucho, Dengue, Mono, Palometa, Vampi };   // A 
        public EnemyBehaviour StartBehaviour = EnemyBehaviour.Gaucho;

        /// <summary>
        /// EntityAI it's a Behaviour Swapper that works as this:
        /// 
        /// EntityAI ------> BehaviourBase          ( Inherited Behaviour )
        /// (Component)             ^
        ///                         |
        ///                         -------------------------------------------------
        ///                         |               |               |               |
        ///                     GauchoSleep     GauchoSearch    MonoAttack       ..etc...  
        /// 
        /// And That's it, every state Manages their transitions to the next by himself
        /// </summary>

        void Awake()
        {
            if (rigidbody)								
            {
                rigidbody.freezeRotation = true;
                rigidbody.useGravity = false;
            }

            if (!aimCompass)
                aimCompass = transform.FindChild("AimCompass");

            animPlay = (AnimSprite)GetComponent<AnimSprite>();

            switch (StartBehaviour)
            {
                case EnemyBehaviour.Gaucho:
                    Behaviours.BehaviourBase state = new Behaviours.GauchoSleep();
                    SetBehaviour(state);
                    break;
                case EnemyBehaviour.Dengue:
                    break;
                case EnemyBehaviour.Mono:
                    break;
                case EnemyBehaviour.Palometa:
                    break;
                case EnemyBehaviour.Vampi:
                    break;
            }
        }

        public Behaviours.BehaviourBase GetBehaviour()
        {
            return ActiveBehaviour;
        }
        
        public void SetBehaviour( Behaviours.BehaviourBase behaviour)
        {
            if (ActiveBehaviour != null)
            {
                ActiveBehaviour.OnDeinit();
                ActiveBehaviour = null;
            }
            
            ActiveBehaviour = behaviour;

            if (ActiveBehaviour != null)
            {
                ActiveBehaviour.OnInit( gameObject);
                //ActiveBehaviour->SetTarget(posX, posY, posZ);
            }
            else
                Debug.Log("The Behaviour was null, nothing created");
        }

        public void Update()
        {
            if (ActiveBehaviour != null) 
                ActiveBehaviour.OnUpdate();
        }

        void OnTriggerEnter(Collider hit)
        {
            ActiveBehaviour.OnTriggerEnter(hit);
        }
        //IEnumerator OnTriggerEnter(Collider other)											// other.transform.position == target.position
        //{
        //    if (other.CompareTag("Player"))
        //    {
        //        if (gameObject.CompareTag("Enemy") && (target.position.y > transform.position.y + .1f))
        //        {
        //            linkToPlayerControls.velocity.y = deathForce;
        //            Managers.Audio.Play(soundCrash, transform, 6.0f, 1.0f);
        //            enemyState = ShooterState.Stunned;
        //            Destroy(Instantiate(ParticleStars, transform.position, transform.rotation), 5);

        //        }
        //        else if (Input.GetAxis("Vertical") != 0)
        //        {
        //            Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);

        //            Managers.Audio.Play(soundCrash, thisTransform);
        //            linkToPlayerControls.velocity.y = deathForce;
        //        }

        //    }
        //    else if (other.CompareTag("p_shot"))
        //    {
        //        Managers.Register.Score += 10 * Random.Range(1, 15);
        //        BeatDown();
        //    }
        //    else if (gameObject.CompareTag("p_shot") && !other.CompareTag("Item"))
        //    {
        //        yield return new WaitForSeconds(0.01f);
        //        BeatDown();
        //    }
        //}

        void OnDestroy()
        {
	        if (ActiveBehaviour != null)
	        {
		        ActiveBehaviour.OnDeinit() ;
                ActiveBehaviour = null;
		        //this is it
	        }
        }

    }

