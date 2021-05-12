/***********************************************
				EasyTouch V
	Copyright © 2014-2015 The Hedgehog Team
    http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;

namespace HedgehogTeam.EasyTouch{
[System.Serializable]
public class ECamera{

	public Camera camera;
	public bool guiCamera;
	
	public ECamera( Camera cam,bool gui){
		this.camera = cam;
		this.guiCamera = gui;
	}
	
}
}
