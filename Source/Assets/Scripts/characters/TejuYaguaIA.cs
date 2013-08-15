using UnityEngine;
using System.Collections;

namespace Bosses
{

    public class TejuYaguaIA : MonoBehaviour
    {
        AnimSprite Anime = null;
        Transform thisTransform = null;
        Transform PlayerTransform = null;
        Transform AimCompass = null;
        Transform Maracuya = null;

        #region Powers Prefabs
        public GameObject[] Powers;                     // 0) Shield  1) RayStart  2) RayTrace  3) RayEnd  4) FireStaff 
        public GameObject DashFire;
        #endregion

        int Orientation = -1;                       // This setup the Monster face direction
        int AttackRange = 0;                        // number of Wipes before change to next behaviour
        int Health = 7;                        // total hits needed to kill it

        float timeLapse = 0;
        float aimDamping = 4;			// speed & time taken for aiming
        Vector3 NewPosition = Vector3.zero;

        public bool DisplayArea = true;
        public Rect BossArea = new Rect(0, 0, 15, 6);
        private Rect LevelArea;

        public enum BossState { Standby = 0, Talking, Standing, Running, Firing, Hurting, HeadLess, FriendShip, Escaping }
        public BossState YaguaState = BossState.Standby;

        public TextAsset file;
        bool Talking = false;
        bool AttackType = true;
        public ParticleSystem particle;

        bool HoldingButton = false;

        //public AudioClip Hit;
        //public AudioClip Fire;
        //public AudioClip Woof;
        //public AudioClip Scissor;

        void Start()
        {
            if (Managers.Register.YaguaDefeated)
            {
                Debug.Log("Yagua Already Beated!");

                DestroyImmediate(gameObject);
                //this.enabled = false;
                return;
            }

            Anime = GetComponent<AnimSprite>();
            thisTransform = transform;
            NewPosition = new Vector3(thisTransform.position.x, thisTransform.position.y, .25f);
            //thisTransform.localScale = new Vector3(3, 3, 1);
            thisTransform.localScale = new Vector3(4, 4, 1);

            BossArea.center = new Vector2(thisTransform.position.x, thisTransform.position.y);
            BossArea.yMin = thisTransform.position.y - (thisTransform.localScale.y * 0.5f);

            if (!AimCompass)
                AimCompass = thisTransform.FindChild("AimCompass");

            if (Managers.Game.PlayerPrefab)
                PlayerTransform = Managers.Game.PlayerPrefab.transform;

            LevelArea = Managers.Display.cameraScroll.originalBounds;
            BossArea = new Rect(BossArea.xMin, BossArea.yMin - 1, BossArea.width, BossArea.height + 2);

            DashFire.SetActive(false);
        }

        void Update()
        {
            NewPosition = thisTransform.position;
            HoldingButton = Input.GetButtonDown("Fire1");

            switch (YaguaState)
            {
                case BossState.Standby:
                    Anime.PlayFrames(1, 0, 3, Orientation);
                    if (!PlayerTransform)
                        if (Managers.Game.PlayerPrefab)
                            PlayerTransform = Managers.Game.PlayerPrefab.transform;
                        else
                            return;

                    if (Mathf.Abs(PlayerTransform.position.x - thisTransform.position.x) < 4)
                    {
                        Talking = true;
                        YaguaState = BossState.Talking;
                    }
                    break;
                case BossState.Talking:
                    TalkingState();
                    break;
                case BossState.Standing:
                    Stand();
                    break;
                case BossState.Running:
                    Run();
                    break;
                case BossState.Firing:
                    Fire();
                    break;
                case BossState.Hurting:
                    Hurt();
                    break;
                case BossState.HeadLess:
                    HeadLess();
                    break;
                case BossState.FriendShip:
                    FriendShip();
                    GiveThanks();
                    break;
                case BossState.Escaping:
                    Escape();
                    break;
            }

            thisTransform.position = NewPosition;

            //if (DisplayArea)
            //{
            //    Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMin), new Vector2(BossArea.xMax, BossArea.yMin), Color.red);
            //    Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMax), new Vector2(BossArea.xMax, BossArea.yMax), Color.red);
            //    Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMin), new Vector2(BossArea.xMin, BossArea.yMax), Color.red);
            //    Debug.DrawLine(new Vector2(BossArea.xMax, BossArea.yMin), new Vector2(BossArea.xMax, BossArea.yMax), Color.red);
            //}

        }

