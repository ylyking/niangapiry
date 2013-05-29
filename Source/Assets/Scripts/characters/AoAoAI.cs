using UnityEngine;
using System.Collections;

namespace Bosses
{

    public class AoAoAI : MonoBehaviour
    {


        int Orientation = -1;                       // This setup the Monster face direction
        float TimeLapse = 0;
        float WalkSpeed = 2;
        static int Health = 15;

        public TextAsset file;
        bool Talking = false;

        public enum AoSize { Huge, Large, Medium, Small }
        public AoSize MySize = AoSize.Huge;


        public enum BossState { Roaming = 0, Talking, Searching, Throwing, Hurting, Die }
        public BossState AoAoState = BossState.Roaming;
        AnimSprite AoAnim;

        Transform ThisTransform;
        Transform PlayerTransform;
        Transform TreeTransform;
        AoAoThing TreeScript;
        Transform TargetTransform;
        Vector3 NewPosition;

        public Rect BossArea;
        Rect LevelArea;


        public GameObject[] Prefabs;


        void Start()
        {
            //if (Health <= 0)
            //    Debug.Log("AoAO Already Beated!");

            if ((MySize == AoSize.Huge) && Managers.Register.AoAoDefeated)
            {
                Debug.Log("AoAO Already Beated!");

                GameObject Door = (GameObject)Instantiate(Prefabs[5],
                                        new Vector3((Managers.Game.PlayerPrefab).transform.position.x,
                                                     Managers.Display.cameraScroll.levelBounds.yMin + 1.5f, .3f),
                                                     gameObject.transform.rotation);

                Door.transform.parent = gameObject.transform.parent;
                (Door.GetComponent<Portal>() as Portal).Target = "/Levels/CampoDelCielo.tmx";

                DestroyImmediate(gameObject);
                //this.enabled = false;
                return;
            }


            PlayerTransform = Managers.Game.PlayerPrefab.transform;
            BossArea.center = new Vector2(PlayerTransform.position.x, PlayerTransform.position.y + 0.5f);
            TargetTransform = PlayerTransform;

            ThisTransform = transform;
            AoAnim = ThisTransform.GetComponent<AnimSprite>();
            NewPosition = new Vector3(BossArea.xMin, PlayerTransform.position.y, PlayerTransform.position.z);

            LevelArea = Managers.Display.cameraScroll.levelBounds;

            if (MySize == AoSize.Huge)
                ThisTransform.localScale = Vector3.one * 4;

            //if (Managers.Game.State == Managers.Game.GetComponentInChildren<WorldState4>()) 
            //    ((WorldState4)Managers.Game.State).AoAoReady = true;

            if (MySize != AoSize.Huge && TreeTransform == null)
            {
                float randomizer = Time.timeSinceLevelLoad;

                if (MySize == AoSize.Large)
                    randomizer *= Random.Range(2, ThisTransform.position.x);
                if (MySize == AoSize.Medium)
                    randomizer *= Random.Range(3, ThisTransform.position.x);
                if (MySize == AoSize.Small)
                    randomizer *= Random.Range(4, ThisTransform.position.x);

                //randomizer = Mathf.Clamp( randomizer, 2, 86);

                WalkSpeed = Random.Range(2, ((int)randomizer) % 7);

                GameObject Tree = GameObject.FindGameObjectWithTag("Platform");

                if (Tree)
                {
                    TreeTransform = Tree.transform;
                    TreeScript = TreeTransform.GetComponentInChildren<AoAoThing>();
                }
            }

            Managers.Dialog.Init(file);
        }




