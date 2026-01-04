Shader "Custom/ColorShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1) // ✅ Add a color property
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off  // ✅ Ensures backfaces are visible
        ZWrite Off // ✅ Prevents depth issues
        Blend SrcAlpha OneMinusSrcAlpha // ✅ Enables transparency

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color; // ✅ Color variable

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color; // ✅ Return custom color
            }
            ENDCG
        }
    }
}