using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.ConditionalActionState.Logic;
using eXpand.ExpressApp.ConditionalControllerState.Logic;
using eXpand.ExpressApp.SystemModule;
using FeatureCenter.Base;
using IQueryable = System.Linq.IQueryable;
using System.Linq;

namespace FeatureCenter.Module
{
        
    #region Detail View of Persistent object with records
    [eXpand.ExpressApp.Attributes.NavigationItem("Navigation/Detail View of Persistent object with records", "Customer_DetailView")]
    #endregion
    #region Conditional Save Delete
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalSaveDelete, "1=1", "1=1", Captions.ViewMessageConditionalSaveDelete, Position.Bottom, View = "ConditionalSaveDelete_DetailView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConditionalSaveDelete, "1=1", "1=1", Captions.HeaderConditionalSaveDelete, Position.Top, View = "ConditionalSaveDelete_DetailView")]
    [ActionStateRule("Disable Save for ConditionalSaveDelete", "Save", "City='Paris'", "1=1", ActionState.Disabled, ExecutionContextGroup = "ConditionalSaveDelete", View = "ConditionalSaveDelete_DetailView")]
    [ActionStateRule("Disable SaveAndClose for ConditionalSaveDelete", "SaveAndClose", "City='Paris'", "1=1", ActionState.Disabled, ExecutionContextGroup = "ConditionalSaveDelete", View = "ConditionalSaveDelete_DetailView")]
    [ActionStateRule("Disable Delete for ConditionalSaveDelete", "Delete", "City='Paris'", "1=1", ActionState.Disabled, ExecutionContextGroup = "ConditionalSaveDelete", View = "ConditionalSaveDelete_DetailView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("ModelArtifact/Conditional Save Delete","ConditionalSaveDelete_DetailView")]
    [CloneView(CloneViewType.DetailView, "ConditionalSaveDelete_DetailView")]
    [DisplayFeatureModelAttribute("ConditionalSaveDelete_DetailView")]
    #endregion
    #region Conditional Foreign Key Violation And Show DetailView
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalFKViolation, "1=1", "1=1",
        Captions.ViewMessageConditionalFKViolation, Position.Bottom, ViewType = ViewType.ListView,
        View = "ConditionalForeignKeyViolationAndShowDetailView_ListView", NotUseSameType = true)]
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalFKViolation + "2", "1=1", "1=1",
        Captions.ViewMessageConditionalFKViolation2, Position.Bottom, ViewType = ViewType.ListView,
        View = "ConditionalForeignKeyViolationAndShowDetailView_ListView", NotUseSameType = true)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConditionalFKViolation, "1=1", "1=1",
        Captions.HeaderConditionalFKViolation, Position.Top, ViewType = ViewType.ListView,
        View = "ConditionalForeignKeyViolationAndShowDetailView_ListView")]
    [ControllerStateRule("ConditionalForeignKeyViolation", typeof (FKViolationViewController), "City='Paris'", "1=1",
        ControllerState.Disabled, View = "ConditionalForeignKeyViolationAndShowDetailView_ListView")]
    [ControllerStateRule("ConditionalShowDetailView", typeof (ListViewProcessCurrentObjectController), "City='Paris'",
        "1=1", ControllerState.Disabled, View = "ConditionalForeignKeyViolationAndShowDetailView_ListView")]
    [eXpand.ExpressApp.Attributes.NavigationItem(
        "ModelArtifact/Conditional Foreign Key Violation And Show DetailView",
        "ConditionalForeignKeyViolationAndShowDetailView_ListView")]
    [CloneView(CloneViewType.ListView, "ConditionalForeignKeyViolationAndShowDetailView_ListView")]
    [DisplayFeatureModelAttribute("ConditionalForeignKeyViolationAndShowDetailView_ListView")]
    #endregion
    #region LookUp ListView Search
    [CloneView(CloneViewType.ListView, "LookUpListViewSearch_LookupListView")]
    #endregion
    #region Load When Filtered
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderLoadWhenFiltered, "1=1", "1=1", Captions.ViewMessageLoadWhenFiltered, Position.Bottom, ViewType = ViewType.ListView, View = "LoadWhenFiltered_ListView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderLoadWhenFiltered, "1=1", "1=1", Captions.HeaderLoadWhenFiltered, Position.Top, ViewType = ViewType.ListView, View = "LoadWhenFiltered_ListView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Load When Filtered", "LoadWhenFiltered_ListView")]
    [CloneView(CloneViewType.ListView, "LoadWhenFiltered_ListView")]
    [DisplayFeatureModelAttribute("LoadWhenFiltered_ListView")]
    #endregion
    #region Linq Query
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderLinqQuery, "1=1", "1=1", Captions.ViewMessageLinqQuery, Position.Bottom, ViewType = ViewType.ListView, View = "LinqQuery_ListView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderLinqQuery, "1=1", "1=1", Captions.HeaderLinqQuery, Position.Top, ViewType = ViewType.ListView, View = "LinqQuery_ListView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("LinqQuery", "Customer_ListView_EmployeesLinq_Linq")]
    [CloneView(CloneViewType.ListView, "LinqQuery_ListView")]
    #endregion
    #region HighlightFocusedLayoutItem
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderHighlightFocusedLayoutItem, "1=1", "1=1", Captions.ViewMessageHighlightFocusedLayoutItem, Position.Bottom, ViewType = ViewType.DetailView, View = "HighlightFocusedLayoutItem_DetailView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderHighlightFocusedLayoutItem, "1=1", "1=1", Captions.HeaderHighlightFocusedLayoutItem, Position.Top, ViewType = ViewType.DetailView, View = "HighlightFocusedLayoutItem_DetailView")]
    [CloneView(CloneViewType.DetailView, "HighlightFocusedLayoutItem_DetailView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Highlight Focused Layout Item", "HighlightFocusedLayoutItem_DetailView")]
    [DisplayFeatureModelAttribute("HighlightFocusedLayoutItem_DetailView")]
    #endregion
    #region HideNestedListViewToolBar
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderHideListViewToolBar+"Nested", "1=1", "1=1", Captions.ViewMessageHideListViewToolBarNested, Position.Bottom,  View = "HideNestedListViewToolBar_DetailView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderHideListViewToolBar+"Nested", "1=1", "1=1", Captions.HeaderHideListViewToolBar, Position.Top,  View = "HideNestedListViewToolBar_DetailView")]
    [CloneView(CloneViewType.DetailView, "HideNestedListViewToolBar_DetailView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Hide Tool Bar/NestedListView", "HideNestedListViewToolBar_DetailView")]
    [DisplayFeatureModelAttribute("HideNestedListViewToolBar_DetailView")]
    #endregion
    #region HideListViewToolBar
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderHideListViewToolBar, "1=1", "1=1", Captions.ViewMessageHideListViewToolBar, Position.Bottom, ViewType = ViewType.ListView, View = "HideListViewToolBar_ListView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderHideListViewToolBar, "1=1", "1=1", Captions.HeaderHideListViewToolBar, Position.Top, ViewType = ViewType.ListView, View = "HideListViewToolBar_ListView")]
    [CloneView(CloneViewType.ListView, "HideListViewToolBar_ListView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Hide Tool Bar/ListView", "HideListViewToolBar_ListView")]
    [DisplayFeatureModelAttribute("HideListViewToolBar_ListView")]
    #endregion
    #region HideDetailViewToolBar
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderHideDetailViewToolBar, "1=1", "1=1", Captions.ViewMessageHideDetailViewToolBar, Position.Bottom, ViewType = ViewType.DetailView, View = "HideDetailViewToolBar_DetailView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderHideDetailViewToolBar, "1=1", "1=1", Captions.HeaderHideDetailViewToolBar, Position.Top, ViewType = ViewType.DetailView, View = "HideDetailViewToolBar_DetailView")]
    [CloneView(CloneViewType.DetailView, "HideDetailViewToolBar_DetailView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Hide Tool Bar/DetailView", "HideDetailViewToolBar_DetailView")]
    [DisplayFeatureModelAttribute("HideDetailViewToolBar_DetailView")]
    #endregion
    #region ForeignKey Violation
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderFKViolation, "1=1", "1=1", Captions.ViewMessageFKViolation, Position.Bottom, ViewType = ViewType.ListView, View = "ForeignKeyViolation_ListView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderFKViolation, "1=1", "1=1", Captions.HeaderFKViolation, Position.Top, View = "ForeignKeyViolation_ListView")]
    [CloneView(CloneViewType.ListView, "ForeignKeyViolation_ListView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Foreign Key Violation", "ForeignKeyViolation_ListView")]
    [DisplayFeatureModelAttribute("ForeignKeyViolation_ListView")]
    #endregion
    #region Disable Full Text For Memo Fields
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderDisableFullTextForMemoFields, "1=1", "1=1", Captions.ViewMessageDisableFullTextForMemoFields, Position.Bottom, ViewType = ViewType.ListView, View = "DisableFullTextForMemoFields_ListView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderDisableFullTextForMemoFields, "1=1", "1=1", Captions.HeaderDisableFullTextForMemoFields, Position.Top, ViewType = ViewType.ListView, View = "DisableFullTextForMemoFields_ListView")]
    [CloneView(CloneViewType.ListView, "DisableFullTextForMemoFields_ListView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Controlling Search/Disable Full Text For Memo Fields", "DisableFullTextForMemoFields_ListView")]
    [DisplayFeatureModelAttribute("DisableFullTextForMemoFields_ListView")]
    #endregion
    #region ControllingListViewSearch
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderControllingListViewSearch, "1=1", "1=1", Captions.ViewMessageControllingListViewSearch, Position.Bottom, ViewType = ViewType.ListView, View = "ControllingSearchCustomer_ListView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderControllingListViewSearch, "1=1", "1=1", Captions.HeaderControllingListViewSearch, Position.Top, ViewType = ViewType.ListView, View = "ControllingSearchCustomer_ListView")]
    [CloneView(CloneViewType.ListView, "ControllingSearchCustomer_ListView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Controlling Search/ListView Search", "ControllingSearchCustomer_ListView")]
    [DisplayFeatureModelAttribute("ControllingSearchCustomer_ListView", "ControllingListViewSearch")]
    
    #endregion
    #region ControllingDetailViewSearch
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderControllingDetailViewSearch, "1=1", "1=1", Captions.ViewMessageControllingDetailViewSearch, Position.Bottom, ViewType = ViewType.DetailView, View = "ControllingSearchCustomer_DetailView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderControllingDetailViewSearch, "1=1", "1=1", Captions.HeaderControllingDetailViewSearch, Position.Top, ViewType = ViewType.DetailView, View = "ControllingSearchCustomer_DetailView")]
    [CloneView(CloneViewType.DetailView, "ControllingSearchCustomer_DetailView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Controlling Search/DetailView Search", "ControllingSearchCustomer_DetailView")]
    [DisplayFeatureModelAttribute("ControllingSearchCustomer_DetailView", "ControllingDetailViewSearch")]
    #endregion
    #region ActionButtonViewItem
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderActionButtonViewItem, "1=1", "1=1",
        Captions.ViewMessageActionButtonViewItem, Position.Bottom, View = "ActionButtonViewItem_DetailView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderActionButtonViewItem, "1=1", "1=1",
        Captions.HeaderActionButtonViewItem, Position.Top, View = "ActionButtonViewItem_DetailView")]
    [CloneView(CloneViewType.DetailView, "ActionButtonViewItem_DetailView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Action Button View Item", "ActionButtonViewItem_DetailView")]
    [DisplayFeatureModelAttribute("ActionButtonViewItem_DetailView")]
    #endregion
    #region ConnectionInfoStatus
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConnectionInfoStatus, "1=1", "1=1", Captions.HeaderConnectionInfoStatus, Position.Top, View = "ConnectionInfoStatus_DetailView")]
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConnectionInfoStatus, "1=1", "1=1",
        Captions.ViewMessageConnectionInfoStatus, Position.Bottom, View = "ConnectionInfoStatus_DetailView")]
    [CloneView(CloneViewType.DetailView, "ConnectionInfoStatus_DetailView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Connection Info Status", "ConnectionInfoStatus_DetailView")]
    [DisplayFeatureModelAttribute("ConnectionInfoStatus_DetailView")]
    #endregion
    #region Disable Edit Detail View
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderDisableEditDetailView, "1=1", "1=1",
        Captions.ViewMessageDisableEditDetailView, Position.Bottom,View = "DisableEditDetailView_DetailView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderDisableEditDetailView, "1=1", "1=1",
        Captions.HeaderDisableEditDetailView, Position.Top, View = "DisableEditDetailView_DetailView")]
    [CloneView(CloneViewType.DetailView, "DisableEditDetailView_DetailView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Disable Edit Detail View","DisableEditDetailView_DetailView")]
    [DisplayFeatureModelAttribute("DisableEditDetailView_DetailView")]
    #endregion
    #region Conditional View Controls Positioning
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalViewControlsPositioning, "1=1",
        "1=1", Captions.ViewMessageConditionalViewControlsPositioning, Position.Bottom, View = "ConditionalViewControlsPositioning_DetailView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConditionalViewControlsPositioning, "1=1", "1=1",
        Captions.HeaderConditionalViewControlsPositioning, Position.Top, View = "ConditionalViewControlsPositioning_DetailView")]
    [AdditionalViewControlsRule(Captions.ConditionalViewControlsPositioningForCustomerName, "1=1", "1=1", null,
        Position.DetailViewItem, MessageProperty = "NameWarning", View = "ConditionalViewControlsPositioning_DetailView")]
    [AdditionalViewControlsRule(Captions.ConditionalViewControlsPositioningForCustomerCity, "1=1", "1=1", null,
        Position.DetailViewItem, MessageProperty = "CityWarning", View = "ConditionalViewControlsPositioning_DetailView")]
    [CloneView(CloneViewType.DetailView, "ConditionalViewControlsPositioning_DetailView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Additional View Controls/Conditional View Controls Positioning",
        "ConditionalViewControlsPositioning_DetailView")]
    [DisplayFeatureModelAttribute("ConditionalViewControlsPositioning_DetailView")]
    #endregion
    #region ConditionalControlAndMessage
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderConditionalControlAndMessage, "1=1", "1=1",
        Captions.HeaderConditionalControlAndMessage, Position.Top,
        View = "ConditionalControlAndMessage_ListView")]
    [AdditionalViewControlsRule(Captions.ConditionalAdditionalViewControlAndMessage, "Orders.Count>7", "1=1", null,
        Position.Bottom, ViewType = ViewType.ListView, MessageProperty = "ConditionalControlAndMessage",
        View = "ConditionalControlAndMessage_ListView")]
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderConditionalControlAndMessage, "1=1", "1=1",
        Captions.ViewMessageAdditionalViewControls, Position.Bottom, ViewType = ViewType.ListView,
        View = "ConditionalControlAndMessage_ListView",
        ExecutionContextGroup = "ConditionalControlAndMessage")]
    [CloneView(CloneViewType.ListView, "ConditionalControlAndMessage_ListView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Additional View Controls/Conditional control with conditional Message",
        "ConditionalControlAndMessage_ListView")]
    [DisplayFeatureModelAttribute("ConditionalControlAndMessage_ListView")]
    #endregion
    #region Add Runtime Fields From Model
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderRuntimeMemberFromModel, "1=1", "1=1",
        Captions.ViewMessageRuntimeMemberFromModel, Position.Bottom, ViewType = ViewType.ListView,
        View = "RuntimeFieldsFromModel_ListView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderRuntimeMemberFromModel, "1=1", "1=1",
        Captions.HeaderRuntimeMemberFromModel, Position.Top, View = "RuntimeFieldsFromModel_ListView")]
    [CloneView(CloneViewType.DetailView, "RuntimeFieldsFromModel_DetailView")]
    [CloneView(CloneViewType.ListView, "RuntimeFieldsFromModel_ListView",
        DetailView = "RuntimeFieldsFromModel_DetailView")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Add Runtime Fields From Model", "RuntimeFieldsFromModel_ListView")]
    [DisplayFeatureModelAttribute("RuntimeFieldsFromModel_ListView", "RuntimeFieldsFromModel")]
    [DisplayFeatureModelAttribute("RuntimeFieldsFromModel_DetailView", "RuntimeFieldsFromModel")]
    #endregion
    public class Customer : CustomerBase {
        public Customer(Session session) : base(session) {
        }

        [Association("Customer-Orders")]
        public XPCollection<Order> Orders {
            get { return GetCollection<Order>("Orders"); }
        }
        private DateTime _birthDate;
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime BirthDate {
            get { return _birthDate; }
            set { SetPropertyValue("BirthDate", ref _birthDate, value); }
        }
        [Browsable(false)]
        public string ConditionalControlAndMessage{
            get { return "Customer " + Name + " has less than " + Orders.Count; }
        }

        [Browsable(false)]
        public string NameWarning {
            get { return (Name + "").Length > 20 ? "Who gave him this name!!! " + Name : null; }
        }

        [Browsable(false)]
        public string CityWarning {
            get { return (City + "").Length < 3 ? "Last week I was staying at " + City : null; }
        }
        
        [CustomQueryProperties("DisplayableProperties", "Name_City;Orders_Last_OrderDate")]
        public static IQueryable EmployeesLinq(Session session)
        {
            return new XPQuery<Customer>(session).Select(customer =>
                                                         new
                                                         {
                                                             Name_City = customer.Name + " " + customer.City,
                                                             Orders_Last_OrderDate = customer.Orders.Max(order => order.OrderDate)
                                                         });
        }
        
    }
}