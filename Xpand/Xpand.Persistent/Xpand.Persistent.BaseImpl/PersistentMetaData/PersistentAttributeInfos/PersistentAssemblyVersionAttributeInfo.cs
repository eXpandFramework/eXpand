using System.ComponentModel;
using System.Reflection;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("Version")]
    public class PersistentAssemblyVersionAttributeInfo : PersistentAssemblyAttributeInfo, IPersistentAssemblyVersionAttributeInfo {
        public PersistentAssemblyVersionAttributeInfo(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            Version = "1.0";
        }

        private string _version;
        [Custom("EditMask", "#.#")]
        public string Version {
            get {
                return _version;
            }
            set {
                SetPropertyValue("Version", ref _version, value);
            }
        }

        public override AttributeInfoAttribute Create() {
            ConstructorInfo constructorInfo =
                typeof(AssemblyVersionAttribute).GetConstructor(new[] { typeof(string) });
            return new AttributeInfoAttribute(constructorInfo, new object[] { Version });
        }
    }
}