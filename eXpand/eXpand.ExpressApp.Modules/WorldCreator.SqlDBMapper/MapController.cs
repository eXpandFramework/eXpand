using System.Data.SqlClient;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
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
            assemblyToolsController.SingleChoiceAction.Items.Add(new ChoiceActionItem("Map Database", "MapSqlDB"));
            assemblyToolsController.SingleChoiceAction.Execute +=AssemblyToolsControllerOnToolExecuted;
        }

        void AssemblyToolsControllerOnToolExecuted(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            if ((string) singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data=="MapSqlDB") {
                var persistentAssemblyInfo = (IPersistentAssemblyInfo)singleChoiceActionExecuteEventArgs.CurrentObject;
                const string ConnectionString = @"Integrated Security=SSPI;Pooling=false;Data Source=.\SQLExpress;Initial Catalog=testsimple;Application Name=testsimple";
                var cn = new SqlConnection(ConnectionString);
                var server = new Server(new ServerConnection(cn));
                Database _database = server.Databases[cn.Database];
                _database.Refresh();
                var dbMapper = new DbMapper(ObjectSpace, persistentAssemblyInfo);
                dbMapper.Map(_database);
            }
        }

    }
}
