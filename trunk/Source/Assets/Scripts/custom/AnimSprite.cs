
using UnityEngine;
using System.Collections;

public class AnimSprite : MonoBehaviour
{

    public int RowSize = 1;		// Input the number of Rows of the sprite sheet 
    public int ColumnSize = 1;		// Input the number of columns of the sprite sheet 
    public int framesPerSecond = 16;		// Speed of Sprite animation



    public void PlayFrames(int rowFrameStart, int colFrameStart, int totalframes, int flipped)
    {
        //	int rowFrameStart =  FrameStart % ColumnSize;
        //	int colFrameStart = Mathf.FloorToInt(FrameStart / ColumnSize ) + ( !rowFrameStart ? 0 : 1 );

        int index = (int)(Time.time * framesPerSecond);							// time control fps
        index = index % totalframes;

        Vector2 size = new Vector2(1.0f / ColumnSize, 1.0f / RowSize);	// scale

        int u = index % ColumnSize;
        int v = index / ColumnSize;

        // int flipped = 1;
        // if (Input.GetAxis("Horizontal") < 0 ) flipped = -1;
        // if (Input.GetAxis("Horizontal") > 0 ) flipped = 1;

        // Vector2 offset= new Vector2 ( u * size.x, (1 - size.y) - ( v * size.y ) );	// offset
        // Vector2 offset= new Vector2 ( ((u + colFrameStart ) * size.x ) , (1.0f - size.y) - ( v + rowFrameStart ) * size.y  ); // offset
        Vector2 offset = new Vector2(((u + colFrameStart) * size.x), (1.0f - size.y) - (v + rowFrameStart) * size.y); // offset

        //  offset.x = (( offset.x * flipped ) - size.x * ( flipped < 0 ? 1 : 0) ) * flipped  ;
        offset.x = ((offset.x * flipped) - size.x * (System.Convert.ToByte(flipped < 0))) * flipped;

        //  offset.x *=  flipped ;
        size.x *= flipped;

        //  if(iY / RowSize == 1)         iY=1;

        renderer.material.mainTextureOffset = offset;		// texture offset
        renderer.material.mainTextureScale = size;		// texture scale

        //if (Input.GetKey(KeyCode.Z)) print(((1.0f - size.y) - (v + rowFrameStart) * size.y));
        // renderer.material.SetTextureOffset( "_BumpMap", offset);
        // renderer.material.SetTextureScale( "_BumpMap", size );
    }


    public void PlayFrames(int rowFrameStart, int colFrameStart, int totalframes, int flipped, int FPS)
    {
        int index = (int)(Time.time * FPS);										// time control fps
        index = index % totalframes;

        Vector2 size = new Vector2(1.0f / ColumnSize, 1.01f / RowSize);	// scale

        int u = index % ColumnSize;
        int v = index / ColumnSize;

        // Vector2 offset= Vector2 ( u * size.x, (1 - size.y) - ( v * size.y ) );	// offset
        Vector2 offset = new Vector2(((u + colFrameStart) * size.x), (1.0f - size.y) - (v + rowFrameStart) * size.y); // offset


        offset.x = ((offset.x * flipped) - size.x * (System.Convert.ToByte(flipped < 0))) * flipped;
        //  offset.x = (( offset.x * flipped ) - size.x * ( flipped < 0 ? 1 : 0) ) * flipped  ;
        size.x *= flipped;

        renderer.material.mainTextureOffset = offset;							// texture offset
        renderer.material.mainTextureScale = size;							// texture scale
    }

    //function PlayFrames(  int rowFrameStart ,   int colFrameStart ,   int totalframes ,   int flipped ,   float HeightFactor  )
    public void PlayFramesFixed(int rowFrameStart, int colFrameStart, int totalframes, int flipped)
    {
        int index = (int)(Time.time * framesPerSecond);							// time control fps
        index = index % totalframes;

        Vector2 size = new Vector2(1.0f / ColumnSize, 1.0f / RowSize);	// scale

        int u = index % ColumnSize;
        int v = index / ColumnSize;

        Vector2 offset = new Vector2(((u + colFrameStart) * size.x), ((1.0f - size.y) - (v + rowFrameStart) * size.y) - 0.0005f); // offset

        offset.x = ((offset.x * flipped) - size.x * (System.Convert.ToByte(flipped < 0))) * flipped;

        size.x *= flipped;

        renderer.material.mainTextureOffset = offset;		// texture offset
        renderer.material.mainTextureScale = size;		// texture scale
    }

    public void PlayFramesFixed(int rowFrameStart, int colFrameStart, int totalframes, int flipped, float fraction)//Ej. 1.005f
    {
        int index = (int)(Time.time * framesPerSecond);							// time control fps
        index = index % totalframes;

        Vector2 size = new Vector2(1.0f / ColumnSize, fraction / RowSize);	// scale

        int u = index % ColumnSize;
        int v = index / ColumnSize;

        Vector2 offset = new Vector2(((u + colFrameStart) * size.x), (1.0f - size.y) - (v + rowFrameStart) * size.y); // offset

        offset.x = ((offset.x * flipped) - size.x * (System.Convert.ToByte(flipped < 0))) * flipped;

        size.x *= flipped;

        renderer.material.mainTextureOffset = offset;		// texture offset
        renderer.material.mainTextureScale = size;		// texture scale
    }

    public void PlayFramesFixedFPS(int rowFrameStart, int colFrameStart, int totalframes, int flipped, float fraction, int FPS) //Ej. 1.005f
    {
        int index = (int)(Time.time * FPS);							// time control fps
        index = index % totalframes;

        Vector2 size = new Vector2(1.0f / ColumnSize, fraction / RowSize);	// scale

        int u = index % ColumnSize;
        int v = index / ColumnSize;

        Vector2 offset = new Vector2(((u + colFrameStart) * size.x), (1.0f - size.y) - (v + rowFrameStart) * size.y); // offset

        offset.x = ((offset.x * flipped) - size.x * (System.Convert.ToByte(flipped < 0))) * flipped;

        size.x *= flipped;

        renderer.material.mainTextureOffset = offset;		// texture offset
        renderer.material.mainTextureScale = size;		// texture scale
    }
}