using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace ModelArtifactStateTester.Module.FunctionalTests.Views{
    [DefaultClassOptions]
    [CloneModelView(CloneViewType.DetailView, "ViewsObject_CustomProcessSelectedItem_DetailView")]
    [CloneModelView(CloneViewType.DetailView, "ViewsObject_New_DetailView")]
    [CloneModelView(CloneViewType.DetailView, "ViewsObject_CurrentObjectChanged_DetailView")]
    public class ViewsObject : BaseObject{
        private string _name;

        public ViewsObject(Session session) : base(session){
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
    }
}