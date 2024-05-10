// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TestHatch"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_OffsetY("OffsetY", Float) = 0
		_Hatching("Hatching", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.25
		_HachingFactor("HachingFactor", Range( 0 , 1)) = 1
		_HatchingScale("HatchingScale", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow novertexlights nolightmap  nodynlightmap nodirlightmap nofog 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Hatching;
		uniform float _OffsetY;
		uniform float _HatchingScale;
		uniform float _HachingFactor;
		uniform float _Cutoff = 0.25;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode89 = tex2D( _MainTex, uv_MainTex );
			float3 ase_worldPos = i.worldPos;
			float4 appendResult6 = (float4(( ase_worldPos.y + _OffsetY ) , ase_worldPos.x , 0.0 , 0.0));
			float4 tex2DNode9 = tex2D( _Hatching, (appendResult6*_HatchingScale + 0.0).xy );
			float4 lerpResult87 = lerp( tex2DNode89 , tex2DNode9 , ( tex2DNode9.a * _HachingFactor ));
			float4 Hatching14 = lerpResult87;
			o.Emission = Hatching14.rgb;
			o.Alpha = 1;
			float MainTex_A90 = tex2DNode89.a;
			clip( MainTex_A90 - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;1;-2649.481,-32;Inherit;False;1972.549;700.6768;;15;90;14;87;66;92;64;89;9;7;6;5;3;2;93;94;Hatching;0,0,0,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;2;-2544,432;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;3;-2512,576;Inherit;False;Property;_OffsetY;OffsetY;1;0;Create;True;0;0;0;False;0;False;0;5.26;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-2352,528;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;6;-2176,448;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-2176,592;Inherit;False;Property;_HatchingScale;HatchingScale;5;0;Create;True;0;0;0;False;0;False;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;7;-1936,240;Inherit;True;Property;_Hatching;Hatching;2;0;Create;True;0;0;0;False;0;False;f475020260d17584690ee7acae6f5dab;f475020260d17584690ee7acae6f5dab;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ScaleAndOffsetNode;93;-1936,464;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;9;-1680,240;Inherit;True;Property;_TextureSample1;Texture Sample 0;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;89;-1872,32;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;64;-1664,432;Inherit;False;Property;_HachingFactor;HachingFactor;4;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;92;-1328,96;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-1360,336;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;87;-1104,208;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;-912,224;Inherit;False;Hatching;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;-1584,128;Inherit;False;MainTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-368,0;Inherit;False;14;Hatching;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-256,224;Inherit;False;90;MainTex_A;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;TestHatch;False;False;False;False;False;True;True;True;True;True;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.25;True;False;0;True;Transparent;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;3;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;2;2
WireConnection;5;1;3;0
WireConnection;6;0;5;0
WireConnection;6;1;2;1
WireConnection;93;0;6;0
WireConnection;93;1;94;0
WireConnection;9;0;7;0
WireConnection;9;1;93;0
WireConnection;92;0;89;0
WireConnection;66;0;9;4
WireConnection;66;1;64;0
WireConnection;87;0;92;0
WireConnection;87;1;9;0
WireConnection;87;2;66;0
WireConnection;14;0;87;0
WireConnection;90;0;89;4
WireConnection;0;2;83;0
WireConnection;0;10;91;0
ASEEND*/
//CHKSM=B3549449F6DA2D968D81C7E5102D5CBB77069BF1