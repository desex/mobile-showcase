Shader "UIParticleSystem/Universal" {
    Properties {
        [MainTexture] _MainTex ("Particle Texture", 2D) = "white" {}
        [MainColor] _Color ("Main Color", Color) = (1,1,1,1)

        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend (Alpha=5, Additive=5)", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend (Alpha=10, Additive=1)", Float) = 10
        
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
        [HideInInspector] _ClipRect ("Clip Rect", Vector) = (-32767, -32767, 32767, 32767)
    }

    // URP-compatible SubShader (selected when Universal RP is active)
    SubShader {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        
        Blend [_SrcBlend] [_DstBlend]
        ColorMask [_ColorMask]

        Pass {
            Name "UniversalUI"
            HLSLPROGRAM
            #pragma vertex ParticleVertex
            #pragma fragment ParticleFragment
            // Built-in includes only — no URP package dependency (UI Canvas path works in URP)
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct ParticleInput {
                float4 pos : POSITION;
                float4 tint : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct ParticleOutput {
                float4 clipPos : SV_POSITION;
                half4 finalTint : COLOR;
                half2 uv : TEXCOORD0;
                float4 localPos : TEXCOORD1;
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float4 _MainTex_ST;
                float4 _ClipRect;
            CBUFFER_END

            ParticleOutput ParticleVertex(ParticleInput input)
            {
                ParticleOutput output;
                output.localPos = input.pos;
                output.clipPos = UnityObjectToClipPos(input.pos);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.finalTint = input.tint * _Color;
                return output;
            }

            half4 ParticleFragment(ParticleOutput input) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, input.uv) * input.finalTint;
                texColor.rgb = GammaToLinearSpace(texColor.rgb);
                
                texColor.a *= UnityGet2DClipping(input.localPos.xy, _ClipRect);
                texColor.rgb *= texColor.a;
                return texColor;
            }
            ENDHLSL
        }
    }

    // Built-in Render Pipeline fallback
    SubShader {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        
        Blend [_SrcBlend] [_DstBlend]
        ColorMask [_ColorMask]

        Pass {
            CGPROGRAM
            #pragma vertex ParticleVertex
            #pragma fragment ParticleFragment
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct ParticleInput {
                float4 pos : POSITION;
                float4 tint : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct ParticleOutput {
                float4 clipPos : SV_POSITION;
                fixed4 finalTint : COLOR;
                half2 uv : TEXCOORD0;
                float4 localPos : TEXCOORD1;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ClipRect;

            ParticleOutput ParticleVertex(ParticleInput input) {
                ParticleOutput output;
                output.localPos = input.pos;
                output.clipPos = UnityObjectToClipPos(input.pos);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.finalTint = input.tint * _Color;
                return output;
            }

            fixed4 ParticleFragment(ParticleOutput input) : SV_Target {
                half4 texColor = tex2D(_MainTex, input.uv) * input.finalTint;
                texColor.rgb = GammaToLinearSpace(texColor.rgb);

                texColor.a *= UnityGet2DClipping(input.localPos.xy, _ClipRect);
                texColor.rgb *= texColor.a;          
                return texColor;
            }
            ENDCG
        }
    }
}
