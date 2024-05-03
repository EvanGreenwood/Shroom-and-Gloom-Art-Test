// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNG/Environment"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.6
		_Ramp("Ramp", 2D) = "white" {}
		[PerRendererData]_MainTex("MainTex", 2D) = "white" {}
		[Toggle(_NOISYFOG_ON)] _NoisyFog("NoisyFog", Float) = 1
		_RampOffset("RampOffset", Float) = 0
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
		#pragma shader_feature_local _USEFOG_ON
		#pragma shader_feature_local _NOISYFOG_ON
		#pragma surface surf Unlit keepalpha noshadow noambient nolightmap  nodynlightmap nodirlightmap vertex:vertexDataFunc 
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

		uniform float4 ShroomLightInnerRadiusAndIntensity[10];
		uniform float4 ShroomLightColors[10];
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 FogColor;
		uniform float FogDistance;
		uniform float FogStart;
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
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 temp_output_3_0 = ( tex2DNode1 * i.vertexColor );
			float4 temp_output_2_0_g64 = FogColor;
			float cameraDepthFade30 = (( i.eyeDepth -_ProjectionParams.y - FogStart ) / FogDistance);
			float temp_output_2_0_g21 = ( saturate( cameraDepthFade30 ) - 1.0 );
			float temp_output_41_0 = sqrt( ( 1.0 - ( temp_output_2_0_g21 * temp_output_2_0_g21 ) ) );
			float temp_output_2_0_g63 = ( 1.0 - temp_output_41_0 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float temp_output_21_0_g62 = 0.5;
			float2 appendResult18_g62 = (float2(( ( _SinTime.y + ase_screenPos.x ) * temp_output_21_0_g62 ) , ( ( _SinTime.x + ase_screenPos.y ) * temp_output_21_0_g62 )));
			float simplePerlin2D14_g62 = snoise( appendResult18_g62*2.0 );
			simplePerlin2D14_g62 = simplePerlin2D14_g62*0.5 + 0.5;
			float2 appendResult13_g62 = (float2(( temp_output_21_0_g62 * ( _SinTime.y + ase_screenPos.x ) ) , ( temp_output_21_0_g62 * ( ase_screenPos.y + _SinTime.x ) )));
			float simplePerlin2D15_g62 = snoise( appendResult13_g62*2.0 );
			simplePerlin2D15_g62 = simplePerlin2D15_g62*0.5 + 0.5;
			float2 appendResult5_g62 = (float2(simplePerlin2D14_g62 , simplePerlin2D15_g62));
			float simplePerlin2D1_g62 = snoise( appendResult5_g62*0.1 );
			simplePerlin2D1_g62 = simplePerlin2D1_g62*0.5 + 0.5;
			float temp_output_21_0_g59 = 0.125;
			float2 appendResult18_g59 = (float2(( ( _SinTime.y + ase_screenPos.x ) * temp_output_21_0_g59 ) , ( ( _SinTime.x + ase_screenPos.y ) * temp_output_21_0_g59 )));
			float simplePerlin2D14_g59 = snoise( appendResult18_g59*2.0 );
			simplePerlin2D14_g59 = simplePerlin2D14_g59*0.5 + 0.5;
			float2 appendResult13_g59 = (float2(( temp_output_21_0_g59 * ( _SinTime.y + ase_screenPos.x ) ) , ( temp_output_21_0_g59 * ( ase_screenPos.y + _SinTime.x ) )));
			float simplePerlin2D15_g59 = snoise( appendResult13_g59*2.0 );
			simplePerlin2D15_g59 = simplePerlin2D15_g59*0.5 + 0.5;
			float2 appendResult5_g59 = (float2(simplePerlin2D14_g59 , simplePerlin2D15_g59));
			float simplePerlin2D1_g59 = snoise( appendResult5_g59*0.2 );
			simplePerlin2D1_g59 = simplePerlin2D1_g59*0.5 + 0.5;
			float temp_output_21_0_g60 = 0.15;
			float2 appendResult18_g60 = (float2(( ( _SinTime.y + ase_screenPos.x ) * temp_output_21_0_g60 ) , ( ( _SinTime.x + ase_screenPos.y ) * temp_output_21_0_g60 )));
			float simplePerlin2D14_g60 = snoise( appendResult18_g60*2.0 );
			simplePerlin2D14_g60 = simplePerlin2D14_g60*0.5 + 0.5;
			float2 appendResult13_g60 = (float2(( temp_output_21_0_g60 * ( _SinTime.y + ase_screenPos.x ) ) , ( temp_output_21_0_g60 * ( ase_screenPos.y + _SinTime.x ) )));
			float simplePerlin2D15_g60 = snoise( appendResult13_g60*2.0 );
			simplePerlin2D15_g60 = simplePerlin2D15_g60*0.5 + 0.5;
			float2 appendResult5_g60 = (float2(simplePerlin2D14_g60 , simplePerlin2D15_g60));
			float simplePerlin2D1_g60 = snoise( appendResult5_g60*0.5 );
			simplePerlin2D1_g60 = simplePerlin2D1_g60*0.5 + 0.5;
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
			float lerpResult135 = lerp( ( 1.0 * ( ( ( ( simplePerlin2D1_g62 + simplePerlin2D1_g59 + simplePerlin2D1_g60 + simplePerlin2D1_g61 ) / 4.0 ) - 0.5 ) * ( 1.0 - Depth134 ) ) ) , 0.0 , ( Depth134 + 1.0 ));
			#ifdef _NOISYFOG_ON
				float staticSwitch169 = lerpResult135;
			#else
				float staticSwitch169 = 0.0;
			#endif
			float4 lerpResult13 = lerp( temp_output_3_0 , ( temp_output_2_0_g64 * temp_output_2_0_g64 ) , ( ( 1.0 - ( temp_output_2_0_g63 * temp_output_2_0_g63 ) ) + staticSwitch169 ));
			#ifdef _USEFOG_ON
				float4 staticSwitch280 = lerpResult13;
			#else
				float4 staticSwitch280 = temp_output_3_0;
			#endif
			int ShroomLightIndex192 = 0;
			float3 temp_output_226_0 = (ShroomLightPositions[ShroomLightIndex192]).xyz;
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult209 = normalize( ( temp_output_226_0 - ase_worldPos ) );
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 tex2DNode178 = UnpackNormal( tex2Dlod( _Normal, float4( uv_Normal, 0, _NormalMip) ) );
			float3 normalizeResult5_g114 = normalize( (WorldNormalVector( i , tex2DNode178 )) );
			float dotResult15_g114 = dot( normalizeResult209 , normalizeResult5_g114 );
			float2 appendResult106_g114 = (float2(( (0.0 + (dotResult15_g114 - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) + _RampOffset ) , 0.5));
			float4 tex2DNode102_g114 = tex2D( _Ramp, appendResult106_g114 );
			#ifdef _DISPLAYNORMALS_ON
				float4 staticSwitch265 = float4( tex2DNode178 , 0.0 );
			#else
				float4 staticSwitch265 = ( staticSwitch280 * tex2DNode102_g114 );
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
Node;AmplifyShaderEditor.CommentaryNode;52;-2720,-304;Inherit;False;1716;435;Applies fog based on camera depth;13;13;14;36;37;29;30;35;41;42;51;28;63;134;Apply Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2704,16;Inherit;False;Global;FogStart;FogStart;2;0;Create;True;0;0;0;False;0;False;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-2704,-64;Inherit;False;Global;FogDistance;FogDistance;2;0;Create;True;0;0;0;False;0;False;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;173;-3552,448;Inherit;False;2228;819; Attempt to add some dynamisim to fog. Not good enough;29;156;158;160;152;162;163;164;165;155;157;159;151;128;130;129;62;136;61;138;64;140;65;171;137;142;172;135;170;169;Noisy Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.CameraDepthFade;30;-2448,-48;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;37;-2160,-80;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-3472,800;Inherit;False;Constant;_Float4;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;-3488,976;Inherit;False;Constant;_Float5;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-3504,1152;Inherit;False;Constant;_Float6;Float 0;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-3504,608;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-3504,496;Inherit;False;Constant;_Float7;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-3504,704;Inherit;False;Constant;_Float8;Float 0;2;0;Create;True;0;0;0;False;0;False;0.125;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-3488,896;Inherit;False;Constant;_Float9;Float 0;2;0;Create;True;0;0;0;False;0;False;0.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-3504,1072;Inherit;False;Constant;_Float10;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;41;-2016,-64;Inherit;False;Easing OutCirc ShaderFunction;-1;;20;181ee7db325895443a774b654a9b8500;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;155;-3264,736;Inherit;False;ScreenNoise;-1;;59;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;157;-3280,912;Inherit;False;ScreenNoise;-1;;60;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;159;-3296,1088;Inherit;False;ScreenNoise;-1;;61;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;151;-3264,576;Inherit;False;ScreenNoise;-1;;62;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;128;-2768,560;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-2800,752;Inherit;False;Constant;_Float16;Float 16;2;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-1552,32;Inherit;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;129;-2640,624;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-2480,816;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;136;-2560,1072;Inherit;False;134;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-2384,608;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;138;-2368,960;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;191;-1088,-1104;Inherit;False;Constant;_Int2;Int 1;5;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-2256,784;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;-2112,976;Inherit;False;134;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-2208,576;Inherit;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-2080,1072;Inherit;False;Constant;_Float13;Float 13;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-944,-1104;Inherit;False;ShroomLightIndex;-1;True;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.OneMinusNode;42;-1696,-64;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-1984,864;Inherit;False;Constant;_Float19;Float 19;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;-2064,672;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;172;-1824,1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;197;-1296,-672;Inherit;False;Constant;_ShroomLightArraySize;ShroomLightArraySize;5;0;Create;True;0;0;0;False;0;False;10;0;False;0;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;188;-944,-800;Inherit;False;192;ShroomLightIndex;1;0;OBJECT;;False;1;INT;0
Node;AmplifyShaderEditor.CommentaryNode;50;-2496,-1168;Inherit;False;868;563;Comment;4;1;45;3;2;Tex And Vertex Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;35;-1552,-64;Inherit;False;Square;-1;;63;fea980a1f68019543b2cd91d506986e8;0;1;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;135;-1776,800;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-1728,656;Inherit;False;Constant;_Float12;Float 12;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GlobalArrayNode;182;-704,-800;Inherit;False;ShroomLightPositions;0;10;2;False;False;0;1;True;Object;-1;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;1;-2416,-1104;Inherit;True;Property;_MainTex;MainTex;4;1;[PerRendererData];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;2;-2112,-816;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;51;-1408,-64;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;-1744,-240;Inherit;False;Global;FogColor;FogColor;2;0;Create;True;0;0;0;False;0;False;0.1887822,0.1643645,0.509434,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;169;-1568,672;Inherit;False;Property;_NoisyFog;NoisyFog;5;0;Create;True;0;0;0;False;0;False;0;1;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;226;-480,-752;Inherit;False;True;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;36;-1408,-192;Inherit;False;Square;-1;;64;fea980a1f68019543b2cd91d506986e8;0;1;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-1216,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-1776,-912;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;207;-432,-640;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;13;-1184,-256;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;208;-192,-832;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;306;-736,-16;Inherit;False;Property;_NormalMip;NormalMip;7;0;Create;True;0;0;0;False;0;False;0;1.86;0;9;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;280;-912,-304;Inherit;False;Property;_UseFog;Use Fog;8;0;Create;True;0;0;0;False;0;False;0;1;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;209;0,-736;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;304;-240,-416;Inherit;False;Property;_RampOffset;RampOffset;6;0;Create;True;0;0;0;False;0;False;0;-0.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;178;-376,-112;Inherit;True;Property;_Normal;Normal;9;1;[Normal];Create;True;0;0;0;False;0;False;178;None;None;True;0;False;bump;Auto;True;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-2112,-912;Inherit;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;326;112,-400;Inherit;False;ShroomLightModel;1;;114;9ba531354399e4286b6b6809cf677f13;0;4;111;FLOAT;0;False;21;FLOAT3;0,0,0;False;10;COLOR;0.5754717,0.5754717,0.5754717,1;False;7;FLOAT3;0,0,0;False;1;COLOR;17
Node;AmplifyShaderEditor.PosFromTransformMatrix;283;-688,-1184;Inherit;False;1;0;FLOAT4x4;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;285;-448,-1072;Inherit;False;True;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;284;-192,-1056;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;286;-16,-1024;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GlobalArrayNode;185;-944,-432;Inherit;False;ShroomLightInnerRadiusAndIntensity;0;10;2;False;False;0;1;True;Object;-1;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;223;-608,-432;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;190;-1008,-544;Inherit;False;192;ShroomLightIndex;1;0;OBJECT;;False;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;189;-928,-656;Inherit;False;192;ShroomLightIndex;1;0;OBJECT;;False;1;INT;0
Node;AmplifyShaderEditor.GlobalArrayNode;183;-704,-624;Inherit;False;ShroomLightColors;0;10;1;False;False;0;1;True;Object;-1;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;202;-208,-1264;Inherit;False;Constant;_IndirectColor;IndirectColor;5;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;203;32,-1232;Inherit;False;Constant;_Attenuation;Attenuation;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;221;-192,-560;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;640,48;Inherit;False;45;MainTexAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;265;304,-144;Inherit;False;Property;_DisplayNormals;Display Normals;10;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;864,-304;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;SNG/Environment;False;False;False;False;True;False;True;True;True;False;False;False;False;False;True;False;False;False;False;False;False;Off;1;False;;1;False;;False;0;False;;0;False;;False;0;Masked;0.6;True;False;0;False;TransparentCutout;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;30;0;28;0
WireConnection;30;1;29;0
WireConnection;37;0;30;0
WireConnection;41;1;37;0
WireConnection;155;21;163;0
WireConnection;155;20;156;0
WireConnection;157;21;164;0
WireConnection;157;20;158;0
WireConnection;159;21;165;0
WireConnection;159;20;160;0
WireConnection;151;21;162;0
WireConnection;151;20;152;0
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
WireConnection;192;0;191;0
WireConnection;42;0;41;0
WireConnection;142;0;65;0
WireConnection;142;1;64;0
WireConnection;172;0;140;0
WireConnection;172;1;171;0
WireConnection;35;2;42;0
WireConnection;135;0;142;0
WireConnection;135;1;137;0
WireConnection;135;2;172;0
WireConnection;182;0;188;0
WireConnection;182;1;197;0
WireConnection;51;0;35;0
WireConnection;169;1;170;0
WireConnection;169;0;135;0
WireConnection;226;0;182;0
WireConnection;36;2;14;0
WireConnection;63;0;51;0
WireConnection;63;1;169;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;13;0;3;0
WireConnection;13;1;36;0
WireConnection;13;2;63;0
WireConnection;208;0;226;0
WireConnection;208;1;207;0
WireConnection;280;1;3;0
WireConnection;280;0;13;0
WireConnection;209;0;208;0
WireConnection;178;2;306;0
WireConnection;45;0;1;4
WireConnection;326;111;304;0
WireConnection;326;21;209;0
WireConnection;326;10;280;0
WireConnection;326;7;178;0
WireConnection;285;0;283;0
WireConnection;284;0;285;0
WireConnection;284;1;226;0
WireConnection;286;0;284;0
WireConnection;185;0;190;0
WireConnection;185;1;197;0
WireConnection;223;0;185;0
WireConnection;183;0;189;0
WireConnection;183;1;197;0
WireConnection;221;0;207;0
WireConnection;221;1;226;0
WireConnection;265;1;326;17
WireConnection;265;0;178;0
WireConnection;0;2;265;0
WireConnection;0;10;46;0
ASEEND*/
//CHKSM=CCA6E560246260401E97BF8C463E93C6F15302BA