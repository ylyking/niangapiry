// Player properties Component
// Description: set and stores pickups and state of player
using UnityEngine;
using System.Collections;

public class PlayerProperties : MonoBehaviour {


public enum PlayerState			// main character states builded as bitfields to had simultaneidity & more control
{	
	Asleep 			= 0,	
	Flickering 		= 1,		
	Normal			= 2,		
	Invisible		= 4,
	Small			= 8,
	WildFire		= 16
}
public PlayerState playerState 	= PlayerState.Normal;		// set display state of mario in Inspector 

public enum Items		{ Empty = 0, Hat = 1, Whistler = 2, Invisibility = 4, Smallibility = 8, Fire = 16 }

public Items Inventory	= Items.Empty;					// Inventory system activation

public GameObject projectileHat;
public GameObject projectileFire;

bool  changeState 	    = true;						    // flag to switch state

[HideInInspector]
   public bool  dead    = false;					    // general flags to knew current player state
[HideInInspector]
    public bool normal  = false;
[HideInInspector]
    public bool inmune  = false;
[HideInInspector]
    public bool BurnOut = false;

private bool  HoldingKey 	= false;					// simple flag to know if the throwing button was released 
private bool  wasKinematic 	= false;					// flag to remeber if the taken thing was Kinematic or not
private bool  hitLeft 	    = false;					// flag to detect player collision with dangerous things
private bool  hitRight 	    = false;

float hitDistance 	        = 3.0f;						// Distance to push the player on being hitted

public  AudioClip soundHurt;
public  AudioClip soundDie;
public  AudioClip soundFalling;
public  AudioClip soundShoot;
public  AudioClip soundHat;
public  AudioClip soundPowerUp;
public  AudioClip soundPowerUp2;
public  AudioClip soundFruits;
public  AudioClip soundExplosion;
public  AudioClip soundFlaming;
public  AudioClip SoundTrack;
public  AudioClip SoundDelirium;

private PlayerControls playerControls;
private AnimSprite animPlay; 							// : Component
private CharacterController charController;

public Vector3 GrabPosition = new Vector3( 0f, 0.5f, 0f);	// Grab & Throw Funcionality assuming obj has a rigidbody attached				
public float ThrowForce		= 400f;						// How strong the throw is. 
public float DownSideLimit	= 0f;						// How strong the throw is. 

[HideInInspector]
    public Transform _pickedObject;					    // is HoldingObj ? 
private Transform thisTransform;					    // own player tranform cached
private Transform PlayerTransform;					    // own player tranform cached
private Transform startPoint = null;				    // donde el player commienza nivel (opcional)
private Vector3 curSavePos ;						    // current saved position

public GameObject ParticleStars ;
public GameObject ParticleFlame ;


void  Start (){
	thisTransform   = transform;
	PlayerTransform = thisTransform.parent.transform;
	playerControls  = GetComponent< PlayerControls>();
    charController  = GetComponent<CharacterController>();
	animPlay 	    = GetComponent<AnimSprite>();
	dead = false;
	
	if ( SoundTrack && Managers.Audio.SoundEnable )
        Managers.Audio.PlayMusic(SoundTrack, 1.0f, 1.0f);
        //Managers.Audio.PlayLoop( SoundTrack, thisTransform, .5f, 1.0f);
	
	
//	Inventory |= Items.Hat ;
//	SetPlayerState( PlayerState.Asleep);


	if ( startPoint != null )
	{
		thisTransform.position = startPoint.position	;
		curSavePos = startPoint.position	;
	}
	else 
        curSavePos = thisTransform.position ;

    Managers.Display.ShowFlash(1.0f);

    if (Managers.Stages.GetComponent<LevelAttributes>())
        DownSideLimit = Managers.Stages.GetComponent<LevelAttributes>().MinScreenLimit;

    StartCoroutine(CoUpdate());

        //yield return new 
}

IEnumerator CoUpdate()
{

    while (true)
    {
        //Managers.Game.Update(Time.deltaTime);
        StartCoroutine(HitDead());
        UpdatePlayerState();
        if (Input.GetButtonDown("Fire1")) HoldingKey = true;
        if (Input.GetButtonUp("Fire1")) HoldingKey = false;
        
        yield return 0;
    }
}	


void  UpdatePlayerState (){
//	dead = ((playerState == 0) || ( playerState == PlayerState.WildFire ));		// Yep, when pombero's on fire it's dead, bad id
	normal = ((playerState != 0) && ( (playerState & PlayerState.WildFire) != PlayerState.WildFire  ));
	inmune = !normal 		|| ( ( playerState  & PlayerState.Flickering) == PlayerState.Flickering );
	
	switch (playerState)
	{
        case PlayerState.Asleep:
            {
                animPlay.PlayFrames(4, 3, 1, playerControls.orientation);

                if (changeState)
                {
                    renderer.enabled = true;
                    renderer.material.color = Color.white;
                    playerControls.enabled = false;
                    changeState = false;
                    StartCoroutine(Sleeping());
                }

                if ((Input.GetButtonUp("Fire1") || Input.GetButtonUp("Jump")) && (Managers.Game.Lifes > 0))
                {
                    playerControls.enabled = true;
                    SetPlayerState(PlayerState.Flickering);
                }
                break;
            }

        case PlayerState.WildFire:
            {
                Debug.Log("U are on Fire!");
                playerState |= PlayerState.Normal;
                StartCoroutine(Burning());
                break;
            }

        default:
            {
                playerState |= PlayerState.Normal; // This its needed in case of direct switch from Unity GUI drag & drop

                if ((playerState & PlayerState.Flickering) == PlayerState.Flickering)
                    if (changeState) { changeState = false; StartCoroutine(Flickering()); }


                if ((playerState & PlayerState.Invisible) == PlayerState.Invisible)
                    if (changeState) { changeState = false; StartCoroutine(Invisible()); }

                if (Input.GetButton("Fire1") && !HoldingKey) StartCoroutine(PlayerThrows());
                else
                    if (!_pickedObject && Input.GetButtonDown("Fire2")) UseInventory();
                break;
            }
//		if (  !_pickedObject && HatShoot && Input.GetButtonDown("Fire2")) ThrowHat(); // ThrowHat();
	}
	
}

////////////////////////////////////////////////////////////////////////////////////////////////// COLLISIONS

 
void  OnTriggerEnter (  Collider other   ){
//	if ( other.CompareTag( "Enemy") && !inmune )//	&& thisTransform.position.y  < other.transform.position.y + 0.1f  )
	if ( other.CompareTag( "Enemy") )//	&& thisTransform.position.y  < other.transform.position.y + 0.1f  )
	{																			// if collide with one side box...
        if ( !inmune)
		{
			 hitLeft =( thisTransform.position.x  < other.transform.position.x );	// check left toggle true  
			 
			 hitRight =( thisTransform.position.x  > other.transform.position.x );	// check right toggle true  
			 
			 Managers.Audio.Play( soundHurt, thisTransform);
		}
		if ( BurnOut )
		{
			Managers.Audio.Play( soundFlaming, thisTransform);
		    Destroy( Instantiate ( ParticleFlame, thisTransform.position, thisTransform.rotation), 5);
			Destroy( other.gameObject );											// Keep falling and die after a while
		}
	}
	
	if ( other.name == "Fruit" )//other.CompareTag( "p_shot") && !HatShoot   )
	{
		Managers.Display.ShowStatus();	
		if (ParticleStars)
		Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
		
		Destroy( other.gameObject );		
		Managers.Game.Fruits++;
        Managers.Game.Score += 50;
		Managers.Audio.Play( soundFruits, thisTransform);
	}
	
	if ( other.name == "Hat" )//other.CompareTag( "p_shot") && !HatShoot   )
	{
		Managers.Audio.Play( soundPowerUp2, thisTransform);
		Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);

		
		Destroy( other.gameObject );
		renderer.material.SetFloat("_KeyY", 0.05f);
		Inventory |= Items.Hat ;
	}

