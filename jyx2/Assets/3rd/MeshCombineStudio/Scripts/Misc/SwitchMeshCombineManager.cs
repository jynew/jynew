using UnityEngine;

namespace MeshCombineStudio {
	public class SwitchMeshCombineManager : MonoBehaviour
	{
		MeshCombiner[] meshCombiners;
		GameObject[] gos;
		Transform t;
		GUIStyle style1, style2; 
		int selectIndex = 1;
		
		void Start()
		{
			t = transform;
			
			meshCombiners = GetComponentsInChildren<MeshCombiner>(true);
			meshCombiners[0].InitMeshCombineJobManager();
			
			for (int i = 0; i < meshCombiners.Length; i++)
            {
				meshCombiners[i].CombineAll();
            }

			gos = new GameObject[t.childCount];

			for (int i = 0; i < t.childCount; i++)
			{
				gos[i] = t.GetChild(i).gameObject;
			}

			SetGosActive(false);

			meshCombiners[0].SwapCombine();
		}

		void SetGosActive(bool active)
        {
			for (int i = 0; i < gos.Length; i++)
			{
				gos[i].SetActive(active);
			}
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
            {
				if (meshCombiners[0].combinedActive) meshCombiners[0].SwapCombine();
				SetGosActive(false);
				selectIndex = 1;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
            {
				if (!meshCombiners[0].combinedActive) meshCombiners[0].SwapCombine();
				SetGosActive(false);
				gos[0].SetActive(true);
				selectIndex = 2;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
				if (!meshCombiners[0].combinedActive) meshCombiners[0].SwapCombine();
				SetGosActive(false);
				gos[1].SetActive(true);
				selectIndex = 3; 
			}
		}

		void OnGUI()
        {
			if (style1 == null)
            {
				style1 = new GUIStyle(GUI.skin.label);
				style1.fontStyle = FontStyle.Bold;
				style2 = new GUIStyle(GUI.skin.label);
				style2.fontSize = 14;
				style2.fontStyle = FontStyle.Bold;
			}

			GUILayout.BeginArea(new Rect(10, 10, 500, 500));

			GUILayout.BeginVertical("Box");
			GUILayout.Label("Select with Keyboard keys 1,2 and 3.", style1);
			GUILayout.Space(15);
			if (selectIndex == 1) GUI.color = Color.green; else GUI.color = Color.red;
			GUILayout.Label("1. No Combining", style2);
			if (selectIndex == 2) GUI.color = Color.green; else GUI.color = Color.red;
			GUILayout.Label("2. Normal Combining", style2);
			if (selectIndex == 3) GUI.color = Color.green; else GUI.color = Color.red;
			GUILayout.Label("3. Separate Shadow Combining without backfaces", style2);
			GUILayout.EndVertical();
			GUILayout.EndArea();
        }
	}
}
