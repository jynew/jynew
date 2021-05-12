// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
    [Serializable]
    public class FallbackColor : IFallbackVars
    {
        [SerializeField]
        private Color m_current;
        [SerializeField]
        private Color m_previous;

        public FallbackColor()
        {
            m_current = new Color(0, 0, 0, 0);
            m_previous = new Color(0, 0, 0, 0);
        }

        public FallbackColor(Color data)
        {
            m_current = data;
            m_previous = data;
        }

        public void Revert()
        {
            Color aux = m_current;
            m_current = m_previous;
            m_previous = aux;
        }
        
        public Color Current
        {
            get
            {
                return m_current;
            }

            set
            {
                m_previous = m_current;
                m_current = value;
            }
        }

        public override string ToString()
        {
            return m_current.ToString();
        }
    }
}
