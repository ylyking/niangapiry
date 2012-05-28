// Genera Carpetas por default en nuestro projecto

import System.IO; // Incluir codigo .NET microchot

//agregar un item de mnu
// generar carpeta y nombrarla desde el script

@MenuItem("Utility / Make Folders")
//@MenuItem("Utility/MakeFolders #&_g") // add shortcut

static function MakeFolder()
{
 GenerateFolders();
}

static function GenerateFolders()
{
// Agregar dirección de carpeta del proyecto
 var ProjectPath :String = Application.dataPath + "/";
 
 // Crear carpetas a voluntad
 Directory.CreateDirectory( ProjectPath + "Materials");
 Directory.CreateDirectory( ProjectPath + "Audio");
 Directory.CreateDirectory( ProjectPath + "Meshes");
 Directory.CreateDirectory( ProjectPath + "Fonts");
 Directory.CreateDirectory( ProjectPath + "Textures");
 Directory.CreateDirectory( ProjectPath + "Resources");
 Directory.CreateDirectory( ProjectPath + "Scripts");
 Directory.CreateDirectory( ProjectPath + "Shaders");
 Directory.CreateDirectory( ProjectPath + "Packages");
 Directory.CreateDirectory( ProjectPath + "Physics");
 
 AssetDatabase.Refresh(); // Actualizar inmediatamente
 }