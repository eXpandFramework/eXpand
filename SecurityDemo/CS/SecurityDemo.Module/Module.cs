using DevExpress.ExpressApp;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using Xpand.Persistent.BaseImpl;


namespace SecurityDemo.Module {
    public sealed partial class SecurityDemoModule : ModuleBase {
        public SecurityDemoModule() {
            InitializeComponent();
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(Xpand.Persistent.BaseImpl.Updater))));
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfo = typesInfo.FindTypeInfo(typeof(SequenceObject));
            var memberInfo = (XafMemberInfo)typeInfo.FindMember("TypeName");
            memberInfo.RemoveAttributes<SizeAttribute>();
            memberInfo.AddAttribute(new SizeAttribute(256));
        }
    }
}
