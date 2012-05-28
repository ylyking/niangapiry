var blockPiece:GameObject[]; // fill this
var debugMat:Material;
var pieces: GameObject[ , ];

var width = 20;
var length = 20;

private var levelArray:int[] = [7,0,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                               0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,
                                0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,
                                0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
                                0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                                0,0,0,0,1,1,1,1,1,1,6,6,1,1,1,1,0,0,0,0,
                                0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                                0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
                                0,0,0,0,0,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,
                                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
function Awake() {
    generateTiles();
}

function generateTiles(){
pieces = new GameObject[width, length]; // creates the object array

for(z=0; z<length; z++){
  for(x=0; x<width; x++){

    var prefab: GameObject;

    // Set pefab to be inLibrary prefab based on level array
    prefab = blockPiece[levelArray[z*width + x]];

    // Add piece to stage
    pieces[x,z] = Instantiate (prefab, Vector3(x,(z*-1),1), Quaternion.Euler(0, 0,   0));
    //pieces[x, z].renderer.material = debugMat;

       }
    }
}
