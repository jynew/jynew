
namespace Jyx2
{
    public abstract class BasePojo : HSFrameWork.ConfigTable.BaseBean {}
    public class SaveableStringDictionary : HSFrameWork.SPojo.SaveableStrDictionary { }
    public class SaveableDictionary<T> : HSFrameWork.SPojo.SaveableNumberDictionary<T> where T : struct { }
}
