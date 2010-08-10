using System.ComponentModel;

namespace eXpand.Persistent.Base.ModelDifference {
    public interface IModelOptionsLoadModelResources {
        [Category("eXpand.ModelDifference")]
        [Description("For all resources that end with .xafml a new model application difference will be created and compined with the master application model")]
        [DefaultValue("MDO_")]
        string ModelApplicationPrefix { get; set; }    
    }
}