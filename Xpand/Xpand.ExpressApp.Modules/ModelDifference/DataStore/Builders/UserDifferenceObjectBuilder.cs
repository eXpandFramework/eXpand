using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Builders{
    public class UserDifferenceObjectBuilder 
    {
        public static void SetUp(UserModelDifferenceObject userModelDifferenceObject)
        {
            userModelDifferenceObject.Name = string.Format("AutoCreated for {0} {1}", ((IAuthenticationStandardUser)SecuritySystem.CurrentUser).UserName, DateTime.Now);
        }

        public static void CreateDynamicMembers(Type userType){
            if (userType != null){
                var xpCustomMemberInfos = XafTypesInfo.Instance.CreateBothPartMembers(userType, typeof(UserModelDifferenceObject), XafTypesInfo.XpoTypeInfoSource.XPDictionary, true, "UsersUserModelDiff", "UserModels", "Users");
                var xpCustomMemberInfo = xpCustomMemberInfos.FirstOrDefault();
                if (xpCustomMemberInfo != null)
                    xpCustomMemberInfo.IntermediateClass.AddAttribute(new PersistentAttribute("UserUsers_UserModelDifferenceObjectUserModelDifferenceObjects"));
            }
        }
    }
}