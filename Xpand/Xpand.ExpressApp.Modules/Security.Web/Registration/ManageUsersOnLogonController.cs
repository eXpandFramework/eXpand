using System.Web;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.Web.Registration {
    public class ManageUsersOnLogonController:Security.Registration.ManageUsersOnLogonController {
        protected override void OnActivated(){
            base.OnActivated();
            var ua = HttpContext.Current.Request.QueryString["Ua"];
            if (!string.IsNullOrEmpty(ua)) {
                using (var objectSpace = Application.CreateObjectSpace(SecuritySystem.UserType)) {
                    var registrationActivation = (IModelRegistrationActivation)((IModelOptionsRegistration)Application.Model.Options).Registration;
                    var name = registrationActivation.ActivationIdMember.Name;
                    var findObject = objectSpace.FindObject(SecuritySystem.UserType, CriteriaOperator.Parse(name + "='" + ua + "'"));
                    objectSpace.TypesInfo.FindTypeInfo(SecuritySystem.UserType).FindMember("IsActive").SetValue(findObject, true);
                    objectSpace.CommitChanges();
                    HttpContext.Current.Response.Write(registrationActivation.SuccessFulActivationOutput);
                    if (!string.IsNullOrEmpty(registrationActivation.SuccessFulActivationReturnUrl)) {
                        HttpContext.Current.Response.RedirectLocation = registrationActivation.SuccessFulActivationReturnUrl;
                    }
                    else
                        HttpContext.Current.Response.End();
                }
            }
        }
    }
}
