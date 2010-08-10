namespace FeatureCenter.Module.Win
{
    public class Captions:Module.Captions
    {
        public const string HeaderAutoCommitListView = "Auto Commit ListView";
        public const string HeaderApplicationMultipleInstances = "Application Multiple Instances";
        public const string HeaderControlXtraGrid = "Controlling XtraGrid at Runtime";
        public const string HeaderFilterControl = "Filter Control";
        public const string HeaderAutoExpandNewRow = "Auto Expand New Row";
        public const string HeaderMasterDetail = "Master Detail at any level";
        public const string HeaderConditionalDetailGridViews = "Conditional Detail GridViews";
        public const string HeaderControlXtraGridColumns = "Control XtraGrid Columns";
        public const string HeaderGuessAutoFilterRowValuesFromFilter = "Guess Auto Filter Row Values From Filter";
        public const string HeaderLogOut = "Log Out";
        public const string HeaderCursorPosition = "Cursor Position";
        public const string HeaderFocusControl = "Focus Control";
        public const string HeaderHideGridPopUpMenu = "Hide Grid PopUp Menu";
        public const string HeaderTrayIcon = "Tray Icon";
        public const string HeaderTabStopForReadOnly = "Tab Stop For Read Only";

        public const string ViewMessageAutoCommitListView = "Make a a change to one record and navigate to next one so the changes you have make to saved automatically";
        public const string ViewMessageApplicationMultipleInstances = "FeatureCenter has been set up using Options/ApplicationMultipleInstances attribute to not to allow multi instances. To test it just click on the show another instance action. This feature does not work in debug mode";
        public const string ViewMessageControlXtraGrid = "In this view you can see how you can control XtraGrid at runtime. Expand exposes all grid options per listview. For this view i have change the following options AllowColumnMoving=false,EnableColumnMenu=false,ShowHorzLines=false,ShowVertLines=false,AnimationType=AnimateAllContent,ShowFilterPanel=true,ShowFilterPanelMode=Always,UseTabKey=true,ShowColumnHeaders=false";
        public const string ViewMessageFilterControl = "In this view you can see how you can display a filter expression editor at a specified position";
        public const string ViewMessageAutoExpandNewRow = "To test this view just write something in the new item row and hit enter. Then the child gridview will be automatically expanded for you and focused to the child view new item row so you can continue addinf records";
        public const string ViewMessageMasterDetail1 =
            "This view demo eXpand master-detail at any level feature see MDCustomer_ListView. You can configure all options of child views from your model.Take a special note that for the 2nd level view (orders) the columsheader are not shown using gridviewoptions node at MDOrder_ListView ";
        public const string ViewMessageMasterDetail2 =
            "For child views all controllers that are specific to that view and will run but some behaviour that comes directly through gridlisteditor will not. See how i have enable NewItemRowPosition for Order,OrderLine child views using NewItemRowListViewController";
        public const string ViewMessageConditionalDetailGridViews = "Expand any detail view for customer that lives in Paris to notice that is different from any other detail view for other cities";
        public const string ViewMessageControlXtraGridColumns = "In this view you can see how you can control all options for each gridvolumn. For example I have set the autofiltercondition=contains and ImmediateUpdateAutoFilter=false(works better on large number of records) for the Name column, also for the City column i have set AllowAutoFilter=false";
        public const string ViewMessageGuessAutoFilterRowValuesFromFilter = "In this view you can see City column autofilter row value automatically have the value that comes from the listview active filter string";
        public const string ViewMessageLogOut = "In this view the same action that is avaliable at File/Logout has been added here so you can logout current user";
        public const string ViewMessageCursorPosition = "Using tab navigate through property editors and note that when Name is focus the cursor moves to the end, but for city all context remain selectd and for description cursor moves to start";
        public const string ViewMessageFocusControl = "Hit Control+G on your keyboard to focus the control that is in the hidden tab";
        public const string ViewMessageHideGridPopUpMenu = "Right click on the listview and the popup will never show";
        public const string ViewMessageTrayIcon = "As you can see at the right bottom conrner (tray taskbar) a tray icon with popup context menu is already enable for this application";
        public const string ViewMessageTabStopForReadOnly = "Using TAB navigate through editors and note that for city that is reaonly you spent one tab";
    }
}
