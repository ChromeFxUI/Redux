using System;
using System.Collections.Generic;

namespace ReduxCore
{
    /// <summary>
    /// 元子分流器
    /// </summary>
    /// <typeparam name="State"></typeparam>
    public class ElementReducer<State> where State : IState
    {
        /// <summary>
        /// 操作者
        /// </summary>
        private readonly Dictionary<Type, Delegate> handlers = new Dictionary<Type, Delegate>();
        /// <summary>
        /// 邮包状态初始化事件
        /// </summary>
        private readonly Func<State> stateInitializer;

        public ElementReducer()
        {
            stateInitializer = () =>
            {
                return default(State);
            };
        }

        public ElementReducer(Func<State> initializer)
        {
            this.stateInitializer = initializer;
        }
        /// <summary>
        /// 处理分离器上的事件 
        /// </summary>
        /// <typeparam name="Event">事件类型</typeparam>
        /// <param name="handler">处理者</param>
        /// <returns>返回元子分流器</returns>
        public ElementReducer<State> Process<Event>(Func<State, Event, State> handler)
        {
            handlers.Add(typeof(Event), handler);
            return this;
        }
        /// <summary>
        /// 获取当前分流器
        /// </summary>
        /// <returns></returns>
        public Reducer<State> Get() 
        {
            return delegate (State state, Object action)
            {
                var prevState = state;// action.GetType() == typeof(InitPackageAction) ? stateInitializer() : state;
                if (prevState.GetType().GetField("action") != null)
                {
                    prevState.GetType().GetField("action").SetValue(prevState, action.GetType());
                }

                if (prevState.GetType().GetField("actionShortName") != null)
                {
                    prevState.GetType().GetField("actionShortName").SetValue(prevState, action.GetType().Name);
                }

                if (prevState.GetType().GetField("msg") != null)
                {
                    prevState.GetType().GetField("msg").SetValue(prevState, "正在处理动作"+ action.GetType().FullName);
                }
                if (prevState.GetType().GetField("status") != null)
                {
                    prevState.GetType().GetField("status").SetValue(prevState, ProcessEnum.Start);
                }

                if (prevState.GetType().GetField("state") != null)
                {
                    var preStateValue = prevState.GetType().GetField("state").GetValue(prevState);
                    if(preStateValue == null)
                    {
                        prevState.GetType().GetField("state").SetValue(prevState, prevState.GetType().GetField("state").FieldType.GetConstructors()[0].Invoke(null));
                    }
                }

                try
                {
                    if (handlers.ContainsKey(action.GetType()))
                    {
                        var handler = handlers[action.GetType()];
                        prevState = (State)handler.DynamicInvoke(prevState, action);
                    }

                    if (prevState.GetType().GetField("msg") != null)
                    {
                        prevState.GetType().GetField("msg").SetValue(prevState, "已处理完动作" + action.GetType().FullName);
                    }
                    if (prevState.GetType().GetField("status") != null)
                    {
                        prevState.GetType().GetField("status").SetValue(prevState, ProcessEnum.Completed);
                    }

                    if (prevState.GetType().GetField("state") != null)
                    {
                        var preStateValue = prevState.GetType().GetField("state").GetValue(prevState);
                        if (preStateValue == null)
                        {
                            prevState.GetType().GetField("state").SetValue(prevState, prevState.GetType().GetField("state").FieldType.GetConstructors()[0].Invoke(null));
                        }
                    }
                }
                catch(Exception ex)
                {
                    if (prevState.GetType().GetField("msg") != null)
                    {
                        prevState.GetType().GetField("msg").SetValue(prevState, "异常信息：\r\n" + ex.StackTrace);
                    }
                    if (prevState.GetType().GetField("status") != null)
                    {
                        prevState.GetType().GetField("status").SetValue(prevState, ProcessEnum.Error);
                    }
                }
                

                return prevState;
            };
        }
    }
}
