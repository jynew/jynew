using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ChatUIPanel
{
	private Button MainBg_Button;
	private RectTransform Content_RectTransform;
	private Text MainContent_Text;
	private RectTransform HeadAvataPre_RectTransform;
	private Image RoleHeadImage_Image;
	private RectTransform SelectionPanel_RectTransform;
	private Button StorySelectionItem_Button;
	private RectTransform Container_RectTransform;

	public void InitTrans()
	{
		MainBg_Button = transform.Find("MainBg").GetComponent<Button>();
		Content_RectTransform = transform.Find("Content").GetComponent<RectTransform>();
		MainContent_Text = transform.Find("Content/MainContent").GetComponent<Text>();
		HeadAvataPre_RectTransform = transform.Find("HeadAvataPre").GetComponent<RectTransform>();
		RoleHeadImage_Image = transform.Find("HeadAvataPre/Mask/RoleHeadImage").GetComponent<Image>();
		SelectionPanel_RectTransform = transform.Find("SelectionPanel").GetComponent<RectTransform>();
		StorySelectionItem_Button = transform.Find("SelectionPanel/SelectMenu/SelectPanel/StorySelectionItem").GetComponent<Button>();
		Container_RectTransform = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container").GetComponent<RectTransform>();

	}
}
