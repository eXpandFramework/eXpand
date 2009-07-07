using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Attributes;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class CustomAttibutesController : WindowController
    {
        public CustomAttibutesController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
               

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            foreach (XPClassInfo xpClassInfo in XafTypesInfo.XpoTypeInfoSource.XPDictionary.Classes)
            {
                foreach (XPMemberInfo member in xpClassInfo.Members)
                {
                    if (member.HasAttribute(typeof(NumericFormat)))
                    {
                        member.AddAttribute(new CustomAttribute(PropertyInfoNodeWrapper.EditMaskAttribute, "f0"));
                        member.AddAttribute(new CustomAttribute(PropertyInfoNodeWrapper.DisplayFormatAttribute, "#"));
                        XafTypesInfo.Instance.RefreshInfo(xpClassInfo.ClassType);
                    }
                    
                }
            }
        }

    }
}