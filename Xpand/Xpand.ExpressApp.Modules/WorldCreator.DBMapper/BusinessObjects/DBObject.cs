using System.Diagnostics.CodeAnalysis;
using DevExpress.Xpo;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.WorldCreator.DBMapper.BusinessObjects{
    [NonPersistent][SuppressMessage("Design", "XAF0023:Do not implement IObjectSpaceLink in the XPO types")]
    public class DBObject : XpandBaseCustomObject{
        private string _name;

        public DBObject(Session session)
            : base(session){
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
    }
}