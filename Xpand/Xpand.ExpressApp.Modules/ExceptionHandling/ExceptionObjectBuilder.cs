using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.ExceptionHandling;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ExceptionHandling {
    public static class ExceptionObjectBuilder {

        public static IExceptionObject Create( Exception exception, XafApplication application, IExceptionObject parent = null) {
            IObjectSpace _os = application.CreateObjectSpace(XafTypesInfo.Instance.FindBussinessObjectType<IExceptionObject>());
            var res = Create(_os, exception, application);
            _os.CommitChanges();
            return res;

        }

        internal static IExceptionObject Create( IObjectSpace objectSpace, Exception exception, XafApplication application , IExceptionObject parent = null) {
            var findBussinessObjectType = XafTypesInfo.Instance.FindBussinessObjectType<IExceptionObject>();
            Guard.ArgumentNotNull(objectSpace, "objectSpace");
            Guard.ArgumentNotNull(exception , "exception");
            Guard.ArgumentNotNull(application , "application");

            IExceptionObject exceptionObject = null;

            exceptionObject =
                (IExceptionObject)
                    ((parent == null
                        ? objectSpace.CreateObject(findBussinessObjectType)
                        : Activator.CreateInstance(findBussinessObjectType)));
            if (exceptionObject == null) {
                Tracing.Tracer.LogError("Could not create ExceptionObject");
                return null;
            }
                exceptionObject.Application = application.ApplicationName;
            exceptionObject.ComputerName = Environment.MachineName;
            exceptionObject.Date = DateTime.Today;
            exceptionObject.FullException = exception.ToString();

            exceptionObject.Message = exception.Message;
            exceptionObject.Screenshot = GetScreenShot();
            exceptionObject.ThreadID = Thread.CurrentThread.Name;
            exceptionObject.Time = DateTime.Now.TimeOfDay;
            var user = (SecuritySystem.CurrentUser);
            if (user != null)
                exceptionObject.UserId = SecuritySystem.CurrentUserName;
            exceptionObject.TracingLastEntries = Tracing.Tracer.GetLastEntriesAsString();
            var windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null) exceptionObject.WindowsID = windowsIdentity.Name;
            if (exception.InnerException != null)
            {
                var childException = Create(objectSpace, exception.InnerException, application, exceptionObject);
                exceptionObject.InnerExceptionObjects.Add(childException);
            }
            return exceptionObject;
        }
        private static Image ResizeImage(Bitmap image, int maxWidth, int maxHeight) {
            Size resizedDimensions = GetDimensions(maxWidth, maxHeight, ref image);
            return new Bitmap(image, resizedDimensions);
        }
        private static Size GetDimensions(int maxWidth, int maxHeight, ref Bitmap bitmap) {
            int height = bitmap.Height;
            int width = bitmap.Width;
            if (height <= maxHeight && width <= maxWidth) {
                return new Size(width, height);
            }
            var multiplier = (float)maxWidth / width;
            if ((height * multiplier) <= maxHeight) {
                height = (int)(height * multiplier);
                return new Size(maxWidth, height);
            }
            multiplier = maxHeight / (float)height;
            width = (int)(width * multiplier);

            return new Size(width, maxHeight);
        }
        static byte[] GetScreenShot() {
            using (var image = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format24bppRgb)) {
                var graphics = Graphics.FromImage(image);
                graphics.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                graphics.Dispose();
                //return ResizeImage(image, 600, 600); resizing loses to much info
                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

    }
}