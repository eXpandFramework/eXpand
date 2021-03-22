using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Extensions.XAF.Attributes;
using Xpand.Persistent.Base.Security;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Security.Core{
    [System.ComponentModel.DisplayName("User")]
    public class XpandUser : SecuritySystemUser,ISecurityRelated {
        public XpandUser(Session session)
            : base(session) {
        }

        private string _email;
        private string _activation;

        [InvisibleInAllViews]
        public string Activation {
            get => _activation;
            set => SetPropertyValue("Activation", ref _activation, value);
        }

        [ModelDefault("IsEmail","True")]
        [EditorAlias(EditorAliases.HyperLinkPropertyEditor)]
        public string Email {
            get => _email;
            set => SetPropertyValue("Email", ref _email, value);
        }
    }
}
