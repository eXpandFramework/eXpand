using System.IO;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.DictionaryDifferenceStore.DictionaryStores;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Web
{
    public class XpoWebModelDictionaryDifferenceStore : XpoModelDictionaryDifferenceStore
    {
        public XpoWebModelDictionaryDifferenceStore(Session updatingSession, XafApplication xafApplication) : base(updatingSession, xafApplication)
        {
            
        }

        protected override string GetModelPath()
        {
            HttpRequest request = HttpContext.Current.Request;
            return Path.Combine(request.MapPath(request.ApplicationPath),"model.xafml");
        }
    }
}