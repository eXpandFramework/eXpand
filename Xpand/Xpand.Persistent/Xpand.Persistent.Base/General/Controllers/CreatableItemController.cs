using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using System.Linq;

namespace Xpand.Persistent.Base.General.Controllers {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class CreateableItemAttribute : Attribute {
        readonly Type _masterObjectType;

        public CreateableItemAttribute() {
        }

        public CreateableItemAttribute(Type masterObjectType) {
            _masterObjectType = masterObjectType;
        }

        public Type MasterObjectType {
            get { return _masterObjectType; }
        }
    }

    public class CreatableItemController : ViewController {

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.Disposing += FrameOnDisposing;
            Frame.GetController<NewObjectViewController>().CollectDescendantTypes += OnCollectDescendantTypes;
        }

        void OnCollectDescendantTypes(object sender, CollectTypesEventArgs e) {
            var nestedFrame = Frame as NestedFrame;
            if (nestedFrame != null && nestedFrame.ViewItem.View!=null) {
                var parentType = nestedFrame.ViewItem.View.ObjectTypeInfo.Type;
                var typeInfos = e.Types.Select(type => Application.TypesInfo.FindTypeInfo(type));
                var typeAttributes = typeInfos.Select(info => new{info.Type, Attributes = info.FindAttributes<CreateableItemAttribute>()}).
                    Where(arg => arg.Attributes.Any()).ToList();
                foreach (var typeAttribute in typeAttributes) {
                    if (typeAttribute.Attributes.Any(attribute => attribute.MasterObjectType == null))
                        e.Types.Remove(typeAttribute.Type);
                    else if(!typeAttribute.Attributes.Any(attribute => attribute.MasterObjectType.IsAssignableFrom(parentType))) {
                        e.Types.Remove(typeAttribute.Type);
                    }
                }                
            }
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.GetController<NewObjectViewController>().CollectDescendantTypes -= OnCollectDescendantTypes;        
        }
    }

}
