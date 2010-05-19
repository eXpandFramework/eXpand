using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelClassCustomSelection : IModelNode
    {
        bool CustomSelection { get; set; }
    }

    public class CustomSelectionListViewViewController : BaseViewController<ListView>, IModelExtender
    {
        protected const string CustomSelection = "CustomSelection";

        public CustomSelectionListViewViewController() { }

        private bool hasCustomSelection;
        public bool HasCustomSelection
        {
            get { return hasCustomSelection; }
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            if (((IModelClassCustomSelection)View.Model.ModelClass).CustomSelection)
            {
                if (View.ObjectTypeInfo.FindMember(CustomSelection) == null) {
                    IMemberInfo member = View.ObjectTypeInfo.CreateMember(CustomSelection, typeof (bool));
                    member.AddAttribute(new NonPersistentAttribute());
                    XafTypesInfo.Instance.RefreshInfo(View.ObjectTypeInfo);
                }
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassCustomSelection>();
        }
    }
}