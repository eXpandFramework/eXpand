using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.Xpo;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ViewVariants {
    [NonPersistent]
    [Appearance("Hide_DeleteView",AppearanceItemType.ViewItem,"CanDeleteView=false",Visibility = ViewItemVisibility.Hide,TargetItems = "DeleteView")]
    [Appearance("Hide_Caption",AppearanceItemType.ViewItem,"ShowCaption=false",Visibility = ViewItemVisibility.Hide,TargetItems = "Caption")]
    public class ViewVariant : XpandBaseCustomObject {
        private bool _showCaption;
        private bool _canDeleteView;
        private bool _deleteView;
        string caption;

        string clonedViewName;

        public ViewVariant(Session session) : base(session) {
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            ShowCaption = true;
        }

        public string Caption {
            get { return caption; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref caption, value); }
        }

        [Browsable(false)]
        public bool ShowCaption {
            get { return _showCaption; }
            set { SetPropertyValue("ShowCaption", ref _showCaption, value); }
        }

        public bool DeleteView {
            get { return _deleteView; }
            set { SetPropertyValue("DeleteView", ref _deleteView, value); }
        }
        [Browsable(false)]
        public bool CanDeleteView {
            get { return _canDeleteView; }
            set { SetPropertyValue("CanDeleteView", ref _canDeleteView, value); }
        }
        [Browsable(false)]
        public string ClonedViewName {
            get { return clonedViewName; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref clonedViewName, value); }
        }
    }
}