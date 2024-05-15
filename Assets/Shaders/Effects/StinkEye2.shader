// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNG/OpArt/StinkEye2"
{
	Properties
	{
		_TimeScale("TimeScale", Float) = 0.1
		_RingFactor("RingFactor", Range( 0 , 1)) = 0.5
		_Scale("Scale", Float) = 0.5
		_Ramp1("Ramp1", 2D) = "white" {}
		_Ramp2("Ramp2", 2D) = "white" {}
		_DistAmt("DistAmt", Float) = 1
		_DistScale("DistScale", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZWrite Off
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustomLighting keepalpha addshadow fullforwardshadows noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _Ramp1;
		uniform float _RingFactor;
		uniform float _TimeScale;
		uniform float _DistScale;
		uniform float _DistAmt;
		uniform float _Scale;
		uniform sampler2D _Ramp2;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float RingScale53 = _RingFactor;
			float2 temp_output_75_0 = ( i.uv_texcoord + ( 1.0 * 0.5 ) );
			float2 UVCoord165 = ( i.uv_texcoord + ( 1.0 * 0.5 ) );
			float mulTime31 = _Time.y * _TimeScale;
			float TimeScaled92 = mulTime31;
			float3 appendResult156 = (float3(UVCoord165 , TimeScaled92));
			float simplePerlin3D154 = snoise( appendResult156*_DistScale );
			simplePerlin3D154 = simplePerlin3D154*0.5 + 0.5;
			float Distortion163 = ( simplePerlin3D154 * _DistAmt );
			float2 WorldUV_77 = ( temp_output_75_0 + ( temp_output_75_0 * Distortion163 ) );
			float UVLength132 = length( (WorldUV_77*_Scale + -( _Scale * 0.5 )) );
			float temp_output_3_0_g43 = ( ( RingScale53 * 0.5 ) - (-1.0 + (fmod( ( UVLength132 + fmod( TimeScaled92 , RingScale53 ) ) , RingScale53 ) - 0.0) * (1.0 - -1.0) / (RingScale53 - 0.0)) );
			float RingMask68 = saturate( ( temp_output_3_0_g43 / fwidth( temp_output_3_0_g43 ) ) );
			float2 appendResult86 = (float2(RingMask68 , (0.0 + (sin( TimeScaled92 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0))));
			float4 lerpResult114 = lerp( tex2D( _Ramp1, appendResult86 ) , tex2D( _Ramp2, appendResult86 ) , (0.0 + (sin( ( UVLength132 * 1.0 ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)));
			float4 Color79 = lerpResult114;
			c.rgb = saturate( Color79 ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;42;-3456,-304;Inherit;False;613.3109;164.7322;;3;92;31;32;Time;0,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;76;-5109.592,368;Inherit;False;1223.229;634.2648;;12;77;102;148;164;75;145;74;73;165;150;151;149;UV;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-3424,-240;Inherit;False;Property;_TimeScale;TimeScale;0;0;Create;True;0;0;0;False;0;False;0.1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;151;-4912,864;Inherit;False;Half;-1;;37;87053cd26e6398b4ebfa9426fa55cab1;0;1;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;149;-5008,736;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;31;-3264,-240;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-4736,736;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;162;-5120,1168;Inherit;False;1080.16;362.5692;;8;163;101;144;154;143;156;104;166;Distortion;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;92;-3072,-240;Inherit;False;TimeScaled;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;165;-4608,736;Inherit;False;UVCoord;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;166;-5072,1216;Inherit;False;165;UVCoord;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;-5072,1296;Inherit;False;92;TimeScaled;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;156;-4848,1216;Inherit;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-4848,1344;Inherit;False;Property;_DistScale;DistScale;6;0;Create;True;0;0;0;False;0;False;0.5;0.75;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;154;-4656,1216;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-4576,1440;Inherit;False;Property;_DistAmt;DistAmt;5;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-4416,1344;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;163;-4272,1344;Inherit;False;Distortion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;145;-4832,576;Inherit;False;Half;-1;;38;87053cd26e6398b4ebfa9426fa55cab1;0;1;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-4656,416;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;164;-4656,576;Inherit;False;163;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;67;-3456,384;Inherit;False;2117.45;455.5633;;18;58;57;68;123;111;51;52;55;132;33;59;93;56;60;64;78;61;62;Ring Mask;0,0,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-4448,496;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;102;-4256,416;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-3392,528;Inherit;False;Property;_Scale;Scale;2;0;Create;True;0;0;0;False;0;False;0.5;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;69;-3456,-48;Inherit;False;565.3323;139.76;;2;53;15;Ring Scale;0,0,0,1;0;0
Node;AmplifyShaderEditor.FunctionNode;61;-3392,608;Inherit;False;Half;-1;;39;87053cd26e6398b4ebfa9426fa55cab1;0;1;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-4128,416;Inherit;False;WorldUV ;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-3424,0;Inherit;False;Property;_RingFactor;RingFactor;1;0;Create;True;0;0;0;False;0;False;0.5;0.32;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-3424,432;Inherit;False;77;WorldUV ;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NegateNode;64;-3392,688;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;-3136,0;Inherit;False;RingScale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;60;-3200,432;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT;-0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;56;-2960,640;Inherit;False;53;RingScale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-2960,560;Inherit;False;92;TimeScaled;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;59;-2976,432;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FmodOpNode;33;-2768,560;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;132;-2832,432;Inherit;False;UVLength;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-2608,560;Inherit;False;53;RingScale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-2608,432;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FmodOpNode;51;-2384,432;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;88;-2768,-1424;Inherit;False;1588.864;710.8723;;12;79;114;115;113;83;86;70;97;91;94;122;134;Color;0,0,0,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;111;-2224,432;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;58;-2384,624;Inherit;False;Half;-1;;44;87053cd26e6398b4ebfa9426fa55cab1;0;1;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;-2720,-1264;Inherit;False;92;TimeScaled;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;57;-1936,448;Inherit;True;Step Antialiasing;-1;;43;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;91;-2528,-1264;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;-1552,432;Inherit;True;RingMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;97;-2400,-1264;Inherit;False;RS01;-1;;40;234e25958dfbf004a8d8c89023d992c1;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;-2400,-1360;Inherit;False;68;RingMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;134;-2368,-960;Inherit;False;132;UVLength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;86;-2208,-1360;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-2176,-960;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;83;-2048,-1360;Inherit;True;Property;_Ramp1;Ramp1;3;0;Create;True;0;0;0;False;0;False;-1;None;0517a8df51b6df24ea6427c2b8837df1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;113;-2048,-1168;Inherit;True;Property;_Ramp2;Ramp2;4;0;Create;True;0;0;0;False;0;False;-1;None;a16a516a83c31af4d98ad90a8fcecbd7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;115;-1920,-960;Inherit;False;Sin01;-1;;41;dfa74d9ebd1bfcc429ab1d37e2a53901;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;114;-1616,-1184;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-1392,-1200;Inherit;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;72;-601.6596,-50;Inherit;False;859.3857;513.7205;;3;124;99;0;;0,0,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;-560,256;Inherit;False;79;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TransformPositionNode;74;-4896,416;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;73;-5088,416;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;124;-384,256;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.AbsOpNode;123;-1888,592;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;SNG/OpArt/StinkEye2;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Back;2;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;31;0;32;0
WireConnection;150;0;149;0
WireConnection;150;1;151;0
WireConnection;92;0;31;0
WireConnection;165;0;150;0
WireConnection;156;0;166;0
WireConnection;156;2;104;0
WireConnection;154;0;156;0
WireConnection;154;1;143;0
WireConnection;101;0;154;0
WireConnection;101;1;144;0
WireConnection;163;0;101;0
WireConnection;75;0;149;0
WireConnection;75;1;145;0
WireConnection;148;0;75;0
WireConnection;148;1;164;0
WireConnection;102;0;75;0
WireConnection;102;1;148;0
WireConnection;61;1;62;0
WireConnection;77;0;102;0
WireConnection;64;0;61;0
WireConnection;53;0;15;0
WireConnection;60;0;78;0
WireConnection;60;1;62;0
WireConnection;60;2;64;0
WireConnection;59;0;60;0
WireConnection;33;0;93;0
WireConnection;33;1;56;0
WireConnection;132;0;59;0
WireConnection;52;0;132;0
WireConnection;52;1;33;0
WireConnection;51;0;52;0
WireConnection;51;1;55;0
WireConnection;111;0;51;0
WireConnection;111;2;55;0
WireConnection;58;1;55;0
WireConnection;57;1;111;0
WireConnection;57;2;58;0
WireConnection;91;0;94;0
WireConnection;68;0;57;0
WireConnection;97;1;91;0
WireConnection;86;0;70;0
WireConnection;86;1;97;0
WireConnection;122;0;134;0
WireConnection;83;1;86;0
WireConnection;113;1;86;0
WireConnection;115;1;122;0
WireConnection;114;0;83;0
WireConnection;114;1;113;0
WireConnection;114;2;115;0
WireConnection;79;0;114;0
WireConnection;124;0;99;0
WireConnection;0;13;124;0
ASEEND*/
//CHKSM=77DCF0987301CBC124B14B2F9FE101EE519E63E4