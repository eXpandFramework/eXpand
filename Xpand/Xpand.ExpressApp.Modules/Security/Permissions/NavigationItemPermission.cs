using System;
using System.Linq;
using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.Security.Permissions{
    public class NavigationItemPermission : IOperationPermission{
        public NavigationItemPermission(string hiddenNavigationItem){
            HiddenNavigationItem = hiddenNavigationItem;
        }

        public string HiddenNavigationItem { get; set; }

        public string Operation => "NavigateToItem";
    }

    [Serializable]
    public class NavigationItemPermissionRequest : IPermissionRequest{
        public NavigationItemPermissionRequest(string navigationItem){
            NavigationItem = navigationItem;
        }

        public string NavigationItem { get; set; }

        public object GetHashObject(){
            return $"{GetType().FullName} - {NavigationItem}";
        }
    }

    public class NavigationItemPermissionRequestProcessor :
        PermissionRequestProcessorBase<NavigationItemPermissionRequest>,ICustomPermissionRequestProccesor{

        public override bool IsGranted(NavigationItemPermissionRequest permissionRequest){
            return Permissions.GetPermissions<NavigationItemPermission>().All(permission => permission.HiddenNavigationItem != permissionRequest.NavigationItem);
        }

        public IPermissionDictionary Permissions{ get; set; }
    }
}