using UnityEngine;
using System.Collections;

public class SpriteChain : MonoBehaviour
{

    public GameObject LinkSprite = null;

    private const int mChainLength = 18;
    private GameObject[] mChainElementArray = new GameObject[mChainLength];
    //private Vector3[] mTargetArray = new Vector3[mChainLength];

    public float DELTA = 1f;
    private float timeCheck = 0;

    public enum TranslationMode { Smooth, Retro, custom };
    public TranslationMode MoveStyle = TranslationMode.Smooth;

    public ParticleSystem particle;

    //Transform headTransform;

    // Use this for initialization
    void Start()
    {
        //headTransform = this.transform;
        mChainElementArray[0] = gameObject;

        if (LinkSprite != null)
            for (int i = 1; i < mChainLength; i++)
            {
                mChainElementArray[i] = Instantiate(LinkSprite, transform.position + Vector3.one * i, transform.rotation) as GameObject;

                mChainElementArray[i].transform.position = new Vector3(mChainElementArray[i].transform.position.x + DELTA * i,
                                                                          mChainElementArray[i].transform.position.y + DELTA * i,
                                                                          mChainElementArray[i].transform.position.z+0.00001f);
            }

        //for (int i = 0; i < mChainLength; i++)
        //{
        //    mTargetArray[i] = mChainElementArray[i].transform.position;
        //}

 
    }




    // Update is called once per frame
    void Update()
    {
        //headTransform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * TimeLapse.deltaTime * 10;

        if (MoveStyle == TranslationMode.Smooth)
            for (int i = 1; i < mChainLength; i++)
            {
                float dX = mChainElementArray[i - 1].transform.position.x - mChainElementArray[i].transform.position.x;
                float dY = mChainElementArray[i - 1].transform.position.y - mChainElementArray[i].transform.position.y;

                float distance = (float)Mathf.Sqrt(dX * dX + dY * dY);

                dX /= distance;
                dY /= distance;

                mChainElementArray[i].transform.position = new Vector3(mChainElementArray[i - 1].transform.position.x - dX * DELTA,
                                                                        mChainElementArray[i - 1].transform.position.y - dY * DELTA,
                                                                        mChainElementArray[i - 1].transform.position.z + 0.00001f);
            }


        if ( MoveStyle == TranslationMode.Retro )
            if (Time.time > timeCheck)   // This gives an interesting retro look 
            {
                timeCheck = Time.time + 0.09f;
                for (int i = mChainLength - 1; i > 0; i--)
                {
                    mChainElementArray[i].transform.position = new Vector3(mChainElementArray[i - 1].transform.position.x,
                                                                            mChainElementArray[i - 1].transform.position.y,
                                                                            mChainElementArray[i - 1].transform.position.z + 0.00001f);
                }
            }


        //timer = TimeLapse.deltaTime * 100;

        //// float timer = 0;
        //if ( Mathf.Approximately( mTargetArray[0].x,  mChainElementArray[1].transform.position.x) &&
        //    Mathf.Approximately( mTargetArray[0].y,  mChainElementArray[1].transform.position.y) )
        //    for (int i = 0; i < mChainLength; i++)
        //    {
        //        //timer = 0;
        //        mTargetArray[i] = mChainElementArray[i].transform.position;
        //    }

        // float smoothTime = 0.005f;
        // Vector3 velocity = Vector3.zero;


        //for (int i = 1; i < mChainLength; i++)
        //    mChainElementArray[i].transform.position = Vector3.SmoothDamp(
        //        mChainElementArray[i].transform.position, mTargetArray[i - 1], ref velocity, smoothTime);
        ////   mChainElementArray[i].transform.position = Vector3.Slerp(mChainElementArray[i].transform.position, mTargetArray[i - 1], timer );

        
    }

    void OnDestroy()
    {
        for (int i = 1; i < mChainLength; i++)
        {
            Destroy( mChainElementArray[i] );
        }

    }

