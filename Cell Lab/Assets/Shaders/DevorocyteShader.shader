Shader "Unlit/DevorocyteShader"
{
    Properties
    {
        col ("Color", Color) = (0.439, 1, 0.086, 1)
        scale ("Scale", float) = 5000
        CR ("Cell Radius", float) = 100
        N ("Spikes", int) = 19
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

            #define M_PI 3.1415926535897932384626433832795
            const float4 col;
            const float CR;
            const float scale;
            static const float NR = 0.005 * scale;
            static const float EW = 0.002 * scale;
            const float N;
            static const float Nhat = N / (2 * M_PI);
            static const float MAX_R = 0.03 * scale;
            static const float R = MAX_R * 0.04;
            static const float H = MAX_R * 0.3;
            static const float HW = MAX_R * 0.1 / 2;
            static const float X = HW + R * 0.8471271;
            static const float x1 = X-R*0.9863939;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 spikes(float2 p)
            {
                float4 fragColor;
                float d = length(p)-CR;
                float a = 2*frac(Nhat*atan2(p.y, p.x));
                if (a>1)
                    a = 2-a;
                    
                float x = (1-a)*CR*M_PI/N;
                float h;
                if (x<x1||x>X)
                {
                    h = H*(1-x/HW);
                }
                else 
                {
                    h = R-sqrt(R*R-(x-X)*(x-X));
                }
                if (d<h)
                {
                    fragColor = float4(0.5*col.rgb, 1);
                }
                return fragColor;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 fragColor = 0;
                float2 fragCoord = i.uv * 200;
                float2 p = fragCoord-(200, 200) / 2;
                float2 tp = fragCoord-(200, 200) / 2;
                float ds = dot(p, p);
                float srr = ds/((CR-EW)*(CR-EW));
                if (ds>CR*CR)
                {
                    fragColor = spikes(p);
                    return 0;
                }
                
                if (ds>NR*NR&&srr<1)
                {
                    fragColor = lerp(0, col, 0.5);
                }
                else 
                {
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
