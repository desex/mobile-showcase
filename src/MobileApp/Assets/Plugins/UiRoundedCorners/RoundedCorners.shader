Shader "UI/RoundedCorners/RoundedCorners" {
    Properties {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}

        // --- Mask support ---
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [HideInInspector] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        
        // Definition in Properties section is required to Mask works properly
        _WidthHeightRadius ("WidthHeightRadius", Vector) = (0,0,0,0)
        _OuterUV ("image outer uv", Vector) = (0, 0, 1, 1)
        // ---
    }
    
    SubShader {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        // --- Mask support ---
        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZTest [unity_GUIZTestMode]
        ColorMask [_ColorMask]
        // ---
        
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZWrite Off

        Pass {
            CGPROGRAM
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"          
            #include "SDFUtils.cginc"
            #include "ShaderSetup.cginc"
            
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            float4 _WidthHeightRadius;
            float4 _OuterUV;
            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            float3 ColorspaceConversionRGBToLinear(float3 value) {
                float3 linearRGBLo = value / 12.92;
                float3 linearRGBHi = pow(max(abs((value + 0.055) / 1.055), 1.192092896e-07), float3(2.4, 2.4, 2.4));
                return lerp(linearRGBHi, linearRGBLo, value <= 0.04045);
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 uvSample = i.uv;
                uvSample.x = (uvSample.x - _OuterUV.x) / (_OuterUV.z - _OuterUV.x);
                uvSample.y = (uvSample.y - _OuterUV.y) / (_OuterUV.w - _OuterUV.y);

                half4 mainTexColor = tex2D(_MainTex, i.uv) + _TextureSampleAdd;
                float3 linearVertexRgb = ColorspaceConversionRGBToLinear(i.color.rgb);
                float linearVertexAlpha = ColorspaceConversionRGBToLinear(i.color.a.xxx).x;

                half4 color;
                color.rgb = mainTexColor.rgb * linearVertexRgb;
                color.a = mainTexColor.a * linearVertexAlpha;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                if (color.a <= 0) {
                    return color;
                }

                float alpha = CalcAlpha(uvSample, _WidthHeightRadius.xy, _WidthHeightRadius.z);

                #ifdef UNITY_UI_ALPHACLIP
                clip(alpha - 0.001);
                #endif
                
                color.a = min(color.a, alpha);
                return color;
            }
            
            ENDCG
        }
    }
}
