using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.IO.Core;
using eXpand.ExpressApp.IO.NodeUpdaters;

namespace eXpand.ExpressApp.IO
{
    public sealed partial class IOModule : ModuleBase
    {

        public IOModule()
        {
            InitializeComponent();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            IEnumerable<Type> selectMany = application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses);
            TypesInfo.Instance.AddTypes(selectMany);
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new AllowEditForClassInfoNodeListViewsUpdater());
        }

//        public override void UpdateModel(IModelApplication applicationModel)
//        {
//            base.UpdateModel(applicationModel);
//            if (Application == null)
//                return;
//
//            allowEditForClassInfoNodeListViews(applicationModel);
//        }

//        private void allowEditForClassInfoNodeListViews(IModelApplication model)
//        {
//            foreach (var view in model.Views.OfType<IModelListView>()
//                .Where(view => view.ModelClass.TypeInfo.Type == TypesInfo.Instance.ClassInfoGraphNodeType))
//            {
//                view.AllowEdit = true;
//            }
//        }
    }
}