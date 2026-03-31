Shader "AkilliMum/Standard/Mirror/Complex"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        [Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

        _BumpScale("Scale", Float) = 1.0
        [Normal] _BumpMap("Normal Map", 2D) = "bump" {}

        _Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
        _ParallaxMap ("Height Map", 2D) = "black" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DetailMask("Detail Mask", 2D) = "white" {}

        _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
        _DetailNormalMapScale("Scale", Float) = 1.0
        [Normal] _DetailNormalMap("Normal Map", 2D) = "bump" {}

        [Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0


        // Blending state
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
        




        //new values
        [HideInInspector]_IsMultiPass("IsMultiPass Mobile", Float) = 0.

        [HideInInspector]_ReflectionTex("Reflection", 2D) = "white" { } //left or all
        [HideInInspector]_ReflectionTexOther("ReflectionOther", 2D) = "white" { } //right
        [HideInInspector]_ReflectionTexDepth("Reflection Depth", 2D) = "white" { } //left or all
        [HideInInspector]_ReflectionTexOtherDepth("ReflectionOther Depth", 2D) = "white" { } //right

        [HideInInspector]_ReflectionIntensity("Reflection Intensity", Range(0.0, 1.0)) = 0.5
        
        [HideInInspector]_DisableGI("Disable GI", Float) = 0.
        [HideInInspector]_UseFresnel ("Use Fresnel", Float) = 0.
        [HideInInspector]_UseObjectUV ("Use Object UV", Float) = 0.
        
        [HideInInspector]_LODLevel("Mip Level", Range(0, 10)) = 0
        
        //[HideInInspector]_WetLevel("WetLevel", Float) = 0.
        
        [HideInInspector]_WorkType("Work Type", Float) = 1.
        [HideInInspector]_Platform("Platform - Device Type", Float) = 1.
        [HideInInspector]_MixBlackColor("Mix Black Color", Float) = 0.
        
        [HideInInspector][Enum(UV0, 0, UV1, 1, UV2, 2, UV3, 3, None, 99)] _ClipUV("Clip UV", int) = 99
        [HideInInspector][Enum(None, 0, Left, 1, Right, 2)] _ClipEye("Clip Eye", int) = 0
        [HideInInspector]_ClipPercentage("Clipping Percentage", int) = 20
        
        //[HideInInspector]_EnableRefraction("Enable Refraction", Float) = 0.
        [HideInInspector]_RefractionTex("Refraction", 2D) = "bump" {}
        [HideInInspector]_ReflectionRefraction("Reflection Refraction", Float) = 0.0

        [HideInInspector]_EnableDepthBlur("Enable Depth Blur", Float) = -1.0
        
        [HideInInspector]_EnableSimpleDepth("Enable Simple Depth", Float) = -1.0
        [HideInInspector]_SimpleDepthCutoff("Simple Depth Cutoff", Range(0.0, 50.0)) = 0.5
        
        [HideInInspector]_NearClip("Near Clip", Float) = 0.3
        [HideInInspector]_FarClip("Far Clip", Float) = 1000
        
        //[HideInInspector]_EnableMask("Enable Mask", Float) = -1.0
        [HideInInspector]_MaskTex("Mask", 2D) = "white" {}
        [HideInInspector]_MaskCutoff("Mask Cutoff", Range(0.0, 1.0)) = 0.5
        [HideInInspector]_MaskEdgeDarkness("Mask Edge Darkness", Range(1.0, 50.0)) = 1.
        [HideInInspector]_MaskTiling("Mask Tiling", Vector) = (1,1,1,1)
        
        //[HideInInspector]_EnableWave("Enable Waves", Float) = -1.0
        [HideInInspector]_WaveNoiseTex("Wave Noise Tex", 2D) = "white" {}
        [HideInInspector]_WaveSize("Wave Size", float) = 12.0
        [HideInInspector]_WaveDistortion("Wave Distortion", Float) = 0.02
        [HideInInspector]_WaveSpeed("Wave Speed", Float) = 3.0
        
        //[HideInInspector]_EnableRipple("Enable Ripples", Float) = -1.0
        [HideInInspector]_RippleTex("Ripple", 2D) = "bump" {}
        [HideInInspector]_RippleSize("Ripple Size", Float) = 2.0
        [HideInInspector]_RippleRefraction("Ripple Refraction", Float) = 0.02
        [HideInInspector]_RippleDensity("Ripple Density", Float) = 1.0
        [HideInInspector]_RippleSpeed("Ripple Speed", Float) = 0.3

        [HideInInspector]_EnableLocallyCorrection("Enable Locally Correction", Float) = 0.
        [HideInInspector]_BBoxMin("BBox Min", Vector) = (0,0,0,0)
        [HideInInspector]_BBoxMax("BBox Max", Vector) = (0,0,0,0)
        [HideInInspector]_EnviCubeMapPos("CubeMap Pos", Vector) = (0,0,0,0)
        [HideInInspector]_EnableRotation("Enable Rotation", Float) = 0
        [HideInInspector]_EnviRotation("Environment Rotation", Vector) = (0,0,0,0)
        [HideInInspector]_EnviPosition("Environment Position", Vector) = (0,0,0,0)
    } 

    CGINCLUDE
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 300


        // ------------------------------------------------------------------
        //  Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM
            #pragma target 3.0

            // -------------------------------------

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_fragment _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local_fragment _DETAIL_MULX2
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature_local _PARALLAXMAP
            
             #pragma shader_feature _FULLMIRROR //my custom keys
            #pragma shader_feature _LOCALLYCORRECTION
            //#pragma shader_feature _FULLWATER
            #pragma shader_feature _AKMU_MIRROR_REFRACTION
            #pragma shader_feature _AKMU_MIRROR_MASK
            #pragma shader_feature _AKMU_MIRROR_RIPPLE
            #pragma shader_feature _AKMU_MIRROR_WAVE
            
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE           

            #pragma vertex vertBase
            #pragma fragment fragBase
            #include "StandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma target 3.0

            // -------------------------------------


            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _DETAIL_MULX2
            #pragma shader_feature_local _PARALLAXMAP

             #pragma shader_feature _FULLMIRROR //my custom keys
            #pragma shader_feature _LOCALLYCORRECTION
            //#pragma shader_feature _FULLWATER
            #pragma shader_feature _AKMU_MIRROR_REFRACTION
            #pragma shader_feature _AKMU_MIRROR_MASK
            #pragma shader_feature _AKMU_MIRROR_RIPPLE
            #pragma shader_feature _AKMU_MIRROR_WAVE

            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE

            #pragma vertex vertAdd
            #pragma fragment fragAdd
            #include "StandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma target 3.0

            // -------------------------------------


            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _PARALLAXMAP
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Deferred pass
        Pass
        {
            Name "DEFERRED"
            Tags { "LightMode" = "Deferred" }

            CGPROGRAM
            #pragma target 3.0
            #pragma exclude_renderers nomrt


            // -------------------------------------

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_fragment _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _DETAIL_MULX2
            #pragma shader_feature_local _PARALLAXMAP
            
             #pragma shader_feature _FULLMIRROR //my custom keys
            #pragma shader_feature _LOCALLYCORRECTION
            //#pragma shader_feature _FULLWATER
            #pragma shader_feature _AKMU_MIRROR_REFRACTION
            #pragma shader_feature _AKMU_MIRROR_MASK
            #pragma shader_feature _AKMU_MIRROR_RIPPLE
            #pragma shader_feature _AKMU_MIRROR_WAVE

            #pragma multi_compile_prepassfinal
            #pragma multi_compile_instancing
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE

            #pragma vertex vertDeferred
            #pragma fragment fragDeferred

            #include "StandardCore.cginc"

            ENDCG
        }

        // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            CGPROGRAM
            #pragma vertex vert_meta
            #pragma fragment frag_meta

            #pragma shader_feature_fragment _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION
            

            #include "UnityStandardMeta.cginc"
            ENDCG
        }
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 150

        // ------------------------------------------------------------------
        //  Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM
            #pragma target 2.0

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_fragment _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _GLOSSYREFLECTIONS_OFF
            // SM2.0: NOT SUPPORTED shader_feature_local _DETAIL_MULX2
            // SM2.0: NOT SUPPORTED shader_feature_local _PARALLAXMAP
            
            #pragma shader_feature _FULLMIRROR //my custom keys
            #pragma shader_feature _LOCALLYCORRECTION
            //#pragma shader_feature _FULLWATER
            #pragma shader_feature _AKMU_MIRROR_REFRACTION
            #pragma shader_feature _AKMU_MIRROR_MASK
            #pragma shader_feature _AKMU_MIRROR_RIPPLE
            #pragma shader_feature _AKMU_MIRROR_WAVE

            #pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog

            #pragma vertex vertBase
            #pragma fragment fragBase
            #include "StandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma target 2.0

             #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _DETAIL_MULX2
            // SM2.0: NOT SUPPORTED shader_feature_local _PARALLAXMAP
            #pragma skip_variants SHADOWS_SOFT
            
             #pragma shader_feature _FULLMIRROR //my custom keys
            #pragma shader_feature _LOCALLYCORRECTION
            //#pragma shader_feature _FULLWATER
            #pragma shader_feature _AKMU_MIRROR_REFRACTION
            #pragma shader_feature _AKMU_MIRROR_MASK
            #pragma shader_feature _AKMU_MIRROR_RIPPLE
            #pragma shader_feature _AKMU_MIRROR_WAVE

            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog

            #pragma vertex vertAdd
            #pragma fragment fragAdd
            #include "StandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma target 2.0

            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma skip_variants SHADOWS_SOFT
            #pragma multi_compile_shadowcaster

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"

            ENDCG
        }

        // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            CGPROGRAM
            #pragma vertex vert_meta
            #pragma fragment frag_meta

            #pragma shader_feature_fragment _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION
            

            #include "UnityStandardMeta.cginc"
            ENDCG
        }
    }


    FallBack "VertexLit"
    CustomEditor "ComplexShaderGUI"
}