    if (other.name == "Ca" + (char)0x00F1 + "a")// Should say 'Caña' //other.CompareTag( "p_shot") && !HatShoot   )
	{
		Managers.Audio.Play( soundPowerUp, thisTransform);
		Destroy( Instantiate ( ParticleStars, thisTransform.position, thisTransform.rotation), 5);
		
		Destroy( other.gameObject );
		Inventory = Items.Fire ;
	}
	if ( other.CompareTag("savePoint") )										// check tag savepoint
	{
		Managers.Audio.Play( soundPowerUp, thisTransform);
		curSavePos = thisTransform.position;									// current player position is saved curSavePosition
	}
	
	if ( other.CompareTag( "killbox") )	
		InstaKill(true , 1);


}

void  OnGUI (){
//	if ( !Managers.Display.IsPlaying ) return;
	
	if( Event.current.type == EventType.Repaint) 
	{
        //Managers.Display.Render();
        Managers.Display.Render();
		
		if( Managers.Dialog.IsInConversation() )
            Managers.Dialog.Render();
	}
}


void OnTriggerStay(  Collider hit  )  					// void  OnControllerColliderHit ( ControllerColliderHit hit  ){
{
    if ( hit.CompareTag( "Platform" ) )
		thisTransform.parent = hit.transform;
		
 	if ( HoldingKey &&  hit.CompareTag( "pickup") )// ||  hit.CompareTag( "p_shot") )
// 	if ( Input.GetButtonDown( "Fire1") && hit.CompareTag( "pickup") ||  hit.CompareTag( "p_shot") )

       if ( (int)playerState < 8 && !_pickedObject  )
       {
//         HoldingKey = true;
             	
         _pickedObject = hit.transform; 									// caches the picked object
         
         ThrowForce = _pickedObject.rigidbody.mass * 5;
         
         wasKinematic = _pickedObject.rigidbody.isKinematic;
         _pickedObject.rigidbody.isKinematic = true;
         
    	  Physics.IgnoreCollision(_pickedObject.collider, gameObject.collider, true );
    	  
         _pickedObject.collider.enabled = false;	 

         									//this will snap the picked object to the "hands" of the player
         _pickedObject.position = thisTransform.position + GrabPosition; 	// Could be changed with every object properties

         									//this will set the HoldPosition as the parent of the pickup so it will stay there
         _pickedObject.parent = thisTransform; 

       }
       
}

