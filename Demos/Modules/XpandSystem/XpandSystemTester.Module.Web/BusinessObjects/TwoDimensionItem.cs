using System;
using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.Web.ListEditors.TwoDimensionListEditor;

namespace XpandSystemTester.Module.Web.BusinessObjects{
    [DefaultClassOptions]
    [DefaultProperty("Title")]
    [Appearance("test_appearance1", AppearanceItemType.ViewItem, "VerticalDimension.Title like '%1%'", TargetItems = "*",
        BackColor = "yellow")]
    [Appearance("test_appearance2", AppearanceItemType.ViewItem, "VerticalDimension.Title like '%1%'",
        TargetItems = "HorizontalDimension", FontColor = "red")]
    [Appearance("test_appearance3", AppearanceItemType.ViewItem, "VerticalDimension.Title like '%1%'",
        TargetItems = "VerticalDimension", FontColor = "blue")]
    public class TwoDimensionItem : BaseObject, ITwoDimensionItem{
        // Fields...
        private HorizontalDimension _horizontalDimension;
        private string _title;
        private VerticalDimension _verticalDimension;

        public TwoDimensionItem(Session session)
            : base(session){
        }

        public string Title{
            get { return _title; }
            set { SetPropertyValue("Title", ref _title, value); }
        }


        public VerticalDimension VerticalDimension{
            get { return _verticalDimension; }
            set { SetPropertyValue("VerticalDimension", ref _verticalDimension, value); }
        }


        public HorizontalDimension HorizontalDimension{
            get { return _horizontalDimension; }
            set { SetPropertyValue("HorizontalDimension", ref _horizontalDimension, value); }
        }


        IComparable ITwoDimensionItem.HorizontalDimension{
            get { return HorizontalDimension; }
        }

        IComparable ITwoDimensionItem.VerticalDimension{
            get { return VerticalDimension; }
        }
    }
}