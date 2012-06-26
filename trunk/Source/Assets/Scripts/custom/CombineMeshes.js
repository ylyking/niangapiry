/// This script should be put on an empty GameObject
// Objects to be combined should be children of the empty GameObject

#pragma strict
#pragma downcast

 
public var FixOptimized 	: boolean = false;			// Do a mesh optimization necesary for some meshes messed up!.. 

//public var SaveCombinedMesh : boolean = false;
//public var CombinedMeshName : String = "'New Mesh'"; // Unity saves the mesh Assets Combination

@script RequireComponent(MeshFilter)
@script RequireComponent(MeshRenderer)

 
function Start () {

    for (var child : Transform in transform)

        child.position += transform.position;
    transform.position = Vector3.zero;
    transform.rotation = Quaternion.identity;

 

    var meshFilters = GetComponentsInChildren.<MeshFilter>();
    var combine : CombineInstance[] = new CombineInstance[meshFilters.Length-1];
    var index = 0;

 

    for (var i = 0; i < meshFilters.Length; i++)
    {
        if (meshFilters[i].sharedMesh == null) continue;
        combine[index].mesh = meshFilters[i].sharedMesh;
        combine[index++].transform = meshFilters[i].transform.localToWorldMatrix;
        meshFilters[i].renderer.enabled = false;
    }

 

    GetComponent(MeshFilter).mesh = new Mesh();
    GetComponent(MeshFilter).mesh.CombineMeshes (combine, true, true);
    
    if ( FixOptimized ) GetComponent(MeshFilter).mesh.Optimize();
    
    renderer.material = meshFilters[1].renderer.sharedMaterial;

}

function OnApplicationQuit()
{
//	if ( SaveCombinedMesh ) 					// insert a Name & Unity saves the mesh Assets Combination, dont use weird chars
//	//print("saving mesh here: " + CombinedMeshName);
//	{
//	   	AssetDatabase.CreateAsset(GetComponent(MeshFilter).mesh, "Assets/Meshes/" + CombinedMeshName + ".asset");
//    	AssetDatabase.SaveAssets();
//    }
}