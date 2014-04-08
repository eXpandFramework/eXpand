using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace Xpand.Utils.Fastreflect{
    public static class PropertyExtensions{
        public static bool IsReservedName(this string name){
            name = name.ToLowerInvariant();
            if (name != ".ctor")
                return name == ".cctor";
            return true;
        }

        public static string TrimExplicitlyImplementedName(this string name){
            int startIndex = IsReservedName(name) ? -1 : name.LastIndexOf('.') + 1;
            if (startIndex <= 0)
                return name;
            return name.Substring(startIndex);
        }

        private static bool ContainsOverride<T>(this IEnumerable<T> candidates, MethodInfo method) where T : MemberInfo{
            if (!method.IsVirtual)
                return false;
            IList<ParameterInfo> list = method.Parameters();
            return candidates.Select(t => (object) t as MethodInfo).Any(methodInfo => !(methodInfo == null) && methodInfo.IsVirtual && method.Name == methodInfo.Name && list.Select(p => p.ParameterType).SequenceEqual(methodInfo.Parameters().Select(p => p.ParameterType)));
        }

        /// <summary>
        ///     This method applies flags-based filtering to a set of members.
        /// </summary>
        public static IList<T> Filter<T>(this IList<T> members, Flags bindingFlags) where T : MemberInfo{
            var list1 = new List<T>(members.Count);
            var list2 = new List<string>(members.Count);
            bool flag1 = bindingFlags.IsSet(Flags.ExcludeHiddenMembers);
            bool flag2 = bindingFlags.IsSet(Flags.ExcludeBackingMembers);
            bool flag3 = bindingFlags.IsSet(Flags.ExcludeExplicitlyImplemented);
            foreach (T member in members){
                bool flag4 = false;
                if (flag1){
                    var method = (object) member as MethodBase;
                    T member1 = member;
                    flag4 = ((((0) | (!(method == null) ? 0 : (list1.Any(m => m.Name == member1.Name) ? 1 : 0))) !=
                              0
                        ? 1
                        : 0) |
                             (!(method != null)
                                 ? 0
                                 : (list1.Where(m => (object) m is MethodBase).Cast<MethodBase>().Any(m =>{
                                     if (m.Name == member.Name)
                                         return m.HasParameterSignature(method.GetParameters());
                                     return false;
                                 })
                                     ? 1
                                     : 0))) != 0;
                }
                if (!flag4 && flag2){
                    flag4 = ((0) | (!((object) member is FieldInfo) ? 0 : ((int) member.Name[0] == 60 ? 1 : 0))) !=
                            0;
                    var method = (object) member as MethodInfo;
                    if (method != null)
                        flag4 = ((flag4 ? 1 : 0) |
                                 (member.Name.Length <= 4 ? 0 : (member.Name.Substring(1, 3) == "et_" ? 1 : 0))) != 0 |
                                ContainsOverride(list1, method);
                    var propertyInfo = (object) member as PropertyInfo;
                    if (propertyInfo != null){
                        MethodInfo getMethod = propertyInfo.GetGetMethod(true);
                        flag4 = ((flag4 ? 1 : 0) | (!getMethod.IsVirtual ? 0 : (list2.Contains(propertyInfo.Name) ? 1 : 0))) !=
                                0;
                        if (!flag4)
                            list2.Add(propertyInfo.Name);
                    }
                }
                if (((flag4 ? 1 : 0) | (!flag3 || !member.Name.Contains(".") ? 0 : (!IsReservedName(member.Name) ? 1 : 0))) ==
                    0)
                    list1.Add(member);
            }
            return list1;
        }

        public static PropertyInfo PropertyX(this Type type, string name, Flags bindingFlags){
            // we need to check all properties to do partial name matches
            if (bindingFlags.IsAnySet(Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented)){
                return type.Properties(bindingFlags, name).FirstOrDefault();
            }

            PropertyInfo result = type.GetProperty(name, bindingFlags | Flags.DeclaredOnly);
            if (result == null && bindingFlags.IsNotSet(Flags.DeclaredOnly)){
                if (type.BaseType != typeof (object) && type.BaseType != null){
                    return type.BaseType.PropertyX(name, bindingFlags);
                }
            }
            bool hasSpecialFlags = bindingFlags.IsSet(Flags.ExcludeExplicitlyImplemented);
            if (hasSpecialFlags){
                IList<PropertyInfo> properties = new List<PropertyInfo>{result};
                properties = properties.Filter(bindingFlags);
                return properties.Count > 0 ? properties[0] : null;
            }
            return result;
        }
    }
}