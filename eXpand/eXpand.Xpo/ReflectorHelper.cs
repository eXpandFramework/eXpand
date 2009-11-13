using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using DevExpress.Xpo;
using DevExpress.Xpo.Exceptions;
using DevExpress.Xpo.Metadata;
using eXpand.Utils.Helpers;

namespace eXpand.Xpo
{
    /// <summary>
    /// Summary description for Reflector.
    /// </summary>
    public class ReflectorHelper {
        private ReflectorHelper() {
        }

        public static object CreateGenerik(string name, params Type[] types) {
            name = Regex.Replace(name, @"`[\d]", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            string t = name + "`" + types.Length;
            Type generic = Type.GetType(t).MakeGenericType(types);
            return Activator.CreateInstance(generic);
        }

        public static PropertyInfo[] GetPropertiesAssignAbleFrom(Type className, Type assignAbleFrom) {
            var arrayList = new ArrayList();
            PropertyInfo[] propertyInfos = className.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
                if (assignAbleFrom.IsAssignableFrom(info.PropertyType))
                    arrayList.Add(info);

            var infos = new PropertyInfo[arrayList.Count];
            arrayList.CopyTo(infos);
            return infos;
        }

        //		private static PropertyInfo[] getProperties(Type type)
        //		{
        //			if (type.BaseType!= null)
        //				
        //		}

        /// <summary>
        /// searches a type for all properties that included in another type
        /// </summary>
        /// <param name="className">the type to be searched</param>
        /// <param name="interfaceType">all properties found must implemented at this type also</param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type className, Type interfaceType) {
            var retList = new ArrayList();
            PropertyInfo[] properties = className.GetProperties();
            #region interfacePropertiesList
            var interfacePropertiesList = new ArrayList();
            PropertyInfo[] interfaceProperties = interfaceType.GetProperties();
            foreach (PropertyInfo property in interfaceProperties) {
                interfacePropertiesList.Add(property.Name);
            }
            #endregion
            foreach (PropertyInfo property in properties) {
                if (interfacePropertiesList.Contains(property.Name))
                    retList.Add(property);
            }
            var propertyInfos = new PropertyInfo[retList.Count];
            retList.CopyTo(propertyInfos);
            return propertyInfos;
        }

        public static ArrayList GetPropertyNames(Type type) {
            var list = new ArrayList(type.GetProperties());
            foreach (PropertyInfo property in type.GetProperties())
                list.Add(property.Name);
            return list;
        }

