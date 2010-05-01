using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.IO.Core;

namespace eXpand.ExpressApp.IO
{
    public sealed partial class IOModule : ModuleBase
    {

        public IOModule()
        {
            InitializeComponent();
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            TypesInfo.Instance.AddTypes(Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses));
        }

        public override void UpdateModel(IModelApplication applicationModel)
        {
            base.UpdateModel(applicationModel);
            if (Application == null)
                return;

            allowEditForClassInfoNodeListViews(applicationModel);
        }

        private void allowEditForClassInfoNodeListViews(IModelApplication model)
        {
            foreach (var view in model.Views.OfType<IModelListView>()
                .Where(view => view.ModelClass.TypeInfo.Type == TypesInfo.Instance.ClassInfoGraphNodeType))
            {
                view.AllowEdit = true;
            }
        }
    }
}