void OnTriggerExit(  Collider hit  )  					// void  OnControllerColliderHit ( ControllerColliderHit hit  ){
{
    if ( hit.CompareTag( "Platform" ) )
		thisTransform.parent = PlayerTransform;
}

IEnumerator  HitDead (){
	if ( thisTransform.position.y < DownSideLimit  ) 
		{ 
			InstaKill( true, 1); 
			yield break;
		}

	if ((hitRight || hitLeft ))												// If we were hitten get pushdirection 
	{
		int pushDirection= System.Convert.ToByte(hitLeft) - System.Convert.ToByte(hitRight) ; // hitLeft == 1, hitRight == -1
		hitLeft = false;
		hitRight = false;

		int orientation= playerControls.orientation;							
		Managers.Display.ShowStatus();
        Managers.Game.Health--;
		Managers.Audio.Play( soundHurt, thisTransform); 
		
		
		if ( Managers.Game.Health > 0 )										// If health still available do damage and continue 
		{
			float hurtTimer= Time.time + 0.2f;
			while( hurtTimer > Time.time )
			{
				thisTransform.Translate( -pushDirection * hitDistance * Time.deltaTime, hitDistance  * Time.deltaTime, 0);
				animPlay.PlayFrames( 2, 3, 1, orientation); 
    	 		yield return 0;
		 	}
			AddPlayerState( PlayerState.Flickering );
		}
		else																// else lose a Life and start dying mode
			InstaKill( false, pushDirection );
	}
}

IEnumerator  InstaKill (  bool ReSpawn ,    int pushDirection   ){
	if ( dead ) yield break;

		ReSpawn = ( ReSpawn || ( thisTransform.position.y <= DownSideLimit  ));	// Little Re-Check for falling bugs
	
		dead = true;
 
		Managers.Display.ShowStatus();
		Managers.Game.Lifes -= 1;
		renderer.material.color = Color.white;
		renderer.enabled = true;
//		Managers.Audio.Play( soundDie, thisTransform); 
		

		playerControls.enabled = false;
		if ( _pickedObject ) PlayerThrows();
		
		if ( !ReSpawn  )
		{
			float dieTimer= Time.time + 0.5f;
 			while( dieTimer > Time.time )									// Do jump and sad animation...
			{
				charController.Move( new Vector3( -pushDirection * hitDistance * Time.deltaTime * .5f, 
												hitDistance * Time.deltaTime , 0) );
				animPlay.PlayFrames( 2, 3, 1, playerControls.orientation );
				yield return 0;
			}

			while ( !charController.isGrounded )							// set to ground the character ...
			{
				charController.Move( new Vector3( 0, -4.0f, 0 ) * Time.deltaTime );
				yield return 0;
			}
		}
		else 
			Managers.Audio.Play(soundFalling, thisTransform, 0.5f, 1f);
		
        for( float dieTimer = 0.0f; dieTimer < 3.0f; dieTimer += Time.deltaTime * 2)  // Do Hat flying animation  
        {
            animPlay.PlayFrames(4, Mathf.FloorToInt(dieTimer), 1, playerControls.orientation);
            yield return 0;
        }
		
		Managers.Display.ShowStatus();

        if (Managers.Game.Lifes > 0)
        {
			if ( ReSpawn )
			{
				yield return new WaitForSeconds( 0.5f );
                Managers.Display.ShowFlash(1.5f);
				thisTransform.position = curSavePos;	
				playerControls.velocity = Vector3.zero;
			}
			SetPlayerState( PlayerState.Asleep );						// if there are life change state to Asleep
            Managers.Game.Health = 3;
        }				
		else
		{ 	
			yield return new WaitForSeconds(5.0f);
            Managers.Game.GameOver();								// else GameOver
			yield return new WaitForSeconds(1.0f);
		}
		
		if ( Managers.Game.IsPlaying )	
			dead = false;

}

