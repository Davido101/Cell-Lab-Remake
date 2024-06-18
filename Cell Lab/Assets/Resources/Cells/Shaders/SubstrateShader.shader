Shader "Unlit/SubstrateShader"
{
    Properties
    {
        bgcol ("Background Color", Color) = (0.8509804,0.8509804,0.99215686,1)
        lightcol ("Light Color", Color) = (0.5,0.35,0.25,1)
        radius ("Radius", float) = 100
        dirX ("DirectionX", float) = 1
        dirY ("DirectionY", float) = 0
        amount ("Light Amount", float) = 2
        lrange ("Light Range", float) = 0.5
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

            float4 bgcol;
            float4 lightcol;
            const float radius;
            float dirX;
            float dirY;
            const float amount;
            const float lrange;
            const float res;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 fragColor;
                float2 fragCoord = i.uv * 202;

                fragColor = bgcol;
                float2 p = fragCoord - (202, 202) / 2;
                p /= radius;
                float ds = dot(p, p);

                if (ds > 1.02) {
                    return 0;
                }
                else {
                    if (ds > 0.999) {
                        fragColor = float4(0, 0, 0, 1);
                    }
                    else {
                        float2 dir = float2(dirX, dirY);
                        ds = 1 + (1 - lrange) * (1 - ds) / lrange;
                        float py = dot(dir, p);
                        float l = amount * max ((py * (1 - lrange) + lrange) / (ds * ds), 0);
                        fragColor = bgcol + float4(l * lightcol.rgb, 1);
                    }
                }

                return fragColor;
            }
            ENDCG
        }
    }
}
