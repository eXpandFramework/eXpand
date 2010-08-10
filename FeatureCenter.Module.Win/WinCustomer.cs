using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.MasterDetail.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win
{
    #region Guess Auto FilterRow Values
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderGuessAutoFilterRowValuesFromFilter, "1=1", "1=1",
            Captions.ViewMessageGuessAutoFilterRowValuesFromFilter, Position.Bottom, View = "GuessAutoFilterRowValues_ListView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderGuessAutoFilterRowValuesFromFilter, "1=1", "1=1",
        Captions.HeaderGuessAutoFilterRowValuesFromFilter, Position.Top, View = "GuessAutoFilterRowValues_ListView")]
    [CloneView(CloneViewType.ListView, "GuessAutoFilterRowValues_ListView")]
    [NavigationItem("Controlling XtraGrid/Guess Auto FilterRow Values", "GuessAutoFilterRowValues_ListView")]
    [DisplayFeatureModelAttribute("GuessAutoFilterRowValues_ListView")]
    #endregion

    #region XtraGridViewOptions
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderControlXtraGrid, "1=1", "1=1",
        Captions.ViewMessageControlXtraGrid, Position.Bottom, View = "XtraGridViewOptions_ListView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderControlXtraGrid, "1=1", "1=1",
        Captions.HeaderControlXtraGrid, Position.Top, View = "XtraGridViewOptions_ListView")]
    [CloneView(CloneViewType.ListView, "XtraGridViewOptions_ListView")]
    [NavigationItem("Controlling XtraGrid/GridView options", "XtraGridViewOptions_ListView")]
    [DisplayFeatureModelAttribute("XtraGridViewOptions_ListView")]
    #endregion

    #region XtraGridColumnsOptions
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderControlXtraGridColumns, "1=1", "1=1",
        Captions.ViewMessageControlXtraGridColumns, Position.Bottom, View = "XtraGridColumnsOptions_ListView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderControlXtraGridColumns, "1=1", "1=1",
        Captions.HeaderControlXtraGridColumns, Position.Top, View = "XtraGridColumnsOptions_ListView")]
    [CloneView(CloneViewType.ListView, "XtraGridColumnsOptions_ListView")]
    [NavigationItem("Controlling XtraGrid/Column options", "XtraGridColumnsOptions_ListView")]
    [DisplayFeatureModelAttribute("XtraGridColumnsOptions_ListView")]
    #endregion

    #region Tab Stop For ReadOnly
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderTabStopForReadOnly, "1=1", "1=1",
        Captions.ViewMessageTabStopForReadOnly, Position.Bottom, View = "TabStopForReadOnly_DetailView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderTabStopForReadOnly, "1=1", "1=1",
        Captions.HeaderTabStopForReadOnly, Position.Top, View = "TabStopForReadOnly_DetailView")]
    [CloneView(CloneViewType.DetailView, "TabStopForReadOnly_DetailView")]
    [NavigationItem("Navigation/Tab Stop For ReadOnly", "TabStopForReadOnly_DetailView")]
    [DisplayFeatureModelAttribute("TabStopForReadOnly_DetailView")]
    #endregion

    #region Conditional Master Detail Views
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderConditionalDetailGridViews, "1=1", "1=1",
        Captions.ViewMessageConditionalDetailGridViews, Position.Bottom, Nesting = Nesting.Root,
        ViewType = ViewType.ListView, View = "ConditionalMasterDetailCustomer_ListView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderConditionalDetailGridViews, "1=1", "1=1",
        Captions.HeaderConditionalDetailGridViews, Position.Top, Nesting = Nesting.Root, ViewType = ViewType.ListView, View = "ConditionalMasterDetailCustomer_ListView")]
    [MasterDetail("Customer_Orders_For_All_Other_Cities", "1=1", "ConditionalMasterDetailOrder_ListView", "Orders", View = "ConditionalMasterDetailCustomer_ListView")]
    [CloneView(CloneViewType.ListView, "ConditionalMasterDetailCustomer_ListView")]
    [NavigationItem("Controlling XtraGrid/Master Detail/Conditional Detail views", "ConditionalMasterDetailCustomer_ListView")]
    [DisplayFeatureModelAttribute("ConditionalMasterDetailCustomer_ListView", "ConditionalDetailViews")]
    #endregion

    #region Master Detail At Any Level
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderMasterDetail + "1", "1=1", "1=1",
        Captions.ViewMessageMasterDetail1, Position.Bottom, ViewType = ViewType.ListView, View = "MasterDetailAtAnyLevelCustomer_ListView")]
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderMasterDetail + "2", "1=1", "1=1",
        Captions.ViewMessageMasterDetail2, Position.Bottom, ViewType = ViewType.ListView, View = "MasterDetailAtAnyLevelCustomer_ListView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderMasterDetail, "1=1", "1=1",
        Captions.HeaderMasterDetail, Position.Top, View = "MasterDetailAtAnyLevelCustomer_ListView")]
    [MasterDetail("AtAnyLevelCustomer_Orders", "1=1", "MasterDetailAtAnyLevelOrder_ListView", "Orders", View = "MasterDetailAtAnyLevelCustomer_ListView")]
    [CloneView(CloneViewType.ListView, "MasterDetailAtAnyLevelCustomer_ListView")]
    [NavigationItem("Controlling XtraGrid/Master Detail/At any level", "MasterDetailAtAnyLevelCustomer_ListView")]
    [DisplayFeatureModelAttribute("MasterDetailAtAnyLevelCustomer_ListView", "AtAnyLevel")]
    #endregion
    #region Auto Expand new row
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderAutoExpandNewRow, "1=1", "1=1",
        Captions.ViewMessageAutoExpandNewRow, Position.Bottom, Nesting = Nesting.Root,ViewType = ViewType.ListView, View = "AutoExpandNewRowCustomer_ListView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderAutoExpandNewRow, "1=1", "1=1",
        Captions.HeaderAutoExpandNewRow, Position.Top, Nesting = Nesting.Root, ViewType = ViewType.ListView, View = "AutoExpandNewRowCustomer_ListView")]
    [MasterDetail("AutoExpandNewRowCustomer_Orders", "1=1", "AutoExpandNewRowOrder_ListView", "Orders", View = "AutoExpandNewRowCustomer_ListView")]
    [CloneView(CloneViewType.ListView, "AutoExpandNewRowCustomer_ListView")]
    [NavigationItem("Controlling XtraGrid/Master Detail/Auto Expand new row", "AutoExpandNewRowCustomer_ListView")]
    [DisplayFeatureModelAttribute("AutoExpandNewRowCustomer_ListView", "AutoExpandNewRow")]
    #endregion
    #region Hide popup menu
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderHideGridPopUpMenu, "1=1", "1=1",
        Captions.ViewMessageHideGridPopUpMenu, Position.Bottom, View = "HidePopupMenu_ListView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderHideGridPopUpMenu, "1=1", "1=1", Captions.HeaderHideGridPopUpMenu
        , Position.Top, View = "HidePopupMenu_ListView")]
    [CloneView(CloneViewType.ListView, "HidePopupMenu_ListView")]
    [NavigationItem("Controlling XtraGrid/Hide Popupmenu", "HidePopupMenu_ListView")]
    [DisplayFeatureModelAttribute("HidePopupMenu_ListView")]
    #endregion
    #region Focus Control
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderFocusControl, "1=1", "1=1",
        Captions.ViewMessageFocusControl, Position.Bottom, View = "FocusControl_DetailView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderFocusControl, "1=1", "1=1",
        Captions.HeaderFocusControl, Position.Top, View = "FocusControl_DetailView")]
    [CloneView(CloneViewType.DetailView, "FocusControl_DetailView")]
    [NavigationItem("Navigation/Focus Control", "FocusControl_DetailView")]
    [DisplayFeatureModelAttribute("FocusControl_DetailView")]
    #endregion
    #region Filter Control
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderFilterControl, "1=1", "1=1",
        Captions.ViewMessageFilterControl, Position.Bottom, View = "FilterControl_ListView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderFilterControl, "1=1", "1=1",
        Captions.HeaderFilterControl, Position.Top, View = "FilterControl_ListView")]
    [CloneView(CloneViewType.ListView, "FilterControl_ListView")]
    [NavigationItem("Controlling XtraGrid/Filter Control", "FilterControl_ListView")]
    [DisplayFeatureModelAttribute("FilterControl_ListView")]
    #endregion
    #region Cursor Position
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderCursorPosition, "1=1", "1=1",
        Captions.ViewMessageCursorPosition, Position.Bottom, View = "CursorPosition_DetailView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderCursorPosition, "1=1", "1=1",
        Captions.HeaderCursorPosition, Position.Top, View = "CursorPosition_DetailView")]
    [CloneView(CloneViewType.DetailView, "CursorPosition_DetailView")]
    [NavigationItem("Navigation/Cursor Position", "CursorPosition_DetailView")]
    [DisplayFeatureModelAttribute("CursorPosition_DetailView")]
    #endregion
    #region Auto Commit List View
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderAutoCommitListView, "1=1", "1=1",
        Captions.ViewMessageAutoCommitListView, Position.Bottom, View = "AutoCommit_ListView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderAutoCommitListView, "1=1", "1=1",
        Captions.HeaderAutoCommitListView, Position.Top, View = "AutoCommit_ListView")]
    [CloneView(CloneViewType.ListView, "AutoCommit_ListView")]
    [NavigationItem("Controlling XtraGrid/Auto Commit List View","AutoCommit_ListView")]
    [DisplayFeatureModelAttribute("AutoCommit_ListView", "AutoCommitListView")]
    #endregion
    public class WinCustomer:CustomerBase
    {
        public WinCustomer(Session session) : base(session) {
        }
        [Association("WinCustomer-WinOrders")]
        public XPCollection<WinOrder> Orders
        {
            get
            {
                return GetCollection<WinOrder>("Orders");
            }
        }
    }
}
