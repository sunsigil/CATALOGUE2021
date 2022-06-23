Shader "Unlit/Whiteout"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Dither ("Dither", 2D) = "white" {}
        _White ("White", Color) = (1,1,1,1)
        _Black ("Black", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
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
                float4 screen_pos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
            sampler2D _Dither;
            float4 _Dither_TexelSize;
            float4 _White;
            float4 _Black;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                if(col.r < 0.5)
                {
                    col *= tex2D(_Dither, i.uv / (_MainTex_TexelSize * 32));
                }
                else
                {
                    col.a *= tex2D(_Dither, i.uv / (_MainTex_TexelSize * 64));
                }

                return col;
            }
            ENDCG
        }
    }
}
