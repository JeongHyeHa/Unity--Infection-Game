Shader "Custom/MaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskRect ("Mask Rectangle", Vector) = (0.5, 0.5, 0.2, 0.2) // x, y, width, height
        _OverlayColor ("Overlay Color", Color) = (0, 0, 0, 0.5) // 반투명 검정색
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            float4 _MaskRect;
            fixed4 _OverlayColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Mask Rect handling
                float2 uv = i.uv;
                float2 center = _MaskRect.xy;
                float2 halfSize = _MaskRect.zw * 0.5;

                if (uv.x > (center.x - halfSize.x) && uv.x < (center.x + halfSize.x) &&
                    uv.y > (center.y - halfSize.y) && uv.y < (center.y + halfSize.y))
                {
                    // 투명하게 처리
                    col.a = 0.0;
                    return col;
                }

                // 반투명 검은색으로 처리
                return _OverlayColor;
            }
            ENDCG
        }
    }
}
