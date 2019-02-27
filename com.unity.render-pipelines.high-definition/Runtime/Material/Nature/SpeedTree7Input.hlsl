#ifndef HDRP_SPEEDTREE7_INPUT_INCLUDED
#define HDRP_SPEEDTREE7_INPUT_INCLUDED

#define SPEEDTREE_Y_UP

#ifdef EFFECT_BUMP
    #define _NORMALMAP
#endif

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

///////////////////////////////////////////////////////////////////////
//  struct SpeedTreeVertexInput

// texcoord setup
//
//      BRANCHES                        FRONDS                      LEAVES
// 0    diffuse uv, branch wind xy      "                           "
// 1    lod xyz, 0                      lod xyz, 0                  anchor xyz, lod scalar
// 2    detail/seam uv, seam amount, 0  frond wind xyz, 0           leaf wind xyz, leaf group

struct SpeedTreeVertexInput
{
    float4 vertex       : POSITION;
    float4 tangent      : TANGENT;
    float3 normal       : NORMAL;
    float4 texcoord     : TEXCOORD0;
    float4 texcoord1    : TEXCOORD1;
    float4 texcoord2    : TEXCOORD2;
    float2 texcoord3    : TEXCOORD3;
    float4 color         : COLOR;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

///////////////////////////////////////////////////////////////////////
//  SpeedTree winds

#ifdef ENABLE_WIND

#define WIND_QUALITY_NONE       0
#define WIND_QUALITY_FASTEST    1
#define WIND_QUALITY_FAST       2
#define WIND_QUALITY_BETTER     3
#define WIND_QUALITY_BEST       4
#define WIND_QUALITY_PALM       5

uniform float _WindQuality;
uniform float _WindEnabled;

#include "SpeedTreeWind.hlsl"

#endif

// Define Input structure

struct Input
{
    float4 color;
    float3 interpolator1;
#ifdef GEOM_TYPE_BRANCH_DETAIL
    float3 interpolator2;
#endif
};

// Define uniforms

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

#ifdef GEOM_TYPE_BRANCH_DETAIL
    #define GEOM_TYPE_BRANCH
#endif

#ifdef GEOM_TYPE_BRANCH_DETAIL
    sampler2D _DetailTex;
#endif

#if defined(GEOM_TYPE_FROND) || defined(GEOM_TYPE_LEAF) || defined(GEOM_TYPE_FACING_LEAF)
    #define SPEEDTREE_ALPHATEST
    float _Cutoff;
#endif

#ifdef SCENESELECTIONPASS
    int _ObjectId;
    int _PassValue;
#endif

#ifdef EFFECT_HUE_VARIATION
    #define HueVariationAmount interpolator1.z
    float4 _HueVariation;
#endif

#if defined(EFFECT_BUMP) && !defined(LIGHTMAP_ON)
    sampler2D _BumpMap;
#endif

float4 _Color;

// This include will define the various Attributes/Varyings structure
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/VaryingMesh.hlsl"

#endif
