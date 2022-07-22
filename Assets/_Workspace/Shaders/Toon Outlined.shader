﻿Shader "4U Games/Toon Outlined" {
    //show values to edit in inspector
    Properties{
        [Header(Base Parameters)]
        _Color("Tint", Color) = (0, 0, 0, 1)
        _MainTex("Texture", 2D) = "white" {}
        [HDR] _Emission("Emission", color) = (0 ,0 ,0 , 1)

        // Эффект обводки
		_OutlineColor ("Outline Color", COLOR) = (0,0,0,1)
		_OutlineScale ("Outline Scale", Range(0,1)) = 0.001
    }
    SubShader
    {
            Tags{ "RenderType" = "Opaque" "Queue" = "Transparent"}

            CGPROGRAM

            #pragma surface surf Standard fullforwardshadows
            #pragma target 3.0

            sampler2D _MainTex;
            fixed4 _Color;
            half3 _Emission;

            struct Input {
                float2 uv_MainTex;
            };

            void surf(Input i, inout SurfaceOutputStandard o) {
                fixed4 col = tex2D(_MainTex, i.uv_MainTex);
                col *= _Color;
                o.Albedo = col.rgb;

                o.Emission = _Emission;
            }
            ENDCG

            Pass{
			Name "Outline"
			 ZWrite off // Отключите глубокую запись, чтобы при нормальной записи следующего канала предыдущий внутренний штрих был перезаписан (вы можете увидеть внутренний штрих, закомментировав его), чтобы гарантировать, что отображается только контур
			Cull Front
			CGPROGRAM
 
			#pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"
 
			float4 _OutlineColor;
			float _OutlineScale;
 
			struct v2f{
				float4 vertex : SV_POSITION;
			};
 
			v2f vert(appdata_base v){
				v2f o;
				 // Метод обводки 1: Нормальное расширение на объектной модели изменяет положение вершины
				v.vertex.xyz += v.normal * _OutlineScale;
				o.vertex = UnityObjectToClipPos(v.vertex);
 
				 // Метод обводки 2: Нормальное расширение изменяет положение вершины под полем обзора View
				//float4 pos = mul(UNITY_MATRIX_V,mul(unity_ObjectToWorld,v.vertex));
				//float3 normal = normalize(mul((float3x3)UNITY_MATRIX_MV,v.normal));
				//pos += float4(normal,0)* _OutlineScale;
				//o.vertex = mul(UNITY_MATRIX_P,pos);
 
				 // Метод обводки третий: изменение положения вершины путем удлинения нормальной линии под областью отсечения
				//o.vertex = UnityObjectToClipPos(v.vertex);
				//float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_MV,v.normal));
				//float2 clipNormal = normalize(TransformViewToProjection(viewNormal.xy));
				//o.vertex.xy += clipNormal * _OutlineScale;
				return o;
			}
 
			fixed4 frag(v2f i):SV_Target{
			
				return _OutlineColor;
			}
 
			ENDCG
		}
        }

        
            FallBack "Standard"
}