﻿using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Web;
using Xpand.ExpressApp.Core.DynamicModel;
using Xpand.ExpressApp.SystemModule;
using DynamicDouplicateTypesMapper = Xpand.ExpressApp.Web.Core.DynamicDouplicateTypesMapper;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface IModelColumnOptions : IModelColumnOptionsBase {
        IModelGridColumnOptions GridColumnOptions { get; }
    }

    public interface IModelGridColumnOptions : IModelNode {
        IModelGridViewColumnSettings Settings { get; }
    }

    public interface IModelGridViewColumnSettings : IModelNode {
    }
    [Obsolete]
    public class GridColumnOptionsController : ColumnOptionsController {

        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes() {
            yield return new DynamicModelType(typeof(IModelGridViewColumnSettings), typeof(GridViewDataColumnSettings), null, null, new DynamicDouplicateTypesMapper());
        }
    }
    [Obsolete]
    public class GridColumnPropertiesEditController : ColumnOptionsController {
        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes() {
            yield return new DynamicModelType(typeof(IModelColumnPropertiesEdit), typeof(EditPropertiesBase));
        }
    }

    public interface IModelColumnProperties : IModelNode {
        IModelColumnPropertiesEdit PropertiesEdit { get; }
    }

    public interface IModelColumnPropertiesEdit : IModelNode {

    }

}
