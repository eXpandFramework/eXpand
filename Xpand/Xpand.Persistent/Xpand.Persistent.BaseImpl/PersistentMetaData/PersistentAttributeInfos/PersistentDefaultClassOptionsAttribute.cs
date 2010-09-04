using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [InterfaceRegistrator(typeof(IPersistentDefaulClassOptionsAttribute))]
    [DefaultProperty("DefaultClassOptionsName")]
    public class PersistentDefaultClassOptionsAttribute : PersistentAttributeInfo, IPersistentDefaulClassOptionsAttribute
    {
        public PersistentDefaultClassOptionsAttribute(Session session) : base(session) {
        }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string DefaultClassOptionsName
        {
            get { return "DefaultClassOptions"; }
        }
        
        public override AttributeInfo Create() {
            return new AttributeInfo(typeof(DefaultClassOptionsAttribute).GetConstructor(new Type[0]));
        }
    }
}