using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTalkMan : MonoBehaviour {

    public string[] Words;

    MapRole mapRole;

	// Use this for initialization
	void Start () {
        mapRole = GetComponent<MapRole>();
        InvokeRepeating("RandomTalk",5,5);
	}
	
    void RandomTalk()
    {
        if(Words != null && Words.Length > 0 && mapRole != null)
        {
            mapRole.Say(Words[Random.Range(0, Words.Length - 1)]);
        }
    }

}
