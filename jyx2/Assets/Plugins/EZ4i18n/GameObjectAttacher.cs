using UnityEngine;

public abstract class GameObjectAttacher : MonoBehaviour
{
    /// <summary>
    /// 刷新文本内容函数，所有Attacher都会用到
    /// </summary>
    public abstract void Refresh();
}
