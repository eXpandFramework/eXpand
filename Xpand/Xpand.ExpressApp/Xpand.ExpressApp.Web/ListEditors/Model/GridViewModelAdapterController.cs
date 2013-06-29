using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxGridView;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Model.Options;
using Xpand.ExpressApp.ModelAdaptor.Logic;
using Xpand.ExpressApp.ModelAdaptor.Model;
using Xpand.ExpressApp.Web.ListEditors.Model.ModelAfaptor;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Web.ListEditors.Model {
    public class GridViewModelAdapterController : ModelAdapterController, IModelExtender {
        protected ASPxGridListEditor _asPxGridListEditor;

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_asPxGridListEditor != null)
                _asPxGridListEditor.CreateCustomModelSynchronizer -= GridListEditorOnCreateCustomModelSynchronizer;
        }

        void GridListEditorOnCreateCustomModelSynchronizer(object sender, CreateCustomModelSynchronizerEventArgs e) {
            var modelAdaptorRuleController = Frame.GetController<ModelAdaptorRuleController>();
            if (modelAdaptorRuleController != null) {
                modelAdaptorRuleController.ExecuteLogic<IModelAdaptorGridViewOptionsRule, IModelModelAdaptorGridViewOptionsRule>(
                        rule => AssignSynchronizer(e, rule));
            }
            CreateCustomModelSynchronizerHelper.Assign(e, new GridViewListEditorModelSynchronizer(_asPxGridListEditor));
        }
        void AssignSynchronizer(CreateCustomModelSynchronizerEventArgs createCustomModelSynchronizerEventArgs, IModelModelAdaptorGridViewOptionsRule rule) {
            CreateCustomModelSynchronizerHelper.Assign(createCustomModelSynchronizerEventArgs, new GridViewListEditorModelSynchronizer(_asPxGridListEditor.Grid, rule));
        }

        protected override void OnActivated() {
            base.OnActivated();
            var listView = View as ListView;
            if (listView != null && listView.Editor != null && listView.Editor is ASPxGridListEditor) {
                _asPxGridListEditor = (ASPxGridListEditor)listView.Editor;
                _asPxGridListEditor.CreateCustomModelSynchronizer += GridListEditorOnCreateCustomModelSynchronizer;
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewOptionsGridView>();
            extenders.Add<IModelColumn, IModelColumnOptionsGridView>();

            var builder = new InterfaceBuilder(extenders);

            var assembly = builder.Build(CreateBuilderData(), GetPath(typeof(ASPxGridView).Name));

            builder.ExtendInteface<IModelOptionsGridView, ASPxGridView>(assembly);
            builder.ExtendInteface<IModelOptionsColumnGridView, GridViewColumn>(assembly);
        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(typeof(ASPxGridView)) {
                Act = info => (info.DXFilter() || typeof(PropertiesBase).IsAssignableFrom(info.PropertyType))
            };
            yield return new InterfaceBuilderData(typeof(GridViewColumn)) {
                Act = info => (info.DXFilter()) && info.Name != "Width"
            };
        }
    }
}
