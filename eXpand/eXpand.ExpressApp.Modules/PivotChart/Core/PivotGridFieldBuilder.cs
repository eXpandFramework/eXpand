using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.XtraPivotGrid;

namespace eXpand.ExpressApp.PivotChart.Core {
    public class PivotGridFieldBuilder : DevExpress.ExpressApp.PivotChart.PivotGridFieldBuilder {
        IModelApplication _modelApplication;
        IModelMember _propertyModel;
        public event EventHandler<SetupGridFieldArgs> SetupGridField;
        protected virtual void OnSetupGridField(SetupGridFieldArgs e)
        {
            EventHandler<SetupGridFieldArgs> handler = SetupGridField;
            if (handler != null) handler(this, e);
        }
        public PivotGridFieldBuilder(IAnalysisControl analysisControl)
            : base(analysisControl)
        {
        }
        public new void SetModel(IModelApplication modelApplication)
        {
            _modelApplication = modelApplication;
            base.SetModel(modelApplication);
        }

        public override void SetupPivotGridField(IMemberInfo memberInfo)
        {
            _propertyModel = GetPropertyModel(memberInfo);
            base.SetupPivotGridField(memberInfo);
            
        }
        IModelMember GetPropertyModel(IMemberInfo memberInfo) {
            IModelMember result = null;
            if (_modelApplication != null) {
                IModelClass modelClass = _modelApplication.BOModel.GetClass(memberInfo.Owner.Type);
                if (modelClass != null) {
                    result = modelClass.FindOwnMember(memberInfo.Name);
                }
            }
            return result;
        }
        private PivotGridFieldBase FindPivotGridField(string bindingPropertyName)
        {
            return Owner.Fields[bindingPropertyName];
        }

        public override void ApplySettings()
        {
            try{
                Owner.BeginUpdate();
                IAnalysisInfo analysisInfo = GetAnalysisInfo();
                if (analysisInfo != null){
                    ITypeInfo objectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(analysisInfo.DataType);
                    foreach (string propertyName in analysisInfo.DimensionProperties){
                        IMemberInfo memberInfo = objectTypeInfo.FindMember(propertyName);
                        _propertyModel = GetPropertyModel(memberInfo);
                        if (memberInfo != null){
                            PivotGridFieldBase field = FindPivotGridField(GetBindingName(memberInfo));
                            if (field != null){
                                SetupPivotGridField(field, memberInfo.MemberType, GetMemberDisplayFormat(memberInfo));
                                field.Caption = CaptionHelper.GetFullMemberCaption(objectTypeInfo, propertyName);
                            }
                        }
                    }
                }
            }
            finally{
                Owner.EndUpdate();
            }

        }
        private string GetMemberDisplayFormat(IMemberInfo memberInfo)
        {
            string result = "";
            IModelMember modelMember = GetPropertyModel(memberInfo);
            if (modelMember != null){
                result = modelMember.DisplayFormat;
            }
            else
            {
                CustomAttribute displayFormatAttribute = memberInfo.FindAttributes<CustomAttribute>().FirstOrDefault(attribute => attribute.Name == "DisplayFormat");
                if (displayFormatAttribute != null){
                    result = displayFormatAttribute.Value;
                }
            }
            return result;
        }

        protected override void SetupPivotGridField(PivotGridFieldBase field, Type memberType, string displayFormat) {
            
            if (memberType == typeof(DateTime)){
                field.GroupInterval = ((IModelMemberAnalysisDisplayDateTime)_propertyModel).PivotGroupInterval;
            }
            else
                base.SetupPivotGridField(field, memberType, displayFormat);
        }
    }
}