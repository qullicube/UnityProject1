Shader "Custom/Tile" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Brightness ("Wave Brightness", Float) = 1.0
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
CGINCLUDE
	struct Input {
			float2 uv_MainTex;
		};

		uniform sampler2D _MainTex;
		uniform fixed4 _Color;
		uniform float _Step;
		uniform float _Brightness;
		
		void surf (Input IN, inout SurfaceOutput o) {
			float2 v = IN.uv_MainTex - 0.5f;
			float r = length(v);
			v = normalize(v);

			//r + _Step*0.5f (in)
			//r - _Step*0.5f + 1.0f (out)

			float2 uv = (fmod(r - _Step *0.5f +1.0f, 0.5f)) * v + 0.5f; 

			fixed4 c = _Color + tex2D(_MainTex, uv) * _Color * _Brightness;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
ENDCG
SubShader {

	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	
	Blend SrcAlpha OneMinusSrcAlpha
	CGPROGRAM
	#pragma debug
	#pragma surface surf Lambert alpha
	ENDCG
		
}
Fallback "Transparent/Diffuse"
}
