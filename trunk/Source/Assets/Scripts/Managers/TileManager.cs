using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using System.Text;
using Ionic.Zlib;


/* TILEMANAGER -
 * RULES OF CONSIDERATIONS:
 * 
 * - Load Files From Assets Datafolder as: "/SomeFolder/SomeFile.SomeExtension"
 * 
 * - Use Tiled Editor and rename file extensions from '.TMX' to '.XML' 
 * 
 * - Remember To always Use 2x Power Texture sizes..
 * 
 * - Never mix two diferent TileSet images inside the same Layer
 *  (Remember when Using Resources.Load(..) to quit the first '/' & the last '.whatever' extension)
 * */

public class TileManager : MonoBehaviour {

    private uint FlippedHorizontallyFlag    = 0x80000000;
    private uint FlippedVerticallyFlag      = 0x40000000;
    private uint FlippedDiagonallyFlag      = 0x20000000;

    public Vector3 TileOutputSize           = new Vector3(1, 1, 0);			        // Tile Poligonal Modulation inside Unity(Plane)
    public Vector2 eps                      = new Vector2(0.000005f, 0.000005f);	// epsilon to fix some Texture bleeding
    public bool CombineMesh                 = false;

    List<cTileSet> TileSets = new List<cTileSet>();
    [HideInInspector] public Transform MapTransform;
    private int LastUsedMat = 0;

    public bool Load( string filePath )
    {
        //Debug.Log(Application.dataPath + filePath);
        
        string fileName = filePath.Remove(0, filePath.LastIndexOf("/") + 1);			        // quit folder path structure
        fileName = fileName.Remove(fileName.LastIndexOf("."));							        // quit .tmx or .xml extension

        StreamReader sr = File.OpenText(Application.dataPath + filePath);				        // Do Stream Read
        XmlDocument Doc = new XmlDocument();

        Doc.LoadXml(sr.ReadToEnd());                                                            // and Read XML
        sr.Close();

       // CHECK IT S A TMX FILE FROM TILED	
        if (Doc.DocumentElement.Name == "map")													// Access root Map		
        {
            GameObject map = new GameObject(fileName);											// inside the editor hierarchy.
            MapTransform = map.transform;														// take map transform cached

            (map.AddComponent<LevelAttributes>() as LevelAttributes).bounds = new Rect(0, 0,	// Set Level bounds for camera 
                int.Parse(Doc.DocumentElement.Attributes["width"].Value) * TileOutputSize.x,
                int.Parse(Doc.DocumentElement.Attributes["height"].Value) * TileOutputSize.y);
            //map.AddComponent("CombineMeshes");


            // SEEK BITMAP SOURCE FILE	 
            foreach (XmlNode TileSetInfo in Doc.GetElementsByTagName("tileset"))				// array of the level nodes.
            {
                var TileSetRef = new cTileSet(TileSetInfo, filePath);
                TileSets.Add(TileSetRef);
            }
            // CREATE LAYERS . . .	    
            for (int i = Doc.GetElementsByTagName("layer").Count - 1; i >= 0; i--)
            {
                BuildLayer(Doc.GetElementsByTagName("layer").Item(i));
            }	// end of each Layer Info 

            //	INSTANTIATE PREFABS OBJECTS 
            foreach (XmlNode ObjectsGroup in Doc.GetElementsByTagName("objectgroup"))
            {
                BuildPrefabs(ObjectsGroup);
            }

            //if (CombineMesh)
            //map.AddComponent<CombineMeshes>();
            TileOutputSize.z = 0;
        }
        else
        {
            Debug.LogError(fileName + " it's not a Tiled File!, wrong load at: "+ filePath);
            return false;
        }

        Debug.Log("Tiled Level Build Finished: "+ fileName);
        //this.Close();

        return true;
    }

    public void Unload()
    {
        Destroy(MapTransform);
    }

    void Start()
    {
        // "Init()"
    }

    void OnApplicationQuit() 	
    {
        // "DeInit()"
        Unload();
    }

