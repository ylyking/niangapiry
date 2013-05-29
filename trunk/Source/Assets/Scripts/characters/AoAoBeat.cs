using UnityEngine;
using System.Collections;

namespace Bosses
{

    public class AoAoBeat : MonoBehaviour
    {
        int Orientation = -1;                       // This setup the Monster face direction
        float TimeLapse = 0;

        public TextAsset file;

        string nameId1 = "Moñai";
        string nameId2 = "Moñai";

        bool Talking = false;

        public enum SceneState { Start, Collapse, Talking, Transition, Luizon, Door }
        public SceneState CurrentState = SceneState.Start;

        AnimSprite AoAnim;
        //AnimSprite LobiAnim;

        Transform ThisTransform;
        Transform PlayerTransform;

        public GameObject LobiPrefab;
        Transform LobiTransform;

        public GameObject DoorPrefab;

        public Rect AoAoArea;
        public Rect LuisonArea;
        Rect LevelArea;

        void Start()
        {
            Managers.Dialog.Init(file);

            LevelArea = Managers.Display.cameraScroll.levelBounds;

            PlayerTransform = Managers.Game.PlayerPrefab.transform;

            AoAoArea.center = new Vector2(PlayerTransform.position.x, LevelArea.yMin + 3);
            LuisonArea.center = AoAoArea.center;

            ThisTransform = transform;
            AoAnim = ThisTransform.GetComponent<AnimSprite>();


            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 0;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 0.01f;
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 2;

            Managers.Display.cameraScroll.ResetBounds(AoAoArea);

            GameObject Lobizon = (GameObject)Instantiate(LobiPrefab,
                                            new Vector3(LuisonArea.xMin + 2, LuisonArea.yMin + 1, .2f),
                                                ThisTransform.rotation);
            LobiTransform = Lobizon.transform;
            //LobiAnim = LobiTransform.GetComponent<AnimSprite>();

            LobiTransform.gameObject.SetActive(false);
        }

        void Update()
        {
            switch (CurrentState)
            {
                case SceneState.Start:
                    Starting();
                    break;
                case SceneState.Collapse:
                    Collapsing();
                    break;
                case SceneState.Talking:
                    TalkingState();
                    break;
                case SceneState.Transition:
                    Transiting();
                    break;
                case SceneState.Luizon:
                    LobiTalking();
                    break;
                case SceneState.Door:
                    //if (Managers.Game.State != Managers.Game.GetComponentInChildren<WorldState4>())
                    //    Destroy(gameObject);
                    if (Managers.Register.currentLevelFile != "/Levels/AoAoField.tmx")
                        Destroy(gameObject);
                    break;
            }

        }

        void Starting()
        {
            Orientation = (int)Mathf.Sign(PlayerTransform.position.x - ThisTransform.position.x);

            if (TimeLapse == 0)
                TimeLapse = Time.time + 4;

            if (TimeLapse > Time.time)
                AoAnim.PlayFrames(0, 2, 2, Orientation);
            else
            {
                TimeLapse = 0;
                CurrentState = SceneState.Collapse;
            }

        }

        void Collapsing()
        {
            TimeLapse += Time.deltaTime;

            if (TimeLapse < 1)
                AoAnim.PlayFramesFixed(3, (int)((TimeLapse) * 4), 1, Orientation);

            if (TimeLapse > 1)
                AoAnim.PlayFramesFixed(3, 3, 1, Orientation);

            if (TimeLapse > 2)
            {
                TimeLapse = 0;
                CurrentState = SceneState.Talking;
                Talking = true;
            }
        }


        void TalkingState()
        {
            if (Talking && !Managers.Dialog.IsInConversation())
                StartTalk();

            if (!Talking && !Managers.Dialog.IsInConversation())
                EndTalk();

            if (!Talking && Managers.Dialog.IsInConversation())
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x =
                    (ThisTransform.position.x - Managers.Game.PlayerPrefab.transform.position.x) * .5f;
        }

        void StartTalk()
        {
            Talking = false;
            Managers.Dialog.StartConversation(nameId1);
            TimeLapse = 0;
        }

        void EndTalk()
        {
            TimeLapse = 0;
            Managers.Dialog.StopConversation();
            CurrentState = SceneState.Transition;
            //Debug.Log("Ending AO Ao Talking ");

        }

        void Transiting()
        {
            if (TimeLapse == 0)
            {
                LobiTransform.gameObject.SetActive(true);
                //LobiTransform.position = 
            }

            TimeLapse += Time.deltaTime;

            if (TimeLapse > 1)
            {
                Managers.Display.cameraScroll.ResetBounds(LuisonArea);
                //Debug.Log("Ending AO Ao Talking State going to -> Luizon Talk");
                Talking = true;
                CurrentState = SceneState.Luizon;
            }
        }



        void LobiTalking()
        {
            if (Talking && !Managers.Dialog.IsInConversation())
                LobiStart();

            if (!Talking && Managers.Dialog.IsInConversation())
                (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x =
                    (LobiTransform.position.x - Managers.Game.PlayerPrefab.transform.position.x) * .5f;

            if (!Talking && !Managers.Dialog.IsInConversation())
                LobiEnd();

            TimeLapse += Time.deltaTime;

            if (TimeLapse > 30)
            {
                TimeLapse = 0;
                //Instantiate(DoorPrefab, new Vector3(LuisonArea.xMin + 4, LuisonArea.yMin + 1, .3f), ThisTransform.rotation);
            }
        }

        void LobiStart()
        {
            Talking = false;
            //Debug.Log(" Lobizon Start Talking");

            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 3;
            Managers.Display.cameraScroll.ResetBounds(LuisonArea);
            Managers.Dialog.StartConversation(nameId2);
            TimeLapse = 0;
        }

        void LobiEnd()
        {
            //TimeLapse = Time.time + 2;
            Managers.Dialog.StopConversation();
            GameObject Door = (GameObject)Instantiate(DoorPrefab, new Vector3(LuisonArea.xMin + 4, LuisonArea.yMin + 1, .3f), ThisTransform.rotation);
            Door.transform.parent = ThisTransform;
            LobiTransform.parent = ThisTransform;

            CurrentState = SceneState.Door;
        }

        //void OnDestroy()
        //{
        //    Debug.Log("Staff Destroying");

        //    if ( LobiTransform != null)
        //    Destroy(LobiTransform.gameObject);
        //}
    }



}
