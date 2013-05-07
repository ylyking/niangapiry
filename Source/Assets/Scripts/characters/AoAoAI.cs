using UnityEngine;
using System.Collections;

public class AoAoAI : MonoBehaviour {


    int Orientation = -1;                       // This setup the Monster face direction
    int AttackRange = 0;                        // number of Wipes before change to next behaviour
    int Health = 5;                        // total hits needed to kill it
    int HoldStaff = 0;

    public TextAsset file;
    bool Fighting = false;
    bool Talking = false;

    public enum BossState { Standby = 0, Talking, Walking, Attacking, Hurting, Die }
    public BossState AoAoState = BossState.Standby;
    AnimSprite AoAnim;

    Transform ThisTransform;
    Transform PlayerTransform;
    Vector3 NewPosition;
    public Rect BossArea;

    public GameObject[] Prefabs;
     GameObject MovingFloor;
     GameObject Tree1;
     GameObject Tree2;
     GameObject Stone;


	void Start () 
    {
        (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 0;
        (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 0;
        (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 2.5f;

        //AoAoState = BossState.Talking;
        //Talking = true;
        AoAoState = BossState.Walking;

        PlayerTransform = Managers.Game.PlayerPrefab.transform;
        BossArea.center = new Vector2(PlayerTransform.position.x, PlayerTransform.position.y + 0.5f);

        ThisTransform = transform;
        AoAnim = ThisTransform.GetComponent<AnimSprite>();
        NewPosition = new Vector3(BossArea.xMin, PlayerTransform.position.y, PlayerTransform.position.z);

        MovingFloor = (GameObject)Instantiate(Prefabs[0], PlayerTransform.position, PlayerTransform.rotation);

	}



	
	void Update ()
    {
        //if ( Fighting )

        //PlayerPos = 

        MovingFloor.transform.position = Vector3.right * PlayerTransform.position.x;
        NewPosition = new Vector3(BossArea.xMin, BossArea.yMin + (ThisTransform.localScale.y * .5f), PlayerTransform.position.z);


        switch (AoAoState)
        {
            case BossState.Talking:
                TalkingState();
                break;
            case BossState.Walking:
                break;
            case BossState.Attacking:
                break;
            case BossState.Hurting:
                break;
            case BossState.Die:
                break;

        }

        ThisTransform.position = NewPosition;

        if (DisplayArea)
        {
            Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMin), new Vector2(BossArea.xMax, BossArea.yMin), Color.red);
            Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMax), new Vector2(BossArea.xMax, BossArea.yMax), Color.red);
            Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMin), new Vector2(BossArea.xMin, BossArea.yMax), Color.red);
            Debug.DrawLine(new Vector2(BossArea.xMax, BossArea.yMin), new Vector2(BossArea.xMax, BossArea.yMax), Color.red);
        }
	}
    public bool DisplayArea = true;


    void Walking()
    {
        AoAnim.PlayFrames(0 + HoldStaff, 2, 4, Orientation, 5);

        NewPosition = new Vector3(NewPosition.x + (Time.deltaTime * 2 * Orientation), BossArea.yMin + .5f, .25f);

        if (NewPosition.x > BossArea.xMax)
            Orientation = -1;

        if (NewPosition.x < BossArea.xMin)
            Orientation = +1;

        //if (Input.GetKeyDown("a"))
        //    Shield.SetActive(!Shield.activeSelf);
    }

    void TalkingState()
    {
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

    }

    void EndTalk()
    {
        AoAoState = BossState.Walking;

        Managers.Dialog.StopConversation();

        (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 0;
        (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 0;
        (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 3;

        //timeLapse = 0;
    }


}
