using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Builders {
    public class UserDifferenceObjectBuilder {
        public static void SetUp(UserModelDifferenceObject userModelDifferenceObject) {
            userModelDifferenceObject.Name = string.Format("AutoCreated for {0} {1}", ((IAuthenticationStandardUser)SecuritySystem.CurrentUser).UserName, DateTime.Now);
        }

        public static void CreateDynamicUserMember(Type userType) {
            if (userType != null) {
                XafTypesInfo.Instance.CreateBothPartMembers(userType, typeof(UserModelDifferenceObject),
                                                            XafTypesInfo.XpoTypeInfoSource.XPDictionary, true,
                                                            "UserUsers_UserModelDifferenceObjectUserModelDifferenceObjects", "UserModelDifferenceObjects", "Users");
            }
        }
    }
}