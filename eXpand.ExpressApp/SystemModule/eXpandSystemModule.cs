using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.Core.DictionaryHelpers;

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

        public override void ValidateModel(Dictionary model)
        {
            base.ValidateModel(model);
            DictionaryHelper.AddFields(model.RootNode, XafTypesInfo.XpoTypeInfoSource.XPDictionary);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            ITypeInfo personClassInfo = typesInfo.PersistentTypes.Where(info => info.Type == typeof(Person)).SingleOrDefault();
            if (personClassInfo != null)
            {
                IMemberInfo personFullNameMemberInfo = personClassInfo.FindMember("FullName");
                var persistentAliasAttribute = personFullNameMemberInfo.FindAttribute<PersistentAliasAttribute>();
                if (persistentAliasAttribute == null)
                    personFullNameMemberInfo.AddAttribute(new PersistentAliasAttribute("FirstName + MiddleName + LastName"));
            }
        }
        
    }
}