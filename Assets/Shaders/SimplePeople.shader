Shader "Custom/SimplePeople" {
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

      LOD 110

      CGPROGRAM
      #pragma surface surf Lambert

      fixed4 _SkinColor;

      sampler2D _MainTex;
      sampler2D _HeadTex;
      sampler2D _BodyTex;
      sampler2D _LegsTex;
      sampler2D _BodyPartMask;
      sampler2D _BloodMask;

      fixed4 _BloodColor;

      struct Input {
         float2 uv_MainTex;
      };

      void surf (Input IN, inout SurfaceOutput o) {
         fixed4 _Skin   = _SkinColor;

         half4 main     = tex2D(_MainTex,      IN.uv_MainTex);
         half4 head     = tex2D(_HeadTex,      IN.uv_MainTex);
         half4 body     = tex2D(_BodyTex,      IN.uv_MainTex);
         half4 legs     = tex2D(_LegsTex,      IN.uv_MainTex);
         half4 bodyPart = tex2D(_BodyPartMask, IN.uv_MainTex);
         half4 blood    = tex2D(_BloodMask,    IN.uv_MainTex);

         head      *= bodyPart.r;
         body      *= bodyPart.g;
         legs      *= bodyPart.b;
         blood.a   *= blood.r;

         _Skin.rgb = lerp (_Skin.rgb, main.rgb,  main.a);
         _Skin.rgb = lerp (_Skin.rgb, head.rgb,  head.a);
         _Skin.rgb = lerp (_Skin.rgb, body.rgb,  body.a);
         _Skin.rgb = lerp (_Skin.rgb, legs.rgb,  legs.a);
         _Skin.rgb = lerp (_Skin.rgb, blood.rgb * _BloodColor.rgb, blood.a);

          o.Albedo = _Skin.rgb;
          o.Alpha  = _Skin.a;
      }

      ENDCG
  }

  Fallback "Custom/SimplePeopleVertexLit", 1
}