using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using System.Text;
using Ionic.Zlib;

/* TILEMANAGER - RULES OF CONSIDERATIONS:
 * 
 * - Load Files From Assets Datafolder as: "/SomeFolder/SomeFile.SomeExtension" ( Don't use 'Ñ' letter)
 * 
 * - Use Tiled Editor and U can rename file extensions from '.TMX' to '.XML' (And save in Gzip + Base64 Compression)
 * 
 * - Remember To always Use 2x Power Texture sizes..
 * 
 * - Never mix two diferent TileSet images inside the same Layer
 *  (when Using Resources.Load(..) always quit the first '/' & the last '.whatever' extension)
 *  
 * - A new Scroll Parallax system was added, the only issue it's that these layers not follow world position b
 * because they are Camera's childs, but the depth property is still working (just with a diferent output)
 * */

public class TileManager : MonoBehaviour {

    private uint FlippedHorizontallyFlag    = 0x80000000;
    private uint FlippedVerticallyFlag      = 0x40000000;
    private uint FlippedDiagonallyFlag      = 0x20000000;

    public Vector3 TileOutputSize           = new Vector3(1, 1, 0);			        // scrollLayer Poligonal Modulation inside Unity(Plane)
    public Vector2 eps                      = new Vector2(0.000005f, 0.000005f);	// epsilon to fix some Texture bleeding
    public bool CombineMesh                 = true;

    List<cTileSet> TileSets = new List<cTileSet>();
    ScrollLayer[] ScrollLayers;
    [HideInInspector] public Transform MapTransform;
    private int LastUsedMat = 0;

    public float ScrollBaseSpeed = 1;
    public  Transform PlayerTransform;
    public  Transform CamTransform;
    private Vector3 oldPos;
    private Vector3 scrollValue;
    //----------------------------------------------------------------------------------------//

    public bool Load( string filePath )
    {
        CamTransform = Managers.Display.MainCamera.transform;
        //Debug.Log(Application.dataPath + filePath);
        if (MapTransform != null )
        {
            Debug.LogWarning("To create a new Map Unload previous one first");
            return false;
        }
        
//        if ( !File.Exists(Application.dataPath + filePath) )
//        {
//            Debug.LogWarning("Couldn't Load the TileMap, File Don't Exists!");
//            return false;
//        }
//        
//        string fileName = filePath.Remove(0, filePath.LastIndexOf("/") + 1);			    // quit folder path structure
//        fileName = fileName.Remove(fileName.LastIndexOf("."));							    // quit .tmx or .xml extension
//
//        StreamReader sr = File.OpenText(Application.dataPath + filePath);				    // Do Stream Read
//        XmlDocument Doc = new XmlDocument();
////		FileData.text.ToString
//        Doc.LoadXml(sr.ReadToEnd());                                                        // and Read XML
//        sr.Close();

        XmlDocument Doc = new XmlDocument();
		Debug.Log ("Levels/" + filePath);
		TextAsset textAsset = (TextAsset) Resources.Load("Levels/" + filePath, typeof(TextAsset));  
		Doc.LoadXml(textAsset.text);    

	
       // CHECK IT'S A TMX FILE FROM TILED	
        if (Doc.DocumentElement.Name == "map")												// Access root Map		
        {
            
			Managers.Register.currentLevelFile = "Levels/" +filePath ;

			GameObject map = new GameObject(filePath);										// inside the editor hierarchy.
            MapTransform = map.transform;													// take map transform cached

            Managers.Display.cameraScroll.ResetBounds(new Rect(0, 0,	// Set Level bounds for camera 
                                            int.Parse(Doc.DocumentElement.Attributes["width"].Value) * TileOutputSize.x,
                                            int.Parse(Doc.DocumentElement.Attributes["height"].Value) * TileOutputSize.y));

            // SEEK BITMAP SOURCE FILE	 
            foreach (XmlNode TileSetInfo in Doc.GetElementsByTagName("tileset"))			// array of the level nodes.
            {
                var TileSetRef = new cTileSet(TileSetInfo);
                TileSets.Add(TileSetRef);
            }

            for (XmlNode Layer = Doc.DocumentElement.LastChild; Layer.Name != "tileset"; Layer = Layer.PreviousSibling)
            {
                switch(Layer.Name)
                {
                    case "layer":                                                           // TagName: TileSet Layers
                        StartCoroutine(BuildLayer(Layer));
                        break;
                    case "imagelayer":                                                      // TagName: Image Layers (for scrolling)
                        StartCoroutine(BuildScrollLayers(Layer));
                        break;
                    case "objectgroup":                                                     // TagName: Object Group Layer
                        StartCoroutine(BuildPrefabs(Layer));
                        break;
                }
            }

            SetupScroll();
        }
        else
        {
			Debug.LogError(filePath + " it's not a Tiled File!, wrong load at: "+ filePath);
            return false;
        }


        if ( Doc.DocumentElement.FirstChild.Name == "properties" )
            foreach(XmlNode MapProperty in Doc.DocumentElement.FirstChild )
            {
                if (MapProperty.Attributes["name"].Value.ToLower() == "music")
                    Managers.Audio.PlayMusic( (AudioClip)Resources.Load("Sound/" + MapProperty.Attributes["value"].Value, typeof(AudioClip)), .3f, 1);
                 
                //if (MapProperty.Attributes["name"].Value.ToLower() == "camerafixedheight")
                //    Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>().FixedHeight = true;

                if (MapProperty.Attributes["name"].Value.ToLower() == "zoom")
                    Managers.Game.PlayerPrefab.GetComponent<CameraTargetAttributes>().distanceModifier = 3.5f;

            }

		Debug.Log("Tiled Level Build Finished: "+ filePath);
        return true;
    }

