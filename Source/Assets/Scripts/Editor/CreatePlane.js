// Look in for Create simple 2D . . .

    @MenuItem("GameObject/Create Other/2D Quad %_q")
//static function CreateQuad()
//{
//	var size : float	= 1.0f;
//	var plane : GameObject = new GameObject();
//	plane.name = "Quad";
//	plane.transform.position = Vector3.zero;
//	 
//	var meshFilter 	 : MeshFilter 	= plane.AddComponent(typeof(MeshFilter));
//    var meshRenderer : MeshRenderer = plane.AddComponent(typeof(MeshRenderer));
//	
//	var mat : Material = AssetDatabase.LoadAssetAtPath("Assets/World 1/Materials/" + "texture0" + ".mat", typeof(Material) );
//   
//   	if ( mat == null)									// Check if default material exists and ifnot build and save it
//    {
//	//  meshRenderer.sharedMaterial =  new Material(Shader.Find("Diffuse")) ;
//    	mat =  new Material(Shader.Find("Diffuse")) ;
//    	AssetDatabase.CreateAsset(mat, "Assets/Materials/" + "material_0" + ".mat");
//    	AssetDatabase.SaveAssets();
//    	print("Re-creating the same material");
//
//	}
//	
//	meshRenderer.sharedMaterial = mat;
//
//////////////////////////////////////////////////////////////////////////////////////////////	 	 
//    var m : Mesh = AssetDatabase.LoadAssetAtPath("Assets/Meshes/" + plane.name + ".asset", typeof(Mesh) );
//  
//  	if ( m == null)										// Check if default Mesh exists and ifnot build and save it   
// 	{    
//    	m = new Mesh();     
//    	m.name = plane.name;
//    	m.vertices = [Vector3(size, -size, 0.01), Vector3(-size, -size, 0.01), Vector3(-size, size, 0.01), Vector3(size, size, 0.01) ];
//     	m.uv = [ Vector2(0, 0.625), Vector2 (-.125, 0.625), Vector2 (-.125, 0.75), Vector2 (0, .75)];  // 1-'    2'-_   3¡-     4¬
//    	m.triangles = [0, 1, 2, 0, 2, 3];
//    	m.RecalculateNormals();
//    	m.Optimize();
//     
//     	AssetDatabase.CreateAsset(m, "Assets/Meshes/" + m.name + ".asset");
//    	AssetDatabase.SaveAssets();
//    	print("Re-creating the same mesh");
//    }
//     
//    meshFilter.sharedMesh = m;
//    m.RecalculateBounds();
//     
//    plane.AddComponent(typeof(BoxCollider));
////    plane.collider.
////	plane.transform.Rotate(Vector3.up, 180, Space.World);
//     
//    Selection.activeObject = plane;
//    
//}
    
static function CreateQuad()
{
	var size : float	= 0.5f;
	var plane : GameObject = new GameObject();
	plane.name = "Quad";
	plane.transform.position = Vector3.zero;
	 
	var meshFilter 	 : MeshFilter 	= plane.AddComponent(typeof(MeshFilter));
    var meshRenderer : MeshRenderer = plane.AddComponent(typeof(MeshRenderer));
	
	var mat : Material = AssetDatabase.LoadAssetAtPath("Assets/Materials/" + "material_0" + ".mat", typeof(Material) );
   
   	if ( mat == null)									// Check if default material exists and ifnot build and save it
    {
	//  meshRenderer.sharedMaterial =  new Material(Shader.Find("Diffuse")) ;
    	mat =  new Material(Shader.Find("Diffuse")) ;
    	AssetDatabase.CreateAsset(mat, "Assets/Materials/" + "material_0" + ".mat");
    	AssetDatabase.SaveAssets();
    	print("Re-creating the same material");

	}
	
	meshRenderer.sharedMaterial = mat;

	 	 
    var m : Mesh = AssetDatabase.LoadAssetAtPath("Assets/Meshes/" + plane.name + ".asset", typeof(Mesh) );
  
  	if ( m == null)										// Check if default Mesh exists and ifnot build and save it   
 	{    
    	m = new Mesh();     
    	m.name = plane.name;
    	m.vertices = [Vector3(size, -size, 0.01), Vector3(-size, -size, 0.01), Vector3(-size, size, 0.01), Vector3(size, size, 0.01) ];
//    	m.vertices = [Vector3(size *2, size*2, -size), Vector3(0, 0, -size), Vector3(0, 0,  size ), Vector3(size * 2, size*2,  size ) ];
     	m.uv = [ Vector2(1, 0), Vector2 (0, 0), Vector2 (0, 1), Vector2 (1, 1)];  // 1-'    2'-_   3¡-     4¬
    	m.triangles = [0, 1, 2, 0, 2, 3];
    	m.RecalculateNormals();
     
     	AssetDatabase.CreateAsset(m, "Assets/Meshes/" + m.name + ".asset");
    	AssetDatabase.SaveAssets();
    	print("Re-creating the same mesh");
    }
     
    meshFilter.sharedMesh = m;
    m.RecalculateBounds();
     
    plane.AddComponent(typeof(BoxCollider));
//	plane.transform.Rotate(Vector3.up, 180, Space.World);
     
    Selection.activeObject = plane;
    
}
   
   
   
   
 
   
   
   
   
   
   
   
   
//static function CreateQuad()
//{
//	 var size : float	= 1.0f;
//     var m : Mesh = AssetDatabase.LoadAssetAtPath("Assets/Meshes/Quad", typeof(Mesh));
//     
// if ( m == null)
// 	{    
//     m = new Mesh();     
//     m.name = "Quad";
//     m.vertices = [Vector3(-size, -size, 0.01), Vector3(size, -size, 0.01), Vector3(size, size, 0.01), Vector3(-size, size, 0.01) ];
//     m.uv = [ Vector2(1, 0), Vector2 (0, 0), Vector2 (0, 1), Vector2 (1, 1)];
//     m.triangles = [0, 1, 2, 0, 2, 3];
//     m.RecalculateNormals();
//     
////     AssetDatabase.CreateAsset(m, "Assets/Editor/" + planeAssetName);
//     AssetDatabase.CreateAsset(m, "Assets/Meshes/Quad");
//     AssetDatabase.SaveAssets();
//     print("Was here!");
//     }
//     
//     var obj : GameObject = new GameObject("New Quad", MeshRenderer, MeshFilter, BoxCollider);
//     obj.GetComponent(MeshFilter).mesh = m;
//     obj.transform.Rotate(Vector3.up, 180, Space.World);
//     
//     Selection.activeObject = obj;
//
//}