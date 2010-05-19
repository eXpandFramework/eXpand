using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.ModelDifference.Win{
    public sealed class ModelDifferenceWindowsFormsModule : ModelDifferenceBaseModule<XpoWinModelDictionaryDifferenceStore>
    {
        public ModelDifferenceWindowsFormsModule()
        {
            this.RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
            this.RequiredModuleTypes.Add(typeof(SystemWindowsFormsModule));
        }

        private bool? _persistentApplicationModelUpdated=false;

        protected override bool? PersistentApplicationModelUpdated{
            get { return _persistentApplicationModelUpdated; }
            set { _persistentApplicationModelUpdated = value; }
        }
    }
}