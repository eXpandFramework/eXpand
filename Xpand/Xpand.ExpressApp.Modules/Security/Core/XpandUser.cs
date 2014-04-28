using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;

namespace Xpand.ExpressApp.Security.Core{
    public class XpandUser : SecuritySystemUser {
        public XpandUser(Session session)
            : base(session) {
        }

        private string _email;
        private string _activation;

        [InvisibleInAllViews]
        public string Activation {
            get { return _activation; }
            set { SetPropertyValue("Activation", ref _activation, value); }
        }

        [ModelDefault("IsEmail","True")]
        [EditorAlias(EditorAliases.HyperLinkPropertyEditor)]
        public string Email {
            get { return _email; }
            set { SetPropertyValue("Email", ref _email, value); }
        }
    }
}
