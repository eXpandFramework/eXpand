using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Web.ListEditors.Model {
//    public class GridViewModelAdapterController : ModelAdapterController, IModelExtender {
//        protected ASPxGridListEditor ASPxGridListEditor;
//
//        protected override void OnDeactivated() {
//            base.OnDeactivated();
//            if (ASPxGridListEditor != null){
////                ASPxGridListEditor.ModelApplied-=ASPxGridListEditorOnModelApplied;
////                ASPxGridListEditor.ModelSaved-=ASPxGridListEditorOnModelSaved;
//            }
//        }
//
//        protected override void OnActivated() {
//            base.OnActivated();
//            var listView = View as ListView;
//            if (listView?.Editor is ASPxGridListEditor) {
//                ASPxGridListEditor = (ASPxGridListEditor)listView.Editor;
////                ASPxGridListEditor.ModelApplied += ASPxGridListEditorOnModelApplied;
////                ASPxGridListEditor.ModelSaved += ASPxGridListEditorOnModelSaved;
//            }
//        }
//
//        private void ASPxGridListEditorOnModelSaved(object sender, EventArgs eventArgs){
//            throw new NotImplementedException();
////            new GridViewListEditorModelSynchronizer(ASPxGridListEditor).SynchronizeModel();
//        }
//
//        private void ASPxGridListEditorOnModelApplied(object sender, EventArgs eventArgs){
//            throw new NotImplementedException();
////            new GridViewListEditorModelSynchronizer(ASPxGridListEditor).ApplyModel();
//        }
//
//        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
//            return;
////            throw new NotImplementedException();
////            extenders.Add<IModelListView, IModelListViewOptionsGridView>();
////            extenders.Add<IModelColumn, IModelColumnOptionsGridView>();
//
//            var builder = new InterfaceBuilder(extenders);
//
//            var interfaceBuilderDatas = CreateBuilderData();
//            var assemblyFilePath = GetPath(typeof(ASPxGridView).Name);
//            var assembly = builder.Build(interfaceBuilderDatas, assemblyFilePath);
//
////            builder.ExtendInteface<IModelOptionsGridView, ASPxGridView>(assembly);
////            builder.ExtendInteface<IModelOptionsColumnGridView, GridViewDataColumn>(assembly);
//        }
//
//        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
//            yield return new InterfaceBuilderData(typeof(ASPxGridView)) {
//                Act =info =>info.Name!="Item"&&info.DXFilter()
//            };
//            yield return new InterfaceBuilderData(typeof(GridViewDataColumn)) {
//                Act = info => (info.DXFilter(BaseGridViewControlTypes(), typeof(object))) && info.Name != "Width"
//            };
//            yield return new InterfaceBuilderData(typeof(GridViewBandColumn)) {
//                Act = info => {
//                    var propertyName = GetPropertyName<GridViewBandColumn>(x=>x.Name);
//                    if (info.Name==propertyName)
//                        info.AddAttribute(new RequiredAttribute());
//                    return info.DXFilter(BaseGridViewBandColumnControlTypes(), typeof(object));
//                } 
//            };
//        }
//
//        IList<Type> BaseGridViewControlTypes() {
//            return BaseGridViewBandColumnControlTypes();
//        }
//
//        IList<Type> BaseGridViewBandColumnControlTypes() {
//            return new List<Type> { typeof(Style),typeof(StateManager) };
//        }
//    }
}
