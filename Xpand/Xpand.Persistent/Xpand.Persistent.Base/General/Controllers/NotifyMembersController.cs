using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Fasterflect;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers {
    public interface ISupportNotifiedMembers{
    }

    public interface IModelClassNotifiedMembers : IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [TypeConverter(typeof(ModelClassMembersConverter))]
        string NotifiedMembers { get; set; }
    }
    public class ModelClassMembersConverter : StringConverter {
        public const string AllMembers = "All";
        public const string Calculated = "Calculated";
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            return new StandardValuesCollection(new[]{AllMembers,Calculated});
        }
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            return false;
        }
    }

    [ModelInterfaceImplementor(typeof(IModelClassNotifiedMembers), "ModelClass")]
    public interface IModelObjectViewNotifiedMembers : IModelClassNotifiedMembers {
    }

    [DomainLogic(typeof(IModelObjectViewNotifiedMembers))]
    public class ModelObjectViewNotifiedMembersDomainLogic {
        public static string Get_NotifiedMembers(IModelObjectViewNotifiedMembers members){
            var modelViewItems = GetModelViewItems((IModelObjectView)members);
            return string.Join(";", modelViewItems.Where(IsEditorSupported).Cast<IModelNode>().Select(item => item.Id()));
        }

        private static IEnumerable<IModelCommonMemberViewItem> GetModelViewItems(IModelObjectView modelObjectView){
            var modelDetailView = modelObjectView as IModelDetailView;
            return modelDetailView != null ? modelDetailView.Items.OfType<IModelCommonMemberViewItem>() : ((IModelListView) modelObjectView).Columns;
        }

        private static bool IsEditorSupported(IModelCommonMemberViewItem item){
            return typeof(ISupportNotifiedMembers).IsAssignableFrom(item.PropertyEditorType);
        }
    }
    public class NotifyMembersController:ViewController<ObjectView>,IModelExtender {
        private MethodInvoker _onChangedmethodInfo;
        private string _notifiedMembers;


        protected override void OnActivated(){
            base.OnActivated();
            _notifiedMembers = ((IModelObjectViewNotifiedMembers)View.Model).NotifiedMembers;
            if (!string.IsNullOrEmpty(_notifiedMembers)){
                _onChangedmethodInfo = View.ObjectTypeInfo.Type.DelegateForCallMethod("OnChanged",new[]{typeof(string)});
                if(_onChangedmethodInfo!=null)
                    ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.ObjectChanged-=ObjectSpaceOnObjectChanged;
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs){
            string propertyName = objectChangedEventArgs.PropertyName;
            if (View != null &&
                (!string.IsNullOrEmpty(propertyName) &&
                 objectChangedEventArgs.Object.GetType() == View.ObjectTypeInfo.Type)){
                foreach (IModelMember notifiedEnabledMember in IsNotifiedEnabled(propertyName)){
                    _onChangedmethodInfo(objectChangedEventArgs.Object, notifiedEnabledMember.Name);
                }
            }
        }
    
        private IEnumerable<IModelMember> IsNotifiedEnabled(string propertyName){
            var modelMembers = View.Model.ModelClass.AllMembers.Where(member => member.Name!=propertyName);
            if (_notifiedMembers == ModelClassMembersConverter.AllMembers) return modelMembers;
            if (_notifiedMembers == ModelClassMembersConverter.Calculated) return modelMembers.Where(member => member.IsCalculated);
            return modelMembers.Where(member => _notifiedMembers.Split(';').Contains(member.Name));
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelClass,IModelClassNotifiedMembers>();
            extenders.Add<IModelObjectView,IModelObjectViewNotifiedMembers>();
        }
    }
}
