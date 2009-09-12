using System.ComponentModel;
using eXpand.ExpressApp.ModelDifference.Win;

namespace eXpand.ExpressApp.ModelDifference.Web
{
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class ModelDifferenceAspNetModule : ModelDifferenceBaseModule<XpoWebModelDictionaryDifferenceStore>
    {
        public ModelDifferenceAspNetModule()
        {
            InitializeComponent();
        }



    }
}