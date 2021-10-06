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

public class CmdEventSender
{
	#region instance

	/// <summary>
	/// 消息发送器单例
	/// </summary>
	public static CmdEventSender Instance
	{
		get { return m_Instance ?? (m_Instance = new CmdEventSender()); }
	}

	static CmdEventSender m_Instance;

	#endregion

	public CmdEventSender() 
	{
		m_Buffer = new BytesBuffer(2 * 1024, false);
	}

	BytesBuffer m_Buffer;

	#region cmd

	public void SendItemCountChange(int itemId, int count) 
	{
		m_Buffer.WriteByte((byte)EnumCmdType.ITEM_COUNT_CHANGE);
		m_Buffer.WriteInt32(itemId);
		m_Buffer.WriteInt32(count);
	}


	#endregion
}
