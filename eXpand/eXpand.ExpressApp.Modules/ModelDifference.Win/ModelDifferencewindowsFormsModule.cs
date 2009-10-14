using System;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.Win.PropertyEditors.StringPropertyEditor;

namespace eXpand.ExpressApp.ModelDifference.Win{
    public sealed partial class ModelDifferenceWindowsFormsModule : ModelDifferenceBaseModule<XpoWinModelDictionaryDifferenceStore>
    {
        public ModelDifferenceWindowsFormsModule()
        {
            InitializeComponent();
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            setPrefferedAspectPropertyEditor(typesInfo, typeof(ModelDifferenceObject));
            setPrefferedAspectPropertyEditor(typesInfo, typeof(RoleModelDifferenceObject));
            setPrefferedAspectPropertyEditor(typesInfo, typeof(UserModelDifferenceObject));
        }

        private void setPrefferedAspectPropertyEditor(ITypesInfo typesInfo, Type typeName) {
            var typeInfo = typesInfo.FindTypeInfo(typeName);
            typeInfo.FindMember("PreferredAspect").AddAttribute(new CustomAttribute(PropertyInfoNodeWrapper.PropertyEditorTypeAttribute,typeof(StringDisableTextEditorPropertyEditor).FullName));
        }

        private bool? _persistentApplicationModelUpdated=false;
        protected override bool? PersistentApplicationModelUpdated{
            get { return _persistentApplicationModelUpdated; }
            set { _persistentApplicationModelUpdated = value; }
        }
    }

}