using System;
using Common.Win.General.ViewItems;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;


namespace XVideoRental.Module.Win.Controllers.Common {
    [ModelAbstractClass]
    public interface IModelHintPanelEx : IModelHintPanel {

    }

    [ViewItem(typeof(IModelHintPanel))]
    public class HintPanelViewItem : global::Common.Win.General.ViewItems.HintPanelViewItem {
        public HintPanelViewItem(IModelHintPanel modelHintPanel, Type objectType)
            : base(modelHintPanel, objectType) {
        }
    }
}
