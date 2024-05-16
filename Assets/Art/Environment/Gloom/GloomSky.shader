// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GloomSky"
{
	Properties
	{
		_GlobalContrast("GlobalContrast", Range( 0 , 10)) = 1
		_GlobalHueShift("GlobalHueShift", Range( 0 , 1)) = 1
		_GlobalBrightness("GlobalBrightness", Range( 0 , 2)) = 1
		_GlobalSaturationFactor("GlobalSaturationFactor", Range( 0 , 1)) = 1
		_SkySize("SkySize", Float) = 100
		_TimeScale("TimeScale", Float) = 1
		[SingleLineTexture]_SkyVerticalRamp("SkyVerticalRamp", 2D) = "white" {}
		_ShiftSkyVertical("ShiftSkyVertical", Range( -1 , 1)) = 0
		[SingleLineTexture]_SkyHorizontalRamp("SkyHorizontalRamp", 2D) = "white" {}
		_ShiftSkyHorizontal("ShiftSkyHorizontal", Range( -1 , 1)) = 0
		[SingleLineTexture]_StarRamp("StarRamp", 2D) = "white" {}
		_StarsBlend("StarsBlend", Range( 0 , 1)) = 0.75
		_StarRemapMin("StarRemapMin", Range( 0 , 1)) = 0.9
		_StarScale("StarScale", Float) = 75
		[SingleLineTexture]_CloudRamp("CloudRamp", 2D) = "white" {}
		_CloudAlpha("CloudAlpha", Range( 0 , 1)) = 1
		_CloudMin("CloudMin", Range( 0 , 0.499)) = 0.1082492
		_CloudMax("CloudMax", Range( 0.499 , 1)) = 0.8364004
		_CloudSize("CloudSize", Float) = 10
		_CloudSquash("CloudSquash", Float) = 3
		_CloudSmoothness("CloudSmoothness", Range( 0 , 1)) = 0.05
		_DistSize("DistSize", Float) = 15
		_DistMul("DistMul", Float) = 0.5
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float3 worldPos;
		};

		uniform float _GlobalContrast;
		uniform sampler2D _CloudRamp;
		uniform float _CloudSize;
		uniform float _SkySize;
		uniform float _CloudSmoothness;
		uniform float _TimeScale;
		uniform float _DistSize;
		uniform float _DistMul;
		uniform float _CloudSquash;
		uniform float _CloudMin;
		uniform float _CloudMax;
		uniform float _CloudAlpha;
		uniform sampler2D _StarRamp;
		uniform float _StarScale;
		uniform float _StarRemapMin;
		uniform sampler2D _SkyHorizontalRamp;
		uniform float _ShiftSkyHorizontal;
		uniform sampler2D _SkyVerticalRamp;
		uniform float _ShiftSkyVertical;
		uniform float _StarsBlend;
		uniform float _GlobalHueShift;
		uniform float _GlobalSaturationFactor;
		uniform float _GlobalBrightness;


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

		float2 voronoihash92( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi92( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash92( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.707 * sqrt(dot( r, r ));
			 //		if( d<F1 ) {
			 //			F2 = F1;
			 			float h = smoothstep(0.0, 1.0, 0.5 + 0.5 * (F1 - d) / smoothness); F1 = lerp(F1, d, h) - smoothness * h * (1.0 - h);mg = g; mr = r; id = o;
			 //		} else if( d<F2 ) {
			 //			F2 = d;
			
			 //		}
			 	}
			}
			return F1;
		}


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


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float SkySize69 = _SkySize;
			float time92 = 0.0;
			float2 voronoiSmoothId92 = 0;
			float voronoiSmooth92 = _CloudSmoothness;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 objToWorld38 = mul( unity_ObjectToWorld, float4( ase_vertex3Pos, 1 ) ).xyz;
			float VertWorldPos_X78 = objToWorld38.x;
			float VertWorldPos_Y79 = objToWorld38.y;
			float mulTime52 = _Time.y * _TimeScale;
			float3 VertexWorldPos49 = objToWorld38;
			float3 break53 = VertexWorldPos49;
			float TimeScaled55 = ( mulTime52 + break53.y + break53.z );
			float2 appendResult64 = (float2(VertWorldPos_X78 , ( VertWorldPos_Y79 - TimeScaled55 )));
			float2 WorldUVScrolling74 = appendResult64;
			float3 appendResult39 = (float3(VertWorldPos_X78 , ( VertWorldPos_Y79 - ( 1.0 - TimeScaled55 ) ) , TimeScaled55));
			float3 DistUVW75 = appendResult39;
			float simplePerlin3D44 = snoise( DistUVW75*( _DistSize / SkySize69 ) );
			simplePerlin3D44 = simplePerlin3D44*0.5 + 0.5;
			float Dist85 = ( simplePerlin3D44 * _DistMul );
			float2 break115 = ( WorldUVScrolling74 + Dist85 );
			float2 appendResult116 = (float2(break115.x , ( break115.y * _CloudSquash )));
			float2 coords92 = appendResult116 * ( _CloudSize / SkySize69 );
			float2 id92 = 0;
			float2 uv92 = 0;
			float fade92 = 0.5;
			float voroi92 = 0;
			float rest92 = 0;
			for( int it92 = 0; it92 <2; it92++ ){
			voroi92 += fade92 * voronoi92( coords92, time92, id92, uv92, voronoiSmooth92,voronoiSmoothId92 );
			rest92 += fade92;
			coords92 *= 2;
			fade92 *= 0.5;
			}//Voronoi92
			voroi92 /= rest92;
			float Clouds147 = (0.0 + (voroi92 - _CloudMin) * (_CloudAlpha - 0.0) / (_CloudMax - _CloudMin));
			float2 appendResult109 = (float2(Clouds147 , abs( ase_vertex3Pos.x )));
			float4 CloudsColor148 = tex2D( _CloudRamp, appendResult109 );
			float3 appendResult122 = (float3(( WorldUVScrolling74 + Dist85 ) , ( sin( TimeScaled55 ) * 0.25 )));
			float simplePerlin3D40 = snoise( appendResult122*( _StarScale / SkySize69 ) );
			simplePerlin3D40 = simplePerlin3D40*0.5 + 0.5;
			float Stars138 = (0.0 + (simplePerlin3D40 - _StarRemapMin) * (1.0 - 0.0) / (1.0 - _StarRemapMin));
			float2 appendResult129 = (float2(Stars138 , (0.0 + (sin( abs( ase_vertex3Pos.x ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0))));
			float4 StarsColor140 = saturate( ( tex2D( _StarRamp, appendResult129 ) * Stars138 ) );
			float2 appendResult19 = (float2((0.0 + (abs( ase_vertex3Pos.x ) - 0.0) * (1.0 - 0.0) / ((0.001 + (( 1.0 - _ShiftSkyHorizontal ) - 0.0) * (0.499 - 0.001) / (1.0 - 0.0)) - 0.0)) , 0.5));
			float4 SkyHorizontalRamp25 = tex2D( _SkyHorizontalRamp, appendResult19 );
			float2 appendResult3 = (float2((0.0 + (ase_vertex3Pos.y - 0.0) * (1.0 - 0.0) / ((0.001 + (( 1.0 - _ShiftSkyVertical ) - 0.0) * (0.499 - 0.001) / (1.0 - 0.0)) - 0.0)) , 0.5));
			float4 SkyVerticalRamp24 = tex2D( _SkyVerticalRamp, appendResult3 );
			float saferPower33 = abs( (0.0 + (0.0 - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) );
			float4 lerpResult29 = lerp( SkyHorizontalRamp25 , SkyVerticalRamp24 , pow( saferPower33 , 1.0 ));
			float4 SkyColor145 = lerpResult29;
			float4 blendOpSrc43 = StarsColor140;
			float4 blendOpDest43 = SkyColor145;
			float4 lerpBlendMode43 = lerp(blendOpDest43,abs( blendOpSrc43 - blendOpDest43 ),_StarsBlend);
			float4 blendOpSrc97 = CloudsColor148;
			float4 blendOpDest97 = ( saturate( lerpBlendMode43 ));
			float4 lerpBlendMode97 = lerp(blendOpDest97,( blendOpSrc97 + blendOpDest97 ),Clouds147);
			float3 hsvTorgb163 = RGBToHSV( ( saturate( lerpBlendMode97 )).rgb );
			float3 hsvTorgb167 = HSVToRGB( float3(fmod( ( hsvTorgb163.x + _GlobalHueShift ) , 1.0 ),( hsvTorgb163.y * _GlobalSaturationFactor ),hsvTorgb163.z) );
			o.Emission = ( CalculateContrast(_GlobalContrast,float4( hsvTorgb167 , 0.0 )) * _GlobalBrightness ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;159;-5104,-1344;Inherit;False;793.7598;392.8765;;5;78;79;38;36;49;World Space Vert Pos;0,0,0,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;36;-5024,-1200;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TransformPositionNode;38;-4848,-1200;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;48;-5184,-1920;Inherit;False;788;307;;6;55;54;53;52;51;50;Time Scaled;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-4592,-1264;Inherit;False;VertexWorldPos;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-5136,-1872;Inherit;False;Property;_TimeScale;TimeScale;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-5136,-1776;Inherit;False;49;VertexWorldPos;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;52;-4992,-1872;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;53;-4928,-1776;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-4768,-1824;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;156;-5216,-96;Inherit;False;980;323;;7;77;66;82;67;83;39;75;Distortion UVW;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;55;-4640,-1824;Inherit;False;TimeScaled;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-5152,-48;Inherit;False;55;TimeScaled;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-4592,-1104;Inherit;False;VertWorldPos_Y;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;158;-4160,-1936;Inherit;False;436;163;;2;68;69;SkySize;0,0,0,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;66;-4912,112;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-4976,32;Inherit;False;79;VertWorldPos_Y;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;-4592,-1184;Inherit;False;VertWorldPos_X;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-4112,-1888;Inherit;False;Property;_SkySize;SkySize;4;0;Create;True;0;0;0;False;0;False;100;150;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;67;-4752,80;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-4832,-48;Inherit;False;78;VertWorldPos_X;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;157;-4000,-112;Inherit;False;1012;387;;8;70;71;76;72;44;73;61;85;Distortion;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-3968,-1888;Inherit;False;SkySize;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;-4608,-48;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;160;-5136,-704;Inherit;False;788;323;;6;56;81;63;80;64;74;ScrollingWorldSpaceVertPos;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-4464,-48;Inherit;False;DistUVW;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;-3936,112;Inherit;False;69;SkySize;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-3936,32;Inherit;False;Property;_DistSize;DistSize;21;0;Create;True;0;0;0;False;0;False;15;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;56;-5040,-496;Inherit;False;55;TimeScaled;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-5072,-576;Inherit;False;79;VertWorldPos_Y;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;76;-3824,-64;Inherit;False;75;DistUVW;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;72;-3760,32;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;63;-4864,-576;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-4928,-656;Inherit;False;78;VertWorldPos_X;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;44;-3632,-64;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-3552,160;Inherit;False;Property;_DistMul;DistMul;22;0;Create;True;0;0;0;False;0;False;0.5;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;133;-5264,1552;Inherit;False;1458.048;555.0562;;14;138;41;42;40;122;88;60;89;87;84;86;124;123;126;Stars;0,0,0,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;64;-4720,-640;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-3376,-16;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;123;-5216,1792;Inherit;False;55;TimeScaled;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-4576,-640;Inherit;False;WorldUVScrolling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;-3216,-16;Inherit;False;Dist;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;151;-5296,624;Inherit;False;1860;595;;17;110;111;112;115;118;94;95;117;96;116;105;92;119;99;91;147;103;Clouds;0,0,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-5072,1696;Inherit;False;85;Dist;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;-5104,1616;Inherit;False;74;WorldUVScrolling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;126;-5040,1792;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;-5216,1008;Inherit;False;85;Dist;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;111;-5248,928;Inherit;False;74;WorldUVScrolling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;23;-7104,144;Inherit;False;1576.459;620.4832;;9;24;3;8;2;11;12;9;106;143;Sky Vertical Ramp;0,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;22;-7136,1072;Inherit;False;1570.77;566.2717;;10;25;19;16;28;15;17;14;18;107;144;Sky Horizontal Ramp;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-4896,1904;Inherit;False;Property;_StarScale;StarScale;13;0;Create;True;0;0;0;False;0;False;75;75;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;-4928,1984;Inherit;False;69;SkySize;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-4896,1616;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-4928,1792;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;112;-5040,928;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-7088,1440;Inherit;False;Property;_ShiftSkyHorizontal;ShiftSkyHorizontal;9;0;Create;True;0;0;0;False;0;False;0;-0.55;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-7056,512;Inherit;False;Property;_ShiftSkyVertical;ShiftSkyVertical;7;0;Create;True;0;0;0;False;0;False;0;0.4;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;88;-4736,1888;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;122;-4768,1680;Inherit;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;115;-4928,928;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;118;-5216,1088;Inherit;False;Property;_CloudSquash;CloudSquash;19;0;Create;True;0;0;0;False;0;False;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;14;-6816,1440;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;17;-6848,1280;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;12;-6784,512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;40;-4576,1760;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-4624,1984;Inherit;False;Property;_StarRemapMin;StarRemapMin;12;0;Create;True;0;0;0;False;0;False;0.9;0.909;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;142;-3632,1584;Inherit;False;1508;357;;9;129;128;137;139;140;178;179;135;136;Stars Color;0,0,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;-4640,848;Inherit;False;69;SkySize;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-4624,768;Inherit;False;Property;_CloudSize;CloudSize;18;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-4768,992;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;15;-6656,1440;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.001;False;4;FLOAT;0.499;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;28;-6656,1312;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;11;-6624,512;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.001;False;4;FLOAT;0.499;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;2;-6624,352;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;41;-4320,1760;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;179;-3600,1760;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;96;-4464,768;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;116;-4608,928;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-4624,1072;Inherit;False;Property;_CloudSmoothness;CloudSmoothness;20;0;Create;True;0;0;0;False;0;False;0.05;0.15;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;16;-6448,1344;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;8;-6416,416;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;-4048,1776;Inherit;False;Stars;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;178;-3424,1760;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-4304,768;Inherit;False;Property;_CloudMax;CloudMax;17;0;Create;True;0;0;0;False;0;False;0.8364004;0.65;0.499;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-4304,1120;Inherit;False;Property;_CloudAlpha;CloudAlpha;15;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-4304,688;Inherit;False;Property;_CloudMin;CloudMin;16;0;Create;True;0;0;0;False;0;False;0.1082492;0.383;0;0.499;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;92;-4304,848;Inherit;True;0;1;5;0;2;False;1;False;True;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.CommentaryNode;35;-7136,1888;Inherit;False;1143.52;406.5989;;8;145;29;33;26;27;34;32;21;Sky Color;0,0,0,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;19;-6240,1344;Inherit;False;FLOAT2;4;0;FLOAT;0.5;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;3;-6208,416;Inherit;False;FLOAT2;4;0;FLOAT;0.5;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;107;-6320,1136;Inherit;True;Property;_SkyHorizontalRamp;SkyHorizontalRamp;8;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;None;2e6499e1040517545a3b427e331ad883;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;106;-6288,224;Inherit;True;Property;_SkyVerticalRamp;SkyVerticalRamp;6;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;None;1a7086b20631ee34b94a42c5fe636409;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;139;-3456,1632;Inherit;False;138;Stars;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;135;-3312,1760;Inherit;False;Sin01;-1;;2;dfa74d9ebd1bfcc429ab1d37e2a53901;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;150;-3355,608;Inherit;False;1189.771;471.7257;;7;148;108;109;113;149;175;174;Clouds Color;0,0,0,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;91;-3904,800;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-7088,2096;Inherit;False;Constant;_RampBlend;RampBlend;4;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;143;-6048,400;Inherit;True;Property;_TextureSample1;Texture Sample 1;21;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;144;-6080,1344;Inherit;True;Property;_TextureSample2;Texture Sample 2;20;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;129;-3168,1712;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;147;-3680,896;Inherit;False;Clouds;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;32;-6816,2096;Inherit;False;RS01;-1;;4;234e25958dfbf004a8d8c89023d992c1;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-6912,2176;Inherit;False;Constant;_BlendPower;BlendPower;4;0;Create;True;0;0;0;False;0;False;1;0;0.01;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-5792,1344;Inherit;False;SkyHorizontalRamp;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;174;-3328,928;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;128;-3008,1712;Inherit;True;Property;_StarRamp;StarRamp;10;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;bfe81db5e0a887a41b76d0da3a8e5dc5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-5744,416;Inherit;False;SkyVerticalRamp;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;149;-3280,848;Inherit;False;147;Clouds;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;33;-6624,2096;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;175;-3152,928;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;-2704,1760;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-6688,1936;Inherit;False;25;SkyHorizontalRamp;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;26;-6656,2016;Inherit;False;24;SkyVerticalRamp;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;113;-3184,656;Inherit;True;Property;_CloudRamp;CloudRamp;14;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;None;5065efe6f2338d84195fe0edd86ba16c;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.DynamicAppendNode;109;-2864,816;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;137;-2528,1760;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;29;-6400,2000;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;108;-2656,752;Inherit;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;155;-1028.592,144;Inherit;False;2451.037;677.4854;;19;0;130;131;167;169;166;168;165;162;163;97;153;152;43;141;146;127;176;177;Result;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;140;-2368,1760;Inherit;False;StarsColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;145;-6224,2016;Inherit;False;SkyColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-992,496;Inherit;False;Property;_StarsBlend;StarsBlend;11;0;Create;True;0;0;0;False;0;False;0.75;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;146;-896,416;Inherit;False;145;SkyColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;141;-896,336;Inherit;False;140;StarsColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;148;-2368,752;Inherit;False;CloudsColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;-608,288;Inherit;False;147;Clouds;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;153;-608,208;Inherit;False;148;CloudsColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;43;-672,368;Inherit;True;Difference;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.51;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;97;-384,288;Inherit;True;LinearDodge;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RGBToHSVNode;163;-96,432;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;162;-160,592;Inherit;False;Property;_GlobalHueShift;GlobalHueShift;1;0;Create;True;0;0;0;False;0;False;1;0.918;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;165;224,432;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;168;-160,688;Inherit;False;Property;_GlobalSaturationFactor;GlobalSaturationFactor;3;0;Create;True;0;0;0;False;0;False;1;0.914;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FmodOpNode;166;336,432;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;176,656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;167;464,432;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;131;384,688;Inherit;False;Property;_GlobalContrast;GlobalContrast;0;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;130;688,432;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;177;688,688;Inherit;False;Property;_GlobalBrightness;GlobalBrightness;2;0;Create;True;0;0;0;False;0;False;1;1.38;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;928,432;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1136,352;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;GloomSky;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;38;0;36;0
WireConnection;49;0;38;0
WireConnection;52;0;50;0
WireConnection;53;0;51;0
WireConnection;54;0;52;0
WireConnection;54;1;53;1
WireConnection;54;2;53;2
WireConnection;55;0;54;0
WireConnection;79;0;38;2
WireConnection;66;0;77;0
WireConnection;78;0;38;1
WireConnection;67;0;82;0
WireConnection;67;1;66;0
WireConnection;69;0;68;0
WireConnection;39;0;83;0
WireConnection;39;1;67;0
WireConnection;39;2;77;0
WireConnection;75;0;39;0
WireConnection;72;0;71;0
WireConnection;72;1;70;0
WireConnection;63;0;81;0
WireConnection;63;1;56;0
WireConnection;44;0;76;0
WireConnection;44;1;72;0
WireConnection;64;0;80;0
WireConnection;64;1;63;0
WireConnection;61;0;44;0
WireConnection;61;1;73;0
WireConnection;74;0;64;0
WireConnection;85;0;61;0
WireConnection;126;0;123;0
WireConnection;60;0;84;0
WireConnection;60;1;86;0
WireConnection;124;0;126;0
WireConnection;112;0;111;0
WireConnection;112;1;110;0
WireConnection;88;0;87;0
WireConnection;88;1;89;0
WireConnection;122;0;60;0
WireConnection;122;2;124;0
WireConnection;115;0;112;0
WireConnection;14;0;18;0
WireConnection;12;0;9;0
WireConnection;40;0;122;0
WireConnection;40;1;88;0
WireConnection;117;0;115;1
WireConnection;117;1;118;0
WireConnection;15;0;14;0
WireConnection;28;0;17;1
WireConnection;11;0;12;0
WireConnection;41;0;40;0
WireConnection;41;1;42;0
WireConnection;96;0;95;0
WireConnection;96;1;94;0
WireConnection;116;0;115;0
WireConnection;116;1;117;0
WireConnection;16;0;28;0
WireConnection;16;2;15;0
WireConnection;8;0;2;2
WireConnection;8;2;11;0
WireConnection;138;0;41;0
WireConnection;178;0;179;1
WireConnection;92;0;116;0
WireConnection;92;2;96;0
WireConnection;92;3;105;0
WireConnection;19;0;16;0
WireConnection;3;0;8;0
WireConnection;135;1;178;0
WireConnection;91;0;92;0
WireConnection;91;1;103;0
WireConnection;91;2;119;0
WireConnection;91;4;99;0
WireConnection;143;0;106;0
WireConnection;143;1;3;0
WireConnection;144;0;107;0
WireConnection;144;1;19;0
WireConnection;129;0;139;0
WireConnection;129;1;135;0
WireConnection;147;0;91;0
WireConnection;32;1;21;0
WireConnection;25;0;144;0
WireConnection;128;1;129;0
WireConnection;24;0;143;0
WireConnection;33;0;32;0
WireConnection;33;1;34;0
WireConnection;175;0;174;1
WireConnection;136;0;128;0
WireConnection;136;1;139;0
WireConnection;109;0;149;0
WireConnection;109;1;175;0
WireConnection;137;0;136;0
WireConnection;29;0;27;0
WireConnection;29;1;26;0
WireConnection;29;2;33;0
WireConnection;108;0;113;0
WireConnection;108;1;109;0
WireConnection;140;0;137;0
WireConnection;145;0;29;0
WireConnection;148;0;108;0
WireConnection;43;0;141;0
WireConnection;43;1;146;0
WireConnection;43;2;127;0
WireConnection;97;0;153;0
WireConnection;97;1;43;0
WireConnection;97;2;152;0
WireConnection;163;0;97;0
WireConnection;165;0;163;1
WireConnection;165;1;162;0
WireConnection;166;0;165;0
WireConnection;169;0;163;2
WireConnection;169;1;168;0
WireConnection;167;0;166;0
WireConnection;167;1;169;0
WireConnection;167;2;163;3
WireConnection;130;1;167;0
WireConnection;130;0;131;0
WireConnection;176;0;130;0
WireConnection;176;1;177;0
WireConnection;0;2;176;0
ASEEND*/
//CHKSM=8FE411CFB890333CF9FCD633B4D19EDA38B7D1F1