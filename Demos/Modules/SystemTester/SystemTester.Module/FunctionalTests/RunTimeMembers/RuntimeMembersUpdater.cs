using System;
using SystemTester.Module.FunctionalTests.RunTimeMembers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;

namespace SystemTester.Module.FunctionalTests.RuntimeMembers {
    public class RuntimeMembersUpdater : ModuleUpdater {
        public RuntimeMembersUpdater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<RunTimeMembersObject>(null)==null){
                var runtimeMembersObject = ObjectSpace.CreateObject<RunTimeMembersObject>();
                runtimeMembersObject.FirstName = "Apostolis";
                runtimeMembersObject.LastName = "Bekiaris";

                var address = ObjectSpace.CreateObject<Address>();
                address.Country = ObjectSpace.CreateObject<Country>();
                address.Country.Name = "Greece";
            }

        }
    }
}
