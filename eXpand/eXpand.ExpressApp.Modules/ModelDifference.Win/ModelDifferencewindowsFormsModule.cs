using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors;

namespace eXpand.ExpressApp.ModelDifference.Win{
    public sealed partial class ModelDifferenceWindowsFormsModule : ModelDifferenceBaseModule<XpoWinModelDictionaryDifferenceStore>
    {
        public ModelDifferenceWindowsFormsModule()
        {
            InitializeComponent();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            typesInfo.FindTypeInfo(typeof(ModelDifferenceObject)).FindMember("PreferredAspect").AddAttribute(new CustomAttribute(PropertyInfoNodeWrapper.PropertyEditorTypeAttribute, typeof(StringDisableTextEditorPropertyEditor).FullName));
        }


        private bool? _persistentApplicationModelUpdated=false;
        protected override bool? PersistentApplicationModelUpdated{
            get { return _persistentApplicationModelUpdated; }
            set { _persistentApplicationModelUpdated = value; }
        }
    }

}