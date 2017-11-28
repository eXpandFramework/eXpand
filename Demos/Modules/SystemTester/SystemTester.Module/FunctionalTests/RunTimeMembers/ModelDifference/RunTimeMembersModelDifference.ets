#DropDB SystemTesterEasyTest
#Application SystemTester.Win
#Application SystemTester.Web
*LogOn
*ScreenCapture


;---------update shared model xml with runtime member data-----------------------
*Navigate Model Difference
*Navigate Model Difference

*Navigate Model Difference
*XpandProcessRecord 
 User Name = Shared Model Difference
 Action = Edit
*XpandProcessRecord Aspects
 Name = (Default language)
 Action = Edit
*XpandFillForm
 Xml = <Application><BOModel><Class Name="SystemTester.Module.FunctionalTests.RunTimeMembers.RunTimeMembersObject"><OwnMembers><RuntimeMember Name="SharedRuntime" DataStoreColumnCreated="False" Type="System.Boolean" CreatedAtDesignTime="False" IsNewNode="True" /></OwnMembers></Class></BOModel><Views><DetailView Id="RuntimeMembersModelDifferenceObject_DetailView"><Items><PropertyEditor Id="SharedRuntime" PropertyName="SharedRuntime" IsNewNode="True" /></Items><Layout><LayoutGroup Id="Main" RelativeSize="13.716814159292035"><LayoutGroup Id="SimpleEditors" RelativeSize="100"><LayoutGroup Id="RunTimeMembersObject" RelativeSize="100"><LayoutItem Id="FirstName" ViewItem="FirstName" RelativeSize="51.612903225806448" /><LayoutItem Id="LastName" ViewItem="LastName" RelativeSize="48.387096774193552" /></LayoutGroup></LayoutGroup></LayoutGroup><LayoutItem Id="SharedRuntime" ViewItem="SharedRuntime" Index="1" RelativeSize="86.283185840707958" IsNewNode="True" /></Layout></DetailView><ListView Id="RuntimeMembersModelDifferenceObject_ListView"><Columns><ColumnInfo Id="SharedRuntime" PropertyName="SharedRuntime" IsNewNode="True" /></Columns></ListView></Views></Application>
*SaveAndClose
*SaveAndClose

;------------check if member is created ok------------------
*ReopenApplication
*LogOn
*Action Navigation(RuntimeMembers.ModelDifference)
*Action New
*XpandFillForm
 First Name = shared
 Shared Runtime = True
*SaveAndClose
*XpandProcessRecord 
 First Name = shared
 Shared Runtime = True


;---------update User model xml with runtime member data-----------------------
*Navigate Model Difference
*XpandProcessRecord 
 User Name = Admin
 Action = Edit
*XpandProcessRecord Aspects
 Name = (Default language)
 Action = Edit
*XpandFillForm
 Xml = <Application><BOModel><Class Name="SystemTester.Module.FunctionalTests.RunTimeMembers.RunTimeMembersObject"><OwnMembers><RuntimeMember Name="UserRuntime" DataStoreColumnCreated="False" Type="System.Boolean" CreatedAtDesignTime="False" IsNewNode="True" /></OwnMembers></Class></BOModel><Views><DetailView Id="RuntimeMembersModelDifferenceObject_DetailView"><Items><PropertyEditor Id="UserRuntime" PropertyName="UserRuntime" IsNewNode="True" /></Items><Layout><LayoutGroup Id="Main" RelativeSize="13.716814159292035"><LayoutGroup Id="SimpleEditors" RelativeSize="100"><LayoutGroup Id="RunTimeMembersObject" RelativeSize="100"><LayoutItem Id="FirstName" ViewItem="FirstName" RelativeSize="51.612903225806448" /><LayoutItem Id="LastName" ViewItem="LastName" RelativeSize="48.387096774193552" /></LayoutGroup></LayoutGroup></LayoutGroup><LayoutItem Id="UserRuntime" ViewItem="UserRuntime" Index="1" RelativeSize="86.283185840707958" IsNewNode="True" /></Layout></DetailView><ListView Id="RuntimeMembersModelDifferenceObject_ListView"><Columns><ColumnInfo Id="UserRuntime" PropertyName="UserRuntime" IsNewNode="True" /></Columns></ListView></Views></Application>
*SaveAndClose
*SaveAndClose

;------------check if member is created ok------------------
*ReopenApplication
*LogOn
*Action Navigation(RuntimeMembers.ModelDifference)
*Action New
*XpandFillForm
 First Name = user
 User Runtime = True
*SaveAndClose
*XpandProcessRecord 
 First Name = user
 User Runtime = True



