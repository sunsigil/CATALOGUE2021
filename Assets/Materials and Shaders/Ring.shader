Shader "Unlit/Ring"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Offset ("Offset", Vector) = (0,0,0,0)
        _Inner_Radius ("Inner Radius", Float) = 0.25
        _Outer_Radius ("Outer Radius", Float) = 0.5
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
            float4 _Color;
            float2 _Offset;
            float _Inner_Radius;
            float _Outer_Radius;

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
                col *= _Color;

                float tau = UNITY_PI * 2;

                float2 center = float2(0.5, 0.5) + _Offset;
                float2 spoke = i.uv - center;
                float dist = length(spoke);

                // distance from center to edge is 0.5, we want a "unit ring"
                // therefore normalize radii to 0.5 = 1
                col.a *= (dist * 2) >= _Inner_Radius;
                col.a *= (dist * 2) <= _Outer_Radius;

                return col;
            }
            ENDCG
        }
    }
}
