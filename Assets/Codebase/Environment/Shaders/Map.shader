Shader "Map/Map" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
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
		
      	void surf(Input IN, inout SurfaceOutput o) {
      		float3 light = IN.color.rgb;
      		float sun = IN.color.a;
      		float3 ambient = UNITY_LIGHTMODEL_AMBIENT * 2;
      		ambient = min(ambient, sun);
      		ambient = max(ambient, light);
        	o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * ambient;
     	}
		
		ENDCG
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 80
		
		Pass {
			Lighting Off
			
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
