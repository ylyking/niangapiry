#pragma strict
// a little script for auto-destroy projectile instances when get out of screen or after some time lapse...

public var LifeTime			: float = 5f;
public var rotationSpeed	: float = 0;	// Add some value for rotation

public var RowSize			: int = 1;		// Input the number of Rows of the sprite sheet 
public var ColumnSize		: int = 1;		// Input the number of columns of the sprite sheet 
public var framesPerSecond 	: int = 12;		// Speed of Sprite animation

public var rowFrameStart	: int = 1;
public var colFrameStart	: int = 1;
public var totalFrames		: int = 1;

private var thisTransform	: Transform;	// own obj's tranform cached
private var moveDirection 	: Vector3 ;		// == orientation * speed (force Magnitude )
private var orientation  	: int = 1;


function Fire( moveSpeed : Vector3, rotSpeed : float)
{
	thisTransform = transform;
	moveDirection 	= moveSpeed; 	// == transform.up * speed
	rotationSpeed 	= rotSpeed;
	orientation 	= Mathf.Sign( moveDirection.x );
		PlayFrames(rowFrameStart, colFrameStart, totalFrames, orientation);
	
	while ( true)
		yield CoUpdate();
}

function FireAnimated( moveSpeed : Vector3, rowStart :int, colStart :int, totalframes :int)
{
	thisTransform = transform;
	moveDirection 	= moveSpeed; 	// == transform.up * speed ( orientation of move + force of impulse )
	rowFrameStart 	= rowStart;
	colFrameStart	= colStart;
	totalFrames		= totalframes;
	orientation 	= Mathf.Sign( moveDirection.x );
	
	while ( true)
	{
		CoUpdate();
		PlayFrames(rowFrameStart, colFrameStart, totalFrames, orientation);
		yield;
	}
}


function CoUpdate () : IEnumerable
{
    	thisTransform.position += Time.deltaTime * moveDirection;							// moveDirection == orientation * speed ;

		thisTransform.RotateAroundLocal( Vector3.forward, rotationSpeed * Time.deltaTime); 	// if > 0, Rotate around own z-axis

 		Destroy(gameObject, LifeTime);
}

function FireBoomerang( moveSpeed : Vector3, rowStart :int, colStart :int, totalframes :int) // Hat throw with a boomerang Fx
{
	thisTransform = transform;
	moveDirection 	= moveSpeed; 	// == transform.up * speed ( orientation of move + force of impulse )
	rowFrameStart 	= rowStart;
	colFrameStart	= colStart;
	totalFrames		= totalframes;
	orientation 	= Mathf.Sign( moveDirection.x );
	
	while ( true)
	{
		CoUpdate();
		moveDirection.x += Mathf.Clamp( Time.deltaTime * 12.0 * -orientation,
												  moveSpeed.x * -orientation,
												  moveSpeed.x *  orientation);
		PlayFrames(rowFrameStart, colFrameStart, totalFrames, Mathf.Sign( moveDirection.x ));
		yield;
	}
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

