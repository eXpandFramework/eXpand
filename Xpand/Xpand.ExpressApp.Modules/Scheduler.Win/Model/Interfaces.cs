using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Scheduler;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraScheduler;
using System.Linq;
using Xpand.ExpressApp.ListEditors;
using Xpand.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Scheduler.Win.Model {
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
    public interface IModelOptionsSchedulerEx : IModelOptionsColumnView {
        IModelSchedulerPopupMenus PopupMenus { get; }
    }
    public interface IModelSchedulerPopupMenus : IModelNode, IModelList<IModelSchedulerPopupMenu> {
    }
    [KeyProperty("MenuId")]
    [DisplayProperty("MenuId")]
    public interface IModelSchedulerPopupMenu : IModelNode {
        [Required]
        [DataSourceProperty("Menus")]
        string MenuId { get; set; }
        [Browsable(false)]
        IEnumerable<string> Menus { get; }
    }
    [DomainLogic(typeof(IModelSchedulerPopupMenu))]
    public class ModelSchedulerPopupMenuDomainLogic {
        public static IEnumerable<string> Get_Menus(IModelSchedulerPopupMenu popupMenu) {
            var enumValues = Enum.GetValues(typeof(SchedulerMenuItemId));
            return (enumValues.Cast<object>().Select(enumValue => enumValue.ToString())).ToList();
        }
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
    public class SchedulerEditorVisibilityCalculator : EditorTypeVisibilityCalculator {
        #region Overrides of EditorTypeVisibilityCalculator
        public override bool IsVisible(IModelNode node, string propertyName) {
            return typeof(SchedulerListEditor).IsAssignableFrom(EditorType(node));

        }
        #endregion
    }

}
