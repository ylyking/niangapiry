using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BonusObjects
{

public class FishingState : GameState 
{
    GUISkin gSkinB                  = null;
    float Increment                 = 0;
    float prevVolume                = 0;
    float RiseSpeed                 = 2;
    float Deep                      = 0;

    float timeLapse                 = 5;
    float LastTime                  = 0;
    float ActualTime                = 0;
    //float TimeStep                  = 0;

    Rect        Limits              = new Rect();

    public Texture2D   Box          = null;
    Rect        BoxPos              = new Rect(0, 0, Screen.width * .8f, Screen.height * 9f);

    public Texture2D   Background   = null;
    BonusObjects.Entity Back;

    public Texture2D BubblesText    = null;
    BonusObjects.Entity Bubbles;

    public Texture2D FishText       = null;
    BonusObjects.Entity Player;
    BonusObjects.Entity Target;

    List<BonusObjects.Entity> Entoids = new List<BonusObjects.Entity>() ;

    public enum FishState {Brief, Start, Falling, Rising, Win, Lose };
    public FishState currentState = FishState.Start;

    static bool Briefing = true;

    public override void Init()
    {
        Managers.Game.IsPaused = true;
        Managers.Display.ShowDelay = 0;
        Managers.Display.ShowState = false;

        prevVolume = Managers.Audio.Music.volume;
        Managers.Audio.Music.volume = 0.15f;
        Time.timeScale = 0.00000000000001f;


        gSkinB = Resources.Load("GUI/GUISkin B") as GUISkin;
        gSkinB.label.fontSize = Mathf.RoundToInt(Screen.width * 0.035f);

        //Managers.Game.PlayerPrefab.active = false;
        //Managers.Game.IsPlaying = false;
        //Managers.Game.PlayerPrefab.GetComponent<PlayerProperties>().enabled = false;
        Managers.Game.IsPaused = true;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////


        currentState = FishState.Start;
        if (Briefing)
            currentState = FishState.Brief;

        Limits = new Rect((Screen.width * .5f) - (Background.width * .5f),
                            (Screen.height * .5f) - (Background.height * .5f),
                            Background.width, Background.height);


        BoxPos = new Rect(Screen.width * .1f, Screen.height * .025f, Screen.width * .8f, Screen.height * .975f);

        Back = new BonusObjects.Entity();                                           // Background Init
        Back.Init(Background);

        Bubbles = new BonusObjects.Entity();                                        // Bubbles Foreground 
        Bubbles.Init(BubblesText);
        Bubbles.Position = Back.Position;

        Player = new BonusObjects.Player();                                         // Player Fisher Bass
        Player.Init(FishText);
        Target = null;

        if (Briefing)
        {
            var Entity = new BonusObjects.Dorado();
            Entity.Init(FishText);
            Entity.Position = new Rect(Entity.Position.x, 512, Entity.Position.width, Entity.Position.height);
            Entoids.Add(Entity);
        }

        for (int i = 1; i < 51; i++)
        {
            if (i % 5 == 0)
            {
                if (i == 50 )
                {
                    if (Managers.Register.Treasure3 == false)
                    {
                        var Entity = new BonusObjects.Treasure();
                        Entity.Init(FishText);
                        Entity.Position = new Rect(Entity.Position.x, 524 + i * 250, Entity.Position.width, Entity.Position.height);
                        Entoids.Add(Entity);
                    }
                    else
                    {
                        var Entity = new BonusObjects.Item();
                        Entity.Init(FishText);
                        Entity.Position = new Rect(Entity.Position.x, 524 + i * 250, Entity.Position.width, Entity.Position.height);
                        Entoids.Add(Entity);
                    }
                }
                else
                {
                    var Entity = new BonusObjects.Dorado();
                    Entity.Init(FishText);
                    Entity.Position = new Rect(Entity.Position.x, 512 + i * 250, Entity.Position.width, Entity.Position.height);
                    Entoids.Add(Entity);
                }
            }
            else
            {
                var Entity = new BonusObjects.Palomet();
                Entity.Init(FishText);
                Entity.Position = new Rect(Entity.Position.x, 512 + i * 250, Entity.Position.width, Entity.Position.height);
                Entoids.Add(Entity);
            }

        }

        LastTime = Time.realtimeSinceStartup;
    }

    public override void DeInit()
    {



        Entoids.Clear();
        Target = null;
        Increment = 0;
        RiseSpeed = 2;
        //Managers.Game.IsPaused = false;
        Time.timeScale = 1;

        Managers.Game.IsPlaying = true;
        Managers.Game.IsPaused = false;
        Managers.Display.ShowDelay = 6;
        Managers.Display.ShowState = true;

        Managers.Audio.Music.volume = prevVolume;
        gSkinB.label.fontSize = Mathf.RoundToInt(Screen.width * 0.035f);
    }
	

    public override void OnUpdate()
    {
        if (Input.GetKeyDown("escape") || Input.GetButtonDown("Start") || Input.GetButtonDown("Select"))
            Managers.Game.PopState();

         //Calculate the time elapsed in seconds
        ActualTime = Time.realtimeSinceStartup;
        float TimeStep = ( ActualTime - LastTime ) * 4 ;
        LastTime = ActualTime;
        Managers.Display.DebugText = " FixeddeltaTime: " + Time.fixedDeltaTime + " Time: " + TimeStep ;

        switch (currentState)
        {
            case FishState.Brief:
                //////////////////////////////////
                Increment += TimeStep;
                Back.Coord = new Rect(0, Increment * -.1f, 1, .1f);  // La textura debe estar SI o SI en Wrap: REPEAT!
                Bubbles.Coord = new Rect(0, Increment * -.1f, 1, 1);  // La textura debe estar SI o SI en Wrap: REPEAT!

                Player.Update(TimeStep);

                if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("return") || Input.GetButtonDown("Start"))
                {
                    Increment = 0;
                    currentState = FishState.Start;
                }
                break;


            case FishState.Start:
//////////////////////////////////
                Increment += TimeStep;
                Back.Coord = new Rect(0, Increment * -.1f, 1, .1f);  // La textura debe estar SI o SI en Wrap: REPEAT!
                Bubbles.Coord = new Rect(0, Increment * -.1f, 1, 1);  // La textura debe estar SI o SI en Wrap: REPEAT!

                Player.Update(TimeStep);

                if (Increment > 10)
                {
                    Increment = 0;
                    currentState = FishState.Falling;
                }
            break;

            case FishState.Falling:
//////////////////////////////////
            Increment += TimeStep;
                Back.Coord = new Rect(0, Increment * -.1f, 1, .1f);  // La textura debe estar SI o SI en Wrap: REPEAT!
                Bubbles.Coord = new Rect(0, Increment * -.1f, 1, 1);  // La textura debe estar SI o SI en Wrap: REPEAT!

                Player.Update(TimeStep);

                for (int i = Entoids.Count - 1; i >= 0; i--)
                {
                    Entoids[i].Update(TimeStep);
                    Entoids[i].Position.y -= TimeStep * 100;

                    if (Entoids[i].Position.center.y < Limits.yMax && Entoids[i].Position.center.y > Limits.yMin + 32)
                    {
                        //if (Player.CheckCollision(Entoids[i]) > 0)
                        switch (Player.CheckCollision(Entoids[i]))
                        {
                            case 1:     // Palomet
                                currentState = FishState.Lose;
                                break;
                            case 2:     // Dorado
                                Target = Entoids[i];
                                Deep = Increment;
                                currentState = FishState.Rising;
                                break;
                            case 3:     // Treasure
                                Target = Entoids[i];
                                //Briefing = false;
                                currentState = FishState.Rising;
                                break;
                            case 4:     // ItemFire
                                Target = Entoids[i];
                                currentState = FishState.Rising;
                                break;
                            case 5:     // ItemHat
                                Target = Entoids[i];
                                currentState = FishState.Rising;
                                break;
                            case 6:     // ItemWhistler
                                Target = Entoids[i];
                                currentState = FishState.Rising;
                                break;
                        }
                    }
                }

                if ( Increment > 135)
                    currentState = FishState.Lose;
            break;

            case FishState.Rising:
//////////////////////////////////
            //Increment -= TimeStep;

            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Jump"))   // Like JUmp
                RiseSpeed = 1;

            RiseSpeed -= TimeStep;                                                              // Like Gravity
            RiseSpeed = Mathf.Clamp(RiseSpeed, -1, 2);

            Increment -= RiseSpeed * TimeStep;
            if (Increment < 0)
            {
                timeLapse = 15;
                Briefing = false;
                currentState = FishState.Win;
            }


            if (Increment > 135)
                currentState = FishState.Lose;

            Back.Coord = new Rect(0, Increment * -.1f, 1, .1f);  // La textura debe estar SI o SI en Wrap: REPEAT!
            Bubbles.Coord = new Rect(0, Increment * -.1f, 1, 1);  // La textura debe estar SI o SI en Wrap: REPEAT!


            if (Player.Position.y < (Screen.height * .5f))
                Player.Position.y += TimeStep * 30;                          // Update

            Player.Update(TimeStep);                                         // Update             


            for (int i = Entoids.Count - 1; i >= 0; i--)
            {
                if (Entoids[i] != Target)
                {
                    Entoids[i].Update(TimeStep);                            // Update
                    Entoids[i].Position.y += RiseSpeed * 100 * TimeStep;

                    //Position.y += -timeStep * 100 * Speed;
                }
                else
                {
                    if (Entoids[i].myId == BonusObjects.Entity.typeId.Dorado)
                    {
                        if (Mathf.Sin(Time.realtimeSinceStartup * 24) > 0)
                            Entoids[i].Coord = new Rect(.25f, .5f, .25f, .25f);
                        else
                            Entoids[i].Coord = new Rect(.5f, .5f, -.25f, .25f);
                    }

                    Entoids[i].Position = new Rect(Player.Position.center.x - (Entoids[i].Sprite.width * .25f),
                                                    Player.Position.yMin,
                                                    Entoids[i].Position.width,
                                                    Entoids[i].Position.height);
                }
                if (Entoids[i].Position.center.y < Limits.yMax && Entoids[i].Position.center.y > Limits.yMin + 32)
                {
                    if (Player.CheckCollision(Entoids[i]) == 1)
                        currentState = FishState.Lose;
                }
            }


            break;

            case FishState.Win:
            timeLapse -= TimeStep;

                if (timeLapse < 0)
                {
                    switch (Target.myId)
                    {
                        case BonusObjects.Entity.typeId.Dorado:     // Dorado
                            if (Deep < 60)
                                Managers.Register.Health++;         // Give one Health plus          
                            //else if (Deep < 80)
                            //    Managers.Register.Health += 2;
                            else
                                Managers.Register.Health = 3;       // Give Full Health plus           
                            break;
                        case BonusObjects.Entity.typeId.Treasure:     // Treasure
                            Managers.Register.Treasure3 = true;  // Give Treasure3
                            break;
                        case BonusObjects.Entity.typeId.ItemFire:
                            Managers.Register.FireGauge++;
                            Managers.Register.FireGauge = Mathf.Clamp(Managers.Register.FireGauge, 0, 3);
                            Managers.Game.PlayerPrefab.renderer.material.SetFloat("_KeyY", 0.25f);
                            Managers.Register.Inventory = DataManager.Items.Fire;
                            break;
                        case BonusObjects.Entity.typeId.ItemHat:
                            Managers.Game.PlayerPrefab.renderer.material.SetFloat("_KeyY", 0.05f);
                            Managers.Register.Inventory = DataManager.Items.Hat;
                            break;
                        case BonusObjects.Entity.typeId.ItemWhistler:
                            Managers.Game.PlayerPrefab.renderer.material.SetFloat("_KeyY", 0.25f);
                            Managers.Register.Inventory = DataManager.Items.Whistler;
                            break;
                    }
                    timeLapse = 15;
                    Managers.Game.PopState();
                }
            break;

            case FishState.Lose:
            timeLapse -= TimeStep;

            if (timeLapse < 0)
            {
                timeLapse = 5;
                Managers.Game.PopState();
            }
            break;
        }


    }


    public override void OnRender()
    {
        if (gSkinB) GUI.skin = gSkinB;

        GUI.color = Color.white;
        //GUI.depth = -1000;

        GUI.DrawTexture(BoxPos, Box);

        switch (currentState)
        {
            case FishState.Brief:

                gSkinB.label.fontSize = 16;
                GUI.Label(new Rect((Screen.width * .5f) - 256, (Screen.height * .5f) + 256, 100, 50), "Profundidad: 0 m");

                Back.Render();
                Player.Render();
                Bubbles.Render();
                GUI.color = new Color(1, 0.36f, 0.22f, 1);
                GUI.Box(new Rect((Screen.width * .5f) - 224,
                                 (Screen.height* .5f) - 150,
                                 448, 300),
                "\n  \n\n\n\nMueve el anzuelo hacía los lados \n para evitar las palometas \n \nCuando enganches un Dorado   \n pulsa varias veces Disparo \n para ascender y atraparlo \n \n \nPresiona Enter/Start para empezar \n¡Buena Suerte! \n");
                gSkinB.label.fontSize = 24;
                GUI.color = new Color(1, 0.5f, 0.15f, 1);
                GUI.Label(new Rect((Screen.width * .5f) - 198,
                                 (Screen.height * .5f) - (Screen.height * .25f),
                                 (Screen.width * .5f), (Screen.height * .5f)), "\n - PESCA DEL DORADO -");
                GUI.color = Color.white;
                break;

            case FishState.Start:
                Back.Render();
                Player.Render();

                gSkinB.label.fontSize = 14;
                GUI.Label(new Rect((Screen.width * .5f) - 256, (Screen.height * .5f) + 256, 100, 50), "Profundidad: 0 m");

                gSkinB.label.fontSize = 48;
                if (Increment < 3.5f)
                {
                    GUI.color = Color.yellow;
                    GUI.Label(new Rect((Screen.width * .5f) - 196, (Screen.height * .25f), 100, 50), "PREPARADO");
                }
                else if (Increment < 6.5f)
                {
                    if (Time.frameCount % 2 == 0)
                        GUI.color = Color.white;
                    else
                        GUI.color = Color.yellow;
                    GUI.Label(new Rect((Screen.width * .5f) - 132, (Screen.height * .5f), 100, 50), "¿LISTO?");
                }
                else
                {
                    //ColorFade -= TimeStep * .25f;
                    //GUI.color = new Color(1, 1, 1, ColorFade);
                    if (Time.frameCount % 2 == 0)
                        GUI.color = Color.white;
                    else
                        GUI.color = Color.clear;
                    GUI.Label(new Rect((Screen.width * .5f) - 64, (Screen.height * .75f), 100, 50), "¡YA!");
                }
                GUI.color = Color.white;
                
                Bubbles.Render();
                break;

            case FishState.Falling:
                GUI.color = Color.white;

                gSkinB.label.fontSize = 14;
                GUI.Label(new Rect((Screen.width * .5f) - 256, (Screen.height * .5f) + 256, 100, 50),
                    "Profundidad: " + (Increment * .1f).ToString("F2") + " m");

                Back.Render();
                Player.Render();
                foreach (BonusObjects.Entity entity in Entoids)
                {
                    if (entity.Position.center.y < Limits.yMax && entity.Position.center.y > Limits.yMin)
                        entity.Render();
                }

                Bubbles.Render();
                break;

            case FishState.Rising:
                GUI.color = Color.white;

                gSkinB.label.fontSize = 14;
                GUI.Label(new Rect((Screen.width * .5f) - 256, (Screen.height * .5f) + 256, 100, 50),
                    "Profundidad: " + (Increment * .1f).ToString("F2") + " m");
                if (Time.frameCount % 2 == 0)
                    GUI.Label(new Rect((Screen.width * .5f)+56 , (Screen.height * .5f) + 256, 100, 50),
                    "¡Pulsa Disparo!");

                Back.Render();

                // Fishing Nylon Rendering
                GUI.DrawTextureWithTexCoords(
                new Rect((Screen.width * .5f) + ((BonusObjects.Player)Player).PosX, Player.Position.y - 256,
                       Player.Sprite.width * .25f, Player.Sprite.height * 1), FishText, new Rect(0, .25f, .25f, .75f));


                Player.Render();
                foreach (BonusObjects.Entity entity in Entoids)
                {
                    if (entity.Position.center.y < Limits.yMax && entity.Position.center.y > Limits.yMin)
                        entity.Render();
                }

                Bubbles.Render();
                break;

            case FishState.Win:
                //if (Time.frameCount % 2 == 0)
                //    GUI.color = Color.blue;
                //else
                    GUI.color = Color.white;

                gSkinB.label.fontSize = 14;
                GUI.Label(new Rect((Screen.width * .5f) - 256, (Screen.height * .5f) + 256, 100, 50),
                    "Profundidad: " + (Increment * .1f).ToString("F2") + " m");

                Back.Render();
                Player.Render();


                //GUI.color = Color.green;
                if (Time.frameCount % 2 == 0)
                    GUI.color = Color.green;
                else
                    GUI.color = Color.white;


                foreach (BonusObjects.Entity entity in Entoids)
                {
                    if (entity.Position.center.y < Limits.yMax && entity.Position.center.y > Limits.yMin)
                        entity.Render();
                }
                Bubbles.Render();

                gSkinB.label.fontSize = 48;
                GUI.Label(new Rect((Screen.width * .5f) - 264, (Screen.height * .5f), 100, 50), "PESCA EXITOSA");
                                                                                                 
                if (Target.myId == BonusObjects.Entity.typeId.Dorado)
                {
                    gSkinB.label.fontSize = 14;
                    if ( Deep < 60)
                    GUI.Label(new Rect((Screen.width * .5f) - 250, (Screen.height * .75f), 100, 50),
                        "Atrapaste un Dorado a " + (Deep * .1f).ToString("F2") + " metros\n" + "                   Recuperaste 30% de Energía extra!");
                    else
                        GUI.Label(new Rect((Screen.width * .5f) - 250, (Screen.height * .75f), 100, 50),
                              "Atrapaste un Dorado a " + (Deep * .1f).ToString("F2") + " metros\n" + "                   Recuperaste 100% Energía extra!");

                }
                else if (Target.myId == BonusObjects.Entity.typeId.Treasure)
                {
                    gSkinB.label.fontSize = 14;
                    GUI.Label(new Rect((Screen.width * .5f) - 204, (Screen.height * .75f), 100, 50),
                        "¡Has Encontrado el 3º Tesoro de Iguazú!");
                }
                else
                {
                    gSkinB.label.fontSize = 14;
                    GUI.Label(new Rect((Screen.width * .5f) - 204, (Screen.height * .75f), 100, 50),
                        "¡Has Encontrado un Item Especial!");
                }
                GUI.color = Color.white;

                break;

            case FishState.Lose:
                //if (Time.frameCount % 2 == 0)
                GUI.color = Color.red;
                //else
                    //GUI.color = Color.white;

                gSkinB.label.fontSize = 14;
                GUI.Label(new Rect((Screen.width * .5f) - 256, (Screen.height * .5f) + 256, 100, 50),
                    "Profundidad: " +  (Increment * .1f).ToString("F2") + " m");

                Back.Render();
                foreach (BonusObjects.Entity entity in Entoids)
                {
                    if (entity.Position.center.y < Limits.yMax && entity.Position.center.y > Limits.yMin)
                        entity.Render();
                }
                Player.Render();

                //GUI.color = Color.red;
                Bubbles.Render();

                gSkinB.label.fontSize = 48;
                if (Time.frameCount % 2 == 0)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                GUI.Label(new Rect((Screen.width * .5f) - 186, (Screen.height * .5f), 100, 50), "¡SONASTE!");

                if ( Increment > 130 )
                {
                    gSkinB.label.fontSize = 14;
                    GUI.Label(new Rect((Screen.width * .5f) - 204, (Screen.height * .75f), 100, 50),
                        "Caiste muy Profundo, Perdiste tu anzuelo");
                }
                else
                {
                    gSkinB.label.fontSize = 14;
                    GUI.Label(new Rect((Screen.width * .5f) - 204, (Screen.height * .75f), 100, 50),
                        "Las Palometas se morfaron tu carnada");
                }
                GUI.color = Color.white;

                break;
        }

    }

    public override void Pause() { ;}
    public override void Resume() { ;}


}


    class Entity
    {
        public Texture2D Sprite = null;
        public Rect Coord       = new Rect();
        public Rect Position    = new Rect();
        public Rect Collision   = new Rect();

        public int Flip = 1;
        public enum typeId {none =0, Palomet =1, Dorado =2, Treasure =3, ItemFire, ItemHat, ItemWhistler};
        public typeId myId = typeId.none;

        public int CheckCollision(Entity Other)
        {
            //Collision.x = Position.center.x - (Collision.width * .5f);
            //Collision.y = Position.center.y - (Collision.height * .5f);

            float x1 = Position.center.x - (Collision.width * .5f);
            float y1 = Position.center.y - (Collision.height * .5f);
            float w1 = Collision.width;
            float h1 = Collision.height;

            float x2 = Other.Position.center.x - (Collision.width * .5f);
            float y2 = Other.Position.center.y - (Collision.height * .5f);
            float w2 = Other.Collision.width;
            float h2 = Other.Collision.height;

            if (((x2 > x1 && x2 < x1 + w1) || (x1 > x2 && x1 < x2 + w2)) &&
                ((y2 > y1 && y2 < y1 + h1) || (y1 > y2 && y1 < y2 + h2)))
            {
                return (int)Other.myId;
            }
            return 0;
        }

        public virtual void Init(Texture2D sprite)
        {
            Sprite = sprite;
            Position = new Rect(Screen.width * .5f - (Sprite.width * .5f),
                            Screen.height * .5f - (Sprite.height * .5f),
                            Sprite.width, Sprite.height);
        }

        public virtual void Update(float timeStep)
        {
            ;
        }

        public virtual void Render()
        {
            GUI.DrawTextureWithTexCoords(Position, Sprite, Coord);
        }

    }


    class Player : BonusObjects.Entity
    {
        public float PosX = 0;

        public override void Init(Texture2D sprite)
        {
            Sprite = sprite;
            Collision = new Rect(0, 0, Sprite.width * .125f, Sprite.height * .25f);
            Position = new Rect((Screen.width * .5f) ,
                                Screen.height * .5f - (512 * .5f),
                                Sprite.width * .25f, Sprite.height * .25f);
            myId = typeId.none;
        }

        public override void Update(float timeStep)
        {
            PosX += Input.GetAxisRaw("Horizontal") * timeStep * 100;
            PosX = Mathf.Clamp(PosX, -264, 512 * .4f);      // Background.width = Background.height = 512

            Position = new Rect((Screen.width * .5f) + PosX, Position.y, Position.width, Position.height );


            if (Time.frameCount % 2 == 0)
                Coord = new Rect(.25f, 0, -.25f, .25f);
            else
                Coord = new Rect(0, 0, .25f, .25f);

            //((BonusObjects.Player)Player).PosX += Input.GetAxisRaw("Horizontal") * Mathf.Abs(TimeStep) * 100;
            //((BonusObjects.Player)Player).PosX = Mathf.Clamp(((BonusObjects.Player)Player).PosX, -512 * .525f, 512 * .4f);      // Background.width = Background.height = 512

            //Player.Position = new Rect((Screen.width * .5f) + ((BonusObjects.Player)Player).PosX, (Screen.height * .5f) , 
            //                            Player.Sprite.width * .25f, Player.Sprite.height * .25f);


            //if (Time.frameCount % 2 == 0)
            //    Player.Coord = new Rect(.25f, 0, -.25f, .25f);
            //else
            //    Player.Coord = new Rect(0, 0, .25f, .25f);
        }

        public override void Render()
        {
            GUI.DrawTextureWithTexCoords(Position, Sprite, Coord);
        }
    }


    class Palomet : BonusObjects.Entity
    {
        //float Speed = 0;

        public override void Init(Texture2D sprite)
        {
            Sprite = sprite;
            Coord = new Rect(.25f, .75f, .25f, .25f);
            Collision = new Rect(0, 0, Sprite.width * .25f, Sprite.height * .25f);
            Position = new Rect( Random.Range((Screen.width * .5f) -256,(Screen.width * .5f) + 196 - 64),
                                 0, Sprite.width * .5f, Sprite.height * .5f);
            Flip =  Random.Range(-1, 1);
            //Speed = Random.Range(1, 2.5f);
            //Speed = Random.Range(1, 2);
            myId = typeId.Palomet;
        }

        public override void Update(float timeStep)
        {

            if (Position.x < (Screen.width * .5f - 264))
            {
                //Coord = new Rect(.25f, .75f, .25f, .25f);
                Flip = 1;
            }
            if (Position.x > (Screen.width * .5f + 196 - 64))
            {
                //Coord = new Rect(.5f, .75f, -.25f, .25f);
                Flip = -1;
            }

            if (Time.frameCount % 3 == 0)
                Coord = new Rect((Flip == 1 ? .25f : .5f), .75f, (Flip == 1 ? .25f : -.25f), .25f);
            else
                Coord = new Rect((Flip == 1 ? .5f : .75f), .75f, (Flip == 1 ? .25f : -.25f), .25f);
                

            Position.x += Flip * timeStep * 50;
            //Position.y += -timeStep * 100 * Speed;
        }
    }


    class Dorado : BonusObjects.Entity
    {
        public override void Init(Texture2D sprite)
        {
            Sprite = sprite;
            Coord = new Rect(.25f, .5f, .25f, .25f);
            Collision = new Rect(0, 0, Sprite.width * .25f, Sprite.height * .25f);
            Position = new Rect(Random.Range((Screen.width * .5f) - 256, (Screen.width * .5f) + 196 - 64),
                                 0, Sprite.width * .5f, Sprite.height * .5f);
            Flip = Random.Range(-1, 1);
            myId = typeId.Dorado;
        }

        public override void Update(float timeStep)
        {
            //Position = new Rect((Screen.width * .5f) - (Sprite.width * .25f),
            //    44, Sprite.width * .5f, Sprite.height * .5f);

            if (Position.x < (Screen.width * .5f - 256))
                Flip = 1;

            if (Position.x > (Screen.width * .5f + 196 - 64))
                Flip = -1;

            Position.x += Flip * timeStep * 50;
            //Position.y += -timeStep * 100;
        }
    }


    class Treasure : BonusObjects.Entity
    {
        public override void Init(Texture2D sprite)
        {
            Sprite = sprite;
            Coord = new Rect(.25f, .25f, .25f, .25f);
            Collision = new Rect(0, 0, Sprite.width * .25f, Sprite.height * .25f);
            Position = new Rect(Random.Range((Screen.width * .5f) - 256, (Screen.width * .5f) + 196 - 64),
                                 0, Sprite.width * .5f, Sprite.height * .5f);
            Flip = Random.Range(-1, 1);
            myId = typeId.Treasure;
        }

        public override void Update(float timeStep)
        {
            if (Position.x < (Screen.width * .5f - 256))
                Flip = 1;

            if (Position.x > (Screen.width * .5f + 196 - 64))
                Flip = -1;

            Position.x += Flip * timeStep * 50;
            //Position.y += -timeStep * 100;
        }
    }


    class Item : BonusObjects.Entity
    {
        public override void Init(Texture2D sprite)
        {
            Sprite = sprite;
            Coord = new Rect(.25f, .25f, .25f, .25f);
            Collision = new Rect(0, 0, Sprite.width * .25f, Sprite.height * .25f);
            Position = new Rect(Random.Range((Screen.width * .5f) - 256, (Screen.width * .5f) + 196 - 64),
                                 0, Sprite.width * .5f, Sprite.height * .5f);
            Flip = Random.Range(-1, 1);

            switch (Random.Range(0, 2))
            {
                case 0:
                    myId = typeId.ItemFire;
                    Coord = new Rect(.25f, 0, .25f, .25f);
                    break;
                case 1:
                    myId = typeId.ItemHat;
                    Coord = new Rect(.75f, 0, .25f, .25f);
                    break;
                case 2:
                    myId = typeId.ItemWhistler;
                    Coord = new Rect(.5f, 0, .25f, .25f);
                    break;
            }
        }

        public override void Update(float timeStep)
        {
            if (Position.x < (Screen.width * .5f - 256))
                Flip = 1;

            if (Position.x > (Screen.width * .5f + 196 - 64))
                Flip = -1;

            Position.x += Flip * timeStep * 50;
        }
    }
}