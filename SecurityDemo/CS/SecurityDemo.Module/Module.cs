using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Demos;
using DevExpress.Xpo;
using Xpand.Persistent.BaseImpl;


namespace SecurityDemo.Module
{
    public sealed partial class SecurityDemoModule : ModuleBase
    {
        public SecurityDemoModule()
        {
			InitializeComponent();

        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfo = typesInfo.FindTypeInfo(typeof(SequenceObject));
            var memberInfo = (XafMemberInfo) typeInfo.FindMember("TypeName");
            memberInfo.RemoveAttributes<SizeAttribute>();
            memberInfo.AddAttribute(new SizeAttribute(256));
        }
    }
}
