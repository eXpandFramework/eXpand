using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;

namespace Xpand.Docs.Module {
    public class Class1:ViewController {
        protected override void OnActivated(){
            base.OnActivated();
            var boolList = Frame.GetController<DeleteObjectsViewController>().Active;
        }

        private void ActiveOnResultValueChanged(object sender, BoolValueChangedEventArgs boolValueChangedEventArgs){
            
        }
    }
}