        public static MemberInfo FindMethod(Type containerType, Type decorationAttributeType) {
            MethodInfo[] methods = containerType.GetMethods();
            foreach (MethodInfo method in methods) {
                object[] attributes = method.GetCustomAttributes(decorationAttributeType, false);
                if (attributes.Length > 0)
                    return method;
            }
            throw new MissingMethodException(containerType.Name, decorationAttributeType.Name);
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

        private static Attribute getAttribute(object[] attributes, Type attributeType) {
            foreach (Attribute attribute in attributes) {
                if (attributeType.IsAssignableFrom(attribute.GetType()))
                    return attribute;
            }
            return null;
        }

        /// <summary>
        /// searches all interfaces of a type to find the one that is decoreated with a specific attribute
        /// </summary>
        /// <param name="objectType">The type to be search</param>
        /// <returns></returns>
        /// <param name="attributeType"></param>
        public static Type FindDecoratedInterface(Type objectType, Type attributeType) {
            Type[] interfaces = objectType.GetInterfaces();
            foreach (Type type in interfaces) {
                object[] attributes = type.GetCustomAttributes(attributeType, true);
                if (attributes.Length > 0)
                    return type;
            }
            return null;
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

        //		[Test]
        //		public void MethodName()
        //		{
        //			PropertyInfo property = typeof(Logout).GetProperty("ProfileUrl");
        //			PropertyInfo[] properties = GetDecoratedProperties(typeof(Logout),typeof(VirtualPathFixerAttribute));
        //			Assert.AreEqual(2, properties.Length);
        //		}

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
            foreach (Type type in interfaces) {
                if (interfaces.Equals(type))
                    return true;
            }
            return false;
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
            foreach (FieldInfo fieldInfo in fieldInfos) {
                object fieldValue = fieldInfo.GetValue(containerControl);
                if (fieldValue != null && fieldValue.Equals(value)) return fieldInfo;
            }
            return null;
        }

        /// <summary>
        /// uses <see cref="Convert.ChangeType(object,Type)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <param name="conversionType"></param>
        public static object ChangeType(object value, Type conversionType) {
            if (value == DBNull.Value)
                value = null;
            if (value == null || value.Equals("")) {
                if (conversionType == typeof(DateTime))
                    return typeof(Nullable).IsAssignableFrom(conversionType) ? (object)null : DateTime.MinValue;
                if (conversionType == typeof(int) || conversionType == typeof(double))
                    return typeof(Nullable).IsAssignableFrom(conversionType) ? (object)null : 0;
                if (conversionType == typeof(bool))
                    return typeof(Nullable).IsAssignableFrom(conversionType) ? (object)null : false;
                if (conversionType.IsValueType)
                    throw new NotImplementedException(conversionType.Name);
            } else if (typeof(Enum).IsAssignableFrom(conversionType))
                return Enum.Parse(conversionType, (string)value);
            else if (StringHelper.IsGuid(value + "") && conversionType == typeof (Guid))
                return new Guid(value.ToString());
            else if (value.GetType().Equals(conversionType))
                return value;
            else if (typeof (XPBaseObject).IsAssignableFrom(value.GetType()))
            {
                if (conversionType == typeof (int))
                    return ((XPBaseObject) value).ClassInfo.KeyProperty.GetValue(value);
                if (conversionType == typeof (string))
                    return ((XPBaseObject) value).ClassInfo.KeyProperty.GetValue(value).ToString();
                return value;
            }
            else if (value.GetType() != conversionType)
            {
                if (conversionType.IsGenericType)
                {
                    return value;
                    //                    Type nullValue=typeof (Nullable<>).MakeGenericType(new Type[] {value.GetType()});
                    //                    nullValue.GetProperty("Value").SetValue();
                    //                    conversionType.GetProperty("Value").SetValue(nullValue, value, null);
                }
            }
            return Convert.ChangeType(value, conversionType);
        }

        public static object GetXpMemberInfoValue(string propertyName, XPBaseObject o) {
            if (propertyName.IndexOf(".") > -1) {
                XPMemberInfo info = o.ClassInfo.GetMember(propertyName.Split(".".ToCharArray())[0]);
                object value = info.GetValue(o);
                if (typeof(XPBaseObject).IsAssignableFrom(info.MemberType)) {
                    o = value as XPBaseObject;
                    return o == null
                               ? null
                               : GetXpMemberInfoValue(propertyName.Substring(propertyName.IndexOf(".") + 1), o);
                }
                return
                    value != null
                        ? GetPropertyInfoValue(propertyName.Substring(propertyName.IndexOf(".") + 1), info.GetValue(o))
                        : null;
            }
            XPMemberInfo xpMemberInfo = o.ClassInfo.GetMember(propertyName);
            if (xpMemberInfo == null)
                throw new PropertyMissingException(o.GetType().FullName, propertyName);
            return xpMemberInfo.GetValue(o);
        }

        public static void SetXpMemberProperty(string propertyName, object value, XPBaseObject dbObject, bool save) {
            if (propertyName.IndexOf(".") > -1) {
                XPMemberInfo member = dbObject.ClassInfo.GetMember(propertyName.Split(".".ToCharArray())[0]);
                object o = member.GetValue(dbObject);
                if (typeof(XPBaseObject).IsAssignableFrom(member.MemberType)) {
                    dbObject = o as XPBaseObject;
                    SetXpMemberProperty(propertyName.Substring(propertyName.IndexOf(".") + 1), value, dbObject, save);
                    return;
                }
                SetPropertyValue(o.GetType().GetProperty(propertyName.Substring(propertyName.IndexOf(".") + 1)), o,
                                 value);
                return;
            }
            XPMemberInfo xpMemberInfo = dbObject.ClassInfo.GetMember(propertyName);
            if (xpMemberInfo == null)
                throw new PropertyMissingException(dbObject.GetType().FullName, propertyName);
            xpMemberInfo.SetValue(dbObject,
                                  xpMemberInfo.Owner.ClassType.GetProperty(propertyName) == null
                                      ? value
                                      : ChangeType(value, xpMemberInfo.MemberType));
            if (save)
                dbObject.Save();

        }
        public static void SetXpMemberProperty(string propertyName, object value, XPBaseObject dbObject) {
            SetXpMemberProperty(propertyName, value, dbObject, false);
        }

        public static XPMemberInfo GetXpMemberInfo(XPClassInfo xpClassInfo, string propertyName, bool throwIfMissing) {
            if (propertyName.IndexOf(".") > -1) {
                XPMemberInfo info = xpClassInfo.FindMember(propertyName.Split(".".ToCharArray())[0]);
                if (info != null) {
                    XPClassInfo type = info.ReferenceType;
                    if (info.IsAssociation && info.IsCollection && info.CollectionElementType != null)
                        type = info.CollectionElementType;
                    return GetXpMemberInfo(type, propertyName.Substring(propertyName.IndexOf(".") + 1), throwIfMissing);
                }
                return null;
            }
            XPMemberInfo xpMemberInfo = xpClassInfo.FindMember(propertyName);
            if (xpMemberInfo == null && throwIfMissing)
                throw new PropertyMissingException(xpClassInfo.FullName, propertyName);
            return xpMemberInfo;
        }


        public static XPMemberInfo GetXpMemberInfo(Session session, Type type, string propertyName, bool throwIfMissing) {
            XPClassInfo xpClassInfo = session.GetClassInfo(type);
            return GetXpMemberInfo(xpClassInfo, propertyName, throwIfMissing);

        }
        public static XPMemberInfo GetXpMemberInfo(XPClassInfo xpClassInfo, string propertyName) {
            return GetXpMemberInfo(xpClassInfo, propertyName, false);
        }
        public static XPMemberInfo GetXpMemberInfo(Session session, Type type, string propertyName) {
            return GetXpMemberInfo(session, type, propertyName, false);
        }

        public static PropertyInfo GetPropertyInfo(Type type, string propertyName) {
            if (propertyName.IndexOf(".") > -1) {
                PropertyInfo info = type.GetProperty(propertyName.Split(".".ToCharArray())[0]);
                return GetPropertyInfo(info.PropertyType, propertyName.Substring(propertyName.IndexOf(".") + 1));
            }
            PropertyInfo propertyInfo = type.GetProperty(propertyName);


            if (propertyInfo == null)
                throw new PropertyMissingException(type.FullName, propertyName);
            return propertyInfo;
        }

        public static void SetProperty(string propertyName, object value, object o) {
            if (propertyName.IndexOf(".") > -1) {
                o = o.GetType().GetProperty(propertyName.Split(".".ToCharArray())[0]).GetValue(o, null);
                SetProperty(propertyName.Substring(propertyName.IndexOf(".") + 1), value, o);
                return;
            }
            PropertyInfo propertyInfo = o.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                throw new PropertyMissingException(o.GetType().FullName, propertyName);
            propertyInfo.SetValue(o, ChangeType(value, propertyInfo.PropertyType), null);
        }

        public static object GetPropertyInfoValue(string propertyName, object o, bool returnNullIfPropertyNotExists) {
            if (propertyName.IndexOf(".") > -1) {
                PropertyInfo info = o.GetType().GetProperty(propertyName.Split(".".ToCharArray())[0]);
                if (returnNullIfPropertyNotExists && info == null)
                    return null;
                o = info.GetValue(o, null);

                return o == null ? null : GetPropertyInfoValue(propertyName.Substring(propertyName.IndexOf(".") + 1), o);
            }
            PropertyInfo propertyInfo = o.GetType().GetProperty(propertyName);
            if (propertyInfo == null && !returnNullIfPropertyNotExists)
                throw new PropertyMissingException(o.GetType().FullName, propertyName);
            if (propertyInfo == null)
                return null;
            return propertyInfo.GetIndexParameters().Length == 0 ? propertyInfo.GetValue(o, null) : null;
        }

        public static object GetPropertyInfoValue(string propertyName, object o) {
            return GetPropertyInfoValue(propertyName, o, false);
        }

        public static XPMemberInfo[] GetXpMemberInfos(XPClassInfo classInfo) {
            PropertyInfo[] propertyInfos = classInfo.ClassType.GetProperties();
            var memberInfos = new XPMemberInfo[propertyInfos.Length];
            int i = 0;
            foreach (PropertyInfo propertyInfo in propertyInfos) {
                XPMemberInfo memberInfo = classInfo.GetMember(propertyInfo.Name);
                if (memberInfo != null)
                    memberInfos[i] = memberInfo;
                i++;
            }
            return memberInfos;
        }

        public static PropertyDescriptor GetPropertyDescriptorByValue(object value, object containedControl) {
            PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(containedControl);
            foreach (PropertyDescriptor descriptor in propertyDescriptorCollection) {
                if (value.Equals(descriptor.GetValue(containedControl)))
                    return descriptor;
            }
            return null;
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