    public void Unload()
    {
        StopAllCoroutines();

        if ( MapTransform == null ) 
            return;

                
        if ( PlayerTransform != null )
        {
            if (Managers.Game.PlayerPrefab)
                Destroy(Managers.Game.PlayerPrefab);

            PlayerTransform = null;
        }

//        Managers.Audio.StopMusic();
        TileSets.Clear();
        LastUsedMat = 0;
        ScrollBaseSpeed = 1;
        TileOutputSize = new Vector3(1, 1, 0);			         
        oldPos = Vector3.zero;
        scrollValue = Vector3.zero;
        CombineMesh = true;

        Managers.Display.cameraScroll.ResetBounds();

        if ( MapTransform != null )
        {
            Destroy(MapTransform.gameObject);
            MapTransform = null;
        }

        if ( ScrollLayers != null && ScrollLayers.Length > 0)
            for ( int LayerIndex = ScrollLayers.Length - 1; LayerIndex >= 0 ; LayerIndex-- )
            {
                if ( ScrollLayers[LayerIndex] != null )
                    Destroy(ScrollLayers[LayerIndex].gameObject);
            }
        ScrollLayers = null;

        Managers.Register.currentLevelFile = "" ;
  
    }

    void OnApplicationQuit() 	
    {
        // "DeInit()"
        Unload();
    }

    //----------------------------------------------------------------------------------------//

