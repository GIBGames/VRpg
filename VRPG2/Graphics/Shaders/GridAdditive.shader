Shader "GIB/GridAdditive" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _ScaleX ("Scale X", Float) = 1.0
        _ScaleY ("Scale Y", Float) = 1.0
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha One // Additive blending
        ZWrite Off
        Cull Off
        Lighting Off
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _ScaleX;
            float _ScaleY;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                float2 worldUV = float2(i.worldPos.x * _ScaleX, i.worldPos.z * _ScaleY);
                float4 texColor = tex2D(_MainTex, worldUV);
                return texColor * _Color;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
