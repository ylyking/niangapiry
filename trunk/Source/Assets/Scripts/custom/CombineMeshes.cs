/// This script should be put on an empty GameObject
// Objects to be combined should be children of the empty GameObject

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class CombineMeshes : MonoBehaviour {

 
public bool  FixOptimized = false;			// Do a mesh optimization necesary for some meshes messed up!.. 

//public bool  SaveCombinedMesh = false;
//public string CombinedMeshName = "'New Mesh'"; // Unity saves the mesh Assets Combination



 
void  Start (){

    foreach(Transform child in transform)

        child.position += transform.position;
    transform.position = Vector3.zero;
    transform.rotation = Quaternion.identity;



    MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
    CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];
    int index= 0;

 

    for (int i= 0; i < meshFilters.Length; i++)
    {
        if (meshFilters[i].sharedMesh == null) continue;
        combine[index].mesh = meshFilters[i].sharedMesh;
        combine[index++].transform = meshFilters[i].transform.localToWorldMatrix;
        meshFilters[i].renderer.enabled = false;
    }

 

    GetComponent<MeshFilter>().mesh = new Mesh();
    GetComponent<MeshFilter>().mesh.CombineMeshes (combine, true, true);
    
    if ( FixOptimized ) GetComponent<MeshFilter>().mesh.Optimize();
    
    renderer.material = meshFilters[1].renderer.sharedMaterial;     // Si en algún momento Unity te manda por aqui 
                                                                //es xq Tienes alguna capa de Tiles VACIA  en Tiled 
}

void  OnApplicationQuit (){
//	if ( SaveCombinedMesh ) 					// insert a Name & Unity saves the mesh Assets Combination, dont use weird chars
//	//print("saving mesh here: " + CombinedMeshName);
//	{
//	   	AssetDatabase.CreateAsset(GetComponent<MeshFilter>().mesh, "Assets/Meshes/" + CombinedMeshName + ".asset");
//    	AssetDatabase.SaveAssets();
//    }
}
}