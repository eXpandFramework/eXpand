//using System;
//using System.Collections.Generic;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Model;
//using DevExpress.ExpressApp.TreeListEditors.Web;
//using DevExpress.Web.ASPxTreeList;
//using Xpand.Persistent.Base.ModelAdapter;
//
//namespace Xpand.ExpressApp.TreeListEditors.Web.Model {
//    public class TreeListModelAdapterController : TreeListEditors.Model.TreeListModelAdapterController {
//        protected override bool FilterTreeList(DynamicModelPropertyInfo info){
//            return base.FilterTreeList(info)&&info.PropertyType!=TreeListType();
//        }
//
//        protected override ModelSynchronizer ModelSynchronizer() {
//            throw new NotImplementedException();
////            return new TreeListEditorDynamicModelSynchronizer(((ASPxTreeListEditor) ((ListView) View).Editor));
//        }
//
//        protected override bool GetValidEditor(){
//            return ((ListView) View).Editor is ASPxTreeListEditor;
//        }
//
//        protected override Type TreeListColumnType() {
//            return typeof(TreeListColumn);
//        }
//
//        protected override Type TreeListType() {
//            return typeof(ASPxTreeList);
//        }
//
//        protected override IList<Type> GetTreeListFilterTypes(){
//            return new[]{typeof(TreeListSettingsBase)};
//        }
//    }
//
//}
