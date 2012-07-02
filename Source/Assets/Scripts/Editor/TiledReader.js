#pragma strict
#pragma downcast

import System.Xml;
import System.IO;

class TiledReader extends EditorWindow {										// we need the same name of the script here
	
	public var TiledXMLFile 			: TextAsset;

	private var FlippedHorizontallyFlag : int = 0x80000000;
	private var FlippedVerticallyFlag 	: int = 0x40000000;
	private var FlippedDiagonallyFlag 	: int = 0x20000000;

	public var TileOutputSize			: Vector3 = new Vector3(1.0,1.0,1.0);	// Tile Poligonal Modulation inside Unity(Plane)
	public var eps 						: Vector2 = new Vector2(0, 0.000005f);	// epsilon to fix some Horrendous Texture bleed
	public var PrefabRebuild			: boolean = false;						// boolean to check mesh and prefabs RE-BUILD
	
	@MenuItem("Utility / Tiled Reader %_t")
	static function Init ()  // in one start I think of settings the path input as reference but I guess this is faster & better
	{

		var source : TextAsset = Selection.activeObject as TextAsset;
		
		if (source == null ||
			( AssetDatabase.GetAssetPath(source).Remove(0, AssetDatabase.GetAssetPath(source).IndexOf(".")) != ".xml" ))
		{
			EditorUtility.DisplayDialog("Select one Tiled File", "You Must Select a XML Tiled file first!", "Ok");
			return;
		}
//		var window : TiledLoader = EditorWindow.GetWindow (typeof (TiledLoader)) as TiledLoader;
		var window : TiledReader = EditorWindow.GetWindowWithRect(  typeof(TiledReader), new Rect(0, 0, 320, 160));

		
		window.TiledXMLFile = source;
	}
	
