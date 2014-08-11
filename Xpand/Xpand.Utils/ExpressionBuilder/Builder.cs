using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Fasterflect;
using Xpand.Utils.BackingFieldResolver;

namespace Xpand.Utils.ExpressionBuilder {
    public abstract class Builder<T> : IBuilder<T> {
        protected Builder() {
            PropertiesAndValues =
                new Dictionary<PropertyInfo, Object>();
        }

        Dictionary<PropertyInfo, Object> PropertiesAndValues { get; set; }
        #region IBuilder<T> Members
        public T Build() {
            Type typeToBuild = typeof (T);
            if (false == HasParameterlessConstructor(typeToBuild))
                throw new InvalidOperationException(
                    "No parameterless constructor.");

            var instance = (T)typeToBuild.CreateInstance(true);
            foreach (var entry in PropertiesAndValues) {
                PropertyInfo property = entry.Key;
                if (IsCollection(property))
                    SetCollectionValuesFor(property, instance,
                                           (List<Object>) entry.Value);
                else
                    SetValueFor(property, instance, entry.Value);
            }

            return instance;
        }
        #endregion
        static PropertyInfo GetProperty(Expression<Func<T, Object>> expression) {
            MemberExpression memberExpression;
            if (ExpressionType.Convert == expression.Body.NodeType) {
                var body = (UnaryExpression) expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else {
                memberExpression = expression.Body as MemberExpression;
            }

            if (null == memberExpression) {
                throw new InvalidOperationException("InvalidMemberExpression");
            }

            return memberExpression.Member as PropertyInfo;
        }

        public static implicit operator T(Builder<T> builder) {
            return builder.Build();
        }

        protected void ProvideValueFor(Expression<Func<T, Object>> expression,
                                       Object value) {
            PropertyInfo property = GetProperty(expression);

            if (false == PropertiesAndValues.ContainsKey(property))
                RegisterPropertyAndValue(property, value);
            else
                SetPropertyAndValue(property, value);
        }

        void SetPropertyAndValue(PropertyInfo property,
                                 Object value) {
            if (IsCollection(property)) {
                var values = (List<Object>) PropertiesAndValues[property];
                values.Add(value);
            }
            else {
                PropertiesAndValues[property] = value;
            }
        }

        void RegisterPropertyAndValue(PropertyInfo property,
                                      Object value) {
            PropertiesAndValues.Add(property,IsCollection(property) ? new List<Object>{value} : value);
        }

        static Boolean IsCollection(PropertyInfo property) {
            if (property.PropertyType == typeof (String))
                return false;

            Type collectionType = typeof (IEnumerable<>);
            return IsCollectionOfType(collectionType,
                                      property.PropertyType);
        }

        static Boolean IsCollection(FieldInfo field) {
            Type collectionType = typeof (ICollection<>);
            return IsCollectionOfType(collectionType, field.FieldType);
        }

        static Boolean IsCollectionOfType(Type collectionType,
                                          Type type) {
            if (collectionType.Name == type.Name)
                return true;

            Type[] interfaces = type.GetInterfaces();
            return interfaces.FirstOrDefault(@interface => @interface.Name == collectionType.Name) != null;
        }

        static Boolean HasParameterlessConstructor(Type type) {
            const BindingFlags bindingFlags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance;

            ConstructorInfo defaultConstructor =
                type.GetConstructor(bindingFlags, null,
                                    new Type[0], null);
            return null != defaultConstructor;
        }

        static void SetValueFor(PropertyInfo property, T instance,
                                Object value) {
            property.SetValue(instance, value, null);
        }

        static void SetCollectionValuesFor(PropertyInfo property,
                                           T instance,
                                           IEnumerable<object> values) {
            FieldInfo backingField = property.GetBackingField();
            if (false == IsCollection(backingField)) {
                throw new InvalidOperationException("InvalidCollectionType");
            }

            object collection = property.GetValue(instance, null);
            foreach (object value in values) {
                const BindingFlags bindingFlags =
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.InvokeMethod;

                backingField.FieldType
                    .InvokeMember("Add", bindingFlags, null,
                                  collection, new[] {value});
            }
        }
    }
}