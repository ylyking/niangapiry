// Look in for Create simple 2D . . .

    @MenuItem("GameObject/Create Other/2D Quad %_q")
static function Awake()
{
	var size : float	= 1.0f;
	var plane : GameObject = new GameObject();
	plane.name = "Quad";
	plane.transform.position = Vector3.zero;
	 
	var meshFilter : MeshFilter = plane.AddComponent(typeof(MeshFilter));
    plane.AddComponent(typeof(MeshRenderer));
	 
    var m : Mesh = AssetDatabase.LoadAssetAtPath("Assets/Meshes/" + plane.name + ".asset", typeof(Mesh));
     
	if ( m == null)
 	{    
    	m = new Mesh();     
    	m.name = plane.name;
    	m.vertices = [Vector3(-size, -size, 0.01), Vector3(size, -size, 0.01), Vector3(size, size, 0.01), Vector3(-size, size, 0.01) ];
     	m.uv = [ Vector2(1, 0), Vector2 (0, 0), Vector2 (0, 1), Vector2 (1, 1)];
    	m.triangles = [0, 1, 2, 0, 2, 3];
    	m.RecalculateNormals();
     
     	AssetDatabase.CreateAsset(m, "Assets/Meshes/" + m.name + ".asset");
    	AssetDatabase.SaveAssets();
    }
     
    meshFilter.sharedMesh = m;
    m.RecalculateBounds();
     
    plane.AddComponent(typeof(BoxCollider));
	plane.transform.Rotate(Vector3.up, 180, Space.World);
     
    Selection.activeObject = plane;
}
    
//static function Awake()
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
//     }
//     
//     var obj : GameObject = new GameObject("New Quad", MeshRenderer, MeshFilter, BoxCollider);
//     obj.GetComponent(MeshFilter).mesh = m;
//     obj.transform.Rotate(Vector3.up, 180, Space.World);
//     
//     Selection.activeObject = obj;
//
//}