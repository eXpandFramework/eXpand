using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [InterfaceRegistrator(typeof(IPersistentSizeAttribute))]
    [DefaultProperty("Size")]
    [System.ComponentModel.DisplayName("Size")]
    [CreateableItem(typeof(IPersistentMemberInfo))]
    [CreateableItem(typeof(IExtendedMemberInfo))]
    public class PersistentSizeAttribute : PersistentAttributeInfo, IPersistentSizeAttribute {
        public PersistentSizeAttribute(Session session)
            : base(session) {
        }


        public override void AfterConstruction() {
            base.AfterConstruction();
            Size = 100;
        }
        private int _size;
        public int Size {
            get {
                return _size;
            }
            set {
                SetPropertyValue("Size", ref _size, value);
            }
        }
        public override AttributeInfoAttribute Create() {
            return new AttributeInfoAttribute(typeof(SizeAttribute).GetConstructor(new[] { typeof(int) }), Size);
        }
    }
}
