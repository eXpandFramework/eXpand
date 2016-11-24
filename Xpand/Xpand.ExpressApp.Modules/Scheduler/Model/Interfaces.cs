using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Scheduler;
using DevExpress.Persistent.Base;
using DevExpress.XtraScheduler;
using System.Linq;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Scheduler.Model {
    public interface IModelAppoitmentLabel : IModelNodeEnabled {

    }
    public interface IModelAppoitmentStatus : IModelNodeEnabled {

    }
    public interface IModelAppoitmentLabels : IModelNode, IModelList<IModelAppoitmentLabel> {
    }
    public interface IModelAppoitmentStatuses : IModelNode, IModelList<IModelAppoitmentStatus> {
    }

    public interface IAppoitmentStorageLabels : IModelNode {
        IModelAppoitmentLabels Labels { get; }
    }
    public interface IAppoitmentStorageStatuses : IModelNode {
        IModelAppoitmentStatuses Statuses { get; }
    }

    [ModelDisplayName("Scheduler")]
    public interface IModelOptionsSchedulerEx : IModelOptionsColumnView {
        IModelSchedulerPopupMenuItems PopupMenuItems { get; }
    }
    public interface IModelSchedulerPopupMenuItems : IModelNode, IModelList<IModelSchedulerPopupMenuItem> {
    }
    
    public interface IModelSchedulerPopupMenuItem : IModelNodeEnabled {
        
    }

    [ModelAbstractClass]
    public interface IModelListViewOptionsScheduler : IModelListViewOptionsColumnView, IModelListViewScheduler {
        [ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
        [Category("Data")]
        [DataSourceProperty("ResourceListViews")]
        IModelListView ResourceListView { get; set; }
        [Category("Data")]
        [ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
        bool ResourcesOnlyWithAppoitments { get; set; }
        [Browsable(false)]
        IModelList<IModelListView> ResourceListViews { get; }

        [ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
        IModelOptionsSchedulerEx OptionsScheduler { get; }
    }

    [DomainLogic(typeof(IModelListViewOptionsScheduler))]
    public class ModelListViewOptionsSchedulerDomainLogic {
        public static IModelList<IModelListView> Get_ResourceListViews(IModelListViewOptionsScheduler listViewOptionsScheduler) {
            return new CalculatedModelNodeList<IModelListView>(listViewOptionsScheduler.Application.Views.OfType<IModelListView>().Where(view => listViewOptionsScheduler.ResourceClasses.Contains(view.ModelClass)));
        }
    }

}
