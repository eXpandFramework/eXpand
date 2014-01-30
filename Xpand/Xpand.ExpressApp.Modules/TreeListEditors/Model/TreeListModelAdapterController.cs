using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.TreeListEditors.Model {
    public abstract class TreeListModelAdapterController : ModelAdapterController, IModelExtender{

        protected override void OnDeactivated() {
            base.OnDeactivated();
            var listView = View as ListView;
            if (listView != null)
                listView.Editor.CreateCustomModelSynchronizer -= GridListEditorOnCreateCustomModelSynchronizer;
        }

        void GridListEditorOnCreateCustomModelSynchronizer(object sender, CreateCustomModelSynchronizerEventArgs e){
            var modelSynchronizer = ModelSynchronizer();
            if (modelSynchronizer!=null)
                CustomModelSynchronizerHelper.Assign(e, modelSynchronizer);
        }

        protected abstract ModelSynchronizer ModelSynchronizer();

        protected override void OnActivated() {
            base.OnActivated();
            var listView = View as ListView;
            if (listView != null && listView.Editor != null && GetValidEditor()) {
                listView.Editor.CreateCustomModelSynchronizer += GridListEditorOnCreateCustomModelSynchronizer;
            }
        }

        protected abstract bool GetValidEditor();

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewOptionsTreeList>();
            extenders.Add<IModelRootNavigationItems, IModelListViewOptionsTreeListNavigation>();
            extenders.Add<IModelColumn, IModelColumnOptionsTreeListView>();


            var builder = new InterfaceBuilder(extenders);

            var assembly = builder.Build(CreateBuilderData(), GetPath(TreeListType().Name));

            builder.ExtendInteface(typeof(IModelOptionsTreeList), TreeListType(), assembly);
            builder.ExtendInteface(typeof(IModelOptionsColumnTreeListView), TreeListColumnType(), assembly);

            ExtendModelInterfaces(builder,assembly);
        }

        protected virtual void ExtendModelInterfaces(InterfaceBuilder builder, Assembly assembly){
            
        }

        protected abstract Type TreeListColumnType();

        protected abstract Type TreeListType();

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(TreeListType()) {
                Act = info => info.DXFilter()
            };
            yield return new InterfaceBuilderData(TreeListColumnType()) {
                Act = info => info.DXFilter()
            };
        }
    }
}
