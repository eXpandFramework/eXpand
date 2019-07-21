using System;
using DevExpress.XtraScheduler;
using System.Linq;
using DevExpress.XtraScheduler.Native;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Scheduler.Model {
//    public class AppoitmentSynchronizer:ModelListSynchronizer {
//        public AppoitmentSynchronizer(IAppointmentLabelStorage labels, IAppointmentStatusStorage statuses, IModelListViewOptionsScheduler modelListViewOptionsScheduler) : base(null,modelListViewOptionsScheduler ) {
//            var appointmentsModel = modelListViewOptionsScheduler.OptionsScheduler.GetNode("Storage").GetNode("Appointments");
//            ModelSynchronizerList.Add(new AppointmentLabelsSynchronizer(labels, (IModelAppointmentLabels)appointmentsModel.GetNode("Labels")));
//            ModelSynchronizerList.Add(new AppoitmentStatusSynchronizer(statuses, (IModelAppoitmentStatuses)appointmentsModel.GetNode("Statuses")));
//        }
//    }

//    public class SchedulerListEditorModelSynchronizer : ModelListSynchronizer {
//        public SchedulerListEditorModelSynchronizer(IInnerSchedulerControlOwner control, IModelListViewOptionsScheduler model, IAppointmentLabelStorage labels, IAppointmentStatusStorage statuses)
//            : base(control, model) {
//            ModelSynchronizerList.Add(new SchedulerControlSynchronizer(control,model));
//
//            var appoitmentSynchronizer = new AppoitmentSynchronizer(labels, statuses,model);
//            ModelSynchronizerList.Add(appoitmentSynchronizer);
//        }
//    }
//    public class SchedulerControlSynchronizer : ComponentSynchronizer<object, IModelOptionsSchedulerEx> {
//        public SchedulerControlSynchronizer(IInnerSchedulerControlOwner control, IModelListViewOptionsScheduler model)
//            : base(control, model.OptionsScheduler, false) {
//        }
//
//        public override void SynchronizeModel() {
//
//        }
//    }

//    public class AppointmentLabelsSynchronizer : ModelSynchronizer<IAppointmentLabelStorage, IModelAppointmentLabels> {
//        public AppointmentLabelsSynchronizer(IAppointmentLabelStorage component, IModelAppointmentLabels modelNode)
//            : base(component, modelNode) {
//        }
//
//        #region Overrides of ModelSynchronizer
//        protected override void ApplyModelCore() {
//            if (Model.Any(label => label.NodeEnabled)) {
//                Control.Clear();
//                foreach (IModelAppointmentLabel modelAppointmentLabel in Model) {
//                    var appointmentLabel = Control.CreateNewLabel(null);
//                    Control.Add(appointmentLabel);
//                    ApplyModel(modelAppointmentLabel, appointmentLabel, ApplyValues);
//                }
//            }
//        }
//
//        public override void SynchronizeModel() {
//
//        }
//        #endregion
//    }

//    public class AppoitmentStatusSynchronizer : ModelSynchronizer<IAppointmentStatusStorage, IModelAppoitmentStatuses> {
//        #region Overrides of ModelSynchronizer
//        public AppoitmentStatusSynchronizer(IAppointmentStatusStorage component, IModelAppoitmentStatuses modelNode)
//            : base(component, modelNode) {
//
//        }
//
//        protected override void ApplyModelCore() {
//            if (Model.Any(label => label.NodeEnabled)) {
//                Control.Clear();
//                foreach (var modelAppoitmentStatus in Model) {
//                    var appointmentLabel = Control.CreateNewStatus(null);
//                    Control.Add(appointmentLabel);
//                    ApplyModel(modelAppoitmentStatus, appointmentLabel, ApplyValues);
//                }
//            }
//
//        }
//
//        public override void SynchronizeModel() {
//
//        }
//        #endregion
//    }
//    public class SchedulerPopupMenuModelSynchronizer : ModelSynchronizer<object, IModelSchedulerPopupMenuItem> {
//        public SchedulerPopupMenuModelSynchronizer(object component, IModelSchedulerPopupMenuItem modelNode)
//            : base(component, modelNode) {
//        }
//        #region Overrides of ModelSynchronizer
//        protected override void ApplyModelCore() {
//            ApplyModel(Model, Control, ApplyValues);
//        }
//
//        public override void SynchronizeModel() {
//            throw new NotImplementedException();
//        }
//        #endregion
//    }
}