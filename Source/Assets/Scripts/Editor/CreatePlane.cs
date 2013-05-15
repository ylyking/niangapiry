// Look in for Create simple 2D . . .
using UnityEngine;
using UnityEditor;
using System.Collections;


public class CreatePlane : MonoBehaviour {


//static function CreateQuad()
//{
//	float size	= 1.0f;
//	GameObject plane = new GameObject();
//	plane.name = "Quad";
//	plane.transform.position = Vector3.zero;
//	 
//	MeshFilter meshFilter 	= plane.AddComponent(typeof(MeshFilter));
//    MeshRenderer meshRenderer = plane.AddComponent(typeof(MeshRenderer));
//	
//	Material mat = AssetDatabase.LoadAssetAtPath("Assets/World 1/Materials/" + "texture0" + ".mat", typeof(Material) );
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
//    Mesh m = AssetDatabase.LoadAssetAtPath("Assets/Meshes/" + plane.name + ".asset", typeof(Mesh) );
//  
//  	if ( m == null)										// Check if default Mesh exists and ifnot build and save it   
// 	{    
//    	m = new Mesh();     
//    	m.name = plane.name;
//    	m.vertices = [Vector3(size, -size, 0.01f), Vector3(-size, -size, 0.01f), Vector3(-size, size, 0.01f), Vector3(size, size, 0.01f) ];
//     	m.uv = [ Vector2(0, 0.625f), Vector2 (-.125, 0.625f), Vector2 (-.125, 0.75f), Vector2 (0, .75)];  // 1-'    2'-_   3¡-     4¬
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
    
[MenuItem("GameObject/Create Other/2D Quad %_q")]

static void  CreateQuad (){
	float size	= 0.5f;
	GameObject plane = new GameObject( "Quad");
	plane.transform.position = Vector3.zero;
	 
	MeshFilter meshFilter 	= plane.AddComponent<MeshFilter>();
    MeshRenderer meshRenderer = plane.AddComponent<MeshRenderer>();
	
	Material mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/" + "material_0" + ".mat", typeof(Material) );
   
   	if ( mat == null)									// Check if default material exists and ifnot build and save it
    {
	//  meshRenderer.sharedMaterial =  new Material(Shader.Find("Diffuse")) ;
    	mat =  new Material(Shader.Find("Diffuse")) ;
    	AssetDatabase.CreateAsset(mat, "Assets/Materials/" + "material_0" + ".mat");
    	AssetDatabase.SaveAssets();
    	print("Re-creating the same material");

	}
	
	meshRenderer.sharedMaterial = mat;

	 	 
    Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Meshes/" + plane.name + ".asset", typeof(Mesh) );
  
  	if ( m == null)										// Check if default Mesh exists and ifnot build and save it   
 	{    
    	m = new Mesh();     
    	m.name = plane.name;
    	m.vertices = new Vector3[4]	{
                                    new Vector3(size, -size, 0.01f),
                                    new  Vector3(-size, -size, 0.01f),
                                    new  Vector3(-size, size, 0.01f),
                                    new  Vector3(size, size, 0.01f) };

//    	m.vertices = [Vector3(size *2, size*2, -size), Vector3(0, 0, -size), Vector3(0, 0,  size ), Vector3(size * 2, size*2,  size ) ];

        //m.uv =  new Vector2[4]	{
        //                            new Vector2(.75f, 0.5f), 
        //                            new Vector2(0.5f, 0.5f),
        //                            new  Vector2 (0.5f, .75f),
        //                            new  Vector2 (.75f, .75f) };  // 1-'    2'-_   3¡-     4¬ // Maracuya Example

        m.uv = new Vector2[4]	{
                                    new Vector2(1, 0), 
                                    new Vector2(0, 0),
                                    new  Vector2 (0, 1),
                                    new  Vector2 (1, 1) };  // 1-'    2'-_   3¡-     4¬

    	m.triangles = new int[6] {0, 1, 2, 0, 2, 3};
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
//	 float size	= 1.0f;
//     Mesh m = AssetDatabase.LoadAssetAtPath("Assets/Meshes/Quad", typeof(Mesh));
//     
// if ( m == null)
// 	{    
//     m = new Mesh();     
//     m.name = "Quad";
//     m.vertices = [Vector3(-size, -size, 0.01f), Vector3(size, -size, 0.01f), Vector3(size, size, 0.01f), Vector3(-size, size, 0.01f) ];
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
//     GameObject obj = new GameObject("New Quad", MeshRenderer, MeshFilter, BoxCollider);
//     obj.GetComponent<MeshFilter>().mesh = m;
//     obj.transform.Rotate(Vector3.up, 180, Space.World);
//     
//     Selection.activeObject = obj;
//
//}
}