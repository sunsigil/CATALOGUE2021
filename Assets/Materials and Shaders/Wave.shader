Shader "Unlit/Wave"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Thickness ("Thickness", Float) = 0.1
        _Progress ("Progress", Float) = 0.5
        _Arc ("Arc", Float) = 3.1416
        _Phase ("Phase", Float) = 1.5707
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
            float _Thickness;
            float _Progress;
            float _Arc;
            float _Phase;

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

                float2 center = float2(0.5, 0.5);
                float2 spoke = i.uv - center;
                float dist = length(spoke);

                // distance from center to edge is 0.5, we want a "unit ring"
                // therefore normalize radii to 0.5 = 1
                col.a *= (dist * 2) >= (_Progress - _Thickness);
                col.a *= (dist * 2) <= _Progress;

                // clamp within arc
                float min = _Phase - _Arc/2;
                float max = _Phase + _Arc/2;
                col.a *= atan2(spoke.y, spoke.x) >= min;
                col.a *= atan2(spoke.y, spoke.x) <= max;

                return col;
            }
            ENDCG
        }
    }
}
