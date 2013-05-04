using System;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

namespace Xpand.ExpressApp.Core {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = true)]
    public class BoBehaviorAttribute : Attribute, IModelClassBehavior {
        public BoBehaviorAttribute() {
        }

        public BoBehaviorAttribute(bool allowNew, bool allowEdit, bool allowDelete) {
            AllowNew = allowNew;
            AllowEdit = allowEdit;
            AllowDelete = allowDelete;
        }

        [Category("Behavior"), DefaultValue(true)]
        public Boolean AllowNew { get; set; }

        [Category("Behavior")]
        public Boolean AllowEdit { get; set; }

        [Category("Behavior"), DefaultValue(true)]
        public Boolean AllowDelete { get; set; }

        public IModelClassBehavior GetBehaviorInfo {
            get { return this; }
        }
    }

    [DomainLogic(typeof(ModelClassBehaviorLogic))]
    public interface IModelClassBehavior {
        [Category("Behavior"), DefaultValue(true)]
        Boolean AllowNew { get; set; }

        [Category("Behavior")]
        Boolean AllowEdit { get; set; }

        [Category("Behavior"), DefaultValue(true)]
        Boolean AllowDelete { get; set; }
        [Browsable(false)]
        IModelClassBehavior GetBehaviorInfo { get; }
    }

    public class ModelClassBehaviorLogic {
        public static IModelClassBehavior Get_GetBehaviorInfo(IModelClassBehavior instance) {
            var attr = ((IModelClass)instance).TypeInfo.FindAttribute<BoBehaviorAttribute>();
            return attr ?? instance;
        }
    }


    public class ViewNodesGeneratorUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            foreach (IModelView view in (IModelViews)node) {
                var objectView = view as IModelObjectView;

                if (objectView == null || objectView.ModelClass == null) continue;
                var behavior = objectView.ModelClass as IModelClassBehavior;
                if (behavior == null) continue;
                behavior = behavior.GetBehaviorInfo;
                objectView.AllowNew = behavior.AllowNew;
                objectView.AllowEdit = behavior.AllowEdit;
                objectView.AllowDelete = behavior.AllowDelete;
            }
        }
    }



}