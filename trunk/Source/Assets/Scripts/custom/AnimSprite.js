#pragma strict

public var RowSize				: int = 1;		// Input the number of Rows of the sprite sheet 
public var ColumnSize			: int = 1;		// Input the number of columns of the sprite sheet 
public var framesPerSecond 		: int = 16;		// Speed of Sprite animation
 
function PlayFrames( rowFrameStart :int, colFrameStart :int, totalframes :int, flipped :int)
{
//	var rowFrameStart : int =  FrameStart % ColumnSize;
//	var colFrameStart : int = Mathf.FloorToInt(FrameStart / ColumnSize ) + ( !rowFrameStart ? 0 : 1 );
	
 var index : int = Time.time * framesPerSecond;							// time control fps
 index = index % totalframes; 
 
 var size : Vector2 = Vector2(  1.0 / ColumnSize , 1.01 / RowSize );	// scale
 
 var u : int = index % ColumnSize;
 var v : int = index / ColumnSize;
 
 // var flipped : int = 1;
// if (Input.GetAxis("Horizontal") < 0 ) flipped = -1;
// if (Input.GetAxis("Horizontal") > 0 ) flipped = 1;
   
 // var offset = Vector2 ( u * size.x, (1 - size.y) - ( v * size.y ) );	// offset
 var offset = Vector2 ( ((u + colFrameStart ) * size.x ) , (1.0 - size.y) - ( v + rowFrameStart ) * size.y ); // offset
 
//  offset.x = (( offset.x * flipped ) - size.x * ( flipped < 0 ? 1 : 0) ) * flipped  ;
   offset.x = (( offset.x * flipped ) - size.x * ( System.Convert.ToByte(flipped < 0) ) ) * flipped  ;

//  offset.x *=  flipped ;
  size.x *= flipped ;
  
 renderer.material.mainTextureOffset = offset ;		// texture offset
 renderer.material.mainTextureScale = size  ;		// texture scale
  
 // renderer.material.SetTextureOffset( "_BumpMap", offset);
 // renderer.material.SetTextureScale( "_BumpMap", size );
}




function PlayFrames( rowFrameStart :int, colFrameStart :int, FPS : int, totalframes :int, flipped :int)
{
 var index : int = Time.time * FPS;										// time control fps
 index = index % totalframes; 
 
 var size : Vector2 = Vector2(  1.0 / ColumnSize , 1.01 / RowSize );	// scale
 
 var u : int = index % ColumnSize;
 var v : int = index / ColumnSize;
   
 // var offset = Vector2 ( u * size.x, (1 - size.y) - ( v * size.y ) );	// offset
 var offset = Vector2 ( ((u + colFrameStart ) * size.x ) , (1.0 - size.y) - ( v + rowFrameStart ) * size.y ); // offset
 

  offset.x = (( offset.x * flipped ) - size.x * ( System.Convert.ToByte(flipped < 0) ) ) * flipped  ;
//  offset.x = (( offset.x * flipped ) - size.x * ( flipped < 0 ? 1 : 0) ) * flipped  ;
  size.x *= flipped ;
  
 renderer.material.mainTextureOffset = offset ;							// texture offset
 renderer.material.mainTextureScale = size  ;							// texture scale
}