using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo.Metadata;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class LookUpListSearchAlwaysEnableController : BaseViewController
    {
        public const string LookUpListSearch = "LookUpListSearch";
        private XPDictionary xpDictionary;

        public LookUpListSearchAlwaysEnableController()
        {
            InitializeComponent();
            RegisterActions(components);
            
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (Frame.Template is ILookupPopupFrameTemplate)
            {
                if (View.Info.GetAttributeValue(LookUpListSearch) == "AlwaysEnable")
                    ((ILookupPopupFrameTemplate)Frame.Template).IsSearchEnabled = true;
            }
        }

        public XPDictionary XpDictionary
        {
            get { return xpDictionary; }
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            XPDictionary dictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            xpDictionary = dictionary;
        }

        
        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            var applicationNodeWrapper = new ApplicationNodeWrapper(model);
            IEnumerable<string> enumerable = applicationNodeWrapper.BOModel.Classes.Where(
                wrapper => typeof (ICategorizedItem).IsAssignableFrom(wrapper.ClassTypeInfo.Type)).Select(wrapper => wrapper.ClassTypeInfo.FullName);
            foreach (var nodeWrapper in applicationNodeWrapper.Views.Items.Where(
                wrapper => wrapper.Id.EndsWith("_LookupListView") && enumerable.Contains(wrapper.ClassName)))
                nodeWrapper.Node.SetAttribute(LookUpListSearch, "AlwaysEnable");

        }

        [CoverageExclude]
        public override Schema GetSchema()
        {
            const string CommonTypeInfos =
                @"<Element Name=""Application"">
                    <Element Name=""Views"" >
                        <Element Name=""ListView"" >
                            <Attribute Name=""" +
                LookUpListSearch +
                @""" Choice=""Default,AlwaysEnable""/>
                        </Element>
                    </Element>
                </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

    }
}