    void BuildLayer(XmlNode LayerInfo)
    {
        GameObject Layer = new GameObject(LayerInfo.Attributes["name"].Value); // add Layer Childs inside hierarchy.
        //    Layer.AddComponent<CombineMeshes>();

        var LayerTransform = Layer.transform;
        LayerTransform.position = new Vector3(Layer.transform.position.x, Layer.transform.position.y, TileOutputSize.z);
        LayerTransform.parent = MapTransform;

        int ColIndex = 0;
        int RowIndex = int.Parse(LayerInfo.Attributes["height"].Value) - 1;
        bool CollisionLayer = false;

        XmlElement Data = (XmlElement)LayerInfo.FirstChild;
        while (Data.Name != "data")	//		if ( Data.Name == "properties" )    // if Layer data has properties get them
        {
            XmlElement LayerProp = (XmlElement)Data.FirstChild;

            if (LayerProp.GetAttribute("name") == "Collision")
                CollisionLayer = true;

            if (LayerProp.GetAttribute("name") == "Depth")
            {
                LayerTransform.position = new Vector3(LayerTransform.position.x,
                                                        LayerTransform.position.y,
                                                        float.Parse(LayerProp.GetAttribute("value")));
                TileOutputSize.z = LayerTransform.position.z;
            }

            Data = (XmlElement)Data.NextSibling;
        }

        TileOutputSize.z += 0.5f;


        // & CHECK IF DATA IS GZIP COMPRESSED OR DEFAULT XML AND CREATE OR BUILD ALL TILES INSIDE EACH LAYER			
        if (Data.HasAttribute("compression") && Data.Attributes["compression"].Value == "gzip")
        {
            // Decode Base64 and then Uncompress Gzip Tile Information
            byte[] decodedBytes = Ionic.Zlib.GZipStream.UncompressBuffer(System.Convert.FromBase64String(Data.InnerText));
            for (int tile_index = 0; tile_index < decodedBytes.Length; tile_index += 4)
            {
                uint global_tile_id = (uint)(decodedBytes[tile_index] | decodedBytes[tile_index + 1] << 8 |
                                            decodedBytes[tile_index + 2] << 16 | decodedBytes[tile_index + 3] << 24);


                GameObject TileRef = BuildTile(global_tile_id);
                if (TileRef != null)
                {
                    TileRef.transform.position = new Vector3(   ColIndex * TileOutputSize.x,
                                                                RowIndex * TileOutputSize.y,
                                                                LayerTransform.position.z);

                    TileRef.transform.parent = LayerTransform;

                    if (CollisionLayer) (TileRef.AddComponent("BoxCollider") as BoxCollider).size = Vector3.one;
                }

                ColIndex++;
                RowIndex -= System.Convert.ToByte(ColIndex >= int.Parse(LayerInfo.Attributes["width"].Value));
                ColIndex = ColIndex % int.Parse(LayerInfo.Attributes["width"].Value);      // ColIndex % TotalColumns           			
            }//end of each Tile GZIP Compression Info 

        }
        else if (Data.HasChildNodes) 								// Else if not a Gzip Compression then read as XML data
        {
            foreach (XmlNode TileInfo in Data.GetElementsByTagName("tile"))
            {
                var TileRefXml = BuildTile(System.Convert.ToUInt32(TileInfo.Attributes["gid"].Value));
                if (TileRefXml != null)
                {
                    TileRefXml.transform.position = new Vector3(ColIndex * TileOutputSize.x,
                                                                 RowIndex * TileOutputSize.y,
                                                                 LayerTransform.position.z);

                    TileRefXml.transform.parent = LayerTransform;

                    if (CollisionLayer) (TileRefXml.AddComponent("BoxCollider") as BoxCollider).size = Vector3.one;
                }

                ColIndex++;
                RowIndex -= System.Convert.ToByte(ColIndex >= int.Parse(LayerInfo.Attributes["width"].Value));
                ColIndex = ColIndex % int.Parse(LayerInfo.Attributes["width"].Value);

            }//end of each Tile XML Info 
        }
        else Debug.LogError(" Format Error: Save Tiled File in XML style or Compressed mode(Gzip + Base64)");

        if (CombineMesh && LastUsedMat == 0)
            Layer.AddComponent<CombineMeshes>();
    }

