using System;
using System.Drawing;
using System.Reflection;
using DevExpress.XtraScheduler;
using System.Linq;
using DevExpress.XtraScheduler.Native;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Scheduler.Model {
    public class SchedulerListEditorModelSynchronizer : ModelListSynchronizer {
        public SchedulerListEditorModelSynchronizer(IInnerSchedulerControlOwner control, IModelListViewOptionsScheduler model, AppointmentLabelBaseCollection labels, AppointmentStatusBaseCollection statuses)
            : base(control, model) {
            ModelSynchronizerList.Add(new SchedulerControlSynchronizer(control,model));
            var appointmentsModel = model.OptionsScheduler.GetNode("Storage").GetNode("Appointments");
            ModelSynchronizerList.Add(new AppoitmentLabelsSynchronizer(labels, (IModelAppoitmentLabels)appointmentsModel.GetNode("Labels")));
            ModelSynchronizerList.Add(new AppoitmentStatusSynchronizer(statuses, (IModelAppoitmentStatuses)appointmentsModel.GetNode("Statuses")));
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
        readonly Type _appoitmentType;
        static readonly MethodInfo _methodInfo;
        static AppoitmentLabelsSynchronizer() {
            _methodInfo = typeof(AppointmentLabelBaseCollection).GetMethod("CreateItem", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Color), typeof(string), typeof(string) }, null);
        }
        public AppoitmentLabelsSynchronizer(AppointmentLabelBaseCollection component, IModelAppoitmentLabels modelNode)
            : base(component, modelNode) {
            _appoitmentType = _methodInfo.Invoke(Control, new object[] { Color.Empty, "", "" }).GetType();
        }
        #region Overrides of ModelSynchronizer
        protected override void ApplyModelCore() {
            if (Model.Any(label => label.NodeEnabled)) {
                Control.Clear();
                foreach (IModelAppoitmentLabel modelAppoitmentLabel in Model) {
                    var appointmentLabel = (AppointmentLabelBase)Activator.CreateInstance(_appoitmentType);
                    Control.Add( appointmentLabel);
                    ApplyModel(modelAppoitmentLabel, appointmentLabel, ApplyValues);
                }
            }

        }

        public override void SynchronizeModel() {

        }
        #endregion
    }

    public class AppoitmentStatusSynchronizer : ModelSynchronizer<AppointmentStatusBaseCollection, IModelAppoitmentStatuses> {
        static readonly MethodInfo _methodInfo;
        readonly Type _appoitmentStatusType;

        static AppoitmentStatusSynchronizer() {
            _methodInfo = typeof(AppointmentStatusBaseCollection).GetMethod("CreateItem", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Color), typeof(string), typeof(string) }, null);
        }
        #region Overrides of ModelSynchronizer
        public AppoitmentStatusSynchronizer(AppointmentStatusBaseCollection component, IModelAppoitmentStatuses modelNode)
            : base(component, modelNode) {
            _appoitmentStatusType = _methodInfo.Invoke(Control, new object[] { Color.Empty, "", "" }).GetType();
        }

        protected override void ApplyModelCore() {
            if (Model.Any(label => label.NodeEnabled)) {
                Control.Clear();
                foreach (var modelAppoitmentStatus in Model) {
                    var appointmentLabel = (AppointmentStatusBase)Activator.CreateInstance(_appoitmentStatusType, new object[] { AppointmentStatusType.Custom, null });
                    Control.Add( appointmentLabel);
                    ApplyModel(modelAppoitmentStatus, appointmentLabel, ApplyValues);
                }
            }

        }

        public override void SynchronizeModel() {

        }
        #endregion
    }
    public class SchedulerPopupMenuModelSynchronizer : ModelSynchronizer<object, IModelSchedulerPopupMenu> {
        public SchedulerPopupMenuModelSynchronizer(object component, IModelSchedulerPopupMenu modelNode)
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