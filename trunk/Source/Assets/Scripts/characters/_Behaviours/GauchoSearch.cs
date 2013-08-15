using UnityEngine;
using System.Collections;

namespace Behaviours                            // Some Enemies Behviours
{
    public class GauchoSearch : Behaviours.BehaviourBase
    {
        public float aimDamping = 4;			// speed & time taken for aiming
        private float nextFire = 3;			    // delay between attacks
        private float timeLapse = 0;			// delay between attacks
        public float shootPower = 5;			// the power or speed of the projectile.
        public float fireRate = 2;			    // delay between shots, more lower, more harder to avoid

        private float distanceToTarget = 0;		// the dist to target position stored in square mag.
        private float attackRange = 5;			// the dist to target position stored in square mag.

        public float AttackRangeX2 = 0;			// set range for speed increase

        public override void OnInit(GameObject enemy)
        {
            base.OnInit(enemy);
            AttackRangeX2 = attackRange * attackRange;
        }

        //public override void OnDeinit()    { ;}


        public override void OnUpdate()
        {
            distanceToTarget = (thisData.target.position - thisTransform.position).sqrMagnitude;	// it's much faster using Square mag. 
            thisData.orientation = (int)Mathf.Sign(thisData.target.position.x - thisTransform.position.x);

            Quaternion lookPos = Quaternion.LookRotation(thisTransform.position - thisData.target.position, Vector3.forward);

            lookPos.y = 0;
            lookPos.x = 0;
            thisData.aimCompass.rotation = 
                Quaternion.Slerp(thisData.aimCompass.rotation, lookPos, Time.deltaTime * aimDamping);

            if (timeLapse > 0)
            {
                thisData.animPlay.PlayFramesFixed(2, 0, 2, thisData.orientation);       // Do Throw Animation
                timeLapse -= Time.deltaTime;
                return;
            }

            if (distanceToTarget <= AttackRangeX2)									// if player's near enemy -> Aim & Attack him
            {
                if (Time.time < nextFire && timeLapse <= 0)												// wait some time between shots 
                    thisData.animPlay.PlayFramesFixed(1, 2, 2, thisData.orientation);			// Do Seeking Animation


                if (Time.time > nextFire)												// wait some time between shots 
                {
                    Shoot();
                    timeLapse = .5f;
                    nextFire = Time.time + fireRate;
                }
            }
            else
                thisData.SetBehaviour(new Behaviours.GauchoSleep());// else if player it's out enemy range -> go Sleep
        }

        void Shoot()
        {
            Managers.Audio.Play(thisData.soundAttack, thisTransform);

            // Instantiate the projectile
            GameObject clone = (GameObject)GameObject.Instantiate(  thisData.projectile, 
                                                                    thisData.aimCompass.position,
                                                                    thisData.aimCompass.rotation);

            Physics.IgnoreCollision(clone.collider, Character.collider); 				// it works but the distance it s lame
            clone.name = "Shot";

            // Add speed to the target
                   
            clone.GetComponent<BulletShot>().FireAnimated(thisData.aimCompass.up * shootPower, 2, 2, 2); 	// shot with a short animation
        }

        public override void OnTriggerEnter(Collider other)
        {
                  
            if (other.CompareTag("Player"))
            {
                if (Character.CompareTag("Enemy") && (thisData.target.position.y > thisTransform.position.y + .1f))
                {
                    thisData.linkToPlayerControls.velocity.y = thisData.deathForce;
                    Managers.Audio.Play(thisData.soundCrash, thisTransform, 6.0f, 1.0f);
                    //enemyState = ShooterState.Stunned;

                    //GameManager.Destroy(
                    //    GameManager.Instantiate(
                    //        thisData.ParticleStars, thisTransform.position, thisTransform.rotation), 5);

                    //thisData.Destroy(thisData.Instantiate(thisData.ParticleStars, thisTransform.position, thisTransform.rotation), 5);

                }
                else if (Input.GetAxis("Vertical") != 0)
                {
                    //Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);

                    //Managers.Audio.Play(soundCrash, thisTransform);
                    //thisData.linkToPlayerControls.velocity.y = deathForce;
                }

            }
            else if (other.CompareTag("p_shot"))
            {
                Managers.Register.Score += 10 * Random.Range(1, 15);
                //BeatDown();
            }
            else if (Character.CompareTag("p_shot") && !other.CompareTag("Item"))
            {
                //yield return new WaitForSeconds(0.01f);
                //BeatDown();
            }
        }
 

    }
}