    GameObject BuildTile(uint TileId)
    {
        bool Flipped_X = false;
        bool Flipped_Y = false;
        bool Rotated = false;
        //		bool AddCollision	 = false;

        if (TileId != 0)    //	 if ( FirstGid => TileId && TileId <= TotalTiles)	// Si es mayor que 0!				 	 
        {
            uint Index = TileId & ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag);
            string TileName = "Tile_" + Index;

            if ((TileId & FlippedHorizontallyFlag) != 0)
            { Flipped_X = true; TileName += "_H"; }

            if ((TileId & FlippedVerticallyFlag) != 0)
            { Flipped_Y = true; TileName += "_V"; }

            if ((TileId & FlippedDiagonallyFlag) != 0)
            { Rotated = true; TileName += "_VH"; }

            for (int i = TileSets.Count - 1; i >= 0; i--)		// Recorrer en reversa la lista y checar el TileSet firstGid
            {
                if (Index >= TileSets[i].FirstGid)
                {

                    int localIndex = (int)(Index - TileSets[i].FirstGid) + 1;

                    GameObject Tile = new GameObject();                                             // Build new Tile inside layer

                    Tile.name = TileName;
                    Tile.transform.position = Vector3.zero;

                    MeshFilter meshFilter = (MeshFilter)Tile.AddComponent<MeshFilter>();            // Add mesh Filter & Renderer
                    MeshRenderer meshRenderer = (MeshRenderer)Tile.AddComponent<MeshRenderer>();
             
                    meshRenderer.sharedMaterial = TileSets[i].mat;                                  // Set His own Material w/ texture
                    LastUsedMat = i;
                    //Debug.Log("\n Mat index: " + i);

                    Mesh m = new Mesh();                                                            // Prepare mesh UVs offsets
                    m.name = TileName + "_mesh";
                    m.vertices = new Vector3[4]	{
									new Vector3( TileOutputSize.x, 			  	   0, 0.01f),
									new Vector3( 0, 					 		   0, 0.01f),
									new Vector3( 0, 			 	TileOutputSize.y, 0.01f),
									new Vector3( TileOutputSize.x, 	TileOutputSize.y, 0.01f) };

                    int offset_x = localIndex % TileSets[i].SrcColumns;
                    int offset_y = TileSets[i].SrcRows - (Mathf.FloorToInt(localIndex / TileSets[i].SrcColumns) +
                                                System.Convert.ToByte((localIndex % TileSets[i].SrcRows) != 0));


                    Vector2 vt1 = new Vector2((offset_x - System.Convert.ToByte(Flipped_X)) * TileSets[i].ModuleWidth,
                                             ((offset_y + System.Convert.ToByte(Flipped_Y)) * TileSets[i].ModuleHeight));
                    Vector2 vt2 = new Vector2((offset_x - System.Convert.ToByte(!Flipped_X)) * TileSets[i].ModuleWidth,
                                             ((offset_y + System.Convert.ToByte(Flipped_Y)) * TileSets[i].ModuleHeight));
                    Vector2 vt3 = new Vector2((offset_x - System.Convert.ToByte(!Flipped_X)) * TileSets[i].ModuleWidth,
                                             ((offset_y + System.Convert.ToByte(!Flipped_Y)) * TileSets[i].ModuleHeight));
                    Vector2 vt4 = new Vector2((offset_x - System.Convert.ToByte(Flipped_X)) * TileSets[i].ModuleWidth,
                                             ((offset_y + System.Convert.ToByte(!Flipped_Y)) * TileSets[i].ModuleHeight));

                    int Xsign = 1;
                    int Ysign = 1;

                    if (Flipped_X) Xsign = -1;
                    if (Flipped_Y) Ysign = -1;

                    vt1.x += -eps.x * Xsign;    vt1.y +=  eps.y * Ysign;
                    vt2.x +=  eps.x * Xsign;    vt2.y +=  eps.y * Ysign;
                    vt3.x +=  eps.x * Xsign;    vt3.y += -eps.y * Ysign;
                    vt4.x += -eps.x * Xsign;    vt4.y += -eps.y * Ysign;

                    if (Rotated)                                                                // This is some Math for flip 
                    {
                        if (Flipped_X && Flipped_Y || !Flipped_X && !Flipped_Y)                 // & I don't want to explain!
                        {
                            Vector2 vtAux1 = vt2;
                            vt2 = vt4;
                            vt4 = vtAux1;
                        }
                        else
                        {
                            Vector2 vtAux2 = vt3;
                            vt3 = vt1;
                            vt1 = vtAux2;
                        }
                    }

                    m.uv = new Vector2[4] { vt1, vt2, vt3, vt4 };
                    m.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
                    m.RecalculateNormals();
                    m.Optimize();

                    /////////////////////////////////////////////////////////////////////////////////////////////////////

                    meshFilter.sharedMesh = m;
                    m.RecalculateBounds();

                                            
                    if (TileSets[i].Collisions.ContainsKey(localIndex))                         // If there's a Collision property,
                        switch ((string)(TileSets[i].Collisions[localIndex]))                   // Check type and Setup:
                        {
                            case "Plane":
                                Tile.AddComponent<MeshCollider>().sharedMesh =
                                    (Mesh)Resources.Load("Prefabs/Collision/1_0 ColPlane", typeof(Mesh));   // One-Way Plane Collision 
                                break;

                            case "Slope":
                                Tile.layer = 8;
                                if (Flipped_X && !Flipped_Y || !Flipped_X && Flipped_Y && Rotated || !Flipped_X && Flipped_Y && !Rotated)
                                {
                                    Tile.AddComponent<MeshCollider>().sharedMesh =
                                        (Mesh)Resources.Load("Prefabs/Collision/SlopeLeft", typeof(Mesh));  // Left Slope Collision
                                }
                                else
                                {
                                    Tile.AddComponent<MeshCollider>().sharedMesh =
                                        (Mesh)Resources.Load("Prefabs/Collision/SlopeRight", typeof(Mesh)); // Right Slope Collision
                                }
                                break;

                            default:
                                (Tile.AddComponent("BoxCollider") as BoxCollider).size = Vector3.one;       // or default's Box Collision 
                                break;
                        }

                        return Tile;
                    }
                    //break;
                }
            
            }
    return null;
    }                                                                                                       // End of BuidTile Function

    void BuildPrefabs(XmlNode ObjectsGroup)
    {
        int height = int.Parse(ObjectsGroup.ParentNode.Attributes["height"].Value);
        int tilewidth = int.Parse(ObjectsGroup.ParentNode.Attributes["tilewidth"].Value);
        int tileheight = int.Parse(ObjectsGroup.ParentNode.Attributes["tileheight"].Value);
        GameObject ObjGroup = new GameObject(ObjectsGroup.Attributes["name"].Value);
        var GrpTransform = ObjGroup.transform;
        GrpTransform.parent = MapTransform;

        foreach (XmlNode ObjInfo in ObjectsGroup.ChildNodes)
        {
            Debug.Log(ObjInfo.Attributes["name"].Value);
            if (ObjInfo.Attributes["name"] == null) continue;

            string ObjName = "Prefabs/" + ObjInfo.Attributes["name"].Value;

            //if (( Resources.Load("Prefabs/" + ObjInfo.Attributes["name"].Value, typeof(GameObject) ) ) )
            if (( Resources.Load( ObjName, typeof(GameObject) ) ) )
            {

                GameObject ObjPrefab = (GameObject)Instantiate(
                    Resources.Load("Prefabs/" + ObjInfo.Attributes["name"].Value, typeof(GameObject)));

                //ObjPrefab.name = ObjName.Remove(0, ObjName.LastIndexOf("/") +1);

                Transform ObjTransform = ObjPrefab.transform;

                ObjTransform.position = new Vector3(
                 (float.Parse(ObjInfo.Attributes["x"].Value) / tilewidth) + (ObjTransform.localScale.x * .5f),		    // X
                 height - (float.Parse(ObjInfo.Attributes["y"].Value) / tileheight - ObjTransform.localScale.y * .5f),	// Y		 		     
                                                                                         MapTransform.position.z);		// Z
                ObjTransform.name = ObjName.Remove(0, ObjName.LastIndexOf("/") + 1);
                ObjTransform.parent = GrpTransform;
            }
            else Debug.LogWarning("Object '" + ObjName + "' Was not found at: " + "Resources/Prefabs/");
        }
    }
}

