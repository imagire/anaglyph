Shader "Mine/MonotoneShader"
{
    Properties
    {
        _MainTexL("Left Texture", 2D) = "white" {}
        _MainTexR("Right Texture", 2D) = "white" {}
        _ColorL("Left Color", Color) = (1.0, 0.0, 0.0)
        _ColorR("Right Color", Color) = (0.0, 0.0, 1.0)
        _Threshold("Threshold", Range(0.0, 1.0)) = 0.4
        _Tension("Tension", Range(0.0, 100.0)) = 30.0
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            #include "UnityCustomRenderTexture.cginc"

            // #define ANAGLYPH_TYPE_MONO 1
            #define ANAGLYPH_TYPE_COLOR 1

            sampler2D _MainTexL;
            sampler2D _MainTexR;
            float4 _MainTexL_ST;
            float4 _MainTexR_ST;
            float4 _ColorL;
            float4 _ColorR;
            float _Threshold;
            float _Tension;

            float GetMonoColor(sampler2D tex, float2 uv)
            {
                fixed4 col = tex2D(tex, uv);

                float lum = 0.2126 * col.r + 0.7152 * col.g + 0.0722 * col.b;// ITU BT.709
                lum = 1.0f / (1.0f - exp(-_Tension * (lum - _Threshold)));// シグモイド関数で0-1を補正

                return 1.0 - lum;
            }

            fixed4 frag (v2f_customrendertexture i) : SV_Target
            {
                float2 uv = i.globalTexcoord;
#ifdef ANAGLYPH_TYPE_MONO
                float L = GetMonoColor(_MainTexL, uv);
                float R = GetMonoColor(_MainTexR, uv);
                float3 col = float3(1,1,1) - _ColorL.rgb * L - _ColorR.rgb * R;
#elif ANAGLYPH_TYPE_COLOR
                float4 colL = tex2D(_MainTexL, uv);
                float4 colR = tex2D(_MainTexR, uv);
                float3 col = _ColorL.rgb * colL.rgb + _ColorR.rgb * colR.rgb;
#endif

                return fixed4(col, 1.0f);
            }
            ENDCG
        }
    }
}
