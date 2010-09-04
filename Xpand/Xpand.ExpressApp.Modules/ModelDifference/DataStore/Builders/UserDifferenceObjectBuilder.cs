using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Builders{
    public class UserDifferenceObjectBuilder 
    {
        public static void SetUp(UserModelDifferenceObject userModelDifferenceObject)
        {
            userModelDifferenceObject.Name = string.Format("AutoCreated for {0} {1}", ((IAuthenticationStandardUser)SecuritySystem.CurrentUser).UserName, DateTime.Now);
        }

        public static bool CreateDynamicMembers(Type userType){
            bool members = false;
            if (userType != null){
                members = XafTypesInfo.Instance.CreateCollection(
                    userType, 
                    typeof(UserModelDifferenceObject), 
                    "UsersUserModelDiff", 
                    XafTypesInfo.XpoTypeInfoSource.XPDictionary, 
                    typeof(UserModelDifferenceObject).Name+"s") != null;
                if (members)
                    members = XafTypesInfo.Instance.CreateCollection(
                        typeof (UserModelDifferenceObject),userType,
                        "UsersUserModelDiff",
                        XafTypesInfo.XpoTypeInfoSource.XPDictionary,
                        "Users") != null;
            }

            return members;
        }
    }
}