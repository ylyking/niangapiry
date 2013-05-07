using UnityEngine;
using System.Collections;

public class YasiYatereIA : MonoBehaviour {

    AnimSprite Anime = null;
    Transform thisTransform = null;
    Transform PlayerTransform = null;

    #region Powers Prefabs
    public GameObject[] Powers;                     // 0) Shield  1) RayStart  2) RayTrace  3) RayEnd  4) FireStaff 

    GameObject Shield = null;
    GameObject Staff = null;

    GameObject lightStart = null;
    GameObject lightTrace = null;
    GameObject lightHead = null;

    AnimSprite lightStartAnim;
    AnimSprite lightHeadAnim;
    //AnimSprite FireStaffAnim;
    #endregion

    int Orientation     = -1;                       // This setup the Monster face direction
    //int AttackRange     = 0;                        // number of Wipes before change to next behaviour
    int Health          = 5;                        // total hits needed to kill it
    int HoldStaff       = 0;
    int StaffOrientation = 1;

    float timeLapse     = 0;
    float timeState     = 0;
    float StaffSpeed    = 4;

    Vector3 NewPosition = Vector3.zero;
    Vector3 StaffPosition = Vector3.zero;
    //Vector3 TargePosition = new Vector3(1, 4);

    public bool DisplayArea = false;
    public Rect BossArea = new Rect(0, 0, 15, 5);
    private Rect PrevArea;

    public enum BossState { Standby = 0, Quiet, Running, Hurting, Crying }
    public BossState YasiState = BossState.Standby;

    public enum AttackState { NoAttack = 0, AttackRay, AttackFire }
    public AttackState Attack = AttackState.NoAttack;

    public TextAsset file;
    bool Talking = false;
    public ParticleSystem particle;

    public AudioClip Hit;
    public AudioClip Laser;
    public AudioClip Explode;
    public AudioClip Electric;

    void Start()
    {
        thisTransform = this.transform;
        Anime = GetComponent<AnimSprite>();
        thisTransform.position = new Vector3(thisTransform.position.x, thisTransform.position.y, .25f); ;

        BossArea.center = new Vector2(thisTransform.position.x, thisTransform.position.y + 0.5f);
        PlayerTransform = Managers.Game.PlayerPrefab.transform;

        Shield = (GameObject)Instantiate(Powers[0], thisTransform.position , thisTransform.rotation);

        lightStart = (GameObject)Instantiate(Powers[1], thisTransform.position, thisTransform.rotation);
        lightStartAnim = lightStart.GetComponent<AnimSprite>();
        lightStart.SetActive(false);

        lightTrace = (GameObject)Instantiate(Powers[2], thisTransform.position, thisTransform.rotation);
        lightTrace.SetActive(false);

        lightHead = (GameObject)Instantiate(Powers[3], thisTransform.position, thisTransform.rotation);
        lightHeadAnim = lightHead.GetComponent<AnimSprite>();
        lightHead.SetActive(false);

        Staff = (GameObject)Instantiate(Powers[4], thisTransform.position, thisTransform.rotation);
        Staff.name = "FireStaff";
        //FireStaffAnim = Staff.GetComponent<AnimSprite>();
        Staff.SetActive(false);

        Managers.Dialog.Init(file);
        //YasiState = BossState.Quiet;
        //YasiState = BossState.Hurting;
        //Attack = AttackState.NoAttack;
        timeState = Time.time + 5;
    }




