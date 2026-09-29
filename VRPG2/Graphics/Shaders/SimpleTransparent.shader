Shader "GIB/SimpleTransparent"
{
    SubShader
    {
        Pass
        {
            ZWrite Off
            ColorMask 0
        }
    }

    Fallback Off
}