using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Builders{
    public class UserDifferenceObjectBuilder 
    {
        public static void SetUp(UserModelDifferenceObject userModelDifferenceObject)
        {
            userModelDifferenceObject.Name = string.Format("AutoCreated for {0} {1}", ((IAuthenticationStandardUser)SecuritySystem.CurrentUser).UserName, DateTime.Now);
        }


        public static bool CreateDynamicMembers(){
            bool members = false;
            if (SecuritySystem.UserType != null){
                members = XafTypesInfo.Instance.CreateBothPartMembers(SecuritySystem.UserType, typeof (UserModelDifferenceObject),
                                                                      XafTypesInfo.XpoTypeInfoSource.XPDictionary, true);
            }
            return members;
        }

    }
}