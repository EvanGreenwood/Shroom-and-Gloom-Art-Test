// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/Enemy Fog WaterColor"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.6
		[PerRendererData]_MainTex("MainTex", 2D) = "white" {}
		_FogOffset("FogOffset", Float) = 0
		[HDR]_GlowColor("GlowColor", Color) = (0,0,0,0)
		_WaterColor("WaterColor", 2D) = "white" {}
		_WaterColorScale("WaterColor Scale", Float) = 1
		_NoiseAmount("Noise Amount", Float) = 1
		_WaterColorScrollSpeed("WaterColor Scroll Speed", Float) = 0.3
		_NoiseScale("Noise Scale", Float) = 0.3
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
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap vertex:vertexDataFunc 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float3 worldPos;
			float eyeDepth;
		};

		uniform float4 FogColor;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _GlowColor;
		uniform sampler2D _WaterColor;
		uniform float _WaterColorScale;
		uniform float _WaterColorScrollSpeed;
		uniform float _NoiseScale;
		uniform float _NoiseAmount;
		uniform float FogDistance;
		uniform float FogStart;
		uniform float _FogOffset;
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
			float4 temp_output_2_0_g18 = FogColor;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 break48 = tex2DNode1;
			float4 appendResult50 = (float4(saturate( ( 4.0 * break48.r ) ) , saturate( ( 4.0 * break48.g ) ) , saturate( ( 4.0 * break48.b ) ) , tex2DNode1.a));
			float4 color43 = IsGammaSpace() ? float4(0.65,0.65,0.65,0) : float4(0.3800563,0.3800563,0.3800563,0);
			float4 break45 = ( tex2DNode1 - color43 );
			float3 ase_worldPos = i.worldPos;
			float mulTime74 = _Time.y * _WaterColorScrollSpeed;
			float2 appendResult66 = (float2(( ase_worldPos.x + mulTime74 ) , ase_worldPos.y));
			float mulTime83 = _Time.y * 0.4;
			float2 appendResult82 = (float2(( ase_worldPos.x + mulTime83 ) , ase_worldPos.y));
			float simplePerlin2D77 = snoise( appendResult82*_NoiseScale );
			simplePerlin2D77 = simplePerlin2D77*0.5 + 0.5;
			float cameraDepthFade30 = (( i.eyeDepth -_ProjectionParams.y - ( FogStart + _FogOffset ) ) / FogDistance);
			float temp_output_2_0_g14 = ( 1.0 - saturate( cameraDepthFade30 ) );
			float temp_output_2_0_g19 = ( temp_output_2_0_g14 * temp_output_2_0_g14 );
			float4 lerpResult13 = lerp( ( temp_output_2_0_g18 * temp_output_2_0_g18 ) , ( i.vertexColor * ( appendResult50 + ( saturate( break45.g ) * _GlowColor * 12.0 ) ) * ( 1.0 - ( saturate( break45.r ) * ( 1.0 - tex2D( _WaterColor, ( ( _WaterColorScale * appendResult66 ) + ( simplePerlin2D77 * _NoiseAmount ) ) ) ) ) ) ) , ( temp_output_2_0_g19 * temp_output_2_0_g19 ));
			o.Emission = ( lerpResult13 * 1.0 ).rgb;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.RangedFloatNode;84;-2752,1216;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;0;False;0;False;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-2672,1008;Inherit;False;Property;_WaterColorScrollSpeed;WaterColor Scroll Speed;7;0;Create;True;0;0;0;False;0;False;0.3;0.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;83;-2560,1200;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;65;-2912,688;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;74;-2272,976;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;85;-2336,1072;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-2192,1056;Inherit;False;Property;_NoiseScale;Noise Scale;8;0;Create;True;0;0;0;False;0;False;0.3;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;82;-2224,1168;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;-2112,752;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;66;-1952,800;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-2032,672;Inherit;False;Property;_WaterColorScale;WaterColor Scale;5;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-1968,1184;Inherit;False;Property;_NoiseAmount;Noise Amount;6;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;77;-1872,1008;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-2000,-112;Inherit;True;Property;_MainTex;MainTex;1;1;[PerRendererData];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;43;-1920,240;Inherit;False;Constant;_Grey;Grey;3;0;Create;True;0;0;0;False;0;False;0.65,0.65,0.65,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;39;-1536,-672;Inherit;False;Property;_FogOffset;FogOffset;2;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1552,-832;Inherit;False;Global;FogStart;FogStart;2;0;Create;True;0;0;0;False;0;False;1.5;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-1792,768;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-1600,1088;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;44;-1584,192;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1376,-912;Inherit;False;Global;FogDistance;FogDistance;2;0;Create;True;0;0;0;False;0;False;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-1328,-720;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;48;-1632,-112;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;54;-1616,-224;Inherit;False;Constant;_2;2;3;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;79;-1536,912;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CameraDepthFade;30;-1152,-720;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-1376,-208;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-1376,-128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1376,-48;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;45;-1424,176;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;64;-1312,736;Inherit;True;Property;_WaterColor;WaterColor;4;0;Create;True;0;0;0;False;0;False;-1;None;dcc11b79ead256146ab34fea74546795;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;57;-1136,112;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;62;-1040,-576;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;49;-1216,-160;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;51;-1216,-96;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;52;-1216,-32;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-1152,448;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;0;False;0;False;12;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;63;-1264,256;Inherit;False;Property;_GlowColor;GlowColor;3;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;3.482202,0.1773949,1.285651,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;47;-1136,176;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;68;-1168,576;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;73;-976,688;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;50;-1040,-112;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;34;-848,-576;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-960,160;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-896,480;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;14;-880,-896;Inherit;False;Global;FogColor;FogColor;2;0;Create;True;0;0;0;False;0;False;0.1887822,0.1643645,0.509434,1;0.04705882,0.2470588,0.2666667,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;33;-704,-544;Inherit;True;Square;-1;;14;fea980a1f68019543b2cd91d506986e8;0;1;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;2;-1168,-400;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-848,-96;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;70;-768,320;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;36;-592,-672;Inherit;False;Square;-1;;18;fea980a1f68019543b2cd91d506986e8;0;1;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;35;-544,-480;Inherit;False;Square;-1;;19;fea980a1f68019543b2cd91d506986e8;0;1;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-576,-256;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;13;-304,-432;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-352,-160;Inherit;False;Constant;_Float1;Float 1;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-112,-192;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;86;-2608,640;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;112,-208;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Custom/Enemy Fog WaterColor;False;False;False;False;True;True;True;True;True;False;False;False;False;False;True;False;False;False;False;False;False;Off;1;False;;1;False;;False;0;False;;0;False;;False;0;Masked;0.6;True;False;0;False;TransparentCutout;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;83;0;84;0
