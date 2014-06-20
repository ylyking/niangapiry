using UnityEngine;
using System.Collections;

namespace BonusObjects
{

    public class BonusManager : MonoBehaviour
    {
		public AudioClip Cheers;

        Transform thisTransform;
        Transform playerTransform;

        public GameObject[] Prefabs;
        GameObject RandomItem;
        public Vector3 ItemPos = new Vector3( 5.5f, 2.5f, .35f);
        
        public GameObject BoxPrefab;
        GameObject MisteryBox;
        public Vector3 BoxPos = new Vector3(10.5f, 2.5f, .35f);

        public float TimeLapse = 5;
        public string PreviousLevel = "/Levels/Home1.tmx";
        public bool SpecialBonus1 = false;
        public bool SpecialBonus4 = false;
        public GameObject SpecialBonus;

        public Texture BonusTexture;
        bool WinMessage = false;
        bool LoseMessage = false;

        void Start()
        {
            thisTransform = transform;
            playerTransform = Managers.Game.PlayerPrefab.transform;

            MisteryBox = (GameObject)Instantiate(BoxPrefab, BoxPos, transform.rotation);
            MisteryBox.transform.parent = thisTransform; 

            int VarietyNum = Prefabs.Length;
            int RandomNum = Random.Range(0, VarietyNum - 1);
            RandomItem = (GameObject)Instantiate(Prefabs[RandomNum], ItemPos, transform.rotation);
            RandomItem.name = Prefabs[RandomNum].name;
            RandomItem.transform.parent = thisTransform;

        }

        void Update()
        {
            if (playerTransform.position.x < 6.2f && playerTransform.position.y < 3.5f && MisteryBox)
            {
                WinMessage = true;

				Managers.Audio.Play(Cheers, Managers.Display.camTransform);

                DestroyImmediate(MisteryBox);
                Destroy(Instantiate(Resources.Load("Prefabs/Particles/ParticleFlam", typeof(GameObject)),
                          BoxPos + (Vector3.back * 2), thisTransform.rotation), 5);

                Destroy(Instantiate(Resources.Load("Prefabs/Particles/Cheers", typeof(GameObject)),
                          ItemPos + (Vector3.back * 2) + (Vector3.up * 2), thisTransform.rotation), 5);
            }

            if (playerTransform.position.x > 9.8f && playerTransform.position.y < 3.5f  && RandomItem)
            {
                if (!SpecialBonus4)
                    LoseMessage = true;
                else
                {
                    WinMessage = true;
                    SpecialBonus = (GameObject)Instantiate(Resources.Load("Prefabs/Secrets/Treasure4", typeof(GameObject))
                        , BoxPos + (Vector3.back * 2) + (Vector3.up * 2), thisTransform.rotation);
                    SpecialBonus.transform.parent = thisTransform;
                    Managers.Register.Treasure4 = true;
                }

                DestroyImmediate(RandomItem);
                DestroyImmediate(MisteryBox);
                Destroy( Instantiate( Resources.Load( "Prefabs/Particles/ParticleFlam", typeof(GameObject)),
                            ItemPos + (Vector3.back * 2), thisTransform.rotation), 5);

                Destroy(Instantiate(Resources.Load("Prefabs/Particles/Cheers", typeof(GameObject)),
                          BoxPos + (Vector3.back * 2) + (Vector3.up * 2), thisTransform.rotation), 5);
            }

            if (WinMessage || LoseMessage)
            {
                TimeLapse -= Time.deltaTime;

                if (SpecialBonus4 && SpecialBonus && SpecialBonus.transform.position.y >= 2.5f)
                    SpecialBonus.transform.position += Vector3.down * Time.deltaTime;

                if (TimeLapse <= 0)
                    BringMeBackHome();
            }
 
        }

        void OnGUI()
        {
            if ( LoseMessage )
                GUI.DrawTextureWithTexCoords( new Rect((Screen.width * .5f) - (BonusTexture.width * .5f),
                                        (Screen.height * .5f) , 
                                         BonusTexture.width ,  BonusTexture.height * .25f  ),
                                         BonusTexture, 
                                          new Rect( 0, 0, 0.5f, 0.125f) );

            if (WinMessage)
                GUI.DrawTextureWithTexCoords(new Rect((Screen.width * .5f) - (BonusTexture.width * .5f),
                                        (Screen.height * .5f),
                                         BonusTexture.width, BonusTexture.height * .25f),
                                         BonusTexture,
                                          new Rect(0.5f, 0, 0.375f, 0.125f));
        }

        void BringMeBackHome()
        {
            Managers.Display.ShowFlash(0.5f);
            Managers.Tiled.Unload();

            if ( Managers.Tiled.Load(PreviousLevel) )
            {
                //Managers.Register.SetPlayerPos();
                if (SpecialBonus1)
                    //Managers.Display.camTransform.position = new Vector3(2.68f, -1, -2.5f);
                    Managers.Display.camTransform.position = Vector3.zero;
                return;
            }
        }


    }
}
