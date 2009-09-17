using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using Foxhound.ExpressApp.Scheduler.BaseObjects.Ranges;

namespace Foxhound.ExpressApp.Scheduler.Controllers{
    public partial class PersistentDateRangeInfoViewController : ViewController{
        public PersistentDateRangeInfoViewController(){
            InitializeComponent();
            RegisterActions(components);
        }

        //private void PopulateInfoTypeFilters(){
        //    FilterByInfoTypeAction.Items.Clear();

        //    FilterByInfoTypeAction.Items.AddRange(
        //        ((ListView) View).CollectionSource.Collection.Cast<PersistentDateRangeInfo>()
        //            .Select(info => info.DateRangeInfoType)
        //            .Distinct()
        //            .Select(item => new ChoiceActionItem(item.Name, item.EnumValue))
        //            .ToList());
        //}

        protected override void OnActivated(){
            base.OnActivated();
            this.View.ControlsCreated +=new System.EventHandler(ViewControlsCreatedHanlder);
            FilterByInfoTypeAction.Active["readyToUse"] = false;
        }

        private void ViewControlsCreatedHanlder(object sender, EventArgs e) {
            //PopulateInfoTypeFilters();
        }

        private void FilterByInfoTypeAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e){
            ((ListView) View).CollectionSource.Criteria.Add(
                new KeyValuePair<string, CriteriaOperator>(
                    "infoTypeFilter",
                    new BinaryOperator(
                        "DateRangeInfoType.EnumValue"
                        , e.SelectedChoiceActionItem.Data)));
            ((ListView) View).CollectionSource.Reload();
        }
    }
}