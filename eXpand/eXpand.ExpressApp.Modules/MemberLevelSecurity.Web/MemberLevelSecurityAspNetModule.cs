using System.Web;

namespace eXpand.ExpressApp.MemberLevelSecurity.Web {
    public sealed partial class MemberLevelSecurityAspNetModule : MemberLevelSecurityModuleBase
    {

        public MemberLevelSecurityAspNetModule()
        {
            InitializeComponent();
        }
        
        protected override bool? ComparerIsSet {
            get {
                bool result;
                bool.TryParse(HttpContext.Current.Application["ComparerIsSet"] + "", out result);
                return result;
            }
            set { HttpContext.Current.Application["ComparerIsSet"] = value; }
        }
    }
}