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

public partial class SelectRolePanel
{
	private GridLayoutGroup RoleParent_GridLayoutGroup;
	private Text TitleText_Text;
	private Button ConfirmBtn_Button;
	private Button CancelBtn_Button;

	public void InitTrans()
	{
		RoleParent_GridLayoutGroup = transform.Find("RoleScroll/Viewport/RoleParent").GetComponent<GridLayoutGroup>();
		TitleText_Text = transform.Find("Title/TitleText").GetComponent<Text>();
		ConfirmBtn_Button = transform.Find("Btns/ConfirmBtn").GetComponent<Button>();
		CancelBtn_Button = transform.Find("Btns/CancelBtn").GetComponent<Button>();

	}
}
