#pragma strict
// a little script for auto-destroy projectile instances when get out of screen or after some time lapse...

public var LifeTime			: float = 5f;
public var speed			: float = 5f;
public var rotationSpeed	: float = 0;

//private var Fired			: boolean = false;
private var moveDirection 	: Vector3 ;

public var RowSize			: int = 1;		// Input the number of Rows of the sprite sheet 
public var ColumnSize		: int = 1;		// Input the number of columns of the sprite sheet 
public var framesPerSecond 	: int = 12;		// Speed of Sprite animation

public var rowFrameStart	: int = 1;
public var colFrameStart	: int = 1;
public var totalFrames		: int = 1;

function Fire( moveSpeed : Vector3, rotSpeed : float)
{
	moveDirection 	= moveSpeed; 	// == transform.up * speed
	rotationSpeed 	= rotSpeed;
		PlayFrames(rowFrameStart, colFrameStart, totalFrames, Mathf.Sign( moveDirection.x ));
	
	while ( true)
		yield CoUpdate();
//	Fired 			= true;
}

function FireAnimated( moveSpeed : Vector3, rowStart :int, colStart :int, totalframes :int)
{
	moveDirection 	= moveSpeed; 	// == transform.up * speed ( orientation of move + force of impulse )
	rowFrameStart 	= rowStart;
	colFrameStart	= colStart;
	totalFrames		= totalframes;
	
	while ( true)
	{
		CoUpdate();
		PlayFrames(rowFrameStart, colFrameStart, totalFrames, Mathf.Sign( moveDirection.x ));
		yield;
	}
//	Fired 			= true;
}


function CoUpdate () : IEnumerable
{
//	if (Fired)
//	{
    	transform.position += Time.deltaTime * moveDirection;		// moveDirection = orientation * speed ;

		transform.RotateAroundLocal( Vector3.forward, rotationSpeed * Time.deltaTime); // if > 0, Rotate around own z-axis

// 	}
 		Destroy(gameObject, LifeTime);
 		
// 		yield;
}


function PlayFrames( rowFrameStart :int, colFrameStart :int, totalframes :int, flipped :int)
{
 var index 	: int = Time.time * framesPerSecond;							// time control fps
 index = index % totalFrames; 
 
 var size 	: Vector2 = Vector2(  1.0 / ColumnSize , 1.01 / RowSize );	// scale
 
 var u 		: int = index % ColumnSize;
 var v 		: int = index / ColumnSize;
   
 var offset = Vector2 ( ((u + colFrameStart ) * size.x ) , (1.0 - size.y) - ( v + rowFrameStart ) * size.y ); // offset
 
  offset.x = (( offset.x * flipped ) - size.x *  System.Convert.ToByte(flipped < 0)  ) * flipped  ;
  size.x *= flipped ;
  
 renderer.material.mainTextureOffset = offset ;		// texture offset
 renderer.material.mainTextureScale = size  ;		// texture scale
}


//function Start ()
//{
//	TotalTime = Time.time + LifeTime;	
//	moveDirection = transform.up;
//}

//function Update ()
//{
//	if (Fired)
//	{ 
//    transform.position += Time.deltaTime * speed * moveDirection;
//	
//	  rotation  = rotationSpeed * Time.deltaTime;
//		transform.RotateAroundLocal( Vector3.forward, rotation);
//	
//	if ( Mathf.Abs( transform.position.y) > ScreenOffset || Mathf.Abs( transform.position.x) > ScreenOffset )
//	if ( Mathf.Abs( transform.position.y) > ScreenOffset || Time.time > TotalTime )
//  { Destroy(gameObject); }
//}