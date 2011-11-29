using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.ExpressApp.IO.Core {
    public class TypesInfo {
        static IValueManager<TypesInfo> instanceManager;

        internal TypesInfo() {
        }

        public static TypesInfo Instance {
            get {
                if (instanceManager == null) {
                    instanceManager = ValueManager.GetValueManager<TypesInfo>("IO_TypesInfo");
                }
                return instanceManager.Value ?? (instanceManager.Value = new TypesInfo());
            }
        }


        public Type ClassInfoGraphNodeType { get; private set; }
        public Type SerializationConfigurationType { get; private set; }
        public Type SerializationConfigurationGroupType { get; private set; }
        public Type XmlFileChooserType { get; private set; }

        public void RegisterTypes(IList<Type> types) {


            SerializationConfigurationGroupType = GetInfoType(types, typeof(ISerializationConfigurationGroup));
            SerializationConfigurationType = GetInfoType(types, typeof(ISerializationConfiguration));
            ClassInfoGraphNodeType = GetInfoType(types, typeof(IClassInfoGraphNode));
            XmlFileChooserType = GetInfoType(types, typeof(IXmlFileChooser));
        }

        Type GetInfoType(IEnumerable<Type> types, Type type1) {
            Type infoType =
                types.Where(type1.IsAssignableFrom).GroupBy(type => type).Select(grouping => grouping.Key)
                    .FirstOrDefault();
            if (infoType == null)
                throw new NoNullAllowedException("No Class that implemenets " + type1.AssemblyQualifiedName +
                                                 " found at AdditionalBusinessClasses list. " + typeof(IOModule).Name +
                                                 " should be the last module added to Application.Modules. Please check InitializeComponent method of your XafApplication descenant");
            return infoType;
        }
    }
}