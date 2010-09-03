using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.IO.Core;
using eXpand.ExpressApp.IO.NodeUpdaters;

namespace eXpand.ExpressApp.IO
{
    public sealed partial class IOModule : XpandModuleBase
    {

        public IOModule()
        {
            InitializeComponent();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            TypesInfo.Instance.AddTypes(GetAdditionalClasses());
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new AllowEditForClassInfoNodeListViewsUpdater());
        }
    }
}