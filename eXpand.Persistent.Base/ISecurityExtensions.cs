using System;
using System.Configuration;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl;

namespace eXpand.Persistent.Base
{
    public static class ISecurityExtensions
    {
        public static Type GetUserType(this ISecurity security)
        {
            if (security.UserType != null)
                return security.UserType;
            string setting = ConfigurationManager.AppSettings["UserType"];
            if (string.IsNullOrEmpty(setting))
                return typeof(User);
            return Type.GetType(setting);
        }
    }
}
