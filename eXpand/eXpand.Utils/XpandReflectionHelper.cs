using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace eXpand.Utils
{
    public class XpandReflectionHelper
    {
        static readonly Random random = new Random();
        public static void Shuffle<T>(IList<T> list)
        {
            list.OrderBy(arg => random.Next()).Take(list.Count());
        }
        /// <summary>
        /// searches a type for all properties that included in another type
        /// </summary>
        /// <param name="className">the type to be searched</param>
        /// <param name="interfaceType">all properties found must implemented at this type also</param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type className, Type interfaceType)
        {
            var retList = new ArrayList();
            PropertyInfo[] properties = className.GetProperties();
            #region interfacePropertiesList
            var interfacePropertiesList = new ArrayList();
            PropertyInfo[] interfaceProperties = interfaceType.GetProperties();
            foreach (PropertyInfo property in interfaceProperties)
            {
                interfacePropertiesList.Add(property.Name);
            }
            #endregion
            foreach (PropertyInfo property in properties)
            {
                if (interfacePropertiesList.Contains(property.Name))
                    retList.Add(property);
            }
            var propertyInfos = new PropertyInfo[retList.Count];
            retList.CopyTo(propertyInfos);
            return propertyInfos;
        }
        public static MemberInfo FindMethod(Type containerType, Type decorationAttributeType)
        {
            MethodInfo[] methods = containerType.GetMethods();
            foreach (MethodInfo method in methods)
            {
                object[] attributes = method.GetCustomAttributes(decorationAttributeType, false);
                if (attributes.Length > 0)
                    return method;
            }
            throw new MissingMethodException(containerType.Name, decorationAttributeType.Name);
        }

        public static object CreateGenerik(string name, params Type[] types)
        {
            name = Regex.Replace(name, @"`[\d]", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            string t = name + "`" + types.Length;
            Type generic = Type.GetType(t).MakeGenericType(types);
            return Activator.CreateInstance(generic);
        }

        public static PropertyInfo[] GetPropertiesAssignAbleFrom(Type className, Type assignAbleFrom)
        {
            var arrayList = new ArrayList();
            PropertyInfo[] propertyInfos = className.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
                if (assignAbleFrom.IsAssignableFrom(info.PropertyType))
                    arrayList.Add(info);

            var infos = new PropertyInfo[arrayList.Count];
            arrayList.CopyTo(infos);
            return infos;
        }

        public static IEnumerable<PropertyInfo> GetExplicitProperties(Type attributeType)
        {
            var explicitProperties = new List<PropertyInfo>();
            while (attributeType.BaseType!=typeof(object)) {
                explicitProperties.AddRange(from prop in attributeType.GetProperties(
                 BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                            let getAccessor = prop.GetGetMethod(true)
                                            where getAccessor.IsFinal && getAccessor.IsPrivate
                                            select prop);
                attributeType = attributeType.BaseType;
            }

            return explicitProperties;
        }

        public static object[] GetAttributes(PropertyInfo propertyInfo, Type attributeType) {
            var list = new ArrayList();
            object[] attributes = propertyInfo.GetCustomAttributes(attributeType, true);
            foreach (Attribute attribute in attributes) {
                if (attributeType.IsAssignableFrom(attribute.GetType()))
                    list.Add(attribute);
            }
            var retAttributes = new object[list.Count];
            list.CopyTo(retAttributes);
            return retAttributes;
        }

        public static Attribute GetAttribute(PropertyInfo propertyInfo, Type attributeType, bool inherit) {
            object[] attributes = GetAttributes(propertyInfo, attributeType);
            if (attributes.Length > 0)
                return (Attribute)attributes[0];
            return null;
        }

        public static Attribute GetAttribute(Type decoratedType, Type attributeType, bool inherit) {
            object[] attributes = decoratedType.GetCustomAttributes(attributeType, inherit);
            return getAttribute(attributes, attributeType);
        }

        public static Attribute GetAttribute(Type decoratedType, Type attributeType) {
            return GetAttribute(decoratedType, attributeType, false);
        }

        /// <summary>
        /// Searches inheritance to find an attribute of type <see cref="attributeType"/>
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static Attribute GetAttribute(PropertyInfo propertyInfo, Type attributeType) {
            object[] attributes = propertyInfo.GetCustomAttributes(attributeType, true);
            return getAttribute(attributes, attributeType);
        }

        static Attribute getAttribute(IEnumerable<object> attributes, Type attributeType) {
            return attributes.Cast<Attribute>().FirstOrDefault(attribute => attributeType.IsAssignableFrom(attribute.GetType()));
        }

        /// <summary>
        /// searches all interfaces of a type to find the one that is decoreated with a specific attribute
        /// </summary>
        /// <param name="objectType">The type to be search</param>
        /// <returns></returns>
        /// <param name="attributeType"></param>
        public static Type FindDecoratedInterface(Type objectType, Type attributeType) {
            Type[] interfaces = objectType.GetInterfaces();
            return (from type in interfaces
                    let attributes = type.GetCustomAttributes(attributeType, true)
                    where attributes.Length > 0
                    select type).FirstOrDefault();
        }

        public static Type[] GetTypes(Type assignAbleFrom, Assembly assembly, Type decoratedAttributeType) {
            var arrayList = new ArrayList();
            Type[] types = GetTypes(assignAbleFrom, assembly);
            foreach (Type type in types) {
                object[] attributes = type.GetCustomAttributes(decoratedAttributeType, false);
                if (attributes.Length > 0)
                    arrayList.Add(type);
            }
            types = new Type[arrayList.Count];
            arrayList.CopyTo(types);
            return types;
        }

        public static Type[] GetTypes(Type assignAbleFrom, Assembly assembly) {
            var arrayList = new ArrayList();
            foreach (Type type in assembly.GetTypes()) {
                if (assignAbleFrom.IsAssignableFrom(type))
                    arrayList.Add(type);
            }
            var types = new Type[arrayList.Count];
            arrayList.CopyTo(types);
            return types;
        }

        /// <summary>
        /// finds all properties decorated with the <see cref="attributeType"/> attribute 
        /// </summary>
        public static PropertyInfo[] GetDecoratedProperties(Type objectType, Type attributeType) {
            return GetDecoratedProperties(objectType, attributeType, BindingFlags.Default);
        }

        /// <summary>
        /// finds all properties decorated with the <see cref="attributeType"/> attribute 
        /// </summary>
        public static PropertyInfo[] GetDecoratedProperties(Type objectType, Type attributeType,
                                                            BindingFlags bindingFlags) {
            var arrayList = new ArrayList();
            PropertyInfo[] properties = objectType.GetProperties();
            foreach (PropertyInfo property in properties) {
                object[] attributes = property.GetCustomAttributes(attributeType, true);
                if (attributes.Length > 0)
                    arrayList.Add(property);
            }
            var propertyInfos = new PropertyInfo[arrayList.Count];
            arrayList.CopyTo(propertyInfos);
            return propertyInfos;
        }

        public static bool FindInterface(Type interfaceType, Type typeToSearch) {
            Type[] interfaces = typeToSearch.GetInterfaces();
            return interfaces.Any(interfaces.Equals);
        }

        public static void SetPropertyValue(PropertyInfo propertyInfo, object obj, object value) {
            propertyInfo.SetValue(obj, value, null);
        }

        /// <summary>
        /// searches <see cref="containerType"/> for all fields that are assignableFrom <see cref="assignableFrom"/>
        /// </summary>
        /// <param name="assignableFrom"></param>
        /// <param name="containerType"></param>
        public static PropertyInfo[] GetAssingAbleProperties(Type assignableFrom, Type containerType) {
            var list = new ArrayList();
            PropertyInfo[] propertyInfos = containerType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
                if (assignableFrom.IsAssignableFrom(info.PropertyType))
                    list.Add(info);

            return (PropertyInfo[])ArrayList.Adapter(list).ToArray(typeof(PropertyInfo));
        }

        public static FieldInfo[] GetFields(Type assignableFrom, Type containerType, BindingFlags bindingFlags) {
            var list = new ArrayList();
            FieldInfo[] fields = containerType.GetFields(bindingFlags);
            foreach (FieldInfo fieldInfo in fields) {
                if (assignableFrom.IsAssignableFrom(fieldInfo.FieldType))
                    list.Add(fieldInfo);
            }
            return (FieldInfo[])ArrayList.Adapter(list).ToArray(typeof(FieldInfo));
        }

        public static object[] GetFieldsValues(Type assignableFrom, object containerObject, bool excludeNullValues) {
            FieldInfo[] fieldInfos = GetFields(assignableFrom, containerObject.GetType());
            var list = new ArrayList();
            for (int i = 0; i < fieldInfos.Length; i++) {
                object value = fieldInfos[0].GetValue(containerObject);
                if (!excludeNullValues || value != null)
                    list[i] = value;
            }
            var objects = new object[list.Count];
            list.CopyTo(objects);
            return objects;
        }

        public static FieldInfo[] GetFields(Type assignableFrom, Type containerType) {
            return GetFields(assignableFrom, containerType, BindingFlags.Default);
        }

        public static void FilterProperties(ref PropertyInfo[] properties, bool readWrite) {
            var arrayList = new ArrayList();
            foreach (PropertyInfo property in properties) {
                if (readWrite) {
                    MethodInfo getMethod = property.GetGetMethod();
                    MethodInfo setMethod = property.GetSetMethod();
                    if (getMethod == null || setMethod == null)
                        continue;
                }
                arrayList.Add(property);
            }
            properties = new PropertyInfo[arrayList.Count];
            arrayList.CopyTo(properties);
        }

        public static void FilterTypes(ref Type[] types, Type[] excludedTypesAssignAbleFrom) {
            var arrayList = new ArrayList();
            foreach (Type type in types) {
                bool found = false;
                foreach (Type type1 in excludedTypesAssignAbleFrom) {
                    if (type1.IsAssignableFrom(type))
                        found = true;
                }
                if (!found)
                    arrayList.Add(type);
            }
            types = new Type[arrayList.Count];
            arrayList.CopyTo(types);
        }

        public static void FilterTypes(ref Type[] types, bool excludeAbstracts) {
            var arrayList = new ArrayList();
            foreach (Type type in types) {
                if (excludeAbstracts && type.IsAbstract)
                    continue;
                arrayList.Add(type);
            }
            types = new Type[arrayList.Count];
            arrayList.CopyTo(types);
        }

        public static MethodInfo[] GetMethods(Type type, Type decoratedAttribute, BindingFlags bindingFlags) {
            var arrayList = new ArrayList();
            MethodInfo[] methods = type.GetMethods(bindingFlags);
            foreach (MethodInfo info in methods) {
                object[] attributes = info.GetCustomAttributes(decoratedAttribute, true);
                if (attributes.Length > 0)
                    arrayList.Add(info);
            }
            var infos = new MethodInfo[arrayList.Count];
            arrayList.CopyTo(infos);
            return infos;
        }

        public static MethodInfo[] GetMethods(Type type, Type decoratedAttribute) {
            return GetMethods(type, decoratedAttribute, BindingFlags.Default);
        }

        public static object[] GetPropertiesValues(Type decoratedAttributeType, object container, bool excludeNullValues) {
            var arrayList = new ArrayList();
            PropertyInfo[] properties = GetDecoratedProperties(container.GetType(), decoratedAttributeType);
            foreach (PropertyInfo info in properties) {
                object value = info.GetValue(container, null);
                if (!excludeNullValues || value != null)
                    arrayList.Add(value);
            }
            var objects = new object[arrayList.Count];
            arrayList.CopyTo(objects);
            return objects;
        }

        public static FieldInfo GetFieldByValue(object value, object containerControl) {
            FieldInfo[] fieldInfos = containerControl.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            return (from fieldInfo in fieldInfos
                    let fieldValue = fieldInfo.GetValue(containerControl)
                    where fieldValue != null && fieldValue.Equals(value)
                    select fieldInfo).FirstOrDefault();
        }





        public static PropertyDescriptor GetPropertyDescriptorByValue(object value, object containedControl) {
            PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(containedControl);
            return propertyDescriptorCollection.Cast<PropertyDescriptor>().FirstOrDefault(descriptor => value.Equals(descriptor.GetValue(containedControl)));
        }

        public static MethodInfo[] GetDecoratedMethods(Type classType, Type attributeType) {
            MethodInfo[] methodInfos =
                classType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var arrayList = new ArrayList();
            foreach (MethodInfo methodInfo in methodInfos) {
                if (methodInfo.GetCustomAttributes(attributeType, true).Length > 0)
                    arrayList.Add(methodInfo);
            }
            methodInfos = new MethodInfo[arrayList.Count];
            arrayList.CopyTo(methodInfos);
            return methodInfos;
        }

        public static bool HasAttribute(PropertyInfo propertyInfo, Type attributeType) {
            return propertyInfo.GetCustomAttributes(attributeType, false).Length > 0;
        }
    }
}