        void Stand()
        {
            timeLapse += Time.deltaTime;

            Orientation = (int)Mathf.Sign(PlayerTransform.position.x - thisTransform.position.x);
            //Anime.PlayFramesFixed(1, 0, 3, Orientation, 1.005f);
            //Anime.PlayFramesFixed(1, 0, 3, Orientation);
            Anime.PlayFrames(1, 0, 3, Orientation);

            Quaternion lookPos = Quaternion.LookRotation(thisTransform.position - PlayerTransform.position, Vector3.forward);

            lookPos.y = 0;
            lookPos.x = 0;
            AimCompass.rotation = Quaternion.Slerp(AimCompass.rotation, lookPos, Time.deltaTime * aimDamping);

            if (timeLapse > 4)
            {
                timeLapse = 0;
                AttackRange = 0;

                if (AttackType)
                {
                    YaguaState = BossState.Running;
                    DashFire.SetActive(true);
                    AttackType = false;
                }
                else
                {
                    YaguaState = BossState.Firing;
                    AttackType = true;
                }

                return;
            }
        }

        void Run()
        {
            Anime.PlayFramesFixed(0, 0, 3, Orientation);

            NewPosition = new Vector3(NewPosition.x + (Time.deltaTime * 5 * Orientation), NewPosition.y, .25f);

            //if ( Mathf.Approximately(NewPosition.x, BossArea.center.x) )
            //    Destroy(Instantiate(particle,
            //                                thisTransform.position + (Vector3.right * Orientation) + (Vector3.back * 2),
            //                                thisTransform.rotation), 5);

            if (NewPosition.x > BossArea.xMax - 1)
            {
                AttackRange++;
                Orientation = -1;
            }

            if (NewPosition.x < BossArea.xMin + 1)
            {
                AttackRange++;
                Orientation = +1;
            }

            if (AttackRange > 2)
            {
                AttackRange = 0;
                timeLapse = 0;
                DashFire.SetActive(false);
                YaguaState = BossState.Standing;
            }
        }

        void Fire()
        {
            timeLapse += Time.deltaTime;

            Orientation = (int)Mathf.Sign(PlayerTransform.position.x - thisTransform.position.x);
            Anime.PlayFramesFixed(2, 0, 2, Orientation);                                   // Do Fire eyes Animation!

            Quaternion lookPos = Quaternion.LookRotation(thisTransform.position - PlayerTransform.position, Vector3.forward);
            lookPos.y = 0;
            lookPos.x = 0;
            AimCompass.rotation = Quaternion.Slerp(AimCompass.rotation, lookPos, Time.deltaTime * aimDamping);

            if (timeLapse > 2 && timeLapse < 3)                                             // Shoot da Fire 
            {
                timeLapse = 3;
                Shoot();
                Destroy(Instantiate(particle,
                        thisTransform.position + (Vector3.right * Orientation) + (Vector3.back * 2),
                        thisTransform.rotation), 5);
            }

            if (timeLapse > 3)												                // If  (Range < 3) Repeat all again 
            {
                timeLapse = 0;
                AttackRange++;

                if (AttackRange > 2)
                {
                    AttackRange = 0;
                    timeLapse = 0;
                    YaguaState = BossState.Standing;
                    return;
                }
            }
        }

        void Shoot()
        {
            //Managers.Audio.Play(soundAttack, thisTransform);

            // Instantiate the projectile
            GameObject clone = (GameObject)Instantiate(Powers[0], AimCompass.position, AimCompass.rotation);

            Physics.IgnoreCollision(clone.collider, this.gameObject.collider); 				// it works but the distance it s lame

            clone.name = "FireShot";
            clone.GetComponent<BulletShot>().FireAnimated(AimCompass.up * 10, 2, 0, 4); 	// shot with a short animation
        }


        void TalkingState()
        {
            //Anime.PlayFramesFixed(1, 0, 3, Orientation, 1.005f);
            Anime.PlayFrames(1, 0, 3, Orientation);

            if (Talking && !Managers.Dialog.IsInConversation())
                StartTalk();

            if (!Talking && !Managers.Dialog.IsInConversation())
                EndTalk();
        }

        void StartTalk()
        {
            Talking = false;

            Managers.Dialog.Init(file);
            Managers.Dialog.StartConversation("Fight");

            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 1;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 1;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 2;

            //PrevArea = Managers.Display.cameraScroll.levelBounds;
            //Rect BossBounds = new Rect(BossArea.xMin, BossArea.yMin - 1, BossArea.width, BossArea.height + 2);
            Managers.Display.cameraScroll.ResetBounds(BossArea);
        }

