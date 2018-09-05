using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
using Xpand.ExpressApp.IO.Controllers;
using Xpand.ExpressApp.IO.Core;

namespace Xpand.ExpressApp.IO.Web.Controllers {
    public class IOViewController : IOViewControllerBase {

        protected override void Save(XDocument document) {
            var stream = new MemoryStream();
            
            using (var textWriter = XmlWriter.Create(stream, ExportEngine.GetXMLWriterSettings(document.IsMinified()))) {
                document.Save(textWriter);
                textWriter.Close();
            }

            HttpContext.Current.Response.ClearHeaders();
            ResponseWriter.WriteFileToResponse(stream, CaptionHelper.GetClassCaption(View.ObjectTypeInfo.Type.FullName) + ".xml");
        }
    }
}
