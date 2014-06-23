// a little script for auto-destroy projectile instances when get out of screen or after some time lapse...

using UnityEngine;
using System.Collections;

public class BulletShot : MonoBehaviour
{

    public float LifeTime = 5f;
    public float rotationSpeed = 0;	// Add some value for rotation

    public int RowSize = 1;		// Input the number of Rows of the sprite sheet 
    public int ColumnSize = 1;		// Input the number of columns of the sprite sheet 
    public int framesPerSecond = 12;		// Speed of Sprite animation

    public int rowFrameStart = 1;
    public int colFrameStart = 1;
    public int totalFrames = 1;

    private Transform thisTransform;	// own obj's tranform cached
    private Vector3 moveDirection;		// == orientation * speed (force Magnitude )
    private int orientation = 1;


    public void Fire(Vector3 moveSpeed, float rotSpeed)
    {
        thisTransform = transform;
        moveDirection = moveSpeed; 	// == transform.up * speed
        rotationSpeed = rotSpeed;
        orientation = (int)Mathf.Sign(moveDirection.x);
        PlayFrames(rowFrameStart, colFrameStart, totalFrames, orientation);

        //while (true)
        //    yield return new CoUpdate();
        StartCoroutine(CoUpdate());
    }

    public void FireAnimated(Vector3 moveSpeed, int rowStart, int colStart, int totalframes)
    {
        thisTransform = transform;
        moveDirection = moveSpeed; 	// == transform.up * speed ( orientation of move + force of impulse )
        rowFrameStart = rowStart;
        colFrameStart = colStart;
        totalFrames = totalframes;
        orientation = (int)Mathf.Sign(moveDirection.x);

        StartCoroutine(MoveAnimated());
    }

    IEnumerator MoveAnimated()
    {
        StartCoroutine(CoUpdate());

        while (true)
        {
            PlayFrames(rowFrameStart, colFrameStart, totalFrames, orientation);
            yield return 0;
        }
    }


    public void FireBall(Vector3 moveSpeed, int rowStart, int colStart, int totalframes)
    {
        thisTransform = transform;
        moveDirection = moveSpeed; 	// == transform.up * speed ( orientation of move + force of impulse )
        rowFrameStart = rowStart;
        colFrameStart = colStart;
        totalFrames = totalframes;
        orientation = (int)Mathf.Sign(moveDirection.x);

        StartCoroutine(MoveFlamed());
    }

    IEnumerator MoveFlamed()
    {
        StartCoroutine(CoUpdate());

        while (true)
        {
            PlayFrames(rowFrameStart, colFrameStart, totalFrames, orientation);
            thisTransform.localScale = Vector3.one + (Vector3.up * Random.Range(0, 2));
            yield return 0;
        }
    }

   //void Start()
   // {
   //    if ( totalFrames > 1 )
   //        FireAnimated(Vector3.zero, rowFrameStart, colFrameStart, totalFrames);
   // }




    public void FireBoomerang(Vector3 moveSpeed, int rowStart, int colStart, int totalframes) // Hat throw with a boomerang Fx
    {
        thisTransform = transform;
        moveDirection = moveSpeed; 	// == transform.up * speed ( orientation of move + force of impulse )
        rowFrameStart = rowStart;
        colFrameStart = colStart;
        totalFrames = totalframes;
        orientation = (int)Mathf.Sign(moveDirection.x);

        StartCoroutine(MoveBoomerang(moveSpeed));
    }

    IEnumerator MoveBoomerang(Vector3 moveSpeed)
    {
        StartCoroutine( CoUpdate() );

        while (true)
        {
            moveDirection.x += Mathf.Clamp(Time.deltaTime * 12.0f * -orientation,
                                                      moveSpeed.x * -orientation,
                                                      moveSpeed.x * orientation);
            PlayFrames(rowFrameStart, colFrameStart, totalFrames, (int)Mathf.Sign(moveDirection.x));
            yield return 0;
        }
    }


    IEnumerator CoUpdate()
    {
        while (true)
        {
            thisTransform.position += Time.deltaTime * moveDirection;							// moveDirection == orientation * speed ;

            thisTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime); 	// if > 0, Rotate around own z-axis

            Destroy(gameObject, LifeTime);

            yield return 0;
        }
    }
    void PlayFrames(int rowFrameStart, int colFrameStart, int totalframes, int flipped)
    {
        int index = (int)(Time.time * framesPerSecond);							// time control fps
        index = index % totalFrames;

        Vector2 size = new Vector2(1.0f / ColumnSize, 1.01f / RowSize);	// scale

        int u = index % ColumnSize;
        int v = index / ColumnSize;

        Vector2 offset = new Vector2(((u + colFrameStart) * size.x), (1.0f - size.y) - (v + rowFrameStart) * size.y); // offset

        offset.x = ((offset.x * flipped) - size.x * System.Convert.ToByte(flipped < 0)) * flipped;
        size.x *= flipped;

        renderer.material.mainTextureOffset = offset;		// texture offset
        renderer.material.mainTextureScale = size;		// texture scale
    }

}