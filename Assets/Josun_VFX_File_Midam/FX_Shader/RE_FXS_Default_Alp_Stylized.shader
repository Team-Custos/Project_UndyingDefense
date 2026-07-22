// Made with Amplify Shader Editor v1.9.9.12
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FXS/FX_Default_Alp_Stylized_URP"
{
	Properties
	{
		_Opacity( "Opacity", Float ) = 1
		_Depth( "Depth", Float ) = 0.72
		[Toggle( _PANNERVERTEXTEXCOORD_ON )] _PannerVertexTexcoord( "Panner/VertexTexcoord", Float ) = 0
		[Toggle( _GRADATION_COLOR_ONOFF_ON )] _Gradation_Color_OnOff( "Gradation_Color_On/Off", Float ) = 0
		_Gradation_Color_Offset( "Gradation_Color_Offset", Range( -1, 1 ) ) = 0
		[HDR] _Color_A( "Color_A", Color ) = ( 1, 0, 0, 0 )
		[HDR] _Color_B( "Color_B", Color ) = ( 1, 1, 1, 0 )
		[HDR] _Emi_Color( "Emi_Color", Color ) = ( 1, 1, 1, 0 )
		_Emi_Ins( "Emi_Ins", Float ) = 1
		_MainTex( "MainTex", 2D ) = "white" {}
		_Main_Pow( "Main_Pow", Float ) = 1
		_Main_Upanner( "Main_Upanner", Float ) = 0
		_Main_Vpanner( "Main_Vpanner", Float ) = 0
		[Toggle( _NOISE_MASK_OFFSET_TEXCOORDONOFF_ON )] _Noise_Mask_Offset_TexCoordOnOff( "Noise_Mask_Offset_TexCoord On/Off", Float ) = 0
		[Toggle( _DISSOLVE_TEXCROOD_ONOFF_ON )] _Dissolve_TexCrood_OnOff( "Dissolve_TexCrood_On/Off", Float ) = 0
		_Noise_Step_Offset( "Noise_Step_Offset", Range( 0, 1 ) ) = 0.1
		[Toggle( _NOISE_STEP_MASK_ONOFF_ON )] _Noise_Step_Mask_OnOff( "Noise_Step_Mask_On/Off", Float ) = 0
		_Noise_Mask_Offset( "Noise_Mask_Offset", Range( -1, 1 ) ) = -0.1767709
		_Noise_Mask_Step_Offset( "Noise_Mask_Step_Offset", Range( -0.5, 1 ) ) = 0.04788033
		[Toggle( _NOISE_STEP_LINEONOFF_ON )] _Noise_Step_LineOnOff( "Noise_Step_Line On/Off", Float ) = 0
		_Noise_Step_Line_Up( "Noise_Step_Line_Up", Range( 0, 0.5 ) ) = 0.3511167
		_Noise_Step_Line_Down( "Noise_Step_Line_Down", Range( 0, 0.5 ) ) = 0.08561724
		_NoiseTex( "NoiseTex", 2D ) = "white" {}
		_Dissolve( "Dissolve", Float ) = 0
		_Noise_Distortion( "Noise_Distortion", Float ) = 0
		_Noise_Upanner( "Noise_Upanner", Float ) = 0
		_Noise_Vpanner( "Noise_Vpanner", Float ) = 0
		_MaskTex( "MaskTex", 2D ) = "white" {}
		_Mask_Pow( "Mask_Pow", Float ) = 1
		_Mask_Ins( "Mask_Ins", Float ) = 1
		[Toggle( _DISTORTION_VERTEXTEX_ONOFF_ON )] _Distortion_VertexTex_OnOff( "Distortion_VertexTex_On/Off", Float ) = 0
		_NormalTex( "NormalTex", 2D ) = "bump" {}
		_Distortion( "Distortion", Float ) = 0
		_Normal_Upanner( "Normal_Upanner", Float ) = 0
		_Normal_Vpanner( "Normal_Vpanner", Float ) = 0
		[Toggle( _MAIN_CIRCLE_OFFSET_ONOFF_ON )] _Main_Circle_Offset_OnOFf( "Main_Circle_Offset_On/OFf", Float ) = 0
		[Toggle( _CIRCLE_VERTEX_ONOFF_ON )] _Circle_Vertex_OnOff( "Circle_Vertex_On/Off", Float ) = 0
		_InCircle( "InCircle", Range( 0, 0.5 ) ) = 0.4
		_OutCircle( "OutCircle", Range( 0, 0.5 ) ) = 0.5
		_Circle_Move( "Circle_Move", Range( -0.5, 0.5 ) ) = 0


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
		Cull Off
		AlphaToMask Off
		ColorMask RGBA
		Blend One Zero, One Zero
		BlendOp Add, Add

		

		HLSLINCLUDE
		#pragma target 4.5
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
			Blend SrcAlpha OneMinusSrcAlpha, Zero Zero
			BlendOp Add, Add

			

			HLSLPROGRAM

			

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _NORMAL_DROPOFF_TS 1
			#pragma shader_feature_local_fragment _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
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
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_TEXTURE_COORDINATES2
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES2
			#define ASE_NEEDS_TEXTURE_COORDINATES4
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES4
			#define ASE_NEEDS_FRAG_SCREEN_POSITION_NORMALIZED
			#pragma shader_feature_local _GRADATION_COLOR_ONOFF_ON
			#pragma shader_feature_local _MAIN_CIRCLE_OFFSET_ONOFF_ON
			#pragma shader_feature_local _DISTORTION_VERTEXTEX_ONOFF_ON
			#pragma shader_feature_local _PANNERVERTEXTEXCOORD_ON
			#pragma shader_feature_local _CIRCLE_VERTEX_ONOFF_ON
			#pragma shader_feature_local _NOISE_STEP_LINEONOFF_ON
			#pragma shader_feature_local _NOISE_STEP_MASK_ONOFF_ON
			#pragma shader_feature_local _DISSOLVE_TEXCROOD_ONOFF_ON
			#pragma shader_feature_local _NOISE_MASK_OFFSET_TEXCOORDONOFF_ON


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
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 positionWSAndFogFactor : TEXCOORD0;
				half3 normalWS : TEXCOORD1;
				half4 tangentWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Emi_Color;
			float4 _Color_A;
			float4 _Color_B;
			float4 _MaskTex_ST;
			float4 _NormalTex_ST;
			float4 _NoiseTex_ST;
			float4 _MainTex_ST;
			float _Dissolve;
			float _Noise_Step_Offset;
			float _Noise_Mask_Offset;
			float _Noise_Step_Line_Down;
			float _Noise_Step_Line_Up;
			float _Main_Pow;
			float _Opacity;
			float _Mask_Pow;
			float _Noise_Mask_Step_Offset;
			float _Noise_Vpanner;
			float _Noise_Distortion;
			float _Mask_Ins;
			float _InCircle;
			float _OutCircle;
			float _Circle_Move;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Distortion;
			float _Normal_Vpanner;
			float _Normal_Upanner;
			float _Emi_Ins;
			float _Gradation_Color_Offset;
			float _Noise_Upanner;
			float _Depth;
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

			sampler2D _MainTex;
			sampler2D _NormalTex;
			sampler2D _NoiseTex;
			sampler2D _MaskTex;


			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				output.ase_color = input.ase_color;
				output.ase_texcoord4 = input.ase_texcoord2;
				output.ase_texcoord5 = input.ase_texcoord4;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

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
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord4 : TEXCOORD4;

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
				output.ase_texcoord2 = input.ase_texcoord2;
				output.ase_texcoord4 = input.ase_texcoord4;
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
				output.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				output.ase_texcoord4 = patch[0].ase_texcoord4 * bary.x + patch[1].ase_texcoord4 * bary.y + patch[2].ase_texcoord4 * bary.z;
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

				float2 texCoord121 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float4 lerpResult116 = lerp( _Color_A , _Color_B , ( texCoord121.x + _Gradation_Color_Offset ));
				#ifdef _GRADATION_COLOR_ONOFF_ON
				float4 staticSwitch122 = lerpResult116;
				#else
				float4 staticSwitch122 = _Emi_Color;
				#endif
				
				float2 appendResult25 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_NormalTex = input.ase_texcoord3.xy * _NormalTex_ST.xy + _NormalTex_ST.zw;
				float2 panner24 = ( 1.0 * _Time.y * appendResult25 + uv_NormalTex);
				#ifdef _DISTORTION_VERTEXTEX_ONOFF_ON
				float staticSwitch40 = input.ase_texcoord4.z;
				#else
				float staticSwitch40 = _Distortion;
				#endif
				float2 temp_output_34_0 = ( (UnpackNormalScale( tex2D( _NormalTex, panner24 ), 1.0f )).xy * staticSwitch40 );
				float2 Normal37 = temp_output_34_0;
				float2 appendResult65 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 uv_MainTex = input.ase_texcoord3.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner66 = ( 1.0 * _Time.y * appendResult65 + uv_MainTex);
				float2 Main_Panner79 = panner66;
				float2 appendResult75 = (float2(( uv_MainTex.x + input.ase_texcoord4.x ) , ( uv_MainTex.y + input.ase_texcoord4.y )));
				float2 Main_VertexTexcoord80 = appendResult75;
				#ifdef _PANNERVERTEXTEXCOORD_ON
				float2 staticSwitch70 = Main_VertexTexcoord80;
				#else
				float2 staticSwitch70 = Main_Panner79;
				#endif
				float2 temp_cast_1 = (-0.5).xx;
				float2 texCoord102 = input.ase_texcoord3.xy * float2( 1,1 ) + temp_cast_1;
				#ifdef _CIRCLE_VERTEX_ONOFF_ON
				float staticSwitch103 = input.ase_texcoord5.y;
				#else
				float staticSwitch103 = _Circle_Move;
				#endif
				float temp_output_105_0 = ( length( texCoord102 ) + staticSwitch103 );
				float Circle_Offset111 = ( step( temp_output_105_0 , _OutCircle ) * step( _InCircle , temp_output_105_0 ) );
				#ifdef _MAIN_CIRCLE_OFFSET_ONOFF_ON
				float staticSwitch113 = Circle_Offset111;
				#else
				float staticSwitch113 = tex2D( _MainTex, ( Normal37 + staticSwitch70 ) ).r;
				#endif
				float2 Noise_Distortion126 = temp_output_34_0;
				float2 appendResult45 = (float2(_Noise_Upanner , _Noise_Vpanner));
				float2 uv_NoiseTex = input.ase_texcoord3.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				float2 panner47 = ( 1.0 * _Time.y * appendResult45 + uv_NoiseTex);
				#ifdef _DISSOLVE_TEXCROOD_ONOFF_ON
				float staticSwitch51 = input.ase_texcoord4.w;
				#else
				float staticSwitch51 = _Dissolve;
				#endif
				float temp_output_89_0 = ( tex2D( _NoiseTex, ( ( Noise_Distortion126 * _Noise_Distortion ) + panner47 ) ).r + staticSwitch51 );
				float2 texCoord147 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _NOISE_MASK_OFFSET_TEXCOORDONOFF_ON
				float staticSwitch166 = input.ase_texcoord4.z;
				#else
				float staticSwitch166 = _Noise_Mask_Offset;
				#endif
				float temp_output_150_0 = ( texCoord147.y + staticSwitch166 );
				float temp_output_153_0 = ( temp_output_150_0 * temp_output_89_0 );
				#ifdef _NOISE_STEP_MASK_ONOFF_ON
				float staticSwitch158 = step( temp_output_153_0 , _Noise_Mask_Step_Offset );
				#else
				float staticSwitch158 = step( temp_output_89_0 , _Noise_Step_Offset );
				#endif
				float temp_output_154_0 = ( temp_output_153_0 + temp_output_150_0 );
				#ifdef _NOISE_STEP_LINEONOFF_ON
				float staticSwitch165 = ( step( temp_output_154_0 , _Noise_Step_Line_Up ) * step( _Noise_Step_Line_Down , temp_output_154_0 ) );
				#else
				float staticSwitch165 = staticSwitch158;
				#endif
				float Noise76 = saturate( staticSwitch165 );
				float temp_output_5_0 = pow( ( staticSwitch113 * Noise76 ) , _Main_Pow );
				float2 uv_MaskTex = input.ase_texcoord3.xy * _MaskTex_ST.xy + _MaskTex_ST.zw;
				float4 tex2DNode60 = tex2D( _MaskTex, uv_MaskTex );
				float Mask84 = ( pow( tex2DNode60.r , _Mask_Pow ) * _Mask_Ins );
				float screenDepth129 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ScreenPosNorm.xy ),_ZBufferParams);
				float distanceDepth129 = ( screenDepth129 - LinearEyeDepth( ScreenPosNorm.z,_ZBufferParams ) ) / ( _Depth );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( ( staticSwitch122 * _Emi_Ins ) * input.ase_color ).rgb;
				float3 Normal = float3(0, 0, 1);
				float Alpha = ( input.ase_color.a * ( temp_output_5_0 * _Opacity ) * Mask84 * saturate( distanceDepth129 ) );
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
			#pragma multi_compile_instancing
			#pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
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
			#define ASE_NEEDS_TEXTURE_COORDINATES2
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES2
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_TEXTURE_COORDINATES4
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES4
			#define ASE_NEEDS_FRAG_SCREEN_POSITION_NORMALIZED
			#pragma shader_feature_local _MAIN_CIRCLE_OFFSET_ONOFF_ON
			#pragma shader_feature_local _DISTORTION_VERTEXTEX_ONOFF_ON
			#pragma shader_feature_local _PANNERVERTEXTEXCOORD_ON
			#pragma shader_feature_local _CIRCLE_VERTEX_ONOFF_ON
			#pragma shader_feature_local _NOISE_STEP_LINEONOFF_ON
			#pragma shader_feature_local _NOISE_STEP_MASK_ONOFF_ON
			#pragma shader_feature_local _DISSOLVE_TEXCROOD_ONOFF_ON
			#pragma shader_feature_local _NOISE_MASK_OFFSET_TEXCOORDONOFF_ON


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
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Emi_Color;
			float4 _Color_A;
			float4 _Color_B;
			float4 _MaskTex_ST;
			float4 _NormalTex_ST;
			float4 _NoiseTex_ST;
			float4 _MainTex_ST;
			float _Dissolve;
			float _Noise_Step_Offset;
			float _Noise_Mask_Offset;
			float _Noise_Step_Line_Down;
			float _Noise_Step_Line_Up;
			float _Main_Pow;
			float _Opacity;
			float _Mask_Pow;
			float _Noise_Mask_Step_Offset;
			float _Noise_Vpanner;
			float _Noise_Distortion;
			float _Mask_Ins;
			float _InCircle;
			float _OutCircle;
			float _Circle_Move;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Distortion;
			float _Normal_Vpanner;
			float _Normal_Upanner;
			float _Emi_Ins;
			float _Gradation_Color_Offset;
			float _Noise_Upanner;
			float _Depth;
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

			sampler2D _MainTex;
			sampler2D _NormalTex;
			sampler2D _NoiseTex;
			sampler2D _MaskTex;


			float3 _LightDirection;
			float3 _LightPosition;

			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.ase_color = input.ase_color;
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				output.ase_texcoord1 = input.ase_texcoord2;
				output.ase_texcoord2 = input.ase_texcoord4;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

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
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord4 : TEXCOORD4;

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
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord2 = input.ase_texcoord2;
				output.ase_texcoord4 = input.ase_texcoord4;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				output.ase_texcoord4 = patch[0].ase_texcoord4 * bary.x + patch[1].ase_texcoord4 * bary.y + patch[2].ase_texcoord4 * bary.z;
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

				float2 appendResult25 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_NormalTex = input.ase_texcoord.xy * _NormalTex_ST.xy + _NormalTex_ST.zw;
				float2 panner24 = ( 1.0 * _Time.y * appendResult25 + uv_NormalTex);
				#ifdef _DISTORTION_VERTEXTEX_ONOFF_ON
				float staticSwitch40 = input.ase_texcoord1.z;
				#else
				float staticSwitch40 = _Distortion;
				#endif
				float2 temp_output_34_0 = ( (UnpackNormalScale( tex2D( _NormalTex, panner24 ), 1.0f )).xy * staticSwitch40 );
				float2 Normal37 = temp_output_34_0;
				float2 appendResult65 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 uv_MainTex = input.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner66 = ( 1.0 * _Time.y * appendResult65 + uv_MainTex);
				float2 Main_Panner79 = panner66;
				float2 appendResult75 = (float2(( uv_MainTex.x + input.ase_texcoord1.x ) , ( uv_MainTex.y + input.ase_texcoord1.y )));
				float2 Main_VertexTexcoord80 = appendResult75;
				#ifdef _PANNERVERTEXTEXCOORD_ON
				float2 staticSwitch70 = Main_VertexTexcoord80;
				#else
				float2 staticSwitch70 = Main_Panner79;
				#endif
				float2 temp_cast_0 = (-0.5).xx;
				float2 texCoord102 = input.ase_texcoord.xy * float2( 1,1 ) + temp_cast_0;
				#ifdef _CIRCLE_VERTEX_ONOFF_ON
				float staticSwitch103 = input.ase_texcoord2.y;
				#else
				float staticSwitch103 = _Circle_Move;
				#endif
				float temp_output_105_0 = ( length( texCoord102 ) + staticSwitch103 );
				float Circle_Offset111 = ( step( temp_output_105_0 , _OutCircle ) * step( _InCircle , temp_output_105_0 ) );
				#ifdef _MAIN_CIRCLE_OFFSET_ONOFF_ON
				float staticSwitch113 = Circle_Offset111;
				#else
				float staticSwitch113 = tex2D( _MainTex, ( Normal37 + staticSwitch70 ) ).r;
				#endif
				float2 Noise_Distortion126 = temp_output_34_0;
				float2 appendResult45 = (float2(_Noise_Upanner , _Noise_Vpanner));
				float2 uv_NoiseTex = input.ase_texcoord.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				float2 panner47 = ( 1.0 * _Time.y * appendResult45 + uv_NoiseTex);
				#ifdef _DISSOLVE_TEXCROOD_ONOFF_ON
				float staticSwitch51 = input.ase_texcoord1.w;
				#else
				float staticSwitch51 = _Dissolve;
				#endif
				float temp_output_89_0 = ( tex2D( _NoiseTex, ( ( Noise_Distortion126 * _Noise_Distortion ) + panner47 ) ).r + staticSwitch51 );
				float2 texCoord147 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _NOISE_MASK_OFFSET_TEXCOORDONOFF_ON
				float staticSwitch166 = input.ase_texcoord1.z;
				#else
				float staticSwitch166 = _Noise_Mask_Offset;
				#endif
				float temp_output_150_0 = ( texCoord147.y + staticSwitch166 );
				float temp_output_153_0 = ( temp_output_150_0 * temp_output_89_0 );
				#ifdef _NOISE_STEP_MASK_ONOFF_ON
				float staticSwitch158 = step( temp_output_153_0 , _Noise_Mask_Step_Offset );
				#else
				float staticSwitch158 = step( temp_output_89_0 , _Noise_Step_Offset );
				#endif
				float temp_output_154_0 = ( temp_output_153_0 + temp_output_150_0 );
				#ifdef _NOISE_STEP_LINEONOFF_ON
				float staticSwitch165 = ( step( temp_output_154_0 , _Noise_Step_Line_Up ) * step( _Noise_Step_Line_Down , temp_output_154_0 ) );
				#else
				float staticSwitch165 = staticSwitch158;
				#endif
				float Noise76 = saturate( staticSwitch165 );
				float temp_output_5_0 = pow( ( staticSwitch113 * Noise76 ) , _Main_Pow );
				float2 uv_MaskTex = input.ase_texcoord.xy * _MaskTex_ST.xy + _MaskTex_ST.zw;
				float4 tex2DNode60 = tex2D( _MaskTex, uv_MaskTex );
				float Mask84 = ( pow( tex2DNode60.r , _Mask_Pow ) * _Mask_Ins );
				float screenDepth129 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ScreenPosNorm.xy ),_ZBufferParams);
				float distanceDepth129 = ( screenDepth129 - LinearEyeDepth( ScreenPosNorm.z,_ZBufferParams ) ) / ( _Depth );
				

				float Alpha = ( input.ase_color.a * ( temp_output_5_0 * _Opacity ) * Mask84 * saturate( distanceDepth129 ) );
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
			#define ASE_NEEDS_TEXTURE_COORDINATES2
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES2
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_TEXTURE_COORDINATES4
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES4
			#pragma shader_feature_local _MAIN_CIRCLE_OFFSET_ONOFF_ON
			#pragma shader_feature_local _DISTORTION_VERTEXTEX_ONOFF_ON
			#pragma shader_feature_local _PANNERVERTEXTEXCOORD_ON
			#pragma shader_feature_local _CIRCLE_VERTEX_ONOFF_ON
			#pragma shader_feature_local _NOISE_STEP_LINEONOFF_ON
			#pragma shader_feature_local _NOISE_STEP_MASK_ONOFF_ON
			#pragma shader_feature_local _DISSOLVE_TEXCROOD_ONOFF_ON
			#pragma shader_feature_local _NOISE_MASK_OFFSET_TEXCOORDONOFF_ON


			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Emi_Color;
			float4 _Color_A;
			float4 _Color_B;
			float4 _MaskTex_ST;
			float4 _NormalTex_ST;
			float4 _NoiseTex_ST;
			float4 _MainTex_ST;
			float _Dissolve;
			float _Noise_Step_Offset;
			float _Noise_Mask_Offset;
			float _Noise_Step_Line_Down;
			float _Noise_Step_Line_Up;
			float _Main_Pow;
			float _Opacity;
			float _Mask_Pow;
			float _Noise_Mask_Step_Offset;
			float _Noise_Vpanner;
			float _Noise_Distortion;
			float _Mask_Ins;
			float _InCircle;
			float _OutCircle;
			float _Circle_Move;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Distortion;
			float _Normal_Vpanner;
			float _Normal_Upanner;
			float _Emi_Ins;
			float _Gradation_Color_Offset;
			float _Noise_Upanner;
			float _Depth;
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

			sampler2D _MainTex;
			sampler2D _NormalTex;
			sampler2D _NoiseTex;
			sampler2D _MaskTex;


			
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

				float4 ase_positionCS = TransformObjectToHClip( ( input.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				output.ase_texcoord3 = screenPos;
				
				output.ase_color = input.ase_color;
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				output.ase_texcoord1 = input.ase_texcoord2;
				output.ase_texcoord2 = input.ase_texcoord4;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

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
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord4 : TEXCOORD4;

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
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord2 = input.ase_texcoord2;
				output.ase_texcoord4 = input.ase_texcoord4;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				output.ase_texcoord4 = patch[0].ase_texcoord4 * bary.x + patch[1].ase_texcoord4 * bary.y + patch[2].ase_texcoord4 * bary.z;
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

				float2 appendResult25 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_NormalTex = input.ase_texcoord.xy * _NormalTex_ST.xy + _NormalTex_ST.zw;
				float2 panner24 = ( 1.0 * _Time.y * appendResult25 + uv_NormalTex);
				#ifdef _DISTORTION_VERTEXTEX_ONOFF_ON
				float staticSwitch40 = input.ase_texcoord1.z;
				#else
				float staticSwitch40 = _Distortion;
				#endif
				float2 temp_output_34_0 = ( (UnpackNormalScale( tex2D( _NormalTex, panner24 ), 1.0f )).xy * staticSwitch40 );
				float2 Normal37 = temp_output_34_0;
				float2 appendResult65 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 uv_MainTex = input.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner66 = ( 1.0 * _Time.y * appendResult65 + uv_MainTex);
				float2 Main_Panner79 = panner66;
				float2 appendResult75 = (float2(( uv_MainTex.x + input.ase_texcoord1.x ) , ( uv_MainTex.y + input.ase_texcoord1.y )));
				float2 Main_VertexTexcoord80 = appendResult75;
				#ifdef _PANNERVERTEXTEXCOORD_ON
				float2 staticSwitch70 = Main_VertexTexcoord80;
				#else
				float2 staticSwitch70 = Main_Panner79;
				#endif
				float2 temp_cast_0 = (-0.5).xx;
				float2 texCoord102 = input.ase_texcoord.xy * float2( 1,1 ) + temp_cast_0;
				#ifdef _CIRCLE_VERTEX_ONOFF_ON
				float staticSwitch103 = input.ase_texcoord2.y;
				#else
				float staticSwitch103 = _Circle_Move;
				#endif
				float temp_output_105_0 = ( length( texCoord102 ) + staticSwitch103 );
				float Circle_Offset111 = ( step( temp_output_105_0 , _OutCircle ) * step( _InCircle , temp_output_105_0 ) );
				#ifdef _MAIN_CIRCLE_OFFSET_ONOFF_ON
				float staticSwitch113 = Circle_Offset111;
				#else
				float staticSwitch113 = tex2D( _MainTex, ( Normal37 + staticSwitch70 ) ).r;
				#endif
				float2 Noise_Distortion126 = temp_output_34_0;
				float2 appendResult45 = (float2(_Noise_Upanner , _Noise_Vpanner));
				float2 uv_NoiseTex = input.ase_texcoord.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				float2 panner47 = ( 1.0 * _Time.y * appendResult45 + uv_NoiseTex);
				#ifdef _DISSOLVE_TEXCROOD_ONOFF_ON
				float staticSwitch51 = input.ase_texcoord1.w;
				#else
				float staticSwitch51 = _Dissolve;
				#endif
				float temp_output_89_0 = ( tex2D( _NoiseTex, ( ( Noise_Distortion126 * _Noise_Distortion ) + panner47 ) ).r + staticSwitch51 );
				float2 texCoord147 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _NOISE_MASK_OFFSET_TEXCOORDONOFF_ON
				float staticSwitch166 = input.ase_texcoord1.z;
				#else
				float staticSwitch166 = _Noise_Mask_Offset;
				#endif
				float temp_output_150_0 = ( texCoord147.y + staticSwitch166 );
				float temp_output_153_0 = ( temp_output_150_0 * temp_output_89_0 );
				#ifdef _NOISE_STEP_MASK_ONOFF_ON
				float staticSwitch158 = step( temp_output_153_0 , _Noise_Mask_Step_Offset );
				#else
				float staticSwitch158 = step( temp_output_89_0 , _Noise_Step_Offset );
				#endif
				float temp_output_154_0 = ( temp_output_153_0 + temp_output_150_0 );
				#ifdef _NOISE_STEP_LINEONOFF_ON
				float staticSwitch165 = ( step( temp_output_154_0 , _Noise_Step_Line_Up ) * step( _Noise_Step_Line_Down , temp_output_154_0 ) );
				#else
				float staticSwitch165 = staticSwitch158;
				#endif
				float Noise76 = saturate( staticSwitch165 );
				float temp_output_5_0 = pow( ( staticSwitch113 * Noise76 ) , _Main_Pow );
				float2 uv_MaskTex = input.ase_texcoord.xy * _MaskTex_ST.xy + _MaskTex_ST.zw;
				float4 tex2DNode60 = tex2D( _MaskTex, uv_MaskTex );
				float Mask84 = ( pow( tex2DNode60.r , _Mask_Pow ) * _Mask_Ins );
				float4 screenPos = input.ase_texcoord3;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float screenDepth129 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_positionSSNorm.xy ),_ZBufferParams);
				float distanceDepth129 = ( screenDepth129 - LinearEyeDepth( ase_positionSSNorm.z,_ZBufferParams ) ) / ( _Depth );
				

				surfaceDescription.Alpha = ( input.ase_color.a * ( temp_output_5_0 * _Opacity ) * Mask84 * saturate( distanceDepth129 ) );
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
			#define ASE_NEEDS_TEXTURE_COORDINATES2
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES2
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_TEXTURE_COORDINATES4
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES4
			#pragma shader_feature_local _MAIN_CIRCLE_OFFSET_ONOFF_ON
			#pragma shader_feature_local _DISTORTION_VERTEXTEX_ONOFF_ON
			#pragma shader_feature_local _PANNERVERTEXTEXCOORD_ON
			#pragma shader_feature_local _CIRCLE_VERTEX_ONOFF_ON
			#pragma shader_feature_local _NOISE_STEP_LINEONOFF_ON
			#pragma shader_feature_local _NOISE_STEP_MASK_ONOFF_ON
			#pragma shader_feature_local _DISSOLVE_TEXCROOD_ONOFF_ON
			#pragma shader_feature_local _NOISE_MASK_OFFSET_TEXCOORDONOFF_ON


			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Emi_Color;
			float4 _Color_A;
			float4 _Color_B;
			float4 _MaskTex_ST;
			float4 _NormalTex_ST;
			float4 _NoiseTex_ST;
			float4 _MainTex_ST;
			float _Dissolve;
			float _Noise_Step_Offset;
			float _Noise_Mask_Offset;
			float _Noise_Step_Line_Down;
			float _Noise_Step_Line_Up;
			float _Main_Pow;
			float _Opacity;
			float _Mask_Pow;
			float _Noise_Mask_Step_Offset;
			float _Noise_Vpanner;
			float _Noise_Distortion;
			float _Mask_Ins;
			float _InCircle;
			float _OutCircle;
			float _Circle_Move;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Distortion;
			float _Normal_Vpanner;
			float _Normal_Upanner;
			float _Emi_Ins;
			float _Gradation_Color_Offset;
			float _Noise_Upanner;
			float _Depth;
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

			sampler2D _MainTex;
			sampler2D _NormalTex;
			sampler2D _NoiseTex;
			sampler2D _MaskTex;


			
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

				float4 ase_positionCS = TransformObjectToHClip( ( input.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				output.ase_texcoord3 = screenPos;
				
				output.ase_color = input.ase_color;
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				output.ase_texcoord1 = input.ase_texcoord2;
				output.ase_texcoord2 = input.ase_texcoord4;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

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
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord4 : TEXCOORD4;

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
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord2 = input.ase_texcoord2;
				output.ase_texcoord4 = input.ase_texcoord4;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				output.ase_texcoord4 = patch[0].ase_texcoord4 * bary.x + patch[1].ase_texcoord4 * bary.y + patch[2].ase_texcoord4 * bary.z;
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

				float2 appendResult25 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_NormalTex = input.ase_texcoord.xy * _NormalTex_ST.xy + _NormalTex_ST.zw;
				float2 panner24 = ( 1.0 * _Time.y * appendResult25 + uv_NormalTex);
				#ifdef _DISTORTION_VERTEXTEX_ONOFF_ON
				float staticSwitch40 = input.ase_texcoord1.z;
				#else
				float staticSwitch40 = _Distortion;
				#endif
				float2 temp_output_34_0 = ( (UnpackNormalScale( tex2D( _NormalTex, panner24 ), 1.0f )).xy * staticSwitch40 );
				float2 Normal37 = temp_output_34_0;
				float2 appendResult65 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 uv_MainTex = input.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner66 = ( 1.0 * _Time.y * appendResult65 + uv_MainTex);
				float2 Main_Panner79 = panner66;
				float2 appendResult75 = (float2(( uv_MainTex.x + input.ase_texcoord1.x ) , ( uv_MainTex.y + input.ase_texcoord1.y )));
				float2 Main_VertexTexcoord80 = appendResult75;
				#ifdef _PANNERVERTEXTEXCOORD_ON
				float2 staticSwitch70 = Main_VertexTexcoord80;
				#else
				float2 staticSwitch70 = Main_Panner79;
				#endif
				float2 temp_cast_0 = (-0.5).xx;
				float2 texCoord102 = input.ase_texcoord.xy * float2( 1,1 ) + temp_cast_0;
				#ifdef _CIRCLE_VERTEX_ONOFF_ON
				float staticSwitch103 = input.ase_texcoord2.y;
				#else
				float staticSwitch103 = _Circle_Move;
				#endif
				float temp_output_105_0 = ( length( texCoord102 ) + staticSwitch103 );
				float Circle_Offset111 = ( step( temp_output_105_0 , _OutCircle ) * step( _InCircle , temp_output_105_0 ) );
				#ifdef _MAIN_CIRCLE_OFFSET_ONOFF_ON
				float staticSwitch113 = Circle_Offset111;
				#else
				float staticSwitch113 = tex2D( _MainTex, ( Normal37 + staticSwitch70 ) ).r;
				#endif
				float2 Noise_Distortion126 = temp_output_34_0;
				float2 appendResult45 = (float2(_Noise_Upanner , _Noise_Vpanner));
				float2 uv_NoiseTex = input.ase_texcoord.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				float2 panner47 = ( 1.0 * _Time.y * appendResult45 + uv_NoiseTex);
				#ifdef _DISSOLVE_TEXCROOD_ONOFF_ON
				float staticSwitch51 = input.ase_texcoord1.w;
				#else
				float staticSwitch51 = _Dissolve;
				#endif
				float temp_output_89_0 = ( tex2D( _NoiseTex, ( ( Noise_Distortion126 * _Noise_Distortion ) + panner47 ) ).r + staticSwitch51 );
				float2 texCoord147 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _NOISE_MASK_OFFSET_TEXCOORDONOFF_ON
				float staticSwitch166 = input.ase_texcoord1.z;
				#else
				float staticSwitch166 = _Noise_Mask_Offset;
				#endif
				float temp_output_150_0 = ( texCoord147.y + staticSwitch166 );
				float temp_output_153_0 = ( temp_output_150_0 * temp_output_89_0 );
				#ifdef _NOISE_STEP_MASK_ONOFF_ON
				float staticSwitch158 = step( temp_output_153_0 , _Noise_Mask_Step_Offset );
				#else
				float staticSwitch158 = step( temp_output_89_0 , _Noise_Step_Offset );
				#endif
				float temp_output_154_0 = ( temp_output_153_0 + temp_output_150_0 );
				#ifdef _NOISE_STEP_LINEONOFF_ON
				float staticSwitch165 = ( step( temp_output_154_0 , _Noise_Step_Line_Up ) * step( _Noise_Step_Line_Down , temp_output_154_0 ) );
				#else
				float staticSwitch165 = staticSwitch158;
				#endif
				float Noise76 = saturate( staticSwitch165 );
				float temp_output_5_0 = pow( ( staticSwitch113 * Noise76 ) , _Main_Pow );
				float2 uv_MaskTex = input.ase_texcoord.xy * _MaskTex_ST.xy + _MaskTex_ST.zw;
				float4 tex2DNode60 = tex2D( _MaskTex, uv_MaskTex );
				float Mask84 = ( pow( tex2DNode60.r , _Mask_Pow ) * _Mask_Ins );
				float4 screenPos = input.ase_texcoord3;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float screenDepth129 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_positionSSNorm.xy ),_ZBufferParams);
				float distanceDepth129 = ( screenDepth129 - LinearEyeDepth( ase_positionSSNorm.z,_ZBufferParams ) ) / ( _Depth );
				

				surfaceDescription.Alpha = ( input.ase_color.a * ( temp_output_5_0 * _Opacity ) * Mask84 * saturate( distanceDepth129 ) );
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
        	#pragma multi_compile_instancing
        	#pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
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
			#define ASE_NEEDS_TEXTURE_COORDINATES2
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES2
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_TEXTURE_COORDINATES4
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES4
			#define ASE_NEEDS_FRAG_SCREEN_POSITION_NORMALIZED
			#pragma shader_feature_local _MAIN_CIRCLE_OFFSET_ONOFF_ON
			#pragma shader_feature_local _DISTORTION_VERTEXTEX_ONOFF_ON
			#pragma shader_feature_local _PANNERVERTEXTEXCOORD_ON
			#pragma shader_feature_local _CIRCLE_VERTEX_ONOFF_ON
			#pragma shader_feature_local _NOISE_STEP_LINEONOFF_ON
			#pragma shader_feature_local _NOISE_STEP_MASK_ONOFF_ON
			#pragma shader_feature_local _DISSOLVE_TEXCROOD_ONOFF_ON
			#pragma shader_feature_local _NOISE_MASK_OFFSET_TEXCOORDONOFF_ON


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
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord4 : TEXCOORD4;
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
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Emi_Color;
			float4 _Color_A;
			float4 _Color_B;
			float4 _MaskTex_ST;
			float4 _NormalTex_ST;
			float4 _NoiseTex_ST;
			float4 _MainTex_ST;
			float _Dissolve;
			float _Noise_Step_Offset;
			float _Noise_Mask_Offset;
			float _Noise_Step_Line_Down;
			float _Noise_Step_Line_Up;
			float _Main_Pow;
			float _Opacity;
			float _Mask_Pow;
			float _Noise_Mask_Step_Offset;
			float _Noise_Vpanner;
			float _Noise_Distortion;
			float _Mask_Ins;
			float _InCircle;
			float _OutCircle;
			float _Circle_Move;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Distortion;
			float _Normal_Vpanner;
			float _Normal_Upanner;
			float _Emi_Ins;
			float _Gradation_Color_Offset;
			float _Noise_Upanner;
			float _Depth;
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

			sampler2D _MainTex;
			sampler2D _NormalTex;
			sampler2D _NoiseTex;
			sampler2D _MaskTex;


			
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

				output.ase_color = input.ase_color;
				output.ase_texcoord2.xy = input.ase_texcoord.xy;
				output.ase_texcoord3 = input.ase_texcoord2;
				output.ase_texcoord4 = input.ase_texcoord4;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

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
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord4 : TEXCOORD4;

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
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord2 = input.ase_texcoord2;
				output.ase_texcoord4 = input.ase_texcoord4;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				output.ase_texcoord4 = patch[0].ase_texcoord4 * bary.x + patch[1].ase_texcoord4 * bary.y + patch[2].ase_texcoord4 * bary.z;
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

				float2 appendResult25 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_NormalTex = input.ase_texcoord2.xy * _NormalTex_ST.xy + _NormalTex_ST.zw;
				float2 panner24 = ( 1.0 * _Time.y * appendResult25 + uv_NormalTex);
				#ifdef _DISTORTION_VERTEXTEX_ONOFF_ON
				float staticSwitch40 = input.ase_texcoord3.z;
				#else
				float staticSwitch40 = _Distortion;
				#endif
				float2 temp_output_34_0 = ( (UnpackNormalScale( tex2D( _NormalTex, panner24 ), 1.0f )).xy * staticSwitch40 );
				float2 Normal37 = temp_output_34_0;
				float2 appendResult65 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 uv_MainTex = input.ase_texcoord2.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner66 = ( 1.0 * _Time.y * appendResult65 + uv_MainTex);
				float2 Main_Panner79 = panner66;
				float2 appendResult75 = (float2(( uv_MainTex.x + input.ase_texcoord3.x ) , ( uv_MainTex.y + input.ase_texcoord3.y )));
				float2 Main_VertexTexcoord80 = appendResult75;
				#ifdef _PANNERVERTEXTEXCOORD_ON
				float2 staticSwitch70 = Main_VertexTexcoord80;
				#else
				float2 staticSwitch70 = Main_Panner79;
				#endif
				float2 temp_cast_0 = (-0.5).xx;
				float2 texCoord102 = input.ase_texcoord2.xy * float2( 1,1 ) + temp_cast_0;
				#ifdef _CIRCLE_VERTEX_ONOFF_ON
				float staticSwitch103 = input.ase_texcoord4.y;
				#else
				float staticSwitch103 = _Circle_Move;
				#endif
				float temp_output_105_0 = ( length( texCoord102 ) + staticSwitch103 );
				float Circle_Offset111 = ( step( temp_output_105_0 , _OutCircle ) * step( _InCircle , temp_output_105_0 ) );
				#ifdef _MAIN_CIRCLE_OFFSET_ONOFF_ON
				float staticSwitch113 = Circle_Offset111;
				#else
				float staticSwitch113 = tex2D( _MainTex, ( Normal37 + staticSwitch70 ) ).r;
				#endif
				float2 Noise_Distortion126 = temp_output_34_0;
				float2 appendResult45 = (float2(_Noise_Upanner , _Noise_Vpanner));
				float2 uv_NoiseTex = input.ase_texcoord2.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				float2 panner47 = ( 1.0 * _Time.y * appendResult45 + uv_NoiseTex);
				#ifdef _DISSOLVE_TEXCROOD_ONOFF_ON
				float staticSwitch51 = input.ase_texcoord3.w;
				#else
				float staticSwitch51 = _Dissolve;
				#endif
				float temp_output_89_0 = ( tex2D( _NoiseTex, ( ( Noise_Distortion126 * _Noise_Distortion ) + panner47 ) ).r + staticSwitch51 );
				float2 texCoord147 = input.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _NOISE_MASK_OFFSET_TEXCOORDONOFF_ON
				float staticSwitch166 = input.ase_texcoord3.z;
				#else
				float staticSwitch166 = _Noise_Mask_Offset;
				#endif
				float temp_output_150_0 = ( texCoord147.y + staticSwitch166 );
				float temp_output_153_0 = ( temp_output_150_0 * temp_output_89_0 );
				#ifdef _NOISE_STEP_MASK_ONOFF_ON
				float staticSwitch158 = step( temp_output_153_0 , _Noise_Mask_Step_Offset );
				#else
				float staticSwitch158 = step( temp_output_89_0 , _Noise_Step_Offset );
				#endif
				float temp_output_154_0 = ( temp_output_153_0 + temp_output_150_0 );
				#ifdef _NOISE_STEP_LINEONOFF_ON
				float staticSwitch165 = ( step( temp_output_154_0 , _Noise_Step_Line_Up ) * step( _Noise_Step_Line_Down , temp_output_154_0 ) );
				#else
				float staticSwitch165 = staticSwitch158;
				#endif
				float Noise76 = saturate( staticSwitch165 );
				float temp_output_5_0 = pow( ( staticSwitch113 * Noise76 ) , _Main_Pow );
				float2 uv_MaskTex = input.ase_texcoord2.xy * _MaskTex_ST.xy + _MaskTex_ST.zw;
				float4 tex2DNode60 = tex2D( _MaskTex, uv_MaskTex );
				float Mask84 = ( pow( tex2DNode60.r , _Mask_Pow ) * _Mask_Ins );
				float screenDepth129 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ScreenPosNorm.xy ),_ZBufferParams);
				float distanceDepth129 = ( screenDepth129 - LinearEyeDepth( ScreenPosNorm.z,_ZBufferParams ) ) / ( _Depth );
				

				float3 Normal = float3(0, 0, 1);
				float Alpha = ( input.ase_color.a * ( temp_output_5_0 * _Opacity ) * Mask84 * saturate( distanceDepth129 ) );
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
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":126,"pos":[-1952,-784],"params":["Inherit","False","Noise_Distortion","-1","True","1","0","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":43,"pos":[-5824,752],"params":["Inherit","False","Property","_Noise_Vpanner","Noise_Vpanner","27","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":44,"pos":[-5824,688],"params":["Inherit","False","Property","_Noise_Upanner","Noise_Upanner","26","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":27,"pos":[-3200,-736],"params":["Inherit","False","Property","_Normal_Vpanner","Normal_Vpanner","35","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":26,"pos":[-3200,-800],"params":["Inherit","False","Property","_Normal_Upanner","Normal_Upanner","34","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor","id":45,"pos":[-5648,688],"params":["Inherit","False","FLOAT2","4","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":46,"pos":[-5712,576],"params":["Inherit","False","0","54","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":125,"pos":[-5888,480],"params":["Inherit","False","126","Noise_Distortion","1","0","OBJECT","","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":128,"pos":[-5680,512],"params":["Inherit","False","Property","_Noise_Distortion","Noise_Distortion","25","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor","id":25,"pos":[-2992,-800],"params":["Inherit","False","FLOAT2","4","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":28,"pos":[-3056,-912],"params":["Inherit","False","0","23","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":148,"pos":[-5520,368],"params":["Inherit","False","Property","_Noise_Mask_Offset","Noise_Mask_Offset","18","0","Create","True","0","0","0","False","0","False","Object","-1","","-0.1767709","0.4930715","-1","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.PannerNode, AmplifyShaderEditor","id":47,"pos":[-5488,576],"params":["Inherit","False","3","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","1","FLOAT","1","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":127,"pos":[-5472,480],"params":["Inherit","False","2","2","0","FLOAT2","0,0","False","1","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.TexCoordVertexDataNode, AmplifyShaderEditor","id":50,"pos":[-5568,800],"params":["Inherit","False","2","4","0","5","FLOAT4","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":68,"pos":[-2496,-176],"params":["Inherit","False","Property","_Main_Vpanner","Main_Vpanner","13","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":67,"pos":[-2496,-240],"params":["Inherit","False","Property","_Main_Upanner","Main_Upanner","12","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.PannerNode, AmplifyShaderEditor","id":24,"pos":[-2848,-848],"params":["Inherit","False","3","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","1","FLOAT","1","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":72,"pos":[-2480,-32],"params":["Inherit","False","0","2","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":99,"pos":[-3456,1504],"params":["Inherit","False","Constant","_Float0","Float 0","25","0","Create","True","0","0","0","False","0","False","Object","-1","","-0.5","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.TexCoordVertexDataNode, AmplifyShaderEditor","id":71,"pos":[-2480,80],"params":["Inherit","False","2","4","0","5","FLOAT4","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":147,"pos":[-4976,144],"params":["Inherit","False","0","-1","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":166,"pos":[-5232,400],"params":["Inherit","False","Property","_Noise_Mask_Offset_TexCoordOnOff","Noise_Mask_Offset_TexCoord On/Off","14","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT","0","False","0","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT","0","False","7","FLOAT","0","False","8","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":123,"pos":[-5296,528],"params":["Inherit","False","2","2","0","FLOAT2","0,0","False","1","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":52,"pos":[-5504,720],"params":["Inherit","False","Property","_Dissolve","Dissolve","24","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":51,"pos":[-5296,752],"params":["Inherit","False","Property","_Dissolve_TexCrood_OnOff","Dissolve_TexCrood_On/Off","15","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT","0","False","0","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT","0","False","7","FLOAT","0","False","8","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor","id":65,"pos":[-2320,-240],"params":["Inherit","False","FLOAT2","4","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":73,"pos":[-2256,16],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":74,"pos":[-2256,112],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":23,"pos":[-2672,-864],"params":["Inherit","True","Property","_NormalTex","NormalTex","32","0","Create","True","0","0","0","False","0","False","","-1","None","None","True","0","True","bump","Auto","True","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.TexCoordVertexDataNode, AmplifyShaderEditor","id":41,"pos":[-2928,-608],"params":["Inherit","False","2","4","0","5","FLOAT4","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":30,"pos":[-2896,-672],"params":["Inherit","False","Property","_Distortion","Distortion","33","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0.1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":69,"pos":[-2384,-352],"params":["Inherit","False","0","2","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.TexCoordVertexDataNode, AmplifyShaderEditor","id":100,"pos":[-3520,1824],"params":["Inherit","False","4","4","0","5","FLOAT4","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":101,"pos":[-3584,1744],"params":["Inherit","False","Property","_Circle_Move","Circle_Move","40","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","-0.5","0.5","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":102,"pos":[-3280,1456],"params":["Inherit","True","0","-1","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":54,"pos":[-5184,528],"params":["Inherit","True","Property","_NoiseTex","NoiseTex","23","0","Create","True","0","0","0","False","0","False","","-1","5cd11d5dc94a1484381073ff4d0d42ed","b62a4ecc69001cc4faf49d30e6d97185","True","0","False","white","Auto","False","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":150,"pos":[-4800,408],"params":["Inherit","True","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":89,"pos":[-4896,608],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":154,"pos":[-4448,544],"params":["Inherit","True","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.PannerNode, AmplifyShaderEditor","id":66,"pos":[-2176,-288],"params":["Inherit","False","3","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","1","FLOAT","1","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor","id":75,"pos":[-2144,48],"params":["Inherit","False","FLOAT2","4","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.ComponentMaskNode, AmplifyShaderEditor","id":22,"pos":[-2384,-864],"params":["Inherit","False","True","True","False","True","1","0","FLOAT3","0,0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":40,"pos":[-2720,-672],"params":["Inherit","False","Property","_Distortion_VertexTex_OnOff","Distortion_VertexTex_On/Off","31","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT","0","False","0","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT","0","False","7","FLOAT","0","False","8","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":103,"pos":[-3296,1744],"params":["Inherit","False","Property","_Circle_Vertex_OnOff","Circle_Vertex_On/Off","37","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT","0","False","0","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT","0","False","7","FLOAT","0","False","8","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.LengthOpNode, AmplifyShaderEditor","id":104,"pos":[-3072,1456],"params":["Inherit","True","1","0","FLOAT2","0,0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":156,"pos":[-4512,464],"params":["Inherit","False","Property","_Noise_Step_Offset","Noise_Step_Offset","16","0","Create","True","0","0","0","False","0","False","Object","-1","","0.1","0.1","0","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":145,"pos":[-4560,880],"params":["Inherit","False","Property","_Noise_Mask_Step_Offset","Noise_Mask_Step_Offset","19","0","Create","True","0","0","0","False","0","False","Object","-1","","0.04788033","0.3","-0.5","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":153,"pos":[-4688,672],"params":["Inherit","True","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":164,"pos":[-4704,1088],"params":["Inherit","False","Property","_Noise_Step_Line_Up","Noise_Step_Line_Up","21","0","Create","True","0","0","0","False","0","False","Object","-1","","0.3511167","0.5","0","0.5","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":163,"pos":[-4704,1168],"params":["Inherit","False","Property","_Noise_Step_Line_Down","Noise_Step_Line_Down","22","0","Create","True","0","0","0","False","0","False","Object","-1","","0.08561724","0.4","0","0.5","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":79,"pos":[-1952,-288],"params":["Inherit","False","Main_Panner","-1","True","1","0","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":80,"pos":[-1952,48],"params":["Inherit","False","Main_VertexTexcoord","-1","True","1","0","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":34,"pos":[-2176,-864],"params":["Inherit","False","2","2","0","FLOAT2","0,0","False","1","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":105,"pos":[-2912,1456],"params":["Inherit","True","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor","id":155,"pos":[-4224,352],"params":["Inherit","True","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":107,"pos":[-2944,1680],"params":["Inherit","False","Property","_InCircle","InCircle","38","0","Create","True","0","0","0","False","0","False","Object","-1","","0.4","0.4","0","0.5","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":106,"pos":[-2976,1376],"params":["Inherit","False","Property","_OutCircle","OutCircle","39","0","Create","True","0","0","0","False","0","False","Object","-1","","0.5","0.5","0","0.5","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor","id":144,"pos":[-4192,656],"params":["Inherit","True","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor","id":160,"pos":[-4160,976],"params":["Inherit","True","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor","id":161,"pos":[-4160,1184],"params":["Inherit","True","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":37,"pos":[-1952,-864],"params":["Inherit","False","Normal","-1","True","1","0","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":83,"pos":[-1584,96],"params":["Inherit","False","80","Main_VertexTexcoord","1","0","OBJECT","","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":82,"pos":[-1552,32],"params":["Inherit","False","79","Main_Panner","1","0","OBJECT","","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":158,"pos":[-3952,432],"params":["Inherit","True","Property","_Noise_Step_Mask_OnOff","Noise_Step_Mask_On/Off","17","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT","0","False","0","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT","0","False","7","FLOAT","0","False","8","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor","id":108,"pos":[-2672,1344],"params":["Inherit","True","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor","id":109,"pos":[-2672,1552],"params":["Inherit","True","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":162,"pos":[-3936,976],"params":["Inherit","True","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":70,"pos":[-1328,64],"params":["Inherit","False","Property","_PannerVertexTexcoord","Panner/VertexTexcoord","2","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT2","0,0","False","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT2","0,0","False","6","FLOAT2","0,0","False","7","FLOAT2","0,0","False","8","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":38,"pos":[-1200,0],"params":["Inherit","False","37","Normal","1","0","OBJECT","","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":110,"pos":[-2464,1344],"params":["Inherit","True","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":165,"pos":[-3648,432],"params":["Inherit","True","Property","_Noise_Step_LineOnOff","Noise_Step_Line On/Off","20","0","Create","True","0","0","0","False","0","False","","0","0","1","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT","0","False","0","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT","0","False","7","FLOAT","0","False","8","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":33,"pos":[-1008,32],"params":["Inherit","False","2","2","0","FLOAT2","0,0","False","1","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":111,"pos":[-2256,1344],"params":["Inherit","False","Circle_Offset","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":90,"pos":[-3312,432],"params":["Inherit","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":98,"pos":[-816,192],"params":["Inherit","False","111","Circle_Offset","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":2,"pos":[-896,0],"params":["Inherit","True","Property","_MainTex","MainTex","9","0","Create","True","0","0","0","False","0","False","","-1","None","ff65351aded79004db909c54915cb69c","True","0","False","white","Auto","False","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":60,"pos":[-2784,880],"params":["Inherit","True","Property","_MaskTex","MaskTex","28","0","Create","True","0","0","0","False","0","False","","-1","None","None","True","0","False","white","Auto","False","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":61,"pos":[-2464,1008],"params":["Float","False","Property","_Mask_Pow","Mask_Pow","29","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":76,"pos":[-2432,400],"params":["Inherit","False","Noise","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":77,"pos":[-384,144],"params":["Inherit","False","76","Noise","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":113,"pos":[-560,48],"params":["Inherit","False","Property","_Main_Circle_Offset_OnOFf","Main_Circle_Offset_On/OFf","36","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT","0","False","0","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT","0","False","7","FLOAT","0","False","8","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.PowerNode, AmplifyShaderEditor","id":62,"pos":[-2320,928],"params":["Inherit","False","False","2","0","FLOAT","0","False","1","FLOAT","1","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":63,"pos":[-2320,1024],"params":["Float","False","Property","_Mask_Ins","Mask_Ins","30","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":11,"pos":[-128,144],"params":["Float","False","Property","_Main_Pow","Main_Pow","10","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":64,"pos":[-2160,928],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":59,"pos":[-208,48],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":130,"pos":[128,512],"params":["Inherit","False","Property","_Depth","Depth","1","0","Create","True","0","0","0","False","0","False","Object","-1","","0.72","0.1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.PowerNode, AmplifyShaderEditor","id":5,"pos":[48,48],"params":["Inherit","False","False","2","0","FLOAT","0","False","1","FLOAT","1","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":58,"pos":[288,192],"params":["Inherit","False","Property","_Opacity","Opacity","0","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":84,"pos":[-1744,944],"params":["Inherit","False","Mask","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.DepthFade, AmplifyShaderEditor","id":129,"pos":[288,480],"params":["Inherit","False","True","False","False","2","1","FLOAT3","0,0,0","False","0","FLOAT","1","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.VertexColorNode, AmplifyShaderEditor","id":12,"pos":[432,16],"params":["Inherit","False","0","5","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":85,"pos":[496,400],"params":["Inherit","False","84","Mask","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":56,"pos":[480,176],"params":["Inherit","True","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":131,"pos":[528,480],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":92,"pos":[-3248,-960],"params":["Inherit","False","1252","561.3333","Normal","11","27","26","25","28","24","23","22","30","41","40","34","","1,1,1,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":112,"pos":[-3632,1296],"params":["Inherit","False","1620","737.3333","Circle_Offset","13","99","100","101","102","103","104","105","106","107","108","109","110","111","","1,1,1,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":81,"pos":[-2496,-80],"params":["Inherit","False","491.3689","345.0998","Main_VertexTexcoord","5","72","75","74","73","71","","1,1,1,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":78,"pos":[-2512,-384],"params":["Inherit","False","513.7705","297.6611","Main_Panner","5","66","69","65","67","68","","1,1,1,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":88,"pos":[-2800,832],"params":["Inherit","False","793.3595","445.0285","Mask","7","95","96","64","63","62","61","60","","1,1,1,1","0","0"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":121,"pos":[-464,-208],"params":["Inherit","False","0","-1","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":119,"pos":[-528,-96],"params":["Float","False","Property","_Gradation_Color_Offset","Gradation_Color_Offset","4","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","-1","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":120,"pos":[-272,-176],"params":["Inherit","True","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":118,"pos":[-272,-528],"params":["Float","False","Property","_Color_A","Color_A","5","1","[HDR]","Create","True","0","0","0","False","0","False","Object","-1","","1,0,0,0","1,0,0,0","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":117,"pos":[-272,-352],"params":["Float","False","Property","_Color_B","Color_B","6","1","[HDR]","Create","True","0","0","0","False","0","False","Object","-1","","1,1,1,0","1,1,1,0","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":116,"pos":[-16,-144],"params":["Inherit","False","3","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","2","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":122,"pos":[144,-160],"params":["Inherit","False","Property","_Gradation_Color_OnOff","Gradation_Color_On/Off","3","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","COLOR","0,0,0,0","False","0","COLOR","0,0,0,0","False","2","COLOR","0,0,0,0","False","3","COLOR","0,0,0,0","False","4","COLOR","0,0,0,0","False","5","COLOR","0,0,0,0","False","6","COLOR","0,0,0,0","False","7","COLOR","0,0,0,0","False","8","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":114,"pos":[304,-64],"params":["Float","False","Property","_Emi_Ins","Emi_Ins","8","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":115,"pos":[448,-96],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":93,"pos":[672,0],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":94,"pos":[-16,-352],"params":["Inherit","False","Property","_Emi_Color","Emi_Color","7","1","[HDR]","Create","True","0","0","0","False","0","False","Object","-1","","1,1,1,0","1,1,1,0","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":8,"pos":[-80,256],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":10,"pos":[-288,272],"params":["Float","False","Property","_Main_Ins","Main_Ins","11","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.TexCoordVertexDataNode, AmplifyShaderEditor","id":96,"pos":[-2784,1104],"params":["Inherit","False","4","4","0","5","FLOAT4","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":95,"pos":[-2464,1072],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":21,"pos":[720,128],"params":["Inherit","True","4","4","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":132,"pos":[896,-32],"params":["Float","False","False","-1","3","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","ExtraPrePass","0","0","ExtraPrePass","6","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","5","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","True","3","False","","True","True","0","False","","0","False","","False","True","0","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":134,"pos":[896,-32],"params":["Float","False","False","-1","3","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","ShadowCaster","0","2","ShadowCaster","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","5","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","False","False","True","False","False","False","False","0","False","","False","False","False","False","False","False","False","False","False","True","1","False","","True","3","False","","False","False","True","1","LightMode=ShadowCaster","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":135,"pos":[896,-32],"params":["Float","False","False","-1","3","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","DepthOnly","0","3","DepthOnly","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","5","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","False","False","True","False","False","False","False","0","False","","False","False","False","False","False","False","False","False","False","True","1","False","","False","False","False","True","1","LightMode=DepthOnly","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":136,"pos":[896,-32],"params":["Float","False","False","-1","3","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","Meta","0","4","Meta","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","5","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","2","False","","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","LightMode=Meta","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":137,"pos":[896,-32],"params":["Float","False","False","-1","3","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","Universal2D","0","5","Universal2D","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","5","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","True","3","False","","True","True","0","False","","0","False","","False","True","1","LightMode=Universal2D","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":138,"pos":[896,-32],"params":["Float","False","False","-1","3","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","SceneSelectionPass","0","6","SceneSelectionPass","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","5","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","2","False","","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","LightMode=SceneSelectionPass","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":139,"pos":[896,-32],"params":["Float","False","False","-1","3","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","ScenePickingPass","0","7","ScenePickingPass","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","5","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","LightMode=Picking","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":140,"pos":[896,-32],"params":["Float","False","False","-1","3","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","DepthNormals","0","8","DepthNormals","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","5","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","False","","True","3","False","","False","False","True","1","LightMode=DepthNormalsOnly","False","False","0","","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":141,"pos":[896,-32],"params":["Float","False","False","-1","3","UnityEditor.ShaderGraphUnlitGUI","0","1","New Amplify Shader","2992e84f91cbeb14eab234972e07ea9d","True","DepthNormalsOnly","0","9","DepthNormalsOnly","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","UniversalMaterialType=Unlit","True","5","True","12","all","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","False","","True","3","False","","False","False","True","1","LightMode=DepthNormalsOnly","False","True","9","d3d11","metal","vulkan","xboxone","xboxseries","playstation","ps4","ps5","switch","0","0","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":133,"pos":[992,0],"params":["Float","False","True","-1","3","UnityEditor.ShaderGraphUnlitGUI","0","19","FXS/FX_Default_Alp_Stylized_URP","2992e84f91cbeb14eab234972e07ea9d","True","Forward","0","1","Forward","11","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","2","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","4","RenderPipeline=UniversalPipeline","RenderType=Transparent=RenderType","Queue=Transparent=Queue=0","UniversalMaterialType=Unlit","True","5","True","12","all","0","True","True","1","5","False","","10","False","","1","0","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","False","False","False","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","2","False","","True","3","False","","True","True","0","False","","0","False","","False","True","1","LightMode=UniversalForwardOnly","False","False","0","","0","0","Standard","28","Surface","1","639157160128781887","  Keep Alpha","0","0","  Blend","0","0","Two Sided","0","639157160158299712","Alpha Clipping","0","0","  Use Shadow Threshold","0","0","Fragment Normal Space","0","0","Forward Only","0","0","Cast Shadows","0","639157160169390976","Receive Shadows","2","0","Receive SSAO","1","0","GPU Instancing","1","0","LOD CrossFade","1","0","Built-in Fog","1","0","Meta Pass","0","0","Extra Pre Pass","0","0","Tessellation","0","0","  Phong","0","0","  Strength","0.5,False,","0","  Type","0","0","  Tess","16,False,","0","  Min","10,False,","0","  Max","25,False,","0","  Edge Length","16,False,","0","  Max Displacement","25,False,","0","Write Depth","0","0","  Conservative","0","0","Vertex Position","1","0","0","10","False","True","False","True","False","False","True","True","True","False","False","","False","0"]}
{"wire":[126,0,34,0]}
{"wire":[45,0,44,0]}
{"wire":[45,1,43,0]}
{"wire":[25,0,26,0]}
{"wire":[25,1,27,0]}
{"wire":[47,0,46,0]}
{"wire":[47,2,45,0]}
{"wire":[127,0,125,0]}
{"wire":[127,1,128,0]}
{"wire":[24,0,28,0]}
{"wire":[24,2,25,0]}
{"wire":[166,1,148,0]}
{"wire":[166,0,50,3]}
{"wire":[123,0,127,0]}
{"wire":[123,1,47,0]}
{"wire":[51,1,52,0]}
{"wire":[51,0,50,4]}
{"wire":[65,0,67,0]}
{"wire":[65,1,68,0]}
{"wire":[73,0,72,1]}
{"wire":[73,1,71,1]}
{"wire":[74,0,72,2]}
{"wire":[74,1,71,2]}
{"wire":[23,1,24,0]}
{"wire":[102,1,99,0]}
{"wire":[54,1,123,0]}
{"wire":[150,0,147,2]}
{"wire":[150,1,166,0]}
{"wire":[89,0,54,1]}
{"wire":[89,1,51,0]}
{"wire":[154,0,153,0]}
{"wire":[154,1,150,0]}
{"wire":[66,0,69,0]}
{"wire":[66,2,65,0]}
{"wire":[75,0,73,0]}
{"wire":[75,1,74,0]}
{"wire":[22,0,23,0]}
{"wire":[40,1,30,0]}
{"wire":[40,0,41,3]}
{"wire":[103,1,101,0]}
{"wire":[103,0,100,2]}
{"wire":[104,0,102,0]}
{"wire":[153,0,150,0]}
{"wire":[153,1,89,0]}
{"wire":[79,0,66,0]}
{"wire":[80,0,75,0]}
{"wire":[34,0,22,0]}
{"wire":[34,1,40,0]}
{"wire":[105,0,104,0]}
{"wire":[105,1,103,0]}
{"wire":[155,0,89,0]}
{"wire":[155,1,156,0]}
{"wire":[144,0,153,0]}
{"wire":[144,1,145,0]}
{"wire":[160,0,154,0]}
{"wire":[160,1,164,0]}
{"wire":[161,0,163,0]}
{"wire":[161,1,154,0]}
{"wire":[37,0,34,0]}
{"wire":[158,1,155,0]}
{"wire":[158,0,144,0]}
{"wire":[108,0,105,0]}
{"wire":[108,1,106,0]}
{"wire":[109,0,107,0]}
{"wire":[109,1,105,0]}
{"wire":[162,0,160,0]}
{"wire":[162,1,161,0]}
{"wire":[70,1,82,0]}
{"wire":[70,0,83,0]}
{"wire":[110,0,108,0]}
{"wire":[110,1,109,0]}
{"wire":[165,1,158,0]}
{"wire":[165,0,162,0]}
{"wire":[33,0,38,0]}
{"wire":[33,1,70,0]}
{"wire":[111,0,110,0]}
{"wire":[90,0,165,0]}
{"wire":[2,1,33,0]}
{"wire":[76,0,90,0]}
{"wire":[113,1,2,1]}
{"wire":[113,0,98,0]}
{"wire":[62,0,60,1]}
{"wire":[62,1,61,0]}
{"wire":[64,0,62,0]}
{"wire":[64,1,63,0]}
{"wire":[59,0,113,0]}
{"wire":[59,1,77,0]}
{"wire":[5,0,59,0]}
{"wire":[5,1,11,0]}
{"wire":[84,0,64,0]}
{"wire":[129,0,130,0]}
{"wire":[56,0,5,0]}
{"wire":[56,1,58,0]}
{"wire":[131,0,129,0]}
{"wire":[120,0,121,1]}
{"wire":[120,1,119,0]}
{"wire":[116,0,118,0]}
{"wire":[116,1,117,0]}
{"wire":[116,2,120,0]}
{"wire":[122,1,94,0]}
{"wire":[122,0,116,0]}
{"wire":[115,0,122,0]}
{"wire":[115,1,114,0]}
{"wire":[93,0,115,0]}
{"wire":[93,1,12,0]}
{"wire":[8,0,5,0]}
{"wire":[8,1,10,0]}
{"wire":[95,0,60,1]}
{"wire":[95,1,96,1]}
{"wire":[21,0,12,4]}
{"wire":[21,1,56,0]}
{"wire":[21,2,85,0]}
{"wire":[21,3,131,0]}
{"wire":[133,2,93,0]}
{"wire":[133,3,21,0]}
ASEEND*/
//CHKSM=430005A60B01830A506C7137778EDADDA244B3A6