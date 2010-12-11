using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Builders {
    public class RoleDifferenceObjectBuilder {

        private static ITypeInfo GetRoleTypeInfo(ISecurityComplex security) {
            return XafTypesInfo.Instance.PersistentTypes.Where(info => info.Type == security.RoleType).Single();
        }

        public static void CreateDynamicMembers(ISecurityComplex security) {
            XafTypesInfo.Instance.CreateBothPartMembers(typeof(RoleModelDifferenceObject),
                GetRoleTypeInfo(security).Type,
                XafTypesInfo.XpoTypeInfoSource.XPDictionary, true);
        }
    }
}