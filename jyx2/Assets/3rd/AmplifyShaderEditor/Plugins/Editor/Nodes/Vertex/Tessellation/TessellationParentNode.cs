namespace AmplifyShaderEditor
{
	public class TessellationParentNode : ParentNode
	{

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_useInternalPortData = true;
		}


		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.PortCategory != MasterNodePortCategory.Tessellation )
			{
				UIUtils.ShowMessage( m_nodeAttribs.Name + " can only be used on Master Node Tessellation port" );
				return "(-1)";
			}

			return BuildTessellationFunction( ref dataCollector );
		}

		protected virtual string BuildTessellationFunction( ref MasterNodeDataCollector dataCollector )
		{
			return string.Empty;
		}
	}
}
