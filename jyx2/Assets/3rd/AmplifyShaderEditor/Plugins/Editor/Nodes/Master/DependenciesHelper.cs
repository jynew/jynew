using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
    [Serializable]
    public class DependenciesData
    {
        private string DependencyFormat = "Dependency \"{0}\"=\"{1}\"\n";
        public string DependencyName;
        public string DependencyValue;
        public bool DependencyFoldout = true;

        public DependenciesData()
        {
            DependencyName = string.Empty;
            DependencyValue = string.Empty;
        }

        public DependenciesData( string data )
        {
            string[] arr = data.Split( IOUtils.VALUE_SEPARATOR );
            if( arr.Length > 1 )
            {
                DependencyName = arr[ 0 ];
                DependencyValue = arr[ 1 ];
            }
        }

        public override string ToString()
        {
            return DependencyName + IOUtils.VALUE_SEPARATOR + DependencyValue;
        }

        public string GenerateDependency()
        {
            return string.Format( DependencyFormat, DependencyName, DependencyValue );
        }

        public bool IsValid { get { return ( !string.IsNullOrEmpty( DependencyValue ) && !string.IsNullOrEmpty( DependencyName ) ); } }
    }

    [Serializable]
    public class DependenciesHelper
    {
        private const string CustomDependencysStr = " Dependencies";
        private const string DependencyNameStr = "Name";
        private const string DependencyValueStr = "Value";

        private const float ShaderKeywordButtonLayoutWidth = 15;
        private ParentNode m_currentOwner;

        [SerializeField]
        private List<DependenciesData> m_availableDependencies = new List<DependenciesData>();

        public void Draw( ParentNode owner, bool isNested = false )
        {
            m_currentOwner = owner;
            bool value = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedDependencies;
			if( isNested )
			{
				NodeUtils.DrawNestedPropertyGroup( ref value, CustomDependencysStr, DrawMainBody, DrawButtons );
			}
			else
			{
				NodeUtils.DrawPropertyGroup( ref value, CustomDependencysStr, DrawMainBody, DrawButtons );
			}
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedDependencies = value;
        }

        void DrawButtons()
        {
            EditorGUILayout.Separator();

            // Add Dependency
            if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
            {
                m_availableDependencies.Add( new DependenciesData() );
                EditorGUI.FocusTextInControl( null );
            }

            //Remove Dependency
            if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
            {
                if( m_availableDependencies.Count > 0 )
                {
                    m_availableDependencies.RemoveAt( m_availableDependencies.Count - 1 );
                    EditorGUI.FocusTextInControl( null );
                }
            }
        }

        void DrawMainBody()
        {
            EditorGUILayout.Separator();
            int itemCount = m_availableDependencies.Count;

            if( itemCount == 0 )
            {
                EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add one.", MessageType.Info );
            }

            int markedToDelete = -1;
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            for( int i = 0; i < itemCount; i++ )
            {
                m_availableDependencies[ i ].DependencyFoldout = m_currentOwner.EditorGUILayoutFoldout( m_availableDependencies[ i ].DependencyFoldout, string.Format( "[{0}] - {1}", i, m_availableDependencies[ i ].DependencyName ) );
                if( m_availableDependencies[ i ].DependencyFoldout )
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUIUtility.labelWidth = 70;
                    //Dependency Name
                    EditorGUI.BeginChangeCheck();
                    m_availableDependencies[ i ].DependencyName = EditorGUILayout.TextField( DependencyNameStr, m_availableDependencies[ i ].DependencyName );
                    if( EditorGUI.EndChangeCheck() )
                    {
                        m_availableDependencies[ i ].DependencyName = UIUtils.RemoveShaderInvalidCharacters( m_availableDependencies[ i ].DependencyName );
                    }

                    //Dependency Value
                    EditorGUI.BeginChangeCheck();
                    m_availableDependencies[ i ].DependencyValue = EditorGUILayout.TextField( DependencyValueStr, m_availableDependencies[ i ].DependencyValue );
                    if( EditorGUI.EndChangeCheck() )
                    {
                        m_availableDependencies[ i ].DependencyValue = UIUtils.RemoveShaderInvalidCharacters( m_availableDependencies[ i ].DependencyValue );
                    }

                    EditorGUIUtility.labelWidth = originalLabelWidth;

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label( " " );
                        // Add new port
                        if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
                        {
                            m_availableDependencies.Insert( i + 1, new DependenciesData() );
                            EditorGUI.FocusTextInControl( null );
                        }

                        //Remove port
                        if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
                        {
                            markedToDelete = i;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel -= 1;
                }

            }
            if( markedToDelete > -1 )
            {
                if( m_availableDependencies.Count > markedToDelete )
                {
                    m_availableDependencies.RemoveAt( markedToDelete );
                    EditorGUI.FocusTextInControl( null );
                }
            }
            EditorGUILayout.Separator();
        }


        public void ReadFromString( ref uint index, ref string[] nodeParams )
        {
            int count = Convert.ToInt32( nodeParams[ index++ ] );
            for( int i = 0; i < count; i++ )
            {
                m_availableDependencies.Add( new DependenciesData( nodeParams[ index++ ] ) );
            }
        }

        public void WriteToString( ref string nodeInfo )
        {
            int dependencyCount = m_availableDependencies.Count;
            IOUtils.AddFieldValueToString( ref nodeInfo, dependencyCount );
            for( int i = 0; i < dependencyCount; i++ )
            {
                IOUtils.AddFieldValueToString( ref nodeInfo, m_availableDependencies[ i ].ToString() );
            }
        }

        public string GenerateDependencies()
        {
            int dependencyCount = m_availableDependencies.Count;
            string result = dependencyCount == 0 ? string.Empty : "\n";
            UIUtils.ShaderIndentLevel++;
            for( int i = 0; i < dependencyCount; i++ )
            {
                if( m_availableDependencies[ i ].IsValid )
                {
                    result += UIUtils.ShaderIndentTabs + m_availableDependencies[ i ].GenerateDependency();
                }
            }
            UIUtils.ShaderIndentLevel--;
            return result;
        }

		public void Destroy()
        {
            m_availableDependencies.Clear();
            m_availableDependencies = null;
            m_currentOwner = null;
        }

        public bool HasDependencies { get { return m_availableDependencies.Count > 0; } }
    }
}
