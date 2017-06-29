using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace SystemTester.Module.FunctionalTests.TrackModifiedProperties {
    public class TrackModifiedProperties:ObjectViewController<DetailView,TrackModifiedPropertiesObject>{
        public TrackModifiedProperties(){
            var simpleAction = new SimpleAction(this, "TrackModifiedProperties", PredefinedCategory.View){Caption = "Modify"};
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            using (var objectSpace = Application.CreateObjectSpace()){
                var objectByKey = objectSpace.GetObjectByKey(View.ObjectTypeInfo.Type,ObjectSpace.GetKeyValue(View.CurrentObject));
                View.ObjectTypeInfo.FindMember("Name2").SetValue(objectByKey,"modify");
                objectSpace.CommitChanges();
            }
        }
    }
}
