using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xpand.Utils.BackingFieldResolver;

namespace Xpand.Utils.Helpers {
    public static class ReflectionExtensions {

        public static bool IsDynamic(this Assembly assembly) {
            return assembly.ManifestModule.GetType().Namespace == "System.Reflection.Emit";
        }

        public static object Invoke(this Type type, object target, string methodName) {
            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, new ParameterModifier[0]);
            if (methodInfo == null) throw new NullReferenceException("methodInfo");
            return methodInfo.Invoke(target, new object[0]);
        }

        public static IEnumerable<Type> GetTypes(this AppDomain appdomain, string typeToFind) {
            var types = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                try {
                    types.AddRange(assembly.GetTypes().Where(type => type.Name == typeToFind));
                } catch (ReflectionTypeLoadException) {
                }
            }
            return types;

        }

        public static IEnumerable<Type> GetTypes(this AppDomain appDomain, Type typeToFind) {
            var types = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                try {
                    types.AddRange(assembly.GetTypes().Where(typeToFind.IsAssignableFrom));
                } catch (ReflectionTypeLoadException) {
                }
            }
            return types;
        }

        public static IEnumerable<Type> GetTypes(this AppDomain appDomain) {
            var types = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                try {
                    types.AddRange(assembly.GetTypes());
                } catch (ReflectionTypeLoadException) {
                }
            }

            return types;
        }

        public static MethodInfo GetMemberInfo<TTarget>(this TTarget target, Expression<Action<TTarget>> method) {
            return GetMemberInfo(method);
        }

        public static MethodInfo GetMemberInfo(Expression method) {
            if (method == null) throw new ArgumentNullException("method");

            var lambda = method as LambdaExpression;
            if (lambda == null) throw new ArgumentException("Not a lambda expression", "method");
            if (lambda.Body.NodeType != ExpressionType.Call) throw new ArgumentException("Not a method call", "method");

            return ((MethodCallExpression)lambda.Body).Method;
        }

        public static MemberInfo GetExpression(LambdaExpression lambda) {

            if (lambda == null) throw new ArgumentException("Not a lambda expression", "lambda");

            MemberExpression memberExpr = null;


            if (lambda.Body.NodeType == ExpressionType.Convert)
                memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
                memberExpr = lambda.Body as MemberExpression;

            if (memberExpr == null) throw new ArgumentException("Not a member access", "lambda");

            return memberExpr.Member;
        }

        public static string GetPropertyName<TTarget>(Expression<Func<TTarget, object>> property) {
            return GetPropertyName<TTarget, object>(property);
        }

        public static string GetPropertyName<TTarget, TObject>(Expression<Func<TTarget, TObject>> property)  {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression != null) return memberExpression.Member.Name;
            var unaryExpression = property.Body as UnaryExpression;
            if (unaryExpression != null) {
                var expression = unaryExpression.Operand as MemberExpression;
                if (expression != null) return expression.Member.Name;
            }
            throw new NotImplementedException();
        }

        public static string GetPropertyName<TTarget>(this TTarget target, Expression<Func<TTarget, object>> property) {
            return GetPropertyInfo(target, property).Name;
        }

        public static void SetPropertyInfoBackingFieldValue<TTarget>(this TTarget target,Expression<Func<TTarget, object>> property,object obj,object value) {
            var propertyInfo = GetPropertyInfo(target, property);
            FieldInfo backingField = propertyInfo.GetBackingField();
            backingField.SetValue(obj, value);
        }

        public static PropertyInfo GetPropertyInfo<TTarget>(this TTarget target, Expression<Func<TTarget, object>> property) {
            var info = target.GetMemberInfo(property) as PropertyInfo;
            if (info == null) throw new ArgumentException("Member is not a property");

            return info;
        }


        public static FieldInfo GetFieldInfo<TTarget>(this TTarget target, Expression<Func<TTarget, object>> field) {
            var info = target.GetMemberInfo(field) as FieldInfo;
            if (info == null) throw new ArgumentException("Member is not a field");

            return info;
        }

        public static MemberInfo GetMemberInfo<TTarget>(this TTarget target, Expression member) {
            if (member == null) throw new ArgumentNullException("member");
            var lambda = member as LambdaExpression;
            return GetExpression(lambda);
        }

        public static void SetProperty<T>(this INotifyPropertyChanged source,
                                          Expression<Func<T>> propExpr,
                                          ref T propertyValueHolder,
                                          T value, Action doIfChanged) where T : class {
            var prop = (PropertyInfo)((MemberExpression)propExpr.Body).Member;
            var currVal = (T)prop.GetValue(source, null);
            if (currVal == null && value == null)
                return;
            if (currVal == null || !currVal.Equals(value)) {
                propertyValueHolder = value;
                var fieldInfo = source.GetType().GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo != null) {
                    var eventDelegate = (MulticastDelegate)fieldInfo.GetValue(source);
                    if (eventDelegate != null) {
                        Delegate[] delegates = eventDelegate.GetInvocationList();
                        var args = new PropertyChangedEventArgs(prop.Name);
                        foreach (Delegate dlg in delegates)
                            dlg.Method.Invoke(dlg.Target, new object[] { source, args });
                    }
                }
                doIfChanged();
            }
        }

        public static void SetProperty<T>(this INotifyPropertyChanged source,
                                          Expression<Func<T>> propExpr,
                                          ref T propertyValueHolder,
                                          T value) where T : class {
            source.SetProperty(propExpr, ref propertyValueHolder, value, () => { });
        }

        public static object CreateGeneric(this Type generic, Type innerType, params object[] args) {
            Type specificType = generic.MakeGenericType(new[] { innerType });
            return Activator.CreateInstance(specificType, args);
        }

        public static bool IsNullableType(this Type theType) {
            if (theType.IsGenericType) {
                var genericTypeDefinition = theType.GetGenericTypeDefinition();
                if (genericTypeDefinition != null) return (genericTypeDefinition == typeof(Nullable<>));
            }
            return false;
        }
    }
}