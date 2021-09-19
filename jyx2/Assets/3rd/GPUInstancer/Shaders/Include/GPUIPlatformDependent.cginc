#ifndef GPU_INSTANCER_PLATFORM_DEPENDENT_INCLUDED
#define GPU_INSTANCER_PLATFORM_DEPENDENT_INCLUDED

#if SHADER_API_GLES3
    #define GPUI_MHT_COPY_TEXTURE 1
    #define gpui_InstanceID 0
#elif SHADER_API_VULKAN
    #define GPUI_MHT_MATRIX_APPEND 1
    #define gpui_InstanceID 0
#else
    #define gpui_InstanceID gpuiTransformationMatrix[unity_InstanceID]
#endif

#endif // GPU_INSTANCER_PLATFORM_DEPENDENT_INCLUDED