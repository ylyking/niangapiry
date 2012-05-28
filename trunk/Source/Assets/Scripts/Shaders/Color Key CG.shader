Shader "Color Keys Cg" 
{
    Properties 
    {
//        _Color ("Base Color", Color) = (1,1,1, .97)
        _Cutoff ("Cutoff", Float) = .97
        _KeyY ("Key Y", Float) = 0
        _MainTex ("Key X texture (Alpha)", 2D) = ""
        _Colors ("Colors", 2D) = ""
    }
 
    SubShader 
    {
        Pass 
        {
            //GLSLPROGRAM
            CGPROGRAM
            //#ifdef VERTEX
            //#endif
            #pragma vertex vert 
            //#ifdef FRAGMENT
            //#endif
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            //varying mediump vec2 uv;

           struct v2f
                {
                   float4  pos : SV_POSITION;
                   float2  uv : TEXCOORD0;
                }; 

            
          //uniform mediump vec4 _MainTex_ST;
          float4 _MainTex_ST;

           v2f vert (appdata_tan v)
           {
                v2f o;
                //gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                //uv = gl_MultiTexCoord0.xy + _MainTex_ST.xy + _MainTex_ST.zw;
                //o.uv = v.texcoord.xy;
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;

            }

            //uniform sampler2D _MainTex, _Colors;
            sampler2D _MainTex, _Colors;
            //uniform highp float _Cutoff, _KeyY;
            float _Cutoff, _KeyY;
//            float  _KeyY; float4 _Color;
            
            float4 frag(v2f i) : COLOR
           {

                //highp float keyX = texture2D(_MainTex, uv).a;
                float keyX = tex2D(_MainTex, i.uv).a;
                //if (keyX > _Cutoff) discard;
//                clip(  keyX > _Cutoff ? -1:1 );
                //Same results can be obtained with if (keyX > _Cutoff) clip (-1);
                clip(  ( keyX > _Cutoff ) * -1); 
//                clip(  ( keyX > _Color.a ) * -1); 
                                
                return tex2D(_Colors, half2(keyX, _KeyY)) ;
//                return tex2D(_Colors, half2(keyX, _KeyY)) * _Color;
            
            }        
            //ENDGLSL

            ENDCG
        }
    }
}