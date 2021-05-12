using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
    [Serializable]
    [NodeAttributes( "Texture Transform", "Textures", "Gives access to texture tiling and offset as set on the material inspector" )]
    public sealed class TextureTransformNode : ParentNode
    {
        private readonly string[] Dummy = { string.Empty };
        [SerializeField]
        private int m_referenceSamplerId = -1;

        [SerializeField]
        private int m_referenceNodeId = -1;

        [SerializeField]
        private TexturePropertyNode m_inputReferenceNode = null;

        private TexturePropertyNode m_referenceNode = null;

        private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();
        protected override void CommonInit( int uniqueId )
        {
            base.CommonInit( uniqueId );
            AddInputPort( WirePortDataType.SAMPLER2D, false, "Tex" );
            m_inputPorts[ 0 ].CreatePortRestrictions( WirePortDataType.SAMPLER1D, WirePortDataType.SAMPLER2D, WirePortDataType.SAMPLER3D, WirePortDataType.SAMPLERCUBE, WirePortDataType.OBJECT );
            AddOutputPort( WirePortDataType.FLOAT2, "Tiling" );
            AddOutputPort( WirePortDataType.FLOAT2, "Offset" );
            m_textLabelWidth = 80;
            m_autoWrapProperties = true;
            m_hasLeftDropdown = true;
        }

        public override void AfterCommonInit()
        {
            base.AfterCommonInit();

            if( PaddingTitleLeft == 0 )
            {
                PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
                if( PaddingTitleRight == 0 )
                    PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
            }
        }

        public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
        {
            base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
            m_inputReferenceNode = m_inputPorts[ 0 ].GetOutputNode() as TexturePropertyNode;
        }


        public override void OnInputPortDisconnected( int portId )
        {
            base.OnInputPortDisconnected( portId );
            m_inputReferenceNode = null;
        }


        void UpdateTitle()
        {
            if( m_inputReferenceNode != null )
            {
                m_additionalContent.text = string.Format( Constants.PropertyValueLabel, m_inputReferenceNode.PropertyInspectorName );
            }
            else if( m_referenceSamplerId > -1 && m_referenceNode != null )
            {
                m_additionalContent.text = string.Format( Constants.PropertyValueLabel, m_referenceNode.PropertyInspectorName );
            }
            else
            {
                m_additionalContent.text = string.Empty;
            }
            m_sizeIsDirty = true;
        }

        public override void DrawProperties()
        {
            base.DrawProperties();
            EditorGUI.BeginChangeCheck();
            string[] arr = UIUtils.TexturePropertyNodeArr();
            bool guiEnabledBuffer = GUI.enabled;

            if( arr != null && arr.Length > 0 )
            {
                GUI.enabled = true && ( !m_inputPorts[ 0 ].IsConnected );
                m_referenceSamplerId = EditorGUILayoutPopup( Constants.AvailableReferenceStr, m_referenceSamplerId, arr );
            }
            else
            {
                m_referenceSamplerId = -1;
                GUI.enabled = false;
                m_referenceSamplerId = EditorGUILayoutPopup( Constants.AvailableReferenceStr, m_referenceSamplerId, Dummy );
            }
            
            GUI.enabled = guiEnabledBuffer;

            if( EditorGUI.EndChangeCheck() )
            {
                m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceSamplerId );
                m_referenceNodeId = m_referenceNode.UniqueId;
                UpdateTitle();
            }
        }

        public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
        {
            base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
            m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
            string texTransform = string.Empty;

            if( m_inputPorts[ 0 ].IsConnected )
            {
                texTransform = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) + "_ST";
            }
            else if( m_referenceNode != null )
            {
                m_referenceNode.BaseGenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
                texTransform = m_referenceNode.PropertyName + "_ST";
            }
            else
            {
                texTransform = "_ST";
                UIUtils.ShowMessage( "Please specify a texture sample on the Texture Transform Size node", MessageSeverity.Warning );
            }

            dataCollector.AddToUniforms( UniqueId, "uniform float4 " + texTransform + ";" );

            switch( outputId )
            {
                case 0: return ( texTransform + ".xy" );
                case 1: return ( texTransform + ".zw" );
                
            }

            return string.Empty;
        }

        public override void Draw( DrawInfo drawInfo )
        {
            base.Draw( drawInfo );

            EditorGUI.BeginChangeCheck();
            {
                string[] arr = UIUtils.TexturePropertyNodeArr();
                bool guiEnabledBuffer = GUI.enabled;

                if( arr != null && arr.Length > 0 )
                {
                    GUI.enabled = true && ( !m_inputPorts[ 0 ].IsConnected );
                    m_referenceSamplerId = m_upperLeftWidget.DrawWidget( this, m_referenceSamplerId, arr );
                }
                else
                {
                    m_referenceSamplerId = -1;
                    GUI.enabled = false;
                    m_referenceSamplerId = m_upperLeftWidget.DrawWidget( this, m_referenceSamplerId, Dummy );
                }
                GUI.enabled = guiEnabledBuffer;
            }
            if( EditorGUI.EndChangeCheck() )
            {
                m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceSamplerId );
                m_referenceNodeId = m_referenceNode.UniqueId;
                UpdateTitle();
            }

            if( m_referenceNode == null && m_referenceNodeId > -1 )
            {
                m_referenceNodeId = -1;
                m_referenceSamplerId = -1;
                UpdateTitle();
            }
        }

        public override void RefreshExternalReferences()
        {
            base.RefreshExternalReferences();
            m_referenceNode = UIUtils.GetNode( m_referenceNodeId ) as TexturePropertyNode;
            m_referenceSamplerId = UIUtils.GetTexturePropertyNodeRegisterId( m_referenceNodeId );
            UpdateTitle();
        }

        public override void ReadFromString( ref string[] nodeParams )
        {
            base.ReadFromString( ref nodeParams );
            m_referenceNodeId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
            
        }

        public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
        {
            base.WriteToString( ref nodeInfo, ref connectionsInfo );
            IOUtils.AddFieldValueToString( ref nodeInfo, m_referenceNodeId );
        }

        public override void Destroy()
        {
            base.Destroy();
            m_referenceNode = null;
            m_inputReferenceNode = null;
            m_upperLeftWidget = null;
        }
    }
}
