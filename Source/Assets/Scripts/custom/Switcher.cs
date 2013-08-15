using UnityEngine;
using System.Collections;

namespace BonusObjects
{

    public class Switcher : MonoBehaviour       // Simple Script for switching On/Off all Magic Blocks like light interruptors
    {
        AnimSprite Anime;   /// 6 - 0, 6 - 1
        Transform thisTransform;
        Transform playerTransform;
        public AudioClip SwitchFx;

        // Use this for initialization
        void Start()
        {
            Anime = GetComponentInChildren<AnimSprite>();

            thisTransform = transform;

            playerTransform = Managers.Game.PlayerPrefab.transform;

            if (Managers.Register.MagicBlockEnabled)
                Anime.PlayFrames(6, 0, 1, 1);
            else
                Anime.PlayFrames(6, 1, 1, 1);
        }

        void OnTriggerStay(Collider hit)
        {
            if (hit.tag == "Player")
            {
                if (playerTransform.position.x > thisTransform.position.x && !Managers.Register.MagicBlockEnabled)
                {
                    Managers.Audio.Play(SwitchFx, thisTransform);
                    Managers.Register.MagicBlockEnabled = true;
                    Anime.PlayFrames(6, 0, 1, 1);
                }
                else if (playerTransform.position.x < thisTransform.position.x && Managers.Register.MagicBlockEnabled)
                {
                    Managers.Audio.Play(SwitchFx, thisTransform);
                    Managers.Register.MagicBlockEnabled = false;
                    Anime.PlayFrames(6, 1, 1, 1);
                }


            }
        }

    }

}
