using UnityEngine;
using System.Collections;


public enum ScrollType
{
    Auto,
    Relative
    //Custom
}

public class ScrollLayer : MonoBehaviour
#if !UNITY_FLASH
, System.IComparable<ScrollLayer>
#endif
{
    public ScrollType scroll    = ScrollType.Auto;

    private MeshFilter meshFilter;                                          
    private MeshRenderer meshRenderer;                                      
    private Texture2D texture;
    private string[] textureNames;

    public bool streched        = true;
    public bool pixelPerfect    = false;
    public bool tileX           = true;
    public bool tileY           = true;

    private float xSize         = 10;
    private float ySize         = 10;
    public float weight         = 0;

    public Vector2 speed        = Vector2.zero;
    public Vector2 scale        = Vector2.one;
    public Vector2 offset       = Vector2.zero;
    public Vector2 padding      = Vector2.zero;

    //----------------------------------------------------------------------------------------//

    public Vector2 GetSpeed()
    {
        return speed;
    }
    public ScrollType GetScrollType()
    {
        return scroll;
    }

    public void SetWeight(float weight)
    {
        this.weight = weight;
		
        if ( speed == Vector2.zero )
          this.speed = new Vector2(1f / (weight + 0.00001f), speed.y);
        //else
        //  this.speed = speed;
    }

    public float GetWeight()
    {
        return this.weight;
    }

    public Material GetMaterial()
    {
        if (!meshRenderer)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            ResetMaterial();
        }
        else if (meshRenderer.sharedMaterial == null)
        {
            ResetMaterial();
        }
        return meshRenderer.sharedMaterial;
    }
    public void SetMaterial(Material material)
    {
        if (meshRenderer)
            meshRenderer.sharedMaterial = material;
    }

    public Texture2D GetTexture()
    {
        return texture;
    }
    public void SetTexture(Texture2D texture)
    {
        if (this.texture != texture)
            this.texture = texture;
    }
    public string[] GetTextureNames()
    {
        return textureNames;        
    }
    public string GetTextureName(int index)
    {
        if (textureNames.Length > index)
            return textureNames[index];
        return "";
    }
    public void SetTextureName(int index, string textureName)
    {
        if (textureNames.Length > index)
             textureNames[index] = textureName;
    }

#if !UNITY_FLASH
    public int CompareTo(ScrollLayer other)
    {
        // If other is not a valid object reference, this instance is greater
        if (other == null) return 1;

        // Compare underlying weights
        return GetWeight().CompareTo(other.GetWeight());
    }
