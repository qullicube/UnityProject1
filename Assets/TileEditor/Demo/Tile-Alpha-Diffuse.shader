Shader "Custom/Tile" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200

CGPROGRAM
#pragma target 3.0
#pragma surface surf Lambert alpha

sampler2D _MainTex;
fixed4 _Color;
float _Step;

struct Input {
	float2 uv_MainTex;
};

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
}

Fallback "Transparent/Diffuse"
}
