Shader "Unlit/CAOutput"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                const fixed2 gRes = fixed2(64, 96);
                fixed2 uv = i.uv;
                fixed2 gst = frac(uv * gRes);
                fixed d = length(gst - .5);
                d = smoothstep(d - .01, d + .1, .45);
                fixed4 pix = tex2D(_MainTex, i.uv);
                fixed val = pix.r;
                fixed4 col = fixed4(
                    val * d * gst.x, 
                    val * d * (gst.x + gst.y),
                    val * d * gst.y, 1
                );
                return col * (1 - sign(pix.g));
            }
            ENDCG
        }
    }
}
