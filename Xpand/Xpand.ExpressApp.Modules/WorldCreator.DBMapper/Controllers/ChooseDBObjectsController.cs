using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.WorldCreator.DBMapper.BusinessObjects;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.DBMapper.Controllers{
    public class ChooseDBObjectsController : ObjectViewController<DetailView,DBLogonObject> {

        public ChooseDBObjectsController(){
            var simpleAction = new SimpleAction(this, "PopulateDBTables", PredefinedCategory.PopupActions) { Caption = "Populate" };
            simpleAction.Execute += SimpleActionOnExecute;
        }

        public IPersistentAssemblyInfo AssemblyInfo { get; set; }


        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            DBLogonObject.PopulateTables();
        }

        public DBLogonObject DBLogonObject => (DBLogonObject)View.CurrentObject;


        public string[] GetSelectedDBObjects(){
            Frame nestedFrame = View.GetItems<ListPropertyEditor>().Single().Frame;
            var listEditor = ((ListView) nestedFrame.View).Editor;
            string[] selectedTables = listEditor.GetSelectedObjects().Cast<DBObject>().Select(table => table.Name).ToArray();
            return selectedTables;
        }

    }
}