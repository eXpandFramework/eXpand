using System;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Model.Options;

namespace Xpand.ExpressApp.Web.ListEditors.Model {
    public class GridListEditorVisibilityCalculatorHelper :
        ExpressApp.Model.Options.GridListEditorVisibilityCalculatorHelper {
        public override bool IsVisible(IModelNode node, string propertyName) {
            Type editorType = EditorType(node);
            if (editorType == typeof (ASPxGridListEditor))
                return true;
            if (typeof (XpandASPxGridListEditor).IsAssignableFrom(editorType))
                return true;
            return false;
        }
    }

    [ModelAbstractClass]
    public interface IModelColumnOptionsGridViewBand : IModelColumnOptionsGridView {
        [DataSourceProperty("Bands")]
        [Category("eXpand")]
        IModelGridViewBand GridViewBand { get; set; }

        [Browsable(false)]
        IModelGridViewBands Bands { get; }
    }

    public interface IModelGridViewBand : IModelNode {
    }

    public interface IModelGridViewBands : IModelList<IModelGridViewBand>, IModelNode {
    }

    public interface IModelGridViewOptionsBands {
        IModelGridViewBands Bands { get; }
    }

    [DomainLogic(typeof (IModelColumnOptionsGridViewBand))]
    public class ModelColumnOptionsGridViewBandDomainLogic {
        public static IModelGridViewBands Get_Bands(IModelColumnOptionsGridViewBand self) {
            return ((IModelGridViewOptionsBands)((IModelListViewOptionsGridView)self.ParentView).GridViewOptions).Bands;
        }
    }
}