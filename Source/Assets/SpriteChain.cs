using UnityEngine;
using System.Collections;

public class SpriteChain : MonoBehaviour
{

    public GameObject LinkSprite = null;

    private const int mChainLength = 7;
    private GameObject[] mChainElementArray = new GameObject[mChainLength];
    //private float mX, mY;
    public float DELTA = 1f;
    Transform headTransform;

    // Use this for initialization
    void Start()
    {
        headTransform = this.transform;
        mChainElementArray[0] = gameObject;

        if (LinkSprite != null)
            for (int i = 1; i < mChainLength; i++)
            {
                mChainElementArray[i] = Instantiate(LinkSprite) as GameObject;

                mChainElementArray[i].transform.position = new Vector3(mChainElementArray[i].transform.position.x + DELTA,
                                                                          mChainElementArray[i].transform.position.y + DELTA,
                                                                          mChainElementArray[i].transform.position.z);
            }

 
    }

    // Update is called once per frame
    void Update()
    {
        headTransform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * 10;

        for (int i = 1; i < mChainLength; i++) 
        {
            float dX = mChainElementArray[i - 1].transform.position.x - mChainElementArray[i].transform.position.x;
            float dY = mChainElementArray[i - 1].transform.position.y - mChainElementArray[i].transform.position.y;

            float distance = (float) Mathf.Sqrt(dX * dX + dY * dY);
            dX /= distance;
            dY /= distance;

            mChainElementArray[i].transform.position = new Vector3(mChainElementArray[i - 1].transform.position.x - dX * DELTA,
                                                                    mChainElementArray[i - 1].transform.position.y - dY * DELTA,
                                                                    mChainElementArray[i - 1].transform.position.z);

        }
        
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