///////////////////////////////////////////////////////////////////////////////////////////////////





void  UseInventory (){
	switch ( Inventory )
	{
		case Items.Empty: 
			break;
			
		case Items.Hat: 								// Throw Hat..
		if( (Inventory  & Items.Hat) == Items.Hat )
			Managers.Audio.Play( soundHat, thisTransform); 
			StartCoroutine(ThrowHat());
			Inventory &= (~Items.Hat);
			break;
						
		case Items.Whistler:							// Wisp Whistler
			Inventory &= (~Items.Whistler);
			break;
			
		case Items.Invisibility:						// Turn Invisible 
			AddPlayerState( PlayerState.Invisible );
			Inventory &= (~Items.Invisibility);
			break;
			
		case Items.Smallibility: 						// Get Smaller
			Inventory &= (~Items.Smallibility);
			break;			
			
		case Items.Fire: 								// Do Fire things..
//				if ( (Managers.Display.FireGauge) == 1 ) ;// instantiate flame
//			else 
//				if ( (Managers.Display.FireGauge)  < 3 ) ;// instantiate fireball
//			else
			Managers.Audio.Play( soundExplosion, thisTransform); 
			SetPlayerState( PlayerState.WildFire );
							  
//			Managers.Display.FireGauge = 0;
			Inventory &= (~Items.Fire);
			break;
	}
}




IEnumerator Sleeping (){
	while ( !charController.isGrounded )
	{
		charController.Move( new Vector3( 0, -1.0f, 0 ) * Time.deltaTime );

        if (playerState > 0) yield break; 
		
		yield return 0;	
	}
}

IEnumerator Flickering (){
	float timertrigger= Time.time + 5;
	
	while( timertrigger > Time.time )
	{
		if(Time.frameCount % 4 == 0)
			renderer.enabled = !renderer.enabled;

		if ( !normal ) yield break;
		
		yield return 0;
	}
	
		renderer.enabled = true;
		playerState &= (~PlayerState.Flickering);
}

IEnumerator Small (){
	SetPlayerState( PlayerState.Normal);
    yield return 0;

}

IEnumerator Invisible (){
	float timertrigger= Time.time + 20;
			 
	while( timertrigger > Time.time )
	{

 			float lerp = Mathf.PingPong (Time.time * .45f, 1.0f) ;
 			renderer.material.SetFloat( "_Cutoff", Mathf.Lerp ( -0.15f, .7f,  lerp));

            if (!normal) yield break;
			
		yield return 0;			 
	}

 		renderer.material.SetFloat( "_Cutoff", 0.9f);

// 		renderer.material.color = Color.white;
		playerState &= (~PlayerState.Invisible);

}

IEnumerator Burning (){
	renderer.material.color = Color.white;
	
    //AudioSource Clip = null;
	
//	if ( SoundDelirium )
//		AudioSource = Managers.Audio.PlayLoop( SoundDelirium, thisTransform, .65f, 1.0f);
//	renderer.enabled = true;
	BurnOut = true;
	
	float timertrigger= Time.time + 30;
	while( timertrigger > Time.time )
	{
        if (Time.frameCount % 2 == 0)
            renderer.material.SetFloat("_KeyY", 0.9f);
        else
            renderer.material.SetFloat("_KeyY", 0.7f);
         //renderer.material.SetFloat("_KeyY",  Mathf.PingPong(Time.time, 0.2f) + 0.7f );   


        if (playerState == PlayerState.Asleep)
        {
            BurnOut = false;
            renderer.material.SetFloat("_KeyY", 0.25f);
            //		    if ( SoundDelirium )
            //		        Managers.Audio.PlayLoop( SoundDelirium, thisTransform, .65f, 1.0f);
            yield break;
        } 

		yield return 0;
	}
	
	Managers.Audio.Play( soundFlaming, thisTransform);

	BurnOut = false;
	renderer.material.SetFloat("_KeyY", 0.25f);
	renderer.material.color = Color.white;
	playerState &= (~PlayerState.WildFire);
}

