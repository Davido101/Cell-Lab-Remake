Shader "Unlit/FlagellocyteShader"
{
    Properties
    {
        col ("Color", Color) = (0.439, 1, 0.086, 1)
        scale ("Scale", float) = 5000
        speed ("Speed", Range(0, 100)) = 1 
        CR ("Cell Radius", float) = 100
        END ("Tail Length", Range(0, 2)) = 0.7
        R ("Radius", float) = 0.1
        TAIL_R ("Tail Radius", float) = 400
        scaleConst ("Scale Constant", float) = 560
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
            const float CR;
            const float speed;
            const float scale;
            const float scaleConst;
            static const float NR = 0.005*scale;
            static const float EW = 0.002*scale;
            const float END;
            const float R;
            const float TAIL_R;
            const float res;
            float4 tail(float2 p)
            {
                float4 fragColor = 0;
                float d = length(p);
                if (d < CR - EW * 0.5)
                    return fragColor;
                    
                float ww = (TAIL_R + p.x) * 0.1 * (d - CR) / CR;
                float my = sin(-_Time.y * speed - p.x * 0.04) * ww;
                float k = 4 * cos(-_Time.y * speed - p.x * 0.04) * ww;
                float w = (1 + p.x / (TAIL_R*END)) * 0.3 * (1 + 0.5 * k * k / 100 / 100) * 100;
                if (d - CR < R * CR && 1 > (1 - (d - CR) / CR / R) * (1 - (d - CR) / CR / R))
                    w += 100 * (R - R * sqrt(1 - (1 - (d - CR) / CR / R) * (1 - (d - CR) / CR / R)));
                    
                if (abs(p.y - my) < w)
                {
                    fragColor = float4(0.5*col.rgb, 1);
                }
                
                fragColor.rgb = pow(fragColor.rgb, 2.2);

                return fragColor;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 fragColor = 0;
                float2 fragCoord = i.uv * scaleConst;
                fragColor = 0;
                float2 p = fragCoord - (scaleConst, scaleConst) / 2;
                float ds = dot(p, p);
                float angle = atan2(-p.y, -p.x);
                if (ds > CR * CR && abs(angle) < 1)
                {
                    fragColor = tail(p);
                    return fragColor;
                }
                
                float srr = ds / ((CR-EW) * (CR-EW));
                if (ds > CR * CR)
                {
                    // discard
                    return 0;
                }
                
                if (ds > NR * NR && srr < 1)
                {
                    // Cell
                    fragColor = lerp(float4(col.rgba), float4(1, 1, 1, col.a), 0.5);
                }
                else 
                {
                    // Wall or Nucleus
                    fragColor = float4(0.5 * col.rgb, 1);
                }

                fragColor.rgb = pow(fragColor.rgb, 2.2);

                return fragColor;
            }
            ENDCG
        }
    }
}
