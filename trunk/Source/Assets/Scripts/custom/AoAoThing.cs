using UnityEngine;
using System.Collections;

public class AoAoThing : MonoBehaviour {

    public enum TreeType {Default, Pindo, Rock }
    public TreeType MyType = TreeType.Default;

    public bool Exists = true;
    public bool PlayerStand = false;

    int Orientation = 1;
    float Height = 1;

    float Angle = 0;

    Transform ThisTransform;
    Vector3 NewPosition;
    public Vector3 PivotPosition = Vector3.zero;
    Transform PlayerTransform;
    PlayerControls playerControl;



	void Start () 
    {
        if (MyType == TreeType.Rock)
            ThisTransform = transform;
        else
            ThisTransform = transform.parent;

        Height = ThisTransform.position.y;

        PlayerTransform = Managers.Game.PlayerPrefab.transform;
        playerControl = PlayerTransform.GetComponent<PlayerControls>();
	
	}
	
	void Update () 
    {
        NewPosition = ThisTransform.position;
        PivotPosition = ThisTransform.position + (Vector3.down * 3);


        if (PlayerTransform && ThisTransform)
            if ( Mathf.Abs(PlayerTransform.position.x - ThisTransform.position.x) > 12)// Out Camera view Rearrange Position
            {
                Orientation = playerControl.orientation;

                if (MyType == TreeType.Default)
                {
                    NewPosition.x = PlayerTransform.position.x + (8 * Orientation);


                    if (!Exists && (ThisTransform.position.y < Height - 5))                 // If we are a Tree and don't exist..
                    {
                        foreach (Transform child in ThisTransform)
                            child.rotation = Quaternion.Euler(Vector3.zero);                // Setup this same one again

                        NewPosition = new Vector3(-5, Height);
                        Exists = true;
                        PlayerStand = false;
                    }
                }

                if (MyType != TreeType.Default)
                {
                    NewPosition.x = PlayerTransform.position.x + (Random.Range(8, 15) * Orientation);

                }
            }


        if (!Exists)                                                    // If AoAo Destroys Tree it starts to crush down
        {
            NewPosition.y -= Time.deltaTime;

            if (PlayerTransform.position.y < Height - 2)                // Even with the player on!
            {
                PlayerTransform.parent = null;
                PlayerStand = false;
            }


            Angle = (Mathf.Sin(Time.time * 7) * .05f);

            //PivotPosition = ThisTransform.position + (Vector3.down * 3);

            foreach (Transform child in ThisTransform)
                child.RotateAround( PivotPosition, Vector3.forward, Angle);
        }


        ThisTransform.position = NewPosition;

        if (MyType == TreeType.Rock && Managers.Game.State != Managers.Game.GetComponentInChildren<WorldState4>())
           //Managers.Register.CurrentLevel != "/Levels/AoAoSkyfield.tmx"
            Destroy(gameObject);

	}

    /// ///////////////////////////////////////////////////////////////////////////////////////////////



    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == "Player" && MyType == TreeType.Default)
        {
            PlayerStand = true;
            PlayerTransform.parent = ThisTransform;
        }

    }

    void OnTriggerExit(Collider hit)
    {
        if (hit.tag == "Player" && MyType == TreeType.Default)
        {
            PlayerStand = false;
            PlayerTransform.parent = null;
        }

    }

    void OnCollisionStay(Collision hit)
    {
        if (MyType == TreeType.Rock && ThisTransform.tag != "pickup")
            ThisTransform.tag = "pickup";
    }

    void OnDestroy()
    {
        playerControl = null;
        PlayerTransform = null;
        ThisTransform = null;
    }
}
