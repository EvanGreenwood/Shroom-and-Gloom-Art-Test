// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hatching"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Hatching("Hatching", 2D) = "white" {}
		_OffsetY("OffsetY", Float) = 0
		[PerRendererData]_MainTex("MainTex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow exclude_path:forward noambient nolightmap  nodynlightmap nodirlightmap nofog 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform sampler2D _Hatching;
		uniform float _OffsetY;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float4 appendResult103 = (float4(( ase_worldPos.y + _OffsetY ) , ase_worldPos.x , 0.0 , 0.0));
			float4 tex2DNode4 = tex2D( _Hatching, appendResult103.xy );
			float Hatching105 = tex2DNode4.r;
			float3 temp_cast_1 = (Hatching105).xxx;
			o.Emission = temp_cast_1;
			o.Alpha = 1;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode117 = tex2D( _MainTex, uv_MainTex );
			float MainTexAlpha80 = tex2DNode117.a;
			clip( saturate( MainTexAlpha80 ) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;108;-1920,-1040;Inherit;False;1412;651;;10;10;103;5;4;9;105;111;112;116;99;;0,0,0,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;10;-2064,-592;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;110;-2166.236,-450.0964;Inherit;False;Property;_OffsetY;OffsetY;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;52;-5696,-912;Inherit;False;868;563;Comment;6;80;62;58;113;114;117;Tex And Vertex Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;116;-1872,-496;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;103;-1696,-576;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;5;-1776,-768;Inherit;True;Property;_Hatching;Hatching;1;0;Create;True;0;0;0;False;0;False;f475020260d17584690ee7acae6f5dab;f475020260d17584690ee7acae6f5dab;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;117;-5648,-816;Inherit;True;Property;_MainTex;MainTex;12;1;[PerRendererData];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1520,-768;Inherit;True;Property;_TextureSample1;Texture Sample 0;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;-5312,-656;Inherit;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;15;-5808,-16;Inherit;False;1919.069;465.2117;Applies fog based on camera depth;15;67;63;61;59;56;53;48;38;35;30;21;19;18;17;16;Apply Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;20;-6672,800;Inherit;False;2228;819; Attempt to add some dynamisim to fog. Not good enough;29;60;55;54;51;50;49;47;46;45;44;43;42;41;40;39;37;36;34;33;32;31;29;28;27;26;25;24;23;22;Noisy Fog (needs work bad atm);1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;64;-3546.318,1200;Inherit;False;822.0012;247.2429;Comment;3;74;68;71;Mip Smoothed Normal Sample;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;105;-752,-784;Inherit;False;Hatching;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;97;-224,320;Inherit;False;80;MainTexAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-5792,304;Inherit;False;Global;FogStart;FogStart;2;0;Create;True;0;0;0;False;0;False;1.5;15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-5792,224;Inherit;False;Global;FogDistance;FogDistance;2;0;Create;True;0;0;0;False;0;False;20;50;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;18;-5536,240;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-5568,368;Inherit;False;Property;_FogIntensityCoefficient;FogIntensityCoefficient;2;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-5280,288;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-6624,1504;Inherit;False;Constant;_Float6;Float 0;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-6624,1424;Inherit;False;Constant;_Float10;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-6592,1152;Inherit;False;Constant;_Float4;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-6608,1328;Inherit;False;Constant;_Float5;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-6624,960;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-6624,848;Inherit;False;Constant;_Float7;Float 0;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-6624,1056;Inherit;False;Constant;_Float8;Float 0;2;0;Create;True;0;0;0;False;0;False;0.125;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-6608,1248;Inherit;False;Constant;_Float9;Float 0;2;0;Create;True;0;0;0;False;0;False;0.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;30;-5120,208;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;31;-6416,1440;Inherit;False;ScreenNoise;-1;;61;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;32;-6384,1088;Inherit;False;ScreenNoise;-1;;64;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;33;-6400,1264;Inherit;False;ScreenNoise;-1;;65;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;34;-6384,928;Inherit;False;ScreenNoise;-1;;66;15abc497825ad4e13a038a4649c4a8a1;0;2;21;FLOAT;0.1;False;20;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;35;-4960,224;Inherit;False;EasingOutCirc;-1;;67;181ee7db325895443a774b654a9b8500;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;36;-5888,912;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-5920,1104;Inherit;False;Constant;_Float16;Float 16;2;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;38;-4800,352;Inherit;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;-5680,1424;Inherit;False;38;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;40;-5760,976;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-5600,1168;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;42;-5504,960;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;43;-5488,1312;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-5376,1136;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;45;-5232,1328;Inherit;False;38;Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-5328,928;Inherit;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-5200,1424;Inherit;False;Constant;_Float13;Float 13;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;48;-4784,224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;-4944,1376;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-5104,1216;Inherit;False;Constant;_Float19;Float 19;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-5184,1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;53;-4640,224;Inherit;False;Square;-1;;69;fea980a1f68019543b2cd91d506986e8;0;1;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;54;-4896,1152;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-4848,1008;Inherit;False;Constant;_Float12;Float 12;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;56;-4496,224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;58;-5312,-560;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;59;-5536,32;Inherit;False;Global;FogColor;FogColor;2;0;Create;True;0;0;0;False;0;False;0.1887822,0.1643645,0.509434,1;0.7735849,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;60;-4688,1024;Inherit;False;Property;_NoisyFog;NoisyFog;3;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-4304,288;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;63;-5200,80;Inherit;False;Square;-1;;70;fea980a1f68019543b2cd91d506986e8;0;1;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-5008,-832;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;67;-4128,80;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;70;-3808,0;Inherit;False;Property;_UseFog;Use Fog;5;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;73;-3472,-48;Inherit;False;Property;_SCENE_VIEW;SCENE_VIEW;10;0;Create;True;0;0;0;False;0;False;0;1;1;False;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-1536,-576;Inherit;False;Constant;_HachingFactor;HachingFactor;15;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-3152,-48;Inherit;False;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;-1232,-656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;65;-1776,112;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-1776,32;Inherit;False;Property;_TunnelDistance;TunnelDistance;9;1;[PerRendererData];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;69;-1600,32;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-3520,1264;Inherit;False;Property;_NormalMip;NormalMip;4;0;Create;True;0;0;0;False;0;False;0;2;0;9;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;71;-3232,1248;Inherit;True;Property;_Normal;Normal;6;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;72;-1488,32;Inherit;False;Noise Sine Wave;-1;;84;a6eff29f739ced848846e3b648af87bd;0;2;1;FLOAT;0;False;2;FLOAT2;-0.1,0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;77;-1280,32;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-1280,-48;Inherit;False;Constant;_Float11;Float 11;17;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;75;-1536,368;Inherit;False;Property;_EmissionColor;EmissionColor;8;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,1,0.7490196,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;76;-1600,176;Inherit;True;Property;_Emission;Emission;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-2944,1264;Inherit;False;NormalSample;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-1280,176;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-240,128;Inherit;False;74;NormalSample;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;83;-1088,-16;Inherit;False;Property;_EmissionWaveBasedOnTunnel;EmissionWaveBasedOnTunnel;10;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;114;-5328,-768;Inherit;False;FLOAT3;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;113;-5184,-752;Inherit;False;MainTexRGB;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;107;-208,48;Inherit;False;105;Hatching;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;100;-32,128;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-16,208;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;104;0,320;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;9;-944,-800;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;-1392,-880;Inherit;False;79;Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;288,64;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Hatching;False;False;False;False;True;False;True;True;True;True;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;TransparentCutout;;Transparent;DeferredOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;116;0;10;2
WireConnection;116;1;110;0
WireConnection;103;0;116;0
WireConnection;103;1;10;1
WireConnection;4;0;5;0
WireConnection;4;1;103;0
WireConnection;80;0;117;4
WireConnection;105;0;4;1
WireConnection;18;0;17;0
WireConnection;18;1;16;0
WireConnection;21;0;18;0
WireConnection;21;1;19;0
WireConnection;30;0;21;0
WireConnection;31;21;23;0
WireConnection;31;20;22;0
WireConnection;32;21;28;0
WireConnection;32;20;24;0
WireConnection;33;21;29;0
WireConnection;33;20;25;0
WireConnection;34;21;27;0
WireConnection;34;20;26;0
WireConnection;35;1;30;0
WireConnection;36;0;34;0
WireConnection;36;1;32;0
WireConnection;36;2;33;0
WireConnection;36;3;31;0
WireConnection;38;0;35;0
WireConnection;40;0;36;0
WireConnection;40;1;37;0
WireConnection;42;0;40;0
WireConnection;42;1;41;0
WireConnection;43;0;39;0
WireConnection;44;0;42;0
WireConnection;44;1;43;0
WireConnection;48;0;35;0
WireConnection;49;0;45;0
WireConnection;49;1;47;0
WireConnection;51;0;46;0
WireConnection;51;1;44;0
WireConnection;53;2;48;0
WireConnection;54;0;51;0
WireConnection;54;1;50;0
WireConnection;54;2;49;0
WireConnection;56;0;53;0
WireConnection;60;1;55;0
WireConnection;60;0;54;0
WireConnection;61;0;56;0
WireConnection;61;1;60;0
WireConnection;63;2;59;0
WireConnection;62;1;58;0
WireConnection;67;0;62;0
WireConnection;67;1;63;0
WireConnection;67;2;61;0
WireConnection;70;1;62;0
WireConnection;70;0;67;0
WireConnection;73;1;70;0
WireConnection;73;0;62;0
WireConnection;79;0;73;0
WireConnection;112;0;4;4
WireConnection;112;1;111;0
WireConnection;69;0;66;0
WireConnection;69;1;65;0
WireConnection;71;2;68;0
WireConnection;72;1;69;0
WireConnection;77;0;72;0
WireConnection;74;0;71;0
WireConnection;82;0;76;0
WireConnection;82;1;75;0
WireConnection;83;1;78;0
WireConnection;83;0;77;0
WireConnection;114;0;117;0
WireConnection;113;0;114;0
WireConnection;100;0;81;0
WireConnection;101;0;83;0
WireConnection;101;1;82;0
WireConnection;104;0;97;0
WireConnection;9;0;99;0
WireConnection;9;2;112;0
WireConnection;0;2;107;0
WireConnection;0;10;104;0
ASEEND*/
//CHKSM=30AF67FEA258A34980D1A7627988F76A5B8F90C7