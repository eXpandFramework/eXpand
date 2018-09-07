using System.IO;
using System.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using Ionic.Zip;
using Xpand.ExpressApp.IO.Controllers;

namespace Xpand.ExpressApp.IO.Web.Controllers {
    public class IOViewController : IOViewControllerBase {
        protected override void Save(MemoryStream memoryStream) {
            Save(memoryStream, false);
        }

        private void Save(MemoryStream memoryStream, bool isZipped){
            var response = HttpContext.Current.Response;
            response.ClearHeaders();
            response.Clear();
            response.BufferOutput = false;
            var fileName = GetFileName(isZipped);
            response.ContentType = $"application/{GetExtension(isZipped)}";

            response.AddHeader("content-disposition", "filename=" + fileName);

            ResponseWriter.WriteFileToResponse(memoryStream, fileName);
        }

        protected override void Save(ZipFile zipFile) {
            using (var outputStream = new MemoryStream()){
                zipFile.Save(outputStream);
                outputStream.Position = 0;
                Save(outputStream,true);
            }
        }
    }
}