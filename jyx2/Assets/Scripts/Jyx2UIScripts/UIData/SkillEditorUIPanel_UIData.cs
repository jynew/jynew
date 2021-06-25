/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SkillEditorUIPanel
{
	private Button btnDisplaySkill_Button;
	private Button btnRunAnim_Button;
	private Button btnSwitchModel_Button;
	private Dropdown dropSkillId_Dropdown;
	private Dropdown dropSkillLevel_Dropdown;
	private Dropdown dropModelId_Dropdown;

	public void InitTrans()
	{
		btnDisplaySkill_Button = transform.Find("btnDisplaySkill").GetComponent<Button>();
		btnRunAnim_Button = transform.Find("btnRunAnim").GetComponent<Button>();
		btnSwitchModel_Button = transform.Find("btnSwitchModel").GetComponent<Button>();
		dropSkillId_Dropdown = transform.Find("dropSkillId").GetComponent<Dropdown>();
		dropSkillLevel_Dropdown = transform.Find("dropSkillLevel").GetComponent<Dropdown>();
		dropModelId_Dropdown = transform.Find("dropModelId").GetComponent<Dropdown>();

	}
}
