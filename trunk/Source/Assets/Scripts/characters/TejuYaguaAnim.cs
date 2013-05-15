using UnityEngine;
using System.Collections;

public class TejuYaguaAnim : MonoBehaviour {

    Transform thisTransform;
    AnimSprite Anima;
    Transform PlayerTransform;

    Vector3 NewPosition;
    bool StartMoving = false;
    int Orientation = 1;


	// Use this for initialization
	void Start () 
    {
        thisTransform = transform;
        Anima = GetComponent<AnimSprite>();
        NewPosition = new Vector3(thisTransform.position.x, thisTransform.position.y, 10);
        thisTransform.position = NewPosition;
        thisTransform.localScale = new Vector3(3, 3, 1);

        //thisTransform.localScale = new Vector3(4, 4, 1);

        if (Managers.Game.PlayerPrefab)
            PlayerTransform = Managers.Game.PlayerPrefab.transform;

        enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        NewPosition = thisTransform.position;

	    if ( ( Mathf.Abs(PlayerTransform.position.x - NewPosition.x) < 2) &&
            ( Mathf.Abs(PlayerTransform.position.y - NewPosition.y) < 3) )
                StartMoving = true;

        if (StartMoving)
        {
            Anima.PlayFramesFixed(0, 0, 3, Orientation);
            NewPosition = new Vector3(NewPosition.x + (Time.deltaTime * 4 * Orientation), NewPosition.y, NewPosition.z);


            if (NewPosition.x > PlayerTransform.position.x + 10)
                Destroy(gameObject);
        }
        else
            Anima.PlayFramesFixed(1, 0, 3, Orientation, 1.005f);

        thisTransform.position = NewPosition;

    }


    void OnBecameVisible()
    {
        enabled = true;
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }
}
