using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ReduxCore
{
    /// <summary>
    /// 复合分流器
    /// </summary>
    /// <typeparam name="State"></typeparam>
    public class ComposableReducer<R>:State<R> where R :IState
    {
        /// <summary>
        /// 属性分流器集
        /// </summary>
        private readonly List<Tuple<FieldInfo, Delegate>> fieldReducers = new List<Tuple<FieldInfo, Delegate>>();
        /// <summary>
        /// 状态初始化
        /// </summary>
        private readonly Func<R> stateInitializer;

        public ComposableReducer()
        {
            stateInitializer = () =>
            default(R);
        }

        public ComposableReducer(Func<R> initializer)
        {
            this.stateInitializer = initializer;
        }
        /// <summary>
        /// 分流器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="composer"></param>
        /// <param name="reducer"></param>
        /// <returns></returns>
        public ComposableReducer<R> Diverter<T>(Expression<Func<State<R>,T>> composer, ElementReducer<T> reducer) where T:IState
        {
            return Diverter(composer, reducer.Get());
        }
       
        private ComposableReducer<R> Diverter<T>(Expression<Func<State<R>, T>> composer, Reducer<T> reducer) where T : IState
        {
            var memberExpr = composer.Body as MemberExpression;
            if (memberExpr == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' should be a field.",
                    composer.ToString()));

            var member = (FieldInfo)memberExpr.Member;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' should be a constant expression",
                    composer.ToString()));

            fieldReducers.Add(new Tuple<FieldInfo, Delegate>(member, reducer));
            return this;
        }
        /// <summary>
        /// 获取当前分流器
        /// </summary>
        /// <returns></returns>
        public Reducer<R> Get()
        {
            return delegate (R state, Object action)
            {
                var result = action.GetType() == typeof(InitPackageAction) ? stateInitializer() : state;
                foreach (var fieldReducer in fieldReducers)
                {
                    Type memberType = fieldReducer.Item1.FieldType;
                    var prevState = action.GetType() == typeof(InitPackageAction)
                        ? memberType.GetConstructors()[0].Invoke(null)
                        : fieldReducer.Item1.GetValue(state);
                    var newState = fieldReducer.Item2.DynamicInvoke(prevState, action);
                    object boxer = result; 
                    fieldReducer.Item1.SetValue(boxer, newState);

                    result = (R)boxer; 
                }
                return result;
            };
        }

    }
}
