using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.ExceptionHandling;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.ExceptionHandling {
    public static class ExceptionObjectBuilder {
        public static IExceptionObject Create(Session session, Exception exception,XafApplication application) {
            var findBussinessObjectType = XafTypesInfo.Instance.FindBussinessObjectType<IExceptionObject>();
            var exceptionObject = (IExceptionObject)ReflectionHelper.CreateObject(findBussinessObjectType, new[] { session });
            if (application != null)
                exceptionObject.Application = application.ApplicationName;
            exceptionObject.ComputerName = Environment.MachineName;
            exceptionObject.Date = DateTime.Today;
            exceptionObject.FullException = exception.ToString();

            exceptionObject.Message = exception.Message;
            exceptionObject.Screenshot = GetScreenShot();
            exceptionObject.ThreadID = Thread.CurrentThread.Name;
            exceptionObject.Time = DateTime.Now.TimeOfDay;
            var user = ((User) SecuritySystem.CurrentUser);
            if (user != null)
                exceptionObject.UserId = (Guid) session.GetKeyValue(user);
            exceptionObject.TracingLastEntries = Tracing.Tracer.GetLastEntriesAsString();
            var windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null) exceptionObject.WindowsID = windowsIdentity.Name;
            if (exception.InnerException != null)
                exceptionObject.InnerExceptionObjects.Add(Create(session, exception.InnerException,application));
            return exceptionObject;
        }
        private static Image ResizeImage(Bitmap image, int maxWidth, int maxHeight)
        {
            Size resizedDimensions =GetDimensions(maxWidth, maxHeight, ref image);
            return new Bitmap(image, resizedDimensions);
        }
        private static Size GetDimensions(int maxWidth, int maxHeight, ref Bitmap bitmap)
        {
            int height = bitmap.Height;
            int width = bitmap.Width;
            if (height <= maxHeight && width <= maxWidth){
                return new Size(width, height);
            }
            var multiplier = (float)maxWidth / width;
            if ((height * multiplier) <= maxHeight){
                height = (int)(height * multiplier);
                return new Size(maxWidth, height);
            }
            multiplier = maxHeight / (float)height;
            width = (int)(width * multiplier);

            return new Size(width, maxHeight);
        }
        static Image GetScreenShot()
        {
            var image = new Bitmap(
                Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height,
                PixelFormat.Format24bppRgb);

            var graphics = Graphics.FromImage(image);

            graphics.CopyFromScreen(
                Screen.PrimaryScreen.Bounds.X,
                Screen.PrimaryScreen.Bounds.Y,
                0,
                0,
                Screen.PrimaryScreen.Bounds.Size,
                CopyPixelOperation.SourceCopy);

            graphics.Dispose();

            return ResizeImage(image, 600, 600);
        }

    }
}