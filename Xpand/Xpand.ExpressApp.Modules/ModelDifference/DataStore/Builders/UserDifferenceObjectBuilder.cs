using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General;
using System.Linq;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Builders {
    public class UserDifferenceObjectBuilder {
        public static void SetUp(UserModelDifferenceObject userModelDifferenceObject) {
            userModelDifferenceObject.Name = string.Format("AutoCreated for {0} {1}", ((IAuthenticationStandardUser)SecuritySystem.CurrentUser).UserName, DateTime.Now);
        }

        public static void CreateDynamicUserMember(Type userType) {
            if (userType != null) {
                var xpCustomMemberInfos = XafTypesInfo.Instance.CreateBothPartMembers(userType, typeof(UserModelDifferenceObject), XpandModuleBase.Dictiorary, true, "UserUsers_UserModelDifferenceObjectUserModelDifferenceObjects", "UserModelDifferenceObjects", "Users");
                xpCustomMemberInfos.First(info => info.Name == "UserModelDifferenceObjects").AddAttribute(new VisibleInDetailViewAttribute(false));
            }
        }
    }
}