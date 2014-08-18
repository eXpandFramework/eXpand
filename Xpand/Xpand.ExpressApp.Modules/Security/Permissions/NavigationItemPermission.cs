using System;
using System.Linq;
using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.Security.Permissions{
    public class NavigationItemPermission : IOperationPermission{
        public NavigationItemPermission(string hiddenNavigationItem){
            HiddenNavigationItem = hiddenNavigationItem;
        }

        public string HiddenNavigationItem { get; set; }

        public string Operation{
            get { return "NavigateToItem"; }
        }
    }

    [Serializable]
    public class NavigationItemPermissionRequest : IPermissionRequest{
        public NavigationItemPermissionRequest(string navigationItem){
            NavigationItem = navigationItem;
        }

        public string NavigationItem { get; set; }

        public object GetHashObject(){
            return String.Format("{0} - {1}", GetType().FullName, NavigationItem);
        }
    }

    public class NavigationItemPermissionRequestProcessor :
        PermissionRequestProcessorBase<NavigationItemPermissionRequest>{
        private readonly IPermissionDictionary _permissions;

        public NavigationItemPermissionRequestProcessor(IPermissionDictionary permissions){
            _permissions = permissions;
        }

        public override bool IsGranted(NavigationItemPermissionRequest permissionRequest){
            return _permissions.GetPermissions<NavigationItemPermission>().All(permission => permission.HiddenNavigationItem != permissionRequest.NavigationItem);
        }
    }
}