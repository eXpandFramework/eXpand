using System.ComponentModel;

using DevExpress.ExpressApp;

namespace Solution3.Module.Win
{
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class Solution3WindowsFormsModule : ModuleBase
    {
        public Solution3WindowsFormsModule()
        {
            InitializeComponent();
            
        }
        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            
//            var worldCreatorWinModule = moduleManager.Modules.OfType<WorldCreatorWinModule>().Single();
//            worldCreatorWinModule.PersistentTypesInfo = new PersistentTypesInfo {
//                                                                                     ClassInfo = typeof (PersistentClassInfo),
//                                                                                     CollectionMemberInfo = typeof (PersistentCollectionMemberInfo),
//                                                                                     CoreMemberInfo = typeof (PersistentCoreTypeMemberInfo),
//                                                                                     ReferenceTypeInfo = typeof (PersistentReferenceMemberInfo),
//                                                                                     AssociationInfo = typeof (PersistentTypeInfo)
//                                                                                 };
        }
    }
}
