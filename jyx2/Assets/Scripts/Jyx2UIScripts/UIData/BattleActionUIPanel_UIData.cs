using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class BattleActionUIPanel
{
	private RectTransform MainActions_RectTransform;
	private VerticalLayoutGroup RightActions_VerticalLayoutGroup;
	private Button Move_Button;
	private Button UsePoison_Button;
	private Button Depoison_Button;
	private Button Heal_Button;
	private Button Item_Button;
	private Button Wait_Button;
	private Button Rest_Button;
	private RectTransform Skills_RectTransform;
	private RectTransform BlockNotice_RectTransform;
	private Button Surrender_Button;
	private Button Cancel_Button;

	public void InitTrans()
	{
		MainActions_RectTransform = transform.Find("MainActions").GetComponent<RectTransform>();
		RightActions_VerticalLayoutGroup = transform.Find("MainActions/RightActions").GetComponent<VerticalLayoutGroup>();
		Move_Button = transform.Find("MainActions/RightActions/Move").GetComponent<Button>();
		UsePoison_Button = transform.Find("MainActions/RightActions/UsePoison").GetComponent<Button>();
		Depoison_Button = transform.Find("MainActions/RightActions/Depoison").GetComponent<Button>();
		Heal_Button = transform.Find("MainActions/RightActions/Heal").GetComponent<Button>();
		Item_Button = transform.Find("MainActions/RightActions/Item").GetComponent<Button>();
		Wait_Button = transform.Find("MainActions/RightActions/Wait").GetComponent<Button>();
		Rest_Button = transform.Find("MainActions/RightActions/Rest").GetComponent<Button>();
		Skills_RectTransform = transform.Find("MainActions/Skills").GetComponent<RectTransform>();
		BlockNotice_RectTransform = transform.Find("BlockNotice").GetComponent<RectTransform>();
		Surrender_Button = transform.Find("Surrender").GetComponent<Button>();
		Cancel_Button = transform.Find("Cancel").GetComponent<Button>();

	}
}
