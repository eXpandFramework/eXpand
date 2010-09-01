using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxGridView;
using eXpand.ExpressApp.Core.DynamicModel;
using eXpand.ExpressApp.SystemModule;
using DynamicDouplicateTypesMapper = eXpand.ExpressApp.Web.Core.DynamicDouplicateTypesMapper;

namespace eXpand.ExpressApp.Web.SystemModule
{
    public interface IModelColumnOptions : IModelColumnOptionsBase
    {
        IModelGridColumnOptions GridColumnOptions { get; set; }
    }

    public interface IModelGridColumnOptions : IModelNode
    {
        IModelGridViewColumnSettings Settings { get; set; }
    }

    public interface IModelGridViewColumnSettings:IModelNode {
    }

    public class GridColumnOptionsController : ColumnOptionsController
    {

        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes() {
            yield return new DynamicModelType(typeof(IModelGridViewColumnSettings), typeof(GridViewDataColumnSettings), null, null,new DynamicDouplicateTypesMapper());
        }


    }

    public class GridColumnPropertiesEditController : ColumnOptionsController
    {
        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes() {
            yield return new DynamicModelType(typeof(IModelColumnPropertiesEdit), typeof(EditPropertiesBase));
        }

        
    }

    public interface IModelColumnProperties : IModelNode {
        IModelColumnPropertiesEdit PropertiesEdit { get; set; }
    }

    public interface IModelColumnPropertiesEdit:IModelNode {
        
    }

}
