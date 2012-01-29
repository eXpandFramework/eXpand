using System.Collections.Generic;
using DevExpress.Xpo.DB;

namespace Xpand.Persistent.Base.PersistentMetaData {
    public enum DataStoreAuthentication {
        Windows,
        Server
    }

    public static class IDataStoreLogonObjectExtensions {
        public static string GetConnectionString(this IDataStoreLogonObject dataStoreLogonObject) {
            return new MSSqlProviderFactory().GetConnectionString(GetParamsDict(dataStoreLogonObject));
        }

        static Dictionary<string, string> GetParamsDict(IDataStoreLogonObject dataStoreLogonObject) {

            var parameters = new Dictionary<string, string>();
            if (dataStoreLogonObject.UserName != null)
                parameters.Add(ProviderFactory.UserIDParamID, dataStoreLogonObject.UserName);
            if (dataStoreLogonObject.PassWord != null)
                parameters.Add(ProviderFactory.PasswordParamID, dataStoreLogonObject.PassWord);
            parameters.Add(ProviderFactory.ReadOnlyParamID, "1");
            parameters.Add(ProviderFactory.ServerParamID, dataStoreLogonObject.ServerName);
            if (dataStoreLogonObject.DataBase != null)
                parameters.Add(ProviderFactory.DatabaseParamID, dataStoreLogonObject.DataBase.Name);
            parameters.Add(ProviderFactory.UseIntegratedSecurityParamID, (dataStoreLogonObject.Authentication == DataStoreAuthentication.Windows) ? "true" : "false");
            return parameters;

        }

    }
    public interface IDataStoreLogonObject {
        DataStoreAuthentication Authentication { get; set; }
        string ServerName { get; set; }
        string UserName { get; set; }
        IDataBase DataBase { get; set; }
        string PassWord { get; set; }
        IList<IDataBase> DataBases { get; }
    }
}