Shader "Unlit/GrayScaleUI"
{
    Properties
    {
        [Toggle] _IsOn ("Is On?", Float) = 0
        _Strength ("Gray Strength", Float) = 1
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata 
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f 
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _IsOn;
            float _Strength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                clip(col.a - 0.001f);
                
                float gray = dot(col.rgb, float3(0.3, 0.59, 0.11)) / _Strength;
                col.rgb = lerp(col.rgb, gray.xxx, _IsOn);

                return col;
            }
            ENDCG
        }
    }
}
