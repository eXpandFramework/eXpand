using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;
using Xpand.Extensions.XAF.Attributes;
using Xpand.Persistent.Base.Security;

namespace Xpand.Persistent.BaseImpl.Security{
    [ImageName("BO_User"), DefaultProperty("UserName")]
    [System.ComponentModel.DisplayName("User")]
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class XpandPermissionPolicyUser(Session session)
        : PermissionPolicyUser(session), ISecurityPermisssionPolicyRelated {
        private string _activation;

        private string _email;

        [InvisibleInAllViews]
        public string Activation{
            get => _activation;
            set => SetPropertyValue("Activation", ref _activation, value);
        }

        [ModelDefault("IsEmail", "True")]
        [EditorAlias("HyperLinkPropertyEditor")]
        public string Email{
            get => _email;
            set => SetPropertyValue("Email", ref _email, value);
        }
    }
}