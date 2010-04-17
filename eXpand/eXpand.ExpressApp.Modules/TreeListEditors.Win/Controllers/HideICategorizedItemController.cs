using System;
using System.Collections;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.SystemModule;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.TreeListEditors.Win.Controllers
{
    public partial class HideICategorizedItemController : BaseViewController
    {
        public HideICategorizedItemController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.ListView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.ObjectTypeInfo.Type.GetInterface(typeof (ICategorizedItem).Name) != null &&
                View.ObjectTypeInfo.Type.GetProperty("Category").PropertyType.GetInterface(typeof(IHidden).Name) != null)
                View.ControlsCreated += View_OnControlsCreated;
        }

        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            var view = (ListView) View;
            var listEditor = (CategorizedListEditor) view.Editor;
            ListView categoriesListView = listEditor.CategoriesListView;


            categoriesListView.CollectionSource.Criteria["Hidden"] =
                new GroupOperator(GroupOperatorType.Or, new BinaryOperator("Hidden", true));

            var ids = new ArrayList();
            XPMemberInfo keyProperty = null;
            foreach (ITreeNode treeNode in categoriesListView.CollectionSource.List)
            {
                var baseObject = (XPBaseObject) treeNode;
                keyProperty = baseObject.ClassInfo.KeyProperty;
                ids.Add(keyProperty.GetValue(baseObject));
                for (int i = treeNode.Children.Count - 1; i > -1; i--)
                {
                    baseObject = (XPBaseObject)treeNode.Children[i];
                    ids.Add(keyProperty.GetValue(baseObject));
                }
            }
            categoriesListView.CollectionSource.Criteria["Hidden"] = keyProperty != null ? new NotOperator(new InOperator(keyProperty.Name, ids)) : null;
        }
    }
}