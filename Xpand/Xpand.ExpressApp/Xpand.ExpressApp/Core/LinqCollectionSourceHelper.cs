using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Attributes;

namespace Xpand.ExpressApp.Core
{
    public static class LinqCollectionSourceHelper
    {
        public static void CreateCustomCollectionSource(object sender, CreateCustomCollectionSourceEventArgs e)
        {
            var listViewInfo = ((XafApplication)sender).FindModelView(e.ListViewID) as IModelListViewLinq;
            if (listViewInfo == null) return;
            if (string.IsNullOrEmpty(listViewInfo.XPQueryMethod)) return;
            IQueryable query = InvokeMethod(e.ObjectType, listViewInfo.XPQueryMethod, e.ObjectSpace.Session);
            if (query == null) return;
            e.CollectionSource = new LinqCollectionSource(e.ObjectSpace, e.ObjectType, query);
        }
        public static string[] GetXPQueryMethods(Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            return (from mi in methods where IsCompatibleMethod(mi) select mi.Name).ToArray();
        }
        public static bool IsCompatibleMethod(MethodInfo mi)
        {
            ParameterInfo[] pis = mi.GetParameters();
            return mi.ReturnType != null && typeof(IQueryable).IsAssignableFrom(mi.ReturnType)
                    && pis.Length == 1 && pis[0].ParameterType.IsAssignableFrom(typeof(Session));
        }
        public static IQueryable InvokeMethod(Type type, string name, Session session)
        {
            MethodInfo method = FindMethod(type, name);
            if (method == null) return null;
            return (IQueryable)method.Invoke(null, new object[] { session });
        }
        private static MethodInfo FindMethod(Type type, string name)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            return methods.FirstOrDefault(mi => mi.Name == name && IsCompatibleMethod(mi));
        }
        public static string[] GetDisplayableProperties(Type type, string name)
        {
            MethodInfo method = FindMethod(type, name);
            if (method == null) return null;
            return (from CustomQueryPropertiesAttribute attribute in method.GetCustomAttributes(typeof (CustomQueryPropertiesAttribute), false)
                    where attribute.Name == "DisplayableProperties"
                    select attribute.Value.Split(';')).FirstOrDefault();
        }
    }
}