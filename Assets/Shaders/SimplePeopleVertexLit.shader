// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SimplePeopleVertexLit" {
   Properties {
     _SkinColor ("Skin color", Color) = (0.96,0.725,0.62,1)
     _MainTex ("Main texture", 2D) = "black" { }
     _HeadTex ("Head texture", 2D) = "black" { }
     _BodyTex ("Body texture", 2D) = "black" { }
     _LegsTex ("Legs texture", 2D) = "black" { }
     _BodyPartMask ("Body part mask", 2D) = "black" { }
     _BloodMask ("Blood mask", 2D) = "black" { }
     _BloodColor ("Blood color", Color) = (1,0,0,1)
   }

   SubShader {
      Tags { "RenderType"="Opaque" }
	  LOD 100

      Pass 
		{
		    Tags {"LightMode"="Vertex"}
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 color : COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv     = v.uv;				
				o.color  = ShadeVertexLights(v.vertex, v.normal);
				return o;
			}

			fixed4 _SkinColor;

			sampler2D _MainTex;
			sampler2D _HeadTex;
			sampler2D _BodyTex;
			sampler2D _LegsTex;
			sampler2D _BodyPartMask;
			sampler2D _BloodMask;

			fixed4 _BloodColor;

			fixed4 frag (v2f i) : SV_Target
            {
                float4 _Skin   = _SkinColor;
			  
			    half4 main     = tex2D(_MainTex,      i.uv);
			    half4 head     = tex2D(_HeadTex,      i.uv);
			    half4 body     = tex2D(_BodyTex,      i.uv);
			    half4 legs     = tex2D(_LegsTex,      i.uv);
			    half4 bodyPart = tex2D(_BodyPartMask, i.uv);
			    half4 blood    = tex2D(_BloodMask,    i.uv);

			    head      *= bodyPart.r;
			    body      *= bodyPart.g;
			    legs      *= bodyPart.b;
			    blood.a   *= blood.r;

			    _Skin.rgb = lerp (_Skin.rgb, main.rgb,  main.a);
			    _Skin.rgb = lerp (_Skin.rgb, head.rgb,  head.a);
			    _Skin.rgb = lerp (_Skin.rgb, body.rgb,  body.a);
			    _Skin.rgb = lerp (_Skin.rgb, legs.rgb,  legs.a);
			    _Skin.rgb = lerp (_Skin.rgb, blood.rgb * _BloodColor.rgb, blood.a);

			    _Skin.rgb *= i.color;
				return _Skin;
            }

			
			ENDCG
		} 
	}
}