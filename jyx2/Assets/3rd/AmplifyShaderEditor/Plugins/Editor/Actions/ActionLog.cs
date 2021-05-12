// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System.Collections.Generic;

namespace AmplifyShaderEditor
{
    public class ActionLog
    {
        private int m_maxCount;
        private int m_index;
        private List<ActionData> m_sequence;

        public ActionLog(int maxCount)
        {
            m_maxCount = maxCount;
            m_index = 0;
            m_sequence = new List<ActionData>();
        }

        public void AddToLog(ActionData actionData)
        {
            if (m_sequence.Count > m_maxCount)
            {
                m_sequence.RemoveAt(0);
            }

            m_sequence.Add(actionData);
            m_index = m_sequence.Count - 1;
        }


        public void UndoLastAction()
        {
            if ( m_index > -1 && m_index < m_sequence.Count )
                m_sequence[m_index--].ExecuteReverse();
        }

        public void RedoLastAction()
        {
            if (m_index < (m_sequence.Count - 1))
                m_sequence[++m_index].ExecuteForward();
            
        }

        public void ClearLog()
        {
            m_sequence.Clear();
            m_index = 0;
        }

        public void Destroy()
        {
            m_sequence.Clear();
            m_sequence = null;
        }
    }
}
