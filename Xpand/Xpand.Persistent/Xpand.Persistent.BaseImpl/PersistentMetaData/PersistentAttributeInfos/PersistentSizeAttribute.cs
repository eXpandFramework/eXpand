using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos
{
    [InterfaceRegistrator(typeof(IPersistentSizeAttribute))]
    [DefaultProperty("Size")]
    public class PersistentSizeAttribute : PersistentAttributeInfo, IPersistentSizeAttribute {
        public PersistentSizeAttribute(Session session) : base(session) {
        }


        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Size = 100;
        }
        private int _size;
        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                SetPropertyValue("Size", ref _size, value);
            }
        }
        public override AttributeInfo Create() {
            return new AttributeInfo(typeof(SizeAttribute).GetConstructor(new[] { typeof(int) }),Size);
        }
    }
}
