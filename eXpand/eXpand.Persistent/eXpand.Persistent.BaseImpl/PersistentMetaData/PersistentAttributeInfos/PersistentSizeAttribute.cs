using System.ComponentModel;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos
{
    [Registrator(typeof(IPersistentSizeAttribute))]
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
