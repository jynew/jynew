using TapTap.Common;

namespace TapTap.AntiAddiction 
{
    public class AntiAddictionException : TapException 
    {
        public string Error { get; internal set; }

        public string Description { get; internal set; }

        public AntiAddictionException(int code, string message) : base(code, message) { }
    }
}
