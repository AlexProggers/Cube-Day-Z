// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Aubergine/ImageEffects/RadialBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
		Pass {
			Tags { "LightMode" = "Always" }
			ZTest Always Cull Off ZWrite Off Fog { Mode off }

			CGPROGRAM
			#pragma exclude_renderers xbox360 ps3 flash
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			uniform float _CenterX, _CenterY;
			uniform float _Strength;

			v2f_img vert(appdata_img v) {
				v2f_img o;
				o.pos = UnityObjectToClipPos(v.vertex);
				#ifdef UNITY_HALF_TEXEL_OFFSET
					v.texcoord.y += _MainTex_TexelSize.y;
				#endif
				#if SHADER_API_D3D9
					if (_MainTex_TexelSize.y < 0)
						v.texcoord.y = 1.0 - v.texcoord.y;
				#endif
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				return o;
			}

			fixed4 frag (v2f_img i) : COLOR {
				fixed4 main = tex2D(_MainTex, i.uv);
				half2 center = float2(_CenterX, _CenterY);
				i.uv -= center;
				for(int j = 1; j < 11; j++) {
					float scale = 1.0 + (-_Strength * j / 10.0);
					main.rgb += tex2D(_MainTex, i.uv * scale + center);
				}
				main.rgb /= 10;
				return main;
			}
			ENDCG
		}
	}

	Fallback off
}