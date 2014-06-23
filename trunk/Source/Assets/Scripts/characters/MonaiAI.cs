using UnityEngine;
using System.Collections;

namespace Bosses
{

    public class MonaiAI : MonoBehaviour
    {
		public AudioClip BossMusic;
		public AudioClip prevMusic;

        SpriteChain Chain = null;
        AnimSprite Head = null;
        Transform thisTransform = null;
        Transform PlayerTransform = null;

        int Orientation = -1;                       // This setup the Monster face direction
        int AttackRange = 0;                        // number of Wipes before change to next behaviour
        int Health = 3;                        // total hits needed to kill it
        float horizon = 3;                        // medium height from where the Sin moves on...
        float PlayerHeight = 0;                        // one TimeLapse fastcheck player position for Sliding Attack

        float ClimbFactor = 7.5f;                     // It's a kind of inversed Gravity 
        float ClimbSpeed = 0;                        // and this It's the Speed that's Reduced by ClimbFactor
        Vector3 NewPosition = Vector3.zero;

        public Rect BossArea = new Rect(0, 0, 15, 5);
        //public Rect PrevArea = Managers.Display.cameraScroll.levelBounds;
		public Rect LevelArea = new Rect(0, 0, 300, 50);

        public bool DisplayArea = false;
        public enum BossState { Standby = 0, Talking, Roaming, WildCircle, AttakingTree, Sliding, Hurting, Dying }
        public BossState MonaiState = BossState.Standby;

        float radius = 2.5f;                                // This is for Sanke Orbit around Player or in WildCircle attack
        float angle = 0;
        Vector3 CenterPosition = Vector3.zero;

        public TextAsset file;
        bool Talking = false;
        public ParticleSystem particle;
        public ParticleSystem Slash;
        //public ParticleSystem Leafing;
        public ParticleSystem Leafs;

        public AudioClip SnakeHit;
        public AudioClip SnakeRattle;
        public AudioClip SnakeOrbit;
        public AudioClip SnakeBus;

        // Use this for initialization
        void Start()
        {
            if (Managers.Register.MonaiDefeated)
            {
                Debug.Log("Moñai Already Beated!");
                DestroyImmediate(gameObject);
                return;
            }

            thisTransform = this.transform;
            Chain = GetComponent<SpriteChain>();
            Head = GetComponent<AnimSprite>();
            thisTransform.position = new Vector3(thisTransform.position.x, thisTransform.position.y, -1);

            LevelArea = Managers.Display.cameraScroll.levelBounds;

            BossArea.center = new Vector2(thisTransform.position.x, thisTransform.position.y);
            if (Managers.Game.PlayerPrefab)
            {
                PlayerTransform = Managers.Game.PlayerPrefab.transform;
                PlayerHeight = PlayerTransform.position.y;
            }
            horizon = BossArea.center.y;
        }

