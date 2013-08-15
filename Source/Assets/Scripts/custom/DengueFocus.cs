using UnityEngine;
using System.Collections;


namespace BonusObjects
{

    public class DengueFocus : MonoBehaviour
    {
        static public int TotalFocus = 3;
        Transform thisTransform;
        Transform playerTransform;

        public AudioClip soundCrash;
        public GameObject ParticleStars;

        public GameObject Prefabs;
        public GameObject Treasure;
        public GameObject Cheers;
        GameObject[] Dengues = new GameObject[5];

        public int TotalDengues = 5;
        public float ReSpawnTime = 5;
        public string PreviousLevel = "/Levels/CampoDelCielo.tmx";

        bool WinMessage = false;
        bool LoseMessage = false;

        void Start()
        {
            if (Managers.Register.Treasure4 )
            {
                GameObject naming = (GameObject)Instantiate(Treasure ,
                  thisTransform.position + Vector3.back * .1f + Vector3.up * 2,
                  thisTransform.rotation);
                naming.name = "Treasure2";
                DestroyImmediate(gameObject);
                return;
            }

            thisTransform = transform;
            playerTransform = Managers.Game.PlayerPrefab.transform;

            StartCoroutine(ReStart());
        }

        IEnumerator ReStart()
        {
            ReSpawnTime = Time.time + Random.Range(3, 15);
            int index = 0;

            while (index < Dengues.Length-1)
            {
                if (ReSpawnTime < Time.time)
                {
                    ReSpawnTime = Time.time + Random.Range(1, 15);
                    index++;
                    Dengues[index] = (GameObject)Instantiate(Prefabs, thisTransform.position, thisTransform.rotation);
                }

                yield return 0;
            }
        }

        void OnTriggerEnter(Collider hit)
        {
            if ((hit.CompareTag("Player") && (playerTransform.position.y > thisTransform.position.y + .1)) 
                || hit.CompareTag("p_shot") )
                if (TotalFocus > 0)
            {
                //Managers.Tiled.MapTransform.BroadcastMessage("FocusDestroyed", SendMessageOptions.DontRequireReceiver);
                playerTransform.GetComponent<PlayerControls>().velocity.y = 7;
                Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);
                Managers.Audio.Play(soundCrash, thisTransform, 6.0f, 1.0f);

                TotalFocus--;
                if (TotalFocus == 0 )
                {
                    GameObject naming = (GameObject)Instantiate(Treasure,
                                            thisTransform.position + Vector3.back * .1f + Vector3.up * 3,
                                            thisTransform.rotation);
                    naming.name = "Treasure2";

                    StartCoroutine(GetDownPrefab(naming));

                    Instantiate(Cheers, thisTransform.position + Vector3.up * 3, thisTransform.rotation);
                }
                else
                {
                    Managers.Register.Score += Random.Range(1, 99);
                    Destroy(Instantiate(ParticleStars, thisTransform.position, thisTransform.rotation), 5);
                    Destroy(thisTransform.gameObject);
                }

            }
     
        }

        IEnumerator GetDownPrefab(GameObject Treasure)
        {
            thisTransform.renderer.enabled = false;
            Managers.Register.Score += Random.Range(1, 99);

            while (Treasure && Treasure.transform.position.y > Treasure.transform.position.y - 3)
            {
                Treasure.transform.position += Vector3.down * Time.deltaTime;

                yield return 0;
            }


            Destroy(thisTransform.gameObject);
        }

        void OnDestroy()
        {
            Destroy(Dengues[0]);
            Destroy(Dengues[1]);
            Destroy(Dengues[2]);
            Destroy(Dengues[3]);
            Destroy(Dengues[4]);
        }
    }

}
