using DevExpress.ExpressApp;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.ModelDifference.Win{
    public class XpoWinModelDictionaryDifferenceStore:DictionaryStores.XpoModelDictionaryDifferenceStore
    {
        public XpoWinModelDictionaryDifferenceStore(Session session, XafApplication application) : base(session, application){
        }

        protected override string GetPath(){
            return System.Windows.Forms.Application.ExecutablePath;
        }
    }
}