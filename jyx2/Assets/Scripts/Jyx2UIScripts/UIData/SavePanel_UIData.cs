using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SavePanel
{
	private RectTransform SaveParent_RectTransform;
	private Button BackButton_Button;
	private Button SaveItem_Button;

	public void InitTrans()
	{
		SaveParent_RectTransform = transform.Find("SaveParent").GetComponent<RectTransform>();
		BackButton_Button = transform.Find("TopbarUI/BackButton").GetComponent<Button>();
		SaveItem_Button = transform.Find("ItemRoot/SaveItem").GetComponent<Button>();

	}
}