        // Update is called once per frame
        void Update()
        {

            NewPosition = thisTransform.position;

            switch (MonaiState)
            {
                case BossState.Standby:
                    StandBy();
                    break;
                case BossState.Talking:
                    TalkingState();
                    break;
                case BossState.Roaming:
                    Roaming();
                    if (AttackRange > 4)
                        RandomState();
                    break;
                case BossState.WildCircle:
                    WildCircle();
                    break;
                case BossState.AttakingTree:
                    JumpTreeAttack();
                    break;
                case BossState.Hurting:
                    HurtState();
                    break;
                case BossState.Sliding:
                    SlideFastAttack(PlayerHeight);
                    break;
                case BossState.Dying:
                    DyingState();
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


        //void SinAttack()
        //{

        //    Chain.MoveStyle = SpriteChain.TranslationMode.Retro;

        //    Head.PlayFrames(0, 1, 1, Orientation);

        //    NewPosition = new Vector3(NewPosition.x + (TimeLapse.deltaTime * 3 * Orientation),
        //                               horizon + (Mathf.Sin(TimeLapse.time * 3) * 1.5f), -1);

        //    if (NewPosition.x > BossArea.xMax && Orientation == +1)
        //    {
        //        AttackRange++;
        //        Orientation = -1;
        //    }

        //    if (NewPosition.x < BossArea.xMin && Orientation == -1)
        //    {
        //        AttackRange++;
        //        Orientation = +1;
        //    }

        //    if (AttackRange > 3)
        //    {
        //        AttackRange = 0;
        //        MboiTuiState = BossState.Roaming;
        //    }
        //}

        void StandBy()
        {


            if (!Managers.Game.PlayerPrefab)
                return;
            else if (!PlayerTransform)
            {
                PlayerTransform = Managers.Game.PlayerPrefab.transform;
                PlayerHeight = PlayerTransform.position.y;
            }

            Chain.MoveStyle = SpriteChain.TranslationMode.Smooth;

            Head.PlayFrames(0, 1, 1, Orientation);

            NewPosition = new Vector3(NewPosition.x + (Time.deltaTime * 3 * Orientation),
                                       horizon + (Mathf.Sin(Time.time * 2) * (BossArea.height * .5f)), -1);   // Max Height taken

            if (NewPosition.x > BossArea.xMax - 4 && Orientation == +1)
            {
                AttackRange++;
                Orientation = -1;
            }

            if (NewPosition.x < BossArea.xMin + 4 && Orientation == -1)
            {
                AttackRange++;
                Orientation = +1;
            }

            if ((thisTransform.position.x - Managers.Game.PlayerPrefab.transform.position.x) < 2)
            {
				Managers.Audio.PlayMusic(BossMusic, 0.75f, 1);
                MonaiState = BossState.Talking;
                Talking = true;
            }
        }

        void WildCircle()
        {
            if (CenterPosition == Vector3.zero)
            {
                //PlayerHeight = Managers.Game.PlayerPrefab.transform.position.y;
                PlayerHeight = BossArea.yMin + .5f;
                CenterPosition = new Vector3(NewPosition.x, PlayerHeight, NewPosition.z);
            }

            Chain.MoveStyle = SpriteChain.TranslationMode.Smooth;

            Head.PlayFrames(0, 1, 1, Orientation);

            CenterPosition += Vector3.right * (Orientation * Time.deltaTime);

            Vector3 newRotation = new Vector3(CenterPosition.x + (radius * Mathf.Cos(angle * (Mathf.PI / 180) * Orientation)),
                                                CenterPosition.y + (radius * Mathf.Sin(angle * (Mathf.PI / 180) * Orientation)),
                                                  NewPosition.z);

            angle += 200 * Time.deltaTime;


            if ((int)angle % 180 == 0)
                if (Orientation == 1)
                    Destroy(Instantiate(Slash, newRotation, thisTransform.rotation), 5);
                else
                    Destroy(Instantiate(Slash, newRotation, thisTransform.rotation), 5);

            if (angle > 360)
            {
                Managers.Audio.Play(SnakeOrbit, thisTransform, 6, 1);
                angle = 0;
            }

            NewPosition = newRotation;

            if (NewPosition.x > (BossArea.xMax + 5) && Orientation == +1)
            {
                AttackRange++;
                Orientation = -1;
            }

            if (NewPosition.x < (BossArea.xMin - 5) && Orientation == -1)
            {
                AttackRange++;
                Orientation = +1;
            }

            if (AttackRange > 2)
            {
                AttackRange = 0;
                MonaiState = BossState.Roaming;
                CenterPosition = Vector3.zero;
            }
        }

        void JumpTreeAttack()
        {
            Chain.MoveStyle = SpriteChain.TranslationMode.Retro;

            Head.PlayFrames(0, 1, 1, Orientation);

            if (NewPosition.y > BossArea.yMax - 1.5f)
            {
                Managers.Audio.Play(SnakeRattle, thisTransform, 5, 1);
                //Leafs.SetActive(true);
                //Leafs.particleSystem.Simulate(5);
                //Leafs.particleSystem.Play();
                Instantiate(Leafs, thisTransform.position, thisTransform.rotation);
                ClimbSpeed = -7;
            }

            NewPosition = new Vector3(NewPosition.x + (Time.deltaTime * 3 * Orientation),
                                           NewPosition.y + (ClimbSpeed * Time.deltaTime), -1);
            ClimbSpeed += ClimbFactor * Time.deltaTime;

            if (NewPosition.x > BossArea.xMax && Orientation == +1)
            {
                AttackRange++;
                Orientation = -1;
            }

            if (NewPosition.x < BossArea.xMin && Orientation == -1)
            {
                AttackRange++;
                Orientation = +1;
            }

            //Debug.DrawLine(Vector3.up * height, new Vector3(BossArea.xMax, height, 0), Color.red);

            if (AttackRange > 5)
            {
                AttackRange = 0;
                MonaiState = BossState.Roaming;
            }

        }

        void Roaming()
        {
            Chain.MoveStyle = SpriteChain.TranslationMode.Smooth;

            Head.PlayFrames(0, 1, 1, Orientation);

            NewPosition = new Vector3(NewPosition.x + (Time.deltaTime * 3 * Orientation),
                                       horizon + (Mathf.Sin(Time.time * 2) * (BossArea.height * .5f)), -1);   // Max Height taken

            if (NewPosition.x > BossArea.xMax + 5 && Orientation == +1)
            {
                AttackRange++;
                Orientation = -1;
            }

            if (NewPosition.x < BossArea.xMin - 5 && Orientation == -1)
            {
                AttackRange++;
                Orientation = +1;
            }


        }

        void SlideFastAttack(float playerHeight)
        {
            Chain.MoveStyle = SpriteChain.TranslationMode.Smooth;

            Head.PlayFrames(0, 1, 1, Orientation);

            NewPosition = new Vector3(NewPosition.x + (Time.deltaTime * (7 * Orientation)),
                               playerHeight + (Mathf.Sin(Time.time * 20) * .25f), -1);

            if (NewPosition.x > BossArea.xMax + 10)
            {
                Orientation = -1;
                Chain.ResetLinks();
                RandomState();
            }

            if (NewPosition.x < BossArea.xMin - 10)
            {
                Orientation = +1;
                Chain.ResetLinks();
                RandomState();
            }
        }

        void TalkingState()
        {
            Chain.MoveStyle = SpriteChain.TranslationMode.Smooth;

            Vector3 newRotation = new Vector3(Managers.Game.PlayerPrefab.transform.position.x + (radius * Mathf.Cos(angle * (Mathf.PI / 180))),
                                                 Managers.Game.PlayerPrefab.transform.position.y + (radius * Mathf.Sin(angle * (Mathf.PI / 180))),
                                                  NewPosition.z);

            angle += 100 * Time.deltaTime;

            if (angle > 360)
                angle = 0;

            NewPosition = newRotation;

            if (Talking && !Managers.Dialog.IsInConversation())
                StartTalk();

            if (!Talking && !Managers.Dialog.IsInConversation())
                EndTalk();
        }

        void StartTalk()
        {
            Talking = false;

            Managers.Dialog.Init(file);
            Managers.Dialog.StartConversation("Moñai");

            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 1;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 0.01f;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 2;

            Rect BossBounds = new Rect(BossArea.xMin, BossArea.yMin - 1, BossArea.width, BossArea.height + 2);
            Managers.Display.cameraScroll.ResetBounds(BossBounds);
        }

        void EndTalk()
        {
            MonaiState = BossState.Roaming;

            Managers.Dialog.StopConversation();

            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 0;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 0;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 2.5f;

            //timeTrailer = 0;
        }


        void HurtState()
        {

            if (Health <= 1)
            {
				Managers.Audio.PlayMusic(prevMusic, 0.75f, 1);
                Managers.Display.cameraScroll.ResetBounds(LevelArea);
//                Managers.Display.cameraScroll.ResetBounds(LevelArea);
                MonaiState = BossState.Dying;
                return;

            }

            AttackRange = 0;
            Head.PlayFrames(0, 0, 2, Orientation);

            PlayerHeight = PlayerTransform.position.y;

            if (NewPosition.y > 0)
                ClimbSpeed = 1;
            else
                ClimbSpeed = 10;

            NewPosition = new Vector3(NewPosition.x + Mathf.Sin(Time.time * 10) * .015f,
                                        NewPosition.y - Time.deltaTime * ClimbSpeed,
                                        NewPosition.z);


            if (NewPosition.y < -10)
            {
                ClimbSpeed = 0;
                Random.seed = (int)Time.time;
                int randomize = Random.Range(0, 100);
                if (randomize % 2 == 0)
                {
                    NewPosition = new Vector3(BossArea.xMax - 1, PlayerHeight, NewPosition.z);
                    Orientation = -1;
                }
                else
                {
                    NewPosition = new Vector3(BossArea.xMin + 1, PlayerHeight, NewPosition.z);
                    Orientation = +1;
                }
                Chain.ResetLinks();

                MonaiState = BossState.Sliding;
                Managers.Audio.Play(SnakeBus, thisTransform, 2, 1);
                Health--;
            }
        }

        void DyingState()
        {
            if (Health >= 0)
                Chain.DestroyLinks();

            NewPosition = new Vector3(NewPosition.x,
                           NewPosition.y - Mathf.Sin(Time.time * 20) * .015f,
                           NewPosition.z);

            AttackRange = 0;
            Health = -1;
            Head.PlayFrames(0, 0, 2, Orientation);
            Managers.Register.MonaiDefeated = true;

        }

        void RandomState()
        {
            AttackRange = 0;
            Random.seed = (int)Time.time;
            int randomize = Random.Range(0, 100);
            if (randomize % 2 == 0)
                MonaiState = BossState.WildCircle;
            else
                MonaiState = BossState.AttakingTree;
        }


        void OnTriggerEnter(Collider hit)
        {

			if (hit.tag == "Player" &&  ((MonaiState == BossState.WildCircle)||(MonaiState == BossState.Roaming))  &&
                (hit.transform.position.y > thisTransform.position.y + .1f))
            {
                //Debug.Log("Found Pombero");


                PlayerControls PlayerControl = (PlayerControls)PlayerTransform.GetComponent<PlayerControls>();
                PlayerControl.velocity.y = 7;
                MonaiState = BossState.Hurting;
                Managers.Audio.Play(SnakeHit, thisTransform, 1, 1);
                Destroy(Instantiate(particle, thisTransform.position, thisTransform.rotation), 5);

            }
        }
    }

}