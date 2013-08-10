using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.Persistent.Base.General.Model.Options {
    public abstract class ComponentSynchronizer<TComponent, TModelColumnViewOptions> : ModelSynchronizer<TComponent, TModelColumnViewOptions>
        where TComponent : class
        where TModelColumnViewOptions : IModelOptionsColumnView {
        readonly bool _overrideViewDesignMode;

        protected ComponentSynchronizer(TComponent control, TModelColumnViewOptions modelNode,
                                                      bool overrideViewDesignMode)
            : base(control, modelNode) {
            _overrideViewDesignMode = overrideViewDesignMode;
        }

        protected bool OverrideViewDesignMode {
            get { return _overrideViewDesignMode; }
        }

        protected override void ApplyModelCore() {
            if (Model.NodeEnabled || _overrideViewDesignMode)
                ApplyModel(Model, Control, ApplyValues);
        }

        public override void SynchronizeModel() {
            if (Model.NodeEnabled || _overrideViewDesignMode)
                ApplyModel(Model, Control, SynchronizeValues);
        }
    }

}
