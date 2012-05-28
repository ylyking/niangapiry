Shader "Color Keys" {
	
Properties {
	_Cutoff ("Cutoff", Float) = .97
	_KeyY ("Key Y", Float) = 0
	_MainTex ("Key X texture (Alpha)", 2D) = ""
	_Colors ("Colors", 2D) = ""
}

SubShader {Pass {
	GLSLPROGRAM
	varying mediump vec2 uv;
	
	#ifdef VERTEX
	uniform mediump vec4 _MainTex_ST;
	void main() {			
		gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
		uv = gl_MultiTexCoord0.xy + _MainTex_ST.xy + _MainTex_ST.zw;
	}
	#endif
	
	#ifdef FRAGMENT
	uniform sampler2D _MainTex, _Colors;
	uniform highp float _Cutoff, _KeyY;
	void main() {
		highp float keyX = texture2D(_MainTex, uv).a;
		if (keyX > _Cutoff) discard;
		
		gl_FragColor = texture2D(_Colors, vec2(keyX, _KeyY));
	}
	#endif		
	ENDGLSL
}}

}
// Use Exlusive for I-Phone or Android without direct X