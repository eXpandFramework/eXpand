using System;
using System.Linq;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.Security;

namespace SecurityTester.Module.Web.FunctionalTests.NewUserActivation{
    public class NewUserActivationController:ObjectViewController<ObjectView, NewUserActivationObject> {
        public NewUserActivationController(){
            var simpleAction = new SimpleAction(this,"ActivateNewUser",PredefinedCategory.View);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            var xpandUser = ObjectSpace.GetObjectsQuery<XpandUser>().First(user => user.UserName== "NewUserActivation");
            var registrationActivation = (IModelRegistrationActivation)((IModelOptionsRegistration)Application.Model.Options).Registration;
            xpandUser.SetMemberValue(registrationActivation.ActivationIdMember.Name,Guid.NewGuid().ToString());
            var oid = xpandUser.Oid;
            WebApplication.Instance.LogOff();
            WebApplication.Redirect(HttpContext.Current.Request.Url.AbsoluteUri+ "/?Ua=" + oid);
        }
    }
}