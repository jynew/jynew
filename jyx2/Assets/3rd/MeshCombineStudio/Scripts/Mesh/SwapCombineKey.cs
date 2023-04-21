using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class SwapCombineKey : MonoBehaviour
    {
        static public SwapCombineKey instance;
        public List<MeshCombiner> meshCombinerList = new List<MeshCombiner>();
        MeshCombiner meshCombiner;
        GUIStyle textStyle;

        void Awake()
        {
            instance = this;
            meshCombiner = GetComponent<MeshCombiner>();
            meshCombinerList.Add(meshCombiner);

            QualitySettings.vSyncCount = 0;
        }

        void OnDestroy()
        {
            instance = null;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                for (int i = 0; i < meshCombinerList.Count; i++)
                {
                    if (meshCombinerList[i].meshCombineJobs.Count > 0) return;
                }

                for (int i = 0; i < meshCombinerList.Count; i++)
                { 
                    meshCombinerList[i].SwapCombine();
                }
            }
        }

        private void OnGUI()
        {
            if (textStyle == null)
            {
                textStyle = new GUIStyle("label");
                textStyle.fontStyle = FontStyle.Bold;
                textStyle.fontSize = 16;
            }

            textStyle.normal.textColor = (this.meshCombiner.combinedActive && this.meshCombiner.combined) ? Color.green : Color.red;

            int meshCombineJobsCount = 0;

            GUI.Box(new Rect(5, 30, 310, 40 + (meshCombinerList.Count * 22)), GUIContent.none);

            for (int i = 0; i < meshCombinerList.Count; i++)
            {
                MeshCombiner meshCombiner = meshCombinerList[i];
                if (meshCombiner.meshCombineJobs.Count > meshCombineJobsCount) meshCombineJobsCount = meshCombiner.meshCombineJobs.Count;
                if (meshCombiner.combinedActive && meshCombiner.combined) GUI.Label(new Rect(10, 30 + (i * 22), 300, 30), meshCombiner.gameObject.name + " is Enabled.", textStyle);
                else GUI.Label(new Rect(10, 30 + (i * 22), 300, 30), meshCombiner.gameObject.name + " is Disabled.", textStyle);
            }


            if (meshCombineJobsCount > 0) GUI.Label(new Rect(10, 45 + (meshCombinerList.Count * 22), 250, 30), "Combining => Jobs Left " + meshCombineJobsCount, textStyle);
            else GUI.Label(new Rect(10, 45 + (meshCombinerList.Count * 22), 200, 30), "Toggle with 'Tab' key.", textStyle);
        }
    }
}
