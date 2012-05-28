// generar un prefab con todo lo seleccionado

@MenuItem("Utility / Make Prefab %#_c") // "Ctrl + Shift + c"

static function CreatePrefab() 
{ 

 var selectedObjects : GameObject[] = Selection.gameObjects; 		// capturar objetos seleccionados
 
 for( var selection : GameObject in selectedObjects) 				// por cada objeto en lista de seleccion...
 {
	var name : String= selection.name; 								// guardar Nombre		
	var localPath : String = "Assets/" + name + ".prefab";			// Crear path
 	
	if ( AssetDatabase.LoadAssetAtPath(localPath, GameObject) ) 	// existe el mismo Prefab ?
	{
		// reemplazar o conservar original ?
	 if( EditorUtility.DisplayDialog( "Prefab existente", "Quieres reemplazarlo","Ok","No") ) 	
	 		createNew( selection, localPath);
	 
	}
	else 
	{
	createNew( selection, localPath); 								// Sino Crear prefab			
    } 
 }
}

static function createNew( selection : GameObject, localPath : String)
{
 var prefab : Object = PrefabUtility.CreateEmptyPrefab(localPath);
  //EditorUtility.CreateEmptyPrefab(localPath);
 PrefabUtility.ReplacePrefab(selection, prefab);
// EditorUtility.ReplacePrefab(selection, prefab);								//  Creando prefab con seleccion
 
 AssetDatabase.Refresh(); 														// Actualizar inmediatamente
 
 DestroyImmediate(selection);													// Eliminar y reemplazar objeto por Prefab
 var clone: GameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
}