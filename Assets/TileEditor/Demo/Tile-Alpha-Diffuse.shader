Shader "Custom/Tile" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
CGINCLUDE
	struct Input {
			float2 uv_MainTex;
		};

		uniform sampler2D _MainTex;
		uniform fixed4 _Color;
		uniform float _Step;
		
		void surf (Input IN, inout SurfaceOutput o) {
			float2 uv = IN.uv_MainTex;
			float bound = 0.0f;
			
			if(uv.x < 0.5f - bound && uv.y < 0.5f - bound)
			{
				uv.x -= _Step;
				uv.y -= _Step;
			}
			if(uv.x < 0.5f - bound && uv.y > 0.5f + bound)
			{
				uv.x -= _Step;
				uv.y += _Step;
			}
			if(uv.x > 0.5f + bound && uv.y < 0.5f - bound)
			{
				uv.x += _Step;
				uv.y -= _Step;
			}
			if(uv.x > 0.5f + bound && uv.y > 0.5f + bound)
			{
				uv.x += _Step;
				uv.y += _Step;
			}
			
			
			fixed4 c = tex2D(_MainTex, uv) * _Color;
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
