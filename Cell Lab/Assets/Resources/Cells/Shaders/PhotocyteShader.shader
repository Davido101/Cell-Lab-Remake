Shader "Unlit/PhotocyteShader"
{
    Properties
    {
        col ("Color", Color) = (0.439, 1, 0.086, 0.5)
        CR ("Cell Radius", float) = 100
        scale ("scale", float) = 5000
        blobScale ("Blob Scale", float) = 20
        speed ("Speed", float) = 0.1
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
            const float CR;
            const float scale;
            const float scaleConst;
            static const float NR = 0.005*scale;
            static const float EW = 0.002*scale;
            static const float EWH = 0.001*scale;
            const float blobScale;
            const float speed;
            
            uint hash(uint x, uint seed)
            {
                const uint m = 1540483477u;
                uint hash = seed;
                // process input
                uint k = x;
                k *= m;
                k ^= k >> 24;
                k *= m;
                hash *= m;
                hash ^= k;
                // some final mixing
                hash ^= hash >> 13;
                hash *= m;
                hash ^= hash >> 15;
                return hash;
            }

            uint hash(uint3 x, uint seed)
            {
                const uint m = 1540483477u;
                uint hash = seed;
                // process first vector element
                uint k = x.x;
                k *= m;
                k ^= k >> 24;
                k *= m;
                hash *= m;
                hash ^= k;
                // process second vector element
                k = x.y;
                k *= m;
                k ^= k >> 24;
                k *= m;
                hash *= m;
                hash ^= k;
                // process third vector element
                k = x.z;
                k *= m;
                k ^= k >> 24;
                k *= m;
                hash *= m;
                hash ^= k;
                // some final mixing
                hash ^= hash >> 13;
                hash *= m;
                hash ^= hash >> 15;
                return hash;
            }

            float3 gradientDirection(uint hash)
            {
                switch (int(hash) & 15) { // look at the last four bits to pick a gradient direction
                case 0:
                return float3(1, 1, 0);
                case 1:
                return float3(-1, 1, 0);
                case 2:
                return float3(1, -1, 0);
                case 3:
                return float3(-1, -1, 0);
                case 4:
                return float3(1, 0, 1);
                case 5:
                return float3(-1, 0, 1);
                case 6:
                return float3(1, 0, -1);
                case 7:
                return float3(-1, 0, -1);
                case 8:
                return float3(0, 1, 1);
                case 9:
                return float3(0, -1, 1);
                case 10:
                return float3(0, 1, -1);
                case 11:
                return float3(0, -1, -1);
                case 12:
                return float3(1, 1, 0);
                case 13:
                return float3(-1, 1, 0);
                case 14:
                return float3(0, -1, 1);
                case 15:
                return float3(0, -1, -1);
}
            }

            float interpolate(float value1, float value2, float value3, float value4, float value5, float value6, float value7, float value8, float3 t)
            {
                return lerp(lerp(lerp(value1, value2, t.x), lerp(value3, value4, t.x), t.y), lerp(lerp(value5, value6, t.x), lerp(value7, value8, t.x), t.y), t.z);
            }

            float3 fade(float3 t)
            {
                return t*t*t*(t*(t*6-15)+10);
            }

            float perlinNoise(float3 position, uint seed)
            {
                float3 floorPosition = floor(position);
                float3 fractPosition = position-floorPosition;
                uint3 cellCoordinates = ((uint3)floorPosition);
                float value1 = dot(gradientDirection(hash(cellCoordinates, seed)), fractPosition);
                float value2 = dot(gradientDirection(hash(cellCoordinates+uint3(1, 0, 0), seed)), fractPosition-float3(1, 0, 0));
                float value3 = dot(gradientDirection(hash(cellCoordinates+uint3(0, 1, 0), seed)), fractPosition-float3(0, 1, 0));
                float value4 = dot(gradientDirection(hash(cellCoordinates+uint3(1, 1, 0), seed)), fractPosition-float3(1, 1, 0));
                float value5 = dot(gradientDirection(hash(cellCoordinates+uint3(0, 0, 1), seed)), fractPosition-float3(0, 0, 1));
                float value6 = dot(gradientDirection(hash(cellCoordinates+uint3(1, 0, 1), seed)), fractPosition-float3(1, 0, 1));
                float value7 = dot(gradientDirection(hash(cellCoordinates+uint3(0, 1, 1), seed)), fractPosition-float3(0, 1, 1));
                float value8 = dot(gradientDirection(hash(cellCoordinates+uint3(1, 1, 1), seed)), fractPosition-float3(1, 1, 1));
                return interpolate(value1, value2, value3, value4, value5, value6, value7, value8, fade(fractPosition));
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 fragColor = 0;
                float2 fragCoord = i.uv * scaleConst;
                float2 p = fragCoord-(scaleConst, scaleConst)/2;
                float ds = dot(p, p);
                if (ds>CR*CR)
                {
                    return 0;
                }
                
                float srr = ds/((CR-EW)*(CR-EW));
                float2 stc = 0.5*p.xy/CR*sqrt(srr);
                float2 noisepos = fragCoord;
                float noise1 = perlinNoise(float3(noisepos/blobScale, _Time.y*speed), uint(0));
                float noise2 = perlinNoise(float3(noisepos/blobScale, _Time.y*speed), uint(1));
                float pl = noise1+noise2;
                if (ds > NR*NR && srr < 1)
                {
                    if (pl>0.5)
                    {
                        // chlorophyll
                        fragColor = float4(0, 0.6, 0.1, 1);
                    }
                    else 
                    {
                        // cell
                        fragColor = lerp(float4(col.rgb, col.a), float4(1, 1, 1, col.a), 0.4);
                    }
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
