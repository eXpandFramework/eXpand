using System;
using System.ComponentModel;
using System.Web;

namespace eXpand.ExpressApp.ModelDifference.Web
{
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class ModelDifferenceAspNetModule : ModelDifferenceBaseModule<XpoWebModelDictionaryDifferenceStore>
    {
        private bool? persistentApplicationModelUpdated;

        public ModelDifferenceAspNetModule()
        {
            InitializeComponent();
        }


        protected override bool? PersistentApplicationModelUpdated{
            get{
                bool result;
                bool.TryParse(HttpContext.Current.Application["persistentApplicationModelUpdated"] + "", out result);
                persistentApplicationModelUpdated = result;
                return persistentApplicationModelUpdated;
            }
            set { HttpContext.Current.Application["persistentApplicationModelUpdated"] = value; }
        }
    }
}