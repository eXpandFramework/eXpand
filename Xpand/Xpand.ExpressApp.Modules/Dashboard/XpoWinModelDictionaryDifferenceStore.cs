using System.IO;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelDifference.Win{
    public class XpoWinModelDictionaryDifferenceStore:DictionaryStores.XpoModelDictionaryDifferenceStore
    {
        public XpoWinModelDictionaryDifferenceStore( XafApplication application, bool enableLoading)
            : base(application, enableLoading) {
        }

        protected override string GetPath(){
            return Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
        }
    }
}