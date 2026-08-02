Shader "ShaderMask/SimpleMask"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _MaskColor ("Mask Color", Color) = (1,1,1,1)
        _ColorTolerance ("Color Tolerance", Range(0,1)) = 0.05
        [Toggle]_InvertMask ("Invert Mask", Float) = 0

        _StencilMask ("Mask Layer", Range(0,255)) = 1
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        //----------------------------------------------------------------------
        // PASS 1 — записывает в stencil только выбранный цвет
        //----------------------------------------------------------------------
        Pass
        {
            Name "StencilWrite"
            ColorMask 0 // не рисуем в цвет, только stencil

            Stencil
            {
                Ref 255
                WriteMask [_StencilMask]
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _MaskColor;
            float _ColorTolerance;
            float _InvertMask;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                float diff = distance(col.rgb, _MaskColor.rgb);
                bool isMask = diff <= _ColorTolerance;

                if (_InvertMask > 0.5) isMask = !isMask;
                if (!isMask) discard;

                return fixed4(0,0,0,0); // исправлено!
            }
            ENDCG
        }

        //----------------------------------------------------------------------
        // PASS 2 — просто рисует изображение без изменения stencil
        //----------------------------------------------------------------------
        Pass
        {
            Name "Visual"
            Stencil
            {
                Ref 255
                Comp Always
                Pass Keep
                WriteMask 0
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag2
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag2(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * i.color;
            }
            ENDCG
        }
    }
}
