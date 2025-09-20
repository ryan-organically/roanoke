Shader "Custom/TerrainHeightBased"
{
    Properties
    {
        _GrassColor ("Grass Color", Color) = (0.3, 0.7, 0.2, 1.0)
        _RockColor ("Rock Color", Color) = (0.5, 0.4, 0.3, 1.0)
        _SnowColor ("Snow Color", Color) = (0.9, 0.9, 1.0, 1.0)
        _GrassHeight ("Grass Height", Range(0, 50)) = 10
        _RockHeight ("Rock Height", Range(0, 50)) = 20
        _BlendRange ("Blend Range", Range(0.1, 10)) = 2
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        fixed4 _GrassColor;
        fixed4 _RockColor;
        fixed4 _SnowColor;
        
        float _GrassHeight;
        float _RockHeight;
        float _BlendRange;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float height = IN.worldPos.y;
            
            // Calculate blend factors based on height
            float grassBlend = 1.0 - smoothstep(_GrassHeight - _BlendRange, _GrassHeight + _BlendRange, height);
            float snowBlend = smoothstep(_RockHeight - _BlendRange, _RockHeight + _BlendRange, height);
            float rockBlend = 1.0 - grassBlend - snowBlend;
            
            // Ensure blend factors sum to 1
            float totalBlend = grassBlend + rockBlend + snowBlend;
            grassBlend /= totalBlend;
            rockBlend /= totalBlend;
            snowBlend /= totalBlend;
            
            // Blend colors based on height
            fixed4 finalColor = _GrassColor * grassBlend + _RockColor * rockBlend + _SnowColor * snowBlend;
            
            o.Albedo = finalColor.rgb;
            o.Metallic = 0.0;
            o.Smoothness = 0.2 + snowBlend * 0.3; // Snow is smoother
            o.Alpha = finalColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}