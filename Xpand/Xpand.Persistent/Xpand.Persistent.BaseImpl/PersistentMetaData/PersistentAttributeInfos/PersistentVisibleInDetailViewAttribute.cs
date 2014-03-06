using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos{
    [DefaultProperty("PersistentVisibleInDetailViewName")]
    [System.ComponentModel.DisplayName("Visible In Detail View")]
    [CreateableItem(typeof (IPersistentMemberInfo))]
    [CreateableItem(typeof (IExtendedMemberInfo))]
    [InterfaceRegistrator(typeof (IPersistentVisibleInDetailViewAttribute))]
    public class PersistentVisibleInDetailViewAttribute : PersistentAttributeInfo,
        IPersistentVisibleInDetailViewAttribute{
        private bool _visible;

        public PersistentVisibleInDetailViewAttribute(Session session)
            : base(session){
        }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string PersistentVisibleInDetailViewName{
            get { return "PersistentVisibleInDetailView"; }
        }

        public bool Visible{
            get { return _visible; }
            set { SetPropertyValue("Visible", ref _visible, value); }
        }

        public override AttributeInfoAttribute Create(){
            return
                new AttributeInfoAttribute(typeof (VisibleInDetailViewAttribute).GetConstructor(new[]{typeof (bool)}),
                    Visible);
        }
    }
}