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
		uniform float4 ShroomLightRadiusAndIntensity[10];
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
			float3 normalizeResult5_g2 = normalize( (WorldNormalVector( i , tex2DNode178 )) );
			float dotResult15_g2 = dot( normalizeResult209 , normalizeResult5_g2 );
			float4 break129_g2 = ShroomLightRadiusAndIntensity[ShroomLightIndex192];
			float InnerRadius130_g2 = break129_g2.x;
			float OuterRadius131_g2 = break129_g2.y;
			float2 appendResult106_g2 = (float2(( (0.0 + (dotResult15_g2 - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) + ( _RampOffset + ( 1.0 - (0.0 + (distance( ase_worldPos , temp_output_226_0 ) - InnerRadius130_g2) * (1.0 - 0.0) / (OuterRadius131_g2 - InnerRadius130_g2)) ) ) ) , 0.5));
			#ifdef _DISPLAYNORMALS_ON
				float4 staticSwitch265 = float4( tex2DNode178 , 0.0 );
			#else
				float4 staticSwitch265 = ( staticSwitch280 * tex2D( _Ramp, appendResult106_g2 ) );
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
Node;AmplifyShaderEditor.CommentaryNode;52;-3312,-368;Inherit;False;1716;435;Applies fog based on camera depth;13;13;14;36;37;29;30;35;41;42;51;28;63;134;Apply Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-3296,-48;Inherit;False;Global;FogStart;FogStart;2;0;Create;True;0;0;0;False;0;False;1.5;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-3296,-128;Inherit;False;Global;FogDistance;FogDistance;2;0;Create;True;0;0;0;False;0;False;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;173;-3824,368;Inherit;False;2228;819; Attempt to add some dynamisim to fog. Not good enough;29;156;158;160;152;162;163;164;165;155;157;159;151;128;130;129;62;136;61;138;64;140;65;171;137;142;172;135;170;169;Noisy Fog (needs work bad atm);1,1,1,1;0;0
Node;AmplifyShaderEditor.CameraDepthFade;30;-3040,-112;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;37;-2752,-144;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-3744,720;Inherit;False;Constant;_Float4;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;-3760,896;Inherit;False;Constant;_Float5;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-3776,1072;Inherit;False;Constant;_Float6;Float 0;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-3776,528;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-3776,416;Inherit;False;Constant;_Float7;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-3776,624;Inherit;False;Constant;_Float8;Float 0;2;0;Create;True;0;0;0;False;0;False;0.125;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-3760,816;Inherit;False;Constant;_Float9;Float 0;2;0;Create;True;0;0;0;False;0;False;0.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-3776,992;Inherit;False;Constant;_Float10;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;41;-2608,-128;Inherit;False;EasingOutCirc;-1;;20;181ee7db325895443a774b654a9b8500;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;155;-3536,656;Inherit;False;ScreenNoise;-1;;59;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;157;-3552,832;Inherit;False;ScreenNoise;-1;;60;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;159;-3568,1008;Inherit;False;ScreenNoise;-1;;61;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;151;-3536,496;Inherit;False;ScreenNoise;-1;;62;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;128;-3040,480;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-3072,672;Inherit;False;Constant;_Float16;Float 16;2;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-2144,-32;Inherit;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;328;-2032,-1616;Inherit;False;2093.142;999.1426;Shroom Lights;14;185;304;221;183;189;190;209;208;207;226;182;188;197;327;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;129;-2912,544;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-2752,736;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;136;-2832,992;Inherit;False;134;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;327;-1984,-1552;Inherit;False;431.5144;149.193;TODO: add loop for multiple lights;2;192;191;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-2656,528;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;138;-2640,880;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-2528,704;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;-2384,896;Inherit;False;134;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-2480,496;Inherit;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-2352,992;Inherit;False;Constant;_Float13;Float 13;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;191;-1952,-1488;Inherit;False;Constant;_Int2;Int 1;5;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.OneMinusNode;42;-2288,-128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-2256,784;Inherit;False;Constant;_Float19;Float 19;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;-2336,592;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;172;-2096,944;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-1760,-1488;Inherit;False;ShroomLightIndex;-1;True;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.CommentaryNode;50;-3584,-1088;Inherit;False;868;563;Comment;4;1;45;3;2;Tex And Vertex Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;35;-2144,-128;Inherit;False;Square;-1;;63;fea980a1f68019543b2cd91d506986e8;0;1;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;135;-2048,720;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-2000,576;Inherit;False;Constant;_Float12;Float 12;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;197;-1872,-1360;Inherit;False;Constant;_ShroomLightArraySize;ShroomLightArraySize;5;0;Create;True;0;0;0;False;0;False;10;0;False;0;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;188;-1520,-1488;Inherit;False;192;ShroomLightIndex;1;0;OBJECT;;False;1;INT;0
Node;AmplifyShaderEditor.SamplerNode;1;-3504,-1024;Inherit;True;Property;_MainTex;MainTex;3;1;[PerRendererData];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;2;-3200,-736;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;51;-2000,-128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;-2336,-304;Inherit;False;Global;FogColor;FogColor;2;0;Create;True;0;0;0;False;0;False;0.1887822,0.1643645,0.509434,1;0.1226415,0.1226415,0.1226415,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;169;-1840,592;Inherit;False;Property;_NoisyFog;NoisyFog;4;0;Create;True;0;0;0;False;0;False;0;1;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GlobalArrayNode;182;-1280,-1488;Inherit;False;ShroomLightPositions;0;10;2;False;False;0;1;True;Object;-1;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;329;-930,-34;Inherit;False;667;277;Comment;2;178;306;Normal Sample (MIP SMOOTHED);1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;36;-2000,-256;Inherit;False;Square;-1;;64;fea980a1f68019543b2cd91d506986e8;0;1;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-1808,-64;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-2864,-832;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;226;-1056,-1440;Inherit;False;True;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;207;-1008,-1328;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;13;-1776,-320;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;208;-768,-1520;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;306;-880,96;Inherit;False;Property;_NormalMip;NormalMip;6;0;Create;True;0;0;0;False;0;False;0;2;0;9;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;190;-1392,-1168;Inherit;False;192;ShroomLightIndex;1;0;OBJECT;;False;1;INT;0
Node;AmplifyShaderEditor.NormalizeNode;209;-576,-1424;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;280;-1440,-464;Inherit;False;Property;_UseFog;Use Fog;7;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;178;-576,16;Inherit;True;Property;_Normal;Normal;8;1;[Normal];Create;True;0;0;0;False;0;False;178;None;None;True;0;False;bump;Auto;True;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;304;-144,-1152;Inherit;False;Property;_RampOffset;RampOffset;5;0;Create;True;0;0;0;False;0;False;0;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;221;-768,-1248;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GlobalArrayNode;185;-1168,-1056;Inherit;False;ShroomLightRadiusAndIntensity;0;10;2;False;False;0;1;True;Object;-1;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-3200,-832;Inherit;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;343;128,-432;Inherit;False;ShroomLightModel;1;;2;9ba531354399e4286b6b6809cf677f13;0;6;111;FLOAT;0;False;21;FLOAT3;0,0,0;False;133;FLOAT;0;False;128;FLOAT4;0,0,0,0;False;10;COLOR;0.5754717,0.5754717,0.5754717,1;False;7;FLOAT3;0,0,0;False;1;COLOR;17
Node;AmplifyShaderEditor.GetLocalVarNode;46;640,48;Inherit;False;45;MainTexAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;189;-1504,-1344;Inherit;False;192;ShroomLightIndex;1;0;OBJECT;;False;1;INT;0
Node;AmplifyShaderEditor.GlobalArrayNode;183;-1280,-1312;Inherit;False;ShroomLightColors;0;10;1;False;False;0;1;True;Object;-1;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;265;480,-272;Inherit;False;Property;_DisplayNormals;Display Normals;9;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
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
WireConnection;42;0;41;0
WireConnection;142;0;65;0
WireConnection;142;1;64;0
WireConnection;172;0;140;0
WireConnection;172;1;171;0
WireConnection;192;0;191;0
WireConnection;35;2;42;0
WireConnection;135;0;142;0
WireConnection;135;1;137;0
WireConnection;135;2;172;0
WireConnection;51;0;35;0
WireConnection;169;1;170;0
WireConnection;169;0;135;0
WireConnection;182;0;188;0
WireConnection;182;1;197;0
WireConnection;36;2;14;0
WireConnection;63;0;51;0
WireConnection;63;1;169;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;226;0;182;0
WireConnection;13;0;3;0
WireConnection;13;1;36;0
WireConnection;13;2;63;0
WireConnection;208;0;226;0
WireConnection;208;1;207;0
WireConnection;209;0;208;0
WireConnection;280;1;3;0
WireConnection;280;0;13;0
WireConnection;178;2;306;0
WireConnection;221;0;207;0
WireConnection;221;1;226;0
WireConnection;185;0;190;0
WireConnection;185;1;197;0
WireConnection;45;0;1;4
WireConnection;343;111;304;0
WireConnection;343;21;209;0
WireConnection;343;133;221;0
WireConnection;343;128;185;0
WireConnection;343;10;280;0
WireConnection;343;7;178;0
WireConnection;183;0;189;0
WireConnection;183;1;197;0
WireConnection;265;1;343;17
WireConnection;265;0;178;0
WireConnection;0;2;265;0
WireConnection;0;10;46;0
ASEEND*/
//CHKSM=4C50E1B046A4A4B0B76AAC97CF28F4F8CDC3EA98