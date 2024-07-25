Shader "Unlit/PageFlip"
{
   Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlipProgress ("Flip Progress", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
LOD100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata_t
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
float _FlipProgress;

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);

    float flipAmount = saturate(_FlipProgress);
    float offset = flipAmount * 3.14159; // Pi for half-circle flip effect

    o.uv = v.uv;
    o.uv.x = (v.uv.x - 0.5) * cos(offset) + 0.5; // Horizontal flip effect
    o.uv.y = (v.uv.y - 0.5) * sin(offset) + 0.5; // Vertical flip effect
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv);
    return col;
}
            ENDCG
        }
    }
FallBack"Unlit/Texture"
}
