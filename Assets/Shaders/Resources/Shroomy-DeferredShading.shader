
Shader "S&G/Shroomy-DeferredShading" {
Properties {
    _LightTexture0 ("", any) = "" {}
    _LightTextureB0 ("", 2D) = "" {}
    _ShadowMapTexture ("", any) = "" {}
    _SrcBlend ("", Float) = 1
    _DstBlend ("", Float) = 1
    _Ramp ("", 2D) = "" {}
}
SubShader {

// Pass 1: Lighting pass
//  LDR case - Lighting encoded into a subtractive ARGB8 buffer
//  HDR case - Lighting additively blended into floating point buffer
Pass {
    ZWrite Off
    Blend [_SrcBlend] [_DstBlend]

CGPROGRAM
#pragma target 3.0
#pragma vertex vert_deferred
#pragma fragment frag
#pragma multi_compile_lightpass
#pragma multi_compile ___ UNITY_HDR_ON

#pragma exclude_renderers nomrt

#include "UnityCG.cginc"
#include "UnityDeferredLibrary.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardUtils.cginc"
#include "UnityGBuffer.cginc"
#include "UnityStandardBRDF.cginc"
#include  "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise3D.hlsl"

sampler2D _CameraGBufferTexture0;
sampler2D _CameraGBufferTexture1;
sampler2D _CameraGBufferTexture2;

sampler2D _Ramp;

float _HighlightValueThreshold;


//https://www.ronja-tutorials.com/post/047-invlerp_remap/#remap
/*float invLerp(float from, float to, float value){
  return (value - from) / (to - from);
}

float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value){
  float rel = invLerp(origFrom, origTo, value);
  return lerp(targetFrom, targetTo, rel);
}*/

//modified from https://stackoverflow.com/questions/15095909/from-rgb-to-hsv-in-opengl-glsl
// All components are in the range [0…1], including hue.
float3 rgb2hsv(float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// All components are in the range [0…1], including hue.
float3 hsv2rgb(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float3 ReplaceColorBasedOnValue(float3 current, float fromValue, float3 to, float range, float fuzziness)
{
    float3 currentHSV = hsv2rgb(current);

    //clip below value threshold, then convert to distance (either 1 (dont replace), or 0 (replace)
    float clip = currentHSV.z - fromValue;
    clip = 1-ceil(clamp(clip, 0, 1));
    
    float3 result = lerp(to, current, saturate((clip - range) / max(fuzziness, 1e-05)));
    return result;
}

half4 CalculateLight (unity_v2f_deferred i)
{
    float3 wpos;
    float2 uv;
    float atten, fadeDist;
    UnityLight light;
    UNITY_INITIALIZE_OUTPUT(UnityLight, light);
    UnityDeferredCalculateLightParams (i, wpos, uv, light.dir, atten, fadeDist);
    light.color = _LightColor.rgb * atten;
    
    // unpack Gbuffer
    half4 gbuffer0 = tex2D (_CameraGBufferTexture0, uv);
    half4 gbuffer1 = tex2D (_CameraGBufferTexture1, uv);
    half4 gbuffer2 = tex2D (_CameraGBufferTexture2, uv);
    UnityStandardData data = UnityStandardDataFromGbuffer(gbuffer0, gbuffer1, gbuffer2);

    // --- CUSTOM BELOW ---
    float3 tangentProjectedDir = light.dir;
    
    float noiseOffset = SimplexNoise(wpos);
    float distToLight = distance(wpos, _LightPos.xyz);
    float d = dot(data.normalWorld, tangentProjectedDir);
    float normD = (1.0+d)/2.0;

    //return float4(data.normalWorld, 1);
    
    //float2 rampUV = float2(rampSamplePos + noiseOffset*0.025, 0.5);
    
    //float3 toLight = _LightPos.xyz - wpos;
    //float radius = dot(toLight, toLight) * _LightPos.w;
    //return float4(radius, 0,0,1);
    
    //w seems to be range, based off internet, but also its not...w is squared, kinda, idk rsqrt works.
    
    float radius = rsqrt(_LightPos.w);

    //1, edge. 0, center.
    float normalizedDist = (distToLight/radius);
    

    //this sample pos will set ramp halfway threshold exactly at half of the lights radius.
    float rampSamplePos = (1.0 - normalizedDist);

    //use the normal as an offset into the ramp sample.
    float normalEffectStrength = 0.1;
    float noiseEffectStrength = 0.015;
    //float normalOffset = pow(d, 1);  //we want the normal to increase in intensity towards the edges
    float normalOffset = 0.0;
    rampSamplePos += normalOffset * normalEffectStrength;
    rampSamplePos += noiseOffset * noiseEffectStrength;
    
     float3 diffuseColor = data.diffuseColor.xyz;
    

    //float2 rampUV = float2(rampSamplePos, 0.5);
    //float4 rampCol = float4(1.0,1.0,1.0,1.0);//tex2D(_Ramp, rampUV);

    //float3 highlightColor = float4(1,1,1,1);
   
    //float3 highlightedDiffuse = ReplaceColorBasedOnValue(diffuseColor, _HighlightValueThreshold, highlightColor, 0.2, 0.2);
    //return float4(highlightedDiffuse, 1);
    //return float4(diffuseColor, 1);
    //return clamp(d*2, 0,1);
    //return abs(d);
    //return d;
    //float diffuseHighlightMix = lerp(diffuseColor, highlightedDiffuse, clamp(d*2, 0,1));
    
    //float3 finalCol = highlightedDiffuse * rampCol.xyz * light.color;
    //float3 finalCol = diffuseHighlightMix ;//* rampCol.xyz * light.color;

    float3 finalCol = diffuseColor * light.color;
    return float4(finalCol, 1.0);

    // ---
}



#ifdef UNITY_HDR_ON
half4
#else
fixed4
#endif
frag (unity_v2f_deferred i) : SV_Target
{
    half4 c = CalculateLight(i);
    #ifdef UNITY_HDR_ON
    return c;
    #else
    return exp2(-c);
    #endif
}

ENDCG
}


// Pass 2: Final decode pass.
// Used only with HDR off, to decode the logarithmic buffer into the main RT
Pass {
    ZTest Always Cull Off ZWrite Off
    Stencil {
        ref [_StencilNonBackground]
        readmask [_StencilNonBackground]
        // Normally just comp would be sufficient, but there's a bug and only front face stencil state is set (case 583207)
        compback equal
        compfront equal
    }

CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers nomrt

#include "UnityCG.cginc"

sampler2D _LightBuffer;
struct v2f {
    float4 vertex : SV_POSITION;
    float2 texcoord : TEXCOORD0;
};

v2f vert (float4 vertex : POSITION, float2 texcoord : TEXCOORD0)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(vertex);
    o.texcoord = texcoord.xy;
#ifdef UNITY_SINGLE_PASS_STEREO
    o.texcoord = TransformStereoScreenSpaceTex(o.texcoord, 1.0f);
#endif
    return o;
}

fixed4 frag (v2f i) : SV_Target
{
    return -log2(tex2D(_LightBuffer, i.texcoord));
}
ENDCG
}

}
Fallback Off
}
