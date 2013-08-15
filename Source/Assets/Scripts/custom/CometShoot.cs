using UnityEngine;
using System.Collections;

namespace BonusObjects
{
    public class CometShoot : MonoBehaviour
    {
        AudioSource Flame;
        public GameObject Xplode;
        public Transform thisTransform;
        public Transform playerTransform;
        public BulletShot ShootMe;

        bool Shooted = false;

        float ActiveDistance = -2;
        public float ActiveSpeed = 2;

        // Use this for initialization
        void Start()
        {
            thisTransform = transform;
            ShootMe = thisTransform.GetComponent<BulletShot>();
            Flame = thisTransform.GetComponent<AudioSource>();

            ActiveDistance = Random.Range(-3, 2);
            ActiveSpeed = Random.Range(5, 10);
            

            if (  Managers.Game.PlayerPrefab != null)
                playerTransform = Managers.Game.PlayerPrefab.transform;

            StartCoroutine(CoUpdate());
        }

        // Update is called once per frame
        IEnumerator CoUpdate()
        {
            while (true)
            {
                if (playerTransform)
                {
                    if (playerTransform.position.x > thisTransform.position.x + ActiveDistance)
                    {
                        if (!Shooted)
                        {
                            Shooted = true;
                            ShootMe.FireAnimated(new Vector3(.25f, -.75f, 0) * ActiveSpeed, 2, 0, 8);
                            Flame.Play();
                        }
               
                    }

                }
                else
                {
                    if (Managers.Game.PlayerPrefab != null)
                        playerTransform = Managers.Game.PlayerPrefab.transform;
                }

                yield return 0;
            }
        }

        void OnTriggerEnter(Collider hit)
        {
            if ( Shooted && !hit.CompareTag("Item"))
            {
                //Debug.Log("Touch ground!");
                Destroy(Instantiate(Xplode, thisTransform.position + Vector3.down  , Xplode.transform.rotation), 1);
                Destroy(thisTransform.gameObject);
            }
        }
    }
}