    void Update()
    {
        NewPosition = thisTransform.position;
        Shield.transform.position = thisTransform.position + Vector3.back;
        Shield.SetActive(HoldStaff == 0);                               // If YasiYatere don't had Staff then neither shield

        switch (YasiState)                  // Default 
        {
            case BossState.Standby:
                if ( ((thisTransform.position.x - Managers.Game.PlayerPrefab.transform.position.x) < 5) )
                    
                    {
                        PrevArea = Managers.Display.cameraScroll.levelBounds;
                        Rect BossBounds = new Rect(BossArea.xMin, BossArea.yMin - 1, BossArea.width, BossArea.height + 3);
                        Managers.Display.cameraScroll.ResetBounds(BossBounds);

                        YasiState = BossState.Running;
                    }
                break;

            case BossState.Running:
                CheckState();
                Running();
                break;
            case BossState.Quiet:
                CheckState();
                Standing();
                break;
            case BossState.Hurting:
                Hurt();
                break;
            case BossState.Crying:
                Cry();
                break;
        }

        switch (Attack)
        {
            case AttackState.NoAttack:          // Puede tenér o no Baculo
                NoAttack();
                break;
            case AttackState.AttackRay:         // No Hacer nada si no Tiene Baculo
                AttackRay();
                break;
            case AttackState.AttackFire:        // Pierde Baculo al tirarlo
                AttackStaff();
                break;
        }


        thisTransform.position = NewPosition;

        if (DisplayArea)
        {
            //Debug.DrawLine(TargePosition + Vector3.right, TargePosition - Vector3.right, Color.yellow);
            //Debug.DrawLine(TargePosition + Vector3.up, TargePosition + Vector3.down, Color.yellow);

            Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMin), new Vector2(BossArea.xMax, BossArea.yMin), Color.red);
            Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMax), new Vector2(BossArea.xMax, BossArea.yMax), Color.red);
            Debug.DrawLine(new Vector2(BossArea.xMin, BossArea.yMin), new Vector2(BossArea.xMin, BossArea.yMax), Color.red);
            Debug.DrawLine(new Vector2(BossArea.xMax, BossArea.yMin), new Vector2(BossArea.xMax, BossArea.yMax), Color.red);
        }
    }

    void CheckState()
    {
        timeState -= Time.deltaTime;

        //if (timeState < Time.time)
        if (timeState <= 0)
        {
            timeState = Random.Range(1, 10);

            //Debug.Log("new state: " + timeState);

            if (HoldStaff != 0)                             // Sino tiene Bastón, A correr
            {
                YasiState = BossState.Running;
                Orientation *= -1;
                return;
            }

            else if (Random.Range(0, 100) < 50)                         // Sino Correr o Quedarse quieto al azar...
                YasiState = BossState.Quiet;
            else
            {
                YasiState = BossState.Running;

                if (Attack == AttackState.NoAttack && Random.Range(0, 100) > 65)
                    Orientation *= -1;
            }

            if (Attack == AttackState.NoAttack)       // Las Randomizaciones de Ataque son llamadas al finalizar cada una
                CheckAttack();                         // Excepto NoAttack que la checamos aqui en transiciónes generales.
            
        }
    }

    void CheckAttack()
    {
       float randomizer = Random.Range(0, 100);

       if (randomizer < 33)   
          Attack = AttackState.AttackRay;
       else if (randomizer < 66)
          Attack = AttackState.NoAttack;
       else
           Attack = AttackState.AttackFire;

    }

    void Standing()
    {
        //Orientation = (int)Mathf.Sign(TargePosition.x - thisTransform.position.x);
        Orientation = (int)Mathf.Sign(PlayerTransform.position.x - thisTransform.position.x);

        if ( Attack == AttackState.AttackRay)
             Anime.PlayFrames(0 + HoldStaff, 3, 1, Orientation);
        else
            Anime.PlayFrames(0 + HoldStaff, 0, 1, Orientation);

    }

    void Running()
    {
        Anime.PlayFrames(0 + HoldStaff, 2, 4, Orientation, 5);

        NewPosition = new Vector3(NewPosition.x + (Time.deltaTime * 2 * Orientation),  BossArea.yMin +.5f , .25f);  

        if (NewPosition.x > BossArea.xMax )
            Orientation = -1;

        if (NewPosition.x < BossArea.xMin )
            Orientation = +1;

        //if (Input.GetKeyDown("a"))
        //    Shield.SetActive(!Shield.activeSelf);
    }


    void AttackRay()
    {
        if (timeLapse == 0)
        {
            Orientation = (int)Mathf.Sign(PlayerTransform.position.x - thisTransform.position.x);
            Managers.Audio.Play(Laser, lightHead.transform );
        }

        timeLapse += Time.deltaTime;

        if (timeLapse < 1.5f)
        {
            lightHead.SetActive(true);
            lightHead.transform.position = thisTransform.position + (Vector3.right * Orientation) + Vector3.back;
            lightHeadAnim.PlayFrames(5, 6, 2, Orientation);
            lightStart.SetActive(false);
            lightTrace.SetActive(false);
        }

        if (timeLapse > 1.5f && timeLapse < 2)
        {
            lightHead.SetActive(false);

            lightStart.SetActive(true);
            lightStart.transform.position = thisTransform.position + (Vector3.right * Orientation) + Vector3.down * .1f;
            lightStartAnim.PlayFrames(5, 0, 3, Orientation);

            lightTrace.SetActive(true);
            lightTrace.transform.position = thisTransform.position + (Vector3.right * 11.5f * Orientation) + Vector3.down * .1f;
        }

        if (timeLapse > 2)
        {
            timeLapse = 0;
            lightStart.SetActive(false);
            lightTrace.SetActive(false);
            lightHead.SetActive(false);

            //CheckAttack();
            timeState += 2;
            Attack = AttackState.NoAttack;
        }
    }
    
    void AttackStaff()
    {
        if (timeLapse == 0)
        {
            StaffOrientation = (int)Mathf.Sign(PlayerTransform.position.x - thisTransform.position.x);
            StaffPosition = thisTransform.position + (Vector3.right * StaffOrientation) + Vector3.back; ;
        }

        timeLapse += Time.deltaTime;

        if (timeLapse < 1)
        {
            HoldStaff = 1;
            Anime.PlayFrames(2, 0, 1, StaffOrientation);
            Staff.SetActive(true);
            StaffPosition += Vector3.right * Time.deltaTime * (StaffSpeed - timeLapse) * StaffOrientation;
        }

        if (timeLapse > 1)
        {
            //Anime.PlayFrames(0 + HoldStaff, 0, 1, Orientation);
            StaffPosition += Vector3.right * Time.deltaTime * (StaffSpeed - timeLapse) * StaffOrientation;
        }

        Staff.transform.position = StaffPosition;
        Staff.transform.RotateAroundLocal(Vector3.forward, Time.deltaTime * -45);


        if (timeLapse > 5 && HoldStaff == 0)
        {
            timeLapse = 0;
            Staff.SetActive(false);

            //CheckAttack();
            timeState += 1;
            Attack = AttackState.NoAttack;
        }

    }

    void NoAttack()
    {
        // No Attack, does nothig, maybe we are running or stand, maybe crying or whatever..
        ;
    }

    float hurtTime = 0;

    void Hurt()
    {
        if (hurtTime == 0)
        {
            Health--;
            Destroy(Instantiate(particle, thisTransform.position, thisTransform.rotation), 5);
            //HoldStaff = false;

            if (Health <= 0)
            {
                YasiState = BossState.Crying;
                Talking = true;
                return;
            }

            hurtTime = Time.time + 3;
        }


        Anime.PlayFrames(0 + HoldStaff, 0, 2, Orientation, 18);
        renderer.enabled = (Mathf.RoundToInt(Time.time * 32) % 2 == 0);



        if (hurtTime < Time.time)
        {
            hurtTime = 0;
            renderer.enabled = true;
            YasiState = BossState.Running;
            //HoldStaff = true;
        }

    }

    void Cry()
    {
        Shield.SetActive(false);
        lightHead.SetActive(false);
        lightTrace.SetActive(false);
        lightStart.SetActive(false);
        Staff.SetActive(false);
        Anime.PlayFramesFixed( 2, 1, 2, Orientation );

        if (Talking && !Managers.Dialog.IsInConversation())
            StartTalk();

        if( !Talking && Managers.Dialog.IsInConversation())
            (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x =
                (transform.position.x - Managers.Game.PlayerPrefab.transform.position.x) * .5f;


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

        //PrevArea = Managers.Display.cameraScroll.levelBounds;
        //Rect BossBounds = new Rect(BossArea.xMin, BossArea.yMin - 1, BossArea.width, BossArea.height + 2);
        //Managers.Display.cameraScroll.ResetBounds(BossBounds);
    }

    void EndTalk()
    {
        Managers.Dialog.StopConversation();


        (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.y = 0;
        (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).Offset.x = 0;
        (Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 2.5f;

        Managers.Display.cameraScroll.ResetBounds(PrevArea);

    }




    void OnTriggerEnter(Collider hit)
    {

        if (hit.tag == "Player" && (HoldStaff != 0) &&
            (hit.transform.position.y > thisTransform.position.y + .1f))
        {

            PlayerControls PlayerControl = (PlayerControls)PlayerTransform.GetComponent<PlayerControls>();
            PlayerControl.velocity.y =10;
            YasiState = BossState.Hurting;
            //Managers.Audio.Play(SnakeHit, thisTransform, 1, 1);
            Destroy(Instantiate(particle, thisTransform.position, thisTransform.rotation), 5);

        }

        if (hit.name == "FireStaff" && (HoldStaff != 0) )
        {
            hit.transform.position = thisTransform.position;
            hit.gameObject.SetActive(false);
            HoldStaff = 0;
            CheckAttack();
            //Managers.Audio.Play(SnakeHit, thisTransform, 1, 1);
        }
    }

    void OnTriggerStay(Collider hit)
    {
        if (hit.tag == "Player" && (YasiState == BossState.Crying) &&(this.InputUp) )
        {
            StartTalk();
        }
    }

    bool ToggleUp = true;
    bool InputUp                             // This it's a little oneShot Up Axis check for doors & like   
    {
        get
        {
            if (Input.GetAxis("Vertical") == 0)                      // It's like an "Input.GetAxisDown" 
                ToggleUp = true;

            if (ToggleUp == true && Input.GetAxis("Vertical") > 0)
            {
                ToggleUp = false;
                return true;
            }
            return false;
        }
    }


}
