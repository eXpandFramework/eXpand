using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("Version")]
    [System.ComponentModel.DisplayName("Version")]
    [CreateableItem]
    public class PersistentAssemblyVersionAttributeInfo : PersistentAssemblyAttributeInfo,
                                                          IPersistentAssemblyVersionAttributeInfo {
        
        string _version;

        public PersistentAssemblyVersionAttributeInfo(Session session)
            : base(session) {
        }

        #region IPersistentAssemblyVersionAttributeInfo Members
        [ModelDefault("EditMask", "#.#")]
        public string Version {
            get { return _version; }
            set { SetPropertyValue("Version", ref _version, value); }
        }
        #endregion
        public override void AfterConstruction() {
            base.AfterConstruction();
            Version = "1.0";
        }

        public override AttributeInfoAttribute Create() {
            ConstructorInfo constructorInfo =
                typeof (AssemblyVersionAttribute).GetConstructor(new[]{typeof (string)});
            return new AttributeInfoAttribute(constructorInfo, new object[]{Version});
        }
    }
}