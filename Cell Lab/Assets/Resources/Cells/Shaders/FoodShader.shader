Shader "Unlit/FoodShader"
{
    Properties
    {
        col ("Color", Color) = (0.6, 0.4, 0.2, 1)
        FR ("Food Radius", float) = 10000
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
            const float FR;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 fragColor;
                float2 fragCoord = i.uv * 200;
                
                float2 p = fragCoord - (200, 200) / 2;
                float ds = dot(p, p);

                if (ds > FR)
                    return 0;

                fragColor = lerp(col, float4(1, 1, 1, 1), 0.43);

                // Gamma Correction
                fragColor.rgb = pow(fragColor.rgb, 2.2);

                return fragColor;
            }
            ENDCG
        }
    }
}
