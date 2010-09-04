using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.FilterDataStore.Model {
    [KeyProperty("Name")]
    [DisplayProperty("Name")]
    public interface IModelDisabledDataStoreFilter:IModelNode
    {
        [DataSourceProperty("DataStoreFilters")]
        [Required]
        string Name { get; set; }
        [Browsable(false)]
        IEnumerable<string> DataStoreFilters { get; }
    }
}