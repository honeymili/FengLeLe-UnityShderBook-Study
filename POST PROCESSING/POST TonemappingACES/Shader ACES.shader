Shader "POST/ACES"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FilmSlope("_FilmSlope", float) = 2.51
        _FilmToe("_FilmToe", float) = 0.03
        _FilmShoulder("_FilmShoulder", float) = 2.43
        _FilmBlackClip("_FilmBlackClip", float) = 0.59
        _FilmWhiteClip("_FilmWhiteClip", float) = 0.14
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float _FilmSlope; 
            float _FilmToe;
            float _FilmShoulder; 
            float _FilmBlackClip; 
            float _FilmWhiteClip;
            float _postExposure;
            CBUFFER_END
            TEXTURE2D(_MainTex);            
            SAMPLER(sampler_MainTex);

            float3 ACESFilm(float3 LinearColor, float a, float b, float c, float d, float e)
            {
                const float ExposureMultiplier = _postExposure; 
                const float3x3 PRE_TONEMAPPING_TRANSFORM =  
                {
                    0.575961650, 0.344143820, 0.079952030,
                    0.070806820, 0.827392350, 0.101774690,
                    0.028035252, 0.131523770, 0.840242300
                };
                const float3x3 EXPOSED_PRE_TONEMAPPING_TRANSFORM = ExposureMultiplier * PRE_TONEMAPPING_TRANSFORM;//key 
                const float3x3 POST_TONEMAPPING_TRANSFORM =
                {
                    1.666954300, -0.601741150, -0.065202855,
                -0.106835220, 1.237778600, -0.130948950,
                -0.004142626, -0.087411870, 1.091555000
                };
                    /*
                    float a; // 2.51
                    float b; // 0.03
                    float c; // 2.43
                    float d; // 0.59
                    float e; // 0.14
                    */
                float3 Color = mul(EXPOSED_PRE_TONEMAPPING_TRANSFORM, LinearColor);                   
                Color = saturate((Color * (a * Color + b)) / (Color * (c * Color + d) + e));          

                return clamp(mul(POST_TONEMAPPING_TRANSFORM, Color), 0.0f, 1.0f);   
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                col.xyz = ACESFilm(col.xyz, _FilmSlope, _FilmToe, _FilmShoulder, _FilmBlackClip, _FilmWhiteClip);
                return col;
            }
            ENDHLSL
        }
    }
}