    IEnumerator BuildLayer(XmlNode LayerInfo)
    {
        GameObject Layer = new GameObject(LayerInfo.Attributes["name"].Value); // add Layer Childs inside hierarchy.

        var LayerTransform = Layer.transform;
        LayerTransform.position = new Vector3(Layer.transform.position.x, Layer.transform.position.y, TileOutputSize.z);
        LayerTransform.parent = MapTransform;
        //TileOutputSize.z += 0.5f;

        int ColIndex = 0;
        int RowIndex = int.Parse(LayerInfo.Attributes["height"].Value) - 1;
        uint CollisionLayer = 0;

        XmlElement Data = (XmlElement)LayerInfo.FirstChild;
        while (Data.Name != "data") 
        {
            foreach( XmlElement property in Data)
            {
                if (property.GetAttribute("name").ToLower() == "collision")
                    if (property.GetAttribute("value").ToLower() != "plane") 
                        CollisionLayer = 1;                                         // check if it's a boxed collision and setup
                    else 
                        CollisionLayer = 2;                                         // else it's a plane type collsion Layer..

                if (property.GetAttribute("name").ToLower() == "depth")
                {
                    LayerTransform.position = new Vector3(  LayerTransform.position.x,
                                                            LayerTransform.position.y,
                                                            float.Parse(property.GetAttribute("value")));
                }

            }
            Data = (XmlElement)Data.NextSibling;

        }

        //while (Data.Name != "data")	//		if ( Data.Name == "properties" )    // if Layer data has properties get them
        //{
        //    XmlElement LayerProp = (XmlElement)Data.FirstChild;

        //    if (LayerProp.GetAttribute("name").ToLower() == "collision")
        //        if (LayerProp.GetAttribute("value").ToLower() != "plane") 
        //            CollisionLayer = 1;                                         // check if it's a boxed collision and setup
        //        else 
        //            CollisionLayer = 2;                                         // else it's a plane type collsion Layer..

        //    if (LayerProp.GetAttribute("name").ToLower() == "depth")
        //    {
        //        LayerTransform.position = new Vector3(  LayerTransform.position.x,
        //                                                LayerTransform.position.y,
        //                                                float.Parse(LayerProp.GetAttribute("value")));
        //        //Debug.Log(float.Parse(LayerProp.GetAttribute("value")));
        //    }

        //    Data = (XmlElement)Data.NextSibling;
        //}

        if ( LayerTransform.position.z == TileOutputSize.z)
            TileOutputSize.z += 0.5f;


        // & CHECK IF DATA IS GZIP COMPRESSED OR DEFAULT XML AND CREATE OR BUILD ALL TILES INSIDE EACH LAYER			
        if (Data.HasAttribute("compression") && Data.Attributes["compression"].Value == "gzip")
        {
            // Decode Base64 and then Uncompress Gzip scrollLayer Information
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

                    if (CollisionLayer > 0)
                        if ( CollisionLayer == 1 && TileRef.GetComponent<BoxCollider>() == null) 
                           (TileRef.AddComponent<BoxCollider>() as BoxCollider).size = Vector3.one; // simple boxed collision
                        else if ( TileRef.GetComponent<MeshCollider>() == null)
                           (TileRef.AddComponent<MeshCollider>()as MeshCollider).sharedMesh = 
                            (Mesh)Resources.Load("Prefabs/Collision/1_0 ColPlane", typeof(Mesh));   // One-Way Plane Collision 
                }

                ColIndex++;
                RowIndex -= System.Convert.ToByte(ColIndex >= int.Parse(LayerInfo.Attributes["width"].Value));
                ColIndex = ColIndex % int.Parse(LayerInfo.Attributes["width"].Value);      // ColIndex % TotalColumns 

            }//end of each scrollLayer GZIP Compression Info 

        }

        else Debug.LogError(" Format Error: Save Tiled File in XML style or Compressed mode(Gzip + Base64)");

        if (CombineMesh && LastUsedMat == 0)
            Layer.AddComponent<CombineMeshes>();

        yield return 0;

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

                    GameObject Tile = new GameObject();                                             // Build new scrollLayer inside layer

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
                        switch ((string)(TileSets[i].Collisions[localIndex]).ToLower())                   // Check type and Setup:
                        {
                            case "plane":
                                Tile.AddComponent<MeshCollider>().sharedMesh =
                                    (Mesh)Resources.Load("Prefabs/Collision/1_0 ColPlane", typeof(Mesh));   // One-Way Plane Collision 
                                break;

                            case "slope":
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
    //----------------------------------------------------------------------------------------//

    IEnumerator BuildPrefabs(XmlNode ObjectsGroup)
    {
        int height = int.Parse(ObjectsGroup.ParentNode.Attributes["height"].Value);
        int tilewidth = int.Parse(ObjectsGroup.ParentNode.Attributes["tilewidth"].Value);
        int tileheight = int.Parse(ObjectsGroup.ParentNode.Attributes["tileheight"].Value);
        GameObject ObjGroup = new GameObject(ObjectsGroup.Attributes["name"].Value);
        Transform GrpTransform = ObjGroup.transform;
        GrpTransform.parent = MapTransform;

        //if (ObjectsGroup.Attributes["type"] != null)
        //    Debug.Log("Obj Null Type: "+ ObjectsGroup.Attributes["type"].Value);           // Get complete Obj Layer Props.

        foreach (XmlNode ObjInfo in ObjectsGroup.ChildNodes)
        {
            string ObjName;

             if ( ObjInfo.Attributes["type"] != null)
                 ObjName = ObjInfo.Attributes["type"].Value.ToLower();                      // Check type match
            else if ( ObjInfo.Attributes["name"] != null )
                 ObjName = ObjInfo.Attributes["name"].Value.ToLower();                      // else take it's name as type
            else 
                continue;                                                                   // else discard object

            if ( Resources.Load( "Prefabs/" + ObjName, typeof(GameObject) ) )                
            {
                GameObject ObjPrefab =(GameObject)Instantiate( Resources.Load( "Prefabs/" + ObjName , typeof(GameObject)));

                Transform ObjTransform = ObjPrefab.transform;
                ObjTransform.position = new Vector3(
                    (float.Parse(ObjInfo.Attributes["x"].Value) / tilewidth) + (ObjTransform.localScale.x * .5f),        // X
                    height - (float.Parse(ObjInfo.Attributes["y"].Value) / tileheight - ObjTransform.localScale.y * .5f),// Y		 		     
                                                                                         MapTransform.position.z);		 // Z
                
                if (ObjInfo.Attributes["gid"] == null)                          //  If not a gid it's a Trigger Volume (Great)
                {
                    ObjTransform.localScale = new Vector3(  float.Parse(ObjInfo.Attributes["width"].Value)/tilewidth,
                                                            float.Parse(ObjInfo.Attributes["height"].Value)/tileheight, 1 )  ;
                
                     ObjTransform.position += new Vector3(  (ObjTransform.localScale.x * .5f) - .5f,
                                                            -(ObjTransform.localScale.y * .5f + .5f), 
                                                             MapTransform.position.z );                // Model your own space!
                }


                ObjTransform.name = ObjName.Remove(0, ObjName.LastIndexOf("/") + 1);
                ObjTransform.parent = GrpTransform;

#region OBJS PROPS

                switch (ObjName.ToLower())
                {
                    case "pombero":
                        {
                            Managers.Game.PlayerPrefab = ObjPrefab;
                            Managers.Display.cameraScroll.SetTarget( Managers.Game.PlayerPrefab.transform, false );
                            PlayerTransform = Managers.Game.PlayerPrefab.transform;
                            //Debug.Log("setting up position in TileManager");
                            Managers.Register.SetPlayerPos();

                            foreach (XmlNode ObjProp in ((XmlElement)ObjInfo).GetElementsByTagName("property") )
                            {
                                if (ObjProp.Attributes["name"].Value.ToLower() == "zoom" )
                                    ((CameraTargetAttributes)ObjPrefab.GetComponent<CameraTargetAttributes>()).distanceModifier = 
                                           float.Parse(ObjProp.Attributes["value"].Value.ToLower());

                                if (ObjProp.Attributes["name"].Value.ToLower() == "offset" )
                                    ((CameraTargetAttributes)ObjPrefab.GetComponent<CameraTargetAttributes>()).Offset = 
                                           ReadVector( ObjProp.Attributes["value"].Value.ToLower(), 0);
                            }
                        }
                        break;
                    case "door":
                        goto case "warp";
                    case "warp":
                        {
                            Portal portal = (Portal)ObjPrefab.GetComponent<Portal>();
                            portal.SetType( (Portal.type)Enum.Parse( typeof(Portal.type), ObjName));

                            if ( ((XmlElement)ObjInfo).GetElementsByTagName("property").Item(0) != null )
                                portal.SetTarget( ((XmlElement)ObjInfo).GetElementsByTagName("property").Item(0).Attributes["value"].Value);
                            
                            portal.SetId( ( ObjInfo.Attributes["name"] != null ? ObjInfo.Attributes["name"].Value : ObjName ) );
                        }
                        break;

                    case "flyPlatformA":
                        goto case "flyPlatform";
                    case "flyPlatformB":
                        goto case "flyPlatform";
                    case "flyPlatform":
                        {
                        PlatformMove platform = (PlatformMove)ObjPrefab.GetComponent<PlatformMove>();
                    
                            foreach (XmlNode ObjProp in ((XmlElement)ObjInfo).GetElementsByTagName("property") )
                            {
                                if (ObjProp.Attributes["name"].Value.ToLower() == "target" )
                                {
                                    var target = ReadVector(ObjProp.Attributes["value"].Value, 0);
                                    platform.EndPosition = target;
                                }
                                if (ObjProp.Attributes["name"].Value.ToLower() == "speed" )
                                    platform.Speed = float.Parse( ObjProp.Attributes["value"].Value );
                            }
                        }
                        break;

                    case "chat":
                        {
                            Conversation chat = (Conversation)ObjPrefab.GetComponent<Conversation>();
                            
                            foreach (XmlNode ObjProp in ((XmlElement)ObjInfo).GetElementsByTagName("property") )
                            {
                                if (ObjProp.Attributes["name"].Value.ToLower() == "file" )
                                   chat.ConversationFile = (TextAsset)Resources.Load( ObjProp.Attributes["value"].Value, typeof(TextAsset));
                               
                                if ( ObjProp.Attributes["name"].Value.ToLower() == "oneshotid" ) 
                                {
                                    chat.OneShot = true;
                                    chat.oneShotId = ObjProp.Attributes["value"].Value;
                                    //chat.NameId = chat.oneShotId ;
                                }

                                if ( ObjProp.Attributes["name"].Value.ToLower() == "sound" ) 
                                {
                                    chat.soundChat = (AudioClip)Resources.Load( ObjProp.Attributes["value"].Value, typeof(AudioClip));
                                }

                                if ( ObjProp.Attributes["name"].Value.ToLower() == "nameid" ) 
                                    chat.NameId = ObjProp.Attributes["value"].Value;

                                if ( ObjProp.Attributes["name"].Value.ToLower() == "zoom" ) 
                                    chat.zoom = float.Parse(ObjProp.Attributes["value"].Value);
                                //Managers.Dialog.Init(chat.ConversationFile);
                            }
                                Debug.Log("Deploying Conversation");
                        }
                        break;

                    case "camerabound":
                        {
                            foreach (XmlNode ObjProp in ((XmlElement)ObjInfo).GetElementsByTagName("property") )
                            {
                                if (ObjProp.Attributes["name"].Value.ToLower() == "zoom" )
                                       ((CameraBounds)ObjPrefab.GetComponent<CameraBounds>()).ZoomFactor = 
                                           float.Parse(ObjProp.Attributes["value"].Value);

                                if (ObjProp.Attributes["name"].Value.ToLower() == "offset" )
                                    ((CameraBounds)ObjPrefab.GetComponent<CameraBounds>()).Offset = 
                                           ReadVector( ObjProp.Attributes["value"].Value, 0);
                            }
                        }
                        break;

                    default:
                            foreach (XmlNode ObjProp in ((XmlElement)ObjInfo).GetElementsByTagName("property") )
                            {
                                if (ObjProp.Attributes["name"].Value.ToLower() == "depth" )
                                    ObjPrefab.transform.position += Vector3.forward * 
                                           float.Parse(ObjProp.Attributes["value"].Value);
                                  
                                if (ObjProp.Attributes["name"].Value.ToLower() == "rotation" )
                                    ObjPrefab.transform.localRotation =  Quaternion.Euler( new Vector3( 0, 0,
                                        float.Parse(ObjProp.Attributes["value"].Value)  ) );

                                if (ObjProp.Attributes["name"].Value.ToLower() == "scale" )
                                {
                                    ObjPrefab.transform.localScale = ReadVector(ObjProp.Attributes["value"].Value) ;
                                    ObjPrefab.transform.localScale += Vector3.forward;
                                }

                            }
                        break;
                }

#endregion

            }
            else Debug.LogWarning("Object '" + ObjName + "' Was not found at: " + "Resources/Prefabs/");

            yield return 0;
        }
    }

    //----------------------------------------------------------------------------------------//

    #region ScrollManager 

    IEnumerator BuildScrollLayers(XmlNode LayerInfo)
    {
        if (!Camera.main)
            yield break;

        var cam = Camera.main;
        float Depth = -120;               //bool AutoDepth = true;
       

        GameObject scrollLayer = new GameObject(LayerInfo.Attributes["name"].Value);   // Build new scrollLayer inside layer

                // Config Layer position from Tiled file 'Depth' property or else by Layer order by Default
        scrollLayer.transform.parent = cam.transform;
        scrollLayer.transform.localScale = Vector3.one;

        // Add magic scroll component
        var scroll = scrollLayer.AddComponent<ScrollLayer>();

        //if (LayerInfo.LastChild.Name == "properties")
        //    foreach (XmlNode LayerProp in LayerInfo.LastChild)
        foreach (XmlNode LayerProp in ((XmlElement)LayerInfo).GetElementsByTagName("property") )
            {
                //Debug.Log(LayerProp.Attributes["name"].Value + ": " + LayerProp.Attributes["value"].Value );
                switch (LayerProp.Attributes["name"].Value.ToLower())
                {
                    case "depth":
                        Depth = float.Parse(LayerProp.Attributes["value"].Value);               // Set scroll Layer depth
                    //  AutoDepth = false;
                        break;

                    case "scroll":
                        scroll.scroll = (LayerProp.Attributes["value"].Value.ToLower() == "auto" ? ScrollType.Auto : ScrollType.Relative);
                        break;

                    case "size":
                         if (LayerProp.Attributes["value"].Value.ToLower() == "fullscreen" )
                             scroll.pixelPerfect = !(scroll.streched = true);                   // Set texture streched 
                         else if (LayerProp.Attributes["value"].Value.ToLower() == "original")
                             scroll.pixelPerfect = !(scroll.streched = false);                  // Set texture pixelperfectv
                         else if (LayerProp.Attributes["value"].Value.ToLower() == "repeatx")
                             scroll.pixelPerfect = !(scroll.tileY = scroll.streched = false);   // Set pixelperfect tiling X
                         else if (LayerProp.Attributes["value"].Value.ToLower() == "repeaty")
                             scroll.pixelPerfect = !(scroll.tileX = scroll.streched = false);   // Set pixelperfect tiling Y
                         else if (LayerProp.Attributes["value"].Value.ToLower() == "norepeat")
                             scroll.pixelPerfect = !(scroll.tileX = scroll.tileY = scroll.streched = false);//Set without tile
                         break;

                    case "speed":
                        scroll.speed = ReadVector(LayerProp.Attributes["value"].Value, 0);
                        //Debug.Log("Speed: " +ReadVector(LayerProp.Attributes["value"].Value, 0));
                        break;

                    case "offset":
                        scroll.offset = ReadVector(LayerProp.Attributes["value"].Value, 0);
                        //Debug.Log("offset: " + scroll.offset);
                        break;

                    case "scale":
                        scroll.scale = ReadVector(LayerProp.Attributes["value"].Value);
                        //Debug.Log("scale: " + scroll.scale);
                        break;

                    case "padding":
                        scroll.padding = ReadVector(LayerProp.Attributes["value"].Value);
                        //Debug.Log("padding: " + scroll.padding);
                        break;
                    case "heightrange":
                        scroll.range = ReadVector(LayerProp.Attributes["value"].Value);
                        //Debug.Log("padding: " + scroll.padding);
                        break;
                }
            }
        		        
        // Config Layer position from Tiled file 'Depth' property or else by Layer order by Default
        //scrollLayer.transform.position =  new Vector3( cam.transform.position.x, cam.transform.position.y, Depth - cam.transform.position.z);
        
        if (Depth == -120 )       //if ( AutoDepth)         
        {
            Depth = TileOutputSize.z -( System.Convert.ToSingle(TileOutputSize.z == 0) );
            TileOutputSize.z += 0.5f * System.Convert.ToSingle(TileOutputSize.z != 0);
        }

        scrollLayer.transform.position =  new Vector3( cam.transform.position.x, cam.transform.position.y, Depth );


//#if TEXTURE_RESOURCE
        string AuxPath = "Levels/" + LayerInfo.FirstChild.Attributes["source"].Value;

		Debug.Log (AuxPath.Remove(AuxPath.LastIndexOf(".")) );
        Texture2D tex = (Texture2D)Resources.Load( AuxPath.Remove(AuxPath.LastIndexOf(".")), typeof(Texture2D) );

//#else
//        // Add textures
//        WWW www = new WWW(  "file://" + Application.dataPath + FilePath.Remove(FilePath.LastIndexOf("/") + 1) +
//                            LayerInfo.FirstChild.Attributes["source"].Value);
//        Texture2D tex = www.texture;
//
//#endif

        tex.filterMode = FilterMode.Point;
        tex.anisoLevel = 0;
        scroll.SetTexture(tex);

        cam.ResetProjectionMatrix();

        scroll.UpdateLayer();

        ///////////////////////////////////////////////////////////////////////

        //if ( HackSize )

        yield return 0;
    }


    void SetupScroll()
    {
        //if (Managers.Game.PlayerPrefab)
            //PlayerTransform = Managers.Game.PlayerPrefab.transform;

        List<ScrollLayer> scrollList = new List<ScrollLayer>(FindObjectsOfType(typeof(ScrollLayer)) as ScrollLayer[]);

        //if ( scrollList.Count == 0 ) return;

        foreach (ScrollLayer scroll in scrollList)
        {
            scroll.SetWeight(Vector3.Distance( Managers.Display.camTransform.position, scroll.transform.position));
        }
    #if UNITY_FLASH
            scrollList.sort(ScrollLayer.Comparision);
    #else
            scrollList.Sort();
    #endif
            ScrollLayers = scrollList.ToArray();

    }

    void UpdateScroll()
    {
        foreach (var scrollLayer in ScrollLayers)
        {
            if (!scrollLayer)
                continue;

            scrollLayer.UpdateLayer(true, false, false);
        }
    }

    void LateUpdate()
    {
        if ( ScrollLayers == null) return;

        UpdateScroll();
        //if ( PlayerTransform )
        //{
            //scrollValue = PlayerTransform.position - oldPos;
            //oldPos = PlayerTransform.position;

            scrollValue = CamTransform.position - oldPos;
            oldPos = CamTransform.position;
        //}

        foreach (ScrollLayer scrollLayer in ScrollLayers)
        {
            if (!scrollLayer)
                continue;

            //if (PlayerTransform)
            //    scrollLayer.gameObject.SetActive( ( scrollLayer.range.y > PlayerTransform.position.y &&
            //        scrollLayer.range.x < PlayerTransform.position.y ) );

            if (PlayerTransform)
                scrollLayer.gameObject.SetActive( ( scrollLayer.range.y > Managers.Display.MainCamera.transform.position.y &&
                    scrollLayer.range.x < Managers.Display.MainCamera.transform.position.y ) );

            if (scrollLayer.GetMaterial())
            {
                foreach (string textureName in scrollLayer.GetTextureNames())
                {
                    if (string.IsNullOrEmpty(textureName)) 
                        continue;

                    if (scrollLayer.GetMaterial().HasProperty(textureName))
                    {
                        scrollLayer.GetMaterial().SetTextureOffset( textureName, WrapVector(
                            scrollLayer.GetMaterial().GetTextureOffset(textureName) +
                            //ScrollBaseSpeed * (scrollLayer.GetScrollType() == ScrollType.Auto ?
                            //                    scrollLayer.GetSpeed() * Time.deltaTime  + 
                            //                    new Vector2(scrollValue.x * scrollLayer.GetSpeed().x,
                            //                        scrollValue.y * scrollLayer.GetSpeed().y)   :
                            //                    new Vector2(scrollValue.x * scrollLayer.GetSpeed().x,
                            //                                scrollValue.y * scrollLayer.GetSpeed().y) )));
                            ScrollBaseSpeed * (scrollLayer.GetScrollType() == ScrollType.Auto ?
                                                scrollLayer.GetSpeed() * Time.deltaTime :
                                                new Vector2(scrollValue.x * scrollLayer.GetSpeed().x,
                                                            scrollValue.y * scrollLayer.GetSpeed().y) )));
                    // So, basically If scrollLayer mode is Auto, update by a deltaTime else it's Relative,
                    // create a new Vector with the new Player Position multiplied by each axis speed(stay quiet if zero) 
                    }
                }
            }
        }
        //if ( PlayerTransform )
        //    oldPos = PlayerTransform.position;
    }

    private Vector2 ReadVector(string input, float AxisY = 1) // AxisY sets value for both axis in case of found only 1 value                                                     
    {                                                                                   // seek float values inside string
        if (input.Contains(","))                                                        // if there's a comma, separate things
        {
            return new Vector2( float.Parse( input.Remove( input.IndexOf(",") )),
                                float.Parse( input.Remove(0, input.IndexOf(",") + 1) ));
        }
                                                                                        // else set just the X Axis 
        return new Vector2( float.Parse(input), AxisY * float.Parse(input));            // or both if 'AxisY' is enabled    
    }

    private Vector2 WrapVector(Vector2 input)
    {
        return new Vector2(input.x - (int)input.x, input.y - (int)input.y);
    }

    #endregion

    //----------------------------------------------------------------------------------------//

}

