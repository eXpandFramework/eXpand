using System.Data.SqlClient;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Controllers;
using eXpand.Persistent.Base.PersistentMetaData;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace eXpand.ExpressApp.WorldCreator.SqlDBMapper
{
    public class MapController:ViewController<DetailView>
    {
        public MapController() {
            TargetObjectType = typeof (IPersistentAssemblyInfo);
            
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            var assemblyToolsController = Frame.GetController<AssemblyToolsController>();
            if (assemblyToolsController.SingleChoiceAction.Items.Find("MapSqlDB")== null) {
                assemblyToolsController.SingleChoiceAction.Items.Add(new ChoiceActionItem("Map Database", "MapSqlDB"));
                assemblyToolsController.SingleChoiceAction.Execute +=AssemblyToolsControllerOnToolExecuted;
            }
        }

        void AssemblyToolsControllerOnToolExecuted(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            if ((string) singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data=="MapSqlDB") {
                var persistentAssemblyInfo = (IPersistentAssemblyInfo)singleChoiceActionExecuteEventArgs.CurrentObject;
                var objectSpace = new ObjectSpace(new UnitOfWork(ObjectSpace.Session.DataLayer),XafTypesInfo.Instance);
                IPersistentAssemblyInfo assemblyInfo = GetMappedAssemblyInfo(objectSpace, persistentAssemblyInfo);
                Show(singleChoiceActionExecuteEventArgs, objectSpace, assemblyInfo);
            }
        }

        void Show(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs, ObjectSpace objectSpace, IPersistentAssemblyInfo assemblyInfo) {
            ShowViewParameters showViewParameters = singleChoiceActionExecuteEventArgs.ShowViewParameters;
            showViewParameters.TargetWindow=TargetWindow.Current;
            showViewParameters.CreatedView = Application.CreateDetailView(objectSpace, assemblyInfo, View.IsRoot);
        }

        IPersistentAssemblyInfo GetMappedAssemblyInfo(ObjectSpace objectSpace, IPersistentAssemblyInfo persistentAssemblyInfo) {
            const string ConnectionString = @"Integrated Security=SSPI;Pooling=false;Data Source=.\SQLExpress;Initial Catalog=mce;";
            var cn = new SqlConnection(ConnectionString);
            var server = new Server(new ServerConnection(cn));
            Database _database = server.Databases[cn.Database];
            _database.Refresh();
            IPersistentAssemblyInfo assemblyInfo = objectSpace.GetObject(persistentAssemblyInfo);
            var dbMapper = new DbMapper(objectSpace, assemblyInfo);
            dbMapper.Map(_database);
            return assemblyInfo;
        }
    }
}
