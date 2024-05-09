// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fire"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.15
		_Fire_Mask("Fire_Mask", 2D) = "white" {}
		[HDR]_Ramp("Ramp", 2D) = "white" {}
		_Scale("Scale", Float) = 0
		_ScalexySpeedzw("Scale(xy)/Speed(zw)", Vector) = (4,2,0,-1.5)
		_WaveDst("WaveDst", Float) = 1
		_Emission("Emission", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _WaveDst;
		uniform sampler2D _Ramp;
		uniform sampler2D _Fire_Mask;
		uniform float _Scale;
		uniform float4 _ScalexySpeedzw;
		uniform float _Emission;
		uniform float _Cutoff = 0.15;


		float2 voronoihash10( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi10( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash10( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F2;
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
			float3 appendResult71 = (float3(( ( sin( ( _Time.y * 4.0 ) ) * 0.05 ) * v.texcoord.xy.y ) , 0.0 , 0.0));
			float3 Wave61 = appendResult71;
			v.vertex.xyz += ( Wave61 * _WaveDst );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float time10 = 45.0;
			float2 voronoiSmoothId10 = 0;
			float voronoiSmooth10 = 0.0;
			float2 appendResult16 = (float2(_ScalexySpeedzw.x , _ScalexySpeedzw.y));
			float2 appendResult17 = (float2(_ScalexySpeedzw.z , _ScalexySpeedzw.w));
			float2 coords10 = (i.uv_texcoord*appendResult16 + ( appendResult17 * _Time.y )) * _Scale;
			float2 id10 = 0;
			float2 uv10 = 0;
			float voroi10 = voronoi10( coords10, time10, id10, uv10, voronoiSmooth10, voronoiSmoothId10 );
			float2 appendResult22 = (float2(( _ScalexySpeedzw.x / 2.0 ) , _ScalexySpeedzw.y));
			float2 appendResult23 = (float2(_ScalexySpeedzw.z , ( _ScalexySpeedzw.w / 2.0 )));
			float simplePerlin2D18 = snoise( (i.uv_texcoord*appendResult22 + ( appendResult23 * _Time.y ))*_Scale );
			simplePerlin2D18 = simplePerlin2D18*0.5 + 0.5;
			float Mask31 = ( 1.0 - saturate( ( ( 1.0 - tex2D( _Fire_Mask, i.uv_texcoord ).r ) + saturate( ( (0.0 + (voroi10 - 0.25) * (1.0 - 0.0) / (1.0 - 0.25)) * (0.0 + (simplePerlin2D18 - 0.2) * (1.0 - 0.0) / (1.0 - 0.2)) ) ) ) ) );
			float2 appendResult39 = (float2(Mask31 , 0.5));
			float4 tex2DNode38 = tex2D( _Ramp, appendResult39 );
			o.Emission = ( tex2DNode38 * _Emission ).rgb;
			o.Alpha = 1;
			clip( tex2DNode38.r - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;30;-3367.288,-210;Inherit;False;2518.307;839.2053;;26;25;22;23;76;77;27;18;26;6;15;31;35;34;29;33;19;28;10;1;8;7;16;2;17;78;79;Mask;0,0,0,1;0;0
Node;AmplifyShaderEditor.Vector4Node;15;-3312,240;Inherit;False;Property;_ScalexySpeedzw;Scale(xy)/Speed(zw);4;0;Create;True;0;0;0;False;0;False;4,2,0,-1.5;4,2,0,-1.5;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;77;-2960,480;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;17;-2848,192;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;6;-3312,416;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;76;-2960,384;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;23;-2848,480;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-3072,-144;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;16;-2848,96;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-2672,192;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-2848,384;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-2688,480;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;8;-2464,48;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;1,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;26;-2496,384;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;1,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-2482.201,256.6732;Inherit;False;Property;_Scale;Scale;3;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;10;-2240,48;Inherit;True;0;0;1;1;1;False;1;False;True;False;4;0;FLOAT2;0,0;False;1;FLOAT;45;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.NoiseGeneratorNode;18;-2288,384;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;28;-2048,48;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.25;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;27;-2048,384;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.2;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;53;-2144,1168;Inherit;False;1306.397;329.8048;;8;70;47;48;50;49;73;61;71;Wave;0,0,0,1;0;0
Node;AmplifyShaderEditor.SamplerNode;1;-2256,-160;Inherit;True;Property;_Fire_Mask;Fire_Mask;1;0;Create;True;0;0;0;False;0;False;-1;None;7449aa8becaeaa14fad8d40307e17a45;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-1776,160;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;49;-2112,1232;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;33;-1664,-112;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;78;-1584,160;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-1872,1232;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-1472,-48;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;48;-1728,1232;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;34;-1360,-48;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1600,1232;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;70;-1664,1344;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;35;-1216,-48;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-1424,1280;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-1056,-48;Inherit;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;71;-1264,1280;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;32;-800,-64;Inherit;True;31;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;-1088,1280;Inherit;False;Wave;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;-592,-64;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;65;-560,352;Inherit;False;61;Wave;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-528,448;Inherit;False;Property;_WaveDst;WaveDst;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-98.52692,146.0349;Inherit;False;Property;_Emission;Emission;6;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;38;-432,-64;Inherit;True;Property;_Ramp;Ramp;2;1;[HDR];Create;True;0;0;0;False;0;False;-1;400e14bdc22133e42882f145972ab384;400e14bdc22133e42882f145972ab384;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-336,352;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;36;-1856,-512;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.05;False;2;FLOAT;0.9;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;192,-16;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;480,-16;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Fire;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.15;True;True;0;True;TransparentCutout;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;77;0;15;4
WireConnection;17;0;15;3
WireConnection;17;1;15;4
WireConnection;76;0;15;1
WireConnection;23;0;15;3
WireConnection;23;1;77;0
WireConnection;16;0;15;1
WireConnection;16;1;15;2
WireConnection;7;0;17;0
WireConnection;7;1;6;2
WireConnection;22;0;76;0
WireConnection;22;1;15;2
WireConnection;25;0;23;0
WireConnection;25;1;6;2
WireConnection;8;0;2;0
WireConnection;8;1;16;0
WireConnection;8;2;7;0
WireConnection;26;0;2;0
WireConnection;26;1;22;0
WireConnection;26;2;25;0
WireConnection;10;0;8;0
WireConnection;10;2;79;0
WireConnection;18;0;26;0
WireConnection;18;1;79;0
WireConnection;28;0;10;0
WireConnection;27;0;18;0
WireConnection;1;1;2;0
WireConnection;19;0;28;0
WireConnection;19;1;27;0
WireConnection;33;0;1;1
WireConnection;78;0;19;0
WireConnection;50;0;49;2
WireConnection;29;0;33;0
WireConnection;29;1;78;0
WireConnection;48;0;50;0
WireConnection;34;0;29;0
WireConnection;47;0;48;0
WireConnection;35;0;34;0
WireConnection;73;0;47;0
WireConnection;73;1;70;2
WireConnection;31;0;35;0
WireConnection;71;0;73;0
WireConnection;61;0;71;0
WireConnection;39;0;32;0
WireConnection;38;1;39;0
WireConnection;75;0;65;0
WireConnection;75;1;74;0
WireConnection;80;0;38;0
WireConnection;80;1;81;0
WireConnection;0;2;80;0
WireConnection;0;10;38;1
WireConnection;0;11;75;0
ASEEND*/
//CHKSM=F3BEB7D3431550A8F9985C38B63F97ECB1FF82FD