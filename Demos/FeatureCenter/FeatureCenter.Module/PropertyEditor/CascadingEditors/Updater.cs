using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;

namespace FeatureCenter.Module.PropertyEditor.CascadingEditors {
    public class Updater : FCUpdater {


        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {

        }



        public override void UpdateDatabaseAfterUpdateSchema() {

            if (ObjectSpace.FindObject<CascadingPropertyEditorObject>(null) == null) {
                var cascadingPropertyEditorObject = ObjectSpace.CreateObject<CascadingPropertyEditorObject>();
                cascadingPropertyEditorObject.ObjectType = typeof(Analysis);
                cascadingPropertyEditorObject.PropertyName = "Name";
                cascadingPropertyEditorObject.Value = "some name";

                cascadingPropertyEditorObject = ObjectSpace.CreateObject<CascadingPropertyEditorObject>();
                cascadingPropertyEditorObject.ObjectType = typeof(Analysis);
                cascadingPropertyEditorObject.PropertyName = "GCRecord";
                cascadingPropertyEditorObject.Value = 1;
                ObjectSpace.CommitChanges();
            }
        }
    }
}
