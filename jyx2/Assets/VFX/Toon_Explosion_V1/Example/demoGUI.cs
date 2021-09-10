using UnityEngine;
using System.Collections;

public class demoGUI : MonoBehaviour {
	
	public GameObject[] explosions;
	
	void OnGUI(){
		
		for (int i=0;i<explosions.Length/2;i++){
		
			GUI.color = new Color(1f,0.75f,0.5f);
			if (GUI.Button(new Rect( 10,10+i*30,100,20),explosions[i].name)){
				Instantiate( explosions[i],new Vector3(0f,2f,0f), Quaternion.identity);
			}
		}
		
		int j=0;
		for (int i=explosions.Length/2;i<explosions.Length;i++){
		
			GUI.color = new Color(1f,0.75f,0.5f);
			if (GUI.Button(new Rect( Screen.width-120,10+j*30,100,20),explosions[i].name)){
				Instantiate( explosions[i],new Vector3(0f,2f,0f), Quaternion.identity);
			}
			j++;
		}
		
	}
}