        void EndTalk()
        {
            YaguaState = BossState.Standing;

            Managers.Dialog.StopConversation();

            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 0;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 0;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 3.5f;

            timeLapse = 0;
            AttackRange = 0;
        }


        void Hurt()
        {
            if (timeLapse == 0)
            {
                Health--;
                gameObject.tag = "Untagged";
                //Destroy(Instantiate(Powers[1], thisTransform.position, thisTransform.rotation), 5);

                if (Health <= 0)
                {
                    YaguaState = BossState.Escaping;
                    return;
                }

                timeLapse = Time.time + 3;
            }

            Anime.PlayFramesFixed(1, 3, 2, Orientation, 1.005f);
            renderer.enabled = (Mathf.RoundToInt(Time.time * 32) % 2 == 0);


            if (timeLapse < Time.time)
            {
                gameObject.tag = "Enemy";
                timeLapse = 0;
                renderer.enabled = true;

                float randomizer = Random.Range(0, 100);

                if (Health > 3)
                {
                    YaguaState = BossState.Running;
                    DashFire.SetActive(true);
                }
                else
                {
                    if (randomizer < 50)
                    {
                        YaguaState = BossState.Running;
                        DashFire.SetActive(true);
                    }
                    else
                    {
                        YaguaState = BossState.Firing;
                        DashFire.SetActive(true);
                    }
                }

            }
        }

        void HeadLess()
        {
            if (timeLapse == 0)
            {
                Health--;
                Destroy(Instantiate(Powers[1], thisTransform.position, thisTransform.rotation), 3);

                if (Health <= 0)
                {
                    YaguaState = BossState.Escaping;
                    return;
                }
            }

            timeLapse += Time.deltaTime;

            if (timeLapse < 1)                             // 1.5f
                Anime.PlayFramesFixed(2, 2, 2, Orientation);

            if (timeLapse > 1 && timeLapse < 2)
                Anime.PlayFramesFixed(3, (int)((timeLapse - 1) * 4), 1, Orientation);

            if (timeLapse > 2)                                  // 3
            {
                timeLapse = 0;
                if (AttackType)
                {
                    YaguaState = BossState.Running;
                    DashFire.SetActive(true);
                    AttackType = false;
                }
                else
                {
                    YaguaState = BossState.Firing;
                    AttackType = true;
                }
                return;
            }
        }



        void FriendShip()
        {
            Orientation = (int)Mathf.Sign(Maracuya.position.x - thisTransform.position.x);

            if (Maracuya != null && Maracuya.position.x < 188)
            {
                if (Maracuya.parent == Managers.Game.PlayerPrefab.transform)
                {
                    Maracuya.parent = null;        		//resets the pickup's parent to null so it won't keep following the player	
                    Maracuya.collider.enabled = true;
                    Maracuya.rigidbody.isKinematic = false;

                    Managers.Game.PlayerPrefab.GetComponent<PlayerProperties>()._pickedObject = null;	    			
                }
                   //StartCoroutine( Managers.Game.PlayerPrefab.GetComponent<PlayerProperties>().PlayerThrows());

                Maracuya.position = new Vector3(188, Maracuya.position.y, Maracuya.position.z);
                Maracuya.rigidbody.AddForce( new Vector3( 1, 0, 0) * 6, ForceMode.VelocityChange);
		
            }

            if (Mathf.Abs(Maracuya.position.x - thisTransform.position.x) > 0.5f)
            {
                if (Maracuya != null)
                    NewPosition +=
                           Vector3.right * Mathf.Abs(Maracuya.position.x - thisTransform.position.x) * Time.deltaTime * Orientation;

                //Anime.PlayFramesFixed(0, 0, 3, Orientation);
                Anime.PlayFrames(0, 0, 3, Orientation);
            }
            else
            {
                Maracuya.rigidbody.Sleep();
                Anime.PlayFrames(1, 0, 3, Orientation);
                Maracuya.rigidbody.useGravity = true;
            }


            //if (Managers.Dialog.IsInConversation())
            //GiveThanks();

            //if (Talking && !Managers.Dialog.IsInConversation())
            //{
            //    Talking = false;
            //    Managers.Dialog.StartConversation("Friendship");

            //    (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 1;
            //    (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 2;
            //}

        }

