using System;

namespace eXpand.ExpressApp.ModelDifference.Win{
    public sealed partial class ModelDifferenceWindowsFormsModule : ModelDifferenceBaseModule<XpoWinModelDictionaryDifferenceStore>
    {
        public ModelDifferenceWindowsFormsModule()
        {
            InitializeComponent();
        }


        private bool? _persistentApplicationModelUpdated=false;
        protected override bool? PersistentApplicationModelUpdated{
            get { return _persistentApplicationModelUpdated; }
            set { _persistentApplicationModelUpdated = value; }
        }
    }
}