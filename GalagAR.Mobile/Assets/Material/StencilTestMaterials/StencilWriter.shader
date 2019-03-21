Shader "Custom/StencilWriter" 
{
	SubShader 
    {
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }
            
        Pass
        {
            Blend Zero One
            ZWrite Off
        }
	}
}
