Shader "Custom/RingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Ring texture
        _Color ("Tint", Color) = (1,1,1,1) // Enable color changes
        _InnerRadius ("Inner Radius (%)", Range(0, 100)) = 85 // Inner transparent part (as percentage)
        _OuterCutoff ("Outer Cutoff (%)", Range(0, 100)) = 95 // Outer transparent part (as percentage)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // Enable transparency

        Cull Off // ✅ Renders both front and back faces
        ZWrite On // ✅ Ensures correct depth ordering
        ZTest LEqual // ✅ Helps with transparency depth sorting

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
            fixed4 _Color; // Add color support
            float _InnerRadius; // Inner transparent cut (in percentage)
            float _OuterCutoff; // Outer transparent cut (in percentage)

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5); // Center of the texture
                float dist = distance(i.uv, center); // Distance from center (0 to 0.5 in UV space)

                // Convert percentage (0 to 100) into UV range (0 to 0.5)
                float normalizedInner = (_InnerRadius / 100.0) * 0.5;
                float normalizedOuter = (_OuterCutoff / 100.0) * 0.5;

                fixed4 color = tex2D(_MainTex, i.uv);
                color *= _Color; // Apply color tinting while keeping texture details

                // Make everything inside the InnerRadius or outside OuterCutoff transparent
                if (dist < normalizedInner || dist > normalizedOuter)
                {
                    discard; // Removes pixels inside the hollow area & outer cutoff
                }

                return color;
            }
            ENDCG
        }
    }
}