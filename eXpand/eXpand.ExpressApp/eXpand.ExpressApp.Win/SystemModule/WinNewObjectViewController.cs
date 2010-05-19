using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Attributes;
using System.Linq;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelViewClassTypeToInstantiate : IModelNode
    {
        [Category("eXpand")]
        [DataSourceProperty("Application.BOModel")]
        string ClassTypeToInstantiate { get; set; }
    }
    public class WinNewObjectViewController : DevExpress.ExpressApp.Win.SystemModule.WinNewObjectViewController, IModelExtender
    {

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelViewClassTypeToInstantiate>();
            extenders.Add<IModelDetailView, IModelViewClassTypeToInstantiate>();
        }
        protected override void UpdateActionState()
        {
            base.UpdateActionState();
            foreach (ITypeInfo typeInfo in currentViewTypes)
            {
                var b = typeInfo.FindAttribute<CanInstantiate>() != null;
                if (b)
                {
                    String diagnosticInfo;
                    if (!CanCreateAndEdit(typeInfo.Type, out diagnosticInfo))
                    {
                        if (NewObjectAction.Items.Count > 0)
                            NewObjectAction.Items[0].BeginGroup = true;
                        IModelCreatableItem modelCreatableItem = GetModelCreatableItem(typeInfo);
                        if (modelCreatableItem != null)
                            NewObjectAction.Items.Insert(0, CreateItem(typeInfo.Type, modelCreatableItem));
                    }
                }
            }

            var type = ReflectionHelper.FindType(((IModelViewClassTypeToInstantiate)View.Model).ClassTypeToInstantiate);
            if (type != null)
            {
                IModelCreatableItem modelCreatableItem = GetModelCreatableItem(XafTypesInfo.CastTypeToTypeInfo(type));
                if (modelCreatableItem != null)
                {
                    NewObjectAction.Items.RemoveAt(0);
                    NewObjectAction.Items.Insert(0, CreateItem(type, modelCreatableItem));
                }
            }
        }

        IModelCreatableItem GetModelCreatableItem(ITypeInfo typeInfo)
        {
            ITypeInfo info = typeInfo;
            return ((IModelApplicationCreatableItems)Application.Model).CreatableItems.Where(item => item.ClassName == info.FullName).FirstOrDefault();
        }
    }
}