using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace SystemTester.Module.Web.FunctionalTests.PropertyEditors.ASPxTokenListPropertyEditor {
    public class ASPxTokenListPropertyEditorUpdater :ModuleUpdater{
        public ASPxTokenListPropertyEditorUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion){
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (!ObjectSpace.GetObjectsQuery<ASPxTokenListPropertyEditorObject>().Any()){
                
                var childObject = ObjectSpace.CreateObject<ASPxTokenListPropertyEditorChildObject>();
                childObject.Name = "aaaa";
                
                childObject = ObjectSpace.CreateObject<ASPxTokenListPropertyEditorChildObject>();
                childObject.Name = "bbbb";
                
                ObjectSpace.CommitChanges();
            }
        }
    }
}
