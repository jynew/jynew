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

public partial class BattleActionOrderPanel
{
	private RectTransform MainRoot_RectTransform;
	private RectTransform HeadItem_RectTransform;

	public void InitTrans()
	{
		MainRoot_RectTransform = transform.Find("MainRoot").GetComponent<RectTransform>();
		HeadItem_RectTransform = transform.Find("Prefabs/HeadItem").GetComponent<RectTransform>();

	}
}
