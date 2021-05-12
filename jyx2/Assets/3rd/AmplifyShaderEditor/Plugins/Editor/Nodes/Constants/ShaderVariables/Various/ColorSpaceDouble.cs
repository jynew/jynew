namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Color Space Double", "Miscellaneous", "Color Space Double" )]
	public class ColorSpaceDouble : ParentNode
	{
		private const string ColorSpaceDoubleStr = "unity_ColorSpaceDouble";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputColorPorts( "RGBA" );
			m_previewShaderGUID = "ac680a8772bb97c46851a7f075fd04e3";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			return GetOutputVectorItem( 0, outputId, ColorSpaceDoubleStr ); ;
		}
	}
}
