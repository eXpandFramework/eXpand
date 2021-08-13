using System;
using System.Drawing;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.Base.AdditionalViewControls;
using Xpand.Persistent.BaseImpl.Security.PermissionPolicyData;
using FontStyle = Xpand.Persistent.Base.AdditionalViewControls.FontStyle;

namespace Xpand.Persistent.BaseImpl.AdditionalViewControls {
    [System.ComponentModel.DisplayName("AdditionalViewControls")]
    public class AdditionalViewControlsOperationPermissionPolicyData : LogicRuleOperationPermissionPolicyData, IContextAdditionalViewControlsRule {

        public AdditionalViewControlsOperationPermissionPolicyData(Session session)
            : base(session) {
        }

        protected override Type GetPermissionType(){
            return typeof(IContextAdditionalViewControlsRule);
        }

        #region IAdditionalViewControlsRule Members

        public string Message { get; set; }
        public string MessageProperty { get; set; }
        public Position Position { get; set; }
        public Color? BackColor { get; set; }
        public Color? ForeColor { get; set; }
        public FontStyle? FontStyle { get; set; }
        public int? Height { get; set; }
        public int? FontSize { get; set; }
        public string ImageName { get; set; }
        #endregion
        [RuleRequiredField]
        [ValueConverter(typeof(TypeValueConverter))]
        public Type ControlType { get; set; }
        [RuleRequiredField]
        [ValueConverter(typeof(TypeValueConverter))]
        public Type DecoratorType { get; set; }
    }
}