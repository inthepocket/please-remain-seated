Shader "Unlit/PRSBackground"
{
    Properties
    {
    	_MainTex ("Main texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        Pass
        {
            Name "Default"
            Tags { "LightMode" = "UniversalForward"}

            ZTest Always
            ZWrite Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            float4x4 _UnityDisplayTransform;

            struct VertexInput
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
            };

            struct VertexOutput
            {
                half4 pos       : SV_POSITION;
                half2 uv        : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
 
            VertexOutput Vertex(VertexInput i)
            {
                VertexOutput o;
                o.pos = TransformObjectToHClip(i.vertex.xyz);
                o.uv.x = (_UnityDisplayTransform[0].x * i.uv.x + _UnityDisplayTransform[1].x * (i.uv.y) + _UnityDisplayTransform[2].x);
                o.uv.y = (_UnityDisplayTransform[0].y * i.uv.x + _UnityDisplayTransform[1].y * (i.uv.y) + _UnityDisplayTransform[2].y);
                return o;
            }

            half4 Fragment(VertexOutput i) : SV_Target
            {
                half4 result = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return result;
            }
            ENDHLSL
        }
    }
}
