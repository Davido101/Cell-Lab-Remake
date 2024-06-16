Shader "Unlit/CellShader"
{
    Properties
    {
        col ("Color", Color) = (0.439, 1., 0.086, 100)
        NR ("Nucleus Radius", float) = 25
        EW ("Exterior Width", float) = 10
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

            fixed4 col;
            float NR;
            float EW;
            float4 frag (v2f __vertex_output) : SV_Target
            {
                float4 fragColor;
                float2 fragCoord = __vertex_output.uv * 200;
                float2 p = fragCoord-(200, 200)/2;
                float ds = dot(p, p);
                float srr = ds/((col.a-EW)*(col.a-EW));
                if (ds>col.a*col.a)
                {
                    // discard
                    return 0;
                }
                if (ds>NR*NR&&srr<1.)
                {
                    fragColor = lerp(float4(col.rgb, 1), float4(1, 1, 1, 1), 0.5);
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
