/*
 * ��ӹȺ����3D���ư�
 * https://github.com/jynew/jynew
 *
 * ���Ǳ���Դ��Ŀ�ļ�ͷ�����д����ʹ��MITЭ�顣
 * ����Ϸ����Դ�͵����������dll������ϸ�Ķ�LICENSE�����ȨЭ���ĵ���
 *
 * ��ӹ������ǧ�ţ�
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SystemUIPanel
{
	private Button SaveButton_Button;
	private Button LoadButton_Button;
	private Button GraphicSettingsButton_Button;
	private Button MainMenuButton_Button;
	private Button ResumeGameBtn_Button;

	public void InitTrans()
	{
		SaveButton_Button = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container/SaveButton").GetComponent<Button>();
		LoadButton_Button = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container/LoadButton").GetComponent<Button>();
		GraphicSettingsButton_Button = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container/GraphicSettingsButton").GetComponent<Button>();
		MainMenuButton_Button = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container/MainMenuButton").GetComponent<Button>();
		ResumeGameBtn_Button = transform.Find("SelectionPanel/SelectMenu/SelectPanel/Container/ResumeGameBtn").GetComponent<Button>();

	}
}
