Shader "Custom/Billboard2" {
   Properties {
      [PerRendererData] _MainTex ("Texture Image", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
   }
   SubShader {
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

      Pass {   
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         // User-specified uniforms            
         uniform sampler2D _MainTex;
		 uniform fixed4 _Color;        
 
         struct vertexInput {
            float4 vertex : POSITION;
			float4 color    : COLOR;
            float4 tex : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
			float4 color    : COLOR;
            float4 tex : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.pos = mul(UNITY_MATRIX_P, 
              mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
              + float4(input.vertex.x, input.vertex.y, 0.0, 0.0));
 
            output.tex = input.tex;
			output.color = input.color * _Color;
		
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
			float4 c = tex2D(_MainTex, float2(input.tex.xy));
			c.rgb *= c.a;
			return c;  
         }
 
         ENDCG
      }
   }
}