WireConnection;74;0;75;0
WireConnection;85;0;65;1
WireConnection;85;1;83;0
WireConnection;82;0;85;0
WireConnection;82;1;65;2
WireConnection;76;0;65;1
WireConnection;76;1;74;0
WireConnection;66;0;76;0
WireConnection;66;1;65;2
WireConnection;77;0;82;0
WireConnection;77;1;78;0
WireConnection;72;0;71;0
WireConnection;72;1;66;0
WireConnection;81;0;77;0
WireConnection;81;1;80;0
WireConnection;44;0;1;0
WireConnection;44;1;43;0
WireConnection;40;0;29;0
WireConnection;40;1;39;0
WireConnection;48;0;1;0
WireConnection;79;0;72;0
WireConnection;79;1;81;0
WireConnection;30;0;28;0
WireConnection;30;1;40;0
WireConnection;53;0;54;0
WireConnection;53;1;48;0
WireConnection;55;0;54;0
WireConnection;55;1;48;1
WireConnection;56;0;54;0
WireConnection;56;1;48;2
WireConnection;45;0;44;0
WireConnection;64;1;79;0
WireConnection;57;0;1;4
WireConnection;62;0;30;0
WireConnection;49;0;53;0
WireConnection;51;0;55;0
WireConnection;52;0;56;0
WireConnection;47;0;45;1
WireConnection;68;0;45;0
WireConnection;73;0;64;0
WireConnection;50;0;49;0
WireConnection;50;1;51;0
WireConnection;50;2;52;0
WireConnection;50;3;57;0
WireConnection;34;0;62;0
WireConnection;58;0;47;0
WireConnection;58;1;63;0
WireConnection;58;2;59;0
WireConnection;69;0;68;0
WireConnection;69;1;73;0
WireConnection;33;2;34;0
WireConnection;46;0;50;0
WireConnection;46;1;58;0
WireConnection;70;0;69;0
WireConnection;36;2;14;0
WireConnection;35;2;33;0
WireConnection;3;0;2;0
WireConnection;3;1;46;0
WireConnection;3;2;70;0
WireConnection;13;0;36;0
WireConnection;13;1;3;0
WireConnection;13;2;35;0
WireConnection;60;0;13;0
WireConnection;60;1;61;0
WireConnection;86;0;65;0
WireConnection;0;2;60;0
WireConnection;0;10;1;4
ASEEND*/
//CHKSM=A5ADEE34E82A2E6316122F9AFDB8D2632864E7A8