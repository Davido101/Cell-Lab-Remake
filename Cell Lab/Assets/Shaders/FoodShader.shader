Shader "Unlit/FoodShader"
{
    Properties
    {
        FR ("Food Radius", float) = 350
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

            float FR;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv =  v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 fragColor;
                float2 fragCoord = i.uv * 200;
                
                float2 p = fragCoord - (200, 200) / 2;
                float ds = dot(p, p);

                if (ds > FR)
                    return 0;

                fragColor = lerp(float4(0.6, 0.4, 0.2, 1), float4(1, 1, 1, 1), 0.43);

                // Gamma Correction
                fragColor.rgb = pow(fragColor.rgb, 2.2);

                return fragColor;
            }
            ENDCG
        }
    }
}
