using System.Web;
using System.Xml.Linq;
using DevExpress.ExpressApp.Utils;
using Xpand.ExpressApp.IO.Controllers;

namespace Xpand.ExpressApp.IO.Web.Controllers {
    public class IOViewController : IOViewControllerBase {


        protected override void Save(XDocument document) {
            var response = HttpContext.Current.Response;
            response.Clear();
            response.AddHeader("Content-Disposition", "attachment;filename=" +CaptionHelper.GetClassCaption(View.ObjectTypeInfo.Type.FullName) + ".xml");
            var xml = document.ToString(SaveOptions.DisableFormatting);
            response.AddHeader("Content-Length", xml.Length.ToString());
            response.ContentType = "application/octet-stream";
            response.Write(xml);
            response.End();
        }
    }
}
