using System.Collections.Generic;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;

namespace XpandSystemTester.Module.Win.FunctionalTests.DetailViewCaching {
    public class DetailViewCaching:ViewController<DetailView> {
        static readonly Dictionary<ITypeInfo,int> _dictionary=new Dictionary<ITypeInfo, int>(); 

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();

            if (!_dictionary.ContainsKey(View.ObjectTypeInfo))
                _dictionary.Add(View.ObjectTypeInfo,0);
            var i = _dictionary[View.ObjectTypeInfo]+1;
            _dictionary[View.ObjectTypeInfo] = i;

            var deleteAction = Application.MainWindow.GetController<DeleteObjectsViewController>().DeleteAction;

            deleteAction.ToolTip = View.Id + " Called " + i.ToString(CultureInfo.InvariantCulture);
        }
    }
}
