using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Workflow;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;
using Xpand.ExpressApp.Workflow.ScheduledWorkflows;

namespace Xpand.ExpressApp.Workflow {
    public class XpandWorkflowDefinitionsUpdater : ModuleUpdater {
        const string XamlPropertyName = "Xaml";

        public XpandWorkflowDefinitionsUpdater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        void UpdateVersionInXaml(ITypeInfo objectsTypeInfo, Version oldVersion, Version newVersion) {
            if (objectsTypeInfo != null && objectsTypeInfo.IsPersistent) {
                IMemberInfo xamlMemberInfo = objectsTypeInfo.FindMember(XamlPropertyName);
                if (xamlMemberInfo == null) {
                    throw new MemberNotFoundException(objectsTypeInfo.Type, XamlPropertyName);
                }
                foreach (object objectToUpdate in ObjectSpace.GetObjects(objectsTypeInfo.Type)) {
                    var currentXaml = xamlMemberInfo.GetValue(objectToUpdate) as string;
                    string updatedXaml = WorkflowDefinitionsUpdater.UpdateDxAssembliesVersions(currentXaml, oldVersion,newVersion);
                    xamlMemberInfo.SetValue(objectToUpdate, updatedXaml);
                    ObjectSpace.SetModified(objectToUpdate);
                }
                if (ObjectSpace.IsModified) {
                    ObjectSpace.CommitChanges();
                }
            }
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            Version currentModuleVersion = typeof (WorkflowModule).Assembly.GetName().Version;
            var workflowModuleInfo = ObjectSpace.FindObject<ModuleInfo>(new BinaryOperator("Name", "WorkflowModule"));
            if (workflowModuleInfo != null) {
                var dbModuleVersion = new Version(workflowModuleInfo.Version);
                if (dbModuleVersion < currentModuleVersion) {
                    if (dbModuleVersion.Major != currentModuleVersion.Major ||
                        dbModuleVersion.Minor != currentModuleVersion.Minor) {
                        UpdateVersionInXaml(XafTypesInfo.Instance.FindTypeInfo(typeof (ScheduledWorkflow)),
                                            dbModuleVersion, currentModuleVersion);
                        UpdateVersionInXaml(XafTypesInfo.Instance.FindTypeInfo(typeof (ObjectChangedWorkflow)),
                                            dbModuleVersion, currentModuleVersion);
                    }
                }
            }
        }
    }
}