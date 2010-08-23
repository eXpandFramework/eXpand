using System.Collections.Generic;

namespace eXpand.Persistent.Base.SqlDBMapper {
    public enum DataStoreAuthentication {
        Windows,
        Server
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