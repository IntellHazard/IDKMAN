Shader "Suimono2/URP Surface"
{
    Properties
    {
        _shallowColor ("Shallow Tint", Color) = (0.25,0.7,0.75,0.65)
        _depthColor ("Deep Tint", Color) = (0.02,0.15,0.3,0.9)
        _BlendColor ("Blend Color", Color) = (0,0,0,0)
        _OverlayColor ("Overlay Color", Color) = (0,0,0,0)
        _ReflectionColor ("Reflection Color", Color) = (1,1,1,1)
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SSSColor ("Subsurface Color", Color) = (1,1,1,1)
        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _CausticsColor ("Caustics Color", Color) = (1,1,1,1)
        _UnderwaterColor ("Underwater Color", Color) = (1,1,1,1)

        _NormalTexS ("Shallow Wave Normal", 2D) = "bump" {}
        _NormalTexD ("Deep Wave Normal", 2D) = "bump" {}
        _NormalTexR ("Rolling Wave Normal", 2D) = "bump" {}
        _ReflectionTex ("Reflection Texture", 2D) = "black" {}
        _CubeTex ("Cubemap Reflection", Cube) = "black" {}
        _FoamTex ("Foam Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _WaveTex ("Wave Texture", 2D) = "gray" {}

        _NormalStrength ("Normal Strength", Range(0,1)) = 0.5
        _RefractStrength ("Refraction Strength", Range(0,1)) = 0.15
        _EdgeFade ("Edge Fade", Range(0.01,500)) = 1
        _EdgeFoamFade ("Edge Foam Amount", Range(5,500)) = 1
        _foamScale ("Foam Scale", Range(5,500)) = 1
        _ShallowFoamAmt ("Shallow Foam Amount", Range(5,500)) = 1
        _HeightFoamAmt ("Height Foam Amount", Range(5,500)) = 1
        _HeightFoamHeight ("Height Foam Height", Range(5,500)) = 1
        _HeightFoamSpread ("Height Foam Spread", Range(5,500)) = 1
        _DepthFade ("Depth Fade", Range(0.01,5)) = 1
        _ShallowFade ("Shallow Fade", Range(0.01,5)) = 1
        _RefractFade ("Refraction Fade", Range(0.01,500)) = 1
        _CausticsFade ("Caustics Fade", Range(0.01,500)) = 1

        _shorelineScale ("Shoreline Scale", Float) = 1
        _shorelineFrequency ("Shoreline Frequency", Float) = 1
        _shorelineSpeed ("Shoreline Speed", Float) = 1
        _shorelineHeight ("Shoreline Height", Float) = 1
        _Tess ("Tessellation (unused in URP version)", Float) = 4
        _minDist ("Tess Min (unused)", Range(-180,0)) = 10
        _maxDist ("Tess Max (unused)", Range(20,500)) = 25
        _suimono_uvx ("UV Offset X", Float) = 0
        _suimono_uvy ("UV Offset Y", Float) = 0
        _WaveHeight ("Wave Height", Range(0,1)) = 0.12
        _WaveSpeed ("Wave Speed", Range(0,5)) = 1
        _WaveScale ("Wave Scale", Range(0.01,10)) = 1
        _ReflectionStrength ("Reflection Strength", Range(0,1)) = 0.35
        _FoamStrength ("Foam Strength", Range(0,1)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }
        LOD 200
        Cull Back
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "SuimonoURP"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_NormalTexS); SAMPLER(sampler_NormalTexS);
            TEXTURE2D(_NormalTexD); SAMPLER(sampler_NormalTexD);
            TEXTURE2D(_NormalTexR); SAMPLER(sampler_NormalTexR);
            TEXTURE2D(_ReflectionTex); SAMPLER(sampler_ReflectionTex);
            TEXTURECUBE(_CubeTex); SAMPLER(sampler_CubeTex);
            TEXTURE2D(_FoamTex); SAMPLER(sampler_FoamTex);
            TEXTURE2D(_MaskTex); SAMPLER(sampler_MaskTex);
            TEXTURE2D(_WaveTex); SAMPLER(sampler_WaveTex);
            TEXTURE2D_X(_CameraOpaqueTexture); SAMPLER(sampler_CameraOpaqueTexture);
            TEXTURE2D_X(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 _shallowColor, _depthColor, _BlendColor, _OverlayColor;
                float4 _ReflectionColor, _SpecularColor, _SSSColor, _FoamColor;
                float4 _CausticsColor, _UnderwaterColor;
                float4 _NormalTexS_ST, _NormalTexD_ST, _NormalTexR_ST;
                float4 _FoamTex_ST, _MaskTex_ST, _WaveTex_ST;
                float _NormalStrength, _RefractStrength, _EdgeFade, _EdgeFoamFade;
                float _foamScale, _ShallowFoamAmt, _HeightFoamAmt, _HeightFoamHeight, _HeightFoamSpread;
                float _DepthFade, _ShallowFade, _RefractFade, _CausticsFade;
                float _shorelineScale, _shorelineFrequency, _shorelineSpeed, _shorelineHeight;
                float _Tess, _minDist, _maxDist, _suimono_uvx, _suimono_uvy;
                float _WaveHeight, _WaveSpeed, _WaveScale, _ReflectionStrength, _FoamStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float time = _Time.y * _WaveSpeed;
                float2 waveUV = positionWS.xz * _WaveScale;

                // Lightweight procedural wave displacement. This replaces the original
                // tessellation-dependent vertex wave stage in the Built-in shader.
                float waveA = sin(waveUV.x * 1.7 + time) * cos(waveUV.y * 1.3 + time * 0.8);
                float waveB = sin(waveUV.y * 2.4 - time * 1.1) * 0.35;
                positionWS.y += (waveA + waveB) * _WaveHeight;

                OUT.positionWS = positionWS;
                OUT.positionCS = TransformWorldToHClip(positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv + float2(_suimono_uvx, _suimono_uvy);
                OUT.screenPos = ComputeScreenPos(OUT.positionCS);
                OUT.viewDirWS = GetWorldSpaceViewDir(positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 screenUV = IN.screenPos.xy / max(IN.screenPos.w, 0.0001);
                float2 uvS = TRANSFORM_TEX(IN.uv, _NormalTexS);
                float2 uvD = TRANSFORM_TEX(IN.uv, _NormalTexD);
                float2 uvR = TRANSFORM_TEX(IN.uv, _NormalTexR);
                float time = _Time.y * _WaveSpeed;

                half3 nS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTexS, sampler_NormalTexS, uvS + float2(time * 0.025, time * 0.018)));
                half3 nD = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTexD, sampler_NormalTexD, uvD - float2(time * 0.012, time * 0.02)));
                half3 nR = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTexR, sampler_NormalTexR, uvR + float2(time * 0.008, -time * 0.014)));
                half3 tangentNormal = normalize(lerp(nS, nD, 0.5) + nR * 0.25);
                tangentNormal.xy *= _NormalStrength;

                half3 normalWS = normalize(IN.normalWS + float3(tangentNormal.x, 0, tangentNormal.y));
                half3 viewDir = normalize(IN.viewDirWS);

                float rawSceneDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV).r;
                float sceneEyeDepth = LinearEyeDepth(rawSceneDepth, _ZBufferParams);
                float waterEyeDepth = LinearEyeDepth(IN.positionCS.z / IN.positionCS.w, _ZBufferParams);
                float depthDifference = max(sceneEyeDepth - waterEyeDepth, 0.0);
                float depthFactor = saturate(depthDifference / max(_DepthFade, 0.001));

                half4 waterColor = lerp(_shallowColor, _depthColor, depthFactor);

                float2 distortedUV = screenUV + tangentNormal.xy * (_RefractStrength * 0.025);
                half3 sceneColor = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, distortedUV).rgb;
                waterColor.rgb = lerp(waterColor.rgb, sceneColor, saturate(_RefractStrength * (1.0 - depthFactor)));

                half3 reflection = SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, screenUV).rgb;
                half3 reflectDir = reflect(-viewDir, normalWS);
                half3 cubeReflection = SAMPLE_TEXTURECUBE(_CubeTex, sampler_CubeTex, reflectDir).rgb;
                reflection = lerp(cubeReflection, reflection, 0.5);
                waterColor.rgb = lerp(waterColor.rgb, reflection * _ReflectionColor.rgb, _ReflectionStrength);

                float2 foamUV = TRANSFORM_TEX(IN.uv, _FoamTex) * max(_foamScale * 0.01, 0.01);
                half foamTex = SAMPLE_TEXTURE2D(_FoamTex, sampler_FoamTex, foamUV + time * 0.02).r;
                half foam = saturate((1.0 - depthFactor) * _FoamStrength * foamTex);
                waterColor.rgb = lerp(waterColor.rgb, _FoamColor.rgb, foam);

                half mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, TRANSFORM_TEX(IN.uv, _MaskTex)).r;
                waterColor.a *= mask;
                waterColor.a = saturate(waterColor.a);
                return waterColor;
            }
            ENDHLSL
        }
    }
    FallBack Off
}
