// Spawn Save Player Location Setup
// Walker Boys
// Descripción: Guarda la posición del player / Localizacion para respawn luego de perder una vida
// Instruction: Ubicar un Save Point (GameObject w/ Collision) en escena con tag name 'SavePoints' además
// se debería incluír killboxes en scene con tag name 'killbox' - que envian al jugador al Savepoint más reciente

var startPoint 			: Transform;				// donde el player commienza nivel (opcional)
var  soundDie			: AudioClip;				// sound for death

@HideInInspector var  curSavePos	: Vector3 ;				// current saved position
private var loseLife		: boolean 		=false;	// toggle for losing life
private var pProp		;						// var to hold player propertiers

private var soundRate	: float 			= 0.0;	// variable que contiene el tiempo actual + delay amount
private var soundDelay	: float 			= 0.0;	// amount of time to delay

function Start()									// one time Init 
{
	if ( startPoint != null )						// quick check - if startPoint exists
	{
		transform.position = startPoint.position	;// then make player position equal to start position 	
		pProp = GetComponent( PlayerProperties );
	}
}

function Update ()
{
	if ( loseLife )
	{
		pProp.lives -= 1;
		loseLife = false;
	}
}

function OnTriggerEnter ( other : Collider )		// check for Event
{
	if ( other.tag == "savePoint" )				// si hay algo (cualquier cosa) con tag savepoint
	{
		curSavePos = transform.position;			// posición actual del playe es almacenada en curSavePosition
	}
	if ( other.tag == "killbox" )					// if killbox tagged on anything
	{
		PlaySound ( soundDie, 0 );
		loseLife = true;
		yield WaitForSeconds( 3 );
		
		renderer.enabled = false;
		pProp.playerState = PlayerState.MarioSmall;
		pProp.changeMario = true;
		if ( pProp.lives == 0 )
			return;
		renderer.enabled = true;
		transform.position = curSavePos;			// then make player position equal to last curSaveposition
	}
}

function PlaySound ( soundName, soundDelay )			// parameter needs sound name from audioclip and delay time
{
	if ( !audio.isPlaying && Time.time > soundRate )	// check to see if playing is done if time + rate is ready to start again
	{
		soundRate = Time.time + soundDelay;			// soundrate equals delay amount so that it plays defined by sound delay
		audio.clip = soundName;						// set the soundName as the audio clip to play 
		audio.Play () ;								// play the audiio clip
		yield WaitForSeconds ( audio.clip.length );	// wait till the audio clip is complete based on its length
	}
}