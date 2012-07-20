
@MenuItem("Utility / Test Script #_t") // "Ctrl + Shift + y"

static function TestScript() 
{ 
	var FlippedHorizontallyFlag : int = 0x80000000;
	var FlippedVerticallyFlag 	: int = 0x40000000;
	var FlippedDiagonallyFlag 	: int = 0x20000000;

	var TileId 		: uint = 3221225480;
	var Index 	  	: uint = TileId & ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag);

	if (TileId & FlippedHorizontallyFlag)			 	
	 print ( "_FlippedX") ;	
			 	
	if (TileId & FlippedVerticallyFlag)		
	 print ( "_FlippedY") ;
			 	 
	if (TileId & FlippedDiagonallyFlag)		
		 print ( "Rotated") ;
		 
//		var myString : String = "testing: " + TileId + " is " + Index;
		

		var myZippedString : String =  "H4sIAAAAAAAAC2NgYGDQBGItBgQwBGIjJL4lEFsh8Q0YGBoMoGwASRH6BUAAAAA=";
		

			 	 
//	var Samples : cTileSet = new cTileSet();
//	print ( Samples.SrcImgName + Samples.ModuleWidth );
			
}

