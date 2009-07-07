using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.ExpressApp.SystemModule
{
    [ToolboxItem(true)]
    [Description("Includes Controllers that represent basic features for XAF applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof (XafApplication), "Resources.SystemModule.ico")]
    public sealed partial class eXpandSystemModule : ModuleBase
    {
        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            new ApplicationNodeWrapper(model).Node.GetChildNode("Options").SetAttribute("UseServerMode", "True");
        }

        public override void CustomizeXPDictionary(XPDictionary xpDictionary)
        {
            base.CustomizeXPDictionary(xpDictionary);
            XPClassInfo personClassInfo = xpDictionary.GetClassInfo(typeof (Person));
            XPMemberInfo personFullNameMemberInfo = personClassInfo.GetMember("FullName");
            var persistentAliasAttribute =
                personFullNameMemberInfo.FindAttributeInfo(typeof (PersistentAliasAttribute)) as
                PersistentAliasAttribute;
            if (persistentAliasAttribute == null)
                personFullNameMemberInfo.AddAttribute(new PersistentAliasAttribute("FirstName + MiddleName + LastName"));
        }
    }
}