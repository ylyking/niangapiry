using UnityEngine;
using System.Collections;

namespace CustomAnimations
{

    public class SimpleAnimUpdated : MonoBehaviour
    {
        public enum AnimationType { Default}
        public AnimationType currentType = AnimationType.Default;

        public int RowStart = 0;
        public int ColumnStart = 0;
        public int TotalFrames = 1;
        public int Orientation = 1;
        //public int FramesPerSeconds = 12;


        private AnimSprite simpleAnim;
//        private Transform thisTransform;
//        Vector3 InitPos;

        void Start()
        {

            simpleAnim = gameObject.GetComponent<AnimSprite>() as AnimSprite;
//            thisTransform = transform;
//            InitPos = thisTransform.position;

            //    StartCoroutine(CoUpdate());                                 // Changed Because it can't hold re-activation
        }


        void Update()
        {
            switch (currentType)
            {
                case AnimationType.Default:
                    simpleAnim.PlayFrames(RowStart, ColumnStart, TotalFrames, Orientation);
                    break;
            }
        }

    


        void OnBecameVisible()
        {
            enabled = true;
        }

        void OnBecameInvisible()
        {
            enabled = false;
        }

        //void OnEnable()
        //{
        //    StartCoroutine(CoUpdate());                                 // Changed Because it can't hold re-activation
        //}
    }

}