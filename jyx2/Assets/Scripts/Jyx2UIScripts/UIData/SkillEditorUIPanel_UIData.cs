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
