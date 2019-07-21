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
//    public interface IModelAppointmentLabel : IModelNodeEnabled {
//
//    }
//    public interface IModelAppoitmentStatus : IModelNodeEnabled {
//
//    }
//    public interface IModelAppointmentLabels : IModelNode, IModelList<IModelAppointmentLabel> {
//    }
//    public interface IModelAppoitmentStatuses : IModelNode, IModelList<IModelAppoitmentStatus> {
//    }

//    public interface IAppoitmentStorageLabels : IModelNode {
//        IModelAppointmentLabels Labels { get; }
//    }
//    public interface IAppoitmentStorageStatuses : IModelNode {
//        IModelAppoitmentStatuses Statuses { get; }
//    }

//    [ModelDisplayName("Scheduler")]
//    public interface IModelOptionsSchedulerPopupMenuItems  {
//        IModelSchedulerPopupMenus PopupMenus { get; }
//    }
//    public interface IModelSchedulerPopupMenus : IModelNode, IModelList<IModelSchedulerPopupMenu> {
//    }
    
//    public interface IModelSchedulerPopupMenu : IModelNodeEnabled {
//        
//    }
    [ModelAbstractClass]
    public interface IModelListViewSchedulerEx:IModelListViewScheduler,IModelNode {
        [ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
        [Category("Xpand.Scheduler")]
        [DataSourceProperty("ResourceListViews")]
        IModelListView ResourceListView { get; set; }
        [Category("Xpand.Scheduler")]
        [ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
        bool ResourcesOnlyWithAppoitments { get; set; }
        [Browsable(false)]
        IModelList<IModelListView> ResourceListViews { get; }
        
    }
//    [ModelAbstractClass]
//    public interface IModelListViewOptionsScheduler : IModelListViewOptionsColumnView, IModelListViewScheduler {
//        [ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
//        [Category("Data")]
//        [DataSourceProperty("ResourceListViews")]
//        IModelListView ResourceListView { get; set; }
//        [Category("Data")]
//        [ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
//        bool ResourcesOnlyWithAppoitments { get; set; }
//        [Browsable(false)]
//        IModelList<IModelListView> ResourceListViews { get; }
//
//        [ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
//        IModelOptionsSchedulerEx OptionsScheduler { get; }
//    }

    [DomainLogic(typeof(IModelListViewSchedulerEx))]
    public class ModelListViewOptionsSchedulerDomainLogic {
        public static IModelList<IModelListView> Get_ResourceListViews(IModelListViewSchedulerEx listViewOptionsScheduler) {
            return new CalculatedModelNodeList<IModelListView>(listViewOptionsScheduler.Application.Views.OfType<IModelListView>().Where(view => listViewOptionsScheduler.ResourceClasses.Contains(view.ModelClass)));
        }
    }

}
