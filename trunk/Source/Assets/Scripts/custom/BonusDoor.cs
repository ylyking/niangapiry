using UnityEngine;
using System.Collections;

namespace BonusObjects
{

    public class BonusDoor : MonoBehaviour
    {
        public enum BonusState { Bonus1, Bonus2, Bonus3, Bonus4, Bonus5 };
        public BonusState CurrentBonus = BonusState.Bonus1;

        static bool PlayedBonus1 = false;
        static bool PlayedBonus2 = false;
        static bool PlayedBonus3 = false;
        static bool PlayedBonus4 = false;
        static bool PlayedBonus5 = false;

        void Start()
        {

            switch (CurrentBonus)
            {
                case BonusState.Bonus1:
                    if ( PlayedBonus1 )
                        Destroy(gameObject);
                    break;
                case BonusState.Bonus2:
                    if (PlayedBonus2)
                        Destroy(gameObject);
                    break;
                case BonusState.Bonus3:
                    if (PlayedBonus3)
                        Destroy(gameObject);
                    break;
                case BonusState.Bonus4:
                    if (PlayedBonus4)
                        Destroy(gameObject);
                    break;
                case BonusState.Bonus5:
                    if (PlayedBonus5)
                        Destroy(gameObject);
                    break;
            }

            return;
        }

        void OnTriggerStay( Collider hit)
        {

            if (hit.tag == "Player" && Input.GetAxis("Vertical") > 0)
            {

                switch (CurrentBonus)
                {
                    case BonusState.Bonus1:
                        PlayedBonus1 = true;
                        break;
                    case BonusState.Bonus2:
                        PlayedBonus2 = true;
                        break;
                    case BonusState.Bonus3:
                        PlayedBonus3 = true;
                        break;
                    case BonusState.Bonus4:
                        PlayedBonus4 = true;
                        break;
                    case BonusState.Bonus5:
                        PlayedBonus5 = true;
                        break;
                }
            }
        }


    }

}
