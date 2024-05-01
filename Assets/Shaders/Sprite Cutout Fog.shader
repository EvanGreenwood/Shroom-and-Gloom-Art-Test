// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/Sprite Cutout Fog"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.6
		[PerRendererData]_MainTex("MainTex", 2D) = "white" {}
		[Toggle(_NOISYFOG_ON)] _NoisyFog("NoisyFog", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		UsePass ""
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _NOISYFOG_ON
		#pragma surface surf Standard keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float eyeDepth;
			float4 screenPos;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 FogColor;
		uniform float FogDistance;
		uniform float FogStart;
		uniform float _Cutoff = 0.6;


		float3 PerturbNormal107_g65( float3 surf_pos, float3 surf_norm, float height, float scale )
		{
			// "Bump Mapping Unparametrized Surfaces on the GPU" by Morten S. Mikkelsen
			float3 vSigmaS = ddx( surf_pos );
			float3 vSigmaT = ddy( surf_pos );
			float3 vN = surf_norm;
			float3 vR1 = cross( vSigmaT , vN );
			float3 vR2 = cross( vN , vSigmaS );
			float fDet = dot( vSigmaS , vR1 );
			float dBs = ddx( height );
			float dBt = ddy( height );
			float3 vSurfGrad = scale * 0.05 * sign( fDet ) * ( dBs * vR1 + dBt * vR2 );
			return normalize ( abs( fDet ) * vN - vSurfGrad );
		}


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

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 surf_pos107_g65 = ase_worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 surf_norm107_g65 = ase_worldNormal;
			float height107_g65 = 0.0;
			float scale107_g65 = 1.0;
			float3 localPerturbNormal107_g65 = PerturbNormal107_g65( surf_pos107_g65 , surf_norm107_g65 , height107_g65 , scale107_g65 );
			o.Normal = localPerturbNormal107_g65;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 temp_output_2_0_g28 = FogColor;
			float cameraDepthFade30 = (( i.eyeDepth -_ProjectionParams.y - FogStart ) / FogDistance);
			float temp_output_2_0_g21 = ( saturate( cameraDepthFade30 ) - 1.0 );
			float temp_output_41_0 = sqrt( ( 1.0 - ( temp_output_2_0_g21 * temp_output_2_0_g21 ) ) );
			float temp_output_2_0_g29 = ( 1.0 - temp_output_41_0 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float temp_output_21_0_g58 = 0.5;
			float2 appendResult18_g58 = (float2(( ( _SinTime.y + ase_screenPos.x ) * temp_output_21_0_g58 ) , ( ( _SinTime.x + ase_screenPos.y ) * temp_output_21_0_g58 )));
			float simplePerlin2D14_g58 = snoise( appendResult18_g58*2.0 );
			simplePerlin2D14_g58 = simplePerlin2D14_g58*0.5 + 0.5;
			float2 appendResult13_g58 = (float2(( temp_output_21_0_g58 * ( _SinTime.y + ase_screenPos.x ) ) , ( temp_output_21_0_g58 * ( ase_screenPos.y + _SinTime.x ) )));
			float simplePerlin2D15_g58 = snoise( appendResult13_g58*2.0 );
			simplePerlin2D15_g58 = simplePerlin2D15_g58*0.5 + 0.5;
			float2 appendResult5_g58 = (float2(simplePerlin2D14_g58 , simplePerlin2D15_g58));
			float simplePerlin2D1_g58 = snoise( appendResult5_g58*0.1 );
			simplePerlin2D1_g58 = simplePerlin2D1_g58*0.5 + 0.5;
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
			float lerpResult135 = lerp( ( 1.0 * ( ( ( ( simplePerlin2D1_g58 + simplePerlin2D1_g59 + simplePerlin2D1_g60 + simplePerlin2D1_g61 ) / 4.0 ) - 0.5 ) * ( 1.0 - Depth134 ) ) ) , 0.0 , ( Depth134 + 1.0 ));
			#ifdef _NOISYFOG_ON
				float staticSwitch169 = lerpResult135;
			#else
				float staticSwitch169 = 0.0;
			#endif
			float4 lerpResult13 = lerp( ( tex2DNode1 * i.vertexColor ) , ( temp_output_2_0_g28 * temp_output_2_0_g28 ) , ( ( 1.0 - ( temp_output_2_0_g29 * temp_output_2_0_g29 ) ) + staticSwitch169 ));
			o.Emission = lerpResult13.rgb;
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
Node;AmplifyShaderEditor.CommentaryNode;52;-2144,-304;Inherit;False;1716;435;Applies fog based on camera depth;13;13;14;36;37;29;30;35;41;42;51;28;63;134;Apply Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2128,16;Inherit;False;Global;FogStart;FogStart;2;0;Create;True;0;0;0;False;0;False;1.5;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-2128,-64;Inherit;False;Global;FogDistance;FogDistance;2;0;Create;True;0;0;0;False;0;False;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;173;-3072,208;Inherit;False;2228;819; Attempt to add some dynamisim to fog. Not good enough;29;156;158;160;152;162;163;164;165;155;157;159;151;128;130;129;62;136;61;138;64;140;65;171;137;142;172;135;170;169;Noisy Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.CameraDepthFade;30;-1872,-48;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;37;-1584,-72;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2992,560;Inherit;False;Constant;_Float4;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;-3008,736;Inherit;False;Constant;_Float5;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-3024,912;Inherit;False;Constant;_Float6;Float 0;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-3024,368;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-3024,256;Inherit;False;Constant;_Float7;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-3024,464;Inherit;False;Constant;_Float8;Float 0;2;0;Create;True;0;0;0;False;0;False;0.125;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-3008,656;Inherit;False;Constant;_Float9;Float 0;2;0;Create;True;0;0;0;False;0;False;0.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-3024,832;Inherit;False;Constant;_Float10;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;41;-1440,-64;Inherit;False;Easing OutCirc ShaderFunction;-1;;20;181ee7db325895443a774b654a9b8500;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;155;-2784,496;Inherit;False;ScreenNoise;-1;;59;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;157;-2800,672;Inherit;False;ScreenNoise;-1;;60;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;159;-2816,848;Inherit;False;ScreenNoise;-1;;61;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;151;-2784,336;Inherit;False;ScreenNoise;-1;;58;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;128;-2288,320;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-2320,512;Inherit;False;Constant;_Float16;Float 16;2;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-976,32;Inherit;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;129;-2160,384;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-2000,576;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;136;-2080,832;Inherit;False;134;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-1904,368;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;138;-1888,720;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-1776,544;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;-1632,736;Inherit;False;134;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-1728,336;Inherit;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-1600,832;Inherit;False;Constant;_Float13;Float 13;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;42;-1120,-64;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-1504,624;Inherit;False;Constant;_Float19;Float 19;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;-1584,432;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;172;-1344,784;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;35;-976,-64;Inherit;False;Square;-1;;29;fea980a1f68019543b2cd91d506986e8;0;1;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;50;-1760,-976;Inherit;False;868;563;Comment;4;1;45;3;2;Tex And Vertex Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;135;-1296,560;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-1248,416;Inherit;False;Constant;_Float12;Float 12;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;2;-1376,-624;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;51;-832,-64;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;-1168,-240;Inherit;False;Global;FogColor;FogColor;2;0;Create;True;0;0;0;False;0;False;0.1887822,0.1643645,0.509434,1;0.04705882,0.2470588,0.2666667,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;169;-1088,432;Inherit;False;Property;_NoisyFog;NoisyFog;2;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1680,-912;Inherit;True;Property;_MainTex;MainTex;1;1;[PerRendererData];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-1040,-720;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-1376,-720;Inherit;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;36;-832,-192;Inherit;False;Square;-1;;28;fea980a1f68019543b2cd91d506986e8;0;1;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-640,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;13;-608,-256;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;32,-64;Inherit;False;45;MainTexAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;176;-320,-752;Inherit;True;Normal From Height;-1;;65;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;1;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;256,-304;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Custom/Sprite Cutout Fog;False;False;False;False;True;True;True;True;True;False;False;False;False;False;True;False;False;False;False;False;False;Off;1;False;;1;False;;False;0;False;;0;False;;False;0;Masked;0.6;True;False;0;False;TransparentCutout;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;1;Above;;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
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
WireConnection;35;2;42;0
WireConnection;135;0;142;0
WireConnection;135;1;137;0
WireConnection;135;2;172;0
WireConnection;51;0;35;0
WireConnection;169;1;170;0
WireConnection;169;0;135;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;45;0;1;4
WireConnection;36;2;14;0
WireConnection;63;0;51;0
WireConnection;63;1;169;0
WireConnection;13;0;3;0
WireConnection;13;1;36;0
WireConnection;13;2;63;0
WireConnection;0;1;176;0
WireConnection;0;2;13;0
WireConnection;0;10;46;0
ASEEND*/
//CHKSM=5FE524AFD267D8D2CB49F680CAB2155C5EE0ABC0