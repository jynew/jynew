using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class BattleActionOrderPanel
{
	private RectTransform MainRoot_RectTransform;
	private RectTransform HeadItem_RectTransform;

	public void InitTrans()
	{
		MainRoot_RectTransform = transform.Find("MainRoot").GetComponent<RectTransform>();
		HeadItem_RectTransform = transform.Find("Prefabs/HeadItem").GetComponent<RectTransform>();

	}
}
