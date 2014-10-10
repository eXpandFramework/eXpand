using DevExpress.EasyTest.Framework;
using Fasterflect;
using Xpand.EasyTest;

namespace Xpand.ExpressApp.EasyTest.WebAdapter {
    public enum ApplicationParams{
        PhysicalPath,
        UseIISExpress,
        UseModel,
        DefaultWindowSize,
        Url,
        DontRunWebDev,
        SingleWebDev,
        WaitDebuggerAttached,
        DontKillWebDev,
        DontRestartIIS,
        WebBrowserType
    }
    public static class Extensions {
        public static void CreateBrowser(this XpandTestWebAdapter adapter,string url) {
            adapter.CallMethod("CreateBrowser", url);
        }
        public static T ParameterValue<T>(this TestApplication application, ApplicationParams applicationParams){
            return application.ParameterValue(applicationParams, default(T));
        }

        public static T ParameterValue<T>(this TestApplication application,ApplicationParams applicationParams,T defaultValue){
            var parameterValue = application.ParameterValue<T>(applicationParams.ToString());
            return Equals(default (T), parameterValue) ? defaultValue : parameterValue;
        }
    }
}
