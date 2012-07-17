
import System.Collections.Generic;
import System.Xml;
import System.IO;



class TiledReader extends EditorWindow {										// we need the same name of the script here
	
 public var TiledXMLFile 			: TextAsset;

 private var FlippedHorizontallyFlag: int = 0x80000000;
 private var FlippedVerticallyFlag 	: int = 0x40000000;
 private var FlippedDiagonallyFlag 	: int = 0x20000000;

 public var TileOutputSize			: Vector3 = new Vector3(1.0,1.0,1.0);		// Tile Poligonal Modulation inside Unity(Plane)
 public var eps 					: Vector2 = new Vector2(0.000005f, 0.000005f);// epsilon to fix some Texture bleeding
 public var PrefabRebuild			: boolean = false;							// boolean to check mesh and prefabs RE-BUILD

	
 @MenuItem("Utility / Tiled Reader %_t")
 static function Init ()  // in one start I think of settings the path input as reference but I guess this is faster & better
 {
 	var source : TextAsset = Selection.activeObject as TextAsset; 
		
	if ((source == null )||
 		( AssetDatabase.GetAssetPath(source).Remove(0, AssetDatabase.GetAssetPath(source).IndexOf(".")) != ".xml" ))
	{
		EditorUtility.DisplayDialog("Select one Tiled File", "You Must Select a XML Tiled file first!", "Ok");
		return;
	}
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
	this.eps 	= EditorGUILayout.Vector2Field(" Offset Compensation: 				(Facu es un putito) ", this.eps);
	EditorGUILayout.Separator();
	this.PrefabRebuild  = EditorGUILayout.Toggle(" Re-Build Assets: ", this.PrefabRebuild);
	EditorGUILayout.Separator();
		
	if(GUILayout.Button(" A COMEEERLA! ")) ReadTiled();
		EditorGUILayout.EndVertical();
 }
 
