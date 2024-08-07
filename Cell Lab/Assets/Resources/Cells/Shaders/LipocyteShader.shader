Shader "Unlit/LipocyteShader"
{
    Properties
    {
        col ("Color", Color) = (0.439, 1, 0.086, 0.5)
        CR ("Cell Radius", float) = 100
        scale ("Scale", float) = 5000
        scaleConst ("Scale Constant", float) = 200
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv =  v.uv;
                return o;
            }

            float4 col;
            float scale;
            const float scaleConst;
            static const float NR = 0.005 * scale;
            static const float EW = 0.002 * scale;
            static const float EWH = 0.001 * scale;
            float CR;
            float4 frag (v2f i) : SV_Target
            {
                float4 fragColor;
                float2 fragCoord = i.uv * scaleConst;
                float2 p = fragCoord - (scaleConst, scaleConst) / 2;
                float ds = dot(p, p);
                float srr = ds / ((CR - EW) * (CR - EW));
                if (ds > CR * CR)
                {
                    // discard
                    return 0;
                }
                if (ds > NR * NR && srr < 1)
                {
                    // cell
                    fragColor = lerp(float4(col.rgba), float4(1, 1, 1, col.a), 0.5);
                }
                else
                {
                    // wall or nucleus
                    fragColor = float4(0.5*col.rgb, 1);
                }
                
                // Gamma correction
                fragColor.rgb = pow(fragColor.rgb, 2.2);

                return fragColor;
            }
            ENDCG
        }
    }
}
