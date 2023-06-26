using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class GameInfoPanel:Jyx2_UIBase
{
	public override UILayer Layer => UILayer.Top;
	public override bool AlwaysDisplay => true;

	protected override void OnCreate()
	{
		InitTrans();
	}

	protected override void OnShowPanel(params object[] allParams)
	{
		base.OnShowPanel(allParams);
		VersionText_Text.text = allParams[0] as string;
	}
}