 function ReadTiled()
{
	var FileLoad 	: String = AssetDatabase.GetAssetPath(TiledXMLFile);
	var FilePath	: String  = FileLoad.Remove( FileLoad.LastIndexOf("/") + 1);	  		// Assets/etc/
	var FileName	: String = FileLoad.Remove( 0, FileLoad.LastIndexOf("/") + 1);			// quit folder path structure
		FileName = FileName.Remove( FileName.LastIndexOf("."));								// quit .xml extension
		
	var FolderPath 	: String = FilePath  + FileName + " Prefabs/"; 							// Assets/Resources/FileName_Prefabs/
	

 ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                   
//   dialogueXMLFile = AssetDatabase.LoadAssetAtPath( FileLoad, typeof(TextAsset) )  ;
   var Doc : XmlDocument = new XmlDocument();
   Doc.LoadXml(TiledXMLFile.text);

    
 if ( Doc ) 
 { 
// CHECK IT S A TMX FILE FROM TILED	
   if ( Doc.DocumentElement.Name == "map" )													// Access root Map		
   {
   
// REBUILD TMX OBJECT IN UNITY SCENE						
	DestroyImmediate( GameObject.Find( FileName ) ); 										// ReBuild the Layers Container & stuff.
	var map : GameObject = new GameObject( FileName );										// inside the editor hierarchy.
	var MapTransform = map.transform;														// take map transform cached
	map.AddComponent("LevelAttributes");	//	map.AddComponent("CombineMeshes");
	
	if ( PrefabRebuild && AssetDatabase.DeleteAsset( FolderPath.Remove(FolderPath.LastIndexOf("/"))) )
		Debug.Log(" Folder deleted & Rebuilded");
	
	Directory.CreateDirectory( FolderPath );												// build a Folder to hold everything
 	Directory.CreateDirectory( FolderPath + "Meshes/" );									// build a Folder to hold Meshes too

     
                          
// SEEK BITMAP SOURCE FILE	 
	var TileSetList 		: XmlNodeList 	= Doc.GetElementsByTagName("tileset"); 						// array of the level nodes.
    var TileSets 			: cTileSet[] 	= new cTileSet[TileSetList.Count] ; 
    var TileSetIndex		: int 			= 0;
    
	Debug.Log("Building Assets from " + TileSetList.Count + " TileSets, please hold a minute.."); 	
	
	for ( var TileSet 		: XmlNode in TileSetList )
	{ 	
	//  TileSets[TileSetIndex] =  new cTileSet();
	//  TileSets[TileSetIndex].LoadNode( TileSet, FolderPath, FilePath); 	 	
	 	 	 
   		TileSets[TileSetIndex] =  new cTileSet(TileSet, FolderPath, FilePath);
   		TileSetIndex += 1 ;
   	}
	 	
	 
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// CREATE LAYERS . . .	    
	var LayersList : XmlNodeList = Doc.GetElementsByTagName("layer"); 					// array of the level nodes.	    
	for (  var LayerInfo : XmlNode in LayersList)
	{
		var Layer : GameObject = new GameObject( LayerInfo.Attributes["name"].Value ); // add Layer Childs inside hierarchy.
		Layer.AddComponent("CombineMeshes");
			
		var LayerTransform = Layer.transform;
		LayerTransform.position.z = TileOutputSize.z;
		LayerTransform.parent = MapTransform;
		TileOutputSize.z -= 0.5f;
	 		
		var ColIndex	 	: int = 0.0;
		var RowIndex		: int = int.Parse( LayerInfo.Attributes["height"].Value );
		var CollisionLayer	: boolean = false;
			
		var Data 			: XmlElement = LayerInfo.FirstChild;
		while ( Data.Name != "data"  )	
//		if ( Data.Name == "properties" )										// if Layer data has properties get them											// if Layer data has properties get them
		{
			var LayerProp : XmlElement = Data.FirstChild;
				
			if ( LayerProp.GetAttribute("name") == "Collision" )
				CollisionLayer = true;
				
			if ( LayerProp.GetAttribute("name") == "Depth" )
				LayerTransform.position.z = float.Parse( LayerProp.GetAttribute("value"));
			
			Data = Data.NextSibling;
				
		}


// & CREATE OR BUILD ALL TILES INSIDE EACH LAYER			
		var TileList 		: XmlNodeList = Data.GetElementsByTagName("tile"); // array of the level nodes.			
	    for (  var TileInfo : XmlNode in TileList)
	  	{
	  		// if ( GameReferences ) 
			var TileRef = BuildTile( TileInfo, TileSets, FolderPath);
			if (TileRef != null)
    		{
				TileRef.transform.position = Vector3( ColIndex * TileOutputSize.x,
													  RowIndex * TileOutputSize.y,
													  LayerTransform.position.z );
			 												
			 	TileRef.transform.parent = LayerTransform;
			 	
			 	if ( CollisionLayer) 
			 		(TileRef.AddComponent("BoxCollider") as BoxCollider).size = Vector3.one;

                                
		 	}
			 	
	 		var LevelColumns : int = System.Convert.ToInt16( LayerInfo.Attributes["width"].Value  );
			ColIndex++;
			RowIndex -= System.Convert.ToByte( ColIndex >= LevelColumns ); 
			ColIndex = ColIndex % LevelColumns;
		
		}//end of each Tile Info
		 	
		 	
	}	// end of each Layer Info 
		 
	TileOutputSize.z = 0.0;
	}		 
	
 else Debug.Log( FileName + " it's not a Tiled File!, try changing extension to .XML");
 }	 		 
 
 else Debug.Log("File not found or wrong load at: " + FilePath);
 
 Debug.Log("Tiled Level Build Sucesfully "); 	 	
 
 this.Close();
}			// End of Method loader
 
