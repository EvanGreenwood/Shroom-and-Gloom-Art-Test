// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Crystal"
{
	Properties
	{
		_GrabPassDistortionFactor("GrabPassDistortionFactor", Range( 0 , 1)) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		_Ramp("Ramp", 2D) = "white" {}
		_RampBlendFactor("RampBlendFactor", Range( 0 , 1)) = 1
		[Normal]_Normals("Normals", 2D) = "bump" {}
		_NormalScale("NormalScale", Float) = 0.25
		_NoiseScale("NoiseScale", Float) = 1
		_EmissionMul("EmissionMul", Range( 0 , 5)) = 1
		_EmissionPower("EmissionPower", Range( 0.001 , 10)) = 1
		[Toggle(_INVERTDARKSIDE_ON)] _InvertDarkSide("Invert Dark Side", Float) = 1
		_DarkSideWidth("DarkSideWidth", Range( 0 , 1)) = 0.75
		_DarkSideLift("DarkSideLift", Range( 0 , 1)) = 0.8
		_DarkSideShadowLift("DarkSideShadowLift", Range( 0 , 1)) = 0.1
		_TimeScale("TimeScale", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _INVERTDARKSIDE_ON
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
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
			float4 vertexColor : COLOR;
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
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

		uniform float _EmissionMul;
		uniform sampler2D _Ramp;
		uniform float _NoiseScale;
		uniform float _TimeScale;
		uniform sampler2D _Normals;
		uniform float4 _Normals_ST;
		uniform float _NormalScale;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _DarkSideWidth;
		uniform float _DarkSideLift;
		uniform float _RampBlendFactor;
		uniform float _EmissionPower;
		uniform float _DarkSideShadowLift;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _GrabPassDistortionFactor;


		float2 voronoihash65( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi65( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
			 		float2 o = voronoihash65( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = max(abs(r.x), abs(r.y));
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F1;
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float mulTime61 = _Time.y * _TimeScale;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 objToWorld84 = mul( unity_ObjectToWorld, float4( ase_vertex3Pos, 1 ) ).xyz;
			float3 VertexWorldPos125 = objToWorld84;
			float3 break127 = VertexWorldPos125;
			float TimeScaled70 = ( mulTime61 + break127.y + break127.z );
			float time65 = TimeScaled70;
			float2 voronoiSmoothId65 = 0;
			float2 uv_Normals = i.uv_texcoord * _Normals_ST.xy + _Normals_ST.zw;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_tangentToWorldFast = float3x3(ase_worldTangent.x,ase_worldBitangent.x,ase_worldNormal.x,ase_worldTangent.y,ase_worldBitangent.y,ase_worldNormal.y,ase_worldTangent.z,ase_worldBitangent.z,ase_worldNormal.z);
			float3 tangentToWorldDir69 = mul( ase_tangentToWorldFast, UnpackScaleNormal( tex2D( _Normals, uv_Normals ), _NormalScale ) );
			float3 WorldSpaceNormals131 = tangentToWorldDir69;
			float2 appendResult113 = (float2(WorldSpaceNormals131.xy));
			float2 coords65 = ( appendResult113 + ( i.uv_texcoord + VertexWorldPos125.z ) ) * _NoiseScale;
			float2 id65 = 0;
			float2 uv65 = 0;
			float fade65 = 0.5;
			float voroi65 = 0;
			float rest65 = 0;
			for( int it65 = 0; it65 <2; it65++ ){
			voroi65 += fade65 * voronoi65( coords65, time65, id65, uv65, 0,voronoiSmoothId65 );
			rest65 += fade65;
			coords65 *= 2;
			fade65 *= 0.5;
			}//Voronoi65
			voroi65 /= rest65;
			float2 appendResult4 = (float2((0.0 + (voroi65 - 0.25) * (1.0 - 0.0) / (0.6 - 0.25)) , (0.0 + (sin( TimeScaled70 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0))));
			float2 RampUV147 = appendResult4;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float MainTex_R141 = tex2DNode1.r;
			float MainTex_A142 = tex2DNode1.a;
			float4 appendResult177 = (float4(MainTex_R141 , MainTex_R141 , MainTex_R141 , MainTex_A142));
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult100 = dot( ase_worldViewDir , WorldSpaceNormals131 );
			float DarkSideMask138 = step( saturate( dotResult100 ) , _DarkSideWidth );
			float lerpResult121 = lerp( MainTex_R141 , ( 1.0 - MainTex_R141 ) , (0.0 + (DarkSideMask138 - 0.0) * (1.0 - 0.0) / (_DarkSideLift - 0.0)));
			float4 appendResult119 = (float4(lerpResult121 , lerpResult121 , lerpResult121 , MainTex_A142));
			#ifdef _INVERTDARKSIDE_ON
				float4 staticSwitch176 = appendResult119;
			#else
				float4 staticSwitch176 = appendResult177;
			#endif
			float4 MainTex_WithInvert145 = staticSwitch176;
			float4 blendOpSrc6 = tex2D( _Ramp, RampUV147 );
			float4 blendOpDest6 = MainTex_WithInvert145;
			float4 lerpBlendMode6 = lerp(blendOpDest6,( blendOpSrc6 + blendOpDest6 - 1.0 ),_RampBlendFactor);
			float4 temp_output_8_0 = ( i.vertexColor * ( saturate( lerpBlendMode6 )) );
			float Color_A150 = temp_output_8_0.a;
			float4 Color102 = temp_output_8_0;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 screenColor20 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPos + float4( ( WorldSpaceNormals131 * _GrabPassDistortionFactor ) , 0.0 ) ).xy/( ase_grabScreenPos + float4( ( WorldSpaceNormals131 * _GrabPassDistortionFactor ) , 0.0 ) ).w);
			float4 GrabPassResult156 = screenColor20;
			float DarkSideLifted155 = (_DarkSideShadowLift + (( 1.0 - DarkSideMask138 ) - 0.0) * (1.0 - _DarkSideShadowLift) / (1.0 - 0.0));
			c.rgb = ( saturate( ( Color102 * GrabPassResult156 ) ) * DarkSideLifted155 ).rgb;
			c.a = Color_A150;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float mulTime61 = _Time.y * _TimeScale;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 objToWorld84 = mul( unity_ObjectToWorld, float4( ase_vertex3Pos, 1 ) ).xyz;
			float3 VertexWorldPos125 = objToWorld84;
			float3 break127 = VertexWorldPos125;
			float TimeScaled70 = ( mulTime61 + break127.y + break127.z );
			float time65 = TimeScaled70;
			float2 voronoiSmoothId65 = 0;
			float2 uv_Normals = i.uv_texcoord * _Normals_ST.xy + _Normals_ST.zw;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_tangentToWorldFast = float3x3(ase_worldTangent.x,ase_worldBitangent.x,ase_worldNormal.x,ase_worldTangent.y,ase_worldBitangent.y,ase_worldNormal.y,ase_worldTangent.z,ase_worldBitangent.z,ase_worldNormal.z);
			float3 tangentToWorldDir69 = mul( ase_tangentToWorldFast, UnpackScaleNormal( tex2D( _Normals, uv_Normals ), _NormalScale ) );
			float3 WorldSpaceNormals131 = tangentToWorldDir69;
			float2 appendResult113 = (float2(WorldSpaceNormals131.xy));
			float2 coords65 = ( appendResult113 + ( i.uv_texcoord + VertexWorldPos125.z ) ) * _NoiseScale;
			float2 id65 = 0;
			float2 uv65 = 0;
			float fade65 = 0.5;
			float voroi65 = 0;
			float rest65 = 0;
			for( int it65 = 0; it65 <2; it65++ ){
			voroi65 += fade65 * voronoi65( coords65, time65, id65, uv65, 0,voronoiSmoothId65 );
			rest65 += fade65;
			coords65 *= 2;
			fade65 *= 0.5;
			}//Voronoi65
			voroi65 /= rest65;
			float2 appendResult4 = (float2((0.0 + (voroi65 - 0.25) * (1.0 - 0.0) / (0.6 - 0.25)) , (0.0 + (sin( TimeScaled70 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0))));
			float2 RampUV147 = appendResult4;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float MainTex_R141 = tex2DNode1.r;
			float MainTex_A142 = tex2DNode1.a;
			float4 appendResult177 = (float4(MainTex_R141 , MainTex_R141 , MainTex_R141 , MainTex_A142));
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult100 = dot( ase_worldViewDir , WorldSpaceNormals131 );
			float DarkSideMask138 = step( saturate( dotResult100 ) , _DarkSideWidth );
			float lerpResult121 = lerp( MainTex_R141 , ( 1.0 - MainTex_R141 ) , (0.0 + (DarkSideMask138 - 0.0) * (1.0 - 0.0) / (_DarkSideLift - 0.0)));
			float4 appendResult119 = (float4(lerpResult121 , lerpResult121 , lerpResult121 , MainTex_A142));
			#ifdef _INVERTDARKSIDE_ON
				float4 staticSwitch176 = appendResult119;
			#else
				float4 staticSwitch176 = appendResult177;
			#endif
			float4 MainTex_WithInvert145 = staticSwitch176;
			float4 blendOpSrc6 = tex2D( _Ramp, RampUV147 );
			float4 blendOpDest6 = MainTex_WithInvert145;
			float4 lerpBlendMode6 = lerp(blendOpDest6,( blendOpSrc6 + blendOpDest6 - 1.0 ),_RampBlendFactor);
			float4 temp_output_8_0 = ( i.vertexColor * ( saturate( lerpBlendMode6 )) );
			float4 Color102 = temp_output_8_0;
			float4 saferPower77 = abs( Color102 );
			float4 temp_cast_3 = (_EmissionPower).xxxx;
			float DarkSideLifted155 = (_DarkSideShadowLift + (( 1.0 - DarkSideMask138 ) - 0.0) * (1.0 - _DarkSideShadowLift) / (1.0 - 0.0));
			float4 Emission160 = ( _EmissionMul * ( saturate( pow( saferPower77 , temp_cast_3 ) ) * DarkSideLifted155 ) );
			o.Emission = Emission160.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha fullforwardshadows exclude_path:deferred 

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
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				half4 color : COLOR0;
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
				o.screenPos = ComputeScreenPos( o.pos );
				o.color = v.color;
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
				surfIN.screenPos = IN.screenPos;
				surfIN.vertexColor = IN.color;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
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
Node;AmplifyShaderEditor.CommentaryNode;163;-5632,-848;Inherit;False;1060;277;;4;32;30;69;131;World Space Normals;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-5584,-784;Inherit;False;Property;_NormalScale;NormalScale;5;0;Create;True;0;0;0;False;0;False;0.25;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;164;-5920,-288;Inherit;False;708;235;;3;111;84;125;World Space Vertex Pos;0,0,0,1;0;0
Node;AmplifyShaderEditor.SamplerNode;30;-5392,-800;Inherit;True;Property;_Normals;Normals;4;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;1e7331ccd8000854db26e3d4f083bd01;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.25;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;111;-5872,-224;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TransformDirectionNode;69;-5104,-800;Inherit;False;Tangent;World;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;84;-5680,-224;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;168;-3664,-1456;Inherit;False;1668;339;;11;105;133;100;101;135;106;138;107;109;110;155;Dark Side;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;131;-4848,-800;Inherit;False;WorldSpaceNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;166;-5936,112;Inherit;False;788;307;;6;59;126;61;127;114;70;Time Scaled;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;125;-5456,-224;Inherit;False;VertexWorldPos;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;105;-3568,-1408;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;133;-3616,-1232;Inherit;False;131;WorldSpaceNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;165;-4384,-304;Inherit;False;1588;611;;15;128;36;129;132;113;85;43;71;112;65;72;67;64;4;147;Noise (Ramp Sample UV);0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-5888,160;Inherit;False;Property;_TimeScale;TimeScale;13;0;Create;True;0;0;0;False;0;False;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;126;-5888,256;Inherit;False;125;VertexWorldPos;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;100;-3296,-1344;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;128;-4336,-16;Inherit;False;125;VertexWorldPos;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;61;-5744,160;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;127;-5680,256;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.CommentaryNode;169;-6336,-848;Inherit;False;596;277;;3;1;141;142;Main Tex;0,0,0,1;0;0
Node;AmplifyShaderEditor.SaturateNode;101;-3184,-1344;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;135;-3312,-1232;Inherit;False;Property;_DarkSideWidth;DarkSideWidth;10;0;Create;True;0;0;0;False;0;False;0.75;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;36;-4224,-144;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;129;-4128,-16;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;132;-4272,-256;Inherit;False;131;WorldSpaceNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;114;-5520,208;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;106;-3024,-1312;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.75;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-6288,-800;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;113;-4016,-256;Inherit;False;FLOAT2;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;85;-3984,-64;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-5392,208;Inherit;False;TimeScaled;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;167;-4150.789,-1008;Inherit;False;1455.716;404.8834;;11;145;176;177;119;144;121;137;122;124;143;140;Main Tex With Invert;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;-2848,-1312;Inherit;False;DarkSideMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;141;-5984,-800;Inherit;False;MainTex_R;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-3904,176;Inherit;False;Property;_NoiseScale;NoiseScale;6;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;-3904,96;Inherit;False;70;TimeScaled;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;112;-3840,-160;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;-4048,-784;Inherit;False;138;DarkSideMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;143;-3984,-928;Inherit;False;141;MainTex_R;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-4112,-704;Inherit;False;Property;_DarkSideLift;DarkSideLift;11;0;Create;True;0;0;0;False;0;False;0.8;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;65;-3648,-80;Inherit;True;0;3;1;0;2;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.GetLocalVarNode;72;-3520,192;Inherit;False;70;TimeScaled;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;142;-5984,-704;Inherit;False;MainTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;124;-3824,-784;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.8;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;122;-3792,-864;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;67;-3456,-80;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.25;False;2;FLOAT;0.6;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;64;-3344,192;Inherit;False;Sin01;-1;;1;dfa74d9ebd1bfcc429ab1d37e2a53901;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;121;-3616,-864;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;144;-3632,-736;Inherit;False;142;MainTex_A;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;4;-3184,16;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;119;-3360,-800;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;177;-3360,-960;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;171;-2608,144;Inherit;False;1348;531;;10;148;146;10;2;7;6;8;102;152;150;Color;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;147;-3040,16;Inherit;False;RampUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;176;-3200,-880;Inherit;False;Property;_InvertDarkSide;Invert Dark Side;9;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;148;-2560,288;Inherit;False;147;RampUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;145;-2944,-880;Inherit;False;MainTex_WithInvert;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;146;-2336,496;Inherit;False;145;MainTex_WithInvert;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;2;-2384,288;Inherit;True;Property;_Ramp;Ramp;2;0;Create;True;0;0;0;False;0;False;-1;None;e2607a1a235301742aa37ce083893b1c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-2368,576;Inherit;False;Property;_RampBlendFactor;RampBlendFactor;3;0;Create;True;0;0;0;False;0;False;1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;170;-2608,-928;Inherit;False;1060;419;;7;21;117;31;136;20;156;118;Grab Pass Result;0,0,0,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;7;-1984,208;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;6;-2032,384;Inherit;False;LinearBurn;True;3;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;136;-2528,-704;Inherit;False;131;WorldSpaceNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-2560,-624;Inherit;False;Property;_GrabPassDistortionFactor;GrabPassDistortionFactor;0;0;Create;True;0;0;0;False;0;False;0.5;0.586;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1776,304;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;173;-2657,-352;Inherit;False;1157;339;;9;174;160;159;97;96;158;95;77;149;Emission;0,0,0,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;107;-2592,-1312;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;21;-2336,-880;Inherit;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-2272,-704;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-1504,304;Inherit;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;109;-2720,-1216;Inherit;False;Property;_DarkSideShadowLift;DarkSideShadowLift;12;0;Create;True;0;0;0;False;0;False;0.1;0.25;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;110;-2416,-1328;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-2096,-784;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;149;-2528,-208;Inherit;False;102;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;174;-2624,-128;Inherit;False;Property;_EmissionPower;EmissionPower;8;0;Create;True;0;0;0;False;0;False;1;8;0.001;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;155;-2240,-1328;Inherit;False;DarkSideLifted;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;20;-1968,-784;Inherit;False;Global;_GrabScreen0;Grab Screen 0;3;0;Create;True;0;0;0;False;0;False;Object;-1;False;True;False;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;77;-2336,-208;Inherit;False;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;8;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;172;-802,-50;Inherit;False;1076;523;;9;0;98;162;157;23;83;103;161;153;Result;0,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;156;-1792,-784;Inherit;False;GrabPassResult;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;95;-2192,-208;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;158;-2256,-112;Inherit;False;155;DarkSideLifted;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;157;-752,240;Inherit;False;156;GrabPassResult;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;-720,160;Inherit;False;102;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-2032,-208;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-2160,-288;Inherit;False;Property;_EmissionMul;EmissionMul;7;0;Create;True;0;0;0;False;0;False;1;5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;152;-1616,384;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-544,208;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;159;-1872,-288;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;150;-1504,448;Inherit;False;Color_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;-464,320;Inherit;False;155;DarkSideLifted;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;83;-400,208;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;160;-1728,-288;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-208,288;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;153;-240,176;Inherit;False;150;Color_A;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;161;-240,48;Inherit;False;160;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Crystal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.3;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;30;5;32;0
WireConnection;69;0;30;0
WireConnection;84;0;111;0
WireConnection;131;0;69;0
WireConnection;125;0;84;0
WireConnection;100;0;105;0
WireConnection;100;1;133;0
WireConnection;61;0;59;0
WireConnection;127;0;126;0
WireConnection;101;0;100;0
WireConnection;129;0;128;0
WireConnection;114;0;61;0
WireConnection;114;1;127;1
WireConnection;114;2;127;2
WireConnection;106;0;101;0
WireConnection;106;1;135;0
WireConnection;113;0;132;0
WireConnection;85;0;36;0
WireConnection;85;1;129;2
WireConnection;70;0;114;0
WireConnection;138;0;106;0
WireConnection;141;0;1;1
WireConnection;112;0;113;0
WireConnection;112;1;85;0
WireConnection;65;0;112;0
WireConnection;65;1;71;0
WireConnection;65;2;43;0
WireConnection;142;0;1;4
WireConnection;124;0;140;0
WireConnection;124;2;137;0
WireConnection;122;0;143;0
WireConnection;67;0;65;0
WireConnection;64;1;72;0
WireConnection;121;0;143;0
WireConnection;121;1;122;0
WireConnection;121;2;124;0
WireConnection;4;0;67;0
WireConnection;4;1;64;0
WireConnection;119;0;121;0
WireConnection;119;1;121;0
WireConnection;119;2;121;0
WireConnection;119;3;144;0
WireConnection;177;0;143;0
WireConnection;177;1;143;0
WireConnection;177;2;143;0
WireConnection;177;3;144;0
WireConnection;147;0;4;0
WireConnection;176;1;177;0
WireConnection;176;0;119;0
WireConnection;145;0;176;0
WireConnection;2;1;148;0
WireConnection;6;0;2;0
WireConnection;6;1;146;0
WireConnection;6;2;10;0
WireConnection;8;0;7;0
WireConnection;8;1;6;0
WireConnection;107;0;138;0
WireConnection;117;0;136;0
WireConnection;117;1;118;0
WireConnection;102;0;8;0
WireConnection;110;0;107;0
WireConnection;110;3;109;0
WireConnection;31;0;21;0
WireConnection;31;1;117;0
WireConnection;155;0;110;0
WireConnection;20;0;31;0
WireConnection;77;0;149;0
WireConnection;77;1;174;0
WireConnection;156;0;20;0
WireConnection;95;0;77;0
WireConnection;96;0;95;0
WireConnection;96;1;158;0
WireConnection;152;0;8;0
WireConnection;23;0;103;0
WireConnection;23;1;157;0
WireConnection;159;0;97;0
WireConnection;159;1;96;0
WireConnection;150;0;152;3
WireConnection;83;0;23;0
WireConnection;160;0;159;0
WireConnection;98;0;83;0
WireConnection;98;1;162;0
WireConnection;0;2;161;0
WireConnection;0;9;153;0
WireConnection;0;13;98;0
ASEEND*/
//CHKSM=CBF140BBB5F7EB7CF6F54E899A4D9C9166EAF965