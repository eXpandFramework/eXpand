using System.Data.SqlClient;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Xpand.ExpressApp.WorldCreator.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;
using System.Linq;

namespace Xpand.ExpressApp.WorldCreator.SqlDBMapper {
    public class MapController : ViewController<DetailView> {
        public MapController() {
            TargetObjectType = typeof(IPersistentAssemblyInfo);

        }
        protected override void OnActivated() {
            base.OnActivated();
            var assemblyToolsController = Frame.GetController<AssemblyToolsController>();
            if (assemblyToolsController.SingleChoiceAction.Items.Find("MapSqlDB") == null) {
                assemblyToolsController.SingleChoiceAction.Items.Add(new ChoiceActionItem("Map Database", "MapSqlDB"));
                assemblyToolsController.SingleChoiceAction.Execute += AssemblyToolsControllerOnToolExecuted;
            }
        }

        void AssemblyToolsControllerOnToolExecuted(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            if ((string)singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data == "MapSqlDB") {
                ITypeInfo typeInfo = ReflectionHelper.FindTypeDescendants(XafTypesInfo.Instance.FindTypeInfo(typeof(ISqlMapperInfo))).Single();
                var space = Application.CreateObjectSpace(typeInfo.Type);
                var showViewParameters = singleChoiceActionExecuteEventArgs.ShowViewParameters;
                showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                showViewParameters.CreatedView = Application.CreateDetailView(space, space.CreateObject(typeInfo.Type));
                var dialogController = new DialogController();
                dialogController.AcceptAction.Execute += AcceptActionOnExecute;
                showViewParameters.Controllers.Add(dialogController);
            }
        }

        void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var persistentAssemblyInfo = (IPersistentAssemblyInfo)View.CurrentObject;
            ObjectSpace.CommitChanges();
            var objectSpace = new XPObjectSpace(XafTypesInfo.Instance, XpandModuleBase.XpoTypeInfoSource, () => new UnitOfWork(((XPObjectSpace)ObjectSpace).Session.DataLayer));
            CreateMappedAssemblyInfo(objectSpace, persistentAssemblyInfo, (ISqlMapperInfo)simpleActionExecuteEventArgs.CurrentObject);
            ObjectSpace.Refresh();
            ObjectSpace.SetModified(View.CurrentObject);
        }



        void CreateMappedAssemblyInfo(XPObjectSpace objectSpace, IPersistentAssemblyInfo persistentAssemblyInfo, ISqlMapperInfo sqlMapperInfo) {
            string connectionString = sqlMapperInfo.GetConnectionString();
            var sqlConnection = (SqlConnection)new SimpleDataLayer(XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.None)).Connection;
            var server = new Server(new ServerConnection(sqlConnection));
            Database database = server.Databases[sqlConnection.Database];
            database.Refresh();
            IPersistentAssemblyInfo assemblyInfo = objectSpace.GetObject(persistentAssemblyInfo);
            var dbMapper = new DbMapper(objectSpace, assemblyInfo, sqlMapperInfo);
            dbMapper.Map(database, sqlMapperInfo.MapperInfo);
            objectSpace.CommitChanges();
        }

    }
}