 function BuildTile( TileInfo : XmlNode, TileSets : cTileSet[], FolderPath : String) : GameObject
{
 	var TileId 		: uint = System.Convert.ToUInt32( TileInfo.Attributes["gid"].Value );
	var Flipped_X 	: boolean = false;
	var Flipped_Y 	: boolean = false;
	var Rotated 	: boolean = false;
	var AddCollision: boolean = false;

	if ( TileId )    //	 if ( FirstGid => TileId && TileId <= TotalTiles)	// Si es mayor que 0!				 	 
	{				
		var Index 	  	: uint = TileId & ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag);
		var TileName 	: String = "Tile_" + Index   ;
		var TileClone	: GameObject ;
		
		 	 
		if (TileId & FlippedHorizontallyFlag)			 	
		{	Flipped_X = true; TileName += "_FlipX";	}
			 	
		if (TileId & FlippedVerticallyFlag)		
		{	Flipped_Y = true; TileName += "_FlipY";	}
			 	 
		if (TileId & FlippedDiagonallyFlag)		
		{	Rotated = true; TileName += "_Rotated";	}
					 	 			 	 
		for ( var i : int = TileSets.Length -1; i >= 0; i--)		// Recorrer en reversa la lista y checar el TileSet firstGid
		{
			if ( Index >= TileSets[i].FirstGid )
		 	{			 	 	
				var localIndex : int = (Index - TileSets[i].FirstGid) + 1;
				 	 				 	 
//			 	if ( localIndex in TileSets[i].CollisionList ) AddCollision = true;
			 	

	
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
				   			 
		    		meshRenderer.sharedMaterial = TileSets[i].mat;
			    		 	 
					var m : Mesh =  AssetDatabase.LoadAssetAtPath( FolderPath + "Meshes/" + TileName + "_mesh.asset", typeof(Mesh) );
							
					if ( m == null )
					{
		   				m  = new Mesh();   
		    				   
		   				m.name = TileName + "_mesh" ;
		   				 
		   				m.vertices = [Vector3( TileOutputSize.x, 			  	 0, 0.01),
						 			   Vector3( 0, 					 			 0, 0.01),
		    			 			   Vector3( 0, 				  TileOutputSize.y, 0.01),
		    			 			   Vector3( TileOutputSize.x, TileOutputSize.y, 0.01) ];
		    				 
		    			var offset_x   :int = localIndex % TileSets[i].SrcColumns;
		    				 
		    			var offset_y 	:int = TileSets[i].SrcRows - (Mathf.FloorToInt( localIndex / TileSets[i].SrcColumns ) + 
		    				 						System.Convert.ToByte( ( localIndex % TileSets[i].SrcRows ) != 0 ) );
		    		 
								
						var vt1 : Vector2 = Vector2((offset_x - System.Convert.ToByte(Flipped_X) ) * TileSets[i].ModuleWidth - eps.x,
		    		 				    ((offset_y + System.Convert.ToByte(Flipped_Y)) * TileSets[i].ModuleHeight ) - eps.y);
						var vt2 : Vector2 = Vector2((offset_x - System.Convert.ToByte(!Flipped_X) ) * TileSets[i].ModuleWidth - eps.x,
		    		 		  		    ((offset_y + System.Convert.ToByte(Flipped_Y)) * TileSets[i].ModuleHeight ) - eps.y);
						var vt3 : Vector2 = Vector2((offset_x - System.Convert.ToByte(!Flipped_X) ) * TileSets[i].ModuleWidth - eps.x,
		    		 		  		   ((offset_y + System.Convert.ToByte(!Flipped_Y)) * TileSets[i].ModuleHeight ) - eps.y);
						var vt4 : Vector2 = Vector2((offset_x - System.Convert.ToByte(Flipped_X) )  * TileSets[i].ModuleWidth - eps.x,
		    		 		           ((offset_y + System.Convert.ToByte(!Flipped_Y)) * TileSets[i].ModuleHeight ) - eps.y);
		     				 
		    			if ( Rotated )
		    			{
		    			 	var vtAux : Vector2 = vt3;
		    			 	vt3 = vt1;
		    			 	vt1 = vtAux;
		    			}
		     				 		           
		    		    m.uv = [ vt1, vt2, vt3, vt4];
		     				 
		     	/////////////////////////////////////////////////////////////////////////////////////////////////////
		     				 	
		    			m.triangles = [0, 1, 2, 0, 2, 3];
		    	
		    			m.RecalculateNormals();
		    			 
		    			m.Optimize();
		    				 
		    			AssetDatabase.CreateAsset(m, FolderPath + "Meshes/" + TileName + "_mesh.asset");
		    					
		    			AssetDatabase.SaveAssets();
				 	}
		    				 
			     			 
					meshFilter.sharedMesh = m;
			     			
					m.RecalculateBounds();
					
					
					if ( localIndex in TileSets[i].Collisions.Keys )							// Prefab Collision Setup
			 		{
			 			switch(	TileSets[i].Collisions[localIndex] )
			 			{
			 			case "Plane": 
			 					Tile.AddComponent("MeshCollider").sharedMesh = 
			 					AssetDatabase.LoadAssetAtPath( "Assets/Meshes/Planes/1_0 ColPlane.asset", typeof(Mesh) ) ;
								break;
								
			 			case "Slope_Left":
			 					Tile.AddComponent("MeshCollider").sharedMesh = 
			 					AssetDatabase.LoadAssetAtPath( "Assets/Meshes/Planes/SlopeLeft.asset", typeof(Mesh) ) ; 
			 				 	break;  
			 				 	
			 			case "Slope_Right": 
			 					Tile.AddComponent("MeshCollider").sharedMesh = 
			 					AssetDatabase.LoadAssetAtPath( "Assets/Meshes/Planes/SlopeRight.asset", typeof(Mesh) ) ;
			 					break;  	
			 								 			
			 			default: 																// same as case "Box":
			 			   		(Tile.AddComponent("BoxCollider") as BoxCollider).size = Vector3.one;
			 					
			 			}
			 		
//			 	  		AddCollision = true;
			 		}
				
					 	 		 	 
		// AND REPLACE IT WITH THE NEW PREFAB			 	 
		 			var newPrefab : Object = PrefabUtility.CreateEmptyPrefab( FolderPath + TileName + ".prefab" );
		  
		 			PrefabUtility.ReplacePrefab(Tile, newPrefab);						//  Creando prefab con seleccion
		 
		 			AssetDatabase.Refresh(); 											// Actualizar inmediatamente
		 
		 			DestroyImmediate(Tile);												// Eliminar y reemplazar objeto por Prefab
		 					
		 			TileClone  = PrefabUtility.InstantiatePrefab( 
					AssetDatabase.LoadAssetAtPath( FolderPath + TileName + ".prefab", typeof(GameObject) ) );
		 
//					Debug.Log( "Building new Prefab: " + TileName );
				}						 	 	 
				break;	
			}
		}
		return TileClone;
	}
} 


}			 

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


