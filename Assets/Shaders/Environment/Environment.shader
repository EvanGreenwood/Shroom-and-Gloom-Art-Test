// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNG/Environment"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.6
		_Ramp("Ramp", 2D) = "white" {}
		_RampOffset("RampOffset", Float) = 0
		_FogIntensityCoefficient("FogIntensityCoefficient", Range( 0 , 1)) = 1
		[PerRendererData]_MainTex("MainTex", 2D) = "white" {}
		[Toggle(_NOISYFOG_ON)] _NoisyFog("NoisyFog", Float) = 1
		_NormalMip("NormalMip", Range( 0 , 9)) = 0
		[Toggle(_USEFOG_ON)] _UseFog("Use Fog", Float) = 1
		[Normal]_Normal("Normal", 2D) = "bump" {}
		[Toggle(_DISPLAYNORMALS_ON)] _DisplayNormals("Display Normals", Float) = 0
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
		#pragma shader_feature_local _DISPLAYNORMALS_ON
		#pragma shader_feature _SCENE_VIEW
		#pragma shader_feature_local _USEFOG_ON
		#pragma shader_feature_local _NOISYFOG_ON
		#pragma surface surf Unlit keepalpha noshadow noambient nolightmap  nodynlightmap nodirlightmap nofog vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float eyeDepth;
			float4 screenPos;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float4 ShroomLightRadiusAndIntensity[10];
		uniform float4 ShroomLightColors[10];
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 FogColor;
		uniform float FogDistance;
		uniform float FogStart;
		uniform float _FogIntensityCoefficient;
		uniform sampler2D _Ramp;
		uniform float4 ShroomLightPositions[10];
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _NormalMip;
		uniform float _RampOffset;
		uniform float _Cutoff = 0.6;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			int temp_output_17_0_g81 = 2;
			float4 break129_g82 = ShroomLightRadiusAndIntensity[temp_output_17_0_g81];
			float Intensity132_g82 = break129_g82.z;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 temp_output_3_0 = ( tex2DNode1 * i.vertexColor );
			float4 temp_output_2_0_g70 = FogColor;
			float cameraDepthFade30 = (( i.eyeDepth -_ProjectionParams.y - FogStart ) / FogDistance);
			float temp_output_2_0_g68 = ( saturate( ( cameraDepthFade30 * _FogIntensityCoefficient ) ) - 1.0 );
			float temp_output_41_0 = sqrt( ( 1.0 - ( temp_output_2_0_g68 * temp_output_2_0_g68 ) ) );
			float temp_output_2_0_g69 = ( 1.0 - temp_output_41_0 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float temp_output_21_0_g66 = 0.5;
			float2 appendResult18_g66 = (float2(( ( _SinTime.y + ase_screenPos.x ) * temp_output_21_0_g66 ) , ( ( _SinTime.x + ase_screenPos.y ) * temp_output_21_0_g66 )));
			float simplePerlin2D14_g66 = snoise( appendResult18_g66*2.0 );
			simplePerlin2D14_g66 = simplePerlin2D14_g66*0.5 + 0.5;
			float2 appendResult13_g66 = (float2(( temp_output_21_0_g66 * ( _SinTime.y + ase_screenPos.x ) ) , ( temp_output_21_0_g66 * ( ase_screenPos.y + _SinTime.x ) )));
			float simplePerlin2D15_g66 = snoise( appendResult13_g66*2.0 );
			simplePerlin2D15_g66 = simplePerlin2D15_g66*0.5 + 0.5;
			float2 appendResult5_g66 = (float2(simplePerlin2D14_g66 , simplePerlin2D15_g66));
			float simplePerlin2D1_g66 = snoise( appendResult5_g66*0.1 );
			simplePerlin2D1_g66 = simplePerlin2D1_g66*0.5 + 0.5;
			float temp_output_21_0_g64 = 0.125;
			float2 appendResult18_g64 = (float2(( ( _SinTime.y + ase_screenPos.x ) * temp_output_21_0_g64 ) , ( ( _SinTime.x + ase_screenPos.y ) * temp_output_21_0_g64 )));
			float simplePerlin2D14_g64 = snoise( appendResult18_g64*2.0 );
			simplePerlin2D14_g64 = simplePerlin2D14_g64*0.5 + 0.5;
			float2 appendResult13_g64 = (float2(( temp_output_21_0_g64 * ( _SinTime.y + ase_screenPos.x ) ) , ( temp_output_21_0_g64 * ( ase_screenPos.y + _SinTime.x ) )));
			float simplePerlin2D15_g64 = snoise( appendResult13_g64*2.0 );
			simplePerlin2D15_g64 = simplePerlin2D15_g64*0.5 + 0.5;
			float2 appendResult5_g64 = (float2(simplePerlin2D14_g64 , simplePerlin2D15_g64));
			float simplePerlin2D1_g64 = snoise( appendResult5_g64*0.2 );
			simplePerlin2D1_g64 = simplePerlin2D1_g64*0.5 + 0.5;
			float temp_output_21_0_g65 = 0.15;
			float2 appendResult18_g65 = (float2(( ( _SinTime.y + ase_screenPos.x ) * temp_output_21_0_g65 ) , ( ( _SinTime.x + ase_screenPos.y ) * temp_output_21_0_g65 )));
			float simplePerlin2D14_g65 = snoise( appendResult18_g65*2.0 );
			simplePerlin2D14_g65 = simplePerlin2D14_g65*0.5 + 0.5;
			float2 appendResult13_g65 = (float2(( temp_output_21_0_g65 * ( _SinTime.y + ase_screenPos.x ) ) , ( temp_output_21_0_g65 * ( ase_screenPos.y + _SinTime.x ) )));
			float simplePerlin2D15_g65 = snoise( appendResult13_g65*2.0 );
			simplePerlin2D15_g65 = simplePerlin2D15_g65*0.5 + 0.5;
			float2 appendResult5_g65 = (float2(simplePerlin2D14_g65 , simplePerlin2D15_g65));
			float simplePerlin2D1_g65 = snoise( appendResult5_g65*0.5 );
			simplePerlin2D1_g65 = simplePerlin2D1_g65*0.5 + 0.5;
			float temp_output_21_0_g61 = 0.2;
			float2 appendResult18_g61 = (float2(( ( _SinTime.y + ase_screenPos.x ) * temp_output_21_0_g61 ) , ( ( _SinTime.x + ase_screenPos.y ) * temp_output_21_0_g61 )));
			float simplePerlin2D14_g61 = snoise( appendResult18_g61*2.0 );
			simplePerlin2D14_g61 = simplePerlin2D14_g61*0.5 + 0.5;
			float2 appendResult13_g61 = (float2(( temp_output_21_0_g61 * ( _SinTime.y + ase_screenPos.x ) ) , ( temp_output_21_0_g61 * ( ase_screenPos.y + _SinTime.x ) )));
			float simplePerlin2D15_g61 = snoise( appendResult13_g61*2.0 );
			simplePerlin2D15_g61 = simplePerlin2D15_g61*0.5 + 0.5;
			float2 appendResult5_g61 = (float2(simplePerlin2D14_g61 , simplePerlin2D15_g61));
			float simplePerlin2D1_g61 = snoise( appendResult5_g61*1.0 );
			simplePerlin2D1_g61 = simplePerlin2D1_g61*0.5 + 0.5;
			float Depth134 = temp_output_41_0;
			float lerpResult135 = lerp( ( 1.0 * ( ( ( ( simplePerlin2D1_g66 + simplePerlin2D1_g64 + simplePerlin2D1_g65 + simplePerlin2D1_g61 ) / 4.0 ) - 0.5 ) * ( 1.0 - Depth134 ) ) ) , 0.0 , ( Depth134 + 1.0 ));
			#ifdef _NOISYFOG_ON
				float staticSwitch169 = lerpResult135;
			#else
				float staticSwitch169 = 0.0;
			#endif
			float4 lerpResult13 = lerp( temp_output_3_0 , ( temp_output_2_0_g70 * temp_output_2_0_g70 ) , ( ( 1.0 - ( temp_output_2_0_g69 * temp_output_2_0_g69 ) ) + staticSwitch169 ));
			#ifdef _USEFOG_ON
				float4 staticSwitch280 = lerpResult13;
			#else
				float4 staticSwitch280 = temp_output_3_0;
			#endif
			#ifdef _SCENE_VIEW
				float4 staticSwitch370 = temp_output_3_0;
			#else
				float4 staticSwitch370 = staticSwitch280;
			#endif
			float4 Diffuse350 = staticSwitch370;
			float3 temp_output_4_0_g81 = (ShroomLightPositions[temp_output_17_0_g81]).xyz;
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult8_g81 = normalize( ( temp_output_4_0_g81 - ase_worldPos ) );
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 NormalSample347 = UnpackNormal( tex2Dlod( _Normal, float4( uv_Normal, 0, _NormalMip) ) );
			float3 normalizeResult5_g82 = normalize( (WorldNormalVector( i , NormalSample347 )) );
			float dotResult15_g82 = dot( normalizeResult8_g81 , normalizeResult5_g82 );
			float InnerRadius130_g82 = break129_g82.x;
			float OuterRadius131_g82 = break129_g82.y;
			float2 appendResult106_g82 = (float2(( (0.0 + (dotResult15_g82 - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) + ( _RampOffset + ( 1.0 - (0.0 + (distance( ase_worldPos , temp_output_4_0_g81 ) - InnerRadius130_g82) * (1.0 - 0.0) / (OuterRadius131_g82 - InnerRadius130_g82)) ) ) ) , 0.5));
			int temp_output_17_0_g79 = 1;
			float4 break129_g80 = ShroomLightRadiusAndIntensity[temp_output_17_0_g79];
			float Intensity132_g80 = break129_g80.z;
			float3 temp_output_4_0_g79 = (ShroomLightPositions[temp_output_17_0_g79]).xyz;
			float3 normalizeResult8_g79 = normalize( ( temp_output_4_0_g79 - ase_worldPos ) );
			float3 normalizeResult5_g80 = normalize( (WorldNormalVector( i , NormalSample347 )) );
			float dotResult15_g80 = dot( normalizeResult8_g79 , normalizeResult5_g80 );
			float InnerRadius130_g80 = break129_g80.x;
			float OuterRadius131_g80 = break129_g80.y;
			float2 appendResult106_g80 = (float2(( (0.0 + (dotResult15_g80 - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) + ( _RampOffset + ( 1.0 - (0.0 + (distance( ase_worldPos , temp_output_4_0_g79 ) - InnerRadius130_g80) * (1.0 - 0.0) / (OuterRadius131_g80 - InnerRadius130_g80)) ) ) ) , 0.5));
			int temp_output_17_0_g75 = 0;
			float4 break129_g76 = ShroomLightRadiusAndIntensity[temp_output_17_0_g75];
			float Intensity132_g76 = break129_g76.z;
			float3 temp_output_4_0_g75 = (ShroomLightPositions[temp_output_17_0_g75]).xyz;
			float3 normalizeResult8_g75 = normalize( ( temp_output_4_0_g75 - ase_worldPos ) );
			float3 normalizeResult5_g76 = normalize( (WorldNormalVector( i , NormalSample347 )) );
			float dotResult15_g76 = dot( normalizeResult8_g75 , normalizeResult5_g76 );
			float InnerRadius130_g76 = break129_g76.x;
			float OuterRadius131_g76 = break129_g76.y;
			float2 appendResult106_g76 = (float2(( (0.0 + (dotResult15_g76 - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) + ( _RampOffset + ( 1.0 - (0.0 + (distance( ase_worldPos , temp_output_4_0_g75 ) - InnerRadius130_g76) * (1.0 - 0.0) / (OuterRadius131_g76 - InnerRadius130_g76)) ) ) ) , 0.5));
			#ifdef _DISPLAYNORMALS_ON
				float4 staticSwitch265 = float4( NormalSample347 , 0.0 );
			#else
				float4 staticSwitch265 = ( ( ( Intensity132_g82 * ( ShroomLightColors[temp_output_17_0_g81] * Diffuse350 ) ) * tex2D( _Ramp, appendResult106_g82 ) ) + ( ( ( Intensity132_g80 * ( ShroomLightColors[temp_output_17_0_g79] * Diffuse350 ) ) * tex2D( _Ramp, appendResult106_g80 ) ) + ( ( Intensity132_g76 * ( ShroomLightColors[temp_output_17_0_g75] * Diffuse350 ) ) * tex2D( _Ramp, appendResult106_g76 ) ) ) );
			#endif
			o.Emission = staticSwitch265.rgb;
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
Node;AmplifyShaderEditor.CommentaryNode;52;-4992,192;Inherit;False;1726.913;467.74;Applies fog based on camera depth;15;134;37;41;368;367;28;30;36;14;13;63;51;35;42;29;Apply Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-4976,512;Inherit;False;Global;FogStart;FogStart;2;0;Create;True;0;0;0;False;0;False;1.5;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-4976,432;Inherit;False;Global;FogDistance;FogDistance;2;0;Create;True;0;0;0;False;0;False;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;30;-4720,448;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;367;-4752,576;Inherit;False;Property;_FogIntensityCoefficient;FogIntensityCoefficient;5;0;Create;True;0;0;0;False;0;False;1;20;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;173;-5856,1008;Inherit;False;2228;819; Attempt to add some dynamisim to fog. Not good enough;29;156;158;160;152;162;163;164;165;155;157;159;151;128;130;129;62;136;61;138;64;140;65;171;137;142;172;135;170;169;Noisy Fog (needs work bad atm);1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;368;-4464,496;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-5808,1712;Inherit;False;Constant;_Float6;Float 0;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-5808,1632;Inherit;False;Constant;_Float10;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-5776,1360;Inherit;False;Constant;_Float4;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;-5792,1536;Inherit;False;Constant;_Float5;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-5808,1168;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-5808,1056;Inherit;False;Constant;_Float7;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-5808,1264;Inherit;False;Constant;_Float8;Float 0;2;0;Create;True;0;0;0;False;0;False;0.125;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-5792,1456;Inherit;False;Constant;_Float9;Float 0;2;0;Create;True;0;0;0;False;0;False;0.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;37;-4304,416;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;159;-5600,1648;Inherit;False;ScreenNoise;-1;;61;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;155;-5568,1296;Inherit;False;ScreenNoise;-1;;64;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;157;-5584,1472;Inherit;False;ScreenNoise;-1;;65;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;151;-5568,1136;Inherit;False;ScreenNoise;-1;;66;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;41;-4144,432;Inherit;False;EasingOutCirc;-1;;67;181ee7db325895443a774b654a9b8500;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;128;-5072,1120;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-5104,1312;Inherit;False;Constant;_Float16;Float 16;2;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-3984,560;Inherit;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;136;-4864,1632;Inherit;False;134;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;129;-4944,1184;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-4784,1376;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-4688,1168;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;138;-4672,1520;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-4560,1344;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;-4416,1536;Inherit;False;134;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-4512,1136;Inherit;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-4384,1632;Inherit;False;Constant;_Float13;Float 13;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;42;-3968,432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;172;-4128,1584;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-4288,1424;Inherit;False;Constant;_Float19;Float 19;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;-4368,1232;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;50;-4880,-704;Inherit;False;868;563;Comment;4;1;45;3;2;Tex And Vertex Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;35;-3824,432;Inherit;False;Square;-1;;69;fea980a1f68019543b2cd91d506986e8;0;1;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;135;-4080,1360;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-4032,1216;Inherit;False;Constant;_Float12;Float 12;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;51;-3680,432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-4800,-640;Inherit;True;Property;_MainTex;MainTex;6;1;[PerRendererData];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;2;-4496,-352;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;-4720,240;Inherit;False;Global;FogColor;FogColor;2;0;Create;True;0;0;0;False;0;False;0.1887822,0.1643645,0.509434,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;169;-3872,1232;Inherit;False;Property;_NoisyFog;NoisyFog;7;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-3488,496;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-4160,-448;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;36;-4384,288;Inherit;False;Square;-1;;70;fea980a1f68019543b2cd91d506986e8;0;1;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;329;-2768,1408;Inherit;False;976.0073;302.9471;Comment;3;347;178;306;Mip Smoothed Normal Sample;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;13;-3456,240;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;306;-2720,1536;Inherit;False;Property;_NormalMip;NormalMip;8;0;Create;True;0;0;0;False;0;False;0;2;0;9;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;280;-2992,208;Inherit;False;Property;_UseFog;Use Fog;9;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;178;-2416,1456;Inherit;True;Property;_Normal;Normal;10;1;[Normal];Create;True;0;0;0;False;0;False;178;None;None;True;0;False;bump;Auto;True;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;370;-2656,160;Inherit;False;Property;_SCENE_VIEW;SCENE_VIEW;10;0;Create;True;0;0;0;False;0;False;0;1;1;False;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;328;-2032,-1616;Inherit;False;2093.142;999.1426;Shroom Lights;11;349;351;353;354;355;356;358;359;360;361;363;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;347;-2016,1488;Inherit;False;NormalSample;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;350;-2336,160;Inherit;False;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;351;-1312,-704;Inherit;False;350;Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;349;-1344,-784;Inherit;False;347;NormalSample;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IntNode;353;-1312,-864;Inherit;False;Constant;_LightIndex0;LightIndex0;9;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;354;-864,-912;Inherit;False;350;Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;355;-896,-992;Inherit;False;347;NormalSample;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IntNode;356;-864,-1072;Inherit;False;Constant;_LightIndex1;LightIndex0;9;0;Create;True;0;0;0;False;0;False;1;0;False;0;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;359;-864,-1152;Inherit;False;350;Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;360;-896,-1232;Inherit;False;347;NormalSample;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IntNode;361;-864,-1312;Inherit;False;Constant;_LightIndex2;LightIndex0;9;0;Create;True;0;0;0;False;0;False;2;0;False;0;1;INT;0
Node;AmplifyShaderEditor.FunctionNode;364;-1056,-832;Inherit;False;SingleLightSample;1;;75;484ad8942c0a84b6fa71c3cc3a86f242;0;3;17;INT;0;False;15;FLOAT3;0,0,0;False;16;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;366;-608,-1040;Inherit;False;SingleLightSample;1;;79;484ad8942c0a84b6fa71c3cc3a86f242;0;3;17;INT;0;False;15;FLOAT3;0,0,0;False;16;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;358;-371.1859,-805.1718;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;365;-608,-1280;Inherit;False;SingleLightSample;1;;81;484ad8942c0a84b6fa71c3cc3a86f242;0;3;17;INT;0;False;15;FLOAT3;0,0,0;False;16;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-4496,-448;Inherit;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;363;-240,-1008;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;352;192,-240;Inherit;False;347;NormalSample;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;265;480,-272;Inherit;False;Property;_DisplayNormals;Display Normals;11;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;544,-32;Inherit;False;45;MainTexAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;864,-304;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;SNG/Environment;False;False;False;False;True;False;True;True;True;True;False;False;False;False;True;False;False;False;False;False;False;Off;1;False;;1;False;;False;0;False;;0;False;;False;0;Masked;0.6;True;False;0;False;TransparentCutout;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;30;0;28;0
WireConnection;30;1;29;0
WireConnection;368;0;30;0
WireConnection;368;1;367;0
WireConnection;37;0;368;0
WireConnection;159;21;165;0
WireConnection;159;20;160;0
WireConnection;155;21;163;0
WireConnection;155;20;156;0
WireConnection;157;21;164;0
WireConnection;157;20;158;0
WireConnection;151;21;162;0
WireConnection;151;20;152;0
WireConnection;41;1;37;0
WireConnection;128;0;151;0
WireConnection;128;1;155;0
WireConnection;128;2;157;0
WireConnection;128;3;159;0
WireConnection;134;0;41;0
WireConnection;129;0;128;0
WireConnection;129;1;130;0
WireConnection;61;0;129;0
WireConnection;61;1;62;0
WireConnection;138;0;136;0
WireConnection;64;0;61;0
WireConnection;64;1;138;0
WireConnection;42;0;41;0
WireConnection;172;0;140;0
WireConnection;172;1;171;0
WireConnection;142;0;65;0
WireConnection;142;1;64;0
WireConnection;35;2;42;0
WireConnection;135;0;142;0
WireConnection;135;1;137;0
WireConnection;135;2;172;0
WireConnection;51;0;35;0
WireConnection;169;1;170;0
WireConnection;169;0;135;0
WireConnection;63;0;51;0
WireConnection;63;1;169;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;36;2;14;0
WireConnection;13;0;3;0
WireConnection;13;1;36;0
WireConnection;13;2;63;0
WireConnection;280;1;3;0
WireConnection;280;0;13;0
WireConnection;178;2;306;0
WireConnection;370;1;280;0
WireConnection;370;0;3;0
WireConnection;347;0;178;0
WireConnection;350;0;370;0
WireConnection;364;17;353;0
WireConnection;364;15;349;0
WireConnection;364;16;351;0
WireConnection;366;17;356;0
WireConnection;366;15;355;0
WireConnection;366;16;354;0
WireConnection;358;0;366;0
WireConnection;358;1;364;0
WireConnection;365;17;361;0
WireConnection;365;15;360;0
WireConnection;365;16;359;0
WireConnection;45;0;1;4
WireConnection;363;0;365;0
WireConnection;363;1;358;0
WireConnection;265;1;363;0
WireConnection;265;0;352;0
WireConnection;0;2;265;0
WireConnection;0;10;46;0
ASEEND*/
//CHKSM=8CC3DEEE651BACB512EBB2C6282347ED0C52FEDE