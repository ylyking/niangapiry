#pragma strict
#pragma implicit

//necesitamos un array que guarde cinco lugares de espacio maximo, que funcione como inventario
//, empezando vacio y que cada vez que se recoge un objeto se agregue al array con un indice propio
//, teniendo la posibilidad de recorrer todo el array, siempre que haya algún objeto, con las teclas
//arriba y abajo: si hay objeto disponible ( sino es nulo) seleccionar y mostrarlo en la interface 
//sino volver a incrementar en uno el indice hasta llegar al fim/principio y seguir buscando hasta encontrar
//algún objeto sino no hay ninguno y el inventario está vacio entonces detener la busqueda y no mostrar nada..

var size 		: int = 5;	// == Inventory.Length
var ActualSize	: int = 0;	// range: 1 - 5 (0 == empty) 
 
var index 		: int = 0;	// range: 0 - 4 ( size -1)

//var	Inventory  :String[] = ["empty","empty","empty","empty","empty"];

var Inventory 	:String[] = new String[size] ;


function Start () 
{
	Inventory = ["empty","empty","empty","empty","empty"];

	while ( true )
		yield CoUpdate();

}

function CoUpdate ()	: IEnumerable
{
	if (Input.GetKeyUp("1")  )   ObjPickUp("Hat" );
	if (Input.GetKeyUp("2")  )   ObjPickUp("Smaller" );
	if (Input.GetKeyUp("3")  )   ObjPickUp("Whisttler" );
	if (Input.GetKeyUp("4")  )   ObjPickUp("Invisible" );
	if (Input.GetKeyUp("5")  )   ObjPickUp("Fire" );
	
	if (Input.GetKeyUp("0")  )   ObjClean( index );

	if (ActualSize > 1 )
	{
		if (Input.GetKeyUp("up")  )  SeekUpside( );

		if (Input.GetKeyUp("down") ) SeekDownside( );
	}
	
//	if (  ActualSize ) Debug.Log( Inventory[index] );	
	if ( Input.GetButtonUp("Fire1") ) Debug.Log( Inventory[index] );	
}




function ObjPickUp( Obj : String )
{

 if (ActualSize < size )
 {
	if ( Obj in Inventory )
	{
		print("Obj already picked");
		return;
	}
	
 	for ( var i : int = 0; i < size ; i++)
 	{
		if ( Inventory[ i ] == "empty"  )
		{
			Inventory[i] = Obj;
			ActualSize += 1;
			index = i;
			return;		
		}
		
		yield;
	}
	 
// 	if ( "empty" in Inventory )
// 	{
// 		Inventory[i] = Obj;
//		ActualSize += 1;
//		index = i;
//		return;	
// 	}
 
 }
}

function ObjClean( ObjIndex : int)
{
 	if ( ActualSize > 0  && (  Inventory[ ObjIndex ] != "empty"  ) )
	{
		Inventory[ ObjIndex ] = "empty";
		ActualSize -= 1;
		if ( ActualSize > 0 ) SeekDownside();
		return;
	}
}

//function ObjClean( Obj : String)
//{
// if ( ActualSize > 0 )
//	for ( luiIndex = 0; luiIndex < size ; luiIndex ++)
//	{
//		if ( Inventory[ luiIndex ] == Obj )
//		{
//			Inventory[ luiIndex ] = "empty"
//			ActualSize -= 1;
//			if ( ActualSize > 0 ) SeekDownside();
//			return;
//		}		
//	}
//}


function SeekUpside( )
{ 
	do
	{
 		index++;
  		index = index % size; 				// max index == size -1
		
  		yield;								// not needed in fact, just added for compatibility with coroutines
 	}
	while ( Inventory[ index ] == "empty" && ActualSize ) ;
}


function SeekDownside( )
{
	do
	{
 		index--;
		index +=  size * System.Convert.ToByte( index < 0 );
		
		yield;
	}
	while ( Inventory[ index ] == "empty" && ActualSize  ) ;
	
	//	index +=  size * ( index < 0  ? 1 : 0);
 	//	if ( index < 0) index = size -1; 	// min index == 0
 	//	if ( !ActualSize ) return;
}


///////////////////////////////////////////////////////////////////////////
//
//
//
//function SeekUpside( ) { 
//
// luiIndex = index ;
// 
// while( luiIndex < size )
// {
//
//	luiIndex++;
//
//	if ( Inventory[ luiIndex  ] != "empty" )
//	{
//		index = luiIndex;
//		return;
//	}
//	
//
// }
// 
// if ( ActualSize )
// {
//	Index = 0;
// 	SeekUpside();
// }
//}
//
//
//function SeekDownside( ) {
//
// luiIndex = index ;
// 
// while(luiIndex > 0)
// {
//
//	luiIndex--;
//
//	if ( Inventory[ luiIndex  ] != "empty" )
//	{
//		index = luiIndex;
//		return;
//	}
//	
// }
//
// if ( ActualSize )
// {
// 	index = size  -1;
// 	SeekDownside();
// }
//
//}


