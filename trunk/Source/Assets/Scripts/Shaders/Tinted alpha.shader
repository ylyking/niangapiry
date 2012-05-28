Shader "Tinted using alpha" { 

Properties 
{ 
   _Color ("Tint Color", Color) = (1,1,1) 
   _MainTex ("Texture", 2D) = "white" 
} 

SubShader 
{   
    Pass
    {
        SetTexture [_MainTex] 
        {
            ConstantColor [_Color]
            combine texture * constant 
        } 
 
        SetTexture [_MainTex] 
        {
            combine texture lerp(texture) previous
        }
    }
} 

 

}

//Shader "Tinted using Alpha" { 
//
// 
//
//Properties 
//
//{ 
//
//    _Color ("Tint Color", Color) = (1,1,1) 
//
//    _MainTex ("Texture", 2D) = "white" 
//
//} 
//
// 
//
//SubShader
//
//{ 
//
//    Pass 
//
//    {       
//
//        SetTexture [_MainTex] 
//
//        { 
//
//            ConstantColor (0,0,0,0) 
//
//            combine texture * one - texture + constant 
//
//        } 
//
//        
//
//        SetTexture [_Color] 
//
//        { 
//
//            ConstantColor [_Color] 
//
//            combine constant * previous 
//
//        }
//
//        
//
//        SetTexture [_MainTex] 
//
//        { 
//
//            combine texture * texture + previous 
//
//        }
//
//    } 
//
//}
//
// 
//
//}