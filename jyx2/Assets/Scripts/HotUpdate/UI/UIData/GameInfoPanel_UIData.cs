using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class GameInfoPanel
{
	private Text VersionText_Text;

	public void InitTrans()
	{
		VersionText_Text = transform.Find("VersionText").GetComponent<Text>();

	}
}
