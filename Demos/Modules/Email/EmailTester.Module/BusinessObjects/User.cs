using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.CustomAttributes;

namespace EmailTester.Module.BusinessObjects {
    public class User : SecuritySystemUser {
        public User(Session session) : base(session) {
        }
        
        private string _email;
        private string _activation;

        [InvisibleInAllViews]
        public string Activation {
            get { return _activation; }
            set { SetPropertyValue("Activation", ref _activation, value); }
        }

        public string Email {
            get { return _email; }
            set { SetPropertyValue("Email", ref _email, value); }
        }
    }
}
