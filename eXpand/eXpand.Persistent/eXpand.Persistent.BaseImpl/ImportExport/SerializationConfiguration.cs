using System;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Xpo.Converters.ValueConverters;
using System.ComponentModel;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl.ImportExport {

    [DefaultClassOptions][NavigationItem("ImportExport")]
    public class SerializationConfiguration : BaseObject, ISerializationConfiguration {
        private Type _typeToSerialize;
        public SerializationConfiguration(Session session) : base(session) { }


        [Index(0)]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type TypeToSerialize
        {
            get { return _typeToSerialize; }
            set { SetPropertyValue("TypeToSerialize", ref _typeToSerialize, value); }
        }
        [Association][Aggregated]
        public XPCollection<ClassInfoGraphNode> SerializationGraph
        {
            get { return GetCollection<ClassInfoGraphNode>("SerializationGraph"); }
        }

        IList<IClassInfoGraphNode> ISerializationConfiguration.SerializationGraph {
            get {
                return new ListConverter<IClassInfoGraphNode,ClassInfoGraphNode>(SerializationGraph);
            }
        }
    }
}