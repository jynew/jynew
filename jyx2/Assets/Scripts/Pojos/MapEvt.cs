
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HSFrameWork.ConfigTable;

namespace Jyx2
{
    [XmlType("mapevt")]
    public class MapEvt : BaseBean
    {
        public override string PK { get { return Id; } }

        [XmlAttribute]
        public string Id;

        [XmlAttribute]
        public string Repeat;

        [XmlAttribute]
        public string ExecuteCode;

        [XmlAttribute]
        public string Result;

        
        /// <summary>
        /// 判断一个事件是否已经完成了
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public bool IsFinished(GameRuntimeData runtime)
        {
            if (runtime == null)
                return false;

            //判断是否已经执行过了
            if (this.Repeat == "MapOnce" && MapRuntimeData.Instance.KeyValues.ContainsKey(this.Id))
            {
                return true;
            }
            else if (this.Repeat == "Once" && runtime.KeyValues.ContainsKey(this.Id))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 标记一个事件为正在执行
        /// </summary>
        /// <param name="runtime"></param>
        public void MarkAsExecuting(GameRuntimeData runtime)
        {
            if (this.Repeat == "MapOnce")
            {
                MapRuntimeData.Instance.KeyValues[this.Id] = "executing";
            }else if(this.Repeat == "Once")
            {
                runtime.KeyValues[this.Id] = "executing";
            }
        }

        /// <summary>
        /// 标记一个事件为已完成
        /// </summary>
        /// <param name="runtime"></param>
        public void MarkAsFinished(GameRuntimeData runtime)
        {
            if (runtime == null)
                return;

            if (this.Repeat == "MapOnce")
            {
                MapRuntimeData.Instance.KeyValues[this.Id] = "finished";
            }
            else if (this.Repeat == "Once")
            {
                runtime.KeyValues[this.Id] = "finished";
            }
        }
    }
}
