using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using System.Text;
using Ionic.Zlib;

// Remember To always Use 2x Power Texture sizes..

public class TiledReader : EditorWindow
{

    public string TiledTMXFile;										// public var TiledXMLFile 	: TextAsset;
    static public string FolderPath;

    private uint FlippedHorizontallyFlag = 0x80000000;
    private uint FlippedVerticallyFlag = 0x40000000;
    private uint FlippedDiagonallyFlag = 0x20000000;

    public Vector3 TileOutputSize = new Vector3(1, 1, 0);			// Tile Poligonal Modulation inside Unity(Plane)
    public Vector2 eps = new Vector2(0.000005f, 0.000005f);	        // epsilon to fix some Texture bleeding
    public bool PrefabRebuild = false;								// boolean to check mesh and prefabs RE-BUILD

    List<cTileSet> TileSets = new List<cTileSet>();
    public Transform MapTransform;

    [MenuItem("Utility / Tiled Reader %_t")]
    static void Init()
    {
        string source = AssetDatabase.GetAssetPath(Selection.activeObject);

        if ((string.IsNullOrEmpty(source)) ||
        ((source.Remove(0, source.IndexOf(".")) != ".tmx") && (source.Remove(0, source.IndexOf(".")) != ".xml")))
        {
            EditorUtility.DisplayDialog("Select one Tiled File", "You Must Choose a .TMX Tiled file first!", "Ok");
            return;
        }
        TiledReader window = (TiledReader)EditorWindow.GetWindowWithRect(typeof(TiledReader), new Rect(0, 0, 320, 160));

        source = source.Remove(0, 6);
        window.TiledTMXFile = source;
    }

    void OnGUI()
    {
        if (TiledTMXFile == null) return;
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.Separator();
        this.TileOutputSize = EditorGUILayout.Vector3Field(" Tile Output size:", this.TileOutputSize);
        EditorGUILayout.Separator();
        this.eps = EditorGUILayout.Vector2Field(" Offset Compensation: 				(Facu es un putito) ", this.eps);
        //	EditorGUILayout.HelpBox("re-scale texture size inside each tile to fix bleed factor", MessageType.Info);
        EditorGUILayout.Separator();
        this.PrefabRebuild = EditorGUILayout.Toggle(" Re-Build Assets: ", this.PrefabRebuild);
        EditorGUILayout.Separator();

        if (GUILayout.Button(" A COMEEERLA! ")) ReadTiled();
        EditorGUILayout.EndVertical();
    }

