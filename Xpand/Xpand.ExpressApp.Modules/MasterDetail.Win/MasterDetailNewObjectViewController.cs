using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace eXpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailNewObjectViewController : MasterDetailBaseController
    {

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            return;
            if (IsHiddenFrame())
                Frame.GetController<WinDetailViewController>().SuppressConfirmation = true;
            XafGridView view = ((GridListEditor)View.Editor).GridView;
            view.MasterRowExpanded += view_MasterRowExpanded;
        }


        void view_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            var xafGridView = ((XafGridView)sender);
            BaseView detailView = xafGridView.GetDetailView(e.RowHandle, e.RelationIndex);
            if (detailView.DataController is XafCurrencyDataController)
                ((XafCurrencyDataController)detailView.DataController).NewItemRowObjectCustomAdding += (o, args) => CreateChildObject(xafGridView, e);

        }
        void CreateChildObject(XafGridView masterView, CustomMasterRowEventArgs e)
        {
            string relationName = masterView.GetRelationName(e.RowHandle, e.RelationIndex);
            object masterObject = masterView.GetRow(e.RowHandle);
            IMemberInfo memberInfo = XafTypesInfo.Instance.FindTypeInfo(masterObject.GetType()).FindMember(relationName);
            Type listElementType = memberInfo.ListElementType;
            IMemberInfo referenceToOwner = memberInfo.AssociatedMemberInfo;
            object obj = GetObjectSpace().CreateObject(listElementType);
            referenceToOwner.SetValue(obj, masterObject);
            //if (IsHiddenFrame())
                //((NestedObjectSpace)ObjectSpace).ParentObjectSpace.CommitChanges();
        }

        ObjectSpace GetObjectSpace() {
            if (!(ObjectSpace is NestedObjectSpace))
                return ObjectSpace;
            var nestedObjectSpace = (NestedObjectSpace) ObjectSpace;
            while (nestedObjectSpace.ParentObjectSpace is NestedObjectSpace) {
                nestedObjectSpace = nestedObjectSpace.ParentObjectSpace as NestedObjectSpace;
            }
            return nestedObjectSpace.ParentObjectSpace;
        }

        bool IsHiddenFrame()
        {
            return !((WinWindow)Frame).Form.Visible;
        }
    }
}