	function OnGUI ()
	{		
		if(TiledXMLFile==null ) return;
		EditorGUILayout.BeginVertical ("box");
		EditorGUILayout.Separator();
		this.TileOutputSize = EditorGUILayout.Vector3Field(" Tile Output size:", this.TileOutputSize);
		EditorGUILayout.Separator();
		this.eps 			= EditorGUILayout.Vector2Field(" Offset Compensation: 				(Facu es un putito) ", this.eps);
		EditorGUILayout.Separator();
		this.PrefabRebuild  = EditorGUILayout.Toggle(" Re-Build Assets: ", this.PrefabRebuild);
		EditorGUILayout.Separator();
		
		if(GUILayout.Button(" A COMEEERLA! ")) ReadTiled();
		EditorGUILayout.EndVertical();
	}


function ReadTiled()
{
	var FileLoad 		: String = AssetDatabase.GetAssetPath(TiledXMLFile);
	var FilePath		: String = FileLoad.Remove( FileLoad.LastIndexOf("/") + 1);	  	// Assets/etc/
	var FileName		: String = FileLoad.Remove( 0, FileLoad.LastIndexOf("/") + 1);	// quit folder path structure
	
	
	FileName = FileName.Remove( FileName.LastIndexOf("."));							  	// quit .xml extension

 ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                   
//   dialogueXMLFile = AssetDatabase.LoadAssetAtPath( FileLoad, typeof(TextAsset) )  ;
   var Doc : XmlDocument = new XmlDocument();
   Doc.LoadXml(TiledXMLFile.text);

    
 if ( Doc ) 
 { 
  var root 	  : XmlElement = Doc.DocumentElement;										// Access Map

// CHECK IT S A TMX FILE FROM TILED	
   if ( root.Name == "map" )
   {
   
// REBUILD TMX OBJECT IN UNITY SCENE						
	DestroyImmediate( GameObject.Find( FileName ) ); 									// ReBuild the Layers Container & stuff.
	var map : GameObject = new GameObject( FileName );									// inside the editor hierarchy.
	var MapTransform = map.transform;													// take map transform cached
//	map.AddComponent("CombineMeshes");
	map.AddComponent("LevelAttributes");
	
//	Debug.Log(Application.dataPath);
	

	var FolderPath : String = FilePath  + FileName + " Prefabs/"; 						// Assets/Resources/FileName_Prefabs/
	
//	Debug.Log( );
	
//	if ( PrefabRebuild && Directory.Exists( Application.dataPath + FolderPath.Remove(0,FolderPath.IndexOf("/"))) )
	//	DeleteDirectory( Application.dataPath + FolderPath.Remove(0,FolderPath.IndexOf("/")) );	

	if ( PrefabRebuild && AssetDatabase.DeleteAsset( FolderPath.Remove(FolderPath.LastIndexOf("/"))) )
		Debug.Log(" Folder deleted & Rebuilded");
//	Debug.Log(" Found folder");
	
	Directory.CreateDirectory( FolderPath );											// build a Folder to hold everything
 	Directory.CreateDirectory( FolderPath + "Meshes/" );								// build a Folder to hold Meshes too
     
// SEEK BITMAP SOURCE FILE	 
	var TileSets : XmlNodeList = Doc.GetElementsByTagName("tileset"); 					// array of the level nodes.
	for ( var TileSet : XmlNode in TileSets )
	{ 
	 	 	 	 	 
     var TileInputWidth : int 	= System.Convert.ToByte( root.GetAttribute("tilewidth")); // Tile width inside bitmap file  ( 64 )
     var TileInputHeight: int 	= System.Convert.ToByte( root.GetAttribute("tileheight"));// Tile height inside bitmap file 
          
	 var SrcImgWidth	: int 	= System.Convert.ToInt16( TileSet.FirstChild.Attributes["width"].Value ); // File Resolution (512)
	 var SrcImgHeight	: int 	= System.Convert.ToInt16( TileSet.FirstChild.Attributes["height"].Value); 
	 
	 var SrcColumns 	: int 	= SrcImgWidth / TileInputWidth;
     var SrcRows 		: int 	= SrcImgHeight / TileInputHeight;
     
     var ModuleWidth 	: double = 1.0 / SrcColumns;
     var ModuleHeight	: double = 1.0 / SrcRows;
	 
	 var SrcImgName		: String = TileSet.Attributes["name"].Value;					// Image Source data references
	 var SrcImgPath		: String = TileSet.FirstChild.Attributes["source"].Value ;

	 var CollisionList  : int[] = new int[(SrcColumns * SrcRows)+1 ];
	 var Properties 	: XmlNodeList = Doc.GetElementsByTagName("property");
	 
	 for ( var Property : XmlNode in Properties )
	 {
	 	if (Property.Attributes["name"].Value == "Collision" && Property.ParentNode.ParentNode.Name == "tile")
//	 	{
//	 		print("Voila!" + int.Parse( Property.ParentNode.ParentNode.Attributes["id"].Value) );
	 		CollisionList[ int.Parse( Property.ParentNode.ParentNode.Attributes["id"].Value ) + 1 ] = 
	 									int.Parse( Property.ParentNode.ParentNode.Attributes["id"].Value ) +1 ;
//	 	}
	 }
	 
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// REBUILD AND HOLD MATERIAL		 Search The material inside that folder or create it if not exists and then hold it
		var mat : Material = AssetDatabase.LoadAssetAtPath( FolderPath + SrcImgName + "_Mat" + ".mat", typeof(Material) );
   
   		if ( mat == null)																// Check if default material exists . . 
    	{
    	 var tex = AssetDatabase.LoadAssetAtPath( FilePath + SrcImgPath , typeof(Texture2D) );
    	 
    	 if ( tex == null)
    	 {
    	 	Debug.Log( SrcImgPath + " texture file not found, put it in the same Tiled map folder: " + FilePath);
    	 	return;
    	 }
    	 else																				// and if not build and save it
     	 {
    	 	tex.filterMode = FilterMode.Point;
    	 	tex.wrapMode = TextureWrapMode.Repeat;
		 	tex.anisoLevel = 0;
    	 
    	 	mat =  new Material(Shader.Find("Transparent/Diffuse")) ;
		 	mat.mainTexture = tex ;
		 
    	 	AssetDatabase.CreateAsset( mat, FolderPath + SrcImgName + "_Mat" + ".mat" );
    	 	AssetDatabase.SaveAssets();
			Debug.Log("Building Assets, please hold a minute.."); 	
    	 	
    	 	Debug.Log("Re-creating new material: " + SrcImgName + "_Mat"); 	 	
    	 }
    	}


	    var LayersList : XmlNodeList = Doc.GetElementsByTagName("layer"); 					// array of the level nodes.
	    
// CREATE LAYERS . . .	    
	    for (  var LayerInfo : XmlNode in LayersList)
	    {
	   		var Layer : GameObject = new GameObject( LayerInfo.Attributes["name"].Value ); // add Layer Childs inside hierarchy.
			Layer.AddComponent("CombineMeshes");
			
			var LayerTransform = Layer.transform;
			LayerTransform.position.z = TileOutputSize.z;
			LayerTransform.parent = MapTransform;
			TileOutputSize.z -= 0.5f;

//	 		var LevelColumns : int = System.Convert.ToInt16( LayerInfo.Attributes["width"].Value  )		// 5	 
//	 		var LevelRows    : int = System.Convert.ToInt16( LayerInfo.Attributes["height"].Value ); 	// 5
	 		
			var ColIndex	 	: int = 0.0;
			var RowIndex		: int = int.Parse( LayerInfo.Attributes["height"].Value );
			var AddCollision	: boolean = false;
			var CollisionLayer	: boolean = false;
			
//			var RowIndex	: int = LevelRows;
			var Data 		: XmlElement = LayerInfo.FirstChild;
	
			if ( Data.Name == "properties" )
			{
				var LayerProp : XmlElement = Data.FirstChild;
				
				Data = Data.NextSibling;
				
				if ( LayerProp.GetAttribute("name") == "Collision")
					CollisionLayer = true;
			}
			
			var TileList 	: XmlNodeList = Data.GetElementsByTagName("tile"); // array of the level nodes.
			
// & CREATE OR BUILD ALL TILES INSIDE			
	        for (  var TileInfo : XmlNode in TileList)
	    	{
			  	var TileId 		: uint = System.Convert.ToUInt32( TileInfo.Attributes["gid"].Value );
				var Flipped_X 	: boolean = false;
				var Flipped_Y 	: boolean = false;
				AddCollision = false;
				

				if ( TileId )    //	 if ( FirstGid => TileId && TileId <= TotalTiles)					 	 
				{				
				 var Index 	  	: uint = TileId & ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag);
			 	 var TileName 	: String = "Tile_" + Index   ;
			 	 
			 	 if (TileId & FlippedHorizontallyFlag)			 	
			 	 {	Flipped_X = true; TileName += "_FlippedX";	}
			 	
			 	 if (TileId & FlippedVerticallyFlag)		
			 	 {	Flipped_Y = true; TileName += "_FlippedY";	}
			 	 
			 	 
			 	 if ( CollisionList[ Index ] == Index   ) AddCollision = true;

			 	 var TileClone: GameObject ;

// IF EXISTS SOME PREFAB WITH THE SAME ID THEN USE IT, ELSE CREATE THE OBJECT AND SAVE IN ONE PREFAB 	 	 
			 	 if ( AssetDatabase.LoadAssetAtPath( FolderPath + TileName + ".prefab", typeof(GameObject) )  )
			 	 {
			 	 	TileClone  = PrefabUtility.InstantiatePrefab( 
			 	 	AssetDatabase.LoadAssetAtPath( FolderPath + TileName + ".prefab", typeof(GameObject) ) );
			 	 	
			 	 }
			 	 else
			 	 {
			 	 	
				 	 var Tile : GameObject = new GameObject();
				 	 
				 	 Tile.name = TileName;
				 	 
				 	 Tile.transform.position = Vector3.zero;
				 	 
				 	 var meshFilter 	 : MeshFilter 	= Tile.AddComponent(typeof(MeshFilter));
		   			 var meshRenderer 	 : MeshRenderer = Tile.AddComponent(typeof(MeshRenderer));
		   			 
	    		 	 meshRenderer.sharedMaterial = mat;
	    		 	 
				var m : Mesh =  AssetDatabase.LoadAssetAtPath( FolderPath + "Meshes/" + TileName + "_mesh.asset", typeof(Mesh) );
					
					if ( m == null )
					{
    				 m  = new Mesh();   
    				   
    				 m.name = TileName + "_mesh" ;
    				 
    				 m.vertices = [Vector3( TileOutputSize.x, 			  	 0, 0.01),
    				 			   Vector3( 0, 					 			 0, 0.01),
    				 			   Vector3( 0, 				  TileOutputSize.y, 0.01),
    				 			   Vector3( TileOutputSize.x, TileOutputSize.y, 0.01) ];

    				 
    				 var offset_x   :int = Index % SrcColumns;
    				 
    				 var offset_y 	:int = SrcRows - (Mathf.FloorToInt( Index / SrcColumns ) + 
    				 						System.Convert.ToByte( ( Index % SrcRows ) != 0 ) );
    				 						
 
//     				 var eps : float = 0.00005f;
//     				 var eps : float = 0.000005f;
     				 m.uv = [ Vector2( (offset_x - System.Convert.ToByte(Flipped_X) ) * ModuleWidth - eps.x,
     				 				    ((offset_y + System.Convert.ToByte(Flipped_Y)) * ModuleHeight ) - eps.y),	// 1-'
     				 
     				 		  Vector2( (offset_x - System.Convert.ToByte(!Flipped_X) ) * ModuleWidth - eps.x,
     				 		  		    ((offset_y + System.Convert.ToByte(Flipped_Y)) * ModuleHeight ) - eps.y),	// 2'-_  
     				 		  
     				 		  Vector2( (offset_x - System.Convert.ToByte(!Flipped_X) ) * ModuleWidth - eps.x,
     				 		  		   ((offset_y + System.Convert.ToByte(!Flipped_Y)) * ModuleHeight ) - eps.y),	// 3¡-    
     				 		  
     				 		  Vector2( (offset_x - System.Convert.ToByte(Flipped_X) )  * ModuleWidth - eps.x,
     				 		           ((offset_y + System.Convert.ToByte(!Flipped_Y)) * ModuleHeight ) - eps.y)];	// 4 ¬
     				 
     				 /////////////////////////////////////////////////////////////////////////////////////////////////////
     				 
    	
    				 m.triangles = [0, 1, 2, 0, 2, 3];
    	
    				 m.RecalculateNormals();
    				 
    				 m.Optimize();
    				 
    				 AssetDatabase.CreateAsset(m, FolderPath + "Meshes/" + TileName + "_mesh.asset");
    					
    				 AssetDatabase.SaveAssets();
			 	 	}
    				 
	     			 
	     			meshFilter.sharedMesh = m;
	     			
	    			m.RecalculateBounds();
	    			
//	    			if ( AddCollision)
//	    			Tile.AddComponent("BoxCollider");

			 	 
			 	 
// AND REPLACE IT WITH THE NEW PREFAB			 	 
 					var newPrefab : Object = PrefabUtility.CreateEmptyPrefab( FolderPath + TileName + ".prefab" );
  
 					PrefabUtility.ReplacePrefab(Tile, newPrefab);						//  Creando prefab con seleccion
 
 					AssetDatabase.Refresh(); 											// Actualizar inmediatamente
 
 					DestroyImmediate(Tile);												// Eliminar y reemplazar objeto por Prefab
 					
 					TileClone  = PrefabUtility.InstantiatePrefab( 
			 	 	AssetDatabase.LoadAssetAtPath( FolderPath + TileName + ".prefab", typeof(GameObject) ) );
 
			 		
					Debug.Log( "Building new Prefab: " + TileName );
			 	 }

			 	 TileClone.transform.position = Vector3( ColIndex * TileOutputSize.x,
			 	 									 	 RowIndex * TileOutputSize.y,
			 	 									 	 TileOutputSize.z);
			 												
			 	 TileClone.transform.parent = LayerTransform; 

			 	 if ( AddCollision || CollisionLayer)
			 	 {
	    			TileClone.AddComponent("BoxCollider");
			 	 	(TileClone.GetComponent("BoxCollider")as BoxCollider).size.z =
			 	 	(TileClone.GetComponent("BoxCollider")as BoxCollider).size.x ;
			 	 }
			 	}
			 	
	 			var LevelColumns : int = System.Convert.ToInt16( LayerInfo.Attributes["width"].Value  );
			 	ColIndex++;
//			  	RowIndex -= System.Convert.ToByte( ColIndex >= TileColumns ); 
			  	RowIndex -= System.Convert.ToByte( ColIndex >= LevelColumns ); 
//			 	ColIndex = ColIndex % TileColumns;
			 	ColIndex = ColIndex % LevelColumns;
		 	}
//		 	TotalTiles *= 2; 
		 }
		 TileOutputSize.z = 0.0;
	}
  }
  else Debug.Log( FileName + " it's not a Tiled File!, try changing extension to .XML");
 }	 
 else Debug.Log("File not found or wrong load at: " + FilePath);
 
 this.Close();
 }
 
// function DeleteDirectory( target_dir : String)
//    {
//        var files : String[] = Directory.GetFiles(target_dir);
//        var dirs  : String[] = Directory.GetDirectories(target_dir);
//
//        for (var file : String in files)
//        {
//            File.SetAttributes(file, FileAttributes.Normal);
//            File.Delete(file);
//        }
//
//        for  (var dir : String in dirs)
//        {
//            DeleteDirectory(dir);
//        }
//
//        Directory.Delete(target_dir, false);
//    }
// 
}