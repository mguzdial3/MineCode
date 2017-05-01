Shader "Map/Water" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Alpha ("Alpha", float) = 0.5
		
		_Speed ("Speed wave", float) = 1
		_Scale ("Scale wave", float) = 1
		
		_Time ("Time", float) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off 
		ZWrite Off
		LOD 300
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float _Alpha;
		
			float _Speed;
			float _Scale;
		
			float _Time;
		
			struct appdata {
    			float4 vertex : POSITION;
    			float2 texcoord : TEXCOORD0;
			};
		
			struct v2f {
    			float4 pos : SV_POSITION;
    			float4 color : COLOR;
    			float2 uv : TEXCOORD0;
			};
		
			v2f vert (appdata v) {
				float4 pos = mul( UNITY_MATRIX_MVP, v.vertex );
				float sx = sin(pos.x + _Time * _Speed);
				float sz = sin(pos.z + _Time * _Speed);
        		pos.y += (sx + sz) * _Scale*0.5f;
        	
        		float2 uv = v.texcoord;
        		uv.y += _Time;
        	
        		v2f o;
    			o.pos = pos;
    			o.uv = uv;
    			return o;
      		}
		
			fixed4 frag( v2f i ) : COLOR {
				fixed4 color = tex2D (_MainTex, i.uv);
				//color.rgb *= i.color;
				color.a = _Alpha;
				return color;
			}
		
			ENDCG
		}
	} 
	FallBack "Unlit/Texture"
}
