// - Unlit
// - Scroll 3 layers /w Multiplicative op

Shader "Transparent/Scroll 3 Layers" { 

Properties { _MainTex ("Base layer (RGB)", 2D) = "white" {} 
			 _FarTex ("2nd layer (RGB)", 2D) = "black" {} 
			 _NearTex ("2nd layer (RGB)", 2D) = "black" {} 
			 _ScrollX ("Base layer Scroll speed X", Float) = 0.1 
			 _ScrollY ("Base layer Scroll speed Y", Float) = 0.0 
			 _Scroll2X ("2nd layer Scroll speed X", Float) = 0.5 
			 _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0.0 
			 _Scroll3X ("3nd layer Scroll speed X", Float) = 1.0 
			 _Scroll3Y ("3nd layer Scroll speed Y", Float) = 0.0 
			 _ColorIntensity ("Layer Multiplier", Float) = 0.5 }

SubShader{
    Tags {"Queue"="Background" "IgnoreProjector"="True" "RenderType"="Background"}
    LOD 100

    ZWrite Off
    Fog { Mode Off }
    Lighting Off

    CGPROGRAM
    #pragma target 3.0
    #pragma surface surf NoLighting

    half4 LightingNoLighting (SurfaceOutput s, half3 lightDir, half atten) {
       half4 c;
       c.rgb = s.Albedo;
       return c;
    }

    struct Input {
       float2 uv_MainTex;
       float2 uv_FarTex;
       float2 uv_NearTex;
    };

    sampler2D _MainTex;
    sampler2D _FarTex;
    sampler2D _NearTex;

    float _ScrollX;
    float _ScrollY;
    float _Scroll2X;
    float _Scroll2Y;
    float _Scroll3X;
    float _Scroll3Y;

    fixed _ColorIntensity;

    void surf (Input IN, inout SurfaceOutput o) {
       float2 mainTex_uv = IN.uv_MainTex;
       float2 farTex_uv = IN.uv_FarTex;
       float2 nearTex_uv = IN.uv_NearTex;

       mainTex_uv.x += _ScrollX * _Time.x;
       mainTex_uv.y += _ScrollY * _Time.x;

       farTex_uv.x += _Scroll2X * _Time.x;
       farTex_uv.y += _Scroll2Y * _Time.x;

       nearTex_uv.x += _Scroll3X * _Time.x;
       nearTex_uv.y += _Scroll3Y * _Time.x;

       float4 mainTex = tex2D (_MainTex, mainTex_uv);
       float4 farTex = tex2D (_FarTex, farTex_uv);
       float4 nearTex = tex2D (_NearTex, nearTex_uv);

       float3 previous = lerp(mainTex.rgb, farTex.rgb, farTex.a);
        o.Albedo = lerp(previous.rgb, nearTex.rgb, nearTex.a) * _ColorIntensity;
    }

    ENDCG
}
}

////////////////////////////////////////////////////////////////////////////////

// - Unlit
// - Scroll 3 layers /w Multiplicative op

//Shader "Transparent/Scroll 3 Layers" { 
//
//Properties { _MainTex ("Base layer (RGB)", 2D) = "white" {} 
//			 _FarTex ("2nd layer (RGB)", 2D) = "black" {} 
//			 _NearTex ("2nd layer (RGB)", 2D) = "black" {} 
//			 _ScrollX ("Base layer Scroll speed X", Float) = 0.1 
//			 _ScrollY ("Base layer Scroll speed Y", Float) = 0.0 
//			 _Scroll2X ("2nd layer Scroll speed X", Float) = 0.5 
//			 _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0.0 
//			 _Scroll3X ("3nd layer Scroll speed X", Float) = 1.0 
//			 _Scroll3Y ("3nd layer Scroll speed Y", Float) = 0.0 
//			 _ColorIntensity ("Layer Multiplier", Float) = 0.5 }
//
//SubShader{
//    Tags {"Queue"="Background" "IgnoreProjector"="True" "RenderType"="Background"}
//    LOD 100
//
//    ZWrite Off
//    Fog { Mode Off }
//    Lighting Off
//
//    CGPROGRAM
////    #pragma target 3.0
//    #pragma surface surf NoLighting
//    #include "UnityCG.cginc"
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
//}
//}