Shader "Hidden/TemplateShaderProperty"
{
	Properties
	{
		[HideFromInspector]_IntData ( "_IntData", Int ) = 0
		[HideFromInspector]_FloatData ( "_FloatData", Float ) = 0
		[HideFromInspector]_VectorData ( "_VectorData", Vector ) = ( 0,0,0,0 )
		[HideFromInspector]_Sampler2DData ( "_Sampler2DData", 2D ) = "white" {}
		[HideFromInspector]_Sampler3DData ( "_Sampler3DData", 3D ) = "white" {}
		[HideFromInspector]_SamplerCubeData ( "_SamplerCubeData", Cube ) = "white" {}
	}

		CGINCLUDE
		#include "UnityCG.cginc"
		uniform int _IntData;
		uniform float _FloatData;
		uniform float4 _VectorData;
		uniform sampler2D _Sampler2DData;
		uniform sampler3D _Sampler3DData;
		uniform samplerCUBE _SamplerCubeData;
		
		#pragma vertex vert_img
		#pragma fragment frag
		
		float4 CalculatePreviewColor ( v2f_img i, const int dataType )
		{
			/*Int*/				if ( dataType == 0 ) return float4( _IntData.xxx,1);
			/*Float*/			if ( dataType == 1 ) return float4( _FloatData.xxx, 1 );
			/*Vector4/Color*/	if ( dataType == 2 ) return  _VectorData;
			/*Sampler 2D*/		if ( dataType == 3 ) return tex2D ( _Sampler2DData, i.uv );
			/*Sampler 3D*/		if ( dataType == 4 ) return tex3D ( _Sampler3DData, float3( i.uv, 0.0 ) );
			/*Sampler Cube*/	if ( dataType == 5 ) return texCUBE ( _SamplerCubeData, float3( i.uv, 0.0 ) );
			return (0).xxxx;
		}

		ENDCG
		
		SubShader
		{
			Pass{ CGPROGRAM	float4 frag ( v2f_img i ) : SV_Target {	return CalculatePreviewColor(i,0); } ENDCG }
			Pass{ CGPROGRAM	float4 frag ( v2f_img i ) : SV_Target {	return CalculatePreviewColor(i,1); } ENDCG }
			Pass{ CGPROGRAM	float4 frag ( v2f_img i ) : SV_Target {	return CalculatePreviewColor(i,2); } ENDCG }
			Pass{ CGPROGRAM	float4 frag ( v2f_img i ) : SV_Target {	return CalculatePreviewColor(i,3); } ENDCG }
			Pass{ CGPROGRAM	float4 frag ( v2f_img i ) : SV_Target {	return CalculatePreviewColor(i,4); } ENDCG }
			Pass{ CGPROGRAM	float4 frag ( v2f_img i ) : SV_Target {	return CalculatePreviewColor(i,5); } ENDCG }
		}
}
