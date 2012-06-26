#pragma strict
#pragma downcast

import System.Xml;
import System.IO;
 
public static var dialogueXMLFile 	: TextAsset;

static var FlippedHorizontallyFlag 	: int = 0x80000000;
static var FlippedVerticallyFlag 	: int = 0x40000000;
static var FlippedDiagonallyFlag 	: int  = 0x20000000;

static var TileOutputSize			: float = 1.0;					// Tile Poligonal Modulation inside Unity ( Plane 
static var LayerDepth 				: float = 0.0;					// depth size separation between Layers  


    @MenuItem("GameObject/Create Other/Read Tiled File %_t")
    
static function ReadTiled()
{
	var FileLoad 					: String = "Assets/Resources/" + "W1-1" + ".xml";
	var FilePath					: String = FileLoad.Remove( FileLoad.LastIndexOf("/") + 1);	  // Assets/etc/
	var FileName					: String = FileLoad.Remove( 0, FileLoad.LastIndexOf("/") + 1);// quit folder path structure
	FileName = FileName.Remove( FileName.LastIndexOf("."));										  // quit .xml extension

 ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                   
   dialogueXMLFile = AssetDatabase.LoadAssetAtPath( FileLoad, typeof(TextAsset) )  ;
   var Doc : XmlDocument = new XmlDocument();
   Doc.LoadXml(dialogueXMLFile.text);

    
 if ( Doc ) 
 { 
  var root 	  : XmlElement = Doc.DocumentElement;										// Access Map

// CHECK IT S A TMX FILE	
   if ( root.Name == "map" )
   {
   
// REBUILD TMX OBJECT IN UNITY SCENE						
	DestroyImmediate( GameObject.Find( FileName ) ); 									// ReBuild the Layers Container & stuff.
	var map : GameObject = new GameObject( FileName );									// inside the editor hierarchy.
	var MapTransform = map.transform;													// take map transform cached
//	map.AddComponent("CombineMeshes");
	map.AddComponent("LevelAttributes");
	
		
	var FolderPath : String = FilePath  + FileName + " Prefabs/"; 						// Assets/Resources/FileName_Prefabs/
	Directory.CreateDirectory( FolderPath );											// build a Folder to hold everything
 	Directory.CreateDirectory( FolderPath + "Meshes/" );											// build a Folder to hold Meshes too
     
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
     
     var ModuleWidth 	: float = 1.0 / SrcColumns;
     var ModuleHeight	: float = 1.0 / SrcRows;
	 
	 var SrcImgName		: String = TileSet.Attributes["name"].Value;					// Image Source data references
	 var SrcImgPath		: String = TileSet.FirstChild.Attributes["source"].Value ;

		
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// REBUILD AND HOLD MATERIAL		 Search The material inside that folder or create it if not exists and then hold it
		var mat : Material = AssetDatabase.LoadAssetAtPath( FolderPath + SrcImgName + "_Mat" + ".mat", typeof(Material) );
   
   		if ( mat == null)																// Check if default material exists . . 
    	{
    	 var tex = AssetDatabase.LoadAssetAtPath( FilePath + SrcImgPath , typeof(Texture2D) );
    	 
    	 if ( tex == null)
    	 {
    	 	print("texture file not found, put it in the same Tiled map folder");
    	 }
    	 else																			// and if not build and save it
     	 {
    	 	tex.filterMode = FilterMode.Point;
    	 	tex.wrapMode = TextureWrapMode.Repeat;
		 	tex.anisoLevel = 0;
    	 
    	 	mat =  new Material(Shader.Find("Transparent/Diffuse")) ;
		 	mat.mainTexture = tex ;
		 
    	 	AssetDatabase.CreateAsset( mat, FolderPath + SrcImgName + "_Mat" + ".mat" );
    	 	AssetDatabase.SaveAssets();
    	 	print("Re-creating new material: " + SrcImgName + "_Mat");
    	 }
    	}


	    var LayersList : XmlNodeList = Doc.GetElementsByTagName("layer"); 				// array of the level nodes.
	    
// CREATE LAYERS . . .	    
	    for (  var LayerInfo : XmlNode in LayersList)
	    {
	   		var Layer : GameObject = new GameObject( LayerInfo.Attributes["name"].Value ); // add Layer Childs inside hierarchy.
			Layer.AddComponent("CombineMeshes");
			
			var LayerTransform = Layer.transform;
			LayerTransform.position.z = LayerDepth;
			LayerTransform.parent = MapTransform;
			LayerDepth -= 0.5f;


//	 		var LevelColumns : int = System.Convert.ToInt16( LayerInfo.Attributes["width"].Value  )	// 5	 
//	 		var LevelRows    : int = System.Convert.ToInt16( LayerInfo.Attributes["height"].Value ); // 5
	 		
			var ColIndex	: int = 0.0;
			var RowIndex	: int = System.Convert.ToInt16( LayerInfo.Attributes["height"].Value );
			var AddCollision	: boolean = false;
//			var RowIndex	: int = LevelRows;
			
			var Data 		: XmlElement = LayerInfo.FirstChild;
			if ( Data.Name == "properties" )
			{
				var Properties : XmlElement = Data.FirstChild;
				
				Data = Data.NextSibling;
				
				if ( Properties.GetAttribute("name") == "Collision")
					AddCollision = true;
			}
			
			var TileList 	: XmlNodeList = Data.GetElementsByTagName("tile"); // array of the level nodes.
			
// & CREATE OR BUILD ALL TILES INSIDE			
	        for (  var TileInfo : XmlNode in TileList)
	    	{
			  	var TileId 		: uint = System.Convert.ToUInt32( TileInfo.Attributes["gid"].Value );
				var Flipped_X 	: boolean = false;
				var Flipped_Y 	: boolean = false;

				if ( TileId )    //	 if ( FirstGid => TileId && TileId <= TotalTiles)
					 	 
				{				
				 var Index 	  	: uint = TileId & ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag);
			 	 var TileName 	: String = "Tile_" + Index   ;
			 	 
			 	 if (TileId & FlippedHorizontallyFlag)			 	
			 	 {	Flipped_X = true; TileName += "_FlippedX";	}
			 	
			 	 if (TileId & FlippedVerticallyFlag)		
			 	 {	Flipped_Y = true; TileName += "_FlippedY";	}
			 	 
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
    				 
    				 m.vertices = [Vector3( TileOutputSize, 			  0, 0.01),
    				 			   Vector3( 0, 							  0, 0.01),
    				 			   Vector3( 0, 				 TileOutputSize, 0.01),
    				 			   Vector3( TileOutputSize,  TileOutputSize, 0.01) ];

    				 
    				 var offset_x   :int = Index % SrcColumns;
    				 
    				 var offset_y 	:int = SrcRows - (Mathf.FloorToInt( Index / SrcColumns ) + 
    				 						System.Convert.ToByte( ( Index % SrcRows ) != 0 ) );
    				 						
//    				 var offset_y 	:int = Mathf.FloorToInt( Index / SrcColumns ) + 
//    				 						System.Convert.ToByte( ( Index % SrcRows ) != 0 );		
    				 						
//    				 offset_y = SrcRows - offset_y ;	   
    				 			   
//     				 m.uv = [ Vector2(1, 0),
//     				 		  Vector2(0, 0),
//     				 		  Vector2(0, 1),
//     				 		  Vector2 (1, 1)];

					

     				 m.uv = [ Vector2((offset_x - System.Convert.ToByte(Flipped_X) ) * ModuleWidth,
     				 				  (offset_y + System.Convert.ToByte(Flipped_Y)) * ModuleHeight),	// 1-'
     				 
     				 		  Vector2((offset_x - System.Convert.ToByte(!Flipped_X) ) * ModuleWidth,
     				 		  		   (offset_y + System.Convert.ToByte(Flipped_Y)) * ModuleHeight),	// 2'-_  
     				 		  
     				 		  Vector2((offset_x - System.Convert.ToByte(!Flipped_X) ) * ModuleWidth,
     				 		  		  (offset_y + System.Convert.ToByte(!Flipped_Y)) * ModuleHeight),	//  3¡-    
     				 		  
     				 		  Vector2((offset_x - System.Convert.ToByte(Flipped_X) )  * ModuleWidth,
     				 		         (offset_y + System.Convert.ToByte(!Flipped_Y)) * ModuleHeight)];	//  4¬
     				     
     				 
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
 
			 		
					print( "Building new Prefab: " + TileName );
			 	 }

			 	 TileClone.transform.position = Vector3( ColIndex * TileOutputSize,	RowIndex * TileOutputSize, LayerDepth);
			 												
			 	 TileClone.transform.parent = LayerTransform; 

			 	if ( AddCollision)
	    			TileClone.AddComponent("BoxCollider");
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
		 LayerDepth = 0.0;
	}
  }
  else print( FileName + " it's not a Tiled File!");
 }	 
 else print("File not found or wrong load at: " + FilePath);

}


    
