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
using UnityEngine.UI;
using System.Collections;

public class NonDrawingGraphic : Graphic
{
	protected NonDrawingGraphic ()
	{
		useLegacyMeshGeneration = false;
	}

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		toFill.Clear();
	}
}
