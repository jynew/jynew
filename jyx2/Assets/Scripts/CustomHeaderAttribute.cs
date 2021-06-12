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

