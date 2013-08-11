using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule.Search {
    [ModelAbstractClass]
    public interface IModelDetailViewFullTextSearch : IModelDetailView {
        [DataSourceProperty("ListViews")]
        [Category(AttributeCategoryNameProvider.Search)]
        IModelListView FullTextListView { get; set; }
        [Browsable(false)]
        IModelList<IModelListView> ListViews { get; }
    }

    [DomainLogic(typeof(IModelDetailViewFullTextSearch))]
    public class ModelDetailViewFullTextSearchDomainLogic {
        public static IModelList<IModelListView> Get_ListViews(IModelDetailViewFullTextSearch modelDetailViewFullTextSearch) {
            var memberViewItems = modelDetailViewFullTextSearch.Items.OfType<IModelMemberViewItem>().Where(item => item.ModelMember.MemberInfo.IsList);
            return new CalculatedModelNodeList<IModelListView>(memberViewItems.Select(item => ((IModelListView)item.View)));
        }
    }
    public class DetailViewFullTextSearchController:ViewController<DetailView>,IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            var fullTextListView = ((IModelDetailViewFullTextSearch)View.Model).FullTextListView;
            if (fullTextListView != null) {
                var filterController = Frame.GetController<FilterController>();
                var listPropertyEditor = View.GetItems<ListPropertyEditor>().First(editor => editor.Model.View == fullTextListView);
                EventHandler<EventArgs>[] listPropertyEditorOnControlCreated = { null };
                listPropertyEditorOnControlCreated[0] = (sender, args) => {
                    filterController.SetView(listPropertyEditor.ListView);
                    listPropertyEditor.ControlCreated -= listPropertyEditorOnControlCreated[0];
                };
                listPropertyEditor.ControlCreated += listPropertyEditorOnControlCreated[0];
            }
        }
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelDetailView, IModelDetailViewFullTextSearch>();
        }

    }
}