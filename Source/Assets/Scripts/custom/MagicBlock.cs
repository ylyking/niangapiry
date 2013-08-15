using UnityEngine;
using System.Collections;

namespace BonusObjects
{
    public class MagicBlock : MonoBehaviour
    {
        BoxCollider collision;
        Renderer render;

        void Awake()
        {
            collision = GetComponent<BoxCollider>();
            render = GetComponent<Renderer>();

            StartCoroutine(CoUpdate());
        }

        IEnumerator CoUpdate()
        {
            while (true)
            {
                render.enabled = Managers.Register.MagicBlockEnabled;
                collision.enabled = Managers.Register.MagicBlockEnabled;
                yield return 0;
            }
        }
    }
}
