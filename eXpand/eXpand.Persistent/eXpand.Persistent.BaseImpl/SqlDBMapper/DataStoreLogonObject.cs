using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalEditorState;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.SqlDBMapper;

namespace eXpand.Persistent.BaseImpl.SqlDBMapper
{
    [EditorStateRule("DataStoreLogonObject_UserName", "UserName,PassWord",EditorState.Disabled, "Authentication=0",ViewType.DetailView)]
    [NonPersistent]
    public class DataStoreLogonObject : BaseObject, IDataStoreLogonObject {
        public DataStoreLogonObject(Session session)
            : base(session)
        {
        }
        private string _serverName;

        public string ServerName
        {
            get { return _serverName; }
            set { SetPropertyValue("ServerName", ref _serverName, value); }
        }
        private DataStoreAuthentication _authentication;

        public DataStoreAuthentication Authentication {
            get { return _authentication; }
            set { SetPropertyValue("Authentication", ref _authentication, value); }
        }

        public override string ToString() {
            return this.GetConnectionString();
        }
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetPropertyValue("UserName", ref _userName, value); }
        }
        [DataSourceProperty("DataBases")]
        public DataBase DataBase
        {
            get { return _dataBase; }
            set { SetPropertyValue("DataStore", ref _dataBase, value); }
        }
        IDataBase IDataStoreLogonObject.DataBase {
            get { return DataBase; }
            set { DataBase=value as DataBase; }
        }

        private DataBase _dataBase;
        readonly IList<IDataBase> _dataBases=new List<IDataBase>();

        
        private string _passWord;
        [Custom("IsPassword","True")]
        public string PassWord {
            get { return _passWord; }
            set { SetPropertyValue("PassWord", ref _passWord, value); }
        }
        [Browsable(false)]
        public IList<IDataBase> DataBases {
            get { return _dataBases; }
        }
    }
}
