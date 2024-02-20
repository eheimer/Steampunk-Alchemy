Shader "Custom/BackgroundRadialShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Center ("Center", Vector) = (0.5,0.5,0,0)
        _Radius ("Radius", Range(0,1)) = 0.5
        _Opacity ("Opacity", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            float4 _Color;
            float2 _Center;
            float _Radius;
            float _Opacity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.uv, _Center);
                float t = smoothstep(_Radius, _Radius - 0.1, dist);
                fixed4 col = _Color;
                col.a *= t * _Opacity;
                return col;
            }
            ENDCG
        }
    }
}