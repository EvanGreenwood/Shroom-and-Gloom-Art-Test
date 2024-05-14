// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNG/Environment"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.6
		_FogIntensityCoefficient("FogIntensityCoefficient", Range( 0 , 1)) = 1
		[PerRendererData]_MainTex("MainTex", 2D) = "white" {}
		[Toggle(_NOISYFOG_ON)] _NoisyFog("NoisyFog", Float) = 1
		_NormalMip("NormalMip", Range( 0 , 9)) = 0
		[Toggle(_USEFOG_ON)] _UseFog("Use Fog", Float) = 1
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_Emission("Emission", 2D) = "black" {}
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		[PerRendererData]_TunnelDistance("TunnelDistance", Float) = 0
		[Toggle(_EMISSIONWAVEBASEDONTUNNEL_ON)] _EmissionWaveBasedOnTunnel("EmissionWaveBasedOnTunnel", Float) = 1
		_PostReplaceLineColor("PostReplaceLineColor", Color) = (0,0,0,1)
		_PreReplaceLineColor("PreReplaceLineColor", Color) = (0,0,0,1)
		_LineReplaceRange("LineReplaceRange", Float) = 0
		_LineReplaceFuzziness("LineReplaceFuzziness", Float) = 0
		_LineColorVsHSVBlend("LineColorVsHSVBlend", Range( 0 , 1)) = 1
		_LineHueOffset("LineHueOffset", Float) = 0
		_LineSaturationOffset("LineSaturationOffset", Float) = 0
		_LineValueOffset("LineValueOffset", Float) = 0
		[Toggle(_REPLACELINECOLOR_ON)] _ReplaceLineColor("ReplaceLineColor", Float) = 1
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
		#pragma shader_feature _SCENE_VIEW
		#pragma shader_feature_local _USEFOG_ON
		#pragma shader_feature_local _REPLACELINECOLOR_ON
		#pragma shader_feature_local _NOISYFOG_ON
		#pragma shader_feature_local _EMISSIONWAVEBASEDONTUNNEL_ON
		#pragma surface surf Standard keepalpha noshadow exclude_path:forward noambient nolightmap  nodynlightmap nodirlightmap nofog vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float eyeDepth;
			float4 screenPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _NormalMip;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _PostReplaceLineColor;
		uniform float _LineHueOffset;
		uniform float _LineSaturationOffset;
		uniform float _LineValueOffset;
		uniform float _LineColorVsHSVBlend;
		uniform float4 _PreReplaceLineColor;
		uniform float _LineReplaceRange;
		uniform float _LineReplaceFuzziness;
		uniform float4 FogColor;
		uniform float FogDistance;
		uniform float FogStart;
		uniform float _FogIntensityCoefficient;
		uniform float _TunnelDistance;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
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
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 normalizeResult374 = normalize( UnpackNormal( tex2Dlod( _Normal, float4( uv_Normal, 0, _NormalMip) ) ) );
			float3 NormalSample347 = normalizeResult374;
			o.Normal = NormalSample347;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 temp_output_3_0 = ( tex2DNode1 * i.vertexColor );
			float3 hsvTorgb410 = RGBToHSV( i.vertexColor.rgb );
			float3 hsvTorgb414 = HSVToRGB( float3(( _LineHueOffset + hsvTorgb410.x ),( hsvTorgb410.y + _LineSaturationOffset ),( hsvTorgb410.z + _LineValueOffset )) );
			float4 lerpResult402 = lerp( _PostReplaceLineColor , float4( hsvTorgb414 , 0.0 ) , _LineColorVsHSVBlend);
			float4 temp_output_1_0_g91 = temp_output_3_0;
			float4 temp_output_2_0_g91 = _PreReplaceLineColor;
			float temp_output_11_0_g91 = distance( temp_output_1_0_g91 , temp_output_2_0_g91 );
			float4 lerpResult21_g91 = lerp( lerpResult402 , temp_output_1_0_g91 , saturate( ( ( temp_output_11_0_g91 - _LineReplaceRange ) / max( _LineReplaceFuzziness , 1E-05 ) ) ));
			#ifdef _REPLACELINECOLOR_ON
				float4 staticSwitch424 = lerpResult21_g91;
			#else
				float4 staticSwitch424 = temp_output_3_0;
			#endif
			float4 BaseColor415 = staticSwitch424;
			float4 temp_output_2_0_g93 = FogColor;
			float cameraDepthFade30 = (( i.eyeDepth -_ProjectionParams.y - FogStart ) / FogDistance);
			float temp_output_2_0_g68 = ( saturate( ( cameraDepthFade30 * _FogIntensityCoefficient ) ) - 1.0 );
			float temp_output_41_0 = sqrt( ( 1.0 - ( temp_output_2_0_g68 * temp_output_2_0_g68 ) ) );
			float temp_output_2_0_g92 = ( 1.0 - temp_output_41_0 );
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
			float4 lerpResult13 = lerp( BaseColor415 , ( temp_output_2_0_g93 * temp_output_2_0_g93 ) , ( ( 1.0 - ( temp_output_2_0_g92 * temp_output_2_0_g92 ) ) + staticSwitch169 ));
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
			o.Albedo = Diffuse350.rgb;
			float2 break19_g94 = float2( -0.1,0.1 );
			float temp_output_1_0_g94 = ( _TunnelDistance + _Time.y );
			float sinIn7_g94 = sin( temp_output_1_0_g94 );
			float sinInOffset6_g94 = sin( ( temp_output_1_0_g94 + 1.0 ) );
			float lerpResult20_g94 = lerp( break19_g94.x , break19_g94.y , frac( ( sin( ( ( sinIn7_g94 - sinInOffset6_g94 ) * 91.2228 ) ) * 43758.55 ) ));
			float clampResult385 = clamp( ( lerpResult20_g94 + sinIn7_g94 ) , 0.0 , 1.0 );
			#ifdef _EMISSIONWAVEBASEDONTUNNEL_ON
				float staticSwitch386 = clampResult385;
			#else
				float staticSwitch386 = 1.0;
			#endif
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float4 Emission390 = ( staticSwitch386 * ( tex2D( _Emission, uv_Emission ) * _EmissionColor ) );
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
Node;AmplifyShaderEditor.CommentaryNode;52;-4256,-576;Inherit;False;1726.913;467.74;Applies fog based on camera depth;16;134;37;41;368;367;28;30;36;14;13;63;51;35;42;29;418;Apply Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-4240,-256;Inherit;False;Global;FogStart;FogStart;2;0;Create;True;0;0;0;False;0;False;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-4240,-336;Inherit;False;Global;FogDistance;FogDistance;2;0;Create;True;0;0;0;False;0;False;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;30;-3984,-320;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;367;-4016,-192;Inherit;False;Property;_FogIntensityCoefficient;FogIntensityCoefficient;1;0;Create;True;0;0;0;False;0;False;1;20;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;173;-5232,208;Inherit;False;2228;819; Attempt to add some dynamisim to fog. Not good enough;29;156;158;160;152;162;163;164;165;155;157;159;151;128;130;129;62;136;61;138;64;140;65;171;137;142;172;135;170;169;Noisy Fog (needs work bad atm);1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;368;-3728,-272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-5184,912;Inherit;False;Constant;_Float6;Float 0;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-5184,832;Inherit;False;Constant;_Float10;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-5152,560;Inherit;False;Constant;_Float4;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;-5168,736;Inherit;False;Constant;_Float5;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-5184,368;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-5184,256;Inherit;False;Constant;_Float7;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-5184,464;Inherit;False;Constant;_Float8;Float 0;2;0;Create;True;0;0;0;False;0;False;0.125;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-5168,656;Inherit;False;Constant;_Float9;Float 0;2;0;Create;True;0;0;0;False;0;False;0.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;37;-3568,-352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;411;-4496,-1040;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;159;-4976,848;Inherit;False;ScreenNoise;-1;;61;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;155;-4944,496;Inherit;False;ScreenNoise;-1;;64;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;157;-4960,672;Inherit;False;ScreenNoise;-1;;65;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;151;-4944,336;Inherit;False;ScreenNoise;-1;;66;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;41;-3408,-336;Inherit;False;EasingOutCirc;-1;;67;181ee7db325895443a774b654a9b8500;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;410;-4272,-992;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;128;-4448,320;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-4480,512;Inherit;False;Constant;_Float16;Float 16;2;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-3248,-208;Inherit;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;50;-5040,-1712;Inherit;False;868;563;Comment;4;1;45;3;2;Tex And Vertex Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;412;-3984,-1088;Inherit;False;Property;_LineHueOffset;LineHueOffset;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;419;-4000,-848;Inherit;False;Property;_LineSaturationOffset;LineSaturationOffset;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;423;-4016,-768;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;420;-3904,-704;Inherit;False;Property;_LineValueOffset;LineValueOffset;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;136;-4240,832;Inherit;False;134;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;129;-4320,384;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-4160,576;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-4960,-1648;Inherit;True;Property;_MainTex;MainTex;2;1;[PerRendererData];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;2;-4656,-1360;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;413;-3760,-1040;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;421;-3776,-928;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;422;-3696,-832;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-4064,368;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;138;-4048,720;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-4320,-1456;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;393;-3472,-1200;Inherit;False;Property;_PostReplaceLineColor;PostReplaceLineColor;11;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.HSVToRGBNode;414;-3600,-992;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;408;-3184,-960;Inherit;False;Property;_LineColorVsHSVBlend;LineColorVsHSVBlend;15;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-3936,544;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;-3792,736;Inherit;False;134;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-3888,336;Inherit;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-3760,832;Inherit;False;Constant;_Float13;Float 13;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;403;-3120,-1280;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;402;-2928,-1120;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;394;-2880,-992;Inherit;False;Property;_LineReplaceRange;LineReplaceRange;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;395;-2880,-896;Inherit;False;Property;_LineReplaceFuzziness;LineReplaceFuzziness;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;392;-3728,-1296;Inherit;False;Property;_PreReplaceLineColor;PreReplaceLineColor;12;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;389;-3744,-3232;Inherit;False;1060;1059;Emission;12;380;379;381;383;376;375;385;387;377;386;384;390;Emission;0,0,0,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;42;-3232,-336;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;172;-3504,784;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-3664,624;Inherit;False;Constant;_Float19;Float 19;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;-3744,432;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;388;-2624,-1216;Inherit;False;Replace Color;-1;;91;896dccb3016c847439def376a728b869;1,12,0;5;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;35;-3088,-336;Inherit;False;Square;-1;;92;fea980a1f68019543b2cd91d506986e8;0;1;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;135;-3456,560;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-3408,416;Inherit;False;Constant;_Float12;Float 12;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;379;-3728,-3152;Inherit;False;Property;_TunnelDistance;TunnelDistance;9;1;[PerRendererData];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;380;-3728,-3056;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;424;-2368,-1360;Inherit;False;Property;_ReplaceLineColor;ReplaceLineColor;19;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;51;-2944,-336;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;-3984,-528;Inherit;False;Global;FogColor;FogColor;2;0;Create;True;0;0;0;False;0;False;0.1887822,0.1643645,0.509434,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;169;-3248,432;Inherit;False;Property;_NoisyFog;NoisyFog;3;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;381;-3520,-3024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;415;-2032,-1216;Inherit;False;BaseColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;329;-3776,-1920;Inherit;False;976.0073;302.9471;Comment;4;347;178;306;374;Mip Smoothed Normal Sample;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-2752,-272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;36;-3648,-480;Inherit;False;Square;-1;;93;fea980a1f68019543b2cd91d506986e8;0;1;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;383;-3520,-2864;Inherit;False;Noise Sine Wave;-1;;94;a6eff29f739ced848846e3b648af87bd;0;2;1;FLOAT;0;False;2;FLOAT2;-0.1,0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;418;-2944,-544;Inherit;False;415;BaseColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;13;-2720,-528;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;306;-3760,-1760;Inherit;False;Property;_NormalMip;NormalMip;4;0;Create;True;0;0;0;False;0;False;0;2;0;9;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;376;-3584,-2384;Inherit;False;Property;_EmissionColor;EmissionColor;8;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;375;-3696,-2656;Inherit;True;Property;_Emission;Emission;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;385;-3280,-2848;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;387;-3376,-3120;Inherit;False;Constant;_Float11;Float 11;17;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;417;-2352,-656;Inherit;False;415;BaseColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;178;-3424,-1872;Inherit;True;Property;_Normal;Normal;6;1;[Normal];Create;True;0;0;0;False;0;False;178;None;None;True;0;False;bump;Auto;True;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;377;-3360,-2656;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;386;-3216,-2992;Inherit;False;Property;_EmissionWaveBasedOnTunnel;EmissionWaveBasedOnTunnel;10;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;280;-2112,-560;Inherit;False;Property;_UseFog;Use Fog;5;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;416;-1968,-400;Inherit;False;415;BaseColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;384;-3088,-2688;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;374;-3120,-1824;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;370;-1744,-736;Inherit;False;Property;_SCENE_VIEW;SCENE_VIEW;10;0;Create;True;0;0;0;False;0;False;0;1;1;False;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-4656,-1456;Inherit;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;390;-2880,-2560;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;347;-3008,-1696;Inherit;False;NormalSample;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;350;-1424,-736;Inherit;False;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;371;-704,-1520;Inherit;False;350;Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;391;-704,-1360;Inherit;False;390;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-640,-1232;Inherit;False;45;MainTexAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;372;-736,-1440;Inherit;False;347;NormalSample;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-432,-1472;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SNG/Environment;False;False;False;False;True;False;True;True;True;True;False;False;False;False;True;False;False;False;False;False;False;Off;1;False;;1;False;;False;0;False;;0;False;;False;0;Masked;0.6;True;False;0;False;TransparentCutout;;AlphaTest;DeferredOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
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
WireConnection;410;0;411;0
WireConnection;128;0;151;0
WireConnection;128;1;155;0
WireConnection;128;2;157;0
WireConnection;128;3;159;0
WireConnection;134;0;41;0
WireConnection;423;0;410;3
WireConnection;129;0;128;0
WireConnection;129;1;130;0
WireConnection;413;0;412;0
WireConnection;413;1;410;1
WireConnection;421;0;410;2
WireConnection;421;1;419;0
WireConnection;422;0;423;0
WireConnection;422;1;420;0
WireConnection;61;0;129;0
WireConnection;61;1;62;0
WireConnection;138;0;136;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;414;0;413;0
WireConnection;414;1;421;0
WireConnection;414;2;422;0
WireConnection;64;0;61;0
WireConnection;64;1;138;0
WireConnection;403;0;3;0
WireConnection;402;0;393;0
WireConnection;402;1;414;0
WireConnection;402;2;408;0
WireConnection;42;0;41;0
WireConnection;172;0;140;0
WireConnection;172;1;171;0
WireConnection;142;0;65;0
WireConnection;142;1;64;0
WireConnection;388;1;403;0
WireConnection;388;2;392;0
WireConnection;388;3;402;0
WireConnection;388;4;394;0
WireConnection;388;5;395;0
WireConnection;35;2;42;0
WireConnection;135;0;142;0
WireConnection;135;1;137;0
WireConnection;135;2;172;0
WireConnection;424;1;3;0
WireConnection;424;0;388;0
WireConnection;51;0;35;0
WireConnection;169;1;170;0
WireConnection;169;0;135;0
WireConnection;381;0;379;0
WireConnection;381;1;380;0
WireConnection;415;0;424;0
WireConnection;63;0;51;0
WireConnection;63;1;169;0
WireConnection;36;2;14;0
WireConnection;383;1;381;0
WireConnection;13;0;418;0
WireConnection;13;1;36;0
WireConnection;13;2;63;0
WireConnection;385;0;383;0
WireConnection;178;2;306;0
WireConnection;377;0;375;0
WireConnection;377;1;376;0
WireConnection;386;1;387;0
WireConnection;386;0;385;0
WireConnection;280;1;417;0
WireConnection;280;0;13;0
WireConnection;384;0;386;0
WireConnection;384;1;377;0
WireConnection;374;0;178;0
WireConnection;370;1;280;0
WireConnection;370;0;416;0
WireConnection;45;0;1;4
WireConnection;390;0;384;0
WireConnection;347;0;374;0
WireConnection;350;0;370;0
WireConnection;0;0;371;0
WireConnection;0;1;372;0
WireConnection;0;2;391;0
WireConnection;0;10;46;0
ASEEND*/
//CHKSM=E8921C37B221126CC441BE94562BEB560E1D279B