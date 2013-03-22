// Developer Express Code Central Example:
// How to: Implement middle tier security with the .NET Remoting service
// 
// The complete description is available in the Middle Tier Security - .NET
// Remoting Service (http://documentation.devexpress.com/#xaf/CustomDocument3438)
// topic.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4035

using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using System.Reflection;
using DevExpress.ExpressApp.Security.Strategy;


namespace SecuritySystemExample.Module {
    public sealed partial class SecuritySystemExampleModule : ModuleBase {
        public SecuritySystemExampleModule() {
            InitializeComponent();
        }
        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            List<Type> result = new List<Type>(base.GetDeclaredExportedTypes());
            result.AddRange(new Type[] { typeof(SecuritySystemUser), typeof(SecuritySystemRole) });
            return result;
        }
    }
}
