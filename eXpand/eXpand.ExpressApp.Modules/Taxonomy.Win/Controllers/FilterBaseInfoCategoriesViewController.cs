using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.Persistent.Base.General;
using eXpand.ExpressApp.Taxonomy.BaseObjects;
using eXpand.Xpo.Collections;

namespace eXpand.ExpressApp.Taxonomy.Win.Controllers{
    public partial class FilterBaseInfoCategoriesViewController : ViewController{
        public FilterBaseInfoCategoriesViewController(){
            InitializeComponent();
            RegisterActions(components);
            Activated += ViewController1_Activated;
            Deactivating += ViewController1_Deactivating;
            TargetViewType=ViewType.ListView;
            TargetObjectType = typeof (BaseInfo);
        }

        private void ViewController1_Deactivating(object sender, EventArgs e){
            if (View.ObjectTypeInfo.Type is ICategorizedItem)
                View.ControlsCreated -= View_ControlsCreated;
        }

        private void ViewController1_Activated(object sender, EventArgs e){
            return;
            if (View.ObjectTypeInfo.Type.GetInterface("ICategorizedItem") != null)
                View.ControlsCreated += View_ControlsCreated;
        }

        private void View_ControlsCreated(object sender, EventArgs e){
            var view = (ListView) View;
            var listEditor = (CategorizedListEditor) view.Editor;
            ListView categoriesListView = listEditor.CategoriesListView;
            
            
            categoriesListView.CollectionSource.Criteria["MyCriteria"] = DBCollection.GetClassTypeFilter(typeof(StructuralTerm),View.ObjectSpace.Session);
        }
    }
}