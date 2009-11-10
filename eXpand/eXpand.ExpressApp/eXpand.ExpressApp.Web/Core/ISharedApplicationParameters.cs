using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Web;

namespace eXpand.ExpressApp.Web.Core{
    public interface ISharedApplicationParameters {
        WebApplication SharedApplication { get; }
        ExpressApplicationSetupParameters Parameters { get; }
        void CreateSharedParameters(string connectionString);
        WebApplication CreateApplicationInstance(ISecurity security);
    }
}