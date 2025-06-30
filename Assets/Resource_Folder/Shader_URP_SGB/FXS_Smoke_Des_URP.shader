// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Amplify Shader/SGB/URP/FX_Smoke_Des"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HDR]_Color("Color", Color) = (1,1,1,0)
		_Main_TEX("Main_TEX", 2D) = "white" {}
		[HDR]_Main_power("Main_power", Float) = 1
		_Normal_TEX("Normal_TEX", 2D) = "bump" {}
		_Distortion("Distortion", Range( 0 , 1)) = 0.5753834
		_Distortion_Speed("Distortion_Speed", Vector) = (0,-1,0,0)
		_Distortion_Tiling("Distortion_Tiling", Vector) = (2,0.5,0,0)
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Noise_T("Noise_T", 2D) = "white" {}
		_Noise_Tiling("Noise_Tiling", Vector) = (0.66,0.1,0,0)
		_Noise_Panner_Speed("Noise_Panner_Speed", Vector) = (0,-0.5,0,0)
		_Float7("Float 7", Float) = -2
		_DepthFade("Depth Fade", Range( -1 , 1)) = 0
		_desaturate("desaturate", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}


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

		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" }

		Cull Back
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 4.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

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

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 140011
			#define REQUIRE_DEPTH_TEXTURE 1


			

			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
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

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_SCREEN_POSITION


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					half4 fogFactorAndVertexLight : TEXCOORD2;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD3;
				#endif
				#if defined(LIGHTMAP_ON)
					float4 lightmapUVOrVertexSH : TEXCOORD4;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					float2 dynamicLightmapUV : TEXCOORD5;
				#endif
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_color : COLOR;
				float4 ase_texcoord8 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _TextureSample1_ST;
			float4 _Color;
			float2 _Distortion_Tiling;
			float2 _Distortion_Speed;
			float2 _Noise_Tiling;
			float2 _Noise_Panner_Speed;
			float _Distortion;
			float _desaturate;
			float _Main_power;
			float _Float7;
			float _DepthFade;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Main_TEX;
			sampler2D _Normal_TEX;
			sampler2D _Sampler60187;
			sampler2D _TextureSample1;
			sampler2D _Noise_T;
			sampler2D _Sampler60202;


			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.ase_texcoord6.xy = input.texcoord.xy;
				output.ase_texcoord7 = input.texcoord1;
				output.ase_color = input.ase_color;
				output.ase_texcoord8 = input.ase_texcoord3;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord6.zw = 0;

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

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV(input.texcoord1, unity_LightmapST, output.lightmapUVOrVertexSH.xy);
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					output.dynamicLightmapUV.xy = input.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					output.fogFactorAndVertexLight = 0;
					#if defined(ASE_FOG) && !defined(_FOG_FRAGMENT)
						output.fogFactorAndVertexLight.x = ComputeFogFactor(vertexInput.positionCS.z);
					#endif
					#ifdef _ADDITIONAL_LIGHTS_VERTEX
						half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );
						output.fogFactorAndVertexLight.yzw = vertexLight;
					#endif
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;

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
				output.texcoord = input.texcoord;
				output.ase_color = input.ase_color;
				output.ase_texcoord3 = input.ase_texcoord3;
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
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
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
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				float3 WorldPosition = input.positionWS;
				float3 WorldViewDirection = GetWorldSpaceNormalizeViewDir( WorldPosition );
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				float2 NormalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 temp_output_1_0_g5 = float2( 1,1 );
				float2 texCoord80_g5 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g5 = (float2(( (temp_output_1_0_g5).x * texCoord80_g5.x ) , ( texCoord80_g5.y * (temp_output_1_0_g5).y )));
				float2 temp_output_11_0_g5 = float2( 0,0 );
				float2 texCoord81_g5 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g5 = ( ( (temp_output_11_0_g5).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g5);
				float2 panner19_g5 = ( ( _TimeParameters.x * (temp_output_11_0_g5).y ) * float2( 0,1 ) + texCoord81_g5);
				float2 appendResult24_g5 = (float2((panner18_g5).x , (panner19_g5).y));
				float2 temp_output_47_0_g5 = _Distortion_Speed;
				float2 texCoord78_g5 = input.ase_texcoord6.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g5 = ( texCoord78_g5 - float2( 1,1 ) );
				float2 appendResult39_g5 = (float2(frac( ( atan2( (temp_output_31_0_g5).x , (temp_output_31_0_g5).y ) / TWO_PI ) ) , length( temp_output_31_0_g5 )));
				float2 panner54_g5 = ( ( (temp_output_47_0_g5).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g5);
				float2 panner55_g5 = ( ( _TimeParameters.x * (temp_output_47_0_g5).y ) * float2( 0,1 ) + appendResult39_g5);
				float2 appendResult58_g5 = (float2((panner54_g5).x , (panner55_g5).y));
				float2 uv_TextureSample1 = input.ase_texcoord6.xy * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
				float2 Distortion199 = ( (UnpackNormalScale( tex2D( _Normal_TEX, ( ( (tex2D( _Sampler60187, ( appendResult10_g5 + appendResult24_g5 ) )).rg * 1.0 ) + ( _Distortion_Tiling * appendResult58_g5 ) ) ), 1.0f )).xy * _Distortion * ( 1.0 - ( tex2D( _TextureSample1, uv_TextureSample1 ).r + 0.12 ) ) );
				float2 texCoord205 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_4_0_g7 = 2.0;
				float temp_output_5_0_g7 = 2.0;
				// *** BEGIN Flipbook UV Animation vars ***
				// Total tiles of Flipbook Texture
				float fbtotaltiles246_g7 = min( temp_output_4_0_g7 * temp_output_5_0_g7, ( ( temp_output_4_0_g7 * temp_output_5_0_g7 ) - 0.0 ) + 1 );
				// Offsets for cols and rows of Flipbook Texture
				float fbcolsoffset246_g7 = 1.0f / temp_output_4_0_g7;
				float fbrowsoffset246_g7 = 1.0f / temp_output_5_0_g7;
				// Speed of animation
				float fbspeed246_g7 = _TimeParameters.x * 0.0;
				// UV Tiling (col and row offset)
				float2 fbtiling246_g7 = float2(fbcolsoffset246_g7, fbrowsoffset246_g7);
				// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
				// Calculate current tile linear index
				float fbcurrenttileindex246_g7 = floor( fmod( fbspeed246_g7 + input.ase_texcoord7.x, fbtotaltiles246_g7) );
				fbcurrenttileindex246_g7 += ( fbcurrenttileindex246_g7 < 0) ? fbtotaltiles246_g7 : 0;
				// Obtain Offset X coordinate from current tile linear index
				float fblinearindextox246_g7 = round ( fmod ( fbcurrenttileindex246_g7, temp_output_4_0_g7 ) );
				// Multiply Offset X by coloffset
				float fboffsetx246_g7 = fblinearindextox246_g7 * fbcolsoffset246_g7;
				// Obtain Offset Y coordinate from current tile linear index
				float fblinearindextoy246_g7 = round( fmod( ( fbcurrenttileindex246_g7 - fblinearindextox246_g7 ) / temp_output_4_0_g7, temp_output_5_0_g7 ) );
				// Reverse Y to get tiles from Top to Bottom
				fblinearindextoy246_g7 = (int)(temp_output_5_0_g7-1) - fblinearindextoy246_g7;
				// Multiply Offset Y by rowoffset
				float fboffsety246_g7 = fblinearindextoy246_g7 * fbrowsoffset246_g7;
				// UV Offset
				float2 fboffset246_g7 = float2(fboffsetx246_g7, fboffsety246_g7);
				// Flipbook UV
				half2 fbuv246_g7 = texCoord205 * fbtiling246_g7 + fboffset246_g7;
				// *** END Flipbook UV Animation vars ***
				int flipbookFrame246_g7 = ( ( int )fbcurrenttileindex246_g7);
				float3 desaturateInitialColor228 = tex2D( _Main_TEX, ( Distortion199 + fbuv246_g7 ) ).rgb;
				float desaturateDot228 = dot( desaturateInitialColor228, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar228 = lerp( desaturateInitialColor228, desaturateDot228.xxx, _desaturate );
				float3 temp_cast_1 = (_Main_power).xxx;
				float4 appendResult237 = (float4(input.ase_texcoord8.x , input.ase_texcoord8.y , input.ase_texcoord8.z , input.ase_texcoord8.w));
				
				float2 texCoord197 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1_0_g6 = float2( 1,1 );
				float2 texCoord80_g6 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g6 = (float2(( (temp_output_1_0_g6).x * texCoord80_g6.x ) , ( texCoord80_g6.y * (temp_output_1_0_g6).y )));
				float2 temp_output_11_0_g6 = float2( 0,0 );
				float2 texCoord81_g6 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g6 = ( ( (temp_output_11_0_g6).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g6);
				float2 panner19_g6 = ( ( _TimeParameters.x * (temp_output_11_0_g6).y ) * float2( 0,1 ) + texCoord81_g6);
				float2 appendResult24_g6 = (float2((panner18_g6).x , (panner19_g6).y));
				float2 temp_output_47_0_g6 = _Noise_Panner_Speed;
				float2 texCoord78_g6 = input.ase_texcoord6.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g6 = ( texCoord78_g6 - float2( 1,1 ) );
				float2 appendResult39_g6 = (float2(frac( ( atan2( (temp_output_31_0_g6).x , (temp_output_31_0_g6).y ) / TWO_PI ) ) , length( temp_output_31_0_g6 )));
				float2 panner54_g6 = ( ( (temp_output_47_0_g6).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g6);
				float2 panner55_g6 = ( ( _TimeParameters.x * (temp_output_47_0_g6).y ) * float2( 0,1 ) + appendResult39_g6);
				float2 appendResult58_g6 = (float2((panner54_g6).x , (panner55_g6).y));
				float4 temp_cast_6 = (2.0).xxxx;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float screenDepth233 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_positionSSNorm.xy ),_ZBufferParams);
				float distanceDepth233 = abs( ( screenDepth233 - LinearEyeDepth( ase_positionSSNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( ( float4( pow( desaturateVar228 , temp_cast_1 ) , 0.0 ) * _Color ) * input.ase_color * appendResult237 ).rgb;
				float Alpha = ( input.ase_color.a * saturate( ( float4( desaturateVar228 , 0.0 ) * ( ( ( 1.0 - ( length( ( texCoord197 + -0.5 ) ) * 1.0 ) ) + pow( tex2D( _Noise_T, ( Distortion199 + ( ( (tex2D( _Sampler60202, ( appendResult10_g6 + appendResult24_g6 ) )).rg * 1.0 ) + ( _Noise_Tiling * appendResult58_g6 ) ) ) ) , temp_cast_6 ) ) + (_Float7 + (input.ase_texcoord7.y - 0.0) * (1.0 - _Float7) / (1.0 - 0.0)) ) ) ) * saturate( distanceDepth233 ) ).r;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;

				#ifdef ASE_FOG
					inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactorAndVertexLight.x);
				#endif
				#ifdef _ADDITIONAL_LIGHTS_VERTEX
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
				#endif

				inputData.normalizedScreenSpaceUV = NormalizedScreenSpaceUV;

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

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask R
			AlphaToMask Off

			HLSLPROGRAM

			

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
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

			#define ASE_NEEDS_FRAG_SCREEN_POSITION


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD1;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD2;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _TextureSample1_ST;
			float4 _Color;
			float2 _Distortion_Tiling;
			float2 _Distortion_Speed;
			float2 _Noise_Tiling;
			float2 _Noise_Panner_Speed;
			float _Distortion;
			float _desaturate;
			float _Main_power;
			float _Float7;
			float _DepthFade;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Main_TEX;
			sampler2D _Normal_TEX;
			sampler2D _Sampler60187;
			sampler2D _TextureSample1;
			sampler2D _Noise_T;
			sampler2D _Sampler60202;


			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.ase_color = input.ase_color;
				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				output.ase_texcoord4 = input.ase_texcoord1;
				
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

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = vertexInput.positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
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
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
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
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 temp_output_1_0_g5 = float2( 1,1 );
				float2 texCoord80_g5 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g5 = (float2(( (temp_output_1_0_g5).x * texCoord80_g5.x ) , ( texCoord80_g5.y * (temp_output_1_0_g5).y )));
				float2 temp_output_11_0_g5 = float2( 0,0 );
				float2 texCoord81_g5 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g5 = ( ( (temp_output_11_0_g5).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g5);
				float2 panner19_g5 = ( ( _TimeParameters.x * (temp_output_11_0_g5).y ) * float2( 0,1 ) + texCoord81_g5);
				float2 appendResult24_g5 = (float2((panner18_g5).x , (panner19_g5).y));
				float2 temp_output_47_0_g5 = _Distortion_Speed;
				float2 texCoord78_g5 = input.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g5 = ( texCoord78_g5 - float2( 1,1 ) );
				float2 appendResult39_g5 = (float2(frac( ( atan2( (temp_output_31_0_g5).x , (temp_output_31_0_g5).y ) / TWO_PI ) ) , length( temp_output_31_0_g5 )));
				float2 panner54_g5 = ( ( (temp_output_47_0_g5).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g5);
				float2 panner55_g5 = ( ( _TimeParameters.x * (temp_output_47_0_g5).y ) * float2( 0,1 ) + appendResult39_g5);
				float2 appendResult58_g5 = (float2((panner54_g5).x , (panner55_g5).y));
				float2 uv_TextureSample1 = input.ase_texcoord3.xy * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
				float2 Distortion199 = ( (UnpackNormalScale( tex2D( _Normal_TEX, ( ( (tex2D( _Sampler60187, ( appendResult10_g5 + appendResult24_g5 ) )).rg * 1.0 ) + ( _Distortion_Tiling * appendResult58_g5 ) ) ), 1.0f )).xy * _Distortion * ( 1.0 - ( tex2D( _TextureSample1, uv_TextureSample1 ).r + 0.12 ) ) );
				float2 texCoord205 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_4_0_g7 = 2.0;
				float temp_output_5_0_g7 = 2.0;
				// *** BEGIN Flipbook UV Animation vars ***
				// Total tiles of Flipbook Texture
				float fbtotaltiles246_g7 = min( temp_output_4_0_g7 * temp_output_5_0_g7, ( ( temp_output_4_0_g7 * temp_output_5_0_g7 ) - 0.0 ) + 1 );
				// Offsets for cols and rows of Flipbook Texture
				float fbcolsoffset246_g7 = 1.0f / temp_output_4_0_g7;
				float fbrowsoffset246_g7 = 1.0f / temp_output_5_0_g7;
				// Speed of animation
				float fbspeed246_g7 = _TimeParameters.x * 0.0;
				// UV Tiling (col and row offset)
				float2 fbtiling246_g7 = float2(fbcolsoffset246_g7, fbrowsoffset246_g7);
				// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
				// Calculate current tile linear index
				float fbcurrenttileindex246_g7 = floor( fmod( fbspeed246_g7 + input.ase_texcoord4.x, fbtotaltiles246_g7) );
				fbcurrenttileindex246_g7 += ( fbcurrenttileindex246_g7 < 0) ? fbtotaltiles246_g7 : 0;
				// Obtain Offset X coordinate from current tile linear index
				float fblinearindextox246_g7 = round ( fmod ( fbcurrenttileindex246_g7, temp_output_4_0_g7 ) );
				// Multiply Offset X by coloffset
				float fboffsetx246_g7 = fblinearindextox246_g7 * fbcolsoffset246_g7;
				// Obtain Offset Y coordinate from current tile linear index
				float fblinearindextoy246_g7 = round( fmod( ( fbcurrenttileindex246_g7 - fblinearindextox246_g7 ) / temp_output_4_0_g7, temp_output_5_0_g7 ) );
				// Reverse Y to get tiles from Top to Bottom
				fblinearindextoy246_g7 = (int)(temp_output_5_0_g7-1) - fblinearindextoy246_g7;
				// Multiply Offset Y by rowoffset
				float fboffsety246_g7 = fblinearindextoy246_g7 * fbrowsoffset246_g7;
				// UV Offset
				float2 fboffset246_g7 = float2(fboffsetx246_g7, fboffsety246_g7);
				// Flipbook UV
				half2 fbuv246_g7 = texCoord205 * fbtiling246_g7 + fboffset246_g7;
				// *** END Flipbook UV Animation vars ***
				int flipbookFrame246_g7 = ( ( int )fbcurrenttileindex246_g7);
				float3 desaturateInitialColor228 = tex2D( _Main_TEX, ( Distortion199 + fbuv246_g7 ) ).rgb;
				float desaturateDot228 = dot( desaturateInitialColor228, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar228 = lerp( desaturateInitialColor228, desaturateDot228.xxx, _desaturate );
				float2 texCoord197 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1_0_g6 = float2( 1,1 );
				float2 texCoord80_g6 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g6 = (float2(( (temp_output_1_0_g6).x * texCoord80_g6.x ) , ( texCoord80_g6.y * (temp_output_1_0_g6).y )));
				float2 temp_output_11_0_g6 = float2( 0,0 );
				float2 texCoord81_g6 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g6 = ( ( (temp_output_11_0_g6).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g6);
				float2 panner19_g6 = ( ( _TimeParameters.x * (temp_output_11_0_g6).y ) * float2( 0,1 ) + texCoord81_g6);
				float2 appendResult24_g6 = (float2((panner18_g6).x , (panner19_g6).y));
				float2 temp_output_47_0_g6 = _Noise_Panner_Speed;
				float2 texCoord78_g6 = input.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g6 = ( texCoord78_g6 - float2( 1,1 ) );
				float2 appendResult39_g6 = (float2(frac( ( atan2( (temp_output_31_0_g6).x , (temp_output_31_0_g6).y ) / TWO_PI ) ) , length( temp_output_31_0_g6 )));
				float2 panner54_g6 = ( ( (temp_output_47_0_g6).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g6);
				float2 panner55_g6 = ( ( _TimeParameters.x * (temp_output_47_0_g6).y ) * float2( 0,1 ) + appendResult39_g6);
				float2 appendResult58_g6 = (float2((panner54_g6).x , (panner55_g6).y));
				float4 temp_cast_2 = (2.0).xxxx;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float screenDepth233 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_positionSSNorm.xy ),_ZBufferParams);
				float distanceDepth233 = abs( ( screenDepth233 - LinearEyeDepth( ase_positionSSNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				

				float Alpha = ( input.ase_color.a * saturate( ( float4( desaturateVar228 , 0.0 ) * ( ( ( 1.0 - ( length( ( texCoord197 + -0.5 ) ) * 1.0 ) ) + pow( tex2D( _Noise_T, ( Distortion199 + ( ( (tex2D( _Sampler60202, ( appendResult10_g6 + appendResult24_g6 ) )).rg * 1.0 ) + ( _Noise_Tiling * appendResult58_g6 ) ) ) ) , temp_cast_2 ) ) + (_Float7 + (input.ase_texcoord4.y - 0.0) * (1.0 - _Float7) / (1.0 - 0.0)) ) ) ) * saturate( distanceDepth233 ) ).r;
				float AlphaClipThreshold = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
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
			#define ASE_VERSION 19801
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

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
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
			float4 _TextureSample1_ST;
			float4 _Color;
			float2 _Distortion_Tiling;
			float2 _Distortion_Speed;
			float2 _Noise_Tiling;
			float2 _Noise_Panner_Speed;
			float _Distortion;
			float _desaturate;
			float _Main_power;
			float _Float7;
			float _DepthFade;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Main_TEX;
			sampler2D _Normal_TEX;
			sampler2D _Sampler60187;
			sampler2D _TextureSample1;
			sampler2D _Noise_T;
			sampler2D _Sampler60202;


			
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

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				output.positionCS = TransformWorldToHClip(positionWS);

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
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
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
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

				float2 temp_output_1_0_g5 = float2( 1,1 );
				float2 texCoord80_g5 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g5 = (float2(( (temp_output_1_0_g5).x * texCoord80_g5.x ) , ( texCoord80_g5.y * (temp_output_1_0_g5).y )));
				float2 temp_output_11_0_g5 = float2( 0,0 );
				float2 texCoord81_g5 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g5 = ( ( (temp_output_11_0_g5).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g5);
				float2 panner19_g5 = ( ( _TimeParameters.x * (temp_output_11_0_g5).y ) * float2( 0,1 ) + texCoord81_g5);
				float2 appendResult24_g5 = (float2((panner18_g5).x , (panner19_g5).y));
				float2 temp_output_47_0_g5 = _Distortion_Speed;
				float2 texCoord78_g5 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g5 = ( texCoord78_g5 - float2( 1,1 ) );
				float2 appendResult39_g5 = (float2(frac( ( atan2( (temp_output_31_0_g5).x , (temp_output_31_0_g5).y ) / TWO_PI ) ) , length( temp_output_31_0_g5 )));
				float2 panner54_g5 = ( ( (temp_output_47_0_g5).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g5);
				float2 panner55_g5 = ( ( _TimeParameters.x * (temp_output_47_0_g5).y ) * float2( 0,1 ) + appendResult39_g5);
				float2 appendResult58_g5 = (float2((panner54_g5).x , (panner55_g5).y));
				float2 uv_TextureSample1 = input.ase_texcoord.xy * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
				float2 Distortion199 = ( (UnpackNormalScale( tex2D( _Normal_TEX, ( ( (tex2D( _Sampler60187, ( appendResult10_g5 + appendResult24_g5 ) )).rg * 1.0 ) + ( _Distortion_Tiling * appendResult58_g5 ) ) ), 1.0f )).xy * _Distortion * ( 1.0 - ( tex2D( _TextureSample1, uv_TextureSample1 ).r + 0.12 ) ) );
				float2 texCoord205 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_4_0_g7 = 2.0;
				float temp_output_5_0_g7 = 2.0;
				// *** BEGIN Flipbook UV Animation vars ***
				// Total tiles of Flipbook Texture
				float fbtotaltiles246_g7 = min( temp_output_4_0_g7 * temp_output_5_0_g7, ( ( temp_output_4_0_g7 * temp_output_5_0_g7 ) - 0.0 ) + 1 );
				// Offsets for cols and rows of Flipbook Texture
				float fbcolsoffset246_g7 = 1.0f / temp_output_4_0_g7;
				float fbrowsoffset246_g7 = 1.0f / temp_output_5_0_g7;
				// Speed of animation
				float fbspeed246_g7 = _TimeParameters.x * 0.0;
				// UV Tiling (col and row offset)
				float2 fbtiling246_g7 = float2(fbcolsoffset246_g7, fbrowsoffset246_g7);
				// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
				// Calculate current tile linear index
				float fbcurrenttileindex246_g7 = floor( fmod( fbspeed246_g7 + input.ase_texcoord1.x, fbtotaltiles246_g7) );
				fbcurrenttileindex246_g7 += ( fbcurrenttileindex246_g7 < 0) ? fbtotaltiles246_g7 : 0;
				// Obtain Offset X coordinate from current tile linear index
				float fblinearindextox246_g7 = round ( fmod ( fbcurrenttileindex246_g7, temp_output_4_0_g7 ) );
				// Multiply Offset X by coloffset
				float fboffsetx246_g7 = fblinearindextox246_g7 * fbcolsoffset246_g7;
				// Obtain Offset Y coordinate from current tile linear index
				float fblinearindextoy246_g7 = round( fmod( ( fbcurrenttileindex246_g7 - fblinearindextox246_g7 ) / temp_output_4_0_g7, temp_output_5_0_g7 ) );
				// Reverse Y to get tiles from Top to Bottom
				fblinearindextoy246_g7 = (int)(temp_output_5_0_g7-1) - fblinearindextoy246_g7;
				// Multiply Offset Y by rowoffset
				float fboffsety246_g7 = fblinearindextoy246_g7 * fbrowsoffset246_g7;
				// UV Offset
				float2 fboffset246_g7 = float2(fboffsetx246_g7, fboffsety246_g7);
				// Flipbook UV
				half2 fbuv246_g7 = texCoord205 * fbtiling246_g7 + fboffset246_g7;
				// *** END Flipbook UV Animation vars ***
				int flipbookFrame246_g7 = ( ( int )fbcurrenttileindex246_g7);
				float3 desaturateInitialColor228 = tex2D( _Main_TEX, ( Distortion199 + fbuv246_g7 ) ).rgb;
				float desaturateDot228 = dot( desaturateInitialColor228, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar228 = lerp( desaturateInitialColor228, desaturateDot228.xxx, _desaturate );
				float2 texCoord197 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1_0_g6 = float2( 1,1 );
				float2 texCoord80_g6 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g6 = (float2(( (temp_output_1_0_g6).x * texCoord80_g6.x ) , ( texCoord80_g6.y * (temp_output_1_0_g6).y )));
				float2 temp_output_11_0_g6 = float2( 0,0 );
				float2 texCoord81_g6 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g6 = ( ( (temp_output_11_0_g6).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g6);
				float2 panner19_g6 = ( ( _TimeParameters.x * (temp_output_11_0_g6).y ) * float2( 0,1 ) + texCoord81_g6);
				float2 appendResult24_g6 = (float2((panner18_g6).x , (panner19_g6).y));
				float2 temp_output_47_0_g6 = _Noise_Panner_Speed;
				float2 texCoord78_g6 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g6 = ( texCoord78_g6 - float2( 1,1 ) );
				float2 appendResult39_g6 = (float2(frac( ( atan2( (temp_output_31_0_g6).x , (temp_output_31_0_g6).y ) / TWO_PI ) ) , length( temp_output_31_0_g6 )));
				float2 panner54_g6 = ( ( (temp_output_47_0_g6).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g6);
				float2 panner55_g6 = ( ( _TimeParameters.x * (temp_output_47_0_g6).y ) * float2( 0,1 ) + appendResult39_g6);
				float2 appendResult58_g6 = (float2((panner54_g6).x , (panner55_g6).y));
				float4 temp_cast_2 = (2.0).xxxx;
				float4 screenPos = input.ase_texcoord2;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float screenDepth233 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_positionSSNorm.xy ),_ZBufferParams);
				float distanceDepth233 = abs( ( screenDepth233 - LinearEyeDepth( ase_positionSSNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				

				surfaceDescription.Alpha = ( input.ase_color.a * saturate( ( float4( desaturateVar228 , 0.0 ) * ( ( ( 1.0 - ( length( ( texCoord197 + -0.5 ) ) * 1.0 ) ) + pow( tex2D( _Noise_T, ( Distortion199 + ( ( (tex2D( _Sampler60202, ( appendResult10_g6 + appendResult24_g6 ) )).rg * 1.0 ) + ( _Noise_Tiling * appendResult58_g6 ) ) ) ) , temp_cast_2 ) ) + (_Float7 + (input.ase_texcoord1.y - 0.0) * (1.0 - _Float7) / (1.0 - 0.0)) ) ) ) * saturate( distanceDepth233 ) ).r;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
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
			#define ASE_VERSION 19801
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

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
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
			float4 _TextureSample1_ST;
			float4 _Color;
			float2 _Distortion_Tiling;
			float2 _Distortion_Speed;
			float2 _Noise_Tiling;
			float2 _Noise_Panner_Speed;
			float _Distortion;
			float _desaturate;
			float _Main_power;
			float _Float7;
			float _DepthFade;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Main_TEX;
			sampler2D _Normal_TEX;
			sampler2D _Sampler60187;
			sampler2D _TextureSample1;
			sampler2D _Noise_T;
			sampler2D _Sampler60202;


			
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

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );
				output.positionCS = TransformWorldToHClip(positionWS);
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
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
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
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

				float2 temp_output_1_0_g5 = float2( 1,1 );
				float2 texCoord80_g5 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g5 = (float2(( (temp_output_1_0_g5).x * texCoord80_g5.x ) , ( texCoord80_g5.y * (temp_output_1_0_g5).y )));
				float2 temp_output_11_0_g5 = float2( 0,0 );
				float2 texCoord81_g5 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g5 = ( ( (temp_output_11_0_g5).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g5);
				float2 panner19_g5 = ( ( _TimeParameters.x * (temp_output_11_0_g5).y ) * float2( 0,1 ) + texCoord81_g5);
				float2 appendResult24_g5 = (float2((panner18_g5).x , (panner19_g5).y));
				float2 temp_output_47_0_g5 = _Distortion_Speed;
				float2 texCoord78_g5 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g5 = ( texCoord78_g5 - float2( 1,1 ) );
				float2 appendResult39_g5 = (float2(frac( ( atan2( (temp_output_31_0_g5).x , (temp_output_31_0_g5).y ) / TWO_PI ) ) , length( temp_output_31_0_g5 )));
				float2 panner54_g5 = ( ( (temp_output_47_0_g5).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g5);
				float2 panner55_g5 = ( ( _TimeParameters.x * (temp_output_47_0_g5).y ) * float2( 0,1 ) + appendResult39_g5);
				float2 appendResult58_g5 = (float2((panner54_g5).x , (panner55_g5).y));
				float2 uv_TextureSample1 = input.ase_texcoord.xy * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
				float2 Distortion199 = ( (UnpackNormalScale( tex2D( _Normal_TEX, ( ( (tex2D( _Sampler60187, ( appendResult10_g5 + appendResult24_g5 ) )).rg * 1.0 ) + ( _Distortion_Tiling * appendResult58_g5 ) ) ), 1.0f )).xy * _Distortion * ( 1.0 - ( tex2D( _TextureSample1, uv_TextureSample1 ).r + 0.12 ) ) );
				float2 texCoord205 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_4_0_g7 = 2.0;
				float temp_output_5_0_g7 = 2.0;
				// *** BEGIN Flipbook UV Animation vars ***
				// Total tiles of Flipbook Texture
				float fbtotaltiles246_g7 = min( temp_output_4_0_g7 * temp_output_5_0_g7, ( ( temp_output_4_0_g7 * temp_output_5_0_g7 ) - 0.0 ) + 1 );
				// Offsets for cols and rows of Flipbook Texture
				float fbcolsoffset246_g7 = 1.0f / temp_output_4_0_g7;
				float fbrowsoffset246_g7 = 1.0f / temp_output_5_0_g7;
				// Speed of animation
				float fbspeed246_g7 = _TimeParameters.x * 0.0;
				// UV Tiling (col and row offset)
				float2 fbtiling246_g7 = float2(fbcolsoffset246_g7, fbrowsoffset246_g7);
				// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
				// Calculate current tile linear index
				float fbcurrenttileindex246_g7 = floor( fmod( fbspeed246_g7 + input.ase_texcoord1.x, fbtotaltiles246_g7) );
				fbcurrenttileindex246_g7 += ( fbcurrenttileindex246_g7 < 0) ? fbtotaltiles246_g7 : 0;
				// Obtain Offset X coordinate from current tile linear index
				float fblinearindextox246_g7 = round ( fmod ( fbcurrenttileindex246_g7, temp_output_4_0_g7 ) );
				// Multiply Offset X by coloffset
				float fboffsetx246_g7 = fblinearindextox246_g7 * fbcolsoffset246_g7;
				// Obtain Offset Y coordinate from current tile linear index
				float fblinearindextoy246_g7 = round( fmod( ( fbcurrenttileindex246_g7 - fblinearindextox246_g7 ) / temp_output_4_0_g7, temp_output_5_0_g7 ) );
				// Reverse Y to get tiles from Top to Bottom
				fblinearindextoy246_g7 = (int)(temp_output_5_0_g7-1) - fblinearindextoy246_g7;
				// Multiply Offset Y by rowoffset
				float fboffsety246_g7 = fblinearindextoy246_g7 * fbrowsoffset246_g7;
				// UV Offset
				float2 fboffset246_g7 = float2(fboffsetx246_g7, fboffsety246_g7);
				// Flipbook UV
				half2 fbuv246_g7 = texCoord205 * fbtiling246_g7 + fboffset246_g7;
				// *** END Flipbook UV Animation vars ***
				int flipbookFrame246_g7 = ( ( int )fbcurrenttileindex246_g7);
				float3 desaturateInitialColor228 = tex2D( _Main_TEX, ( Distortion199 + fbuv246_g7 ) ).rgb;
				float desaturateDot228 = dot( desaturateInitialColor228, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar228 = lerp( desaturateInitialColor228, desaturateDot228.xxx, _desaturate );
				float2 texCoord197 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1_0_g6 = float2( 1,1 );
				float2 texCoord80_g6 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g6 = (float2(( (temp_output_1_0_g6).x * texCoord80_g6.x ) , ( texCoord80_g6.y * (temp_output_1_0_g6).y )));
				float2 temp_output_11_0_g6 = float2( 0,0 );
				float2 texCoord81_g6 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g6 = ( ( (temp_output_11_0_g6).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g6);
				float2 panner19_g6 = ( ( _TimeParameters.x * (temp_output_11_0_g6).y ) * float2( 0,1 ) + texCoord81_g6);
				float2 appendResult24_g6 = (float2((panner18_g6).x , (panner19_g6).y));
				float2 temp_output_47_0_g6 = _Noise_Panner_Speed;
				float2 texCoord78_g6 = input.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g6 = ( texCoord78_g6 - float2( 1,1 ) );
				float2 appendResult39_g6 = (float2(frac( ( atan2( (temp_output_31_0_g6).x , (temp_output_31_0_g6).y ) / TWO_PI ) ) , length( temp_output_31_0_g6 )));
				float2 panner54_g6 = ( ( (temp_output_47_0_g6).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g6);
				float2 panner55_g6 = ( ( _TimeParameters.x * (temp_output_47_0_g6).y ) * float2( 0,1 ) + appendResult39_g6);
				float2 appendResult58_g6 = (float2((panner54_g6).x , (panner55_g6).y));
				float4 temp_cast_2 = (2.0).xxxx;
				float4 screenPos = input.ase_texcoord2;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float screenDepth233 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_positionSSNorm.xy ),_ZBufferParams);
				float distanceDepth233 = abs( ( screenDepth233 - LinearEyeDepth( ase_positionSSNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				

				surfaceDescription.Alpha = ( input.ase_color.a * saturate( ( float4( desaturateVar228 , 0.0 ) * ( ( ( 1.0 - ( length( ( texCoord197 + -0.5 ) ) * 1.0 ) ) + pow( tex2D( _Noise_T, ( Distortion199 + ( ( (tex2D( _Sampler60202, ( appendResult10_g6 + appendResult24_g6 ) )).rg * 1.0 ) + ( _Noise_Tiling * appendResult58_g6 ) ) ) ) , temp_cast_2 ) ) + (_Float7 + (input.ase_texcoord1.y - 0.0) * (1.0 - _Float7) / (1.0 - 0.0)) ) ) ) * saturate( distanceDepth233 ) ).r;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;

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
        	#define ASE_VERSION 19801
        	#define ASE_SRP_VERSION 140011
        	#define REQUIRE_DEPTH_TEXTURE 1


			

        	#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS

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

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            #if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_SCREEN_POSITION


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				float3 normalWS : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _TextureSample1_ST;
			float4 _Color;
			float2 _Distortion_Tiling;
			float2 _Distortion_Speed;
			float2 _Noise_Tiling;
			float2 _Noise_Panner_Speed;
			float _Distortion;
			float _desaturate;
			float _Main_power;
			float _Float7;
			float _DepthFade;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Main_TEX;
			sampler2D _Normal_TEX;
			sampler2D _Sampler60187;
			sampler2D _TextureSample1;
			sampler2D _Noise_T;
			sampler2D _Sampler60202;


			
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
				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				output.ase_texcoord4 = input.ase_texcoord1;
				
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

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				output.normalWS = TransformObjectToWorldNormal( input.normalOS );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
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
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
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
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );
				float3 WorldPosition = input.positionWS;
				float3 WorldNormal = input.normalWS;
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				float2 temp_output_1_0_g5 = float2( 1,1 );
				float2 texCoord80_g5 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g5 = (float2(( (temp_output_1_0_g5).x * texCoord80_g5.x ) , ( texCoord80_g5.y * (temp_output_1_0_g5).y )));
				float2 temp_output_11_0_g5 = float2( 0,0 );
				float2 texCoord81_g5 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g5 = ( ( (temp_output_11_0_g5).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g5);
				float2 panner19_g5 = ( ( _TimeParameters.x * (temp_output_11_0_g5).y ) * float2( 0,1 ) + texCoord81_g5);
				float2 appendResult24_g5 = (float2((panner18_g5).x , (panner19_g5).y));
				float2 temp_output_47_0_g5 = _Distortion_Speed;
				float2 texCoord78_g5 = input.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g5 = ( texCoord78_g5 - float2( 1,1 ) );
				float2 appendResult39_g5 = (float2(frac( ( atan2( (temp_output_31_0_g5).x , (temp_output_31_0_g5).y ) / TWO_PI ) ) , length( temp_output_31_0_g5 )));
				float2 panner54_g5 = ( ( (temp_output_47_0_g5).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g5);
				float2 panner55_g5 = ( ( _TimeParameters.x * (temp_output_47_0_g5).y ) * float2( 0,1 ) + appendResult39_g5);
				float2 appendResult58_g5 = (float2((panner54_g5).x , (panner55_g5).y));
				float2 uv_TextureSample1 = input.ase_texcoord3.xy * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
				float2 Distortion199 = ( (UnpackNormalScale( tex2D( _Normal_TEX, ( ( (tex2D( _Sampler60187, ( appendResult10_g5 + appendResult24_g5 ) )).rg * 1.0 ) + ( _Distortion_Tiling * appendResult58_g5 ) ) ), 1.0f )).xy * _Distortion * ( 1.0 - ( tex2D( _TextureSample1, uv_TextureSample1 ).r + 0.12 ) ) );
				float2 texCoord205 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_4_0_g7 = 2.0;
				float temp_output_5_0_g7 = 2.0;
				// *** BEGIN Flipbook UV Animation vars ***
				// Total tiles of Flipbook Texture
				float fbtotaltiles246_g7 = min( temp_output_4_0_g7 * temp_output_5_0_g7, ( ( temp_output_4_0_g7 * temp_output_5_0_g7 ) - 0.0 ) + 1 );
				// Offsets for cols and rows of Flipbook Texture
				float fbcolsoffset246_g7 = 1.0f / temp_output_4_0_g7;
				float fbrowsoffset246_g7 = 1.0f / temp_output_5_0_g7;
				// Speed of animation
				float fbspeed246_g7 = _TimeParameters.x * 0.0;
				// UV Tiling (col and row offset)
				float2 fbtiling246_g7 = float2(fbcolsoffset246_g7, fbrowsoffset246_g7);
				// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
				// Calculate current tile linear index
				float fbcurrenttileindex246_g7 = floor( fmod( fbspeed246_g7 + input.ase_texcoord4.x, fbtotaltiles246_g7) );
				fbcurrenttileindex246_g7 += ( fbcurrenttileindex246_g7 < 0) ? fbtotaltiles246_g7 : 0;
				// Obtain Offset X coordinate from current tile linear index
				float fblinearindextox246_g7 = round ( fmod ( fbcurrenttileindex246_g7, temp_output_4_0_g7 ) );
				// Multiply Offset X by coloffset
				float fboffsetx246_g7 = fblinearindextox246_g7 * fbcolsoffset246_g7;
				// Obtain Offset Y coordinate from current tile linear index
				float fblinearindextoy246_g7 = round( fmod( ( fbcurrenttileindex246_g7 - fblinearindextox246_g7 ) / temp_output_4_0_g7, temp_output_5_0_g7 ) );
				// Reverse Y to get tiles from Top to Bottom
				fblinearindextoy246_g7 = (int)(temp_output_5_0_g7-1) - fblinearindextoy246_g7;
				// Multiply Offset Y by rowoffset
				float fboffsety246_g7 = fblinearindextoy246_g7 * fbrowsoffset246_g7;
				// UV Offset
				float2 fboffset246_g7 = float2(fboffsetx246_g7, fboffsety246_g7);
				// Flipbook UV
				half2 fbuv246_g7 = texCoord205 * fbtiling246_g7 + fboffset246_g7;
				// *** END Flipbook UV Animation vars ***
				int flipbookFrame246_g7 = ( ( int )fbcurrenttileindex246_g7);
				float3 desaturateInitialColor228 = tex2D( _Main_TEX, ( Distortion199 + fbuv246_g7 ) ).rgb;
				float desaturateDot228 = dot( desaturateInitialColor228, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar228 = lerp( desaturateInitialColor228, desaturateDot228.xxx, _desaturate );
				float2 texCoord197 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1_0_g6 = float2( 1,1 );
				float2 texCoord80_g6 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g6 = (float2(( (temp_output_1_0_g6).x * texCoord80_g6.x ) , ( texCoord80_g6.y * (temp_output_1_0_g6).y )));
				float2 temp_output_11_0_g6 = float2( 0,0 );
				float2 texCoord81_g6 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g6 = ( ( (temp_output_11_0_g6).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g6);
				float2 panner19_g6 = ( ( _TimeParameters.x * (temp_output_11_0_g6).y ) * float2( 0,1 ) + texCoord81_g6);
				float2 appendResult24_g6 = (float2((panner18_g6).x , (panner19_g6).y));
				float2 temp_output_47_0_g6 = _Noise_Panner_Speed;
				float2 texCoord78_g6 = input.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g6 = ( texCoord78_g6 - float2( 1,1 ) );
				float2 appendResult39_g6 = (float2(frac( ( atan2( (temp_output_31_0_g6).x , (temp_output_31_0_g6).y ) / TWO_PI ) ) , length( temp_output_31_0_g6 )));
				float2 panner54_g6 = ( ( (temp_output_47_0_g6).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g6);
				float2 panner55_g6 = ( ( _TimeParameters.x * (temp_output_47_0_g6).y ) * float2( 0,1 ) + appendResult39_g6);
				float2 appendResult58_g6 = (float2((panner54_g6).x , (panner55_g6).y));
				float4 temp_cast_2 = (2.0).xxxx;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float screenDepth233 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_positionSSNorm.xy ),_ZBufferParams);
				float distanceDepth233 = abs( ( screenDepth233 - LinearEyeDepth( ase_positionSSNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				

				float Alpha = ( input.ase_color.a * saturate( ( float4( desaturateVar228 , 0.0 ) * ( ( ( 1.0 - ( length( ( texCoord197 + -0.5 ) ) * 1.0 ) ) + pow( tex2D( _Noise_T, ( Distortion199 + ( ( (tex2D( _Sampler60202, ( appendResult10_g6 + appendResult24_g6 ) )).rg * 1.0 ) + ( _Noise_Tiling * appendResult58_g6 ) ) ) ) , temp_cast_2 ) ) + (_Float7 + (input.ase_texcoord4.y - 0.0) * (1.0 - _Float7) / (1.0 - 0.0)) ) ) ) * saturate( distanceDepth233 ) ).r;
				float AlphaClipThreshold = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float3 normalWS = normalize(input.normalWS);
					float2 octNormalWS = PackNormalOctQuadEncode(normalWS);
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					float3 normalWS = input.normalWS;
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
Version=19801
Node;AmplifyShaderEditor.CommentaryNode;182;-3744,-1584;Inherit;False;2027.562;849.1772;Distortion;12;199;194;192;191;190;189;188;187;186;185;184;183;;0,1,0.004989147,1;0;0
Node;AmplifyShaderEditor.Vector2Node;183;-3680,-1104;Float;False;Property;_Distortion_Tiling;Distortion_Tiling;6;0;Create;True;0;0;0;False;0;False;2,0.5;2,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;184;-3696,-896;Float;False;Property;_Distortion_Speed;Distortion_Speed;5;0;Create;True;0;0;0;False;0;False;0,-1;0,-1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;185;-3184,-1536;Inherit;True;Property;_TextureSample1;Texture Sample 1;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;186;-3088,-1248;Float;False;Constant;_Float4;Float 4;6;0;Create;True;0;0;0;False;0;False;0.12;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;187;-3424,-1056;Inherit;True;RadialUVDistortion;-1;;5;051d65e7699b41a4c800363fd0e822b2;0;7;60;SAMPLER2D;_Sampler60187;False;1;FLOAT2;1,1;False;11;FLOAT2;0,0;False;65;FLOAT;1;False;68;FLOAT2;1,1;False;47;FLOAT2;1,1;False;29;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;188;-2784,-1408;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;189;-2912,-1040;Inherit;True;Property;_Normal_TEX;Normal_TEX;3;0;Create;True;0;0;0;False;0;False;-1;51fe2c9d5b236124d9f9e7ea528b0bea;51fe2c9d5b236124d9f9e7ea528b0bea;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.OneMinusNode;190;-2496,-1344;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;191;-2320,-832;Float;False;Property;_Distortion;Distortion;4;0;Create;True;0;0;0;False;0;False;0.5753834;0.028;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;192;-2528,-1056;Inherit;True;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;193;-3904,16;Inherit;False;2434.191;912.9056;Dissolve;21;225;223;221;220;218;217;216;215;214;211;210;208;207;204;203;202;200;198;197;196;195;;0.02595139,0,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;194;-2064,-1152;Inherit;True;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;195;-3840,720;Float;False;Property;_Noise_Panner_Speed;Noise_Panner_Speed;10;0;Create;True;0;0;0;False;0;False;0,-0.5;0,-0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;196;-3360,400;Float;False;Constant;_Float3;Float 3;9;0;Create;True;0;0;0;False;0;False;-0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;197;-3664,160;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;198;-3840,544;Float;False;Property;_Noise_Tiling;Noise_Tiling;9;0;Create;True;0;0;0;False;0;False;0.66,0.1;0.66,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-1936,-1312;Float;False;Distortion;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;200;-3184,160;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;201;-3472,-672;Inherit;False;1795.87;567.4167;Main;10;239;228;224;222;219;213;212;209;206;205;;1,0,0,1;0;0
Node;AmplifyShaderEditor.FunctionNode;202;-3584,544;Inherit;True;RadialUVDistortion;-1;;6;051d65e7699b41a4c800363fd0e822b2;0;7;60;SAMPLER2D;_Sampler60202;False;1;FLOAT2;1,1;False;11;FLOAT2;0,0;False;65;FLOAT;1;False;68;FLOAT2;1,1;False;47;FLOAT2;1,1;False;29;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-3152,480;Inherit;False;199;Distortion;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LengthOpNode;204;-2912,160;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;205;-3424,-608;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;206;-3328,-384;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;207;-2960,528;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;208;-2720,64;Float;False;Constant;_Float6;Float 6;9;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;209;-3296,-464;Float;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-2544,144;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;211;-2384,368;Float;False;Constant;_Noise_power;Noise_power;9;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;212;-2640,-544;Inherit;False;199;Distortion;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;213;-2848,-448;Inherit;True;Flipbook;-1;;7;53c2488c220f6564ca6c90721ee16673;3,68,0,217,0,244,0;11;51;SAMPLER2D;0.0;False;167;SAMPLERSTATE;0;False;13;FLOAT2;0,0;False;24;FLOAT;0;False;210;FLOAT;4;False;4;FLOAT;3;False;5;FLOAT;3;False;130;FLOAT;0;False;2;FLOAT;0;False;55;FLOAT;0;False;70;FLOAT;0;False;5;COLOR;53;FLOAT2;0;FLOAT;47;FLOAT;48;INT;218
Node;AmplifyShaderEditor.SamplerNode;214;-2544,480;Inherit;True;Property;_Noise_T;Noise_T;8;0;Create;True;0;0;0;False;0;False;-1;3bac350f5971ab843916135c9c326465;3bac350f5971ab843916135c9c326465;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TexCoordVertexDataNode;215;-2192,560;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;216;-2160,736;Float;False;Property;_Float7;Float 7;11;0;Create;True;0;0;0;False;0;False;-2;-2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;217;-2288,160;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;218;-2176,432;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;219;-2352,-480;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;220;-2144,832;Float;False;Constant;_Float8;Float 8;9;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;221;-1984,240;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;222;-1904,-256;Float;False;Property;_desaturate;desaturate;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;223;-1920,640;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;224;-2128,-496;Inherit;True;Property;_Main_TEX;Main_TEX;1;0;Create;True;0;0;0;False;0;False;-1;9b89e0cbe7918f04fbc88deb42fa0c3f;9b89e0cbe7918f04fbc88deb42fa0c3f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleAddOpNode;225;-1728,400;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;227;-1216,448;Float;False;Property;_DepthFade;Depth Fade;12;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;228;-1680,-432;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;231;-1120,160;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;233;-896,416;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;235;-832,64;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;236;-624,432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;238;-768,288;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;226;-1616,-320;Float;False;Property;_Main_power;Main_power;2;1;[HDR];Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;229;-1440,-464;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;230;-1376,-320;Float;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TexCoordVertexDataNode;232;-960,-544;Inherit;False;3;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;234;-1152,-464;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;237;-688,-448;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;239;-3280,-192;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;-448,-128;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;241;-3824,-512;Float;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;0;0;0;3.9999;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;242;-496,176;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;111;48,-112;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;113;48,-112;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;114;48,-112;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;True;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;115;48,-112;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;116;48,-112;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;117;48,-112;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;118;48,-112;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;119;48,-112;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;120;48,-112;Float;False;False;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;112;-16,-64;Float;False;True;-1;3;UnityEditor.ShaderGraphUnlitGUI;0;13;Amplify Shader/SGB/URP/FX_Smoke_Des;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;9;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;True;True;2;5;False;;10;False;;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;25;Surface;1;638796154315452985;  Blend;0;638796156417235116;Two Sided;1;638796160933903893;Alpha Clipping;0;638796160400726309;  Use Shadow Threshold;0;0;Forward Only;0;0;Cast Shadows;0;638796154376265655;Receive Shadows;0;638796160353476927;GPU Instancing;0;638796154381061795;LOD CrossFade;0;638796154383250567;Built-in Fog;0;638796154385173787;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;False;True;False;False;True;True;True;False;False;;False;0
WireConnection;187;68;183;0
WireConnection;187;47;184;0
WireConnection;188;0;185;1
WireConnection;188;1;186;0
WireConnection;189;1;187;0
WireConnection;190;0;188;0
WireConnection;192;0;189;0
WireConnection;194;0;192;0
WireConnection;194;1;191;0
WireConnection;194;2;190;0
WireConnection;199;0;194;0
WireConnection;200;0;197;0
WireConnection;200;1;196;0
WireConnection;202;68;198;0
WireConnection;202;47;195;0
WireConnection;204;0;200;0
WireConnection;207;0;203;0
WireConnection;207;1;202;0
WireConnection;210;0;204;0
WireConnection;210;1;208;0
WireConnection;213;13;205;0
WireConnection;213;24;206;1
WireConnection;213;4;209;0
WireConnection;213;5;209;0
WireConnection;214;1;207;0
WireConnection;217;0;210;0
WireConnection;218;0;214;0
WireConnection;218;1;211;0
WireConnection;219;0;212;0
WireConnection;219;1;213;0
WireConnection;221;0;217;0
WireConnection;221;1;218;0
WireConnection;223;0;215;2
WireConnection;223;3;216;0
WireConnection;223;4;220;0
WireConnection;224;1;219;0
WireConnection;225;0;221;0
WireConnection;225;1;223;0
WireConnection;228;0;224;0
WireConnection;228;1;222;0
WireConnection;231;0;228;0
WireConnection;231;1;225;0
WireConnection;233;0;227;0
WireConnection;236;0;233;0
WireConnection;238;0;231;0
WireConnection;229;0;228;0
WireConnection;229;1;226;0
WireConnection;234;0;229;0
WireConnection;234;1;230;0
WireConnection;237;0;232;1
WireConnection;237;1;232;2
WireConnection;237;2;232;3
WireConnection;237;3;232;4
WireConnection;240;0;234;0
WireConnection;240;1;235;0
WireConnection;240;2;237;0
WireConnection;242;0;235;4
WireConnection;242;1;238;0
WireConnection;242;2;236;0
WireConnection;112;2;240;0
WireConnection;112;3;242;0
ASEEND*/
//CHKSM=1FA8DF5E3EF68C17C883B8151033C749986EB67C