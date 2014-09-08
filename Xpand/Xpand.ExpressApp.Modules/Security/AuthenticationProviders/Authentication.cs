using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    public interface IModelAthentication : IModelNode {
        IModelAnonymousAuthentication AnonymousAuthentication { get; }
        IModelAutoAthentication AutoAthentication { get; }
    }

    public interface IModelAutoAthentication:IModelNode {
        [Description("If enabled you need to use XpandLogonParameters object in your authentication ")]
        bool Enabled { get; set; }
        [DefaultValue(15)]
        [Description("The time in days after which the authentication ticket will expire. To ue the Session timeout set this value to zero")]
        [ModelBrowsable(typeof(WebOnlyVisibilityCalculator))]
        int TicketExpiration { get; set; }
        [Description("Enable this to override XAF's default behaviour and create only the encrypted ticket")]
        [ModelBrowsable(typeof(WebOnlyVisibilityCalculator))]
        [DefaultValue(true)]
        bool UseOnlySecuredStorage { get; set; }
    }

    public interface IModelAnonymousAuthentication : IModelNode {
        [ModelBrowsable(typeof(WebOnlyVisibilityCalculator))]
        bool Enabled { get; set; }
        [ModelBrowsable(typeof(WebOnlyVisibilityCalculator))]
        [DefaultValue("Anonymous")]
        string AnonymousUser { get; set; }
    }

    public interface IModelOptionsAuthentication {
        IModelAthentication Athentication { get; }
    }
}
