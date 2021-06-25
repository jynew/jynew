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

public class CustomHeaderAttribute : HeaderAttribute
{
    public CustomHeaderAttribute(string header) : base(header)
    {
    }

    public override object TypeId => base.TypeId;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool IsDefaultAttribute()
    {
        return base.IsDefaultAttribute();
    }

    public override bool Match(object obj)
    {
        return base.Match(obj);
    }

    public override string ToString()
    {
        return base.ToString();
    }
}

