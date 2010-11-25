using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [InterfaceRegistrator(typeof(IPersistentPersistentAliasAttribute))]
    [DefaultProperty("AliasExpression")]
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

        public override AttributeInfo Create() {
            return
                new AttributeInfo(typeof(PersistentAliasAttribute).GetConstructor(new[] { typeof(string) }),
                                  AliasExpression);
        }
    }
}
