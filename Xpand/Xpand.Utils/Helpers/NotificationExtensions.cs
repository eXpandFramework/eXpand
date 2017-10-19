using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Xpand.Utils.Helpers{
    public static class NotificationExtensions{
        #region Delegates

        public delegate void PropertyChangedHandler<in TSender>(TSender sender);

        #endregion


        public static void Notify(this PropertyChangedEventHandler eventHandler, Expression<Func<object>> property){
            if (eventHandler == null)
                return;


            var lambda = property as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression){
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else{
                memberExpression = lambda.Body as MemberExpression;
            }
            if (memberExpression != null){
                var constantExpression = memberExpression.Expression as ConstantExpression;
                var propertyInfo = memberExpression.Member as PropertyInfo;
                foreach (var del in eventHandler.GetInvocationList())
                    if (constantExpression != null && propertyInfo != null)
                        del.DynamicInvoke(constantExpression.Value, new PropertyChangedEventArgs(propertyInfo.Name));
            }
        }


        public static void SubscribeToPropertyChange<T>(this T objectThatNotifies, Expression<Func<object>> property,
            PropertyChangedHandler<T> handler) where T : INotifyPropertyChanged{
            var info = objectThatNotifies.GetMemberInfo(property) as PropertyInfo;
            Subscribe(objectThatNotifies, info, handler);
        }

        public static void SubscribeToPropertyChange<T>(this T objectThatNotifies, Expression<Func<T, object>> property,
            PropertyChangedHandler<T> handler) where T : INotifyPropertyChanged{
            var info = objectThatNotifies.GetPropertyInfo(property);
            Subscribe(objectThatNotifies, info, handler);
        }

        private static void Subscribe<T>(T objectThatNotifies, PropertyInfo info, PropertyChangedHandler<T> handler)
            where T : INotifyPropertyChanged{
            objectThatNotifies.PropertyChanged += (s, e) => {
                if (e.PropertyName.Equals(info.Name))
                    handler(objectThatNotifies);
            };
        }
    }
}