using System.Data.SqlClient;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using eXpand.ExpressApp.WorldCreator.Controllers;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.SqlDBMapper;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using eXpand.ExpressApp.Core;

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
                ObjectSpace space = Application.CreateObjectSpace();
                var showViewParameters = singleChoiceActionExecuteEventArgs.ShowViewParameters;
                showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                showViewParameters.CreatedView = Application.CreateDetailView(space, space.CreateObjectFromInterface<IDataStoreLogonObject>());
                var dialogController = new DialogController();
                dialogController.AcceptAction.Execute +=AcceptActionOnExecute;   
                showViewParameters.Controllers.Add(dialogController);   
            }
        }

        void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var persistentAssemblyInfo = (IPersistentAssemblyInfo)View.CurrentObject;
            var objectSpace = new ObjectSpace(new UnitOfWork(ObjectSpace.Session.DataLayer), XafTypesInfo.Instance);
            CreateMappedAssemblyInfo(objectSpace, persistentAssemblyInfo, (IDataStoreLogonObject)simpleActionExecuteEventArgs.CurrentObject);
            ObjectSpace.Refresh();
            ObjectSpace.SetModified(View.CurrentObject);
        }



        void CreateMappedAssemblyInfo(ObjectSpace objectSpace, IPersistentAssemblyInfo persistentAssemblyInfo, IDataStoreLogonObject dataStoreLogonObject) {
            string connectionString=dataStoreLogonObject.GetConnectionString();
            var sqlConnection = (SqlConnection)new SimpleDataLayer(XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.None)).Connection;
            var server = new Server(new ServerConnection(sqlConnection));
            Database database = server.Databases[sqlConnection.Database];
            database.Refresh();
            IPersistentAssemblyInfo assemblyInfo = objectSpace.GetObject(persistentAssemblyInfo);
            var dbMapper = new DbMapper(objectSpace, assemblyInfo);
            dbMapper.Map(database);
            objectSpace.CommitChanges();
        }

    }
}
