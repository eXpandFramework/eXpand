using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Builders{
    public class RoleDifferenceObjectBuilder
    {

        private static ITypeInfo GetRoleTypeInfo(){
            return XafTypesInfo.Instance.PersistentTypes.Where(info => info.Type== ((ISecurityComplex) SecuritySystem.Instance).RoleType).Single();
        }
    
        public static bool CreateDynamicMembers(){
            if (SecuritySystem.Instance is ISecurityComplex){
                return XafTypesInfo.Instance.CreateBothPartMembers(typeof(RoleModelDifferenceObject), GetRoleTypeInfo().Type,
                                                            XafTypesInfo.XpoTypeInfoSource.XPDictionary, true);
            }
            return false;
        }
    }
}