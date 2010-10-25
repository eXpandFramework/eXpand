using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using DevExpress.Xpo;
using DevExpress.Xpo.Exceptions;
using DevExpress.Xpo.Metadata;
using Xpand.Utils.Helpers;

namespace Xpand.Xpo {
    /// <summary>
    /// Summary description for Reflector.
    /// </summary>
    public class XpandReflectionHelper : Utils.XpandReflectionHelper {
        private XpandReflectionHelper() {
        }

        public static object ChangeType(object value, Type conversionType, CultureInfo cultureInfo) {
            if (value == DBNull.Value)
                value = null;
            if (value == null || value.Equals("")) {
                if (conversionType == typeof(DateTime))
                    return typeof(Nullable).IsAssignableFrom(conversionType) ? (object)null : DateTime.MinValue;
                if (conversionType == typeof(int) || conversionType == typeof(double))
                    return typeof(Nullable).IsAssignableFrom(conversionType) ? (object)null : 0;
                if (conversionType == typeof(bool))
                    return typeof(Nullable).IsAssignableFrom(conversionType) ? (object)null : false;
                if (typeof(IEnumerable).IsAssignableFrom(conversionType) && string.IsNullOrEmpty(value + ""))
                    return null;
                if (conversionType.IsValueType)
                    return Activator.CreateInstance(conversionType);
            } else if (typeof(Enum).IsAssignableFrom(conversionType))
                return Enum.Parse(conversionType, (string)value);
            else if ((value + "").IsGuid() && conversionType == typeof(Guid))
                return new Guid(value.ToString());
            else if (value.GetType().Equals(conversionType))
                return value;
            else if (typeof(XPBaseObject).IsAssignableFrom(value.GetType())) {
                if (conversionType == typeof(int))
                    return ((XPBaseObject)value).ClassInfo.KeyProperty.GetValue(value);
                if (conversionType == typeof(string))
                    return ((XPBaseObject)value).ClassInfo.KeyProperty.GetValue(value).ToString();
                return value;
            } else if (conversionType == typeof(DateTime)) {
                if ((value + "").Length > 0) {
                    var val = (value + "").Val();
                    if (val > 0)
                        return new DateTime(val);
                }
            } else if (value.GetType() != conversionType) {
                if (conversionType.IsGenericType) {
                    return value;
                }
            }

            return Convert.ChangeType(value, conversionType, cultureInfo);

        }

        public static object ChangeType(object value, Type conversionType) {
            return ChangeType(value, conversionType, null);
        }

        /// <summary>
        /// uses <see cref="Convert.ChangeType(object,Type)"/>
        /// </summary>
        /// <returns></returns>
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

    }
}