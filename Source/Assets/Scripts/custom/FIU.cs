using UnityEngine;
using System.Collections;

public class FIU : MonoBehaviour 
{

    public int orientation = 0;
    float Speed = 4;
    float Brake = 3.75f;

    public enum CharType { F, I, U, i };
    public CharType myChar = CharType.F;
    public GameObject nextRightPrefab;
    public GameObject nextLeftPrefab;

    bool isSolid = false;
    bool isReady = true;
    bool isFaded = false;

    public float TimeLapse = .5f;
    public float TimeDelay = 1;
    Transform thisTransform;

	// Use this for initialization
	void Start () 
    { 
        //renderer.material.SetColor("_MainTex", new Color(.5f, .5f, .5f, 1));
        //Debug.Log(" Geting color info: " + renderer.material.tex.GetColor()));
        //renderer.material.color = new Color(.5f, .5f, .5f, .25f);
        //renderer.material.color = Color.white;
        thisTransform = transform;
        thisTransform.localScale = new Vector3(0.1f, 0.1f, 1);

	}
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log(" Geting color info: " + renderer.material.GetColor());
        //isHold = Input.GetButtonDown("Fire2");

        if (orientation != 0 && !isSolid)
            Flowing();

        TimeLapse -= Time.deltaTime;

        if (isReady && TimeLapse < .25f)
            Spawn();


        if (TimeLapse <= 0)
            EndCycle();

        //Managers.Display.DebugText =  " InputHold: " + !Input.GetButtonUp("Fire2") + " TimeLapse: " + TimeLapse ;

	}

    void Flowing()
    {
        thisTransform.position += Vector3.right * orientation * Speed * Time.deltaTime;
        Speed -= Brake * Time.deltaTime;

        thisTransform.localScale += new Vector3(Time.deltaTime, Time.deltaTime, 0) * .6f;

        if (isFaded)
        {
            TimeDelay -= Time.deltaTime;
            //renderer.material.color = new Color(1, 1, 1, TimeDelay);
            //Debug.Log(renderer.material.GetColor("_TintColor"));
            renderer.material.SetColor("_TintColor", new Color(1, 1, 1, TimeDelay));

            if (TimeDelay < 0)
                Destroy(thisTransform.gameObject, 5);
        }

    }

    void Spawn()
    {
        isReady = false;

        if (orientation == 1 && myChar != CharType.i)             // Fin Sentido Lectura Normal              
        {
            GameObject NextChar = (GameObject)Instantiate(nextRightPrefab, thisTransform.position, thisTransform.rotation);
            NextChar.GetComponent<FIU>().orientation = 1;
        }

        if (orientation == -1 && myChar != CharType.F)            // Fin Sentido Lectura japonés
        {
            GameObject NextChar = (GameObject)Instantiate(nextLeftPrefab, thisTransform.position, thisTransform.rotation);
            NextChar.GetComponent<FIU>().orientation = -1;
        }
    }

    void EndCycle()
    {
        //isSolid = true;

        //if (TimeLapse > -2)
        //    return;
        if ((TimeLapse < -1 && TimeLapse > -2) && Input.GetButton("Fire2"))
            isSolid = true;

        if (isSolid)
        {
            renderer.material.SetColor("_TintColor", new Color(1, 1, 1, 1));
            thisTransform.localScale = Vector3.one;
            rigidbody.isKinematic = false;
            thisTransform.collider.enabled = true;
            rigidbody.useGravity = true;

            if (TimeLapse < -60)
            {
                TimeDelay -= Time.deltaTime;
                renderer.material.SetColor("_TintColor", new Color(1, 1, 1, TimeDelay));

                if (TimeDelay < 0)
                    Destroy(thisTransform.gameObject, .5f);
            }
        }

        else if (Speed < 0 )
        {
            //Speed = 3;
            //TimeDelay -= Time.deltaTime;
            isFaded = true;
            Brake = 0;
            Speed = 1;
            //renderer.material.color = new Color(1, 1, 1, TimeDelay * .5f);

            //Debug.Log(renderer.material.color);
        }
    }

}
