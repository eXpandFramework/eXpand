using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Fasterflect;

namespace Xpand.Persistent.Base.General.Controllers {
    public interface ISupportNotifiedMembers{
    }

    public interface IModelClassNotifiedMembers : IModelNode {
        [Category("eXpand")]
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
        private MethodInfo _onChangedmethodInfo;


        protected override void OnActivated(){
            base.OnActivated();
            _onChangedmethodInfo = View.ObjectTypeInfo.Type.Method("OnChanged",new[]{typeof(string)});
            if(_onChangedmethodInfo!=null)
                ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.ObjectChanged-=ObjectSpaceOnObjectChanged;
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
            string propertyName = objectChangedEventArgs.PropertyName;
            if (View != null && (!string.IsNullOrEmpty(propertyName) && objectChangedEventArgs.Object.GetType() == View.ObjectTypeInfo.Type)) {
                foreach (var notifiedEnabledMember in NotifiedEnabledMembers(propertyName)) {
                    _onChangedmethodInfo.Call(View.CurrentObject, notifiedEnabledMember.Name);
                }
            }
        }

        private IEnumerable<IModelMember> NotifiedEnabledMembers(string propertyName){
            var notifiedCalculatedMembers = ((IModelObjectViewNotifiedMembers)View.Model).NotifiedMembers;
            var modelMembers = View.Model.ModelClass.AllMembers.Where(member => member.Name!=propertyName);
            if (notifiedCalculatedMembers == ModelClassMembersConverter.AllMembers) return modelMembers;
            if (notifiedCalculatedMembers == ModelClassMembersConverter.Calculated) return modelMembers.Where(member => member.IsCalculated);
            if (!string.IsNullOrEmpty(notifiedCalculatedMembers)){
                return modelMembers.Where(member => notifiedCalculatedMembers.Split(';').Contains(member.Name));
            }
            return Enumerable.Empty<IModelMember>();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelClass,IModelClassNotifiedMembers>();
            extenders.Add<IModelObjectView,IModelObjectViewNotifiedMembers>();
        }
    }
}
