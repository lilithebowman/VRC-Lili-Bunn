// Made with Amplify Shader Editor v1.9.9.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hidden/PJKT/CardMakerFEST26"
{
	Properties
	{
		_MainTex( "MainTex", 2D ) = "white" {}
		_NamePic( "NamePic", 2D ) = "black" {}
		_TitlePic( "TitlePic", 2D ) = "black" {}
		_ProfileIconMask3( "ProfileIconMask", 2D ) = "white" {}
		_ProfileDesc( "ProfileDesc", 2D ) = "black" {}
		_TitlePos( "TitlePos", Vector ) = ( 5, 5, -0.6, -0.6 )
		_NamePos( "NamePos", Vector ) = ( 1, 1, 0, 0 )
		_DescPosition( "DescPosition", Vector ) = ( 5, 5, -0.6, -0.6 )
		_ProfileMaksPos( "ProfileMaksPos", Vector ) = ( 3.291, 3.291, -0.17, -0.1375 )
		_ProfilePos( "ProfilePos", Vector ) = ( 3.291, 3.291, -0.17, -0.1375 )
		_Sticker0( "Sticker0", 2D ) = "black" {}
		_ProfilePic( "ProfilePic", 2D ) = "black" {}
		_stickerPos0( "stickerPos0", Vector ) = ( 5, 5, -0.6, -0.6 )
		_stickerRotation0( "stickerRotation0", Float ) = 0
		_Sticker1( "Sticker1", 2D ) = "black" {}
		_StickerPos1( "StickerPos1", Vector ) = ( 5, 5, -0.6, -0.6 )
		_stickerRotation1( "stickerRotation1", Float ) = 0
		_Sticker2( "Sticker2", 2D ) = "black" {}
		_StickerPos2( "StickerPos2", Vector ) = ( 5, 5, -0.6, -0.6 )
		_stickerRotation2( "stickerRotation2", Float ) = 0
		_Sticker3( "Sticker3", 2D ) = "black" {}
		_StickerPos3( "StickerPos3", Vector ) = ( 5, 5, -0.6, -0.6 )
		_stickerRotation3( "stickerRotation3", Float ) = 0
		_Sticker4( "Sticker4", 2D ) = "black" {}
		_StickerPos4( "StickerPos4", Vector ) = ( 5, 5, -0.6, -0.6 )
		_stickerRotation4( "stickerRotation4", Float ) = 0
		_Sticker5( "Sticker5", 2D ) = "black" {}
		_StickerPos5( "StickerPos5", Vector ) = ( 5, 5, -0.6, -0.6 )
		_stickerRotation5( "stickerRotation5", Float ) = 0
		_RotationAnchorOffest10( "Rotation Anchor Offest", Vector ) = ( 0.5, 0.5, 0, 0 )
		_RotationAnchorOffest6( "Rotation Anchor Offest", Vector ) = ( 0.5, 0.5, 0, 0 )
		_RotationAnchorOffest9( "Rotation Anchor Offest", Vector ) = ( 0.5, 0.5, 0, 0 )
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		

		Tags { "RenderType"="Opaque" }

	LOD 100

		

		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		

		CGINCLUDE
			#pragma target 3.5

			float4 ComputeClipSpacePosition( float2 screenPosNorm, float deviceDepth )
			{
				float4 positionCS = float4( screenPosNorm * 2.0 - 1.0, deviceDepth, 1.0 );
			#if UNITY_UV_STARTS_AT_TOP
				positionCS.y = -positionCS.y;
			#endif
				return positionCS;
			}
		ENDCG

		
		Pass
		{
			Name "Unlit"

			CGPROGRAM
				#define ASE_VERSION 19903

				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
					#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing
				#include "UnityCG.cginc"

				#define ASE_NEEDS_TEXTURE_COORDINATES0
				#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0


				struct appdata
				{
					float4 vertex : POSITION;
					float4 ase_texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float4 ase_texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
				};

				uniform float4 _StickerPos5;
				uniform float4 _StickerPos3;
				uniform float4 _StickerPos2;
				uniform float4 _StickerPos1;
				uniform float4 _StickerPos4;
				uniform float _stickerRotation1;
				uniform float _stickerRotation2;
				uniform float _stickerRotation5;
				uniform float _stickerRotation4;
				uniform float _stickerRotation3;
				uniform float4 _stickerPos0;
				uniform float _stickerRotation0;
				uniform sampler2D _Sticker3;
				uniform sampler2D _Sticker4;
				uniform sampler2D _Sticker5;
				uniform sampler2D _Sticker1;
				uniform sampler2D _Sticker2;
				uniform float4 _DescPosition;
				uniform float4 _TitlePos;
				uniform sampler2D _TitlePic;
				uniform sampler2D _ProfileDesc;
				uniform sampler2D _Sticker0;
				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform float4 _NamePos;
				uniform float2 _RotationAnchorOffest9;
				uniform sampler2D _NamePic;
				uniform float4 _ProfilePos;
				uniform float2 _RotationAnchorOffest6;
				uniform sampler2D _ProfilePic;
				uniform sampler2D _ProfileIconMask3;
				uniform float4 _ProfileMaksPos;
				uniform float2 _RotationAnchorOffest10;


				
				v2f vert ( appdata v )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID( v );
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
					UNITY_TRANSFER_INSTANCE_ID( v, o );

					o.ase_texcoord.xy = v.ase_texcoord.xy;
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord.zw = 0;

					float3 vertexValue = float3( 0, 0, 0 );
					#if ASE_ABSOLUTE_VERTEX_POS
						vertexValue = v.vertex.xyz;
					#endif
					vertexValue = vertexValue;
					#if ASE_ABSOLUTE_VERTEX_POS
						v.vertex.xyz = vertexValue;
					#else
						v.vertex.xyz += vertexValue;
					#endif

					o.pos = UnityObjectToClipPos( v.vertex );
					return o;
				}

				half4 frag( v2f IN  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( IN );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
					half4 finalColor;

					float4 ScreenPosNorm = float4( IN.pos.xy * ( _ScreenParams.zw - 1.0 ), IN.pos.zw );
					float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, IN.pos.z ) * IN.pos.w;
					float4 ScreenPos = ComputeScreenPos( ClipPos );

					float2 uv_MainTex = IN.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					float4 break350 = _NamePos;
					float2 appendResult351 = (float2(break350.x , break350.y));
					float4 break356 = _NamePos;
					float2 appendResult357 = (float2(break356.z , break356.w));
					float cos358 = cos( 0.0 );
					float sin358 = sin( 0.0 );
					float2 rotator358 = mul( (IN.ase_texcoord.xy*appendResult351 + appendResult357) - _RotationAnchorOffest9 , float2x2( cos358 , -sin358 , sin358 , cos358 )) + _RotationAnchorOffest9;
					float4 temp_output_359_0 = ( ( 1.0 - (( ( float2 )( any( floor( rotator358 ) ) ? 1 : 0 ) )).xyyy ) * tex2D( _NamePic, rotator358 ) );
					float4 lerpResult363 = lerp( tex2D( _MainTex, uv_MainTex ) , temp_output_359_0 , temp_output_359_0.w);
					float4 break297 = _ProfilePos;
					float2 appendResult298 = (float2(break297.x , break297.y));
					float4 break301 = _ProfilePos;
					float2 appendResult302 = (float2(break301.z , break301.w));
					float cos303 = cos( 0.0 );
					float sin303 = sin( 0.0 );
					float2 rotator303 = mul( (IN.ase_texcoord.xy*appendResult298 + appendResult302) - _RotationAnchorOffest6 , float2x2( cos303 , -sin303 , sin303 , cos303 )) + _RotationAnchorOffest6;
					float4 temp_output_306_0 = ( ( 1.0 - (( ( float2 )( any( floor( rotator303 ) ) ? 1 : 0 ) )).xyyy ) * tex2D( _ProfilePic, rotator303 ) );
					float4 break404 = _ProfileMaksPos;
					float2 appendResult405 = (float2(break404.x , break404.y));
					float4 break408 = _ProfileMaksPos;
					float2 appendResult409 = (float2(break408.z , break408.w));
					float cos410 = cos( 0.0 );
					float sin410 = sin( 0.0 );
					float2 rotator410 = mul( (IN.ase_texcoord.xy*appendResult405 + appendResult409) - _RotationAnchorOffest10 , float2x2( cos410 , -sin410 , sin410 , cos410 )) + _RotationAnchorOffest10;
					float4 lerpResult308 = lerp( lerpResult363 , temp_output_306_0 , ( temp_output_306_0 * tex2D( _ProfileIconMask3, rotator410 ) ).w);
					

					finalColor = lerpResult308;

					return finalColor;
				}
			ENDCG
		}
	}
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
	
	Fallback Off
}