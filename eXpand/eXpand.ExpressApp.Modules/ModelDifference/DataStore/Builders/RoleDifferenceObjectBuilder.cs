using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Builders{
    public class RoleDifferenceObjectBuilder
    {

        private static ITypeInfo GetRoleTypeInfo(ISecurityComplex security){
            return XafTypesInfo.Instance.PersistentTypes.Where(info => info.Type== security.RoleType).Single();
        }
    
        public static bool CreateDynamicMembers(ISecurity security){
            return XafTypesInfo.Instance.CreateBothPartMembers(typeof(RoleModelDifferenceObject), GetRoleTypeInfo((ISecurityComplex) security).Type,
                                                            XafTypesInfo.XpoTypeInfoSource.XPDictionary, true)!=null;
        }
    }
}