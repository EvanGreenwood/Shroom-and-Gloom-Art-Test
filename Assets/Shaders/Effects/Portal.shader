// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Portal"
{
	Properties
	{
		_InsideTex("InsideTex", 2D) = "white" {}
		_GlowColor("GlowColor", Color) = (0,0.6498157,0.7600917,1)
		_Speed("Speed", Float) = 0.1
		_Distortion("Distortion", Range( 0 , 1)) = 0
		_InsideColor("InsideColor", Color) = (0,0,0,0)
		_NoiseScale("NoiseScale", Float) = 5
		_InsideGlowAlpha("InsideGlowAlpha", Range( 0 , 1)) = 1
		_InsideGlowColor("InsideGlowColor", Color) = (1,0,0,1)
		_InsideGlowSize("InsideGlowSize", Range( 0 , 0.99)) = 0
		_PerimeterGlowAlpha("PerimeterGlowAlpha", Range( 0 , 1)) = 1
		_NormalStrength("NormalStrength", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform float _Speed;
		uniform float _NoiseScale;
		uniform float _InsideGlowSize;
		uniform float4 _InsideGlowColor;
		uniform float4 _InsideColor;
		uniform sampler2D _InsideTex;
		uniform float _Distortion;
		uniform float _InsideGlowAlpha;
		uniform float4 _GlowColor;
		uniform float _PerimeterGlowAlpha;
		uniform float _NormalStrength;


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


		float3 PerturbNormal107_g2( float3 surf_pos, float3 surf_norm, float height, float scale )
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


		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 surf_pos107_g2 = ase_worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 surf_norm107_g2 = ase_worldNormal;
			float2 UV87 = i.uv_texcoord;
			float CircleMask68 = ( 1.0 - length( (float2( -1,-1 ) + (UV87 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) ) );
			float2 center45_g1 = float2( 0.5,0.5 );
			float2 delta6_g1 = ( (UV87*1.0 + 0.0) - center45_g1 );
			float angle10_g1 = ( length( delta6_g1 ) * 10.0 );
			float x23_g1 = ( ( cos( angle10_g1 ) * delta6_g1.x ) - ( sin( angle10_g1 ) * delta6_g1.y ) );
			float2 break40_g1 = center45_g1;
			float2 break41_g1 = float2( 0,0 );
			float y35_g1 = ( ( sin( angle10_g1 ) * delta6_g1.x ) + ( cos( angle10_g1 ) * delta6_g1.y ) );
			float2 appendResult44_g1 = (float2(( x23_g1 + break40_g1.x + break41_g1.x ) , ( break40_g1.y + break41_g1.y + y35_g1 )));
			float Time84 = ( _Time.y * _Speed );
			float cos9 = cos( Time84 );
			float sin9 = sin( Time84 );
			float2 rotator9 = mul( appendResult44_g1 - float2( 0.5,0.5 ) , float2x2( cos9 , -sin9 , sin9 , cos9 )) + float2( 0.5,0.5 );
			float3 appendResult82 = (float3(rotator9 , ( Time84 * 0.15 )));
			float simplePerlin3D5 = snoise( appendResult82*_NoiseScale );
			simplePerlin3D5 = simplePerlin3D5*0.5 + 0.5;
			float Noise35 = simplePerlin3D5;
			float Mask18 = saturate( ( (0.0 + (CircleMask68 - 0.25) * (1.0 - 0.0) / (0.85 - 0.25)) + ( CircleMask68 * Noise35 ) ) );
			float2 temp_cast_0 = (Noise35).xx;
			float temp_output_58_0 = saturate( ( Noise35 - CircleMask68 ) );
			float2 lerpResult55 = lerp( i.uv_texcoord , ( i.uv_texcoord - temp_cast_0 ) , ( _Distortion * temp_output_58_0 ));
			float2 DistortedUV70 = lerpResult55;
			float DistortionMask94 = temp_output_58_0;
			float4 lerpResult77 = lerp( _InsideColor , tex2D( _InsideTex, DistortedUV70 ) , DistortionMask94);
			float4 FinalInsideTex116 = lerpResult77;
			float4 blendOpSrc121 = ( (0.0 + (( 1.0 - Mask18 ) - _InsideGlowSize) * (1.0 - 0.0) / (1.0 - _InsideGlowSize)) * _InsideGlowColor );
			float4 blendOpDest121 = FinalInsideTex116;
			float4 lerpBlendMode121 = lerp(blendOpDest121,	max( blendOpSrc121, blendOpDest121 ),_InsideGlowAlpha);
			float FinalOpacity102 = (0.0 + (Mask18 - 0.45) * (1.0 - 0.0) / (0.5 - 0.45));
			float PerimeterGlow37 = saturate( ( 1.0 - ( (0.0 + (( (0.0 + (( 1.0 - FinalOpacity102 ) - 0.1) * (1.0 - 0.0) / (1.0 - 0.1)) * (0.0 + (FinalOpacity102 - 0.0) * (1.0 - 0.0) / (-6.14 - 0.0)) ) - 0.0) * (1.0 - 0.0) / (0.1 - 0.0)) - (0.0 + (Noise35 - 0.84) * (1.0 - 0.0) / (1.0 - 0.84)) ) ) );
			float4 PerimaterGlowColor106 = ( PerimeterGlow37 * _GlowColor );
			float4 lerpResult44 = lerp( ( saturate( lerpBlendMode121 )) , PerimaterGlowColor106 , ( saturate( PerimeterGlow37 ) * _PerimeterGlowAlpha ));
			float4 FinalColor111 = lerpResult44;
			float grayscale132 = Luminance(FinalColor111.rgb);
			float height107_g2 = ( 1.0 - (0.0 + (saturate( grayscale132 ) - 0.0) * (1.0 - 0.0) / (0.03 - 0.0)) );
			float scale107_g2 = _NormalStrength;
			float3 localPerturbNormal107_g2 = PerturbNormal107_g2( surf_pos107_g2 , surf_norm107_g2 , height107_g2 , scale107_g2 );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentDir42_g2 = mul( ase_worldToTangent, localPerturbNormal107_g2);
			o.Normal = worldToTangentDir42_g2;
			o.Albedo = FinalColor111.rgb;
			float Opacity105 = saturate( FinalOpacity102 );
			o.Alpha = Opacity105;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Lambert alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;93;-4848,-512;Inherit;False;660;483;;6;6;87;14;48;24;84;UV / Time;0,0.07041359,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-4752,-464;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;87;-4528,-464;Inherit;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;73;-4864,512;Inherit;False;1667.178;460.1678;;14;88;86;8;50;85;82;83;49;7;5;35;9;81;101;Noise;0,0,0,1;0;0
Node;AmplifyShaderEditor.TimeNode;14;-4800,-304;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;-4736,-144;Inherit;False;Property;_Speed;Speed;2;0;Create;True;0;0;0;False;0;False;0.1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-4576,-304;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;-4704,576;Inherit;False;87;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;92;-4850,94;Inherit;False;1029.125;291.6045;;5;68;12;10;11;91;Circle Mask;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-4608,880;Inherit;False;Constant;_2;2;2;0;Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;-4432,-304;Inherit;False;Time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;49;-4496,576;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-4800,144;Inherit;False;87;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;7;-4240,576;Inherit;False;Twirl;-1;;1;90936742ac32db8449cd21ab6dd337c8;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT;0;False;4;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-4240,720;Inherit;False;84;Time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;11;-4592,144;Inherit;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;-1,-1;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;9;-4048,576;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-4048,704;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.15;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;10;-4400,144;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-3904,832;Inherit;False;Property;_NoiseScale;NoiseScale;5;0;Create;True;0;0;0;False;0;False;5;7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;82;-3840,592;Inherit;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;12;-4240,144;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;5;-3648,576;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;90;-2736,320;Inherit;False;1268;480;;7;72;13;17;16;21;18;89;TwirlMask;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;35;-3408,576;Inherit;False;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;-4048,144;Inherit;False;CircleMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;72;-2688,448;Inherit;False;68;CircleMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;-2688,528;Inherit;False;35;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-2432,544;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;17;-2416,368;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.25;False;2;FLOAT;0.85;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-2112,416;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;21;-1888,416;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;114;-816,-176;Inherit;False;1213.058;434.1152;;8;105;98;97;100;113;102;96;20;Opacity;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-1712,416;Inherit;True;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-768,16;Inherit;False;18;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;96;-576,16;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.45;False;2;FLOAT;0.5;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;74;-3040,-656;Inherit;False;1923;764;;13;127;34;25;37;47;46;33;36;32;30;51;27;104;PerimeterGlow;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-320,16;Inherit;False;FinalOpacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;75;-2880,-1792;Inherit;False;1476;528;;11;52;69;61;53;58;56;63;64;55;70;94;DistortedUV;0,0,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;-2992,-576;Inherit;True;102;FinalOpacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-2832,-1648;Inherit;False;35;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;69;-2832,-1568;Inherit;False;68;CircleMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;27;-2736,-352;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-2576,-1520;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;51;-2544,-352;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;30;-2592,-576;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;-6.14;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;53;-2464,-1744;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;58;-2368,-1520;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-2480,-1600;Inherit;False;Property;_Distortion;Distortion;3;0;Create;True;0;0;0;False;0;False;0;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-2288,-448;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;-2256,-224;Inherit;False;35;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;63;-2112,-1680;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-2064,-1472;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;33;-2064,-448;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;46;-2064,-224;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.84;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;55;-1808,-1744;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;34;-1824,-448;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;110;-988.2496,-1888;Inherit;False;1862.277;1380.958;;28;111;44;121;109;107;108;122;124;117;106;123;119;116;40;118;77;41;38;95;80;1;2;71;125;126;128;129;132;Color;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-1648,-1744;Inherit;True;DistortedUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;127;-1632,-448;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;94;-2128,-1360;Inherit;False;DistortionMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;-768,-1456;Inherit;False;70;DistortedUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-800,-1648;Inherit;True;Property;_InsideTex;InsideTex;0;0;Create;True;0;0;0;False;0;False;None;756e86ec23b92fb418cf96f840d0ef75;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SaturateNode;47;-1488,-448;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-560,-1648;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;80;-496,-1840;Inherit;False;Property;_InsideColor;InsideColor;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0.05490187,0.01747464,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;95;-480,-1456;Inherit;False;94;DistortionMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;118;-928,-992;Inherit;True;18;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-1344,-448;Inherit;True;PerimeterGlow;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;176,-1824;Inherit;False;37;PerimeterGlow;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;41;176,-1744;Inherit;False;Property;_GlowColor;GlowColor;1;0;Create;True;0;0;0;False;0;False;0,0.6498157,0.7600917,1;1,0.8999875,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;77;-240,-1696;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;119;-736,-992;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;126;-896,-752;Inherit;False;Property;_InsideGlowSize;InsideGlowSize;8;0;Create;True;0;0;0;False;0;False;0;0.18;0;0.99;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;400,-1824;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;116;-96,-1696;Inherit;False;FinalInsideTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;123;-608,-784;Inherit;False;Property;_InsideGlowColor;InsideGlowColor;7;0;Create;True;0;0;0;False;0;False;1,0,0,1;0.1474042,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;125;-576,-992;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;-160,-832;Inherit;False;37;PerimeterGlow;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;106;608,-1824;Inherit;False;PerimaterGlowColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;117;-640,-1168;Inherit;False;116;FinalInsideTex;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-704,-1088;Inherit;False;Property;_InsideGlowAlpha;InsideGlowAlpha;6;0;Create;True;0;0;0;False;0;False;1;0.16;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-400,-1008;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0.5,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;109;48,-832;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;128;-80,-752;Inherit;False;Property;_PerimeterGlowAlpha;PerimeterGlowAlpha;9;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;107;112,-992;Inherit;False;106;PerimaterGlowColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;121;-176,-1120;Inherit;True;Lighten;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.06;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;208,-832;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;44;416,-1056;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;111;656,-1056;Inherit;False;FinalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;132;688,-848;Inherit;True;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;134;752,-912;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;97;-112,16;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;135;912,-848;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.03;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;76;1444.762,-752;Inherit;False;495.2161;534.0409;;3;112;115;0;Result;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;105;160,16;Inherit;True;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;131;882.4597,-408.525;Inherit;False;Property;_NormalStrength;NormalStrength;10;0;Create;True;0;0;0;False;0;False;0;25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;133;1120,-800;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;113;-320,-128;Inherit;False;37;PerimeterGlow;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;100;-112,-128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;98;48,-80;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-4832,704;Inherit;False;68;CircleMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-4432,704;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-2992,-384;Inherit;True;18;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;1488,-496;Inherit;False;105;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;112;1472,-640;Inherit;False;111;FinalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;50;-4640,704;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;130;1136,-480;Inherit;False;Normal From Height;-1;;2;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;1;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1680,-688;Float;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;Portal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;87;0;6;0
WireConnection;24;0;14;2
WireConnection;24;1;48;0
WireConnection;84;0;24;0
WireConnection;49;0;88;0
WireConnection;7;1;49;0
WireConnection;7;3;8;0
WireConnection;11;0;91;0
WireConnection;9;0;7;0
WireConnection;9;2;85;0
WireConnection;101;0;85;0
WireConnection;10;0;11;0
WireConnection;82;0;9;0
WireConnection;82;2;101;0
WireConnection;12;0;10;0
WireConnection;5;0;82;0
WireConnection;5;1;81;0
WireConnection;35;0;5;0
WireConnection;68;0;12;0
WireConnection;13;0;72;0
WireConnection;13;1;89;0
WireConnection;17;0;72;0
WireConnection;16;0;17;0
WireConnection;16;1;13;0
WireConnection;21;0;16;0
WireConnection;18;0;21;0
WireConnection;96;0;20;0
WireConnection;102;0;96;0
WireConnection;27;0;104;0
WireConnection;61;0;52;0
WireConnection;61;1;69;0
WireConnection;51;0;27;0
WireConnection;30;0;104;0
WireConnection;58;0;61;0
WireConnection;32;0;51;0
WireConnection;32;1;30;0
WireConnection;63;0;53;0
WireConnection;63;1;52;0
WireConnection;64;0;56;0
WireConnection;64;1;58;0
WireConnection;33;0;32;0
WireConnection;46;0;36;0
WireConnection;55;0;53;0
WireConnection;55;1;63;0
WireConnection;55;2;64;0
WireConnection;34;0;33;0
WireConnection;34;1;46;0
WireConnection;70;0;55;0
WireConnection;127;0;34;0
WireConnection;94;0;58;0
WireConnection;47;0;127;0
WireConnection;1;0;2;0
WireConnection;1;1;71;0
WireConnection;37;0;47;0
WireConnection;77;0;80;0
WireConnection;77;1;1;0
WireConnection;77;2;95;0
WireConnection;119;0;118;0
WireConnection;40;0;38;0
WireConnection;40;1;41;0
WireConnection;116;0;77;0
WireConnection;125;0;119;0
WireConnection;125;1;126;0
WireConnection;106;0;40;0
WireConnection;122;0;125;0
WireConnection;122;1;123;0
WireConnection;109;0;108;0
WireConnection;121;0;122;0
WireConnection;121;1;117;0
WireConnection;121;2;124;0
WireConnection;129;0;109;0
WireConnection;129;1;128;0
WireConnection;44;0;121;0
WireConnection;44;1;107;0
WireConnection;44;2;129;0
WireConnection;111;0;44;0
WireConnection;132;0;111;0
WireConnection;134;0;132;0
WireConnection;97;0;102;0
WireConnection;135;0;134;0
WireConnection;105;0;97;0
WireConnection;133;0;135;0
WireConnection;100;0;113;0
WireConnection;98;0;100;0
WireConnection;83;0;50;0
WireConnection;50;0;86;0
WireConnection;130;20;133;0
WireConnection;130;110;131;0
WireConnection;0;0;112;0
WireConnection;0;1;130;40
WireConnection;0;9;115;0
ASEEND*/
//CHKSM=EA09183825F75B38B86A18A27A95877D19C5E540