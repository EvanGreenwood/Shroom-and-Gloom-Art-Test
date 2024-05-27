// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNG/Environment"
{
	Properties
	{
		_ShadowHSVOffset("ShadowHSVOffset", Vector) = (0,0,0,0)
		_LineHSVOffset("LineHSVOffset", Vector) = (0,0,0,0)
		_HighlightHSVOffset2("HighlightHSVOffset", Vector) = (0,0,0,0)
		[Toggle(_DEBUGHIGHLIGHTLOWLIGHT_ON)] _DebugHighlightLowlight("DebugHighlightLowlight", Float) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.6
		_FogIntensityCoefficient("FogIntensityCoefficient", Range( 0 , 1)) = 1
		[PerRendererData]_MainTex("MainTex", 2D) = "white" {}
		_NormalMip("NormalMip", Range( 0 , 9)) = 0
		[Toggle(_USEFOG_ON)] _UseFog("Use Fog", Float) = 1
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_Emission("Emission", 2D) = "black" {}
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		[PerRendererData]_TunnelDistance("TunnelDistance", Float) = 0
		_EmissionUsesTunnelWave("EmissionUsesTunnelWave", Float) = 0
		[Toggle(_FANCYCOLORSHADING_ON)] _FancyColorShading("FancyColorShading", Float) = 1
		_LineFade("LineFade", Range( 0 , 1)) = 0
		[Toggle(_LINEFADEWITHFOG_ON)] _LineFadeWithFog("LineFadeWithFog", Float) = 0
		[PerRendererData]_TunnelIndex("TunnelIndex", Int) = 0
		[Toggle(_USEIFDOORMASK_ON)] _UseIfDoorMask("Use If Door Mask", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile __ _SCENE_VIEW
		#pragma multi_compile_local __ _USEFOG_ON
		#pragma multi_compile_local __ _FANCYCOLORSHADING_ON
		#pragma shader_feature_local _LINEFADEWITHFOG_ON
		#pragma shader_feature_local _DEBUGHIGHLIGHTLOWLIGHT_ON
		#pragma shader_feature_local _USEIFDOORMASK_ON
		#define ASE_USING_SAMPLING_MACROS 1
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex.SampleLevel(samplerTex,coord, lod)
		#define SAMPLE_TEXTURE2D_BIAS(tex,samplerTex,coord,bias) tex.SampleBias(samplerTex,coord,bias)
		#define SAMPLE_TEXTURE2D_GRAD(tex,samplerTex,coord,ddx,ddy) tex.SampleGrad(samplerTex,coord,ddx,ddy)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
		#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex2Dlod(tex,float4(coord,0,lod))
		#define SAMPLE_TEXTURE2D_BIAS(tex,samplerTex,coord,bias) tex2Dbias(tex,float4(coord,0,bias))
		#define SAMPLE_TEXTURE2D_GRAD(tex,samplerTex,coord,ddx,ddy) tex2Dgrad(tex,coord,ddx,ddy)
		#endif//ASE Sampling Macros

		#pragma surface surf Standard keepalpha noshadow exclude_path:forward noambient nolightmap  nodynlightmap nodirlightmap nofog vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float eyeDepth;
			float4 screenPos;
		};

		UNITY_DECLARE_TEX2D_NOSAMPLER(_Normal);
		uniform float4 _Normal_ST;
		uniform float _NormalMip;
		SamplerState sampler_Normal;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_MainTex);
		uniform float4 _MainTex_ST;
		SamplerState sampler_MainTex;
		uniform float3 _LineHSVOffset;
		uniform float _LineFade;
		uniform float FogDistance;
		uniform float FogStart;
		uniform float _FogIntensityCoefficient;
		uniform float3 _ShadowHSVOffset;
		uniform float3 _HighlightHSVOffset2;
		uniform float4 FogColor;
		uniform int _TunnelIndex;
		uniform int _MaskThresholdIndex;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_DoorMask);
		SamplerState sampler_Point_Clamp;
		uniform int _UseDoorMasking;
		uniform float _TunnelDistance;
		uniform float _EmissionUsesTunnelWave;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Emission);
		uniform float4 _Emission_ST;
		SamplerState sampler_Emission;
		uniform float4 _EmissionColor;
		uniform float _Cutoff = 0.6;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		float TunnelMaskBrokenRip446( int TunnelIndex, int MaskThresholdIndex, float TunnelIndexFromScreenTex )
		{
			// un-normalize mask textures tunnel index.
			int pixelIndex = TunnelIndexFromScreenTex * 256;
			/*
			if(TunnelIndex != pixelIndex)
			{
				return 0;
			}
			else 
			{
				return 1;
			}
			*/
			// find the difference between textures mask index and current
			// rendering objects index. If they are the same we invert and clamp.
			// result is 1. if they are different result is 0.
			int result = clamp(1-abs(pixelIndex - TunnelIndex), 0, 1);
			// override result if threshold index is >= tunnel index. This allows us to see the unmasked tunnel we are in.
			float thresholdPassthrough= lerp(1, (float)result, step(MaskThresholdIndex+1, TunnelIndex));
			return thresholdPassthrough;
		}


		float TunnelMaskIf453( int TunnelIndex, int MaskThresholdIndex, float TunnelIndexFromScreenTex )
		{
			// un-normalize mask textures tunnel index.
			float pixelIndex =TunnelIndexFromScreenTex * 256;
			if(TunnelIndex <= MaskThresholdIndex)
			{
				return 1;
			}
			if(abs(TunnelIndex - pixelIndex) < 0.5)
			{
				return 1;
			}
			return 0;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 normalizeResult374 = normalize( UnpackNormal( SAMPLE_TEXTURE2D_LOD( _Normal, sampler_Normal, uv_Normal, _NormalMip ) ) );
			float3 NormalSample347 = normalizeResult374;
			o.Normal = NormalSample347;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, uv_MainTex );
			float4 TexCol20_g106 = tex2DNode1;
			float4 temp_output_61_0_g106 = i.vertexColor;
			float3 hsvTorgb37_g106 = RGBToHSV( temp_output_61_0_g106.rgb );
			float3 break34_g106 = ( _LineHSVOffset + hsvTorgb37_g106 );
			float3 hsvTorgb36_g106 = HSVToRGB( float3(break34_g106.x,break34_g106.y,break34_g106.z) );
			float3 LineFadeCol33_g106 = hsvTorgb36_g106;
			float cameraDepthFade30 = (( i.eyeDepth -_ProjectionParams.y - FogStart ) / FogDistance);
			float temp_output_2_0_g105 = ( saturate( ( cameraDepthFade30 * _FogIntensityCoefficient ) ) - 1.0 );
			float temp_output_2_0_g107 = ( 1.0 - sqrt( ( 1.0 - ( temp_output_2_0_g105 * temp_output_2_0_g105 ) ) ) );
			float temp_output_51_0 = ( 1.0 - ( temp_output_2_0_g107 * temp_output_2_0_g107 ) );
			float FogCoeff562 = temp_output_51_0;
			#ifdef _LINEFADEWITHFOG_ON
				float staticSwitch563 = saturate( ( _LineFade + FogCoeff562 ) );
			#else
				float staticSwitch563 = _LineFade;
			#endif
			float4 lerpResult14_g106 = lerp( TexCol20_g106 , float4( LineFadeCol33_g106 , 0.0 ) , staticSwitch563);
			float4 LineCol49_g106 = lerpResult14_g106;
			float grayscale29_g106 = Luminance(TexCol20_g106.rgb);
			float Grayscale30_g106 = grayscale29_g106;
			float smoothstepResult80_g106 = smoothstep( ( 0.6 - 0.0 ) , ( 0.6 + 0.0 ) , ( 1.0 - Grayscale30_g106 ));
			float temp_output_94_0_g106 = ( 1.0 - 0.6 );
			float smoothstepResult79_g106 = smoothstep( ( temp_output_94_0_g106 - 0.1 ) , ( temp_output_94_0_g106 + 0.1 ) , Grayscale30_g106);
			float2 appendResult74_g106 = (float2(smoothstepResult80_g106 , smoothstepResult79_g106));
			float3 hsvTorgb25_g106 = RGBToHSV( temp_output_61_0_g106.rgb );
			float3 break32_g106 = ( _ShadowHSVOffset + hsvTorgb25_g106 );
			float3 hsvTorgb31_g106 = HSVToRGB( float3(break32_g106.x,break32_g106.y,break32_g106.z) );
			float3 ShadowCol47_g106 = hsvTorgb31_g106;
			float3 hsvTorgb44_g106 = RGBToHSV( temp_output_61_0_g106.rgb );
			float3 break41_g106 = ( _HighlightHSVOffset2 + hsvTorgb44_g106 );
			float3 hsvTorgb43_g106 = HSVToRGB( float3(break41_g106.x,break41_g106.y,break41_g106.z) );
			float3 HighlightCol40_g106 = hsvTorgb43_g106;
			float2 layeredBlendVar107_g106 = appendResult74_g106;
			float4 layeredBlend107_g106 = ( lerp( lerp( LineCol49_g106 , float4( ShadowCol47_g106 , 0.0 ) , layeredBlendVar107_g106.x ) , float4( HighlightCol40_g106 , 0.0 ) , layeredBlendVar107_g106.y ) );
			float4 color63_g106 = IsGammaSpace() ? float4(1,0,0,1) : float4(1,0,0,1);
			float4 color62_g106 = IsGammaSpace() ? float4(0.1942594,1,0,1) : float4(0.03134425,1,0,1);
			float2 layeredBlendVar52_g106 = appendResult74_g106;
			float4 layeredBlend52_g106 = ( lerp( lerp( LineCol49_g106 , color63_g106 , layeredBlendVar52_g106.x ) , color62_g106 , layeredBlendVar52_g106.y ) );
			#ifdef _DEBUGHIGHLIGHTLOWLIGHT_ON
				float4 staticSwitch106_g106 = layeredBlend52_g106;
			#else
				float4 staticSwitch106_g106 = layeredBlend107_g106;
			#endif
			float smoothstepResult102_g106 = smoothstep( 0.0 , 0.3 , Grayscale30_g106);
			float4 lerpResult97_g106 = lerp( LineCol49_g106 , staticSwitch106_g106 , smoothstepResult102_g106);
			#ifdef _FANCYCOLORSHADING_ON
				float4 staticSwitch424 = lerpResult97_g106;
			#else
				float4 staticSwitch424 = ( i.vertexColor * tex2DNode1 );
			#endif
			float4 BaseColor415 = staticSwitch424;
			float4 temp_output_2_0_g108 = FogColor;
			float4 lerpResult13 = lerp( BaseColor415 , ( temp_output_2_0_g108 * temp_output_2_0_g108 ) , temp_output_51_0);
			#ifdef _USEFOG_ON
				float4 staticSwitch280 = lerpResult13;
			#else
				float4 staticSwitch280 = BaseColor415;
			#endif
			#ifdef _SCENE_VIEW
				float4 staticSwitch370 = BaseColor415;
			#else
				float4 staticSwitch370 = staticSwitch280;
			#endif
			float4 Diffuse350 = staticSwitch370;
			int TunnelIndex446 = _TunnelIndex;
			int MaskThresholdIndex446 = _MaskThresholdIndex;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float4 tex2DNode431 = SAMPLE_TEXTURE2D( _DoorMask, sampler_Point_Clamp, ase_screenPosNorm.xy );
			float TunnelIndexFromScreenTex446 = tex2DNode431.r;
			float localTunnelMaskBrokenRip446 = TunnelMaskBrokenRip446( TunnelIndex446 , MaskThresholdIndex446 , TunnelIndexFromScreenTex446 );
			int TunnelIndex453 = _TunnelIndex;
			int MaskThresholdIndex453 = _MaskThresholdIndex;
			float TunnelIndexFromScreenTex453 = tex2DNode431.r;
			float localTunnelMaskIf453 = TunnelMaskIf453( TunnelIndex453 , MaskThresholdIndex453 , TunnelIndexFromScreenTex453 );
			#ifdef _USEIFDOORMASK_ON
				float staticSwitch454 = localTunnelMaskIf453;
			#else
				float staticSwitch454 = localTunnelMaskBrokenRip446;
			#endif
			clip( ( staticSwitch454 + ( 1.0 - _UseDoorMasking ) ) - 0.01);
			float4 MaskedDiffuse458 = Diffuse350;
			o.Albedo = MaskedDiffuse458.rgb;
			float2 break19_g109 = float2( -0.1,0.1 );
			float temp_output_1_0_g109 = ( _TunnelDistance + _Time.y );
			float sinIn7_g109 = sin( temp_output_1_0_g109 );
			float sinInOffset6_g109 = sin( ( temp_output_1_0_g109 + 1.0 ) );
			float lerpResult20_g109 = lerp( break19_g109.x , break19_g109.y , frac( ( sin( ( ( sinIn7_g109 - sinInOffset6_g109 ) * 91.2228 ) ) * 43758.55 ) ));
			float clampResult385 = clamp( ( lerpResult20_g109 + sinIn7_g109 ) , 0.0 , 1.0 );
			float lerpResult426 = lerp( 1.0 , clampResult385 , _EmissionUsesTunnelWave);
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float4 Emission390 = ( lerpResult426 * ( SAMPLE_TEXTURE2D( _Emission, sampler_Emission, uv_Emission ) * _EmissionColor ) );
			o.Emission = Emission390.rgb;
			o.Alpha = 1;
			float MainTexAlpha45 = tex2DNode1.a;
			clip( MainTexAlpha45 - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;52;-4592,-16;Inherit;False;2626.349;498.1353;Applies fog based on camera depth;20;350;416;370;417;280;30;13;418;51;36;35;14;42;41;37;368;367;28;29;562;Apply Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-4576,320;Inherit;False;Global;FogStart;FogStart;2;0;Create;True;0;0;0;False;0;False;1.5;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-4576,240;Inherit;False;Global;FogDistance;FogDistance;2;0;Create;True;0;0;0;False;0;False;20;50;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;367;-4352,400;Inherit;False;Property;_FogIntensityCoefficient;FogIntensityCoefficient;6;0;Create;True;0;0;0;False;0;False;1;0.227;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;30;-4320,256;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;368;-4064,304;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;37;-3904,224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;41;-3744,240;Inherit;False;EasingOutCirc;-1;;104;181ee7db325895443a774b654a9b8500;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;42;-3568,240;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;35;-3424,240;Inherit;False;Square;-1;;107;fea980a1f68019543b2cd91d506986e8;0;1;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;51;-3280,240;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;50;-4590,-960;Inherit;False;1799.184;791.7819;Comment;14;563;561;45;415;424;560;3;474;473;2;1;564;565;566;Tex And Vertex Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;562;-3072,256;Inherit;False;FogCoeff;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;561;-4560,-896;Inherit;False;Property;_LineFade;LineFade;16;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;564;-4560,-784;Inherit;False;562;FogCoeff;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;565;-4288,-816;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-4368,-704;Inherit;True;Property;_MainTex;MainTex;7;1;[PerRendererData];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;566;-4176,-784;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;2;-3968,-624;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;473;-4000,-704;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;474;-4016,-464;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;563;-4016,-864;Inherit;False;Property;_LineFadeWithFog;LineFadeWithFog;17;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-3712,-496;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;560;-3776,-752;Inherit;False;ColorShading;0;;106;0305f7aefb8244231b081132dcf9bfb0;0;3;15;FLOAT;0;False;18;COLOR;1,1,1,1;False;61;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;424;-3504,-640;Inherit;False;Property;_FancyColorShading;FancyColorShading;15;0;Create;True;0;0;0;False;0;False;1;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;14;-4320,48;Inherit;False;Global;FogColor;FogColor;2;0;Create;True;0;0;0;False;0;False;0.1887822,0.1643645,0.509434,1;0.07058821,0.6026263,0.772549,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;415;-3200,-624;Inherit;False;BaseColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;389;-4528,-3328;Inherit;False;1060;1059;Emission;13;380;379;381;383;376;375;385;387;377;384;390;425;426;Emission;0,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;438;-4544,-2096;Inherit;False;2035.372;568.2113;Door Mask Clipping (Fake stencil) Set global tex in c#;15;458;428;461;463;462;437;371;454;453;446;452;431;427;445;430;Door Tunnel Mask;0,0,0,1;0;0
Node;AmplifyShaderEditor.FunctionNode;36;-3984,96;Inherit;False;Square;-1;;108;fea980a1f68019543b2cd91d506986e8;0;1;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;418;-3280,64;Inherit;False;415;BaseColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;379;-4512,-3248;Inherit;False;Property;_TunnelDistance;TunnelDistance;13;1;[PerRendererData];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;380;-4512,-3152;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;430;-4480,-1856;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerStateNode;445;-4480,-1648;Inherit;False;1;1;1;0;-1;None;1;0;SAMPLER2D;;False;1;SAMPLERSTATE;0
Node;AmplifyShaderEditor.LerpOp;13;-3056,80;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;417;-2864,64;Inherit;False;415;BaseColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;381;-4304,-3120;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;427;-4144,-2032;Inherit;False;Property;_TunnelIndex;TunnelIndex;18;1;[PerRendererData];Create;True;0;0;0;False;0;False;0;0;True;0;1;INT;0
Node;AmplifyShaderEditor.SamplerNode;431;-4256,-1840;Inherit;True;Global;_DoorMask;_DoorMask;19;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;452;-4208,-1952;Inherit;False;Global;_MaskThresholdIndex;_MaskThresholdIndex;21;0;Create;True;0;0;0;False;0;False;1;4;True;0;1;INT;0
Node;AmplifyShaderEditor.StaticSwitch;280;-2640,160;Inherit;False;Property;_UseFog;Use Fog;9;0;Create;True;0;0;0;False;0;False;1;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;416;-2608,272;Inherit;False;415;BaseColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;329;-4560,-1360;Inherit;False;976.0073;302.9471;Comment;4;347;178;306;374;Mip Smoothed Normal Sample;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;383;-4304,-2960;Inherit;False;Noise Sine Wave;-1;;109;a6eff29f739ced848846e3b648af87bd;0;2;1;FLOAT;0;False;2;FLOAT2;-0.1,0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;446;-3856,-1984;Inherit;False;$$$// un-normalize mask textures tunnel index.$int pixelIndex = TunnelIndexFromScreenTex * 256@$$/*$if(TunnelIndex != pixelIndex)${$	return 0@$}$else ${$	return 1@$}$*/$$$// find the difference between textures mask index and current$// rendering objects index. If they are the same we invert and clamp.$// result is 1. if they are different result is 0.$$int result = clamp(1-abs(pixelIndex - TunnelIndex), 0, 1)@$$// override result if threshold index is >= tunnel index. This allows us to see the unmasked tunnel we are in.$float thresholdPassthrough= lerp(1, (float)result, step(MaskThresholdIndex+1, TunnelIndex))@$return thresholdPassthrough@$;1;Create;3;True;TunnelIndex;INT;0;In;;Float;False;True;MaskThresholdIndex;INT;0;In;;Inherit;False;True;TunnelIndexFromScreenTex;FLOAT;0;In;;Float;False;Tunnel Mask (Broken Rip);True;False;0;;False;3;0;INT;0;False;1;INT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;453;-3872,-1824;Inherit;False;// un-normalize mask textures tunnel index.$float pixelIndex =TunnelIndexFromScreenTex * 256@$$if(TunnelIndex <= MaskThresholdIndex)${$	return 1@$}$if(abs(TunnelIndex - pixelIndex) < 0.5)${$	return 1@$}$return 0@$;1;Create;3;True;TunnelIndex;INT;0;In;;Float;False;True;MaskThresholdIndex;INT;0;In;;Inherit;False;True;TunnelIndexFromScreenTex;FLOAT;0;In;;Float;False;Tunnel Mask(If);True;False;0;;False;3;0;INT;0;False;1;INT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;461;-3536,-1776;Inherit;False;Global;_UseDoorMasking;_UseDoorMasking;23;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.StaticSwitch;370;-2400,160;Inherit;False;Property;_SCENE_VIEW;SCENE_VIEW;10;0;Create;True;0;0;0;False;0;False;1;1;1;False;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;306;-4544,-1200;Inherit;False;Property;_NormalMip;NormalMip;8;0;Create;True;0;0;0;False;0;False;0;2;0;9;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;376;-4368,-2480;Inherit;False;Property;_EmissionColor;EmissionColor;12;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,1,0.7490196,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;387;-4160,-3216;Inherit;False;Constant;_Float11;Float 11;17;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;425;-4192,-2848;Inherit;False;Property;_EmissionUsesTunnelWave;EmissionUsesTunnelWave;14;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;385;-4064,-2992;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;454;-3520,-1888;Inherit;False;Property;_UseIfDoorMask;Use If Door Mask;20;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;463;-3296,-1776;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.SamplerNode;375;-4480,-2752;Inherit;True;Property;_Emission;Emission;11;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;350;-2176,160;Inherit;False;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;178;-4208,-1312;Inherit;True;Property;_Normal;Normal;10;1;[Normal];Create;True;0;0;0;False;0;False;178;None;None;True;0;False;bump;Auto;True;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;377;-4144,-2752;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;426;-3840,-3120;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;371;-3456,-2032;Inherit;False;350;Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;437;-3424,-1680;Inherit;False;Constant;_MaskClipThreshold;MaskClipThreshold;21;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;462;-3136,-1872;Inherit;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;384;-3872,-2784;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;374;-3904,-1264;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClipNode;428;-2960,-1904;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;347;-3792,-1136;Inherit;False;NormalSample;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;458;-2784,-1904;Inherit;False;MaskedDiffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;390;-3664,-2656;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-3952,-352;Inherit;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;391;-1248,-1904;Inherit;False;390;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;372;-1280,-1984;Inherit;False;347;NormalSample;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;459;-1280,-2080;Inherit;False;458;MaskedDiffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-1280,-1776;Inherit;False;45;MainTexAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1008,-2016;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SNG/Environment;False;False;False;False;True;False;True;True;True;True;False;False;False;False;True;False;False;False;False;False;False;Off;1;False;;1;False;;False;0;False;;0;False;;False;0;Masked;0.6;True;False;0;False;TransparentCutout;;AlphaTest;DeferredOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;5;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;True;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;30;0;28;0
WireConnection;30;1;29;0
WireConnection;368;0;30;0
WireConnection;368;1;367;0
WireConnection;37;0;368;0
WireConnection;41;1;37;0
WireConnection;42;0;41;0
WireConnection;35;2;42;0
WireConnection;51;0;35;0
WireConnection;562;0;51;0
WireConnection;565;0;561;0
WireConnection;565;1;564;0
WireConnection;566;0;565;0
WireConnection;473;0;1;0
WireConnection;474;0;1;0
WireConnection;563;1;561;0
WireConnection;563;0;566;0
WireConnection;3;0;2;0
WireConnection;3;1;474;0
WireConnection;560;15;563;0
WireConnection;560;18;473;0
WireConnection;560;61;2;0
WireConnection;424;1;3;0
WireConnection;424;0;560;0
WireConnection;415;0;424;0
WireConnection;36;2;14;0
WireConnection;13;0;418;0
WireConnection;13;1;36;0
WireConnection;13;2;51;0
WireConnection;381;0;379;0
WireConnection;381;1;380;0
WireConnection;431;1;430;0
WireConnection;431;7;445;0
WireConnection;280;1;417;0
WireConnection;280;0;13;0
WireConnection;383;1;381;0
WireConnection;446;0;427;0
WireConnection;446;1;452;0
WireConnection;446;2;431;1
WireConnection;453;0;427;0
WireConnection;453;1;452;0
WireConnection;453;2;431;1
WireConnection;370;1;280;0
WireConnection;370;0;416;0
WireConnection;385;0;383;0
WireConnection;454;1;446;0
WireConnection;454;0;453;0
WireConnection;463;0;461;0
WireConnection;350;0;370;0
WireConnection;178;2;306;0
WireConnection;377;0;375;0
WireConnection;377;1;376;0
WireConnection;426;0;387;0
WireConnection;426;1;385;0
WireConnection;426;2;425;0
WireConnection;462;0;454;0
WireConnection;462;1;463;0
WireConnection;384;0;426;0
WireConnection;384;1;377;0
WireConnection;374;0;178;0
WireConnection;428;0;371;0
WireConnection;428;1;462;0
WireConnection;428;2;437;0
WireConnection;347;0;374;0
WireConnection;458;0;428;0
WireConnection;390;0;384;0
WireConnection;45;0;1;4
WireConnection;0;0;459;0
WireConnection;0;1;372;0
WireConnection;0;2;391;0
WireConnection;0;10;46;0
ASEEND*/
//CHKSM=47277AD1F5C4CDB73AAA9F3564FD83A5284852E3