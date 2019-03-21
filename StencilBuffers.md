Stencil Buffers
===============

According to the Unity documentation (https://docs.unity3d.com/Manual/SL-Stencil.html)

    The stencil buffer is usually an 8 bit integer per pixel. The value can be written to, increment or decremented. Subsequent draw calls can test against the value, to decide if a pixel should be discarded before running the pixel shader.


Reading from the Stencil buffer

Writing to the Stencil Buffer
    Writing to the stencil buffers is done by defining the Pass as Replace the value in the stencil buffer is replaced with the value of the Ref parameter


Order of execution