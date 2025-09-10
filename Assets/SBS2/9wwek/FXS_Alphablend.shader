// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Amplify Shader/SBS2/9week/FX_Alphablend"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Main_Power("Main_Power", Float) = 1
		_Main_Intensity("Main_Intensity", Float) = 1
		[HDR]_Main_Color("Main_Color", Color) = (1,1,1,0)
		_Opacity("Opacity", Float) = 1
		_Depth_Fade("Depth_Fade", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 screenPos;
		};

		uniform float4 _Main_Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Main_Power;
		uniform float _Main_Intensity;
		uniform float _Opacity;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Depth_Fade;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 temp_cast_0 = (_Main_Power).xxxx;
			o.Emission = ( ( _Main_Color * ( pow( tex2DNode1 , temp_cast_0 ) * _Main_Intensity ) ) * i.vertexColor ).rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth16 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth16 = abs( ( screenDepth16 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth_Fade ) );
			o.Alpha = ( i.vertexColor.a * saturate( ( tex2DNode1.r * _Opacity ) ) * saturate( distanceDepth16 ) );
		}

		ENDCG
	}
	// CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
-255;1021;1920;1019;653.8696;511.5533;1;True;True
Node;AmplifyShaderEditor.SamplerNode;1;-403,-129.5;Float;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;False;0;07954efd18d3db14d9d0ffa578f427b3;c7f4f0551b91915478046e420434229f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;3;-101,-21.5;Float;False;Property;_Main_Power;Main_Power;2;0;Create;True;0;0;False;0;1;-0.26;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-26.04452,402.1996;Float;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;57.3999,515.4344;Float;False;Property;_Depth_Fade;Depth_Fade;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;2;55,-122.5;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;5;141,-24.5;Float;False;Property;_Main_Intensity;Main_Intensity;3;0;Create;True;0;0;False;0;1;1.92;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;337,-120.5;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;6;265,-357.5;Float;False;Property;_Main_Color;Main_Color;4;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0.2026522,0.900452,1.301887,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;16;274.3999,392.4345;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;215.4558,235.7996;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;17;533.3999,382.4345;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;8;500,-15.5;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;14;415.5687,231.1448;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;527,-141.5;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;695,-75.5;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;666.5687,212.1448;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;886,-127;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Amplify Shader/SBS2/9week/FX_Alphablend;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;4;0;2;0
WireConnection;4;1;5;0
WireConnection;16;0;18;0
WireConnection;10;0;1;1
WireConnection;10;1;11;0
WireConnection;17;0;16;0
WireConnection;14;0;10;0
WireConnection;7;0;6;0
WireConnection;7;1;4;0
WireConnection;9;0;7;0
WireConnection;9;1;8;0
WireConnection;15;0;8;4
WireConnection;15;1;14;0
WireConnection;15;2;17;0
WireConnection;0;2;9;0
WireConnection;0;9;15;0
ASEEND*/
//CHKSM=F1BA414129D8B47A200EAAF6A2B9EC27C1FF9975