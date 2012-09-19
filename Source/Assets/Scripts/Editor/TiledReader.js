//#pragma strict


import System.Collections.Generic;
import System.Xml;
import System.IO;
import System;
import System.Text;
import Ionic.Zlib;


class TiledReader extends EditorWindow {											// we need the same name of the script here
	
 public var TiledTMXFile 			: String;										// public var TiledXMLFile 	: TextAsset;
 static public	var FolderPath  	: String;

 private var FlippedHorizontallyFlag: int = 0x80000000;
 private var FlippedVerticallyFlag 	: int = 0x40000000;
 private var FlippedDiagonallyFlag 	: int = 0x20000000;

 public var TileOutputSize			: Vector3 = new Vector3(1.0,1.0,0.0);			// Tile Poligonal Modulation inside Unity(Plane)
 public var eps 					: Vector2 = new Vector2(0.000005f, 0.000005f);	// epsilon to fix some Texture bleeding
 public var PrefabRebuild			: boolean = false;								// boolean to check mesh and prefabs RE-BUILD

 var TileSets 						: List.<cTileSet> = new List.<cTileSet>();
 public var MapTransform			: Transform;
	
 @MenuItem("Utility / Tiled Reader %_t")
 static function Init ()  
 {
 	var source : String = AssetDatabase.GetAssetPath( Selection.activeObject );

	if (( !source ) ||
	( (  source.Remove(0, source.IndexOf(".")) != ".tmx" ) && ( source.Remove(0, source.IndexOf(".")) != ".xml" ) ) )		
	{
		EditorUtility.DisplayDialog("Select one Tiled File", "You Must Choose a .TMX Tiled file first!", "Ok");
		return;
	}
	var window : TiledReader = EditorWindow.GetWindowWithRect(  typeof(TiledReader), new Rect(0, 0, 320, 160));
	
	source = source.Remove(0, 6);
	window.TiledTMXFile = source;
 }
	
 function OnGUI ()
 {		
	if(TiledTMXFile == null ) return;
		EditorGUILayout.BeginVertical ("box");
		
	EditorGUILayout.Separator();
	this.TileOutputSize = EditorGUILayout.Vector3Field(" Tile Output size:", this.TileOutputSize);
	EditorGUILayout.Separator();
	this.eps 	= EditorGUILayout.Vector2Field(" Offset Compensation: 				(Facu es un putito) ", this.eps);
//	EditorGUILayout.HelpBox("re-scale texture size inside each tile to fix bleed factor", MessageType.Info);
	EditorGUILayout.Separator();
	this.PrefabRebuild  = EditorGUILayout.Toggle(" Re-Build Assets: ", this.PrefabRebuild);
	EditorGUILayout.Separator();
		
	if(GUILayout.Button(" A COMEEERLA! ")) ReadTiled();
		EditorGUILayout.EndVertical();
 }
 
 function ReadTiled()
{
   var sr = new StreamReader( Application.dataPath  + TiledTMXFile  );						// some String Games

   var FileLoad : String = "Assets"  + TiledTMXFile;										// Assets/etc/File.tmx
   var FilePath	: String = FileLoad.Remove( FileLoad.LastIndexOf("/") + 1);	  			// Assets/etc/
   var FileName	: String = FileLoad.Remove( 0, FileLoad.LastIndexOf("/") + 1);				// quit folder path structure
		FileName = FileName.Remove( FileName.LastIndexOf("."));								// quit .xml extension
		
   		FolderPath = FilePath + FileName + " Prefabs/"; 							// Assets/Resources/FileName_Prefabs/
   var FileContent : String = sr.ReadToEnd();
   sr.Close();
 ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                   
   var Doc : XmlDocument = new XmlDocument();
   Doc.LoadXml(FileContent);
 

// CHECK IT S A TMX FILE FROM TILED	
 if ( Doc && (Doc.DocumentElement.Name == "map") )											// Access root Map		
 {
   
// REBUILD TMX OBJECT IN UNITY SCENE						
	DestroyImmediate( GameObject.Find( FileName ) ); 										// ReBuild the Layers Container & stuff.
	var map : GameObject = new GameObject( FileName );										// inside the editor hierarchy.
	MapTransform = map.transform;															// take map transform cached
	(map.AddComponent("LevelAttributes")as LevelAttributes).bounds = Rect( 	0, 0,			// Set Level bounds for camera 
			int.Parse( Doc.DocumentElement.Attributes["width"].Value ) * TileOutputSize.x,
		 	int.Parse( Doc.DocumentElement.Attributes["height"].Value )* TileOutputSize.y );
	//	map.AddComponent("CombineMeshes");
	
	if ( PrefabRebuild && AssetDatabase.DeleteAsset( FolderPath.Remove(FolderPath.LastIndexOf("/")) )  )
		Debug.Log(" Folder deleted & Rebuilded");
	
	Directory.CreateDirectory( FolderPath );												// build a Folder to hold everything
 	Directory.CreateDirectory( FolderPath + "Meshes/" );									// build a Folder to hold Meshes too
                         
// SEEK BITMAP SOURCE FILE	 
    for ( var TileSetInfo	: XmlNode in Doc.GetElementsByTagName("tileset") )				// array of the level nodes.
    {
   		var TileSetRef =  new cTileSet( TileSetInfo, FilePath) ;
   		TileSets.Add( TileSetRef );
   	}
// CREATE LAYERS . . .	    
	for ( var i : int = Doc.GetElementsByTagName("layer").Count-1; i >= 0 ; i-- )
	{			
		BuildLayer( Doc.GetElementsByTagName("layer").Item(i) );
	}	// end of each Layer Info 

//	INSTANTIATE PREFABS OBJECTS 
	for (  var ObjectsGroup : XmlNode in Doc.GetElementsByTagName("objectgroup") )
	{
		BuildPrefabs( ObjectsGroup );
	}
	
	TileOutputSize.z = 0.0;
	
 } else Debug.LogError( FileName + " it's not a Tiled File!, wrong load at: " + FilePath );
 
 Debug.Log("Tiled Level Build Finished"); 	 	
 
 this.Close();
}	// End of Method loader
     