    void ReadTiled()
    {
        var sr = new StreamReader(Application.dataPath + TiledTMXFile);					// some String Games

        string FileLoad = "Assets" + TiledTMXFile;											// Assets/etc/File.tmx
        string FilePath = FileLoad.Remove(FileLoad.LastIndexOf("/") + 1);	  				// Assets/etc/
        string FileName = FileLoad.Remove(0, FileLoad.LastIndexOf("/") + 1);				// quit folder path structure
        FileName = FileName.Remove(FileName.LastIndexOf("."));							// quit .xml extension

        FolderPath = FilePath + FileName + " Prefabs/"; 								// Assets/Resources/FileName_Prefabs/
        string FileContent = sr.ReadToEnd();
        sr.Close();
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        XmlDocument Doc = new XmlDocument();
        Doc.LoadXml(FileContent);


        // CHECK IT S A TMX FILE FROM TILED	
        if (Doc.DocumentElement.Name == "map")														// Access root Map		
        {

            // REBUILD TMX OBJECT IN UNITY SCENE						
            DestroyImmediate(GameObject.Find(FileName)); 											// ReBuild the Layers Container & stuff.
            GameObject map = new GameObject(FileName);												// inside the editor hierarchy.
            MapTransform = map.transform;																// take map transform cached

            //(map.AddComponent<LevelAttributes>() as LevelAttributes).bounds = new Rect(0, 0,			// Set Level bounds for camera 
            //    int.Parse(Doc.DocumentElement.Attributes["width"].Value) * TileOutputSize.x,
            //    int.Parse(Doc.DocumentElement.Attributes["height"].Value) * TileOutputSize.y);
            //	map.AddComponent("CombineMeshes");

            if (PrefabRebuild && AssetDatabase.DeleteAsset(FolderPath.Remove(FolderPath.LastIndexOf("/"))))
                Debug.Log(" Folder deleted & Rebuilded");

            Directory.CreateDirectory(FolderPath);												// build a Folder to hold everything
            Directory.CreateDirectory(FolderPath + "Meshes/");									// build a Folder to hold Meshes too

            // SEEK BITMAP SOURCE FILE	 
            foreach (XmlNode TileSetInfo in Doc.GetElementsByTagName("tileset"))				// array of the level nodes.
            {
                var TileSetRef = new cTileSet(TileSetInfo, FilePath);
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

            TileOutputSize.z = 0;

        }
        else Debug.LogError(FileName + " it's not a Tiled File!, wrong load at: " + FilePath);

        Debug.Log("Tiled Level Build Finished");

        this.Close();
    }	// End of Method loader

    void BuildLayer(XmlNode LayerInfo)
    {
        GameObject Layer = new GameObject(LayerInfo.Attributes["name"].Value); // add Layer Childs inside hierarchy.
        Layer.AddComponent<CombineMeshes>();

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
                LayerTransform.position = new Vector3(  LayerTransform.position.x,
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
            byte[] decodedBytes = Ionic.Zlib.GZipStream.UncompressBuffer(Convert.FromBase64String(Data.InnerText));
            for (int tile_index = 0; tile_index < decodedBytes.Length; tile_index += 4)
            {
                uint global_tile_id = (uint)(decodedBytes[tile_index] | decodedBytes[tile_index + 1] << 8 |
                                            decodedBytes[tile_index + 2] << 16 | decodedBytes[tile_index + 3] << 24);


                GameObject TileRef = BuildTile(global_tile_id);
                if (TileRef != null)
                {
                    TileRef.transform.position = new Vector3(ColIndex * TileOutputSize.x,
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
        else if (Data.HasChildNodes) 								// Else if not a Gzip Compression then try as XML data
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
            GameObject TileClone = null;


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

                    //			 	if ( localIndex in TileSets[i].CollisionList ) AddCollision = true;

                    // IF EXISTS SOME PREFAB WITH THE SAME ID THEN USE IT, ELSE CREATE THE OBJECT AND SAVE IN ONE PREFAB 	 	 
                    if (AssetDatabase.LoadAssetAtPath(FolderPath + TileName + ".prefab", typeof(GameObject)))
                    {
                        TileClone = (GameObject)PrefabUtility.InstantiatePrefab(
                            AssetDatabase.LoadAssetAtPath(FolderPath + TileName + ".prefab", typeof(GameObject)));
                    }
                    else
                    {
                        GameObject Tile = new GameObject();

                        Tile.name = TileName;
                        Tile.transform.position = Vector3.zero;

                        MeshFilter meshFilter = (MeshFilter)Tile.AddComponent<MeshFilter>();
                        MeshRenderer meshRenderer = (MeshRenderer)Tile.AddComponent<MeshRenderer>();
                        meshRenderer.sharedMaterial = TileSets[i].mat;

                        Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath(FolderPath + "Meshes/" + TileName + "_mesh.asset", typeof(Mesh));

                        if (m == null)
                        {
                            m = new Mesh();
                            m.name = TileName + "_mesh";
                            m.vertices = new Vector3[4]	{
											new Vector3( TileOutputSize.x, 			  	   0, 0.01f),
											new Vector3( 0, 					 		   0, 0.01f),
											new Vector3( 0, 			 	TileOutputSize.y, 0.01f),
											new Vector3( TileOutputSize.x, 	TileOutputSize.y, 0.01f) };
                            //		   				m.vertices = [ Vector3( TileOutputSize.x *.5f,-TileOutputSize.y *.5f, 0.01),
                            //						 			   Vector3( -TileOutputSize.x*.5f,-TileOutputSize.y *.5f, 0.01),
                            //		    			 			   Vector3( -TileOutputSize.x*.5f, TileOutputSize.y *.5f, 0.01),
                            //		    			 			   Vector3( TileOutputSize.x *.5f, TileOutputSize.y *.5f, 0.01) ];

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

                            vt1.x += -eps.x * Xsign; vt1.y += eps.y * Ysign;
                            vt2.x += eps.x * Xsign; vt2.y += eps.y * Ysign;
                            vt3.x += eps.x * Xsign; vt3.y += -eps.y * Ysign;
                            vt4.x += -eps.x * Xsign; vt4.y += -eps.y * Ysign;

                            if (Rotated)
                            {
                                if (Flipped_X && Flipped_Y || !Flipped_X && !Flipped_Y)
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

                            AssetDatabase.CreateAsset(m, FolderPath + "Meshes/" + TileName + "_mesh.asset");
                            AssetDatabase.SaveAssets();
                        }
                        /////////////////////////////////////////////////////////////////////////////////////////////////////

                        meshFilter.sharedMesh = m;
                        m.RecalculateBounds();

                        if (TileSets[i].Collisions.ContainsKey(localIndex))
                        //						if ( localIndex in TileSets[i].Collisions.Keys )							// Prefab Collision Setup
                        {

                            switch ((string)(TileSets[i].Collisions[localIndex]))
                            {
                                case "Plane":
                                    Tile.AddComponent<MeshCollider>().sharedMesh =
                                    (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Meshes/Planes/1_0 ColPlane.asset", typeof(Mesh));
                                    break;

                                case "Slope":
                                    Tile.layer = 8;
                                    if (Flipped_X && !Flipped_Y || !Flipped_X && Flipped_Y && Rotated || !Flipped_X && Flipped_Y && !Rotated)
                                    {
                                        Tile.AddComponent<MeshCollider>().sharedMesh =
                                            (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Meshes/Planes/SlopeLeft.asset", typeof(Mesh));
                                    }
                                    else
                                    {
                                        Tile.AddComponent<MeshCollider>().sharedMesh =
                                        (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Meshes/Planes/SlopeRight.asset", typeof(Mesh));
                                    }
                                    break;

                                default: 																// same as case "Box":
                                    (Tile.AddComponent("BoxCollider") as BoxCollider).size = Vector3.one;
                                    break;
                            }

                            //			 	  		AddCollision = true;
                        }


                        // AND REPLACE IT WITH THE NEW PREFAB			 	 
                        //			 			Object newPrefab = PrefabUtility.CreateEmptyPrefab( FolderPath + TileName + ".prefab" );
                        UnityEngine.Object newPrefab = PrefabUtility.CreateEmptyPrefab(FolderPath + TileName + ".prefab");
                        PrefabUtility.ReplacePrefab(Tile, newPrefab);						//  Creando prefab con seleccion
                        AssetDatabase.Refresh(); 											// Actualizar inmediatamente
                        DestroyImmediate(Tile);												// Eliminar y reemplazar objeto por Prefab		 					
                        TileClone = (GameObject)PrefabUtility.InstantiatePrefab(
                        AssetDatabase.LoadAssetAtPath(FolderPath + TileName + ".prefab", typeof(GameObject)));

                        //					Debug.Log( "Building new Prefab: " + TileName );
                    }
                    break;
                }
            }
            return TileClone;
        }
        return null;
    }

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
            //		Debug.Log(ObjInfo.Attributes["name"].Value);
            if (ObjInfo.Attributes["name"] == null) continue;

            if ((AssetDatabase.LoadAssetAtPath("Assets/Prefabs/" + ObjInfo.Attributes["name"].Value + ".prefab", typeof(GameObject))))
            {
                GameObject ObjPrefab = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(
                                            "Assets/Prefabs/" + ObjInfo.Attributes["name"].Value + ".prefab", typeof(GameObject)));

                Transform ObjTransform = ObjPrefab.transform;

                ObjTransform.position = new Vector3(
                 (float.Parse(ObjInfo.Attributes["x"].Value) / tilewidth) + (ObjTransform.localScale.x * .5f),		// X
                 height - (float.Parse(ObjInfo.Attributes["y"].Value) / tileheight - ObjTransform.localScale.y * .5f),	// Y		 		     
                                                                                         MapTransform.position.z);		// Z

                ObjTransform.parent = GrpTransform;
            }
            else Debug.LogWarning("Object '" + ObjInfo.Attributes["name"].Value + "' Was not found at: " + "Assets/Prefabs/");
        }
    }

}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



class cTileSet
{

    public int SrcColumns = 1;
    public int SrcRows = 1;
    public int FirstGid = 0;

    public float ModuleWidth = 1.0f;
    public float ModuleHeight = 1.0f;

    public string SrcImgName = "";											// Image Source data references
    public string SrcImgPath = "";

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

            }
            else if (TileSetNode.Name == "tile")
            {
                if (TileSetNode.FirstChild.FirstChild.Attributes["name"].Value == "Collision")
                    Collisions.Add(int.Parse(TileSetNode.Attributes["id"].Value) + 1,
                                                TileSetNode.FirstChild.FirstChild.Attributes["value"].Value);
            }

            // REBUILD AND HOLD MATERIAL		 Search The material inside that folder or create it if not exists and then hold it
            mat = (Material)AssetDatabase.LoadAssetAtPath(TiledReader.FolderPath + SrcImgName + "_Mat" + ".mat", typeof(Material));
            if (mat == null)																	// Check if default material exists . . 
            {

                Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(FilePath + SrcImgPath, typeof(Texture2D));
                if (tex == null)
                {
                    Debug.LogError(SrcImgPath + " texture file not found, put it in the same Tiled map folder: " + FilePath);
                    return;   //	 this.close; 
                }
                else																			// and if not build and save it
                {
                    tex.filterMode = FilterMode.Point;
                    tex.wrapMode = TextureWrapMode.Repeat;
                    tex.anisoLevel = 0;

                    //   	 		mat =  new Material(Shader.Find("Unlit/Transparent Cutout")) ;
                    mat = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
                    mat.mainTexture = tex;

                    AssetDatabase.CreateAsset(mat, TiledReader.FolderPath + SrcImgName + "_Mat" + ".mat");
                    AssetDatabase.SaveAssets();

                    Debug.Log("Re-creating new material: " + SrcImgName + "_Mat");
                }
            }
        }
    }


}







