Shader "Voxelizer/Demo/AlphaVoxelWave" {

	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_Wave ("Wave", Vector) = (1.0, 0.025, -1, -1)
		_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_EmissionStrength("Emission Strength", float) = 1
	}

	SubShader {
		Tags { "Queue" = "Transparent" }
		LOD 200

		CGPROGRAM

		#pragma surface surf Standard alpha:fade vertex:vert

		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _Wave;
		half4 _EmissionColor;
		float _EmissionStrength;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)


		fixed2 random2(fixed2 st){
            st = fixed2( dot(st,fixed2(127.1,311.7)),
                           dot(st,fixed2(269.5,183.3)) );
            return -1.0 + 2.0*frac(sin(st)*43758.5453123);
        }



		float perlinNoise(fixed2 st) 
        {
            fixed2 p = floor(st);
            fixed2 f = frac(st);
            fixed2 u = f*f*(3.0-2.0*f);

            float v00 = random2(p+fixed2(0,0));
            float v10 = random2(p+fixed2(1,0));
            float v01 = random2(p+fixed2(0,1));
            float v11 = random2(p+fixed2(1,1));

            return lerp( lerp( dot( v00, f - fixed2(0,0) ), dot( v10, f - fixed2(1,0) ), u.x ),
                         lerp( dot( v01, f - fixed2(0,1) ), dot( v11, f - fixed2(1,1) ), u.x ), 
                         u.y)+0.5f;
        }



		void vert (inout appdata_full v) {
			float3 seed = v.tangent.xyz;
			fixed2 data = v.vertex.y;
			float noise = perlinNoise(data);
			float t = (sin(seed.y * _Wave.y + _Time.y * _Wave.x) + 1.0) * 0.5;
			v.vertex.xyz = lerp(v.vertex.xyz, v.tangent.xyz, t);
		}

		

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = _EmissionColor * _EmissionStrength;
		}

		ENDCG
	}

	FallBack "Diffuse"
}
