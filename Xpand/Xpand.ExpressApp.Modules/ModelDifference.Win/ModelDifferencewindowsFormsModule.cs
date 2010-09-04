using System.IO;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.ModelDifference.Win{
    public sealed class ModelDifferenceWindowsFormsModule : ModelDifferenceBaseModule
    {
        public ModelDifferenceWindowsFormsModule()
        {
            RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
            RequiredModuleTypes.Add(typeof(SystemWindowsFormsModule));
        }
        public static ModelApplicationCreator ApplicationCreator { get; set; }
        private bool? _persistentApplicationModelUpdated=false;

        public override string GetPath() {
            return Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
        }

        protected override bool? PersistentApplicationModelUpdated{
            get { return _persistentApplicationModelUpdated; }
            set { _persistentApplicationModelUpdated = value; }
        }
    }
}