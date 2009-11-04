using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using eXpand.Utils.BackingFieldResolver;

namespace eXpand.Utils.ExpressionBuilder {
    public abstract class Builder<T> : IBuilder<T>
    {
        private Dictionary<PropertyInfo, Object> PropertiesAndValues
        { get; set; }

        protected Builder()
        {
            PropertiesAndValues =
                new Dictionary<PropertyInfo, Object>();
        }

        private static PropertyInfo GetProperty(Expression<Func<T, Object>> expression)
        {
            

            MemberExpression memberExpression;
            if (ExpressionType.Convert == expression.Body.NodeType)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (null == memberExpression)
            {
                throw new InvalidOperationException("InvalidMemberExpression");
            }

            return memberExpression.Member as PropertyInfo;
        }

        public static implicit operator T(Builder<T> builder)
        {
            return builder.Build();
        }

        protected void ProvideValueFor(Expression<Func<T, Object>> expression,
                                       Object value)
        {
            
            var property = GetProperty(expression);

            if (false == PropertiesAndValues.ContainsKey(property))
                RegisterPropertyAndValue(property, value);
            else
                SetPropertyAndValue(property, value);
        }

        private void SetPropertyAndValue(PropertyInfo property,
                                         Object value)
        {
            if (IsCollection(property))
            {
                var values = (List<Object>)PropertiesAndValues[property];
                values.Add(value);
            }
            else
            {
                PropertiesAndValues[property] = value;
            }
        }

        private void RegisterPropertyAndValue(PropertyInfo property,
                                              Object value)
        {
            if (IsCollection(property))
                PropertiesAndValues.Add(property,
                                        new List<Object> { value });
            else
                PropertiesAndValues.Add(property, value);
        }

        private static Boolean IsCollection(PropertyInfo property)
        {
            if (property.PropertyType == typeof(String))
                return false;

            var collectionType = typeof(IEnumerable<>);
            return IsCollectionOfType(collectionType,
                                      property.PropertyType);
        }

        private static Boolean IsCollection(FieldInfo field)
        {
            var collectionType = typeof(ICollection<>);
            return IsCollectionOfType(collectionType, field.FieldType);
        }

        private static Boolean IsCollectionOfType(Type collectionType,
                                                  Type type)
        {
            if (collectionType.Name == type.Name)
                return true;

            var interfaces = type.GetInterfaces();
            return interfaces.Where(@interface =>@interface.Name == collectionType.Name).FirstOrDefault()!= null;
        }

        public T Build()
        {
            var typeToBuild = typeof(T);
            if (false == HasParameterlessConstructor(typeToBuild))
                throw new InvalidOperationException(
                    "No parameterless constructor.");

            var instance = (T)Activator.CreateInstance(typeToBuild, true);
            foreach (var entry in PropertiesAndValues)
            {
                var property = entry.Key;
                if (IsCollection(property))
                    SetCollectionValuesFor(property, instance,
                                           (List<Object>)entry.Value);
                else
                    SetValueFor(property, instance, entry.Value);
            }

            return instance;
        }

        private static Boolean HasParameterlessConstructor(Type type)
        {
            const BindingFlags bindingFlags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance;

            var defaultConstructor =
                type.GetConstructor(bindingFlags, null,
                                    new Type[0], null);
            return null != defaultConstructor;
        }

        private static void SetValueFor(PropertyInfo property, T instance,
                                        Object value)
        {
            property.SetValue(instance, value, null);
        }

        private static void SetCollectionValuesFor(PropertyInfo property,
                                                   T instance,
                                                   List<Object> values)
        {
            var backingField = property.GetBackingField();
            if (false == IsCollection(backingField)) {
                throw new InvalidOperationException("InvalidCollectionType");
            }

            var collection = property.GetValue(instance, null);
            foreach (var value in values)
            {
                const BindingFlags bindingFlags =
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.InvokeMethod;

                backingField.FieldType
                    .InvokeMember("Add", bindingFlags, null,
                                  collection, new[] { value });
            }
        }
    }
}