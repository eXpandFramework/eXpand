using System.ComponentModel;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos
{
    [InterfaceRegistrator(typeof(IPersistentPersistentAttribute))]
    [DefaultProperty("MapTo")]
    public class PersistentPersistentAttribute : PersistentAttributeInfo, IPersistentPersistentAttribute {
        public PersistentPersistentAttribute(Session session) : base(session) {
        }
        private string _mapTo;

        public string MapTo {
            get { return _mapTo; }
            set { SetPropertyValue("MapTo", ref _mapTo, value); }
        }
        public override AttributeInfo Create()
        {
            return
                new AttributeInfo(typeof(PersistentAttribute).GetConstructor(new[] { typeof(string) }),
                                  MapTo);
        }
    }
}