        void GiveThanks()
        {
            if (Talking && Managers.Dialog.IsInConversation())
            {
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 1;
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 
                            (transform.position.x - Managers.Game.PlayerPrefab.transform.position.x) * .5f;
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 2.5f;

            }
            //else if (soundSource && soundSource.isPlaying)
            //{
            //    soundSource.Stop();
            //    Destroy(soundSource);
            //}

            if (Talking && (!Managers.Dialog.IsInConversation() ||
                (Mathf.Abs(transform.position.x - Managers.Game.PlayerPrefab.transform.position.x) > 4)))
            {
                Managers.Dialog.StopConversation();
                Talking = false;
                //Managers.Dialog.DeInit();

                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 0;
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 0;
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = oldZoom;
            }
        }

        void Escape()
        {
            timeLapse += Time.deltaTime;

            if (timeLapse < 2)
                Anime.PlayFramesFixed(1, 3, 2, Orientation, 1.005f);

            if (timeLapse > 2)
            {
                Orientation = (int)Mathf.Sign(PlayerTransform.position.x - thisTransform.position.x) * -1;
                Anime.PlayFramesFixed(0, 0, 3, Orientation);
                NewPosition = new Vector3(NewPosition.x + (Time.deltaTime * 6 * Orientation), NewPosition.y, .25f);

                if (NewPosition.x > BossArea.xMax + 5 || NewPosition.x < BossArea.xMin - 5)
                {
                    timeLapse = 0;
                    Managers.Display.cameraScroll.ResetBounds(LevelArea);
                    (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 0;
                    (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 0;
                    (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 2.5f;
                    Destroy(thisTransform.gameObject);
                }
                Managers.Register.YaguaDefeated = true;
            }
        }


        void OnTriggerEnter(Collider hit)
        {

            if (hit.tag == "Player" && (YaguaState == BossState.Standing) &&
                (hit.transform.position.y > thisTransform.position.y - .5f))
            {

                PlayerControls PlayerControl = (PlayerControls)PlayerTransform.GetComponent<PlayerControls>();
                PlayerControl.velocity.y = 11;
                YaguaState = BossState.Hurting;
                timeLapse = 0;
                AttackRange = 0;

                //Managers.Audio.Play(SnakeHit, thisTransform, 1, 1);
                Destroy(Instantiate(Powers[2], thisTransform.position, thisTransform.rotation), 5);
            }


            if (hit.tag == "p_shot" && hit.name != "Maracuya" && (YaguaState == BossState.Standing) &&
                (hit.transform.position.y > thisTransform.position.y - 1))
            {
                if (hit.name == "Hat")
                {
                    timeLapse = 0;
                    AttackRange = 0;
                    YaguaState = BossState.HeadLess;
                    //Managers.Audio.Play(SnakeHit, thisTransform, 1, 1);
                }
                else
                {
                    YaguaState = BossState.Hurting;
                    timeLapse = 0;
                    AttackRange = 0;

                    //Managers.Audio.Play(SnakeHit, thisTransform, 1, 1);
                    Destroy(Instantiate(Powers[2], thisTransform.position, thisTransform.rotation), 5);
                }
            }


            if (hit.name == "maracuya" && (YaguaState != BossState.HeadLess)
                && (YaguaState != BossState.Escaping) && (YaguaState != BossState.FriendShip))
            {
                if (YaguaState == BossState.Talking && Managers.Dialog.IsInConversation())
                    EndTalk();

                Managers.Display.cameraScroll.ResetBounds(LevelArea);
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 0;
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 0;
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 2.5f;

                DashFire.SetActive(false);
                //hit.rigidbody.velocity = Vector3.zero;
                gameObject.tag = "Untagged";
                hit.tag = "pickup";

                if (Maracuya == null)
                    Maracuya = hit.transform;

                timeLapse = 0;
                AttackRange = 0;
                YaguaState = BossState.FriendShip;

                Managers.Register.YaguaDefeated = true;

                //Managers.Audio.Play(SnakeHit, thisTransform, 1, 1);
            }

        }

        void OnTriggerStay(Collider hit)
        {
            if (hit.tag == "Player" && (YaguaState == BossState.FriendShip)
                && !Talking && !Managers.Dialog.IsInConversation() && (Managers.Game.InputUp ||
                (HoldingButton && Managers.Game.PlayerPrefab.GetComponent<PlayerProperties>()._pickedObject == null)))
            {
                oldZoom = (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier;

                Talking = true;
                //soundSource = Managers.Audio.Play(soundChat, gameObject.transform, 1f, 2.0f);

                Managers.Dialog.StartConversation("Friendship");
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 1;
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 2;
            }
        }


        float oldZoom = 3;

    }


}