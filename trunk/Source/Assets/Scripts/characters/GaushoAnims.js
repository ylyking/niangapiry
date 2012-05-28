#pragma strict

private var animPlay 			: AnimSprite; // : Component

function Start ()
{
	animPlay = GetComponent(AnimSprite);
	animPlay.PlayFrames(0 , 2, 1, 1);
}

function Update () 
{
	animPlay.PlayFrames(0 , 2, 2, 1);
}