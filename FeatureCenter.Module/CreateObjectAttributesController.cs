using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Attributes;
using eXpand.Persistent.Base.General;

namespace FeatureCenter.Module {
    public abstract class CreateObjectAttributesController:ViewController {
        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfo = typesInfo.FindTypeInfo(GetTypeToDecorate());
            
            if (typeInfo != null) {
                var displayFeatureModelAttribute = GetDisplayFeatureModelAttribute();
                AddAttribute(typeInfo, displayFeatureModelAttribute);
                CloneViewAttribute cloneViewAttribute = GetCloneViewAttribute();
                AddAttribute(typeInfo, cloneViewAttribute);
                NavigationItemAttribute navigationItemAttribute=GetNavigationItemAttribute();
                AddAttribute(typeInfo, navigationItemAttribute);
            }
        }

        protected virtual NavigationItemAttribute GetNavigationItemAttribute() {
            return null;
        }

        protected virtual CloneViewAttribute GetCloneViewAttribute() {
            return null;
        }

        void AddAttribute(ITypeInfo typeInfo, ISupportViewId attribute) {
            if (attribute == null) return;
            var attributes = typeInfo.FindAttributes<Attribute>().OfType<ISupportViewId>();
            if (attributes.Where(attr =>attr.GetType()==attribute.GetType()&& attr.ViewId == attribute.ViewId).FirstOrDefault() == null)
                typeInfo.AddAttribute((Attribute) attribute);
        }

        protected abstract string GetTypeToDecorate();

        protected abstract DisplayFeatureModelAttribute GetDisplayFeatureModelAttribute();

        
    }
}