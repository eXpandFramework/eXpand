using System.ComponentModel;
using System.Reflection;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("Version")]
    public class PersistentAssemblyVersionAttributeInfo : PersistentAssemblyAttributeInfo
    {
        public PersistentAssemblyVersionAttributeInfo(Session session) : base(session) {
        }
        private string _version;

        public string Version {
            get { return _version; }
            set { SetPropertyValue("Version", ref _version, value); }
        }
        public override AttributeInfo Create() {
            ConstructorInfo constructorInfo =
                typeof(AssemblyVersionAttribute).GetConstructor(new[] { typeof(string)});
            return new AttributeInfo(constructorInfo, Version);
        }
    }
}