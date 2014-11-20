using System.IO;
using System.Web;
using System.Xml.Linq;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Web.Internal;
using Xpand.ExpressApp.IO.Controllers;

namespace Xpand.ExpressApp.IO.Web.Controllers {
    public class IOViewController : IOViewControllerBase {

        protected override void Save(XDocument document) {
            var stream = new MemoryStream();
            document.Save(stream);
            HttpContext.Current.Response.ClearHeaders();
            ResponseWriter.WriteFileToResponse(stream, CaptionHelper.GetClassCaption(View.ObjectTypeInfo.Type.FullName) + ".xml");
        }
    }
}
