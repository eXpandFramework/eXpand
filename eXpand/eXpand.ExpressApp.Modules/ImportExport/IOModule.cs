using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.IO.Core;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.IO {
    public sealed partial class IOModule : ModuleBase
    {
        
        public IOModule(){
            InitializeComponent();
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            TypesInfo.Instance.AddTypes(Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses));
        }
        public override void UpdateModel(Dictionary model) {
            base.UpdateModel(model);
            if (Application == null)
                return;
            allowEditForClassInfoNodeListViews(model);
        }

        void allowEditForClassInfoNodeListViews(Dictionary model) {
            var applicationNodeWrapper = new ApplicationNodeWrapper(model);
            ClassInfoNodeWrapper classInfoNodeWrapper = applicationNodeWrapper.BOModel.FindClassByType(TypesInfo.Instance.ClassInfoGraphNodeType);
            foreach (var listViewInfoNodeWrapper in applicationNodeWrapper.Views.GetListViews(classInfoNodeWrapper)) {
                listViewInfoNodeWrapper.AllowEdit = true;
            }
        }
    }
}