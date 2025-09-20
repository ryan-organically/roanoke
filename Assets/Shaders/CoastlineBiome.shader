Shader "Custom/CoastlineBiome"
{
    Properties
    {
        _DeepWaterColor ("Deep Water Color", Color) = (0.0, 0.2, 0.5, 1.0)
        _ShallowWaterColor ("Shallow Water Color", Color) = (0.0, 0.5, 0.7, 1.0)
        _BeachColor ("Beach Color", Color) = (0.9, 0.8, 0.6, 1.0)
        _DuneColor ("Dune Color", Color) = (0.8, 0.7, 0.5, 1.0)
        _ForestColor ("Forest Color", Color) = (0.2, 0.5, 0.2, 1.0)
        _SeaLevel ("Sea Level", Range(-0.5, 0.5)) = 0.0
        _BeachLevel ("Beach Level", Range(-0.5, 0.5)) = 0.1
        _DuneLevel ("Dune Level", Range(-0.5, 0.5)) = 0.3
        _BlendRange ("Blend Range", Range(0.01, 0.2)) = 0.05
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        fixed4 _DeepWaterColor;
        fixed4 _ShallowWaterColor;
        fixed4 _BeachColor;
        fixed4 _DuneColor;
        fixed4 _ForestColor;
        
        float _SeaLevel;
        float _BeachLevel;
        float _DuneLevel;
        float _BlendRange;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float height = IN.worldPos.y;
            
            fixed4 finalColor;
            float smoothness;
            
            if (height < _SeaLevel)
            {
                // Deep water
                float waterDepth = (_SeaLevel - height) / 0.3;
                waterDepth = saturate(waterDepth);
                finalColor = lerp(_ShallowWaterColor, _DeepWaterColor, waterDepth);
                smoothness = 0.9;
            }
            else if (height < _BeachLevel)
            {
                // Shallow water to beach transition
                float blend = smoothstep(_SeaLevel - _BlendRange, _SeaLevel + _BlendRange, height);
                finalColor = lerp(_ShallowWaterColor, _BeachColor, blend);
                smoothness = lerp(0.8, 0.3, blend);
            }
            else if (height < _DuneLevel)
            {
                // Beach to dune transition
                float blend = smoothstep(_BeachLevel - _BlendRange, _BeachLevel + _BlendRange, height);
                finalColor = lerp(_BeachColor, _DuneColor, blend);
                smoothness = 0.2;
            }
            else
            {
                // Dunes to forest transition
                float blend = smoothstep(_DuneLevel - _BlendRange, _DuneLevel + _BlendRange, height);
                finalColor = lerp(_DuneColor, _ForestColor, blend);
                smoothness = 0.1;
            }
            
            o.Albedo = finalColor.rgb;
            o.Metallic = 0.0;
            o.Smoothness = smoothness;
            o.Alpha = finalColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}