using System.Web;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;

namespace eXpand.ExpressApp.ModelDifference.Web
{
    public class XpoWebModelDictionaryDifferenceStore : XpoModelDictionaryDifferenceStore
    {
        public XpoWebModelDictionaryDifferenceStore(Session updatingSession, XafApplication xafApplication, bool enableLoading)
            : base(updatingSession, xafApplication, enableLoading)
        {
            
        }


        protected override string GetPath(){
            HttpRequest request = HttpContext.Current.Request;
            return request.MapPath(request.ApplicationPath);
        }
    }
}