class cTileSet
{

    public int SrcColumns       = 1;
    public int SrcRows          = 1;
    public int FirstGid         = 0;

    public float ModuleWidth    = 1.0f;
    public float ModuleHeight   = 1.0f;

    public string SrcImgName    = "";											        // Image Source data references
    public string SrcImgPath    = "";

    public Material mat;
    public Dictionary<int, string> Collisions = new Dictionary<int, string>();

    public cTileSet(XmlNode TileSet)			// if ( TileSet.HasChildNodes ) {  var lTileSet : cTileSet = new cTileSet(); lTileSet.Load(
    {

        foreach (XmlNode TileSetNode in TileSet)
        {
            if (TileSetNode.Name == "image")
            {
                int TileInputWidth = System.Convert.ToInt32(TileSet.Attributes["tilewidth"].Value); // scrollLayer width inside bitmap file  ( 64 )
                int TileInputHeight = System.Convert.ToInt32(TileSet.Attributes["tileheight"].Value);// scrollLayer height inside bitmap file 

                int SrcImgWidth = System.Convert.ToInt32(TileSetNode.Attributes["width"].Value); // File Resolution (512)
                int SrcImgHeight = System.Convert.ToInt32(TileSetNode.Attributes["height"].Value);

                SrcColumns = SrcImgWidth / TileInputWidth;
                SrcRows = SrcImgHeight / TileInputHeight;

                this.ModuleWidth = 1.0f / SrcColumns;
                this.ModuleHeight = 1.0f / SrcRows;

                FirstGid = System.Convert.ToInt16(TileSet.Attributes["firstgid"].Value);

                SrcImgName = TileSet.Attributes["name"].Value;						    // Image Source data references
                SrcImgPath = TileSetNode.Attributes["source"].Value;
                //SrcImgPath = FilePath.Remove(FilePath.LastIndexOf("/") + 1) + SrcImgName;

            }
            else if (TileSetNode.Name == "tile")
            {
                if (TileSetNode.FirstChild.FirstChild.Attributes["name"].Value.ToLower() == "collision")
                    Collisions.Add(int.Parse(TileSetNode.Attributes["id"].Value) + 1,
                                                TileSetNode.FirstChild.FirstChild.Attributes["value"].Value);
            }



//        #if TEXTURE_RESOURCE
            //SrcImgPath = "Assets" + FilePath.Remove(FilePath.LastIndexOf("/") + 1) + SrcImgPath;
            //Texture2D tex = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(SrcImgPath, typeof(Texture2D));
            //Debug.Log(SrcImgPath);

//            SrcImgPath = Managers.Register.currentLevelFile;
			Debug.Log ("Levels/" + SrcImgPath.Remove(SrcImgPath.LastIndexOf(".")) );
			           Texture2D tex = (Texture2D)Resources.Load("Levels/" + SrcImgPath.Remove(SrcImgPath.LastIndexOf(".")), typeof(Texture2D) );

//			Debug.Log (AuxPath.Remove(AuxPath.LastIndexOf(".")) );
//			string AuxPath = "Levels/" + LayerInfo.FirstChild.Attributes["source"].Value;
//			Texture2D tex = (Texture2D)Resources.Load( AuxPath.Remove(AuxPath.LastIndexOf(".")), typeof(Texture2D) );

            //        #else
//            //Debug.Log(        "file://" + Application.dataPath + FilePath.Remove(FilePath.LastIndexOf("/") + 1) + SrcImgPath);
//            WWW www = new WWW("file://" + Application.dataPath + FilePath.Remove(FilePath.LastIndexOf("/") + 1) + SrcImgPath);
//            Texture2D tex = www.texture;
//
//        #endif

            if (tex == null)
            {
				Debug.LogError( " texture file not found, put it in the same Tiled map folder: " + SrcImgPath);
                return;   //	 this.close; 
            }
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.anisoLevel = 0;

            //this.mat = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
            this.mat = new Material(Shader.Find("Unlit/Transparent"));
            this.mat.mainTexture = tex;

        }
    }


}


