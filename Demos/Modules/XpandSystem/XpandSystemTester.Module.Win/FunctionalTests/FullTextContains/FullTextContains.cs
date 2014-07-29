using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace XpandSystemTester.Module.Win.FunctionalTests {
    public class FullTextContains:ViewController{
        public FullTextContains(){
            var singleChoiceAction = new SingleChoiceAction(this,GetType().Name,PredefinedCategory.ObjectsCreation){
                ItemType = SingleChoiceActionItemType.ItemIsOperation
            };
            singleChoiceAction.Items.Add(new ChoiceActionItem("XpandGridListEditor", null));
            singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs){
            
        }
    }
}
