using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SkillEditorUIPanel
{
	private InputField iptSkillid_InputField;
	private Button btnSwitchSkill_Button;
	private InputField iptSkillLevel_InputField;
	private Button btnDisplaySkill_Button;
	private Button btnRunAnim_Button;
	private InputField iptModelId_InputField;
	private Button btnSwitchModel_Button;

	public void InitTrans()
	{
		iptSkillid_InputField = transform.Find("iptSkillid").GetComponent<InputField>();
		btnSwitchSkill_Button = transform.Find("btnSwitchSkill").GetComponent<Button>();
		iptSkillLevel_InputField = transform.Find("iptSkillLevel").GetComponent<InputField>();
		btnDisplaySkill_Button = transform.Find("btnDisplaySkill").GetComponent<Button>();
		btnRunAnim_Button = transform.Find("btnRunAnim").GetComponent<Button>();
		iptModelId_InputField = transform.Find("iptModelId").GetComponent<InputField>();
		btnSwitchModel_Button = transform.Find("btnSwitchModel").GetComponent<Button>();

	}
}