    public void ResetLinks()
    {
        for (int i = 1; i < mChainLength; i++)
            mChainElementArray[i].transform.position =
                new Vector3(transform.position.x, transform.position.y, mChainElementArray[i].transform.position.z);
    }

    public void DestroyLinks()
    {
        //for (int i = mChainLength -1; i > 0; i--)
        //{
        //    Destroy(Instantiate(particle,
        //            mChainElementArray[i].transform.position,
        //            mChainElementArray[i].transform.rotation), 5);

        //    Destroy(mChainElementArray[i]);

        //}
        MoveStyle = TranslationMode.custom;
        StartCoroutine(ExplodeLinks());
    }

    IEnumerator ExplodeLinks()
    {
       int i = mChainLength -1;

       while (i >= -1)
       {
           Destroy(Instantiate(particle, mChainElementArray[i].transform.position,
                                            mChainElementArray[i].transform.rotation), 5);
           Destroy(mChainElementArray[i]);

           i--;

           yield return new WaitForSeconds(0.25f);
       }

       yield return 0;
    }



    //private GameObject spawnLink(float pX, float pY) {
    //final Sprite chainLink = new Sprite(pX, pY, mTextureRegion.deepCopy());
    //chainLink.setWidth(16);
    //chainLink.setHeight(16);
    //chainLink.setScale(scale);
    //        return chainLink;
    //}
}
/*
      private TextureRegion mTextureRegion;
        private Sprite[] mChainElementArray;
        private int mChainLength = 0;
        private float mX, mY;
        private float DELTA = 7f;
 
        public SpriteChain(TextureRegion pTextureRegion, int pChainLength,
                        float pX, float pY) {
                this.mTextureRegion = pTextureRegion;
                this.mChainLength = pChainLength;
                this.mX = pX;
                this.mY = pY;
 
                mChainElementArray = new Sprite[pChainLength];
                mChainElementArray[0] = spawnLink(pX, pY);
 
                for (int i = 1; i < mChainLength; i++) {
                        mChainElementArray[i] = spawnLink(mChainElementArray[i - 1].getX(),
                                        mChainElementArray[i - 1].getY() + DELTA);
                }
        }
 
        @Override
        public void setPosition(float pX, float pY) {
                super.setPosition(pX, pY);
 
                mChainElementArray[0].setPosition(pX, pY);
                for (int i = 1; i < mChainLength; i++) {
                        float dX = mChainElementArray[i - 1].getX()
                                        - mChainElementArray[i].getX();
                        float dY = mChainElementArray[i - 1].getY()
                                        - mChainElementArray[i].getY();
                        final float distance = (float) Math.sqrt(dX * dX + dY * dY);
                        dX /= distance;
                        dY /= distance;
 
                        mChainElementArray[i].setPosition(mChainElementArray[i - 1].getX()
                                        - dX * DELTA, mChainElementArray[i - 1].getY()
                                        - dY * DELTA);
 
                }
        }
 
        private Sprite spawnLink(float pX, float pY) {
                return this.spawnLink(pX, pY, 1.0f);
        }
 
        private Sprite spawnLink(float pX, float pY, float scale) {
                final Sprite chainLink = new Sprite(pX, pY, mTextureRegion.deepCopy());
                chainLink.setWidth(16);
                chainLink.setHeight(16);
                chainLink.setScale(scale);
                return chainLink;
        }
 
        public void setDelta(float pDelta) {
                this.DELTA = pDelta;
        }
 
        @Override
        public float getX() {
                return mChainElementArray[0].getX();
        }
 
        @Override
        public float getY() {
                return mChainElementArray[0].getY();
        }
 
        @Override
        protected void onManagedDraw(final GL10 pGL, final Camera pCamera) {
                for (int i = 0; i < mChainElementArray.length; i++) {
                        mChainElementArray[i].onDraw(pGL, pCamera);
                }
        }
}
*/