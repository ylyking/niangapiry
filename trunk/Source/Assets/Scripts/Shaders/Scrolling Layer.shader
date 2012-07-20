Shader "Transparent/Scrolling Layer"  {
    Properties {
        _MainTex ("Base layer (RGB)", 2D) = "white" {}
        //_DetailTex ("2nd layer (RGB)", 2D) = "white" {}
        _ScrollX ("Base layer Scroll speed X", Float) = 1.0
        _ScrollY ("Base layer Scroll speed Y", Float) = 0.0
        //_Scroll2X ("2nd layer Scroll speed X", Float) = 1.0
        //_Scroll2Y ("2nd layer Scroll speed Y", Float) = 0.0
        _Intensity ("Intensity", Float) = 1.0
        _Alpha ("Alpha", Range(0.0, 1.0)) = 1.0
    }

    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Lighting Off 
        Fog { Mode Off }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        LOD 100


        CGINCLUDE
        #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        //sampler2D _DetailTex;

        float4 _MainTex_ST;
        //float4 _DetailTex_ST;

        float _ScrollX;
        float _ScrollY;
        //float _Scroll2X;
        //float _Scroll2Y;
        float _Intensity;
        float _Alpha;

        struct v2f {
           float4 pos : SV_POSITION;
           float2 uv : TEXCOORD0;
           //float2 uv2 : TEXCOORD1;
           fixed4 color : TEXCOORD1;     
        };


        v2f vert (appdata_full v)
        {
           v2f o;
           o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
           o.uv = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time);
           //o.uv2 = TRANSFORM_TEX(v.texcoord.xy,_DetailTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time);
           o.color = fixed4(_Intensity, _Intensity, _Intensity, _Alpha);

           return o;
        }
        ENDCG


        Pass {
           CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag
           #pragma fragmentoption ARB_precision_hint_fastest     
           fixed4 frag (v2f i) : COLOR
           {
             fixed4 o;
             fixed4 tex = tex2D (_MainTex, i.uv);
             //fixed4 tex2 = tex2D (_DetailTex, i.uv2);

             //o = (tex * tex2) * i.color;
             o = tex * i.color;

             return o;
           }
           ENDCG 
        }  
    }
}



////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////

//	 Properties { _MainTex ("Base layer (RGB)", 2D) = "white" {} 
// 				_FarTex ("2nd layer (RGB)", 2D) = "black" {} 
// 				_NearTex ("2nd layer (RGB)", 2D) = "black" {} 
// 				_ScrollX ("Base layer Scroll speed X", Float) = 0.1 
// 				_ScrollY ("Base layer Scroll speed Y", Float) = 0.0 
// 				_Scroll2X ("2nd layer Scroll speed X", Float) = 0.5 
// 				_Scroll2Y ("2nd layer Scroll speed Y", Float) = 0.0 
// 				_Scroll3X ("3nd layer Scroll speed X", Float) = 1.0 
// 				_Scroll3Y ("3nd layer Scroll speed Y", Float) = 0.0 
// 				_ColorIntensity ("Layer Multiplier", Float) = 0.5 }
//SubShader{
//    Tags {"Queue"="Background" "IgnoreProjector"="True" "RenderType"="Background"}
//    LOD 100
//
//    ZWrite Off
//    Lighting Off
//
//    CGPROGRAM
//    #pragma surface surf NoLighting
//
//    half4 LightingNoLighting (SurfaceOutput s, half3 lightDir, half atten) {
//       half4 c;
//       c.rgb = s.Albedo;
//       return c;
//    }
//
//    struct Input {
//       float2 uv_MainTex;
//       float2 uv_FarTex;
//       float2 uv_NearTex;
//    };
//
//    sampler2D _MainTex;
//    sampler2D _FarTex;
//    sampler2D _NearTex;
//
//    half _ScrollX;
//    half _ScrollY;
//    half _Scroll2X;
//    half _Scroll2Y;
//    half _Scroll3X;
//    half _Scroll3Y;
//
//    fixed _ColorIntensity;
//
//    void surf (Input IN, inout SurfaceOutput o) {
//       float2 mainTex_uv = IN.uv_MainTex;
//       float2 farTex_uv = IN.uv_FarTex;
//       float2 nearTex_uv = IN.uv_NearTex;
//
//       mainTex_uv.x += _ScrollX * _Time.x;
//       mainTex_uv.y += _ScrollY * _Time.x;
//
//       farTex_uv.x += _Scroll2X * _Time.x;
//       farTex_uv.y += _Scroll2Y * _Time.x;
//
//       nearTex_uv.x += _Scroll3X * _Time.x;
//       nearTex_uv.y += _Scroll3Y * _Time.x;
//
//       float4 mainTex = tex2D (_MainTex, mainTex_uv);
//       float4 farTex = tex2D (_FarTex, farTex_uv);
//       float4 nearTex = tex2D (_NearTex, nearTex_uv);
//
//       float3 previous = lerp(mainTex.rgb, farTex.rgb, farTex.a);
//        o.Albedo = lerp(previous.rgb, nearTex.rgb, nearTex.a) * _ColorIntensity;
//    }
//
//    ENDCG
//
//}
//}







////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////

//    Properties 
//    {
//  		_Offset ("Scroll Speed", Range (-0.99, 0.99)) = 0.99
//        _Cutoff ("Cutoff", Float) = .97
//        _KeyY ("Key Y", Float) = 0
//        _MainTex ("Key X texture (Alpha)", 2D) = ""
//        _Colors ("Colors", 2D) = ""
//    }
// 
//    SubShader 
//    {
//        Pass 
//        {
//            //GLSLPROGRAM
//            CGPROGRAM
//            //#ifdef VERTEX
//            //#endif
//            #pragma vertex vert 
//            //#ifdef FRAGMENT
//            //#endif
//            #pragma fragment frag
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #include "UnityCG.cginc"
//
//            
//
//
//           struct v2f
//                {
//                   float4  pos : SV_POSITION;
//                   float2  uv : TEXCOORD0;
//                   float3 color : COLOR;
//
//                }; 
//	
//	      float _Offset;
//            
//          //uniform mediump vec4 _MainTex_ST;
//          float4 _MainTex_ST;
//
//   			v2f vert (appdata_base v)
//   			{
//	    		v2f o;
//                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//	    		o.uv = float4(v.texcoord.x + _Offset, v.texcoord.y +  _Offset, 0, 1);
//	    		return o;
//	    		
//			}
//
//            sampler2D _MainTex, _Colors;
//            float _Cutoff, _KeyY;
//
//            
//            float4 frag(v2f i) : COLOR
//           {
//
//                float keyX = tex2D(_MainTex, i.uv).a;
//                clip(  ( keyX > _Cutoff ) * -1); 
//                                
//                return tex2D(_Colors, half2(keyX, _KeyY)) ;
//            
//            }        
//            //ENDGLSL
//
//            ENDCG
//        }
//    }
//}
