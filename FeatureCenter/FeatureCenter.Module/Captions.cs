namespace FeatureCenter.Module {
    public class Captions {
        public const string HeaderUpdateMembers = "Update Members";
        public const string Importexport = "ImportExport/";
        public const string Miscellaneous = "Miscellaneous/";
        public const string PropertyEditors = "PropertyEditors/";
        public const string Security = "Security/";
        public const string MemberLevelSecurity = "Member Level";
        public const string Header = "Header";
        public const string HeaderWelcome = "Welcome";
        public const string DetailViewCotrol = "DetailView Cotrol/";
        public const string ListViewCotrol = "ListView Control/";
        public const string TreeListView = "Tree ListView/";
        public const string HeaderPropertyPathFilters = "Property Path Filters";
        public const string HeaderNonPersistent = "Non Persistent Detail View";

        public const string HeaderMultipleDataStores = "Multiple DataStores";
        public const string HeaderActionButtonViewItem = "Action Button ViewItem";
        public const string HeaderControllingListViewSearch = "Controlling ListView Search";
        public const string HeaderControllingDetailViewSearch = "Controlling DetailView Search";
        public const string HeaderDetailViewNavigation = "Navigation to Detail View of Persistent object with records";
        public const string HeaderRuntimeMemberFromModel = "Runtime Member From Model";
        public const string HeaderDisableEditDetailView = "Disable Edit Detail View";
        public const string HeaderConnectionInfoStatus = "Connection Info Status";
        public const string HeaderDisableFullTextForMemoFields = "Disable Full Text For Memo Fields";
        public const string HeaderExpandAbleMembers = "Expand Able Members";
        public const string HeaderFKViolation = "Foreign Key Violation";
        public const string HeaderHideFromNewMenu = "Hide From New Menu";
        public const string HeaderHideListViewToolBar = "Hide ListView ToolBar";
        public const string HeaderHideNestedListViewToolBar = "Hide Nested ListView ToolBar";
        public const string HeaderHideDetailViewToolBar = "Hide DetailView ToolBar";
        public const string HeaderHighlightFocusedLayoutItem = "Highlight Focused Layout Item";
        public const string HeaderLoadWhenFiltered = "Do Not Load When No Filter Exists";
        public const string HeaderLookUpListSearch = "Look Up List Search";
        public const string HeaderUpdateOnlyChangeFields = "Update Only Changed Fields";
        public const string HeaderLinqQuery = "Linq Query";
        public const string ConditionalAdditionalViewControlAndMessage = "Conditional Additional View Control And Message";
        public const string HeaderConditionalControlAndMessage = "Conditional Control And Message";
        public const string HeaderConditionalViewControlsPositioning = "Conditional View Controls Positioning";
        public const string ConditionalViewControlsPositioningForCustomerName = "Conditional View Controls Positioning For Customer Name";
        public const string ConditionalViewControlsPositioningForCustomerCity = "Conditional View Controls Positioning For Customer City";
        public const string HeaderConditionalFKViolation = "Conditional Foreign Key Violations";
        public const string HeaderConditionalSaveDelete = "Conditional Save And Delete";
        public const string HeaderFilterDataStoreSkinFilter = "Skin Filter";
        public const string HeaderFilterDataStoreUserFilter = "User Filter";
        public const string HeaderFilterDataStoreContinentFilter = "Continent Filter";
        public const string HeaderExistentAssemblyMasterDetail = "Existent Assembly Master Detail";
        public const string HeaderShowInAnalysis = "Show In Analysis";
        public const string HeaderConnectWithCustomer = "Connect With Customer";
        public const string HeaderMemberLevelSecurity = "Member Level Security";
        public const string HeaderViewVariants = "View Variants";
        public const string HeaderExceptionHandling = "Exception Handling";
        public const string HeaderNorthWind = "NorthWind";
        public const string HeaderQuartz = "Quartz";
        public const string HeaderRoleDifference = "Role Difference";
        public const string HeaderConditionalDetailViews = "Conditional DetailViews";
        public const string HeaderRuntimeCalculatedFields = "Runtime Calculated Fields";
        public const string HeaderRuntimeCalculatedFieldsFromModel = "Runtime Calculated Fields From Model";
        public const string HeaderExistentAssemblyRuntimeCalculatedField = "Existent Assembly Runtime Calculated Field";
        public const string HeaderOrphanedCollectionFromModel = "Orphaned Collection using model editor";
        public const string HeaderOrphanedCollectionWithCode = "Orphaned Collection With Code";
        public const string HeaderExistentAssemblyRuntimeOrphanedCollection = "Existent Assembly Runtime Orphaned Collection";
        public const string HeaderSequence = "Sequence Numbers";
        public const string HeaderPessimisticLocking = "Pessimistic Locking";
        public const string HeaderTooltip = "Tooltip";
        public const string HeaderCascadingEditors = "Cascading Editors";
        public const string HeaderRuntimeUnboundColumn = "UnboundColumn";
        public const string HeaderLayoutViewGridListEditor = "LayoutView GridListEditor";
        public const string HeaderValidation = "Validation Warnings";


        public const string ViewMessage = "ViewMessage";



        public const string ViewMessageNonPersistent = "This view demo how you can assign a non persistent object as a navigation item";

        public const string ViewMessageMultipleDataStores =
            "The objects in this view are stored in a different data store (FeatureCenterMultipleDataStore.mdf) with the help of DataStoreAttribute (see MDSCustomer.cs file)" +
            " and a connection string on your application config file";

        public const string ViewMessagePropertyPathFilters =
            "This view demo you can use property path to filter your view either at design time or runtime, " +
            "property path filters can be used in combination with listview criteria and you can apply multiple of them in a view. " +
            "Use the Search By action to modify or remove current filter";
        public const string ViewMessageActionButtonViewItem = "In this view you can see how you can position your actions anywhere with out having to write any special code for it. Only by using the model editor see ActionButtonViewItem_DetailView";
        public const string ViewMessageControllingListViewSearch = "In this view i have remove the name and description field of customer out of the fulltext search able fields again using model editor. Try to search for 'Benjamin CISCO' to test the feature";
        public const string ViewMessageControllingDetailViewSearch = "In this view i have add the city field of customer to be searchable so if you execute the search action on the toolbar it is going to search for all customers of the current city. Also if customers found you will notice that the navigation items will be enable and will allow you to navigate through search results";
        public const string ViewMessageDetailViewNavigation = "In this view you can see that you can set a detailview as a navigation item and if records are found it is going to allow you to navigate through them";
        public const string ViewMessageRuntimeMemberFromModel = "In this view you can see how to create a runtime member using your model editor. Go to Admin Navigation group/ Difference and open application model (1st row). There note the diffs of AddRuntimeFieldsFromModelObject";
        public const string ViewMessageDisableEditDetailView = "In this view can see how you can disable edit for a detail view by using the AllowEdit attribute of a detailview";
        public const string ViewMessageConnectionInfoStatus = "In this view can how you can display the current connection info to the datastore at the status bar by using the options/ConnectionInfoMessage attribute (see bottom left corner of this window";
        public const string ViewMessageDisableFullTextForMemoFields = "In this view can how you can disable full text for memo fields so you can speed up your search queries. eg search for 'description' to test it. It should return no records even if all customers descriptions have the word 'description'";
        public const string ViewMessageExpandAbleMembers = "In this view can see that expand will create automatically any reference object that marked with ExpandObjectMembers attribute. By default xaf requires to hadle that by code";
        public const string ViewMessageFKViolation = "In this view can see how can you disable deleting an object that has childs. To test it just try to delete a customer";
        public const string ViewMessageHideFromNewMenu = "In this view can see how to hide an object from new menu by decorating the class with the HideFromNewMenuAttribute, just click on the new menu to see that there is not entry for the currectobject which name is HideFromNewMenuObject";
        public const string ViewMessageHideListViewToolBar = "In this view can see how to hide toolbars of list view";
        public const string ViewMessageHideListViewToolBarNested = "In this view can see how to hide toolbars of a nested list view";
        public const string ViewMessageHideDetailViewToolBar = "In this view can see how to a DetailView toolbar ";
        public const string ViewMessageHighlightFocusedLayoutItem = "In this view can see how to Highlight Focused Layout Item";
        public const string ViewMessageLoadWhenFiltered = "In this view to speed start up loading time i have disable loading of records. Loading will only occur when a filter on the grid exists. To test just hit the accept filter button or use the fulltext search action at the top right corner";
        public const string ViewMessageLookUpListSearch = "In this view i have enable customer lookup search for customer lookup independent of the count of customer records. By default Xaf shows a a non search able lookup list view if customer records are less than 25 ";
        public const string ViewMessageUpdateOnlyChangeFields = "In this view you can see that only fields that are changed will be updated in the datastore and that will speed up your system. By default xaf updates all fields. To test make a change to Name property for example and hit save , then note at the audit trail info that will update only the name field";
        public const string ViewMessageLinqQuery = "This view demo how you can query your data using linq";
        public const string ViewMessageAdditionalViewControls = "All controls except grid in this view come through model additional view controls rules. Try changing record selection to note a conditional control shown with a conditional message";
        public const string ViewMessageConditionalViewControlsPositioning = "In this view you can see how can can position your additionalviewcontrols anywhere for example try writng more than 20 characters for name field or less than 3 for city. The controls are now viewitems so you are free to right click and choose modify layout to position them where ever you want";
        public const string ViewMessageConditionalFKViolation = "eXpand has a foreign key delete contrain attribute. That means that does not allow object with childs to be deleted if set. That check is executed automatically when an object is deleted. So in order to make it conditional (eg only allow customers that live in Paris to be deleted) I have used a controller state rule here. Try to delete any customer that does not live in Paris and you will be allowed to.";
        public const string ViewMessageConditionalFKViolation2 = "Also for customers that live in Paris using a controllerstate rule again i have disable the browse to their detail view when you click or double click the listview ";
        public const string ViewMessageConditionalSaveDelete = "In this view since Save and delete are expose through actions i have used ActionState rules to disable Save And Delete for Customers that live in Paris (just hit Ctrl+Page Down) to navigate through records and note the state of the actions";
        public const string ViewMessageFilterDataStoreSkinFilter = "Here you can see how to apply a low level filter to your data. To test it try to add a customer with the same name and you note that a unique value validation will be thrown. Then change your current application skin and you see that no dummy data are avaliable for this skin, try to add a customer with the same name and the customer will be added. Lastly go back to the black skin and note that your data remain unchanged";
        public const string ViewMessageFilterDataStoreUserFilter = "Here you can see how to apply a low level filter to your data. To test it try to add a customer with the same name and you note that a unique value validation will be thrown. Then change your current user using the File/Logout action (logon as username:filterbyuser) and you see that no dummy data are avaliable for this user in this view but other dummy data are shared among other views, try to add a customer with the same name and the customer will be added. Finally login again to admin and note that your data remain unchanged";
        public const string ViewMessageFilterDataStoreContinentFilter = "Here you can see how to apply a low level filter to your data. This example is special cause i have modify only the selects statements that go to your datastore and have filter an already created field (city) uppon a range of values which i have categorize to continents. Use the continent single choise above to filter your views. Also name has a rule required validation but since you are modifying statements you are bellow the validation system and that allows you to add customer with the same names but different continents";
        public const string ViewMessageExistentAssemblyMasterDetail = "In this view you can see how can you add a master detail by extending dynamically existent classes that you have design in visual studio";
        public const string ViewMessageShowInAnalysis = "In this view you can see how can you enable the ShowInAnalysis action works similar with show in reports action. Just select some records and execute the action to create an analysis for the selected records. The same behaviour can be applied using the ShowInAnalysis permission";
        public const string ViewMessageConnectWithCustomer = "In this view you can see how you can apply the pivoted attribute to your customer in order to display in the same detail view pre deisigned analysis";
        public const string ViewMessageUpdateMembers = "Here you can see how to use IO import engine to update only the city member of this customer list. Right click and choose import then navigate to the bin directory of the application and select the updatemembers.xml file. After import just hit refresh and note that the city names of your customers have changed. To see how to configure the creation of updatemembers.xml see the Update Members serialization configuration";
        public const string ViewMessageMemberLevelSecurity = "Here you can see how to to apply conditional member level security permissions. eg. For Customers that live in paris Deny Read permission have been applied to their Name and for those that live in New York a Deny Write to their name. Deny read mean that editor will display a 'Protected Content' message instead of their value and Deny write means that the editor will be disabled";
        public const string ViewMessageViewVariants = "Here you can see how you can allow end users to create views at runtime with out giving them access to model editor. Note the viewvariants single choise actions 2 buttons on the right that allow you to edit delete a view and the clone view action";
        public const string ViewMessageExceptionHandling = "Here you can see how you can apply the power full Microsoft Enterpise Logging module to log your exception. FeatureCenter has been configured to log your exceptions at a special log with the name FeatureCenter (see your windows event viewer) You can add more tracelisteners easy using MS EL config tool. There are listener avaliable that can log exceptions to database,xml,WMI,email etc\r\nAlso at your application config at the ConnectionStrings section there is one with the name ExceptionHandlingConnectionString. That is going to be used by eXpand to store your exceptions at database so you can analyze them at any time";
        public const string ViewMessageNorthWind = "In this view you can see the structure of the NorthWind database. This structure has been created by using the tools/mapdatabase action and is pointing to northwind database and not to the feature center. You can map any sql database you want to WC and edit/query/analyze or make reports on it";
        public const string ViewMessageQuartz = "In this view you can see the structure of the Quartz database. This structure has been created by using the tools/mapdatabase action and is pointing to northwind database and not to the feature center. You can map any sql database you want to WC and edit/query/analyze or make reports on it";
        public const string ViewMessageRoleDifference = "In this view you can see how you can design role differences and apply them to a role by just adding an RDO prefix on your xafml file. Browse the source code for more info under the ApplicationDifferences/RoleDifference folder. To test the feature just login as another user and come back to this view to see that the layout is totally different ";
        public const string ViewMessageConditionalDetailViews = "In this view you can see how you can create conditional detail views. For example just double click on a customer that lives in Paris and then to another customer that lives in another town and note that their detailviews are different";
        public const string ViewMessageRuntimeCalculatedFields = "The SumOfOrderTotals field is a dynamically created caclulated property see CreateRuntimeCalculatedFieldController at feature center on how to create such properties using code";
        public const string ViewMessageRuntimeCalculatedFieldsFromModel = "The MaxOfOrderTotals field is a dynamically created caclulated property using the AliasExpression attribute of a runtime model member. See current differences using the model difference action (next to the refresh button)";
        public const string ViewMessageExistentAssemblyRuntimeCalculatedField = "The MinxOfOrderTotals field is a dynamically created caclulated property using the Extended Persistent Classes of WorldCreadtor";
        public const string ViewMessageOrphanedCollectionFromModel = "The OrderLines nested list view you see above displays the is bound to the customer instance. It is a runtime collection of orderlines that is filtered according to the currect customer. Use the the model difference action (next to the refresh button) to see the diffs you have to apply";
        public const string ViewMessageOrphanedCollectionWithCode = "The OrderLinesFromCode collection is a dynamically created collection property see CreateRuntimeOrphanedCollectionController at feature center on how to create such kind of properties";
        public const string ViewMessageExistentAssemblyRuntimeOrphanedCollection = "The OrderLines nested list view you see above displays the is bound to the customer instance. It is a runtime collection of orderlines that is filtered according to the currect customer and has been created using worldcreator ExtendedOrphanedCollection class";
        public const string ViewMessageSequence = "In this view you can see how easy it is to implement sequence numbers (Id property). The sequence generation is done according to SeqEnum. You can also try to open a SequenceOrder object. There the generation is done with the respect to the parent customer object";
        public const string ViewMessagePessimisticLocking = "Use the edit action and make a change to the current object, then use the New Instance action, login as a different user and come back to this view. A message informing you about the locking will be displayed";
        public const string ViewMessageTooltip = "You can set tooltips for the the propertyeditor either from the model or by decorating your property with the Tooltip attribute. Using the model you have greater flexibility. For instance i have set a tooltip for only this view for Name field. Move your over the Name field to see it in action";
        public const string ViewMessageCascadingEditors = "By decorating your properties with some attributes you can control the editor rendering and datasource";
        public const string ViewMessageRuntimeUnboundColumn = "Right click on the header of the Unbound Column and choose Expression Editor to change the expression.";
        public const string ViewMessageLayoutViewGridListEditor = "This is a demo of the Grid Layout View";
        public const string ViewMessageValidation = "Modify a property of this listview and choose save to see a compination of warnings and normal rules in action. You can watch a similar behaviour for the detail view of this object";
    }
}