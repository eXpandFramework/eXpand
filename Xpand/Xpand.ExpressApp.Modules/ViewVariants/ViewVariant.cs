using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ViewVariants {
    [NonPersistent]
    [Appearance("Hide_Caption", AppearanceItemType.ViewItem, "ShowCaption=false", Visibility = ViewItemVisibility.Hide, TargetItems = "ViewCaption,VariantCaption")]
    public class ViewVariant : XpandBaseCustomObject {
        private string _variantCaption;
        private bool _showCaption;
        string _viewCaption;

        string _clonedViewName;

        public ViewVariant(Session session) : base(session) {
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            ShowCaption = true;
        }

        [RuleRequiredField(TargetCriteria = "ShowCaption=true")]
        public string ViewCaption {
            get { return _viewCaption; }
            set { SetPropertyValue("ViewCaption", ref _viewCaption, value); }
        }

        [RuleRequiredField(TargetCriteria = "ShowCaption=true")]
        public string VariantCaption{
            get { return _variantCaption; }
            set { SetPropertyValue("VariantCaption", ref _variantCaption, value); }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue){
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "ViewCaption" && VariantCaption == null)
                VariantCaption = ViewCaption;
        }

        [Browsable(false)]
        public bool ShowCaption {
            get { return _showCaption; }
            set { SetPropertyValue("ShowCaption", ref _showCaption, value); }
        }

        [Browsable(false)]
        public string ClonedViewName {
            get { return _clonedViewName; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _clonedViewName, value); }
        }
    }
}