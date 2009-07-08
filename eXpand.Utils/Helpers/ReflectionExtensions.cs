using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace eXpand.Utils.Helpers
{
    public static class ReflectionExtensions
    {
        public static MethodInfo GetMethodInfo<TTarget>(this TTarget target, Expression<Action<TTarget>> method)
        {
            return GetMethodInfo(method);
        }

        private static MethodInfo GetMethodInfo(Expression method)
        {
            if (method == null) throw new ArgumentNullException("method");

            var lambda = method as LambdaExpression;
            if (lambda == null) throw new ArgumentException("Not a lambda expression", "method");
            if (lambda.Body.NodeType != ExpressionType.Call) throw new ArgumentException("Not a method call", "method");

            return ((MethodCallExpression) lambda.Body).Method;
        }

        public static PropertyInfo GetPropertyInfo<TTarget>(this TTarget target, Expression<Func<TTarget, object>> property)
        {
            var info = target.GetMemberInfo(property) as PropertyInfo;
            if (info == null) throw new ArgumentException("Member is not a property");

            return info;
        }


        public static FieldInfo GetFieldInfo<TTarget>(this TTarget target, Expression<Func<TTarget, object>> field)
        {
            var info = target.GetMemberInfo(field) as FieldInfo;
            if (info == null) throw new ArgumentException("Member is not a field");

            return info;
        }

        public static MemberInfo GetMemberInfo<TTarget>(this TTarget target,Expression member)
        {
            if (member == null) throw new ArgumentNullException("member");

            var lambda = member as LambdaExpression;
            if (lambda == null) throw new ArgumentException("Not a lambda expression", "member");

            MemberExpression memberExpr = null;


            if (lambda.Body.NodeType == ExpressionType.Convert)
                memberExpr = ((UnaryExpression) lambda.Body).Operand as MemberExpression;
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
                memberExpr = lambda.Body as MemberExpression;

            if (memberExpr == null) throw new ArgumentException("Not a member access", "member");

            return memberExpr.Member;
        }
        
        public static void SetProperty<T>(this INotifyPropertyChanged source,
                                          Expression<Func<T>> propExpr,
                                          ref T propertyValueHolder,
                                          T value, Action doIfChanged) where T : class
        {
            var prop = (PropertyInfo)((MemberExpression)propExpr.Body).Member;
            var currVal = (T)prop.GetValue(source, null);
            if (currVal == null && value == null)
                return;
            if (currVal == null || !currVal.Equals(value))
            {
                propertyValueHolder = value;
                var eventDelegate = (MulticastDelegate)source.GetType().GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic).
                                                           GetValue(source);
                if (eventDelegate != null)
                {
                    Delegate[] delegates = eventDelegate.GetInvocationList();
                    var args = new PropertyChangedEventArgs(prop.Name);
                    foreach (Delegate dlg in delegates)
                        dlg.Method.Invoke(dlg.Target, new object[] { source, args });
                }
                doIfChanged();
            }
        }
        public static void SetProperty<T>(this INotifyPropertyChanged source,
                                          Expression<Func<T>> propExpr,
                                          ref T propertyValueHolder,
                                          T value) where T : class
        {
            source.SetProperty(propExpr, ref propertyValueHolder, value, () => { });
        }


    }
}