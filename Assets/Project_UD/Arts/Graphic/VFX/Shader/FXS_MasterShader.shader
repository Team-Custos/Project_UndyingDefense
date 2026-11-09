// Made with Amplify Shader Editor v1.9.9.12
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Amplify_Shader/VFX/Master_Shader_jsd"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)] _CullMode( "CullMode", Float ) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _SRC( "SRC", Float ) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _DST( "DST", Float ) = 0
		_Main_Tex( "Main_Tex", 2D ) = "white" {}
		_Main_Ins( "Main_Ins", Float ) = 1
		[Toggle( _USE_INS_CUSTOM_ON )] _USE_Ins_Custom( "USE_Ins_Custom", Float ) = 0
		_Main_Power( "Main_Power", Float ) = 1
		[HDR] _Main_Color( "Main_Color", Color ) = ( 1, 1, 1, 1 )
		[HDR] _Sub_Color( "Sub_Color", Color ) = ( 1, 1, 1, 1 )
		[Toggle( _USE_SUB_COLOR_ON )] _USE_Sub_Color( "USE_Sub_Color", Float ) = 0
		_Emmision_Ins( "Emmision_Ins", Float ) = 1
		[KeywordEnum( U,V )] _Color_UV_Custom( "Color_UV_Custom", Float ) = 0
		_Emission_Offset( "Emission_Offset", Range( -1, 1 ) ) = -0.5
		[Toggle( _MAIN_PANNER_CUSTOM_ON )] _Main_Panner_Custom( "Main_Panner_Custom", Float ) = 0
		_Main_Upanner( "Main_Upanner", Float ) = 0
		_Main_Vpanner( "Main_Vpanner", Float ) = 0
		_Normal_Tex( "Normal_Tex", 2D ) = "white" {}
		[Toggle( _USE_NORMAL_ON_ON )] _USE_Normal_ON( "USE_Normal_ON", Float ) = 0
		_Normal_Strength( "Normal_Strength", Float ) = 0
		_Normal_Upanner( "Normal_Upanner", Float ) = 0
		_Normal_Vpanner( "Normal_Vpanner", Float ) = 0
		[Toggle( _DISTORTION_CUSTOM_ON )] _Distortion_Custom( "Distortion_Custom", Float ) = 0
		_Normal_Offset( "Normal_Offset", Vector ) = ( 0, 0, 0, 0 )
		_Normal_Speed( "Normal_Speed", Vector ) = ( 0, 0, 0, 0 )
		_Mask_Tex( "Mask_Tex", 2D ) = "white" {}
		_Mask_power( "Mask_power", Float ) = 0
		[Toggle( _MASK_TEXCOORD_CUSTOM_ON_ON )] _Mask_Texcoord_Custom_on( "Mask_Texcoord_Custom_on", Float ) = 0
		_Mask_Upanner( "Mask_Upanner", Float ) = 0
		_Mask_Vpanner( "Mask_Vpanner", Float ) = 0
		_Dissolve_Tex( "Dissolve_Tex", 2D ) = "white" {}
		[Toggle( _USE_DISSOLVE_ON_ON )] _USE_Dissolve_On( "USE_Dissolve_On", Float ) = 0
		[Toggle( _DISSOLVE_PANNER_CUSTOM_ON )] _Dissolve_Panner_Custom( "Dissolve_Panner_Custom", Float ) = 0
		_Dissolve_Upanner( "Dissolve_Upanner", Float ) = 0
		_Dissolve_Vpanner( "Dissolve_Vpanner", Float ) = 0
		[Toggle( _STEP_CUSTOM_ON )] _Step_Custom( "Step_Custom", Float ) = 0
		_Step_Value( "Step_Value", Float ) = 0.1
		_VertexNormal_Offset( "VertexNormal_Offset", Range( -1, 1 ) ) = 0
		_DepthFade( "DepthFade", Float ) = 0


		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		[HideInInspector][ToggleUI] _ReceiveShadows("Receive Shadows", Float) = 1.0

		//[HideInInspector] _AlphaClip("__clip", Float) = 0.0
	}

	SubShader
	{
		PackageRequirements
		{
			"com.unity.render-pipelines.universal": "[14.0,15.0]"
		}

		

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" }

	LOD 0

		ZWrite On
		Cull [_CullMode]
		AlphaToMask Off
		ColorMask RGBA
		Blend One Zero, One Zero
		BlendOp Add, Add

		

		HLSLINCLUDE
		#pragma target 3.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#define ASE_ADJUST_CLIP_POSITION( x ) x

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			Blend [_SRC] [_DST]
			BlendOp Add, Add

			

			HLSLPROGRAM

			

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _NORMAL_DROPOFF_TS 1
			#pragma shader_feature_local_fragment _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_VERSION 19912
			#define ASE_SRP_VERSION 140011
			#define REQUIRE_DEPTH_TEXTURE 1


			

			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3

			

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_UNLIT

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140009
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES1
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_SCREEN_POSITION_NORMALIZED
			#pragma shader_feature_local _DISTORTION_CUSTOM_ON
			#pragma shader_feature_local _USE_SUB_COLOR_ON
			#pragma shader_feature_local _MAIN_PANNER_CUSTOM_ON
			#pragma shader_feature_local _USE_NORMAL_ON_ON
			#pragma shader_feature_local _USE_INS_CUSTOM_ON
			#pragma shader_feature_local _COLOR_UV_CUSTOM_U _COLOR_UV_CUSTOM_V
			#pragma shader_feature_local _USE_DISSOLVE_ON_ON
			#pragma shader_feature_local _MASK_TEXCOORD_CUSTOM_ON_ON
			#pragma shader_feature_local _STEP_CUSTOM_ON
			#pragma shader_feature_local _DISSOLVE_PANNER_CUSTOM_ON


			#if defined(ASE_WRITE_DEPTH_CONSERVATIVE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 positionWSAndFogFactor : TEXCOORD0;
				half3 normalWS : TEXCOORD1;
				half4 tangentWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Dissolve_Tex_ST;
			float4 _Normal_Tex_ST;
			float4 _Mask_Tex_ST;
			float4 _Main_Tex_ST;
			float4 _Sub_Color;
			float4 _Main_Color;
			float2 _Normal_Offset;
			float2 _Normal_Speed;
			float _CullMode;
			float _Dissolve_Vpanner;
			float _Dissolve_Upanner;
			float _Mask_power;
			float _Mask_Vpanner;
			float _Mask_Upanner;
			float _Emission_Offset;
			float _Emmision_Ins;
			float _Main_Power;
			float _Step_Value;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Normal_Strength;
			float _VertexNormal_Offset;
			float _Normal_Vpanner;
			float _Normal_Upanner;
			float _DST;
			float _SRC;
			float _Main_Ins;
			float _DepthFade;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Normal_Tex;
			sampler2D _Sampler60327;
			sampler2D _Main_Tex;
			sampler2D _Mask_Tex;
			sampler2D _Dissolve_Tex;


			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float2 appendResult324 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = input.ase_texcoord.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner323 = ( 1.0 * _Time.y * appendResult324 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch325 = ( ( (tex2Dlod( _Sampler60327, float4( ( appendResult10_g1 + appendResult24_g1 ), 0, 0.0) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch325 = panner323;
				#endif
				float3 tex2DNode321 = UnpackNormalScale( tex2Dlod( _Normal_Tex, float4( staticSwitch325, 0, 0.0) ), 1.0f );
				
				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				output.ase_texcoord4 = input.ase_texcoord1;
				output.ase_color = input.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( tex2DNode321 * ( input.normalOS * _VertexNormal_Offset ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				float fogFactor = 0;
				#if defined(ASE_FOG) && !defined(_FOG_FRAGMENT)
					fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
				#endif

				output.positionCS = ASE_ADJUST_CLIP_POSITION( vertexInput.positionCS );
				output.positionWSAndFogFactor = float4( vertexInput.positionWS, fogFactor );
				output.normalWS = normalInput.normalWS;
				output.tangentWS = half4( normalInput.tangentWS, ( input.tangentOS.w > 0.0 ? 1.0 : -1.0 ) * GetOddNegativeScale() );;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_color = input.ase_color;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag ( PackedVaryings input
						#if defined( ASE_WRITE_DEPTH )
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined( _SURFACE_TYPE_TRANSPARENT )
					const bool isTransparent = true;
				#else
					const bool isTransparent = false;
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					float4 shadowCoord = TransformWorldToShadowCoord( input.positionWSAndFogFactor.xyz );
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				// @diogo: mikktspace compliant
				float renormFactor = 1.0 / max( FLT_MIN, length( input.normalWS ) );

				float3 PositionWS = input.positionWSAndFogFactor.xyz;
				float3 PositionRWS = GetCameraRelativePositionWS( PositionWS );
				half3 ViewDirWS = GetWorldSpaceNormalizeViewDir( PositionWS );
				float4 ShadowCoord = shadowCoord;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );
				float3 TangentWS = input.tangentWS.xyz * renormFactor;
				float3 BitangentWS = cross( input.normalWS, input.tangentWS.xyz ) * input.tangentWS.w * renormFactor;
				float3 NormalWS = input.normalWS * renormFactor;

				float3 temp_cast_0 = (0.0).xxx;
				float2 appendResult324 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = input.ase_texcoord3.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner323 = ( 1.0 * _Time.y * appendResult324 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = input.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch325 = ( ( (tex2D( _Sampler60327, ( appendResult10_g1 + appendResult24_g1 ) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch325 = panner323;
				#endif
				float3 tex2DNode321 = UnpackNormalScale( tex2D( _Normal_Tex, staticSwitch325 ), 1.0f );
				#ifdef _USE_NORMAL_ON_ON
				float3 staticSwitch438 = (tex2DNode321).xyz;
				#else
				float3 staticSwitch438 = temp_cast_0;
				#endif
				float3 temp_output_330_0 = ( staticSwitch438 * _Normal_Strength );
				float2 uv_Main_Tex = input.ase_texcoord3.xy * _Main_Tex_ST.xy + _Main_Tex_ST.zw;
				float2 appendResult317 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 panner316 = ( 1.0 * _Time.y * appendResult317 + uv_Main_Tex);
				#ifdef _MAIN_PANNER_CUSTOM_ON
				float3 staticSwitch309 = ( temp_output_330_0 + float3( panner316 ,  0.0 ) );
				#else
				float3 staticSwitch309 = ( temp_output_330_0 + float3( uv_Main_Tex ,  0.0 ) );
				#endif
				float4 tex2DNode281 = tex2D( _Main_Tex, staticSwitch309.xy );
				#ifdef _USE_INS_CUSTOM_ON
				float staticSwitch454 = input.ase_texcoord4.x;
				#else
				float staticSwitch454 = _Main_Ins;
				#endif
				float4 temp_output_285_0 = ( ( pow( tex2DNode281.r , _Main_Power ) * staticSwitch454 ) * _Main_Color );
				float2 texCoord278 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				#if defined( _COLOR_UV_CUSTOM_U )
				float staticSwitch279 = texCoord278.x;
				#elif defined( _COLOR_UV_CUSTOM_V )
				float staticSwitch279 = texCoord278.y;
				#else
				float staticSwitch279 = texCoord278.x;
				#endif
				#ifdef _USE_SUB_COLOR_ON
				float4 staticSwitch432 = ( ( _Sub_Color * ( _Emmision_Ins * ( saturate( ( ( 1.0 - staticSwitch279 ) + _Emission_Offset ) ) * tex2DNode281.r ) ) ) + temp_output_285_0 );
				#else
				float4 staticSwitch432 = temp_output_285_0;
				#endif
				
				float2 appendResult344 = (float2(_Mask_Upanner , _Mask_Vpanner));
				float2 uv_Mask_Tex = input.ase_texcoord3.xy * _Mask_Tex_ST.xy + _Mask_Tex_ST.zw;
				float2 panner343 = ( 1.0 * _Time.y * appendResult344 + uv_Mask_Tex);
				#ifdef _MASK_TEXCOORD_CUSTOM_ON_ON
				float2 staticSwitch338 = ( panner343 + input.ase_texcoord4.z );
				#else
				float2 staticSwitch338 = panner343;
				#endif
				float temp_output_334_0 = ( tex2DNode281.r * pow( tex2D( _Mask_Tex, staticSwitch338 ).r , _Mask_power ) );
				float2 uv_Dissolve_Tex = input.ase_texcoord3.xy * _Dissolve_Tex_ST.xy + _Dissolve_Tex_ST.zw;
				float2 appendResult355 = (float2(_Dissolve_Upanner , _Dissolve_Vpanner));
				float2 panner354 = ( 1.0 * _Time.y * appendResult355 + uv_Dissolve_Tex);
				#ifdef _DISSOLVE_PANNER_CUSTOM_ON
				float3 staticSwitch356 = float3( panner354 ,  0.0 );
				#else
				float3 staticSwitch356 = ( temp_output_330_0 + float3( uv_Dissolve_Tex ,  0.0 ) );
				#endif
				float temp_output_435_0 = ( temp_output_334_0 * saturate( ( input.ase_texcoord4.y + tex2D( _Dissolve_Tex, staticSwitch356.xy ).r ) ) );
				#ifdef _STEP_CUSTOM_ON
				float staticSwitch386 = ( temp_output_334_0 * step( _Step_Value , temp_output_435_0 ) );
				#else
				float staticSwitch386 = temp_output_435_0;
				#endif
				#ifdef _USE_DISSOLVE_ON_ON
				float staticSwitch433 = staticSwitch386;
				#else
				float staticSwitch433 = temp_output_334_0;
				#endif
				float screenDepth446 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ScreenPosNorm.xy ),_ZBufferParams);
				float distanceDepth446 = abs( ( screenDepth446 - LinearEyeDepth( ScreenPosNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( staticSwitch432 * input.ase_color ).rgb;
				float3 Normal = float3(0, 0, 1);
				float Alpha = ( ( input.ase_color.a * saturate( staticSwitch433 ) ) * saturate( distanceDepth446 ) );
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _Cutoff;
					float AlphaClipThresholdShadow = 0.5;
				#endif


				#if defined( ASE_WRITE_DEPTH )
					input.positionCS.z = input.positionCS.z;
				#endif

				#if defined( _ALPHATEST_ON )
					AlphaDiscard( Alpha, AlphaClipThreshold );
				#endif

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_CHANGES_WORLD_POS)
					ShadowCoord = TransformWorldToShadowCoord( PositionWS );
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = PositionWS;
				inputData.positionCS = input.positionCS;
				inputData.normalizedScreenSpaceUV = ScreenPosNorm.xy;
				inputData.normalWS = NormalWS;
				inputData.viewDirectionWS = ViewDirWS;

				#ifdef ASE_FOG
					inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.positionWSAndFogFactor.w);
				#endif

				#if defined(_DBUFFER)
					ApplyDecalToBaseColor(input.positionCS, Color);
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						Color.rgb = MixFogColor(Color.rgb, half3(0,0,0), inputData.fogCoord);
					#else
						Color.rgb = MixFog(Color.rgb, inputData.fogCoord);
					#endif
				#endif

				#if defined( ASE_WRITE_DEPTH )
					outputDepth = input.positionCS.z;
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				#if defined( ASE_OPAQUE_KEEP_ALPHA )
					return half4( Color, Alpha );
				#else
					return half4( Color, OutputAlpha( Alpha, isTransparent ) );
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_VERSION 19912
			#define ASE_SRP_VERSION 140011
			#define REQUIRE_DEPTH_TEXTURE 1


			

			#pragma vertex vert
			#pragma fragment frag

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES1
			#define ASE_NEEDS_FRAG_SCREEN_POSITION_NORMALIZED
			#pragma shader_feature_local _DISTORTION_CUSTOM_ON
			#pragma shader_feature_local _USE_DISSOLVE_ON_ON
			#pragma shader_feature_local _MAIN_PANNER_CUSTOM_ON
			#pragma shader_feature_local _USE_NORMAL_ON_ON
			#pragma shader_feature_local _MASK_TEXCOORD_CUSTOM_ON_ON
			#pragma shader_feature_local _STEP_CUSTOM_ON
			#pragma shader_feature_local _DISSOLVE_PANNER_CUSTOM_ON


			#if defined(ASE_WRITE_DEPTH_CONSERVATIVE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Dissolve_Tex_ST;
			float4 _Normal_Tex_ST;
			float4 _Mask_Tex_ST;
			float4 _Main_Tex_ST;
			float4 _Sub_Color;
			float4 _Main_Color;
			float2 _Normal_Offset;
			float2 _Normal_Speed;
			float _CullMode;
			float _Dissolve_Vpanner;
			float _Dissolve_Upanner;
			float _Mask_power;
			float _Mask_Vpanner;
			float _Mask_Upanner;
			float _Emission_Offset;
			float _Emmision_Ins;
			float _Main_Power;
			float _Step_Value;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Normal_Strength;
			float _VertexNormal_Offset;
			float _Normal_Vpanner;
			float _Normal_Upanner;
			float _DST;
			float _SRC;
			float _Main_Ins;
			float _DepthFade;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Normal_Tex;
			sampler2D _Sampler60327;
			sampler2D _Main_Tex;
			sampler2D _Mask_Tex;
			sampler2D _Dissolve_Tex;


			float3 _LightDirection;
			float3 _LightPosition;

			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float2 appendResult324 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = input.ase_texcoord.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner323 = ( 1.0 * _Time.y * appendResult324 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch325 = ( ( (tex2Dlod( _Sampler60327, float4( ( appendResult10_g1 + appendResult24_g1 ), 0, 0.0) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch325 = panner323;
				#endif
				float3 tex2DNode321 = UnpackNormalScale( tex2Dlod( _Normal_Tex, float4( staticSwitch325, 0, 0.0) ), 1.0f );
				
				output.ase_color = input.ase_color;
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				output.ase_texcoord1 = input.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( tex2DNode321 * ( input.normalOS * _VertexNormal_Offset ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = ASE_ADJUST_CLIP_POSITION( vertexInput.positionCS );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_color = input.ase_color;
				output.ase_texcoord1 = input.ase_texcoord1;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input
						#if defined( ASE_WRITE_DEPTH )
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );

				float3 temp_cast_0 = (0.0).xxx;
				float2 appendResult324 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = input.ase_texcoord.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner323 = ( 1.0 * _Time.y * appendResult324 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch325 = ( ( (tex2D( _Sampler60327, ( appendResult10_g1 + appendResult24_g1 ) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch325 = panner323;
				#endif
				float3 tex2DNode321 = UnpackNormalScale( tex2D( _Normal_Tex, staticSwitch325 ), 1.0f );
				#ifdef _USE_NORMAL_ON_ON
				float3 staticSwitch438 = (tex2DNode321).xyz;
				#else
				float3 staticSwitch438 = temp_cast_0;
				#endif
				float3 temp_output_330_0 = ( staticSwitch438 * _Normal_Strength );
				float2 uv_Main_Tex = input.ase_texcoord.xy * _Main_Tex_ST.xy + _Main_Tex_ST.zw;
				float2 appendResult317 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 panner316 = ( 1.0 * _Time.y * appendResult317 + uv_Main_Tex);
				#ifdef _MAIN_PANNER_CUSTOM_ON
				float3 staticSwitch309 = ( temp_output_330_0 + float3( panner316 ,  0.0 ) );
				#else
				float3 staticSwitch309 = ( temp_output_330_0 + float3( uv_Main_Tex ,  0.0 ) );
				#endif
				float4 tex2DNode281 = tex2D( _Main_Tex, staticSwitch309.xy );
				float2 appendResult344 = (float2(_Mask_Upanner , _Mask_Vpanner));
				float2 uv_Mask_Tex = input.ase_texcoord.xy * _Mask_Tex_ST.xy + _Mask_Tex_ST.zw;
				float2 panner343 = ( 1.0 * _Time.y * appendResult344 + uv_Mask_Tex);
				#ifdef _MASK_TEXCOORD_CUSTOM_ON_ON
				float2 staticSwitch338 = ( panner343 + input.ase_texcoord1.z );
				#else
				float2 staticSwitch338 = panner343;
				#endif
				float temp_output_334_0 = ( tex2DNode281.r * pow( tex2D( _Mask_Tex, staticSwitch338 ).r , _Mask_power ) );
				float2 uv_Dissolve_Tex = input.ase_texcoord.xy * _Dissolve_Tex_ST.xy + _Dissolve_Tex_ST.zw;
				float2 appendResult355 = (float2(_Dissolve_Upanner , _Dissolve_Vpanner));
				float2 panner354 = ( 1.0 * _Time.y * appendResult355 + uv_Dissolve_Tex);
				#ifdef _DISSOLVE_PANNER_CUSTOM_ON
				float3 staticSwitch356 = float3( panner354 ,  0.0 );
				#else
				float3 staticSwitch356 = ( temp_output_330_0 + float3( uv_Dissolve_Tex ,  0.0 ) );
				#endif
				float temp_output_435_0 = ( temp_output_334_0 * saturate( ( input.ase_texcoord1.y + tex2D( _Dissolve_Tex, staticSwitch356.xy ).r ) ) );
				#ifdef _STEP_CUSTOM_ON
				float staticSwitch386 = ( temp_output_334_0 * step( _Step_Value , temp_output_435_0 ) );
				#else
				float staticSwitch386 = temp_output_435_0;
				#endif
				#ifdef _USE_DISSOLVE_ON_ON
				float staticSwitch433 = staticSwitch386;
				#else
				float staticSwitch433 = temp_output_334_0;
				#endif
				float screenDepth446 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ScreenPosNorm.xy ),_ZBufferParams);
				float distanceDepth446 = abs( ( screenDepth446 - LinearEyeDepth( ScreenPosNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				

				float Alpha = ( ( input.ase_color.a * saturate( staticSwitch433 ) ) * saturate( distanceDepth446 ) );
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _Cutoff;
				#endif

				#if defined( ASE_WRITE_DEPTH )
					input.positionCS.z = input.positionCS.z;
				#endif

				#if defined( _ALPHATEST_ON )
					AlphaDiscard( Alpha, AlphaClipThreshold );
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined( ASE_WRITE_DEPTH )
					outputDepth = input.positionCS.z;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_VERSION 19912
			#define ASE_SRP_VERSION 140011
			#define REQUIRE_DEPTH_TEXTURE 1


			

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140009
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES1
			#pragma shader_feature_local _DISTORTION_CUSTOM_ON
			#pragma shader_feature_local _USE_DISSOLVE_ON_ON
			#pragma shader_feature_local _MAIN_PANNER_CUSTOM_ON
			#pragma shader_feature_local _USE_NORMAL_ON_ON
			#pragma shader_feature_local _MASK_TEXCOORD_CUSTOM_ON_ON
			#pragma shader_feature_local _STEP_CUSTOM_ON
			#pragma shader_feature_local _DISSOLVE_PANNER_CUSTOM_ON


			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Dissolve_Tex_ST;
			float4 _Normal_Tex_ST;
			float4 _Mask_Tex_ST;
			float4 _Main_Tex_ST;
			float4 _Sub_Color;
			float4 _Main_Color;
			float2 _Normal_Offset;
			float2 _Normal_Speed;
			float _CullMode;
			float _Dissolve_Vpanner;
			float _Dissolve_Upanner;
			float _Mask_power;
			float _Mask_Vpanner;
			float _Mask_Upanner;
			float _Emission_Offset;
			float _Emmision_Ins;
			float _Main_Power;
			float _Step_Value;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Normal_Strength;
			float _VertexNormal_Offset;
			float _Normal_Vpanner;
			float _Normal_Upanner;
			float _DST;
			float _SRC;
			float _Main_Ins;
			float _DepthFade;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Normal_Tex;
			sampler2D _Sampler60327;
			sampler2D _Main_Tex;
			sampler2D _Mask_Tex;
			sampler2D _Dissolve_Tex;


			
			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float2 appendResult324 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = input.ase_texcoord.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner323 = ( 1.0 * _Time.y * appendResult324 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch325 = ( ( (tex2Dlod( _Sampler60327, float4( ( appendResult10_g1 + appendResult24_g1 ), 0, 0.0) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch325 = panner323;
				#endif
				float3 tex2DNode321 = UnpackNormalScale( tex2Dlod( _Normal_Tex, float4( staticSwitch325, 0, 0.0) ), 1.0f );
				
				float4 ase_positionCS = TransformObjectToHClip( ( input.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				output.ase_texcoord2 = screenPos;
				
				output.ase_color = input.ase_color;
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				output.ase_texcoord1 = input.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( tex2DNode321 * ( input.normalOS * _VertexNormal_Offset ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = ASE_ADJUST_CLIP_POSITION( vertexInput.positionCS );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_color = input.ase_color;
				output.ase_texcoord1 = input.ase_texcoord1;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float3 temp_cast_0 = (0.0).xxx;
				float2 appendResult324 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = input.ase_texcoord.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner323 = ( 1.0 * _Time.y * appendResult324 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch325 = ( ( (tex2D( _Sampler60327, ( appendResult10_g1 + appendResult24_g1 ) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch325 = panner323;
				#endif
				float3 tex2DNode321 = UnpackNormalScale( tex2D( _Normal_Tex, staticSwitch325 ), 1.0f );
				#ifdef _USE_NORMAL_ON_ON
				float3 staticSwitch438 = (tex2DNode321).xyz;
				#else
				float3 staticSwitch438 = temp_cast_0;
				#endif
				float3 temp_output_330_0 = ( staticSwitch438 * _Normal_Strength );
				float2 uv_Main_Tex = input.ase_texcoord.xy * _Main_Tex_ST.xy + _Main_Tex_ST.zw;
				float2 appendResult317 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 panner316 = ( 1.0 * _Time.y * appendResult317 + uv_Main_Tex);
				#ifdef _MAIN_PANNER_CUSTOM_ON
				float3 staticSwitch309 = ( temp_output_330_0 + float3( panner316 ,  0.0 ) );
				#else
				float3 staticSwitch309 = ( temp_output_330_0 + float3( uv_Main_Tex ,  0.0 ) );
				#endif
				float4 tex2DNode281 = tex2D( _Main_Tex, staticSwitch309.xy );
				float2 appendResult344 = (float2(_Mask_Upanner , _Mask_Vpanner));
				float2 uv_Mask_Tex = input.ase_texcoord.xy * _Mask_Tex_ST.xy + _Mask_Tex_ST.zw;
				float2 panner343 = ( 1.0 * _Time.y * appendResult344 + uv_Mask_Tex);
				#ifdef _MASK_TEXCOORD_CUSTOM_ON_ON
				float2 staticSwitch338 = ( panner343 + input.ase_texcoord1.z );
				#else
				float2 staticSwitch338 = panner343;
				#endif
				float temp_output_334_0 = ( tex2DNode281.r * pow( tex2D( _Mask_Tex, staticSwitch338 ).r , _Mask_power ) );
				float2 uv_Dissolve_Tex = input.ase_texcoord.xy * _Dissolve_Tex_ST.xy + _Dissolve_Tex_ST.zw;
				float2 appendResult355 = (float2(_Dissolve_Upanner , _Dissolve_Vpanner));
				float2 panner354 = ( 1.0 * _Time.y * appendResult355 + uv_Dissolve_Tex);
				#ifdef _DISSOLVE_PANNER_CUSTOM_ON
				float3 staticSwitch356 = float3( panner354 ,  0.0 );
				#else
				float3 staticSwitch356 = ( temp_output_330_0 + float3( uv_Dissolve_Tex ,  0.0 ) );
				#endif
				float temp_output_435_0 = ( temp_output_334_0 * saturate( ( input.ase_texcoord1.y + tex2D( _Dissolve_Tex, staticSwitch356.xy ).r ) ) );
				#ifdef _STEP_CUSTOM_ON
				float staticSwitch386 = ( temp_output_334_0 * step( _Step_Value , temp_output_435_0 ) );
				#else
				float staticSwitch386 = temp_output_435_0;
				#endif
				#ifdef _USE_DISSOLVE_ON_ON
				float staticSwitch433 = staticSwitch386;
				#else
				float staticSwitch433 = temp_output_334_0;
				#endif
				float4 screenPos = input.ase_texcoord2;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float screenDepth446 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_positionSSNorm.xy ),_ZBufferParams);
				float distanceDepth446 = abs( ( screenDepth446 - LinearEyeDepth( ase_positionSSNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				

				surfaceDescription.Alpha = ( ( input.ase_color.a * saturate( staticSwitch433 ) ) * saturate( distanceDepth446 ) );
				#if defined( _ALPHATEST_ON )
					surfaceDescription.AlphaClipThreshold = _Cutoff;
				#endif

				#ifdef _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

			

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_VERSION 19912
			#define ASE_SRP_VERSION 140011
			#define REQUIRE_DEPTH_TEXTURE 1


			

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT

			#define SHADERPASS SHADERPASS_DEPTHONLY

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140009
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES1
			#pragma shader_feature_local _DISTORTION_CUSTOM_ON
			#pragma shader_feature_local _USE_DISSOLVE_ON_ON
			#pragma shader_feature_local _MAIN_PANNER_CUSTOM_ON
			#pragma shader_feature_local _USE_NORMAL_ON_ON
			#pragma shader_feature_local _MASK_TEXCOORD_CUSTOM_ON_ON
			#pragma shader_feature_local _STEP_CUSTOM_ON
			#pragma shader_feature_local _DISSOLVE_PANNER_CUSTOM_ON


			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Dissolve_Tex_ST;
			float4 _Normal_Tex_ST;
			float4 _Mask_Tex_ST;
			float4 _Main_Tex_ST;
			float4 _Sub_Color;
			float4 _Main_Color;
			float2 _Normal_Offset;
			float2 _Normal_Speed;
			float _CullMode;
			float _Dissolve_Vpanner;
			float _Dissolve_Upanner;
			float _Mask_power;
			float _Mask_Vpanner;
			float _Mask_Upanner;
			float _Emission_Offset;
			float _Emmision_Ins;
			float _Main_Power;
			float _Step_Value;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Normal_Strength;
			float _VertexNormal_Offset;
			float _Normal_Vpanner;
			float _Normal_Upanner;
			float _DST;
			float _SRC;
			float _Main_Ins;
			float _DepthFade;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Normal_Tex;
			sampler2D _Sampler60327;
			sampler2D _Main_Tex;
			sampler2D _Mask_Tex;
			sampler2D _Dissolve_Tex;


			
			float4 _SelectionID;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float2 appendResult324 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = input.ase_texcoord.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner323 = ( 1.0 * _Time.y * appendResult324 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch325 = ( ( (tex2Dlod( _Sampler60327, float4( ( appendResult10_g1 + appendResult24_g1 ), 0, 0.0) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch325 = panner323;
				#endif
				float3 tex2DNode321 = UnpackNormalScale( tex2Dlod( _Normal_Tex, float4( staticSwitch325, 0, 0.0) ), 1.0f );
				
				float4 ase_positionCS = TransformObjectToHClip( ( input.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				output.ase_texcoord2 = screenPos;
				
				output.ase_color = input.ase_color;
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				output.ase_texcoord1 = input.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( tex2DNode321 * ( input.normalOS * _VertexNormal_Offset ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = ASE_ADJUST_CLIP_POSITION( vertexInput.positionCS );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_color = input.ase_color;
				output.ase_texcoord1 = input.ase_texcoord1;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float3 temp_cast_0 = (0.0).xxx;
				float2 appendResult324 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = input.ase_texcoord.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner323 = ( 1.0 * _Time.y * appendResult324 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch325 = ( ( (tex2D( _Sampler60327, ( appendResult10_g1 + appendResult24_g1 ) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch325 = panner323;
				#endif
				float3 tex2DNode321 = UnpackNormalScale( tex2D( _Normal_Tex, staticSwitch325 ), 1.0f );
				#ifdef _USE_NORMAL_ON_ON
				float3 staticSwitch438 = (tex2DNode321).xyz;
				#else
				float3 staticSwitch438 = temp_cast_0;
				#endif
				float3 temp_output_330_0 = ( staticSwitch438 * _Normal_Strength );
				float2 uv_Main_Tex = input.ase_texcoord.xy * _Main_Tex_ST.xy + _Main_Tex_ST.zw;
				float2 appendResult317 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 panner316 = ( 1.0 * _Time.y * appendResult317 + uv_Main_Tex);
				#ifdef _MAIN_PANNER_CUSTOM_ON
				float3 staticSwitch309 = ( temp_output_330_0 + float3( panner316 ,  0.0 ) );
				#else
				float3 staticSwitch309 = ( temp_output_330_0 + float3( uv_Main_Tex ,  0.0 ) );
				#endif
				float4 tex2DNode281 = tex2D( _Main_Tex, staticSwitch309.xy );
				float2 appendResult344 = (float2(_Mask_Upanner , _Mask_Vpanner));
				float2 uv_Mask_Tex = input.ase_texcoord.xy * _Mask_Tex_ST.xy + _Mask_Tex_ST.zw;
				float2 panner343 = ( 1.0 * _Time.y * appendResult344 + uv_Mask_Tex);
				#ifdef _MASK_TEXCOORD_CUSTOM_ON_ON
				float2 staticSwitch338 = ( panner343 + input.ase_texcoord1.z );
				#else
				float2 staticSwitch338 = panner343;
				#endif
				float temp_output_334_0 = ( tex2DNode281.r * pow( tex2D( _Mask_Tex, staticSwitch338 ).r , _Mask_power ) );
				float2 uv_Dissolve_Tex = input.ase_texcoord.xy * _Dissolve_Tex_ST.xy + _Dissolve_Tex_ST.zw;
				float2 appendResult355 = (float2(_Dissolve_Upanner , _Dissolve_Vpanner));
				float2 panner354 = ( 1.0 * _Time.y * appendResult355 + uv_Dissolve_Tex);
				#ifdef _DISSOLVE_PANNER_CUSTOM_ON
				float3 staticSwitch356 = float3( panner354 ,  0.0 );
				#else
				float3 staticSwitch356 = ( temp_output_330_0 + float3( uv_Dissolve_Tex ,  0.0 ) );
				#endif
				float temp_output_435_0 = ( temp_output_334_0 * saturate( ( input.ase_texcoord1.y + tex2D( _Dissolve_Tex, staticSwitch356.xy ).r ) ) );
				#ifdef _STEP_CUSTOM_ON
				float staticSwitch386 = ( temp_output_334_0 * step( _Step_Value , temp_output_435_0 ) );
				#else
				float staticSwitch386 = temp_output_435_0;
				#endif
				#ifdef _USE_DISSOLVE_ON_ON
				float staticSwitch433 = staticSwitch386;
				#else
				float staticSwitch433 = temp_output_334_0;
				#endif
				float4 screenPos = input.ase_texcoord2;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float screenDepth446 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_positionSSNorm.xy ),_ZBufferParams);
				float distanceDepth446 = abs( ( screenDepth446 - LinearEyeDepth( ase_positionSSNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				

				surfaceDescription.Alpha = ( ( input.ase_color.a * saturate( staticSwitch433 ) ) * saturate( distanceDepth446 ) );
				#if defined( _ALPHATEST_ON )
					surfaceDescription.AlphaClipThreshold = _Cutoff;
				#endif

				#ifdef _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = unity_SelectionID;

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			

        	#define _SURFACE_TYPE_TRANSPARENT 1
        	#define _NORMAL_DROPOFF_TS 1
        	#define ASE_FOG 1
        	#define ASE_VERSION 19912
        	#define ASE_SRP_VERSION 140011
        	#define REQUIRE_DEPTH_TEXTURE 1


			

        	#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS
			#define VARYINGS_NEED_TANGENT_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140009
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            #if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES1
			#define ASE_NEEDS_FRAG_SCREEN_POSITION_NORMALIZED
			#pragma shader_feature_local _DISTORTION_CUSTOM_ON
			#pragma shader_feature_local _USE_DISSOLVE_ON_ON
			#pragma shader_feature_local _MAIN_PANNER_CUSTOM_ON
			#pragma shader_feature_local _USE_NORMAL_ON_ON
			#pragma shader_feature_local _MASK_TEXCOORD_CUSTOM_ON_ON
			#pragma shader_feature_local _STEP_CUSTOM_ON
			#pragma shader_feature_local _DISSOLVE_PANNER_CUSTOM_ON


			#if defined(ASE_WRITE_DEPTH_CONSERVATIVE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				half3 normalWS : TEXCOORD0;
				float4 tangentWS : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Dissolve_Tex_ST;
			float4 _Normal_Tex_ST;
			float4 _Mask_Tex_ST;
			float4 _Main_Tex_ST;
			float4 _Sub_Color;
			float4 _Main_Color;
			float2 _Normal_Offset;
			float2 _Normal_Speed;
			float _CullMode;
			float _Dissolve_Vpanner;
			float _Dissolve_Upanner;
			float _Mask_power;
			float _Mask_Vpanner;
			float _Mask_Upanner;
			float _Emission_Offset;
			float _Emmision_Ins;
			float _Main_Power;
			float _Step_Value;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Normal_Strength;
			float _VertexNormal_Offset;
			float _Normal_Vpanner;
			float _Normal_Upanner;
			float _DST;
			float _SRC;
			float _Main_Ins;
			float _DepthFade;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Normal_Tex;
			sampler2D _Sampler60327;
			sampler2D _Main_Tex;
			sampler2D _Mask_Tex;
			sampler2D _Dissolve_Tex;


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float2 appendResult324 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = input.ase_texcoord.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner323 = ( 1.0 * _Time.y * appendResult324 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch325 = ( ( (tex2Dlod( _Sampler60327, float4( ( appendResult10_g1 + appendResult24_g1 ), 0, 0.0) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch325 = panner323;
				#endif
				float3 tex2DNode321 = UnpackNormalScale( tex2Dlod( _Normal_Tex, float4( staticSwitch325, 0, 0.0) ), 1.0f );
				
				output.ase_color = input.ase_color;
				output.ase_texcoord2.xy = input.ase_texcoord.xy;
				output.ase_texcoord3 = input.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( tex2DNode321 * ( input.normalOS * _VertexNormal_Offset ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				output.positionCS = ASE_ADJUST_CLIP_POSITION( vertexInput.positionCS );
				output.normalWS = normalInput.normalWS;
				output.tangentWS = float4( normalInput.tangentWS, ( input.tangentOS.w > 0.0 ? 1.0 : -1.0 ) * GetOddNegativeScale() );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_color = input.ase_color;
				output.ase_texcoord1 = input.ase_texcoord1;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			void frag(PackedVaryings input
						, out half4 outNormalWS : SV_Target0
						#if defined( ASE_WRITE_DEPTH )
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				// @diogo: mikktspace compliant
				float renormFactor = 1.0 / max( FLT_MIN, length( input.normalWS ) );

				float3 NormalWS = input.normalWS * renormFactor;
				float3 TangentWS = input.tangentWS.xyz * renormFactor;
				float3 BitangentWS = cross( input.normalWS, input.tangentWS.xyz ) * input.tangentWS.w * renormFactor;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );

				float3 temp_cast_0 = (0.0).xxx;
				float2 appendResult324 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = input.ase_texcoord2.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner323 = ( 1.0 * _Time.y * appendResult324 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = input.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = input.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = input.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch325 = ( ( (tex2D( _Sampler60327, ( appendResult10_g1 + appendResult24_g1 ) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch325 = panner323;
				#endif
				float3 tex2DNode321 = UnpackNormalScale( tex2D( _Normal_Tex, staticSwitch325 ), 1.0f );
				#ifdef _USE_NORMAL_ON_ON
				float3 staticSwitch438 = (tex2DNode321).xyz;
				#else
				float3 staticSwitch438 = temp_cast_0;
				#endif
				float3 temp_output_330_0 = ( staticSwitch438 * _Normal_Strength );
				float2 uv_Main_Tex = input.ase_texcoord2.xy * _Main_Tex_ST.xy + _Main_Tex_ST.zw;
				float2 appendResult317 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 panner316 = ( 1.0 * _Time.y * appendResult317 + uv_Main_Tex);
				#ifdef _MAIN_PANNER_CUSTOM_ON
				float3 staticSwitch309 = ( temp_output_330_0 + float3( panner316 ,  0.0 ) );
				#else
				float3 staticSwitch309 = ( temp_output_330_0 + float3( uv_Main_Tex ,  0.0 ) );
				#endif
				float4 tex2DNode281 = tex2D( _Main_Tex, staticSwitch309.xy );
				float2 appendResult344 = (float2(_Mask_Upanner , _Mask_Vpanner));
				float2 uv_Mask_Tex = input.ase_texcoord2.xy * _Mask_Tex_ST.xy + _Mask_Tex_ST.zw;
				float2 panner343 = ( 1.0 * _Time.y * appendResult344 + uv_Mask_Tex);
				#ifdef _MASK_TEXCOORD_CUSTOM_ON_ON
				float2 staticSwitch338 = ( panner343 + input.ase_texcoord3.z );
				#else
				float2 staticSwitch338 = panner343;
				#endif
				float temp_output_334_0 = ( tex2DNode281.r * pow( tex2D( _Mask_Tex, staticSwitch338 ).r , _Mask_power ) );
				float2 uv_Dissolve_Tex = input.ase_texcoord2.xy * _Dissolve_Tex_ST.xy + _Dissolve_Tex_ST.zw;
				float2 appendResult355 = (float2(_Dissolve_Upanner , _Dissolve_Vpanner));
				float2 panner354 = ( 1.0 * _Time.y * appendResult355 + uv_Dissolve_Tex);
				#ifdef _DISSOLVE_PANNER_CUSTOM_ON
				float3 staticSwitch356 = float3( panner354 ,  0.0 );
				#else
				float3 staticSwitch356 = ( temp_output_330_0 + float3( uv_Dissolve_Tex ,  0.0 ) );
				#endif
				float temp_output_435_0 = ( temp_output_334_0 * saturate( ( input.ase_texcoord3.y + tex2D( _Dissolve_Tex, staticSwitch356.xy ).r ) ) );
				#ifdef _STEP_CUSTOM_ON
				float staticSwitch386 = ( temp_output_334_0 * step( _Step_Value , temp_output_435_0 ) );
				#else
				float staticSwitch386 = temp_output_435_0;
				#endif
				#ifdef _USE_DISSOLVE_ON_ON
				float staticSwitch433 = staticSwitch386;
				#else
				float staticSwitch433 = temp_output_334_0;
				#endif
				float screenDepth446 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ScreenPosNorm.xy ),_ZBufferParams);
				float distanceDepth446 = abs( ( screenDepth446 - LinearEyeDepth( ScreenPosNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				

				float3 Normal = float3(0, 0, 1);
				float Alpha = ( ( input.ase_color.a * saturate( staticSwitch433 ) ) * saturate( distanceDepth446 ) );
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _Cutoff;
				#endif

				#if defined( ASE_WRITE_DEPTH )
					input.positionCS.z = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined( ASE_WRITE_DEPTH )
					outputDepth = input.positionCS.z;
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float2 octNormalWS = PackNormalOctQuadEncode(NormalWS);
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					#if defined(_NORMALMAP)
						#if _NORMAL_DROPOFF_TS
							float3 normalWS = TransformTangentToWorld(Normal, half3x3(TangentWS, BitangentWS, NormalWS));
						#elif _NORMAL_DROPOFF_OS
							float3 normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							float3 normalWS = Normal;
						#endif
					#else
						float3 normalWS = NormalWS;
					#endif
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
				#endif
			}
			ENDHLSL
		}

	
	}
	

	

	CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19912
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":265,"pos":[-2196.476,170.1431],"params":["Inherit","False","1707.205","631.7745","Comment","21","416","387","351","350","349","348","347","333","332","330","329","328","327","326","325","324","323","322","321","438","439","Main_Distortion_Setting","1,0.7888081,0.6,1","0","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":347,"pos":[-546.3677,416.1182],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":332,"pos":[-2116.039,338.1413],"params":["Inherit","False","Property","_Normal_Upanner","Normal_Upanner","19","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":333,"pos":[-2112.097,410.9337],"params":["Inherit","False","Property","_Normal_Vpanner","Normal_Vpanner","20","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":348,"pos":[-544.4017,597.0182],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor","id":324,"pos":[-1903.583,334.613],"params":["Inherit","False","FLOAT2","4","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor","id":328,"pos":[-2145.427,640.9176],"params":["Inherit","False","Property","_Normal_Speed","Normal_Speed","23","0","Create","True","0","0","0","False","0","False","Object","-1","","0,0","0,0","0","3","FLOAT2","0","FLOAT","1","FLOAT","2"]}
{"type":"AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor","id":329,"pos":[-2146.476,512.2675],"params":["Inherit","False","Property","_Normal_Offset","Normal_Offset","22","0","Create","True","0","0","0","False","0","False","Object","-1","","0,0","0,0","0","3","FLOAT2","0","FLOAT","1","FLOAT","2"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":326,"pos":[-2064.512,205.419],"params":["Inherit","False","0","321","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":349,"pos":[-556.1996,638.3105],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.PannerNode, AmplifyShaderEditor","id":323,"pos":[-1766.583,248.6133],"params":["Inherit","False","3","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","1","FLOAT","1","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor","id":327,"pos":[-1946.12,478.9053],"params":["Inherit","False","RadialUVDistortion","-1","","1","051d65e7699b41a4c800363fd0e822b2","0","7","60","SAMPLER2D","_Sampler60327","False","1","FLOAT2","1,1","False","11","FLOAT2","0,0","False","65","FLOAT","1","False","68","FLOAT2","1,1","False","47","FLOAT2","1,1","False","29","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":350,"pos":[-800.0214,636.3443],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":263,"pos":[-746.151,1329.333],"params":["Inherit","False","1668.875","640.5963","Comment","14","369","368","364","363","362","361","358","357","356","355","354","353","352","434","Dissolve_Setting","0.6,1,0.8287886,1","0","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":325,"pos":[-1592.62,354.9053],"params":["Inherit","False","Property","_Distortion_Custom","Distortion_Custom","21","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT2","0,0","False","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT2","0,0","False","6","FLOAT2","0,0","False","7","FLOAT2","0,0","False","8","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":351,"pos":[-809.8525,691.4008],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":266,"pos":[-387.2501,257.3794],"params":["Inherit","False","1229.296","443.3408","Comment","20","319","318","317","316","309","305","304","302","301","300","298","294","290","284","283","282","281","440","431","454","Main_Setting","1,0.9824723,0.6,1","0","0"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":321,"pos":[-1320.219,335.4054],"params":["Inherit","True","Property","_Normal_Tex","Normal_Tex","16","0","Create","True","0","0","0","False","0","False","","-1","None","None","True","0","False","white","Auto","True","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":352,"pos":[-705.9821,1383.548],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.ComponentMaskNode, AmplifyShaderEditor","id":322,"pos":[-1046.219,337.4054],"params":["Inherit","False","True","True","True","True","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":439,"pos":[-985.3373,237.8477],"params":["Inherit","False","Constant","_Float0","Float 0","32","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":318,"pos":[-340.7811,614.7202],"params":["Inherit","False","Property","_Main_Vpanner","Main_Vpanner","15","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":319,"pos":[-350.2511,534.7206],"params":["Inherit","False","Property","_Main_Upanner","Main_Upanner","14","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":362,"pos":[-696.151,1432.705],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":416,"pos":[-968.1745,429.9181],"params":["Inherit","False","Property","_Normal_Strength","Normal_Strength","18","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":438,"pos":[-829.8748,240.7766],"params":["Inherit","False","Property","_USE_Normal_ON","USE_Normal_ON","17","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT3","0,0,0","False","0","FLOAT3","0,0,0","False","2","FLOAT3","0,0,0","False","3","FLOAT3","0,0,0","False","4","FLOAT3","0,0,0","False","5","FLOAT3","0,0,0","False","6","FLOAT3","0,0,0","False","7","FLOAT3","0,0,0","False","8","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor","id":317,"pos":[-168.2511,532.7205],"params":["Inherit","False","FLOAT2","4","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":431,"pos":[-379.3932,413.5769],"params":["Inherit","False","0","281","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":363,"pos":[-250.8341,1435.878],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":368,"pos":[-694.5822,1855.93],"params":["Inherit","False","Property","_Dissolve_Vpanner","Dissolve_Vpanner","33","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":369,"pos":[-695.743,1780.658],"params":["Inherit","False","Property","_Dissolve_Upanner","Dissolve_Upanner","32","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":264,"pos":[-642.639,706.3782],"params":["Inherit","False","1311.392","610.1761","Comment","15","391","390","346","345","344","343","339","338","337","336","335","342","451","453","306","Mask_Setting","0.7785853,1,0.6,1","0","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":330,"pos":[-674.2694,345.018],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT","0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.PannerNode, AmplifyShaderEditor","id":316,"pos":[-8.251099,513.7205],"params":["Inherit","False","3","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","1","FLOAT","1","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor","id":355,"pos":[-461.6611,1785.629],"params":["Inherit","False","FLOAT2","4","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":364,"pos":[-243.8341,1468.878],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":358,"pos":[-636.3551,1538.621],"params":["Inherit","False","0","357","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":440,"pos":[63.66272,402.8477],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT2","0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":290,"pos":[-205.311,304.7203],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT2","0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":345,"pos":[-631.6,850.3782],"params":["Inherit","False","Property","_Mask_Upanner","Mask_Upanner","27","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":346,"pos":[-631.4391,930.6495],"params":["Inherit","False","Property","_Mask_Vpanner","Mask_Vpanner","28","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":353,"pos":[-207.6681,1517.368],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT2","0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.PannerNode, AmplifyShaderEditor","id":354,"pos":[-314.2601,1689.129],"params":["Inherit","False","3","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","1","FLOAT","1","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":387,"pos":[-1053.924,395.4051],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":309,"pos":[-92.11609,299.8586],"params":["Inherit","False","Property","_Main_Panner_Custom","Main_Panner_Custom","13","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT3","0,0,0","False","0","FLOAT3","0,0,0","False","2","FLOAT3","0,0,0","False","3","FLOAT3","0,0,0","False","4","FLOAT3","0,0,0","False","5","FLOAT3","0,0,0","False","6","FLOAT3","0,0,0","False","7","FLOAT3","0,0,0","False","8","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor","id":344,"pos":[-455.5181,859.3484],"params":["Inherit","False","FLOAT2","4","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":342,"pos":[-560,736],"params":["Inherit","False","0","337","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":356,"pos":[-86.35706,1558.665],"params":["Inherit","False","Property","_Dissolve_Panner_Custom","Dissolve_Panner_Custom","31","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT3","0,0,0","False","0","FLOAT3","0,0,0","False","2","FLOAT3","0,0,0","False","3","FLOAT3","0,0,0","False","4","FLOAT3","0,0,0","False","5","FLOAT3","0,0,0","False","6","FLOAT3","0,0,0","False","7","FLOAT3","0,0,0","False","8","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":388,"pos":[-936.0092,1292.934],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.TexCoordVertexDataNode, AmplifyShaderEditor","id":339,"pos":[-639.655,1004.406],"params":["Inherit","False","1","4","0","5","FLOAT4","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":281,"pos":[180.7999,307.7779],"params":["Inherit","True","Property","_Main_Tex","Main_Tex","3","0","Create","True","0","0","0","False","0","False","","-1","None","None","True","0","False","white","Auto","False","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.PannerNode, AmplifyShaderEditor","id":343,"pos":[-288,736],"params":["Inherit","False","3","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","1","FLOAT","1","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":357,"pos":[224.0659,1548.333],"params":["Inherit","True","Property","_Dissolve_Tex","Dissolve_Tex","29","0","Create","True","0","0","0","False","0","False","","-1","None","None","True","0","False","white","Auto","False","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":389,"pos":[-893.724,1299.011],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":455,"pos":[-264.8951,931.316],"params":["Inherit","False","2","2","0","FLOAT2","0,0","False","1","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":262,"pos":[925.0656,1381.564],"params":["Inherit","False","620.5991","357.9984","Comment","4","386","375","436","433","Step_Setting","0.6,0.6777313,1,1","0","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":300,"pos":[439.429,386.6805],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":338,"pos":[-121.004,834.9229],"params":["Inherit","False","Property","_Mask_Texcoord_Custom_on","Mask_Texcoord_Custom_on","26","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT2","0,0","False","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT2","0,0","False","6","FLOAT2","0,0","False","7","FLOAT2","0,0","False","8","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":361,"pos":[552.0659,1521.333],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":390,"pos":[105.0619,1298.867],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":379,"pos":[785.2349,1639.069],"params":["Inherit","False","Property","_Step_Value","Step_Value","35","0","Create","True","0","0","0","False","0","False","Object","-1","","0.1","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":336,"pos":[472.5309,780.6805],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":337,"pos":[211.7789,806.8707],"params":["Inherit","True","Property","_Mask_Tex","Mask_Tex","24","0","Create","True","0","0","0","False","0","False","","-1","None","None","True","0","False","white","Auto","False","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":453,"pos":[432,1008],"params":["Inherit","False","Property","_Mask_power","Mask_power","25","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":434,"pos":[681.274,1524.186],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":391,"pos":[628.3608,1304.832],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor","id":375,"pos":[1007.933,1609.672],"params":["Inherit","False","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":335,"pos":[474.7549,828.1586],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.PowerNode, AmplifyShaderEditor","id":451,"pos":[592,880],"params":["Inherit","False","False","2","0","FLOAT","0","False","1","FLOAT","1","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":435,"pos":[849.5596,1518.088],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":392,"pos":[914.8957,1306.389],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.VertexColorNode, AmplifyShaderEditor","id":372,"pos":[1263.388,743.4688],"params":["Inherit","False","0","5","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":436,"pos":[1119.827,1609.561],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":334,"pos":[752,832],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":394,"pos":[1350.965,1305.696],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":419,"pos":[1480.018,888.8618],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":386,"pos":[1074.143,1507.048],"params":["Inherit","False","Property","_Step_Custom","Step_Custom","34","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT","0","False","0","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT","0","False","7","FLOAT","0","False","8","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":395,"pos":[1558.654,1314.162],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":417,"pos":[1481.591,918.7411],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":433,"pos":[1305.061,1417.246],"params":["Inherit","False","Property","_USE_Dissolve_On","USE_Dissolve_On","30","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT","0","False","0","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT","0","False","7","FLOAT","0","False","8","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":261,"pos":[1558.091,1458.25],"params":["Inherit","False","747.755","354.0595","Comment","9","405","404","403","402","401","400","399","398","397","Vertex_Normal_Setting","0.6828581,0.6,1,1","0","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":396,"pos":[1603.165,1319.996],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":418,"pos":[1481.342,983.1962],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":437,"pos":[1444.125,1185.767],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":397,"pos":[1608.091,1508.25],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":393,"pos":[1567.489,1023.222],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":449,"pos":[1694,1166],"params":["Inherit","False","692","227","Comment","3","446","447","448","","1,1,1,1","0","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":398,"pos":[1656.365,1519.496],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":427,"pos":[1731.548,1012.845],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":399,"pos":[1961.865,1523.396],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":430,"pos":[1735.527,921.0029],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":448,"pos":[1744,1280],"params":["Inherit","False","Property","_DepthFade","DepthFade","37","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":400,"pos":[2078.865,1527.296],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.NormalVertexDataNode, AmplifyShaderEditor","id":404,"pos":[1715.558,1547.773],"params":["Inherit","False","0","5","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":405,"pos":[1673.59,1699.309],"params":["Inherit","False","Property","_VertexNormal_Offset","VertexNormal_Offset","36","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","-1","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":428,"pos":[1741.691,865.2844],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.DepthFade, AmplifyShaderEditor","id":446,"pos":[1936,1232],"params":["Inherit","False","True","False","True","2","1","FLOAT3","0,0,0","False","0","FLOAT","1","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":445,"pos":[2544,800],"params":["Inherit","False","228","323","Comment","3","441","442","444","","1,1,1,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":267,"pos":[-503.8251,-269.3839],"params":["Inherit","False","1400.328","452.6003","Comment","25","293","292","291","286","279","278","277","276","275","274","273","272","271","270","269","268","260","259","258","257","256","255","254","253","251","Color_Setting","1,0.6,0.6,1","0","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":402,"pos":[2085.365,1562.396],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":403,"pos":[1956.647,1584.432],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT","0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":429,"pos":[1801.49,856.4839],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":447,"pos":[2208,1216],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":401,"pos":[2127.846,1575.632],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor","id":268,"pos":[-32.82507,-29.78369],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":269,"pos":[230.1749,-28.78369],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":270,"pos":[381.1749,-30.78369],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":271,"pos":[540.1749,-52.78369],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":272,"pos":[697.1747,-56.78369],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":273,"pos":[183.174,-124.9834],"params":["Inherit","False","Property","_Emmision_Ins","Emmision_Ins","10","0","Create","True","0","0","0","False","0","False","Object","-1","","1","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":276,"pos":[814.5026,-166.653],"params":["Inherit","False","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":277,"pos":[814.5032,-128.2538],"params":["Inherit","False","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.PowerNode, AmplifyShaderEditor","id":282,"pos":[497.9039,309.17],"params":["Inherit","False","False","2","0","FLOAT","0","False","1","FLOAT","1","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":283,"pos":[664.0447,307.3795],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":284,"pos":[810.8071,369.4997],"params":["Inherit","False","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":285,"pos":[851.9543,332.4387],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":286,"pos":[844.8071,-16.3277],"params":["Inherit","False","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":291,"pos":[-195.8251,58.21631],"params":["Inherit","False","Property","_Emission_Offset","Emission_Offset","12","0","Create","True","0","0","0","False","0","False","Object","-1","","-0.5","0","-1","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":292,"pos":[114.1749,-28.78369],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":293,"pos":[349.111,32.55713],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":294,"pos":[440.111,341.5571],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":295,"pos":[441.111,253.5571],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":296,"pos":[346.111,206.5571],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":297,"pos":[350.111,244.5571],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":298,"pos":[468.2699,374.1372],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":301,"pos":[478.6439,390.991],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":302,"pos":[635.6439,445.991],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":304,"pos":[629.6439,391.991],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":305,"pos":[481.6439,412.991],"params":["Inherit","False","Property","_Main_Power","Main_Power","6","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":373,"pos":[1487.922,614.1536],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":424,"pos":[1714.077,815.2779],"params":["Inherit","False","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":425,"pos":[1689.077,808.2779],"params":["Inherit","False","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":426,"pos":[1693.077,669.2779],"params":["Inherit","False","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.WireNode, AmplifyShaderEditor","id":423,"pos":[1687.477,646.0779],"params":["Inherit","False","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":289,"pos":[938.7744,121.2437],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":275,"pos":[373.975,-219.3839],"params":["Inherit","False","Property","_Sub_Color","Sub_Color","8","1","[HDR]","Create","True","0","0","0","False","0","False","Object","-1","","1,1,1,1","0,0,0,0","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":274,"pos":[593.4142,-216.3531],"params":["Inherit","False","Property","_Main_Color","Main_Color","7","1","[HDR]","Create","True","0","0","0","False","0","False","Object","-1","","1,1,1,1","1,1,1,1","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":432,"pos":[1106.389,92.43127],"params":["Inherit","False","Property","_USE_Sub_Color","USE_Sub_Color","9","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","COLOR","0,0,0,0","False","0","COLOR","0,0,0,0","False","2","COLOR","0,0,0,0","False","3","COLOR","0,0,0,0","False","4","COLOR","0,0,0,0","False","5","COLOR","0,0,0,0","False","6","COLOR","0,0,0,0","False","7","COLOR","0,0,0,0","False","8","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":278,"pos":[-507.1012,-54.1023],"params":["Inherit","False","0","-1","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":279,"pos":[-297.821,-34.49109],"params":["Inherit","False","Property","_Color_UV_Custom","Color_UV_Custom","11","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","KeywordEnum","2","U","V","Create","True","True","All","9","1","FLOAT","0","False","0","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT","0","False","7","FLOAT","0","False","8","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":441,"pos":[2592,848],"params":["Inherit","False","Property","_CullMode","CullMode","0","1","[Enum]","Create","True","0","0","1","UnityEngine.Rendering.CullMode","True","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":442,"pos":[2592,928],"params":["Inherit","False","Property","_SRC","SRC","1","1","[Enum]","Create","True","0","0","1","UnityEngine.Rendering.BlendMode","True","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":444,"pos":[2592,1008],"params":["Inherit","False","Property","_DST","DST","2","1","[Enum]","Create","True","0","0","1","UnityEngine.Rendering.BlendMode","True","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":450,"pos":[2144,960],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":454,"pos":[218.2887,591.2423],"params":["Inherit","False","Property","_USE_Ins_Custom","USE_Ins_Custom","5","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT","0","False","0","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT","0","False","7","FLOAT","0","False","8","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":306,"pos":[18.53645,695.8957],"params":["Inherit","False","Property","_Main_Ins","Main_Ins","4","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":251,"pos":[193.9,789.7311],"params":["Float","False","False","-1","2","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","ExtraPrePass","0","0","ExtraPrePass","6","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","3","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","True","3","False","","True","True","0","False","","0","False","","False","True","0","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":253,"pos":[193.9,145.2001],"params":["Float","False","False","-1","2","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","ShadowCaster","0","2","ShadowCaster","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","3","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","False","False","True","False","False","False","False","0","False","","False","False","False","False","False","False","False","False","False","True","1","False","","True","3","False","","False","False","True","1","LightMode=ShadowCaster","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":254,"pos":[193.9,145.2001],"params":["Float","False","False","-1","2","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","DepthOnly","0","3","DepthOnly","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","3","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","False","False","True","False","False","False","False","0","False","","False","False","False","False","False","False","False","False","False","True","1","False","","False","False","False","True","1","LightMode=DepthOnly","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":255,"pos":[193.9,145.2001],"params":["Float","False","False","-1","2","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","Meta","0","4","Meta","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","3","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","2","False","","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","LightMode=Meta","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":256,"pos":[193.9,145.2001],"params":["Float","False","False","-1","2","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","Universal2D","0","5","Universal2D","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","3","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","True","3","False","","True","True","0","False","","0","False","","False","True","1","LightMode=Universal2D","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":257,"pos":[193.9,145.2001],"params":["Float","False","False","-1","2","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","SceneSelectionPass","0","6","SceneSelectionPass","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","3","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","2","False","","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","LightMode=SceneSelectionPass","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":258,"pos":[193.9,145.2001],"params":["Float","False","False","-1","2","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","ScenePickingPass","0","7","ScenePickingPass","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","3","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","LightMode=Picking","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":259,"pos":[193.9,145.2001],"params":["Float","False","False","-1","2","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","DepthNormals","0","8","DepthNormals","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","3","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","False","","True","3","False","","False","False","True","1","LightMode=DepthNormalsOnly","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":260,"pos":[193.9,145.2001],"params":["Float","False","False","-1","2","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","DepthNormalsOnly","0","9","DepthNormalsOnly","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","3","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","False","","True","3","False","","False","False","True","1","LightMode=DepthNormalsOnly","False","True","9","d3d11","metal","vulkan","xboxone","xboxseries","playstation","ps4","ps5","switch","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":252,"pos":[2307.115,789.7311],"params":["Float","False","True","-1","2","UnityEditor.ShaderGraphUnlitGUI","0","19","Amplify_Shader/VFX/Master_Shader_jsd","2992e84f91cbeb14eab234972e07ea9d","True","Forward","0","1","Forward","11","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","True","True","0","True","_CullMode","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Transparent=RenderType","Queue=Transparent=Queue=0","UniversalMaterialType=Unlit","True","3","True","12","all","0","True","True","2","5","True","_SRC","10","True","_DST","0","5","False","","10","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","False","False","False","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","2","False","","True","3","False","","True","True","0","False","","0","False","","False","True","1","LightMode=UniversalForwardOnly","False","False","0","","0","0","Standard","28","Surface","1","638442204140107906","  Keep Alpha","0","0","  Blend","0","0","Two Sided","1","638731601386588927","Alpha Clipping","0","0","  Use Shadow Threshold","0","0","Fragment Normal Space","0","0","Forward Only","0","0","Cast Shadows","0","638450590446273772","Receive Shadows","2","0","Receive SSAO","1","0","GPU Instancing","0","638763166008124916","LOD CrossFade","0","638763166012663712","Built-in Fog","1","639265177140798763","Meta Pass","0","0","Extra Pre Pass","0","0","Tessellation","0","0","  Phong","0","0","  Strength","0.5,False,","0","  Type","0","0","  Tess","16,False,","0","  Min","10,False,","0","  Max","25,False,","0","  Edge Length","16,False,","0","  Max Displacement","25,False,","0","Write Depth","0","0","  Conservative","0","0","Vertex Position","1","0","0","10","False","True","False","True","False","False","True","True","True","False","False","","False","0"]}
{"wire":[347,0,330,0]}
{"wire":[348,0,347,0]}
{"wire":[324,0,332,0]}
{"wire":[324,1,333,0]}
{"wire":[349,0,348,0]}
{"wire":[323,0,326,0]}
{"wire":[323,2,324,0]}
{"wire":[327,68,329,0]}
{"wire":[327,47,328,0]}
{"wire":[350,0,349,0]}
{"wire":[325,1,323,0]}
{"wire":[325,0,327,0]}
{"wire":[351,0,350,0]}
{"wire":[321,1,325,0]}
{"wire":[352,0,351,0]}
{"wire":[322,0,321,0]}
{"wire":[362,0,352,0]}
{"wire":[438,1,439,0]}
{"wire":[438,0,322,0]}
{"wire":[317,0,319,0]}
{"wire":[317,1,318,0]}
{"wire":[363,0,362,0]}
{"wire":[330,0,438,0]}
{"wire":[330,1,416,0]}
{"wire":[316,0,431,0]}
{"wire":[316,2,317,0]}
{"wire":[355,0,369,0]}
{"wire":[355,1,368,0]}
{"wire":[364,0,363,0]}
{"wire":[440,0,330,0]}
{"wire":[440,1,316,0]}
{"wire":[290,0,330,0]}
{"wire":[290,1,431,0]}
{"wire":[353,0,364,0]}
{"wire":[353,1,358,0]}
{"wire":[354,0,358,0]}
{"wire":[354,2,355,0]}
{"wire":[387,0,321,0]}
{"wire":[309,1,290,0]}
{"wire":[309,0,440,0]}
{"wire":[344,0,345,0]}
{"wire":[344,1,346,0]}
{"wire":[356,1,353,0]}
{"wire":[356,0,354,0]}
{"wire":[388,0,387,0]}
{"wire":[281,1,309,0]}
{"wire":[343,0,342,0]}
{"wire":[343,2,344,0]}
{"wire":[357,1,356,0]}
{"wire":[389,0,388,0]}
{"wire":[455,0,343,0]}
{"wire":[455,1,339,3]}
{"wire":[300,0,281,1]}
{"wire":[338,1,343,0]}
{"wire":[338,0,455,0]}
{"wire":[361,0,339,2]}
{"wire":[361,1,357,1]}
{"wire":[390,0,389,0]}
{"wire":[336,0,300,0]}
{"wire":[337,1,338,0]}
{"wire":[434,0,361,0]}
{"wire":[391,0,390,0]}
{"wire":[375,0,379,0]}
{"wire":[375,1,435,0]}
{"wire":[335,0,336,0]}
{"wire":[451,0,337,1]}
{"wire":[451,1,453,0]}
{"wire":[435,0,334,0]}
{"wire":[435,1,434,0]}
{"wire":[392,0,391,0]}
{"wire":[436,0,334,0]}
{"wire":[436,1,375,0]}
{"wire":[334,0,335,0]}
{"wire":[334,1,451,0]}
{"wire":[394,0,392,0]}
{"wire":[419,0,372,4]}
{"wire":[386,1,435,0]}
{"wire":[386,0,436,0]}
{"wire":[395,0,394,0]}
{"wire":[417,0,419,0]}
{"wire":[433,1,334,0]}
{"wire":[433,0,386,0]}
{"wire":[396,0,395,0]}
{"wire":[418,0,417,0]}
{"wire":[437,0,433,0]}
{"wire":[397,0,396,0]}
{"wire":[393,0,418,0]}
{"wire":[393,1,437,0]}
{"wire":[398,0,397,0]}
{"wire":[427,0,393,0]}
{"wire":[399,0,398,0]}
{"wire":[430,0,427,0]}
{"wire":[400,0,399,0]}
{"wire":[428,0,430,0]}
{"wire":[446,0,448,0]}
{"wire":[402,0,400,0]}
{"wire":[403,0,404,0]}
{"wire":[403,1,405,0]}
{"wire":[429,0,428,0]}
{"wire":[447,0,446,0]}
{"wire":[401,0,402,0]}
{"wire":[401,1,403,0]}
{"wire":[268,0,279,0]}
{"wire":[269,0,292,0]}
{"wire":[270,0,269,0]}
{"wire":[270,1,293,0]}
{"wire":[271,0,273,0]}
{"wire":[271,1,270,0]}
{"wire":[272,0,275,0]}
{"wire":[272,1,271,0]}
{"wire":[276,0,274,0]}
{"wire":[277,0,276,0]}
{"wire":[282,0,281,1]}
{"wire":[282,1,298,0]}
{"wire":[283,0,282,0]}
{"wire":[283,1,454,0]}
{"wire":[284,0,277,0]}
{"wire":[285,0,283,0]}
{"wire":[285,1,284,0]}
{"wire":[286,0,272,0]}
{"wire":[292,0,268,0]}
{"wire":[292,1,291,0]}
{"wire":[293,0,296,0]}
{"wire":[294,0,281,1]}
{"wire":[295,0,294,0]}
{"wire":[296,0,297,0]}
{"wire":[297,0,295,0]}
{"wire":[298,0,301,0]}
{"wire":[301,0,304,0]}
{"wire":[302,0,305,0]}
{"wire":[304,0,302,0]}
{"wire":[373,0,432,0]}
{"wire":[373,1,372,0]}
{"wire":[424,0,425,0]}
{"wire":[425,0,426,0]}
{"wire":[426,0,423,0]}
{"wire":[423,0,373,0]}
{"wire":[289,0,286,0]}
{"wire":[289,1,285,0]}
{"wire":[432,1,285,0]}
{"wire":[432,0,289,0]}
{"wire":[279,1,278,1]}
{"wire":[279,0,278,2]}
{"wire":[450,0,429,0]}
{"wire":[450,1,447,0]}
{"wire":[454,1,306,0]}
{"wire":[454,0,339,1]}
{"wire":[252,2,424,0]}
{"wire":[252,3,450,0]}
{"wire":[252,5,401,0]}
ASEEND*/
//CHKSM=FEF97FE6BFD7636D917128868455CABB270D6B27