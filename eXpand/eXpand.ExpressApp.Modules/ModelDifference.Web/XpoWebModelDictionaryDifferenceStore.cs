using System.Web;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;

namespace eXpand.ExpressApp.ModelDifference.Web
{
    public class XpoWebModelDictionaryDifferenceStore : XpoModelDictionaryDifferenceStore
    {
        public XpoWebModelDictionaryDifferenceStore(XafApplication xafApplication, bool enableLoading)
            : base(xafApplication, enableLoading)
        {
            
        }


        protected override string GetPath(){
            HttpRequest request = HttpContext.Current.Request;
            return request.MapPath(request.ApplicationPath);
        }
    }
}