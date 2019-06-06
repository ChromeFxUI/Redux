using ChromFXUI;
using Newtonsoft.Json;
using ReduxCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ReduxStyleUI
{
    public partial class ReduxStyleForm<State> : ChromFXBaseForm where State : struct, IState
    {
        private Package<State> store;
        public virtual Package<State> Store
        {
            private set
            {
                store = value;
            }
            get
            {
                return store;
            }
        }
        private string jsDispatchMethod = "vm.dispatch({0})";
        public string JsDispatchMethod
        {
            set { jsDispatchMethod = value; }
            get { return jsDispatchMethod; }
        }

        private string jsEmiterMethod = "vm.emit('{0}',{1})";
        /// <summary>
        /// 事件发射器
        /// </summary>
        public string JsEmiterMethod
        {
            set { jsEmiterMethod = value; }
            get { return jsEmiterMethod; }
        }

        public ReduxStyleForm()
            : base(null)
        {

        }

        private Dictionary<string, string> FieldValuePairs = new Dictionary<string, string>();

        public ReduxStyleForm(Package<State> store, string initialUrl)
            : base(initialUrl)
        {
            Store = store;

            Store.Subscribe((subscription, action) =>
            {
                var fields = subscription.GetType().GetFields();
                Parallel.ForEach(fields, new Action<FieldInfo>((field) =>
                {
                    if (FieldValuePairs.ContainsKey(field.Name))
                    {
                        if (JsonConvert.SerializeObject(field.GetValue(subscription)) != FieldValuePairs[field.Name])
                        {
                            var curValue = JsonConvert.SerializeObject(field.GetValue(subscription));
                            string cmd = string.Format(jsEmiterMethod, field.Name, curValue);
                            ExecuteJavascript(cmd);
                            FieldValuePairs[field.Name] = curValue;
                        }
                    }
                    else
                    {
                        var curValue = JsonConvert.SerializeObject(field.GetValue(subscription));

                        string cmd = string.Format(jsEmiterMethod, field.Name, curValue);
                        ExecuteJavascript(cmd);
                        FieldValuePairs.Add(field.Name, curValue);
                    }

                }));

            });
        }

    }
}
