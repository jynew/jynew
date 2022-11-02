using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Jyx2.InputCore
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class Jyx2InputContextBase:MonoBehaviour,IJyx2InputContext
    {
        
        protected virtual void OnEnable()
        {
            InputContextManager.Instance.AddInputContext(this);
        }

        protected virtual void OnDisable()
        {
            InputContextManager.Instance.RemoveInputContext(this);
        }

        public virtual void OnUpdate()
        {
           
        }
    }
}
