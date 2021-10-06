/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
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
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CmdEventReceiver
{
	/// <summary>
	/// 消息接收器单例
	/// </summary>
	public static CmdEventReceiver Instance
	{
        get { return m_Instance ?? (m_Instance = new CmdEventReceiver()); }
	}
	
	static CmdEventReceiver m_Instance;

}
