//Shader "My Shaders/Tinted Transparent with Opacity Control" {
//
//Properties
//{
//    _Color ("Tint Color (A = Opacity)", Color) = (1,1,1)
//    _MainTex ("Texture  (A = Opacity)", 2D) = ""
//}
//
//SubShader 
//{
//    Tags {Queue = Transparent}
//    ZWrite Off
//    Colormask RGB
//    Blend SrcAlpha OneMinusSrcAlpha   
//    Color [_Color]
//  
//      Pass
//    {       
//        SetTexture [_MainTex]
//        {
//           combine texture * primary
//        }      
//    }
//}
//
//
//}

Shader "Transparent/Color Key diffuse" {
Properties {
    _Cutoff ("Cutoff", Float) = .97
    _Color ("Main Color", Color) = (1,1,1)
    _KeyY ("Key Y", Float) = 0
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Colors ("Colors", 2D) = ""
}
  SubShader 
 {



        Pass 
        {
            CGPROGRAM
            #pragma vertex vert 
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
   
           struct v2f
                {
                   float4  pos : SV_POSITION;
                   float2  uv : TEXCOORD0;
                }; 

            
          float4 _MainTex_ST;

           v2f vert (appdata_tan v)
           {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;


            }

            sampler2D _MainTex, _Colors;
            float _Cutoff, _KeyY;
            float4 _Color;
            
            float4 frag(v2f i) : COLOR
           {
           		
                float keyX = tex2D(_MainTex, i.uv).a;
                clip(  ( keyX > _Cutoff ) * -1); 
// 				clip(  keyX > _Cutoff ? -1:1 );
  
                                            
                 return(tex2D(_Colors, half2(keyX, _KeyY)) * _Color);
// 					return tex2D(_Colors, half2(keyX, _KeyY)) ;

            }        
            //ENDGLSL
            
            ENDCG
        }
        
 Tags {"Queue" = "Transparent" "RenderType"="Transparent"}

    ZWrite Off
    Colormask RGB
    Blend SrcAlpha OneMinusDstAlpha
    
    Color [_Color]
    Pass
    {       
        SetTexture [_MainTex]
        {
            combine texture * primary
        }       
    }
}
        
 Fallback "Transparent/VertexLit"
}

//	SubShader 
//	{
//    	Tags {Queue = Transparent}
//    	ZWrite Off
//    	Colormask RGB
//    	Blend SrcAlpha OneMinusSrcAlpha   
//    	Color [_Color]
//  
//    	  Pass
//    	{       
//    	    SetTexture [_MainTex]
//    	    {
//    	       combine texture * primary
//    	    }      
//    	}
//	}
    
    
// SubShader
// {
//	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
//	LOD 200
//
//	CGPROGRAM
//	#pragma surface surf Lambert alpha
//
//	sampler2D _MainTex;
//	fixed4 _Color;
//  
//
//	struct Input
//	{
//		float2 uv_MainTex;
//	};
//
//	void surf (Input IN, inout SurfaceOutput o)
//	{
//		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
//		o.Albedo = c.rgb;
//		o.Alpha = c.a;
//	}
//	
//	ENDCG
// }

//Fallback "Transparent/VertexLit"
//}
//


//
//Shader "Transparent/Color Key diffuse" {
//Properties {
//	_Color ("Main Color", Color) = (1,1,1,1)
//	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
//}
//
//SubShader {
//	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
//	LOD 200
//
//CGPROGRAM
//#pragma surface surf Lambert alpha
//
//sampler2D _MainTex;
//fixed4 _Color;
//
//struct Input {
//	float2 uv_MainTex;
//};
//
//void surf (Input IN, inout SurfaceOutput o) {
//	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
//	o.Albedo = c.rgb;
//	o.Alpha = c.a;
//}
//ENDCG
//}
//
//Fallback "Transparent/VertexLit"
//}