class cTileSet
{

    public int SrcColumns       = 1;
    public int SrcRows          = 1;
    public int FirstGid         = 0;

    public float ModuleWidth    = 1.0f;
    public float ModuleHeight   = 1.0f;

    public string SrcImgName    = "";											// Image Source data references
    public string SrcImgPath    = "";

    public Material mat;
    public Dictionary<int, string> Collisions = new Dictionary<int, string>();
    // var Prefabs		: Dictionary.< int, String > = new Dictionary.< int, String >();

    public cTileSet(XmlNode TileSet, string FilePath)			// if ( TileSet.HasChildNodes ) {  var lTileSet : cTileSet = new cTileSet(); lTileSet.Load(
    {

        foreach (XmlNode TileSetNode in TileSet)
        {
            if (TileSetNode.Name == "image")
            {
                int TileInputWidth = System.Convert.ToInt32(TileSet.Attributes["tilewidth"].Value); // Tile width inside bitmap file  ( 64 )
                int TileInputHeight = System.Convert.ToInt32(TileSet.Attributes["tileheight"].Value);// Tile height inside bitmap file 

                int SrcImgWidth = System.Convert.ToInt32(TileSetNode.Attributes["width"].Value); // File Resolution (512)
                int SrcImgHeight = System.Convert.ToInt32(TileSetNode.Attributes["height"].Value);

                SrcColumns = SrcImgWidth / TileInputWidth;
                SrcRows = SrcImgHeight / TileInputHeight;

                this.ModuleWidth = 1.0f / SrcColumns;
                this.ModuleHeight = 1.0f / SrcRows;

                FirstGid = System.Convert.ToInt16(TileSet.Attributes["firstgid"].Value);

                SrcImgName = TileSet.Attributes["name"].Value;						// Image Source data references
                SrcImgPath = TileSetNode.Attributes["source"].Value;
                //SrcImgPath = FilePath.Remove(FilePath.LastIndexOf("/") + 1) + SrcImgName;

            }
            else if (TileSetNode.Name == "tile")
            {
                if (TileSetNode.FirstChild.FirstChild.Attributes["name"].Value == "Collision")
                    Collisions.Add(int.Parse(TileSetNode.Attributes["id"].Value) + 1,
                                                TileSetNode.FirstChild.FirstChild.Attributes["value"].Value);
            }


        #if UNITY_EDITOR
            //SrcImgPath = "Assets" + FilePath.Remove(FilePath.LastIndexOf("/") + 1) + SrcImgPath;
            //Texture2D tex = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(SrcImgPath, typeof(Texture2D));
            //Debug.Log(SrcImgPath);

            //SrcImgPath = FilePath.Remove(FilePath.LastIndexOf("/") + 1) + SrcImgName;
            //Texture2D tex = (Texture2D)Resources.Load(SrcImgPath.Remove(0, 11), typeof(Texture2D));

            //WWW www = new WWW("file://d:/TiledReader.png");
            WWW www = new WWW("file://" + Application.dataPath + FilePath.Remove(FilePath.LastIndexOf("/") + 1) + SrcImgPath);
            Texture2D tex = www.texture;                                                               // We have some 'Ñ' issue
            //Debug.Log        ("file://" + Application.dataPath + FilePath.Remove(FilePath.LastIndexOf("/") + 1) + SrcImgPath);
        #else
            //Debug.Log(        "file://" + Application.dataPath + FilePath.Remove(FilePath.LastIndexOf("/") + 1) + SrcImgPath);
            WWW www = new WWW("file://" + Application.dataPath + FilePath.Remove(FilePath.LastIndexOf("/") + 1) + SrcImgPath);
            Texture2D tex = www.texture;
        #endif

            if (tex == null)
            {
                Debug.LogError(SrcImgPath + " texture file not found, put it in the same Tiled map folder: " + FilePath);
                return;   //	 this.close; 
            }
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.anisoLevel = 0;

            this.mat = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
            this.mat.mainTexture = tex;

        }
    }


}
