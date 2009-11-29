using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGrid.Views.Base;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public class MoreSerializer : XmlXtraSerializer
    {
        public static void SaveFilter(ColumnView view, Stream stream, string[] propertyNames)
        {
            var serializer = new MoreSerializer();
            serializer.Serialize(stream, serializer.GetFilterProps(view, propertyNames), "View");
        }

        public static void LoadFilter(ColumnView view, Stream stream, string[] propertyNames)
        {
            view.RestoreLayoutFromStream(stream, null);
        }

        public static void LoadFilter(ColumnView view, string file, string[] propertyNames)
        {
            view.RestoreLayoutFromXml(file, null);
        }

        protected IXtraPropertyCollection GetFilterProps(ColumnView view, string[] propertyNames)
        {
            var store = new XtraPropertyInfoCollection();
            if (propertyNames.Length > 0)
            {
                var propList = new ArrayList();
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(view);
                foreach (string propertyName in propertyNames)
                    propList.Add(properties.Find(propertyName, false));

                var helper = new SerializeHelper();
                MethodInfo mi = typeof (SerializeHelper).GetMethod("SerializeProperty",
                                                                   BindingFlags.NonPublic | BindingFlags.Instance);
                (view as IXtraSerializable).OnStartSerializing();
                foreach (PropertyDescriptor prop in propList)
                {
                    mi.Invoke(helper, new object[] {store, view, prop, XtraSerializationFlags.None, null});
                }
                (view as IXtraSerializable).OnEndSerializing();
            }
            return store;
        }
    }
}