 function BuildLayer( LayerInfo : XmlNode )
{
		var Layer : GameObject = new GameObject( LayerInfo.Attributes["name"].Value ); // add Layer Childs inside hierarchy.
		Layer.AddComponent("CombineMeshes");
			
		var LayerTransform			= Layer.transform;
		LayerTransform.position.z 	= TileOutputSize.z;
		LayerTransform.parent 		= MapTransform;
	 		
		var ColIndex	 	: int 	= 0.0;
		var RowIndex		: int 	= int.Parse( LayerInfo.Attributes["height"].Value ) -1;
		var CollisionLayer	: boolean = false;
			
		var Data 			: XmlElement = LayerInfo.FirstChild;
		while ( Data.Name != "data"  )	//		if ( Data.Name == "properties" )    // if Layer data has properties get them
		{
			var LayerProp : XmlElement = Data.FirstChild;
				
			if ( LayerProp.GetAttribute("name") == "Collision" )
				CollisionLayer = true;
				
			if ( LayerProp.GetAttribute("name") == "Depth" ){
				LayerTransform.position.z = float.Parse( LayerProp.GetAttribute("value"));
				TileOutputSize.z  = LayerTransform.position.z; }
						
			Data = Data.NextSibling;			
		}
		
 		TileOutputSize.z 		   += 0.5f;
		
			
// & CHECK IF DATA IS GZIP COMPRESSED OR DEFAULT XML AND CREATE OR BUILD ALL TILES INSIDE EACH LAYER			
		if ( Data.HasAttribute("compression") && Data.Attributes["compression"].Value == "gzip" )
		{	
			// Decode Base64 and then Uncompress Gzip Tile Information
			var decodedBytes : byte[] = Ionic.Zlib.GZipStream.UncompressBuffer( Convert.FromBase64String( Data.InnerText ) );			
			for ( var tile_index : int = 0; tile_index < decodedBytes.Length; tile_index += 4 )
			{
				var global_tile_id : uint = decodedBytes[tile_index] 			| decodedBytes[tile_index + 1] << 8  |
                              				decodedBytes[tile_index + 2] << 16 	| decodedBytes[tile_index + 3] << 24;
               
               	var TileRef = BuildTile( global_tile_id );              	
				if (TileRef != null)
    			{
					TileRef.transform.position = Vector3( ColIndex * TileOutputSize.x,
														  RowIndex * TileOutputSize.y,
														  LayerTransform.position.z );
			 												
			 		TileRef.transform.parent = LayerTransform;
			 	
			 		if ( CollisionLayer) (TileRef.AddComponent("BoxCollider") as BoxCollider).size = Vector3.one;                  
		 		}
			 	
				ColIndex++;
				RowIndex -= System.Convert.ToByte( ColIndex >= int.Parse(  LayerInfo.Attributes["width"].Value ) ); 
				ColIndex = ColIndex % int.Parse(  LayerInfo.Attributes["width"].Value ) ;      // ColIndex % TotalColumns           			
			}//end of each Tile GZIP Compression Info 
		
		}			
		else if ( Data.HasChildNodes ) 								// Else if not a Gzip Compression then try as XML data
		{		
	    	for (  var TileInfo : XmlNode in Data.GetElementsByTagName("tile"))
	  		{
				var TileRefXml = BuildTile( System.Convert.ToUInt32( TileInfo.Attributes["gid"].Value ));
				if (TileRefXml != null)
    			{
					TileRefXml.transform.position = Vector3( ColIndex * TileOutputSize.x,
														  	 RowIndex * TileOutputSize.y,
														     LayerTransform.position.z );
			 												
				 	TileRefXml.transform.parent = LayerTransform;
			 	
				 	if ( CollisionLayer) ( TileRefXml.AddComponent("BoxCollider") as BoxCollider).size = Vector3.one;                  
		 		}
			 	
				ColIndex++;
				RowIndex -= System.Convert.ToByte( ColIndex >= int.Parse(  LayerInfo.Attributes["width"].Value ) ); 
				ColIndex = ColIndex % int.Parse(  LayerInfo.Attributes["width"].Value)  ;  
		
			}//end of each Tile XML Info 
		}
	 else Debug.LogError(" Format Error: Save Tiled File in XML style or Compressed mode(Gzip + Base64)");		
}		