#endif
    internal static int Comparision(ScrollLayer layer, ScrollLayer other)
    {
        if (other == null) return 1;

        float weight = layer.GetWeight();
        float otherWeight = other.GetWeight();
        return ( weight < otherWeight ? -1 : (weight > otherWeight ? 1 : 0));
    }


    private void ResetMesh()
    {
        //if (!ScrollManager) return;

        if (!Camera.main || !Camera.main.isOrthoGraphic) 
        { Debug.LogWarning("Scroll Layers can't found ortho Camera");  return; }

        Camera cam          = Camera.main;
        float distance      = Vector3.Distance(cam.transform.position, transform.position);

        Vector3 topLeft     = Vector3.zero;
        Vector3 topRight    = Vector3.zero;
        Vector3 bottomLeft  = Vector3.zero;
        Vector3 bottomRight = Vector3.zero;
        Vector3 ppxy0       = Vector3.zero;
        Vector3 ppxy1       = Vector3.zero;

        Matrix4x4 m         = cam.cameraToWorldMatrix;
        Matrix4x4 mm        = cam.projectionMatrix.inverse;
        float x             = cam.orthographicSize * cam.aspect;

        float width         = 0;
        float height        = 0;

        Vector2 tempOffset = this.offset ;          ///////// <----------- Cam.GetPixelWidth(scrollLayer.transform.position)

        if (texture)
        {
            //width = texture.width + pixelOffset.x;
            //height = texture.height + pixelOffset.y;
            width = texture.width;
            height = texture.height;

            if (!streched && pixelPerfect && tileX && !tileY)
            {
                ppxy0 = cam.transform.position - mm.MultiplyPoint3x4(new Vector3(0, 0, -distance));
                ppxy1 = cam.transform.position - mm.MultiplyPoint3x4(new Vector3(width / cam.pixelWidth, height / cam.pixelHeight, -distance));

                topLeft = cam.transform.position - m.MultiplyPoint3x4(new Vector3(-x + tempOffset.x, -(ppxy1.y - ppxy0.y) + tempOffset.y, -distance));
                topRight = cam.transform.position - m.MultiplyPoint3x4(new Vector3(x + tempOffset.x, -(ppxy1.y - ppxy0.y) + tempOffset.y, -distance));
                bottomLeft = cam.transform.position - m.MultiplyPoint3x4(new Vector3(-x + tempOffset.x, (ppxy1.y - ppxy0.y) + tempOffset.y, -distance));
                bottomRight = cam.transform.position - m.MultiplyPoint3x4(new Vector3(x + tempOffset.x, (ppxy1.y - ppxy0.y) + tempOffset.y, -distance));
            }
            else if (!streched && pixelPerfect && tileY && !tileX)
            {
                ppxy0 = cam.transform.position - mm.MultiplyPoint3x4(new Vector3(0, 0, -distance));
                ppxy1 = cam.transform.position - mm.MultiplyPoint3x4(new Vector3(width / cam.pixelWidth, height / cam.pixelHeight, -distance));

                float xS = -(ppxy1.y - ppxy0.y) * (width / height);

                topLeft = cam.transform.position - m.MultiplyPoint3x4(new Vector3(-xS + tempOffset.x, cam.orthographicSize + tempOffset.y, -distance));
                topRight = cam.transform.position - m.MultiplyPoint3x4(new Vector3(xS + tempOffset.x, cam.orthographicSize + tempOffset.y, -distance));
                bottomLeft = cam.transform.position - m.MultiplyPoint3x4(new Vector3(-xS + tempOffset.x, -cam.orthographicSize + tempOffset.y, -distance));
                bottomRight = cam.transform.position - m.MultiplyPoint3x4(new Vector3(xS + tempOffset.x, -cam.orthographicSize + tempOffset.y, -distance));
            }
            else if (!streched && pixelPerfect && !tileY && !tileX)
            {
                ppxy0 = cam.transform.position - mm.MultiplyPoint3x4(new Vector3(0, 0, -distance));
                ppxy1 = cam.transform.position - mm.MultiplyPoint3x4(new Vector3(width / cam.pixelWidth, height / cam.pixelHeight, -distance));

                float xS = -(ppxy1.y - ppxy0.y) * (width / height);

                topLeft = cam.transform.position - m.MultiplyPoint3x4(new Vector3(-xS + tempOffset.x, -(ppxy1.y - ppxy0.y) + tempOffset.y, -distance));
                topRight = cam.transform.position - m.MultiplyPoint3x4(new Vector3(xS + tempOffset.x, -(ppxy1.y - ppxy0.y) + tempOffset.y, -distance));
                bottomLeft = cam.transform.position - m.MultiplyPoint3x4(new Vector3(-xS + tempOffset.x, (ppxy1.y - ppxy0.y) + tempOffset.y, -distance));
                bottomRight = cam.transform.position - m.MultiplyPoint3x4(new Vector3(xS + tempOffset.x, (ppxy1.y - ppxy0.y) + tempOffset.y, -distance));

            }
            else
            {
                topLeft = cam.transform.position - m.MultiplyPoint3x4(new Vector3(-x + tempOffset.x, cam.orthographicSize +tempOffset.y, -distance));
                topRight = cam.transform.position - m.MultiplyPoint3x4(new Vector3(x + tempOffset.x, cam.orthographicSize + tempOffset.y, -distance));
                bottomLeft = cam.transform.position - m.MultiplyPoint3x4(new Vector3(-x + tempOffset.x, -cam.orthographicSize + tempOffset.y, -distance));
                bottomRight = cam.transform.position - m.MultiplyPoint3x4(new Vector3(x + tempOffset.x, -cam.orthographicSize + tempOffset.y, -distance));
            }

            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[4]{
                new Vector3(topLeft.x, topLeft.y, 0),
                new Vector3(topRight.x, topRight.y, 0),
                new Vector3(bottomLeft.x, bottomLeft.y, 0),
                new Vector3(bottomRight.x, bottomRight.y, 0)
            };
            mesh.triangles = new int[6] { 0, 1, 2, 1, 3, 2 };

            if (!streched)
            {
                float xx = -(x / xSize);
                float yy = -(cam.orthographicSize / ySize);

                if (pixelPerfect && texture)          ///////// <----------- OPTIMIZABLE !             
                {
                    if (tileX && tileY)
                    {
                        ppxy0 = cam.transform.position - mm.MultiplyPoint3x4(new Vector3(0, 0, -distance));
                        ppxy1 = cam.transform.position - mm.MultiplyPoint3x4(new Vector3(width / cam.pixelWidth,
                                                                                         height / cam.pixelHeight,
                                                                                         -distance));
                    }

                    ySize = -(ppxy1.y - ppxy0.y);
                    xSize = ySize * width / height;

                    if (!tileX && !tileY)
					{
						xx = -1 - padding.x;
						yy = -1 - padding.y;
					}
					else
					{
                        if (tileX)
                            xx = -(x / xSize) - padding.x;
                        else
                            xx = -1 - padding.x;

                        if (tileY)
                            yy = -(cam.orthographicSize / ySize) - padding.y;
                        else
                            yy = -1 - padding.y;
                    }
                }

                mesh.uv = new Vector2[4] {
                    new Vector2(padding.x, yy),
                    new Vector2(xx, yy),
                    new Vector2(padding.x, padding.y),
                    new Vector2(xx, padding.y)};
            }
            else
            {
                mesh.uv = new Vector2[4] {
                    new Vector2(    padding.x,  - padding.y),
                    new Vector2(-1 -padding.x,  - padding.y),
                    new Vector2(    padding.x,1 - padding.y),
                    new Vector2(-1 -padding.x,1 - padding.y)};
            }

            mesh.Optimize();

            if (meshFilter.sharedMesh)
                DestroyImmediate(meshFilter.sharedMesh);

            meshFilter.sharedMesh = mesh;

            transform.localScale = scale;
                
        }
    }

    private void ResetMaterial()
    {
        if (meshRenderer.sharedMaterial)
            DestroyImmediate(meshRenderer.sharedMaterial);
        meshRenderer.sharedMaterial = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
        //meshRenderer.sharedMaterial = new Material(Shader.Find("Transparent/Scrolling Layer"));   // Great Shader, but not working here
        textureNames = new string[1] { "_MainTex" };
    }

    private void ResetTexture()
    {
        if (texture)
            meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void UpdateLayer(bool resetMesh = true, bool resetMaterial = true, bool resetTexture = true) // Just a Shortcut
    {
        if (!meshFilter)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        if ( !meshRenderer ) //gameObject.GetComponent<MeshRenderer>() == null )
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.castShadows = false;
            meshRenderer.receiveShadows = false;
        }

        if ( resetMesh)
            ResetMesh();
        if ( resetMaterial)
            ResetMaterial();
        if ( resetTexture)
            ResetTexture();
    }

    void Awake()
    {
        StartCoroutine(SetupLayer());
    }
    IEnumerator SetupLayer()
    {
        yield return new WaitForEndOfFrame();
		 yield return new WaitForEndOfFrame();
        UpdateLayer();      // VS 2008, No rompas las bolas, SI ACEPTA 0 argumentos porque ya los tiene definidos x default
    }

    //void Start()
    //{
    //    UpdateLayer();      // VS 2008, No rompas las bolas, SI ACEPTA 0 argumentos porque ya los tiene definidos x default
    //}
    //----------------------------------------------------------------------------------------//


    //public Dictionary<int, string> Collisions = new Dictionary<int, string>();

    //    // Use this for initialization
    //void Start () {
	
    //}
	
    //// Update is called once per frame
    //void Update () {
	
    //}

}