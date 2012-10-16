using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotGrid;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.XtraPivotGrid;
using Xpand.Persistent.Base.ModelAdapter;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.PivotGrid.Win.Model {
    public class PivotGridModelAdapterController : ModelAdapterController, IModelExtender {
        PivotGridListEditor _pivotGridListEditor;

        protected override void OnActivated() {
            base.OnActivated();
            var listView = View as ListView;
            if (listView != null && listView.Editor != null && listView.Editor.GetType() == typeof(PivotGridListEditor)) {
                _pivotGridListEditor = (PivotGridListEditor)listView.Editor;
                _pivotGridListEditor.ControlsCreated += PivotGridListEditorOnControlsCreated;
            }
        }

        void PivotGridListEditorOnControlsCreated(object sender, EventArgs eventArgs) {
            new PivotGridListEditorModelSynchronizer(_pivotGridListEditor).ApplyModel();
        }

        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelPivotSettings, IModelPivotSettingsEx>();
            extenders.Add<IModelListView, IModelListViewOptionsPivotGrid>();

            var builder = new InterfaceBuilder(extenders);
            var interfaceBuilderDatas = CreateBuilderData();
            var assembly = builder.Build(interfaceBuilderDatas, GetPath(typeof(PivotGridControl).Name));

            builder.ExtendInteface<IModelOptionsPivotGrid, PivotGridControl>(assembly);
        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return CreatePivotGridControlData();
        }

        InterfaceBuilderData CreatePivotGridControlData() {
            return new InterfaceBuilderData(typeof(PivotGridControl)) {
                Act = info => {
                    if (info.Name == "Editable") {
                        info.CreateValueCalculator("((IModelListView)this.Parent.Parent).AllowEdit");
                    }
                    return info.DXFilter(typeof(Control));
                }
            };
        }

        internal InterfaceBuilderData CreatePivotGridFieldData() {
            return new InterfaceBuilderData(typeof(PivotGridField)) {
                Act = info => info.DXFilter(typeof(PivotGridFieldBase))
            };
        }
        #endregion
    }
}
