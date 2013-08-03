using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [InterfaceRegistrator(typeof(IPersistentPersistentAliasAttribute))]
    [DefaultProperty("AliasExpression")]
    [System.ComponentModel.DisplayName("Persistent Alias")]
    [CreateableItem(typeof(IPersistentMemberInfo))]
    [CreateableItem(typeof(IExtendedMemberInfo))]
    public class PersistentPersistentAliasAttribute : PersistentAttributeInfo, IPersistentPersistentAliasAttribute {


        public PersistentPersistentAliasAttribute(Session session)
            : base(session) {
        }

        private string _aliasExpression;
        public string AliasExpression {
            get {
                return _aliasExpression;
            }
            set {
                SetPropertyValue("AliasExpression", ref _aliasExpression, value);
            }
        }

        public override AttributeInfoAttribute Create() {
            return
                new AttributeInfoAttribute(typeof(PersistentAliasAttribute).GetConstructor(new[] { typeof(string) }),
                                  AliasExpression);
        }
    }
}
