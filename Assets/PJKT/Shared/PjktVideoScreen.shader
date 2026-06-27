Shader "PJKT/Video Screen"
{
    Properties
    {
        [MainTexture] _MainTex ("Standby / Fallback Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue"="Geometry" 
            "IgnoreProjector"="True"
            "VRCFallback"="Unlit"
        }
        
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _Udon_VideoTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 standby = tex2D(_MainTex, i.uv);
                fixed4 video   = tex2D(_Udon_VideoTex, i.uv);

                // With no video bound the global samples as a flat
                float mx = max(video.r, max(video.g, video.b));
                float mn = min(video.r, min(video.g, video.b));
                bool neutral = (mx - mn) < 0.01;
                if (neutral && (mx < 0.003 || abs(mx - 0.2159) < 0.02 || abs(mx - 0.5) < 0.02))
                    return standby;

                return video;
            }
            ENDCG
        }
    }
    
    Fallback "Unlit/Texture"
}