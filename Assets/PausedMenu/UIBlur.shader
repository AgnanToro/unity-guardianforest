Shader "UI/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 10)) = 3
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = fixed4(0,0,0,0);

                col += tex2D(_MainTex, i.uv + float2(-1, -1) * _MainTex_TexelSize.xy * _BlurSize);
                col += tex2D(_MainTex, i.uv + float2( 0, -1) * _MainTex_TexelSize.xy * _BlurSize);
                col += tex2D(_MainTex, i.uv + float2( 1, -1) * _MainTex_TexelSize.xy * _BlurSize);

                col += tex2D(_MainTex, i.uv + float2(-1,  0) * _MainTex_TexelSize.xy * _BlurSize);
                col += tex2D(_MainTex, i.uv);
                col += tex2D(_MainTex, i.uv + float2( 1,  0) * _MainTex_TexelSize.xy * _BlurSize);

                col += tex2D(_MainTex, i.uv + float2(-1,  1) * _MainTex_TexelSize.xy * _BlurSize);
                col += tex2D(_MainTex, i.uv + float2( 0,  1) * _MainTex_TexelSize.xy * _BlurSize);
                col += tex2D(_MainTex, i.uv + float2( 1,  1) * _MainTex_TexelSize.xy * _BlurSize);

                col /= 9;
                col.a = 0.75;

                return col;
            }
            ENDCG
        }
    }
}