class cTileSet {
	 	 
 var SrcColumns 	: int 		= 1;
 var SrcRows 		: int 		= 1;
 var FirstGid 		: int 		= 0;
          
 var ModuleWidth 	: double 	= 1.0;
 var ModuleHeight	: double 	= 1.0;
 
 var SrcImgName		: String 	= "";											// Image Source data references
 var SrcImgPath		: String 	= "";
	 
 var mat			: Material;

// var CollisionList 	: List.<int> = new List.<int>();
 var Collisions		: Dictionary.<int, String> = new Dictionary.<int, String>();
	 
 function cTileSet( TileSet : XmlNode, FolderPath : String, FilePath : String )			// if ( TileSet.HasChildNodes ) {  var lTileSet : cTileSet = new cTileSet(); lTileSet.Load(
 {	
	 	
	for( var TileSetNode : XmlNode = TileSet.FirstChild; TileSetNode != null ;  TileSetNode = TileSetNode.NextSibling )
 	{
 		if ( TileSetNode.Name == "image" )
 		{
 			var TileInputWidth 	: int 	= System.Convert.ToByte( TileSet.Attributes["tilewidth"].Value); // Tile width inside bitmap file  ( 64 )
   			var TileInputHeight	: int 	= System.Convert.ToByte( TileSet.Attributes["tileheight"].Value);// Tile height inside bitmap file 
          
 			var SrcImgWidth		: int 	= System.Convert.ToInt16( TileSetNode.Attributes["width"].Value ); // File Resolution (512)
 			var SrcImgHeight	: int 	= System.Convert.ToInt16( TileSetNode.Attributes["height"].Value); 
	 
 			SrcColumns 					= SrcImgWidth / TileInputWidth;
   			SrcRows 					= SrcImgHeight / TileInputHeight;
     			
   			ModuleWidth 				= 1.0 / SrcColumns;
   			ModuleHeight				= 1.0 / SrcRows;
     	
   			FirstGid				 	= System.Convert.ToInt16( TileSet.Attributes["firstgid"].Value );
     	 
 			SrcImgName					= TileSet.Attributes["name"].Value;						// Image Source data references
 			SrcImgPath					= TileSetNode.Attributes["source"].Value ;
	 		
 		}
 		else if ( TileSetNode.Name == "tile" )
 		{
	 		
	 		
 			if (TileSetNode.FirstChild.FirstChild.Attributes["name"].Value == "Collision" )
 			{ 			
// 			CollisionList.Add(int.Parse( TileSetNode.Attributes["id"].Value)+1 )  ;
 			
 			Collisions.Add( int.Parse( TileSetNode.Attributes["id"].Value)+1, 
 								TileSetNode.FirstChild.FirstChild.Attributes["value"].Value ); 			
 			}
	 			
 		}	
	 		
 		// REBUILD AND HOLD MATERIAL		 Search The material inside that folder or create it if not exists and then hold it
		mat = AssetDatabase.LoadAssetAtPath( FolderPath + SrcImgName + "_Mat" + ".mat", typeof(Material) );
   
		if ( mat == null)																	// Check if default material exists . . 
   		{
   	 		var tex = AssetDatabase.LoadAssetAtPath( FilePath + SrcImgPath , typeof(Texture2D) );
    	 
   	 		if ( tex == null)
   	 		{
   	 			Debug.Log( SrcImgPath + " texture file not found, put it in the same Tiled map folder: " + FilePath);
// 	 			this.close;
   	 			return;
   	 		}
			else																			// and if not build and save it
   	 		{
   	 			tex.filterMode = FilterMode.Point;
   	 			tex.wrapMode = TextureWrapMode.Repeat;
	 			tex.anisoLevel = 0;
    	 
//   	 			mat =  new Material(Shader.Find("Unlit/Transparent Cutout")) ;
   	 			mat =  new Material(Shader.Find("Mobile/Particles/Alpha Blended")) ;
	 			mat.mainTexture = tex ;
		 
   	 			AssetDatabase.CreateAsset( mat, FolderPath + SrcImgName + "_Mat" + ".mat" );
   	 			AssetDatabase.SaveAssets();
    	 	
   	 			Debug.Log("Re-creating new material: " + SrcImgName + "_Mat"); 	 	
   	 		}
   	 	}	
 	}
 }	 	
 
 
}