using System;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

namespace SystemTester.Module.Win.FunctionalTests.DetailViewCaching {
    public class DetailViewCaching:ObjectViewController<ObjectView,DetailViewCachingObject> {
        private const string NewModalWindow = "NewModalWindow";
        static readonly Dictionary<ITypeInfo,int> _dictionary=new Dictionary<ITypeInfo, int>();

        public DetailViewCaching(){
            var singleChoiceAction = new SingleChoiceAction(this,"DetailViewCaching",PredefinedCategory.View);
            singleChoiceAction.Items.Add(new ChoiceActionItem(NewModalWindow, null));
            singleChoiceAction.ItemType=SingleChoiceActionItemType.ItemIsOperation;
            singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            var objectSpace = Application.CreateObjectSpace();
            e.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace,objectSpace.CreateObject<DetailViewCachingObject>());
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();

            if (View is DetailView){
                if (!_dictionary.ContainsKey(View.ObjectTypeInfo))
                    _dictionary.Add(View.ObjectTypeInfo, 0);
                var i = _dictionary[View.ObjectTypeInfo] + 1;
                _dictionary[View.ObjectTypeInfo] = i;

                var deleteAction = Application.MainWindow.GetController<DeleteObjectsViewController>().DeleteAction;

                deleteAction.ToolTip = View.Id + " Called " + i.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