 function BuildTile( TileId : uint) : GameObject
{
	var Flipped_X 		: boolean = false;
	var Flipped_Y 		: boolean = false;
	var Rotated 		: boolean = false;
	var AddCollision	: boolean = false;

	if ( TileId )    //	 if ( FirstGid => TileId && TileId <= TotalTiles)	// Si es mayor que 0!				 	 
	{				
		var Index 	  	: uint = TileId & ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag);
		var TileName 	: String = "Tile_" + Index   ;
		var TileClone	: GameObject ;
		
		 	 
		if (TileId & FlippedHorizontallyFlag)			 	
			{	Flipped_X = true; TileName += "_H";	}
			 	
		if (TileId & FlippedVerticallyFlag)		
			{	Flipped_Y = true; TileName += "_V";	}
			 	 
		if (TileId & FlippedDiagonallyFlag)		
			{	Rotated = true; TileName += "_VH";	}
					 	 			 	 					 	 			 	 
		for ( var i : int = TileSets.Count -1; i >= 0; i--)		// Recorrer en reversa la lista y checar el TileSet firstGid
		{
			if ( Index >= TileSets[i].FirstGid )
		 	{			 	 	
		 		
				var localIndex : int = (Index - TileSets[i].FirstGid) + 1;
				 	 				 	 
//			 	if ( localIndex in TileSets[i].CollisionList ) AddCollision = true;
	
// IF EXISTS SOME PREFAB WITH THE SAME ID THEN USE IT, ELSE CREATE THE OBJECT AND SAVE IN ONE PREFAB 	 	 
				if ( AssetDatabase.LoadAssetAtPath( FolderPath + TileName + ".prefab", typeof(GameObject) )  )
				{
					TileClone  = PrefabUtility.InstantiatePrefab( 
				 		AssetDatabase.LoadAssetAtPath( FolderPath + TileName + ".prefab", typeof(GameObject) ) 	);			 	 	
				}else
				{			 	 	
					var Tile : GameObject = new GameObject();
	
					Tile.name = TileName;
					Tile.transform.position = Vector3.zero;
						 	 
					var meshFilter 	 : MeshFilter 	= Tile.AddComponent(typeof(MeshFilter));
			   		var meshRenderer : MeshRenderer = Tile.AddComponent(typeof(MeshRenderer));				   			 
		    		meshRenderer.sharedMaterial = TileSets[i].mat;
			    		 	 
					var m : Mesh =  AssetDatabase.LoadAssetAtPath( FolderPath + "Meshes/" + TileName + "_mesh.asset", typeof(Mesh) );
							
					if ( m == null )
					{
		   				m  = new Mesh();   
		    			m.name = TileName + "_mesh" ;
		   				m.vertices = [ Vector3( TileOutputSize.x, 			  	 0, 0.01),
						 			   Vector3( 0, 					 			 0, 0.01),
		    			 			   Vector3( 0, 				  TileOutputSize.y, 0.01),
		    			 			   Vector3( TileOutputSize.x, TileOutputSize.y, 0.01) ];
//		   				m.vertices = [ Vector3( TileOutputSize.x *.5f,-TileOutputSize.y *.5f, 0.01),
//						 			   Vector3( -TileOutputSize.x*.5f,-TileOutputSize.y *.5f, 0.01),
//		    			 			   Vector3( -TileOutputSize.x*.5f, TileOutputSize.y *.5f, 0.01),
//		    			 			   Vector3( TileOutputSize.x *.5f, TileOutputSize.y *.5f, 0.01) ];
		    				 
		    			var offset_x   :int = localIndex % TileSets[i].SrcColumns;	 
		    			var offset_y 	:int = TileSets[i].SrcRows - (Mathf.FloorToInt( localIndex / TileSets[i].SrcColumns ) + 
		    				 						System.Convert.ToByte( ( localIndex % TileSets[i].SrcRows ) != 0 ) );
		    		 
								
						var vt1 : Vector2 = Vector2((offset_x - System.Convert.ToByte(Flipped_X) ) * TileSets[i].ModuleWidth,
		    		 				    ((offset_y + System.Convert.ToByte(Flipped_Y)) * TileSets[i].ModuleHeight ));
						var vt2 : Vector2 = Vector2((offset_x - System.Convert.ToByte(!Flipped_X) ) * TileSets[i].ModuleWidth,
		    		 		  		    ((offset_y + System.Convert.ToByte(Flipped_Y)) * TileSets[i].ModuleHeight ));
						var vt3 : Vector2 = Vector2((offset_x - System.Convert.ToByte(!Flipped_X) ) * TileSets[i].ModuleWidth,
		    		 		  		   ((offset_y + System.Convert.ToByte(!Flipped_Y)) * TileSets[i].ModuleHeight ));
						var vt4 : Vector2 = Vector2((offset_x - System.Convert.ToByte(Flipped_X) )  * TileSets[i].ModuleWidth,
		    		 		           ((offset_y + System.Convert.ToByte(!Flipped_Y)) * TileSets[i].ModuleHeight ));

			    		var Xsign : uint = 1;
			    		var Ysign : uint = 1;
			    		
			    		if ( Flipped_X ) Xsign = -1;
			    		if ( Flipped_Y ) Ysign = -1;
			    					    					    		
			    		vt1.x += -eps.x * Xsign; 	vt1.y +=  eps.y * Ysign;
         				vt2.x +=  eps.x * Xsign ; 	vt2.y +=  eps.y * Ysign;
         				vt3.x +=  eps.x * Xsign ; 	vt3.y += -eps.y * Ysign;
         				vt4.x += -eps.x * Xsign; 	vt4.y += -eps.y * Ysign;	

		    			if ( Rotated )
		    			{
		    				if (Flipped_X && Flipped_Y || !Flipped_X && !Flipped_Y){
		    					var vtAux1 : Vector2 = vt2;
		    			 		vt2 = vt4;
		    			 		vt4 = vtAux1;
		    				}
		    				else {
		    			 		var vtAux2 : Vector2 = vt3;
		    			 		vt3 = vt1;
		    			 		vt1 = vtAux2;
		    			 	}
		    			}
		    			

		     				 		           
		    		    m.uv = [ vt1, vt2, vt3, vt4]; 		 	
		    			m.triangles = [0, 1, 2, 0, 2, 3];		    	
		    			m.RecalculateNormals(); 
		    			m.Optimize();
		    				 
		    			AssetDatabase.CreateAsset(m, FolderPath + "Meshes/" + TileName + "_mesh.asset");					
		    			AssetDatabase.SaveAssets();
				 	}
		     	/////////////////////////////////////////////////////////////////////////////////////////////////////
		    				 
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
			 			case "Slope":
			 						Tile.layer = 8; 
			 					if (Flipped_X && !Flipped_Y || !Flipped_X && Flipped_Y && Rotated || !Flipped_X && Flipped_Y && !Rotated)
			 						Tile.AddComponent("MeshCollider").sharedMesh = 
			 						AssetDatabase.LoadAssetAtPath( "Assets/Meshes/Planes/SlopeLeft.asset", typeof(Mesh) ) ;
			 					else 
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

 function BuildPrefabs( ObjectsGroup : XmlNode ) 
 {
    var	height 		: int = int.Parse( ObjectsGroup.ParentNode.Attributes["height"].Value );
 	var tilewidth 	: int = int.Parse( ObjectsGroup.ParentNode.Attributes["tilewidth"].Value );
 	var tileheight 	: int = int.Parse( ObjectsGroup.ParentNode.Attributes["tileheight"].Value );	
 	var ObjGroup	: GameObject = new GameObject( ObjectsGroup.Attributes["name"].Value );
 	var GrpTransform = ObjGroup.transform ;
 	GrpTransform.parent = MapTransform;
 	
	for ( var ObjInfo : XmlNode in  ObjectsGroup.ChildNodes)
	{
//		Debug.Log(ObjInfo.Attributes["name"].Value);
	 if ( !ObjInfo.Attributes["name"] ) continue;

	 if ( (AssetDatabase.LoadAssetAtPath( "Assets/Prefabs/" + ObjInfo.Attributes["name"].Value + ".prefab", typeof(GameObject)))  )
		{
			var ObjPrefab : GameObject = PrefabUtility.InstantiatePrefab( AssetDatabase.LoadAssetAtPath( 
										"Assets/Prefabs/" + ObjInfo.Attributes["name"].Value + ".prefab", typeof(GameObject) ) );
				
			var ObjTransform : Transform = ObjPrefab.transform ;
				
			ObjTransform.position = Vector3(
			 (float.Parse( ObjInfo.Attributes["x"].Value) / tilewidth) + (ObjTransform.localScale.x * .5 ),			// X
			 height -(float.Parse( ObjInfo.Attributes["y"].Value) / tileheight - ObjTransform.localScale.y * .5),	// Y		 		     
																					 MapTransform.position.z );		// Z
				 
				 ObjTransform.parent = GrpTransform ;
		} else Debug.LogWarning( "Object '" + ObjInfo.Attributes["name"].Value + "' Was not found at: " + "Assets/Prefabs/" );		
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

 var Collisions		: Dictionary.< int, String > = new Dictionary.< int, String >();
// var Prefabs		: Dictionary.< int, String > = new Dictionary.< int, String >();
	 
 function cTileSet( TileSet : XmlNode, FilePath : String )			// if ( TileSet.HasChildNodes ) {  var lTileSet : cTileSet = new cTileSet(); lTileSet.Load(
 {	
	 	
	for( var TileSetNode : XmlNode in TileSet  )
 	{
 		if ( TileSetNode.Name == "image" )
 		{
 			var TileInputWidth 	: int 	= System.Convert.ToInt32( TileSet.Attributes["tilewidth"].Value); // Tile width inside bitmap file  ( 64 )
   			var TileInputHeight	: int 	= System.Convert.ToInt32( TileSet.Attributes["tileheight"].Value);// Tile height inside bitmap file 
          
 			var SrcImgWidth		: int 	= System.Convert.ToInt32( TileSetNode.Attributes["width"].Value ); // File Resolution (512)
 			var SrcImgHeight	: int 	= System.Convert.ToInt32( TileSetNode.Attributes["height"].Value); 
	 
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
 			if ( TileSetNode.FirstChild.FirstChild.Attributes["name"].Value == "Collision" )
 					Collisions.Add( int.Parse( 	TileSetNode.Attributes["id"].Value)+1, 
 										  		TileSetNode.FirstChild.FirstChild.Attributes["value"].Value ); 	
 		}	
	 		
 		// REBUILD AND HOLD MATERIAL		 Search The material inside that folder or create it if not exists and then hold it
		mat = AssetDatabase.LoadAssetAtPath( TiledReader.FolderPath + SrcImgName + "_Mat" + ".mat", typeof(Material) );
		if ( mat == null)																	// Check if default material exists . . 
   		{
   		
   	 		var tex = AssetDatabase.LoadAssetAtPath( FilePath + SrcImgPath , typeof(Texture2D) );   	 
   	 		if ( tex == null)
   	 		{
   	 			Debug.LogError( SrcImgPath + " texture file not found, put it in the same Tiled map folder: " + FilePath);
   	 			return;   //	 this.close; 
   	 		}
			else																			// and if not build and save it
   	 		{
   	 			tex.filterMode = FilterMode.Point;
   	 			tex.wrapMode = TextureWrapMode.Repeat;
	 			tex.anisoLevel = 0;
    	 
//   	 		mat =  new Material(Shader.Find("Unlit/Transparent Cutout")) ;
   	 			mat =  new Material(Shader.Find("Mobile/Particles/Alpha Blended")) ;
	 			mat.mainTexture = tex ;
		 
   	 			AssetDatabase.CreateAsset( mat, TiledReader.FolderPath + SrcImgName + "_Mat" + ".mat" );
   	 			AssetDatabase.SaveAssets();
    	 	
   	 			Debug.Log("Re-creating new material: " + SrcImgName + "_Mat"); 	 	
   	 		}
   	 	}	
 	}
 }	 	
 
 
}

