using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using XpandSystemTester.Module.Web.BusinessObjects;

namespace XpandSystemTester.Module.Web{
    public class Updater : ModuleUpdater{
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion){
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            const string name = "Vertical Dimension 1";
            var vd1 = ObjectSpace.FindObject<VerticalDimension>(CriteriaOperator.Parse("Title=?", name));
            if (vd1 == null){
                vd1 = ObjectSpace.CreateObject<VerticalDimension>();
                vd1.Title = name;
                var vd2 = ObjectSpace.CreateObject<VerticalDimension>();
                vd2.Title = "Vertical Dimension 2";
                var hd1 = ObjectSpace.CreateObject<HorizontalDimension>();
                hd1.Title = "Horizontal Dimension 1";
                var hd2 = ObjectSpace.CreateObject<HorizontalDimension>();
                hd2.Title = "Horizontal Dimension 2";

                var item = ObjectSpace.CreateObject<TwoDimensionItem>();
                item.VerticalDimension = vd1;
                item.HorizontalDimension = hd1;
                item.Title = "Vertical 1 Horizontal 1";
                item = ObjectSpace.CreateObject<TwoDimensionItem>();
                item.VerticalDimension = vd1;
                item.HorizontalDimension = hd2;
                item.Title = "Vertical 1 Horizontal 2";
                item = ObjectSpace.CreateObject<TwoDimensionItem>();
                item.VerticalDimension = vd2;
                item.HorizontalDimension = hd1;
                item.Title = "Vertical 2 Horizontal 1";
                item = ObjectSpace.CreateObject<TwoDimensionItem>();
                item.VerticalDimension = vd2;
                item.HorizontalDimension = hd2;
                item.Title = "Vertical 2 Horizontal 2";
            }
        }
    }
}