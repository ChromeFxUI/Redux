using ChromFXUI;
using Newtonsoft.Json;
using ReduxCore;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ReduxStyleUI
{
    public partial class ReduxStyleForm<State> : ChromFXBaseForm where State : struct,IState
    {
        private State preStore;
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

        public ReduxStyleForm(Package<State> store, string initialUrl)
            : base(initialUrl)
        {
            Store = store;

            Store.Subscribe((subscription, action) =>
            {
                //var state = store.GetState();
                //string cmd = string.Format(jsDispatchMethod, JsonConvert.SerializeObject(state));
                //ExecuteJavascript(cmd);

                var fields = subscription.GetType().GetFields();
                Parallel.ForEach(fields, new Action<FieldInfo>((field) =>
                {
                    if (JsonConvert.SerializeObject(field.GetValue(subscription)) != JsonConvert.SerializeObject(field.GetValue(preStore)))
                    {
                        field.SetValue(preStore, field.GetValue(subscription));
                        string cmd = string.Format(jsEmiterMethod, field.Name, JsonConvert.SerializeObject(field.GetValue(subscription)));
                        ExecuteJavascript(cmd);
                    }

                }));
   
            });
        }

    }
}
