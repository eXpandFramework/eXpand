#DropDB ModelDifferenceTesterEasyTest
;#Application ModelDifferenceTester.Win
#Application ModelDifferenceTester.Web
*UseModel
*LogOn
*ScreenCapture

;------add a runtime member to the application model--------
*Navigate Application Difference
*XpandProcessRecord
 Name = ModelDifferenceTester*
 Action = Edit
*Action Xml Content
*FillForm
 Xml Content = <Application Title="ModelDifferenceTester" Logo="ExpressAppLogo"><BOModel><Class Name="ModelDifferenceTester.Module.FunctionalTests.RuntimeMembers.RuntimeMembersObject"><OwnMembers><RuntimeMember Name="AppModelRuntime" IsCustom="True" Type="System.Boolean" CreatedAtDesignTime="False" IsNewNode="True" /></OwnMembers></Class></BOModel><Views><DetailView Id="RuntimeMembersObject_DetailView"><Items><PropertyEditor Id="@4c6b5e38-db10-4ef6-a4ff-ecd33b58aa58" PropertyName="AppModelRuntime" IsNewNode="True" /></Items><Layout><LayoutGroup Id="Main" RelativeSize="5.1630434782608692"><LayoutGroup Id="SimpleEditors" RelativeSize="100"><LayoutGroup Id="RuntimeMembersObject" RelativeSize="100"><LayoutItem Id="Name" RelativeSize="100" /></LayoutGroup></LayoutGroup></LayoutGroup><LayoutItem Id="@4c6b5e38-db10-4ef6-a4ff-ecd33b58aa58" ViewItem="@4c6b5e38-db10-4ef6-a4ff-ecd33b58aa58" Index="1" RelativeSize="94.836956521739125" IsNewNode="True" /></Layout></DetailView><ListView Id="RuntimeMembersObject_ListView"><Columns><ColumnInfo Id="@13ec572f-f8a8-4881-9853-bf9905746a17" PropertyName="AppModelRuntime" IsNewNode="True" /></Columns></ListView></Views></Application>
*SaveAndClose

;----------------check creation---------
*LogOff
*LogOn
*Navigate Runtime Members Object
*Action New
*XpandFillForm
 App Model Runtime = True
*SaveAndClose
*XpandProcessRecord
 App Model Runtime = True

;------add a runtime member to the user model--------
*Navigate Application Difference
*XpandProcessRecord
 Name = *Admin*
 Action = Edit
*Action Xml Content
*FillForm
 Xml Content = <Application Title="ModelDifferenceTester" Logo="ExpressAppLogo"><BOModel><Class Name="ModelDifferenceTester.Module.FunctionalTests.RuntimeMembers.RuntimeMembersObject"><OwnMembers><RuntimeMember Name="UserModelRuntime" IsCustom="True" Type="System.Boolean" CreatedAtDesignTime="False" IsNewNode="True" /></OwnMembers></Class></BOModel><Views><DetailView Id="RuntimeMembersObject_DetailView"><Items><PropertyEditor Id="@4c6b5e38-db10-4ef6-a4ff-ecd33b58aa58" PropertyName="UserModelRuntime" IsNewNode="True" /></Items><Layout><LayoutGroup Id="Main" RelativeSize="5.1630434782608692"><LayoutGroup Id="SimpleEditors" RelativeSize="100"><LayoutGroup Id="RuntimeMembersObject" RelativeSize="100"><LayoutItem Id="Name" RelativeSize="100" /></LayoutGroup></LayoutGroup></LayoutGroup><LayoutItem Id="@4c6b5e38-db10-4ef6-a4ff-ecd33b58aa58" ViewItem="@4c6b5e38-db10-4ef6-a4ff-ecd33b58aa58" Index="1" RelativeSize="94.836956521739125" IsNewNode="True" /></Layout></DetailView><ListView Id="RuntimeMembersObject_ListView"><Columns><ColumnInfo Id="@13ec572f-f8a8-4881-9853-bf9905746a17" PropertyName="UserModelRuntime" IsNewNode="True" /></Columns></ListView></Views></Application>
*SaveAndClose

;----------------check creation---------
*Navigate Runtime Members Object
*Action New
*XpandFillForm
 User Model Runtime = True
*SaveAndClose
*XpandProcessRecord
 User Model Runtime = True
 