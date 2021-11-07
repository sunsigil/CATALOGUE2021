Shader "Unlit/Tester"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
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
                float4 col : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 col : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;

            float random (float2 uv)
            {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.col = v.col;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= i.col;

                uint resolution = 64;

                float x_f = i.uv.x * resolution;
                float y_f = i.uv.y * resolution;
                uint x_i = int(x_f);
                uint y_i = int(y_f);

                col.a *= (x_i ^ y_i) % 3 != 0;

                float rand = clamp(random(float2(x_i, y_i)) * 0.5, 0.3, 5);

                float x_c = x_i + 0.5;
                float y_c = y_i + 0.5;

                float r = length(float2(x_f - x_c, y_f - y_c));

                col.a *= r < rand;

                return col;
            }
            ENDCG
        }
    }
}
