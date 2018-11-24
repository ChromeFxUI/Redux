using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ReduxCore
{
    public class State<T> where T:IState 
    {
        public readonly string msg;
        public readonly ProcessEnum status;
        public readonly Type action;
        public readonly string actionShortName;
        public T state = default(T);
    }
}
