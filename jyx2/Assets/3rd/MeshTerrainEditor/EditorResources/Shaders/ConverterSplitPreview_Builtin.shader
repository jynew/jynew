// UNITY_SHADER_NO_UPGRADE
//Mesh Terain Editor's terrain split preview shader for Builtin Pipeline
//http://u3d.as/oWB
//This is a modified shader from https://github.com/ColinLeung-NiloCat/UnityURPUnlitScreenSpaceDecalShader

Shader "Hidden/MTE/ConverterSplitPreview/Builtin"
{
    Properties
    {
        [Header(MTE Parameters)]
        _SplitNumber("Split Number", int) = 2
        _NormalizedLineWidth("Normalized Line Width", float) = 0.01

        [Header(Basic)]
        _MainTex("Texture", 2D) = "white" {}
        [HDR]_Color("_Color (default = 1,1,1,1)", color) = (1,1,1,1)

        [Header(Blending)]
        //https://docs.unity3d.com/ScriptReference/Rendering.BlendMode.html
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("_SrcBlend (default = One)", Float) = 1 //1 = One
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("_DstBlend (default = One)", Float) = 1 //1 = One

        [Header(Alpha remap(extra alpha control))]
        _AlphaRemap("_AlphaRemap (default = 1,0,0,0) _____alpha will first mul x, then add y    (zw unused)", vector) = (1,0,0,0)

        [Header(Prevent Side Stretching(Compare projection direction with scene normal and Discard if needed))]
        [Toggle(_ProjectionAngleDiscardEnable)] _ProjectionAngleDiscardEnable("_ProjectionAngleDiscardEnable (default = off)", float) = 0
        _ProjectionAngleDiscardThreshold("_ProjectionAngleDiscardThreshold (default = 0)", range(-1,1)) = 0

        [Header(Mul alpha to rgb)]
        [Toggle]_MulAlphaToRGB("_MulAlphaToRGB (default = off)", Float) = 0

        [Header(Ignore texture wrap mode setting)]
        [Toggle(_FracUVEnable)] _FracUVEnable("_FracUVEnable (default = off)", Float) = 0

        //====================================== below = usually can ignore in normal use case =====================================================================
        [Header(Stencil Masking)]
        //https://docs.unity3d.com/ScriptReference/Rendering.CompareFunction.html
        _StencilRef("_StencilRef", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("_StencilComp (default = Disable) _____Set to NotEqual if you want to mask by specific _StencilRef value, else set to Disable", Float) = 0 //0 = disable

        [Header(ZTest)]
        //https://docs.unity3d.com/ScriptReference/Rendering.CompareFunction.html
        //default need to be Disable, because we need to make sure decal render correctly even if camera goes into decal cube volume, although disable ZTest by default will prevent EarlyZ (bad for GPU performance)
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("_ZTest (default = Disable) _____to improve GPU performance, Set to LessEqual if camera never goes into cube volume, else set to Disable", Float) = 0 //0 = disable

        [Header(Cull)]
        //https://docs.unity3d.com/ScriptReference/Rendering.CullMode.html
        //default need to be Front, because we need to make sure decal render correctly even if camera goes into decal cube
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("_Cull (default = Front) _____to improve GPU performance, Set to Back if camera never goes into cube volume, else set to Front", Float) = 1 //1 = Front

    }

    SubShader
    {
        //To avoid render order problems, Queue must >= 2501, which enters the transparent queue, in transparent queue Unity will always draw from back to front
        //https://github.com/ColinLeung-NiloCat/UnityURPUnlitScreenSpaceDecalShader/issues/6#issuecomment-615940985
        /*
        //https://docs.unity3d.com/Manual/SL-SubShaderTags.html
        Queues up to 2500 (“Geometry+500”) are consided “opaque” and optimize the drawing order of the objects for best performance. 
        Higher rendering queues are considered for “transparent objects” and sort objects by distance, starting rendering from the furthest ones and ending with the closest ones. 
        Skyboxes are drawn in between all opaque and all transparent objects.
        */
        Tags { "RenderType" = "Overlay" "Queue" = "Transparent-499" }

        Pass
        {
            Stencil
            {
                Ref[_StencilRef]
                Comp[_StencilComp]
            }

            Cull[_Cull]
            ZTest[_ZTest]

            ZWrite off
            Blend[_SrcBlend][_DstBlend]

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            // make fog work
            #pragma multi_compile_fog

            //due to using ddx() & ddy()
            #pragma target 3.0

            #pragma shader_feature_local _ProjectionAngleDiscardEnable
            #pragma shader_feature_local _UnityFogEnable
            #pragma shader_feature_local _FracUVEnable

            // Required by all Builtin pipeline shaders.
            // It will include Unity built-in shader variables (except the lighting variables)
            // (https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
            // It will also include many utilitary functions. 
            #include "UnityCG.cginc"

            // Return the PreTranslated ObjectToWorld Matrix (i.e matrix with _WorldSpaceCameraPos apply to it if we use camera relative rendering)
            float4x4 GetObjectToWorldMatrix()
            {
                return UNITY_MATRIX_M;
            }
            float4x4 GetWorldToViewMatrix()
            {
                return UNITY_MATRIX_V;
            }
            // Transform to homogenous clip space
            float4x4 GetWorldToHClipMatrix()
            {
                return UNITY_MATRIX_VP;
            }
            float3 TransformObjectToWorld(float3 positionOS)
            {
                return mul(GetObjectToWorldMatrix(), float4(positionOS, 1.0)).xyz;
            }
            // Transforms position from object space to homogenous space
            float4 TransformObjectToHClip(float3 positionOS)
            {
                // More efficient than computing M*VP matrix product
                return mul(GetWorldToHClipMatrix(), mul(GetObjectToWorldMatrix(), float4(positionOS, 1.0)));
            }
            float3 TransformWorldToView(float3 positionWS)
            {
                return mul(GetWorldToViewMatrix(), float4(positionWS, 1.0)).xyz;
            }
            float LinearEyeDepth(float depth, float4 zBufferParam)
            {
                return 1.0 / (zBufferParam.z * depth + zBufferParam.w);
            }

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenUV : TEXCOORD0;
                float4 viewRayOS : TEXCOORD1;
                float4 cameraPosOSAndFogFactor : TEXCOORD2;
            };
            
            sampler2D _MainTex;               
            float4 _MainTex_ST;
            int _SplitNumber;
            float _NormalizedLineWidth;
            float _ProjectionAngleDiscardThreshold;
            half4 _Color;
            half2 _AlphaRemap;
            half _MulAlphaToRGB;

            sampler2D _CameraDepthTexture;

            v2f vert(appdata v)
            {
                v2f o;

                //regular MVP
                o.vertex = TransformObjectToHClip(v.vertex.xyz);

                //regular unity fog
                o.cameraPosOSAndFogFactor.a = 0;

                //prepare depth texture's screen space UV
                o.screenUV = ComputeScreenPos(o.vertex);

                //get "camera to vertex" ray in View space
                float3 viewRay = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));

                //***WARNING***
                //=========================================================
                //"viewRay z division" must do in the fragment shader, not vertex shader! (due to rasteriazation varying interpolation's perspective correction)
                //We skip the "viewRay z division" in vertex shader for now, and pass the division value to varying o.viewRayOS.w first, we will do the division later when we enter fragment shader
                //viewRay /= viewRay.z; //skip the "viewRay z division" in vertex shader for now
                o.viewRayOS.w = viewRay.z;//pass the division value to varying o.viewRayOS.w
                //=========================================================

                viewRay *= -1; //unity's camera space is right hand coord(negativeZ pointing into screen), we want positive z ray in fragment shader, so negate it

                //it is ok to write very expensive code in decal's vertex shader, it is just a unity cube(4*6 vertices) per decal only, won't affect GPU performance at all.
                float4x4 ViewToObjectMatrix = mul(unity_WorldToObject, UNITY_MATRIX_I_V);

                //transform everything to object space(decal space) in vertex shader first, so we can skip all matrix mul() in fragment shader
                o.viewRayOS.xyz = mul((float3x3)ViewToObjectMatrix, viewRay);
                o.cameraPosOSAndFogFactor.xyz = mul(ViewToObjectMatrix, float4(0,0,0,1)).xyz;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                //***WARNING***
                //=========================================================
                //now do "viewRay z division" that we skipped in vertex shader earlier.
                i.viewRayOS /= i.viewRayOS.w;
                //=========================================================

                float sceneCameraSpaceDepth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, i.screenUV).r, _ZBufferParams);

                //scene depth in any space = rayStartPos + rayDir * rayLength
                //here all data in ObjectSpace(OS) or DecalSpace
                float3 decalSpaceScenePos = i.cameraPosOSAndFogFactor.xyz + i.viewRayOS.xyz * sceneCameraSpaceDepth;

                //convert unity cube's [-0.5,0.5] vertex pos range to [0,1] uv. Only works if you use unity cube in mesh filter!
                float2 decalSpaceUV = decalSpaceScenePos.xy + 0.5;

                //discard logic
                //===================================================
                // discard "out of cube volume" pixels
                //2020-4-17: tried fix clip() bug by removing all possible bool
                //https://github.com/ColinLeung-NiloCat/UnityURPUnlitScreenSpaceDecalShader/issues/6#issuecomment-614633460            
                float mask = (abs(decalSpaceScenePos.x) < 0.5 ? 1.0 : 0.0) * (abs(decalSpaceScenePos.y) < 0.5 ? 1.0 : 0.0) * (abs(decalSpaceScenePos.z) < 0.5 ? 1.0 : 0.0);

                //call discard
                clip(mask - 0.5);//if ZWrite is off, clip() is fast enough on mobile, because it won't write the DepthBuffer, so no pipeline stall(confirmed by ARM staff).
                //===================================================

                // sample the decal texture
                float2 uv = decalSpaceUV.xy * _MainTex_ST.xy + _MainTex_ST.zw;//Texture tiling & offset

                half4 col = tex2D(_MainTex, uv);
                col *= _Color;//tint color
                col.a = saturate(col.a * _AlphaRemap.x + _AlphaRemap.y);//alpha remap MAD
                col.rgb *= lerp(1, col.a, _MulAlphaToRGB);//extra multiply alpha to RGB
                
	            float2 st = uv;
                st *= _SplitNumber;
                st = frac(st);
                float min = 0;
                float max = _NormalizedLineWidth;
                float c = smoothstep(min, max, st.x)*smoothstep(min, max, st.y);
	            col = half4(1-c,1-c,1-c,0.5);
                return col;
            }

            ENDCG
        }
    }
}
