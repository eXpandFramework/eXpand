using System.Collections;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.TreeListEditors.Win.Controllers
{


    using System;
    using DevExpress.ExpressApp;
    using DevExpress.Persistent.Base.General;
    using DevExpress.Data.Filtering;


    public partial class HideICategorizedItemController : ViewController
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
                View.ObjectTypeInfo.Type.GetProperty("Category").PropertyType.GetInterface(typeof(IObsoleteTreeNode).Name) != null)
                View.ControlsCreated += View_OnControlsCreated;
        }

        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            var view = (ListView) View;
            var listEditor = (DevExpress.ExpressApp.TreeListEditors.Win.CategorizedListEditor) view.Editor;
            ListView categoriesListView = listEditor.CategoriesListView;

            IObsoleteTreeNode obsoleteTreeNode = null;
            var propertyName = obsoleteTreeNode.GetPropertyName(node => node.Obsolete);
            categoriesListView.CollectionSource.Criteria[propertyName] =
                new GroupOperator(GroupOperatorType.Or, new BinaryOperator(propertyName, true));

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
            categoriesListView.CollectionSource.Criteria[propertyName] = keyProperty != null ? new NotOperator(new InOperator(keyProperty.Name, ids)) : null;
        }
    }
}