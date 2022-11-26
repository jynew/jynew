/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class ChatUIPanel
{
	private Button MainBg_Button;
	private RectTransform Content_RectTransform;
	private Text MainContent_Text;
    private EventTrigger Panel_Trigger;
    private RectTransform Name_RectTransform;
	private Text NameTxt_Text;
	private RectTransform kuang_RectTransform;
	private RectTransform HeadAvataPre_RectTransform;
	private Image RoleHeadImage_Image;
	private RectTransform SelectionPanel_RectTransform;
	private RectTransform Container_RectTransform;

	public void InitTrans()
	{
		MainBg_Button = transform.Find("MainBg").GetComponent<Button>();
		Content_RectTransform = transform.Find("Content").GetComponent<RectTransform>();
		//MainContent_Text = transform.Find("Content/MainContent").GetComponent<Text>();
        MainContent_Text = transform.Find("Content/MainContent/Panel/Text").GetComponent<Text>();
        Panel_Trigger = transform.Find("Content/MainContent/Panel").GetComponent<EventTrigger>();
        Name_RectTransform = transform.Find("Name").GetComponent<RectTransform>();
		NameTxt_Text = transform.Find("Name/NameTxt").GetComponent<Text>();
		kuang_RectTransform = transform.Find("kuang").GetComponent<RectTransform>();
		HeadAvataPre_RectTransform = transform.Find("HeadAvataPre").GetComponent<RectTransform>();
		RoleHeadImage_Image = transform.Find("HeadAvataPre/Mask/RoleHeadImage").GetComponent<Image>();
		SelectionPanel_RectTransform = transform.Find("SelectionPanel").GetComponent<RectTransform>();
		Container_RectTransform = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container").GetComponent<RectTransform>();

	}
}