IEnumerator  ThrowHat (){
	// Instantiate the projectile
    GameObject clone = (GameObject)Instantiate(projectileHat, thisTransform.position, thisTransform.rotation);
    
    Physics.IgnoreCollision(clone.collider, this.gameObject.collider, true );	 // it works but the distance it s lame

    clone.name = "Hat";

	clone.GetComponent<BulletShot>().FireBoomerang( new Vector3( playerControls.orientation * 8, 0, 0) , 1, 0, 4);
	// shot with a short animation
  
	renderer.material.SetFloat("_KeyY", 0.25f);
     
    float timertrigger= Time.time + 0.25f;
	while( timertrigger > Time.time )
	{
		animPlay.PlayFrames( 3, 1, 1, playerControls.orientation); 
     	yield return 0;
	}
	Physics.IgnoreCollision(clone.collider, this.gameObject.collider, false);
}

IEnumerator  ThrowFire (){
    int orientation= playerControls.orientation;  	 							// Instantiate the projectile
//	Managers.Audio.Play( soundFlaming, thisTransform);
     	 	    
	
    GameObject clone = (GameObject)Instantiate(projectileFire,
    									  thisTransform.position + new Vector3(orientation * .25f,0,0),
    									   thisTransform.rotation);
    Physics.IgnoreCollision(clone.collider, this.gameObject.collider); 			// it works but the distance it s lame
 // clone.thisTransform.Translate( Vector3( 0, 1, 0) ); 					// avoid hits between shot & shooter own colliders  

    clone.name = "Fire";

	playerControls.enabled = false;
	
	// Add speed to the target
	clone.GetComponent<BulletShot>().Fire(  new Vector3( orientation * 4, 0, 0), 10);  // shot with a short animation
    
    float timertrigger= Time.time + 0.75f;
	while( timertrigger > Time.time )
	{
		animPlay.PlayFrames( 3, 6, 2, orientation); 
     	yield return 0;
	}
   	playerControls.enabled = true;
	
}

IEnumerator PlayerThrows()												// Object Throwing without physics engine
{
    if ( _pickedObject )
    {    	
    	 float orientation= playerControls.orientation;
	
    	 Managers.Audio.Play(soundShoot, thisTransform);
	   	   	
         _pickedObject.tag = "p_shot" ;
	   	
       	 _pickedObject.parent = null;        		//resets the pickup's parent to null so it won't keep following the player	

         _pickedObject.collider.enabled = true;
                    
         _pickedObject.rigidbody.isKinematic =	wasKinematic;
                           
         //applies force to the rigidbody to create a throw
//       pickedObject.rigidbody.AddForce( Vector3( orientation, 1,0) * ThrowForce, ForceMode.Impulse);
		 if ( !_pickedObject.rigidbody.isKinematic )
         {
//         	_pickedObject.collider.isTrigger = true;
		   	_pickedObject.rigidbody.AddForce( new Vector3( orientation, Input.GetAxis( "Vertical"),0) * ThrowForce, ForceMode.Impulse);
//       	pickedObject.position += Vector3( orientation, Input.GetAxis( "Vertical"),0) * 1.5f;
  		 }
    	
    	 Physics.IgnoreCollision(_pickedObject.collider, gameObject.collider, false );	
    	 
//    	    EditorApplication.isPaused = true;
  		
         _pickedObject = null;	    												//resets the _pickedObject   			
         
         float timertrigger= Time.time + 0.55f;
		 while( timertrigger > Time.time )
		 {
			animPlay.PlayFrames( 3, 1, 1, (int)orientation); 
         	HoldingKey = false;
			
    	 	yield return 0;
		 }
		 
		 
	}
}




void  SetPlayerState (  PlayerState newState   ){
	playerState = newState;									// change to a new playerState & delete all previous 
	changeState = true;
}

void  AddPlayerState (  PlayerState newState   ){
	playerState |= newState;								// add a new playerState & keep all previous 
	changeState = true;
}
	

}

