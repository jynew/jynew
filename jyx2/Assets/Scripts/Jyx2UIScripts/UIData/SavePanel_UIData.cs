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

public partial class SavePanel
{
	private Button BackButton_Button;
	private Text MainText_Text;
	private RectTransform SaveParent_RectTransform;
	private Button Export_Button;
	private Button Import_Button;

	public void InitTrans()
	{
		BackButton_Button = transform.Find("TopbarUI/BackButton").GetComponent<Button>();
		MainText_Text = transform.Find("MainText").GetComponent<Text>();
		SaveParent_RectTransform = transform.Find("SaveParent").GetComponent<RectTransform>();
		Export_Button = transform.Find("FileIO/Export").GetComponent<Button>();
		Import_Button = transform.Find("FileIO/Import").GetComponent<Button>();

	}
}
