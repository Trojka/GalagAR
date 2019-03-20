Shader "Custom/StencilWriter" 
{
	SubShader 
    {
        Pass
        {
            Blend Zero One
            ZWrite Off
        }
	}
}
