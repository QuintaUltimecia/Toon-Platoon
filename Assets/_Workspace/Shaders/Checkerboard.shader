Shader "4U Games/Checkerboard"
{
    Properties
    {
        _Density("Density", Range(2,50)) = 30
        _Color1("Color1", Color) = (1,1,1,1)
        _Color2("Color2", Color) = (1,1,1,1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _Density;
            fixed4 _Color1;
            fixed4 _Color2;

            v2f vert(float4 pos : POSITION, float2 uv : TEXCOORD0)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = uv * _Density;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 c = i.uv;
                c = floor(c) / 2;
                float checker = frac(c.x + c.y);

                if (checker == 0)
                {
                    return _Color1;
                }
                else
                {
                    return _Color2;
                }
            }
            ENDCG
        }
    }
}
