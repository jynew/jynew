using System;
using System.Collections;
using System.Collections.Generic;
using HanSquirrel.ResourceManager;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEditor;
using UnityEngine;
using XNode;

[CreateNodeMenu("对话")]
[NodeWidth(256)]
public class Jyx2TalkNode : Jyx2BaseNode
{

	private void Reset() {
		name = "对话";
	}
	
	public int roleId;
	public string content;
	
	// Use this for initialization
	protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}

	public Texture2D GetRoleHeadTexture()
	{
		int id = GetInputValue("roleId", this.roleId);

		return AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/BuildSource/head/{id}.png");
	}

	/*[ContextMenu("中文测试")]
	void Hello()
	{
		Debug.Log("hello!");
	}*/
}