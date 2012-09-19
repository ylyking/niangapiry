#pragma strict

public var RowStart 			: int = 0;
public var ColumnStart 			: int = 0;
public var TotalFrames 			: int = 1;
public var Orientation 			: int = 1;
//public var FramesPerSeconds 	: int = 12;


private var SimpleAnim 		: AnimSprite;  



function Start () {

	SimpleAnim = GetComponent("AnimSprite")as AnimSprite;

	while(true) 
		yield CoUpdate();
}

function CoUpdate ()
{
	
	SimpleAnim.PlayFrames( RowStart, ColumnStart, TotalFrames, Orientation);

	yield;

}