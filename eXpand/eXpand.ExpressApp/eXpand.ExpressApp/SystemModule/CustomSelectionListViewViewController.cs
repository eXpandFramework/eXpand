using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelClassCustomSelection : IModelNode
    {
        [Category("eXpand")]
        bool CustomSelection { get; set; }
    }

    public class CustomSelectionListViewViewController : BaseViewController<ListView>, IModelExtender
    {
        protected const string CustomSelection = "CustomSelection";

        private bool _hasCustomSelection;
        public bool HasCustomSelection
        {
            get { return _hasCustomSelection; }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            _hasCustomSelection = ((IModelClassCustomSelection)View.Model.ModelClass).CustomSelection;
            if (_hasCustomSelection)
            {
                if (View.ObjectTypeInfo.FindMember(CustomSelection) == null)
                {
                    IMemberInfo member = View.ObjectTypeInfo.CreateMember(CustomSelection, typeof(bool));
                    member.AddAttribute(new NonPersistentAttribute());
                    ObjectSpace.Session.UpdateSchema(View.ObjectTypeInfo.Type);
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