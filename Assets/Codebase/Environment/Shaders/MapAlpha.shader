Shader "Map/Map Alpha" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="TransparentCutout" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
		CGPROGRAM
      	#pragma surface surf Lambert finalcolor:MyFinalColor noambient
		struct Input {
        	float2 uv_MainTex;
        	float4 color : COLOR;
      	};
		sampler2D _MainTex;
		
		void MyFinalColor(Input IN, SurfaceOutput o, inout fixed4 color) {
			color.rgb = o.Albedo.rgb;
			color.a = o.Alpha;
      	}
		
      	void surf (Input IN, inout SurfaceOutput o) {
      		float3 light = IN.color.rgb;
      		float sun = IN.color.a;
      		float3 ambient = UNITY_LIGHTMODEL_AMBIENT * 2;
      		ambient = min(ambient, sun);
      		ambient = max(ambient, light);
      		
      		fixed4 color = tex2D (_MainTex, IN.uv_MainTex);
      	
        	o.Albedo = color.rgb * ambient;
        	o.Alpha = color.a;
			clip(color.a-0.01f);
     	}
		
		ENDCG
	}
	
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 80
		
		Pass {
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			
			Material {
				Diffuse (1,1,1,1)
				Ambient (1,1,1,1)
			}
			
			BindChannels {
				Bind "Vertex", vertex
   				Bind "texcoord", texcoord
   				Bind "Color", color
			}
			
			SetTexture [_MainTex] {
				Combine texture * primary
			} 
		}
	}
	FallBack "Unlit/Texture"
}