        void Update()
        {
            NewPosition = ThisTransform.position;


            switch (AoAoState)
            {
                case BossState.Roaming:
                    Roam();
                    break;
                case BossState.Talking:
                    TalkingState();
                    break;
                case BossState.Searching:
                    Searching();
                    break;
                case BossState.Throwing:
                    Throwing();
                    break;
                case BossState.Hurting:
                    Hurting();
                    break;
                case BossState.Die:
                    Dying();
                    break;

            }

            //NewPosition.y = Mathf.Clamp(NewPosition.y, 1, 50);

            ThisTransform.position = NewPosition;
            Debug.DrawLine(new Vector2(ThisTransform.position.x, ThisTransform.position.y),
                            ThisTransform.position + Vector3.right, Color.red);


            if (DisplayArea)
            {
                Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMin), new Vector2(BossArea.xMax, BossArea.yMin), Color.red);
                Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMax), new Vector2(BossArea.xMax, BossArea.yMax), Color.red);
                Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMin), new Vector2(BossArea.xMin, BossArea.yMax), Color.red);
                Debug.DrawLine(new Vector2(BossArea.xMax, BossArea.yMin), new Vector2(BossArea.xMax, BossArea.yMax), Color.red);
            }
        }
        public bool DisplayArea = true;


        void Roam()
        {
            if (TreeTransform == null)          // The obj Instantiation is slow so AoAO keeps looking for The tree transform
            {
                GameObject Tree = GameObject.FindGameObjectWithTag("Platform");

                if (Tree)
                {
                    TreeTransform = Tree.transform;
                    TreeScript = TreeTransform.GetComponentInChildren<AoAoThing>();
                }
            }

            AoAnim.PlayFrames(1, 0, 4, Orientation);

            NewPosition = new Vector3(NewPosition.x + (Time.deltaTime * Orientation), NewPosition.y, .15f);

            if (NewPosition.x > LevelArea.xMax)
                Orientation = -1;

            if (NewPosition.x < LevelArea.xMin)
                Orientation = +1;

            if (Mathf.Abs(PlayerTransform.position.x - NewPosition.x) < 3)
            {
                Talking = true;
                AoAoState = BossState.Talking;
            }
        }

        void TalkingState()
        {
            if (Talking && !Managers.Dialog.IsInConversation())
                StartTalk();

            if (!Talking && !Managers.Dialog.IsInConversation())
                EndTalk();

            AoAnim.PlayFramesFixed(3, 0, 2, Orientation);
        }

        void StartTalk()
        {
            Talking = false;

            Managers.Dialog.StartConversation("Moñai");

            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 0;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 0.01f;

            BossArea.center = new Vector2(ThisTransform.position.x, ThisTransform.position.y);

            Managers.Display.cameraScroll.ResetBounds(BossArea);
        }

        void EndTalk()
        {
            AoAoState = BossState.Searching;

            Managers.Dialog.StopConversation();

            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 0;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 3.5f;

            Managers.Display.cameraScroll.ResetBounds(LevelArea);
        }


        void Searching()
        {
            if (TreeScript.PlayerStand)
                TargetTransform = TreeTransform;
            //else 
            //    TargetTransform = PlayerTransform;

            if (TargetTransform)
                if (Mathf.Abs(TargetTransform.position.x - ThisTransform.position.x) >
                    ((ThisTransform.localScale.x * .5f) - ThisTransform.localScale.x * .25f)) // si está a 1/2 dist atacar
                    Walking();
                else
                    Attacking();
            else
                Stalk();
        }

        void Walking()
        {
            Orientation = (int)Mathf.Sign(TargetTransform.position.x - ThisTransform.position.x);

            AoAnim.PlayFramesFixed(1, 0, 4, Orientation);
            NewPosition.x += Time.deltaTime * WalkSpeed * Orientation;

            if (Mathf.Abs(PlayerTransform.position.x - ThisTransform.position.x) > 10)
            {
                if (Mathf.Abs(LevelArea.xMin - PlayerTransform.position.x) < 15)
                    NewPosition.x = LevelArea.xMin + 1;
                else if (Mathf.Abs(LevelArea.xMax - PlayerTransform.position.x) < 15)
                    NewPosition.x = LevelArea.xMax - 1;
                else
                    NewPosition.x = PlayerTransform.position.x + (7 * -Orientation);
            }

            (ThisTransform.GetComponent<BoxCollider>() as BoxCollider).center = new Vector3(0, -0.15f, 0);
            (ThisTransform.GetComponent<BoxCollider>() as BoxCollider).size = new Vector3(0.45f, 0.7f, .2f);
        }


        void Attacking()
        {
            if (!TreeScript.PlayerStand && PlayerTransform.position.y > (ThisTransform.position.y + .5f))
            {
                if (TimeLapse == 0)
                    TimeLapse = Time.time + 2;

                //if ( MySize != AoSize.Large )
                AoAnim.PlayFrames(0, 0, 3, Orientation, 5);                      // No Atacar estár quieto
                //else
                //    AoAnim.PlayFrames(1, 0, 1, Orientation);

                if (TimeLapse < Time.time)
                {
                    TimeLapse = 0;
                    TargetTransform = null;                                      // Ir a merodear un rato y dejar tranquilo al Player
                }

                return;
            }
            else if (TargetTransform.position.y < 0)
                TargetTransform = PlayerTransform;
            else
            {
                (ThisTransform.GetComponent<BoxCollider>() as BoxCollider).center =
                    new Vector3(Orientation * .15f, -.325f, 0);

                (ThisTransform.GetComponent<BoxCollider>() as BoxCollider).size = new Vector3(.6f, 0.35f, 1);


                AoAnim.PlayFramesFixed(2, 0, 4, Orientation);                  // Atacar (Arbol)
            }


            //if (!TreeScript.PlayerStand && TargetTransform == PlayerTransform)
            //    PlayerTransform.GetComponent<CharacterController>().Move(new Vector3(Orientation,0, .25f));

            if (TreeScript.PlayerStand && TreeScript.Exists)
            {
                Destroy(Instantiate(Prefabs[2], TreeScript.PivotPosition + Vector3.up, ThisTransform.rotation), 3);
                TreeScript.Exists = false;
            }
        }


        void Stalk()
        {
            if (TimeLapse < Time.time)
            {
                TimeLapse = Time.time + Random.Range(1, Time.timeSinceLevelLoad % 10);

                (ThisTransform.GetComponent<BoxCollider>() as BoxCollider).center = new Vector3(0, -0.15f, 0);
                (ThisTransform.GetComponent<BoxCollider>() as BoxCollider).size = new Vector3(0.45f, 0.7f, .2f);

                if (Random.Range(1, 100) > 50)
                    Orientation *= -1;
            }



            AoAnim.PlayFrames(1, 0, 4, Orientation);                            // Mejor seguír caminando..

            NewPosition = new Vector3(NewPosition.x + (Time.deltaTime * (WalkSpeed * .5f) * Orientation), NewPosition.y, .15f);

            if (PlayerTransform)
            {

                if (NewPosition.x > PlayerTransform.position.x + 7)
                    Orientation = -1;

                if (NewPosition.x < PlayerTransform.position.x - 7)
                    Orientation = +1;

                if (PlayerTransform.position.y < (ThisTransform.position.y))
                    TargetTransform = PlayerTransform;
            }

        }

        void Throwing()
        {
            AoAnim.PlayFramesFixed(2, 0, 4, Orientation);

            if (TimeLapse < Time.time)
            {
                TimeLapse = 0;
                AoAoState = BossState.Searching;
            }
        }

        void Hurting()
        {
            if (TimeLapse == 0)
            {
                Destroy(Instantiate(Prefabs[0], ThisTransform.position, ThisTransform.rotation), 5);
                TimeLapse = Time.time + 4;
            }

            ThisTransform.tag = "Untagged";

            if (TimeLapse > Time.time)
                AoAnim.PlayFrames(0, 2, 2, Orientation);
            else
                Dying();
        }

        void Dying()
        {
            //if (Managers.Game.State == Managers.Game.GetComponentInChildren<WorldState4>())
            //Managers.Game.GetComponentInChildren<WorldState4>().WorldFinished = true;

            Health--;


            if (MySize != AoSize.Small)
            {
                Destroy(Instantiate(Prefabs[1], ThisTransform.position, ThisTransform.rotation), 5);
                Instantiate(Prefabs[4], ThisTransform.position + Vector3.up - Vector3.right, ThisTransform.rotation);
                Instantiate(Prefabs[4], ThisTransform.position + Vector3.up + Vector3.right, ThisTransform.rotation);
                Destroy(ThisTransform.gameObject);
            }
            else
            {
                if (Health <= 0)
                {
                    Managers.Register.AoAoDefeated = true;

                    Instantiate(Prefabs[4], new Vector3((PlayerTransform.position.x + 2),
                                                            ThisTransform.position.y + 4, .3f),
                                                            ThisTransform.rotation);
                    Debug.Log("AoAo Was Destroyed");
                }

                Destroy(Instantiate(Prefabs[1], ThisTransform.position + Vector3.left, ThisTransform.rotation), 5);
                Destroy(Instantiate(Prefabs[1], ThisTransform.position + Vector3.right, ThisTransform.rotation), 5);
                Destroy(Instantiate(Prefabs[1], ThisTransform.position + Vector3.up, ThisTransform.rotation), 5);
                Destroy(Instantiate(Prefabs[0], ThisTransform.position, ThisTransform.rotation), 5);
                Destroy(ThisTransform.gameObject);
            }
        }

        void OnTriggerEnter(Collider hit)
        {
            if (hit.tag == "p_shot" && AoAoState != BossState.Talking)
            {
                hit.transform.position = PlayerTransform.position + (Vector3.right * Orientation * 9);
                AoAoState = BossState.Hurting;
            }

            if (hit.tag == "Player" && MySize != AoSize.Huge &&
                hit.transform.position.y > ThisTransform.position.y + .25f)
            {
                PlayerControls PlayerControl =
                    (PlayerControls)PlayerTransform.GetComponent<PlayerControls>();

                PlayerControl.velocity.y = 3 * ThisTransform.localScale.y;
                AoAoState = BossState.Hurting;
            }
        }

        void OnCollisionEnter(Collision hit)
        {

            if (hit.transform.tag == "pickup" && AoAoState != BossState.Talking)
            {
                if (Managers.Dialog.IsInConversation())
                    EndTalk();

                AoAoState = BossState.Throwing;
                TimeLapse = Time.time + 1;

                var killBox = (GameObject)Instantiate(Prefabs[3], hit.transform.position, hit.transform.rotation);
                killBox.transform.parent = hit.transform;
                hit.transform.rigidbody.AddForce(
                    new Vector3(Orientation,
                    //0, 0) * Random.Range(65, 120), ForceMode.Impulse);
                                        Random.Range(0.35f, 0.65f), 0) * Random.Range(65, 120), ForceMode.Impulse);

                Destroy(killBox, 3);
            }


        }




    }


}
