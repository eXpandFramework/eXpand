using System;
using System.Drawing;
using DevExpress.XtraScheduler;
using System.Linq;
using DevExpress.XtraScheduler.Native;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Scheduler.Model {
    public class AppoitmentSynchronizer:ModelListSynchronizer {
        public AppoitmentSynchronizer(AppointmentLabelBaseCollection labels, AppointmentStatusBaseCollection statuses, IModelListViewOptionsScheduler modelListViewOptionsScheduler) : base(null,modelListViewOptionsScheduler ) {
            var appointmentsModel = modelListViewOptionsScheduler.OptionsScheduler.GetNode("Storage").GetNode("Appointments");
            ModelSynchronizerList.Add(new AppoitmentLabelsSynchronizer(labels, (IModelAppoitmentLabels)appointmentsModel.GetNode("Labels")));
            ModelSynchronizerList.Add(new AppoitmentStatusSynchronizer(statuses, (IModelAppoitmentStatuses)appointmentsModel.GetNode("Statuses")));
        }
    }

    public class SchedulerListEditorModelSynchronizer : ModelListSynchronizer {
        public SchedulerListEditorModelSynchronizer(IInnerSchedulerControlOwner control, IModelListViewOptionsScheduler model, AppointmentLabelBaseCollection labels, AppointmentStatusBaseCollection statuses)
            : base(control, model) {
            ModelSynchronizerList.Add(new SchedulerControlSynchronizer(control,model));

            var appoitmentSynchronizer = new AppoitmentSynchronizer(labels, statuses,model);
            ModelSynchronizerList.Add(appoitmentSynchronizer);
        }
    }
    public class SchedulerControlSynchronizer : ComponentSynchronizer<object, IModelOptionsSchedulerEx> {
        public SchedulerControlSynchronizer(IInnerSchedulerControlOwner control, IModelListViewOptionsScheduler model)
            : base(control, model.OptionsScheduler, false) {
        }

        public override void SynchronizeModel() {

        }
    }

    public class AppoitmentLabelsSynchronizer : ModelSynchronizer<AppointmentLabelBaseCollection, IModelAppoitmentLabels> {
        public AppoitmentLabelsSynchronizer(AppointmentLabelBaseCollection component, IModelAppoitmentLabels modelNode)
            : base(component, modelNode) {
        }
        #region Overrides of ModelSynchronizer
        protected override void ApplyModelCore() {
            if (Model.Any(label => label.NodeEnabled)) {
                Control.Clear();
                foreach (IModelAppoitmentLabel modelAppoitmentLabel in Model.Where(label => label.NodeEnabled)) {
                    Control.Add(modelAppoitmentLabel.GetValue<Color>("Color"),
                                modelAppoitmentLabel.GetValue<string>("DisplayName"),
                                modelAppoitmentLabel.GetValue<string>("MenuCaption"));
                }
            }

        }

        public override void SynchronizeModel() {

        }
        #endregion
    }

    public class AppoitmentStatusSynchronizer : ModelSynchronizer<AppointmentStatusBaseCollection, IModelAppoitmentStatuses> {
        #region Overrides of ModelSynchronizer
        public AppoitmentStatusSynchronizer(AppointmentStatusBaseCollection component, IModelAppoitmentStatuses modelNode)
            : base(component, modelNode) {

        }

        protected override void ApplyModelCore() {
            if (Model.Any(label => label.NodeEnabled)) {
                Control.Clear();
                foreach (var modelAppoitmentStatus in Model) {
                    Control.Add(modelAppoitmentStatus.GetValue<Color>("Color"),
                                modelAppoitmentStatus.GetValue<string>("DisplayName"),
                                modelAppoitmentStatus.GetValue<string>("MenuCaption"));
                }
            }

        }

        public override void SynchronizeModel() {

        }
        #endregion
    }
    public class SchedulerPopupMenuModelSynchronizer : ModelSynchronizer<object, IModelSchedulerPopupMenuItem> {
        public SchedulerPopupMenuModelSynchronizer(object component, IModelSchedulerPopupMenuItem modelNode)
            : base(component, modelNode) {
        }
        #region Overrides of ModelSynchronizer
        protected override void ApplyModelCore() {
            ApplyModel(Model, Control, ApplyValues);
        }

        public override void SynchronizeModel() {
            throw new NotImplementedException();
        }
        #endregion
    }
}